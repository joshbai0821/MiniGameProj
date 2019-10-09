using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

namespace MiniProj
{
    public class ResourceManager : MonoBehaviour
    {
        private StringBuilder m_strBuilder;
        private List<BundleItem> m_bundleList;
        private List<Asset> m_assetList;
        private Dictionary<string, List<String>> m_manifest;
        private int m_count;
        private static int CountInterval = 20;
        public static string AssetBundlePath;

        public ResourceManager()
        {
            m_manifest = null;
            m_strBuilder = new StringBuilder();
            m_bundleList = new List<BundleItem>(8);
            m_assetList = new List<Asset>(8);
            m_count = 0;
        }

        private void Awake()
        {
            AssetBundlePath = System.IO.Path.Combine(Application.streamingAssetsPath, "AssetBundles");
        }

        public UnityEngine.Object LoadPrefabSync(string path, string name, Type type, string abName = null)
        {
            UnityEngine.Object _obj = null;
            var _assets = LoadAssetSync(path, name, type, abName);
            //Debug.Log(name);
            if (_assets != null)
            {
                _obj = UnityEngine.Object.Instantiate(_assets.m_asset);
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

            for (int i = 0, max = m_assetList.Count; i < max; i++)
            {
                var item = m_assetList[i];
                if (!item.Id.Equals(path+name))
                    continue;
                item.Retain();
                return item;
            }
#if UNITY_EDITOR
            Asset asset = new Asset(path, name, type);
            m_assetList.Add(asset);
            asset.Load();
            asset.Retain();
            return asset;
#else
            Asset asset = new BundleAsset(path, name, type);
            m_assetList.Add(asset);
            asset.Load();
            asset.Retain();
            return asset;
#endif
        }

        protected bool LoadAssetBundleManifest()
        {
            bool _ret = true;
            string _filePath = System.IO.Path.Combine(AssetBundlePath, "AssetBundles");

            var _bundle = AssetBundle.LoadFromFile(_filePath);
            if (_bundle == null)
            {
                Debug.Log("Load asset bundle manifest failed");
                _ret = false;
                return _ret;
            }

            // 加载AssetBundleManifest
            AssetBundleManifest _manifest = _bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

            m_manifest = new Dictionary<string, List<string>>(8);
            foreach (string _abName in _manifest.GetAllAssetBundles())
            {
                string[] _dependencies = _manifest.GetAllDependencies(_abName);
                List<string> _dpList = new List<string>(_dependencies);
                m_manifest.Add(_abName, _dpList);
            }

            _bundle.Unload(true);
            _bundle = null;
            return _ret;
        }

        public BundleItem LoadAssetBundleSync(string abName)
        {
            if (m_manifest == null)
            {
                // 加载 manifest 先
                if (!LoadAssetBundleManifest())
                {
                    return null;
                }
            }

            for (int _i = 0, _max = m_bundleList.Count; _i < _max; _i++)
            {
                var _item = m_bundleList[_i];
                if(!_item.Name.Equals(abName))
                {
                    continue;
                }
                _item.Retain();
                return _item;
            }

            BundleItem _bundleItem = new BundleItem(abName);
            m_bundleList.Add(_bundleItem);
            _bundleItem.Load();
            LoadDependencies(abName);
            _bundleItem.Retain();
            return _bundleItem;

        }

        protected bool LoadDependencies(string bundleName)
        {
            bool _ret = true;
            if (m_manifest != null)
            {
                List<string> _dpList;
                if (m_manifest.TryGetValue(bundleName, out _dpList))
                {
                    foreach (string _abName in _dpList)
                    {
                        bool _find = false;
                        for (int _i = 0, _max = m_bundleList.Count; _i < _max; _i++)
                        {
                            var _item = m_bundleList[_i];
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
                            m_bundleList.Add(_bundleItem);
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
            for (var i = 0; i < bundle.m_dependencies.Count; i++)
            {
                var item = bundle.m_dependencies[i];
                item.Release();
            }

            bundle.m_dependencies.Clear();
        }

        private void Update()
        {
            ++m_count;
            if(m_count >= CountInterval)
            {
                for (var _i = 0; _i < m_assetList.Count; _i++)
                {
                    var _item = m_assetList[_i];
                    _item.Update();
                    if (!_item.IsUnused())
                        continue;
                    _item.Unload();
                    m_assetList.RemoveAt(_i);
                    _i--;
                }

                for (var _j = 0; _j < m_bundleList.Count; _j++)
                {
                    var _item = m_bundleList[_j];
                    if (!_item.IsUnused())
                        continue;
                    UnloadDependencies(_item);
                    _item.Unload();
                    m_bundleList.RemoveAt(_j);
                    _j--;
                }
                m_count = 0;
            }
            
        }
    }

}
