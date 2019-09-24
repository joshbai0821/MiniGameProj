using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MiniProj
{
    public class SceneDataExporter : MonoBehaviour
    {
        private static string ConfigPath = "Assets/Resources/SceneConfig.asset";
        [MenuItem("Assets/Create SceneConfig")]
        static void CreateSceneConfig()
        {
            // 将对象实例化
            SceneConfig _sceneConfig = ScriptableObject.CreateInstance<SceneConfig>();

            //创建类为可编辑的配置文件
            AssetDatabase.CreateAsset(_sceneConfig, ConfigPath);
            AssetDatabase.SaveAssets();
        }
    }
}

