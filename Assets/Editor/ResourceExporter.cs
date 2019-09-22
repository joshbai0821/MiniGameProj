using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ResourceExporter {
    protected readonly string m_exportPath;

    [MenuItem("Assets/Export AssetBundle")]
    public static void ExportAssetBundleResource()
    {
        ResourceExporter _exporter = new ResourceExporter();
        _exporter.ExportAb();
        _exporter = null;
        System.GC.Collect();
    }

    public ResourceExporter()
    {
        m_exportPath = Application.streamingAssetsPath + "/AssetBundles";
    }

    public bool ExportAb()
    {
        bool _ret = true;
        string _path = Application.dataPath + "/prefabs";
        Dictionary<string, List<string>> _abResDic = new Dictionary<string, List<string>>(8);

        CollectBuildMap(_path, ref _abResDic);

        int _abCount = _abResDic.Count;
        AssetBundleBuild[] _buildMap = new AssetBundleBuild[_abCount];

        int _count = 0;
        foreach (var _item in _abResDic)
        {
            string[] _assets = new string[_item.Value.Count];
            int _j = 0;
            for (_j = 0; _j < _item.Value.Count; _j++)
            {
                _assets[_j] = _item.Value[_j];
            }
            _buildMap[_count].assetBundleName = _item.Key;
            _buildMap[_count].assetNames = _assets;
        }

        if(!Directory.Exists(m_exportPath))
        {
            Directory.CreateDirectory(m_exportPath);
        }

        BuildTarget target = EditorUserBuildSettings.activeBuildTarget;
        BuildAssetBundleOptions options = BuildAssetBundleOptions.None;
        BuildPipeline.BuildAssetBundles(m_exportPath, _buildMap, options, target);
        if (_ret)
        {
#if UNITY_EDITOR_WIN
            EditorUtility.DisplayDialog("导出资源", "导出资源成功 ！！！", "确定");
#endif
        }
        else
        {
#if UNITY_EDITOR_WIN
            EditorUtility.DisplayDialog("导出资源", "导出资源失败 ！！！", "确定");
#endif
        }

        return _ret;
    }

    protected bool CollectBuildMap(string path, ref Dictionary<string, List<string>> abResDic)
    {
        bool _ret = true;
        foreach (string _dir in Directory.GetFileSystemEntries(path))
        {
            if (File.Exists(_dir))
            {
                if(!(_dir.EndsWith(".meta")) && !(_dir.EndsWith(".cs"))
                    && !_dir.EndsWith(".mm") && !_dir.EndsWith(".m")
                        && !_dir.EndsWith(".h") && !_dir.EndsWith(".DS_Store"))
                {
                    string _relativePath = path.Substring(Application.dataPath.Length);
                    string _abName = _relativePath.Replace('/', '_');
                    List<string> _assetsList;
                    if(abResDic.TryGetValue(_abName, out _assetsList))
                    {
                        _assetsList.Add(_relativePath + _dir);
                    }
                    else
                    {
                        _assetsList = new List<string>(8);
                        abResDic.Add(_abName, _assetsList);
                    }
                }
            }
            else
            {
                //文件夹
                if(!_dir.Contains(".svn"))
                {
                    _ret = _ret && CollectBuildMap(path + _dir, ref abResDic);
                }
            }
        }

        return _ret;
    }
}
