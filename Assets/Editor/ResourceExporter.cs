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
        ResourceExporter exporter = new ResourceExporter();
        exporter.ExportAb();
        exporter = null;
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
        Dictionary<string, List<string>> abResDic = new Dictionary<string, List<string>>(8);

        CollectBuildMap(_path, ref abResDic);

        int _abCount = abResDic.Count;
        AssetBundleBuild[] _buildMap = new AssetBundleBuild[_abCount];

        int count = 0;
        foreach (var item in abResDic)
        {
            string[] assets = new string[item.Value.Count];
            int _j = 0;
            for (_j = 0; _j < item.Value.Count; _j++)
            {
                assets[_j] = item.Value[_j];
            }
            _buildMap[count].assetBundleName = item.Key;
            _buildMap[count].assetNames = assets;
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
        foreach (string dir in Directory.GetFileSystemEntries(path))
        {
            if (File.Exists(dir))
            {
                if(!(dir.EndsWith(".meta")) && !(dir.EndsWith(".cs"))
                    && !dir.EndsWith(".mm") && !dir.EndsWith(".m")
                        && !dir.EndsWith(".h") && !dir.EndsWith(".DS_Store"))
                {
                    string _relativePath = path.Substring(Application.dataPath.Length);
                    string _abName = _relativePath.Replace('/', '_');
                    List<string> _assetsList;
                    if(abResDic.TryGetValue(_abName, out _assetsList))
                    {
                        _assetsList.Add(_relativePath + dir);
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
                if(!dir.Contains(".svn"))
                {
                    _ret = _ret && CollectBuildMap(path + dir, ref abResDic);
                }
            }
        }

        return _ret;
    }
}
