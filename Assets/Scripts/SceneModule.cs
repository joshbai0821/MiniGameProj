using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniProj
{
    public class Module : MonoBehaviour
    {
        protected float m_TimeStamp;
        protected string m_Name;
        public virtual string Name
        {
            get { return m_Name; }
        }

        public float TimeStamp
        {
            get { return m_TimeStamp; }
        }

        public void UpdateFreeTime(float delta)
        {
            m_TimeStamp += delta;
        }

        public Module(string name = "Module")
        {
            m_TimeStamp = 0.0f;
            m_Name = name;
        }
    }

    public class SceneModule : Module
    {
        private GameObject m_MapRoot;

        public SceneModule():base("SceneModule")
        {
        }

        private static string[] PrefabName =
        {
            "BlueCube.Prefab",
            "GreenCube.Prefab",
            "RedCube.Prefab",
        };

        private static float MapPrefabSizeX = 1;
        private static float MapPrefabSizeZ = 1;

        private static string MapPrefabPath = "Prefabs/Map";

        private List<List<int>> m_MapData;

        private void Awake()
        {
            
            m_MapRoot = new GameObject("MapRoot");
            LoadMap("map1");
        }

        public void LoadMap(string name)
        {
            ClearMapData();
            TextAsset _textAsset = Resources.Load<TextAsset>(name);
            string[] _rowString = _textAsset.text.Trim().Split('\n');
            float _minX = -(_rowString.Length / 2.0f) * MapPrefabSizeX;
            m_MapData = new List<List<int>>(_rowString.Length);
            for (int _i = 0; _i < _rowString.Length; ++_i)
            {
                
                string[] _str = _rowString[_i].Split(',');
                float _minZ = -(_str.Length / 2.0f) * MapPrefabSizeZ;
                List<int> _dataList = new List<int>(_str.Length);
                for (int _j = 0; _j < _str.Length; ++_j)
                {
                    int _mapPrefabId = int.Parse(_str[_j]);
                    GameObject obj = (GameObject)GameManager.ResManager.LoadPrefabSync(MapPrefabPath, PrefabName[_mapPrefabId], typeof(GameObject));
                    obj.transform.SetParent(m_MapRoot.transform, false);
                    obj.transform.position = new Vector3(_minX + _i * MapPrefabSizeX, obj.transform.position.y, _minZ + _j * MapPrefabSizeZ);
                    _dataList.Add(obj.GetComponent<MapData>().Data);
                }
                m_MapData.Add(_dataList);
            }
        }

        private void OnEnable()
        {
            //Debug.Log("SceneModule | OnEnable");
            if(m_MapRoot != null)
                m_MapRoot.SetActive(true);
        }

        private void OnDisable()
        {
            //Debug.Log("SceneModule | Disable");
            if (m_MapRoot != null)
                m_MapRoot.SetActive(false);
        }

        private void OnDestroy()
        {
            //Debug.Log("SceneModule | OnDestroy");
            if (m_MapRoot != null)
                GameObject.Destroy(m_MapRoot);
        }

        public void ClearMap()
        {
            if(m_MapRoot != null)
            {
                for (int _i = 0; _i < m_MapRoot.transform.childCount; _i++)
                {
                    GameObject.Destroy(m_MapRoot.transform.GetChild(_i).gameObject);
                }
            }
            else
            {
                Debug.Log("SceneModule | m_MapRoot is Null");
            }
            ClearMapData();
        }

        private void ClearMapData()
        {
            if(m_MapData != null)
            {
                for (int _i = 0, _max = m_MapData.Count; _i < _max; _i++)
                {
                    m_MapData[_i].Clear();
                }
                m_MapData.Clear();
            }
            m_MapData = null;
        }

    }
}

