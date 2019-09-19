using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

namespace MiniProj
{
    public class ResourceManager : MonoBehaviour
    {
        private StringBuilder mStrBuilder;
        private List<BundleItem> m_BundleList;
        private List<Asset> m_AssetList;
        private Dictionary<string, List<String>> m_Manifest;
        private int count;
        private static int CountInterval = 20;
        public static string AssetBundlePath;

        public ResourceManager()
        {
            mStrBuilder = new StringBuilder();
            m_Manifest = new Dictionary<string, List<string>>(8);
            m_BundleList = new List<BundleItem>(8);
            m_AssetList = new List<Asset>(8);
            count = 0;
        }

        private void Awake()
        {
            AssetBundlePath = Application.streamingAssetsPath + "/AssetBundles";
        }

        public UnityEngine.Object LoadPrefabSync(string path, string name, Type type, string abName = null)
        {
            UnityEngine.Object _obj = null;
            var _assets = LoadAssetSync(path, name, type, abName);
            if (_assets != null)
            {
                _obj = UnityEngine.Object.Instantiate(_assets.m_Asset);
                _assets.Require(_obj);
                
                return _obj;
            }
            return _obj;
        }

        protected Asset LoadAssetSync(string path, string name, Type type, string abName = null)
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("invalid path");
                return null;
            }

            for (int i = 0, max = m_AssetList.Count; i < max; i++)
            {
                var item = m_AssetList[i];
                if (!item.Id.Equals(path+name))
                    continue;
                item.Retain();
                return item;
            }
#if UNITY_EDITOR
            Asset asset = new Asset(path, name, type);
            m_AssetList.Add(asset);
            asset.Load();
            asset.Retain();
            return asset;
#else
            Asset asset = new BundleAsset(path, name, type);
            m_AssetList.Add(asset);
            asset.Load();
            asset.Retain();
            return asset;
#endif
        }

        protected bool LoadAssetBundleManifest()
        {
            bool _ret = true;
            mStrBuilder.Length = 0;
            mStrBuilder.Append(AssetBundlePath);
            mStrBuilder.Append("/AssetBundles");

            var _bundle = AssetBundle.LoadFromFile(mStrBuilder.ToString());
            if (_bundle == null)
            {
                Debug.Log("Load asset bundle manifest failed");
                _ret = false;
                return _ret;
            }

            // 加载AssetBundleManifest
            AssetBundleManifest _manifest = _bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

            foreach (string _abName in _manifest.GetAllAssetBundles())
            {
                string[] _dependencies = _manifest.GetAllDependencies(_abName);
                List<string> _dpList = new List<string>(_dependencies);
                m_Manifest.Add(_abName, _dpList);
            }

            _bundle.Unload(true);
            _bundle = null;
            return _ret;
        }

        public BundleItem LoadAssetBundleSync(string abName)
        {
            if (m_Manifest == null)
            {
                // 加载 manifest 先
                if (!LoadAssetBundleManifest())
                {
                    return null;
                }
            }

            for (int _i = 0, _max = m_BundleList.Count; _i < _max; _i++)
            {
                var _item = m_BundleList[_i];
                if(!_item.Name.Equals(abName))
                {
                    continue;
                }
                _item.Retain();
                return _item;
            }

            BundleItem _bundleItem = new BundleItem(abName);
            m_BundleList.Add(_bundleItem);
            _bundleItem.Load();
            LoadDependencies(abName);
            _bundleItem.Retain();
            return _bundleItem;

        }

        protected bool LoadDependencies(string bundleName)
        {
            bool _ret = true;
            if (m_Manifest != null)
            {
                List<string> _dpList;
                if (m_Manifest.TryGetValue(bundleName, out _dpList))
                {
                    foreach (string _abName in _dpList)
                    {
                        bool _find = false;
                        for (int _i = 0, _max = m_BundleList.Count; _i < _max; _i++)
                        {
                            var _item = m_BundleList[_i];
                            if (!_item.Name.Equals(_abName))
                            {
                                continue;
                            }
                            _item.Retain();
                            _find = true;
                            break;
                        }

                        if(!_find)
                        {
                            BundleItem _bundleItem = new BundleItem(_abName);
                            m_BundleList.Add(_bundleItem);
                            _bundleItem.Load();
                            _bundleItem.Retain();
                        }
                    }
                }
                else
                {
                    Debug.Log(string.Format("BundleName {0} is Error", bundleName));
                    _ret = false;
                }
            }
            return _ret;
        }

        protected static void UnloadDependencies(BundleItem bundle)
        {
            for (var i = 0; i < bundle.m_Dependencies.Count; i++)
            {
                var item = bundle.m_Dependencies[i];
                item.Release();
            }

            bundle.m_Dependencies.Clear();
        }

        private void Update()
        {
            ++count;
            if(count >= CountInterval)
            {
                for (var _i = 0; _i < m_AssetList.Count; _i++)
                {
                    var _item = m_AssetList[_i];
                    _item.Update();
                    if (!_item.IsUnused())
                        continue;
                    _item.Unload();
                    m_AssetList.RemoveAt(_i);
                    _i--;
                }

                for (var _j = 0; _j < m_BundleList.Count; _j++)
                {
                    var _item = m_BundleList[_j];
                    if (!_item.IsUnused())
                        continue;
                    UnloadDependencies(_item);
                    _item.Unload();
                    m_BundleList.RemoveAt(_j);
                    _j--;
                }
                count = 0;
            }
            
        }
    }

}
