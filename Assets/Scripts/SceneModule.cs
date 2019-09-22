using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniProj
{
    public class Module : MonoBehaviour
    {
        protected float m_timeStamp;
        protected string m_name;
        public virtual string Name
        {
            get { return m_name; }
        }

        public float TimeStamp
        {
            get { return m_timeStamp; }
        }

        public void UpdateFreeTime(float delta)
        {
            m_timeStamp += delta;
        }

        public Module(string name = "Module")
        {
            m_timeStamp = 0.0f;
            m_name = name;
        }
    }

    public class SceneModule : Module
    {
        private GameObject m_mapRoot;

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

        private List<List<int>> m_mapData;

        private void Awake()
        {
            
            m_mapRoot = new GameObject("MapRoot");
            LoadMap("map1");
        }

        public void LoadMap(string name)
        {
            ClearMapData();
            TextAsset _textAsset = Resources.Load<TextAsset>(name);
            string[] _rowString = _textAsset.text.Trim().Split('\n');
            DealSkillData(_rowString[0]);
            DealMapData(ref _rowString);

            
        }

        private void DealSkillData(string skillStr)
        {
            string[] _str = skillStr.Split(',');
            for(int _i = 0; _i < _str.Length; ++_i)
            {
                int _skillId = -1;
                if (int.TryParse(_str[_i], out _skillId))
                {

                }
            }

            return;
        }

        private void DealMapData(ref string[] rowString)
        {
            float _minX = -((rowString.Length - 1) / 2.0f) * MapPrefabSizeX;
            m_mapData = new List<List<int>>(rowString.Length);
            for (int _i = 1; _i < rowString.Length; ++_i)
            {

                string[] _str = rowString[_i].Split(',');
                float _minZ = -(_str.Length / 2.0f) * MapPrefabSizeZ;
                List<int> _dataList = new List<int>(_str.Length);
                for (int _j = 0; _j < _str.Length; ++_j)
                {
                    int _mapPrefabId = -1;
                    if (int.TryParse(_str[_j], out _mapPrefabId) && _mapPrefabId >= -1 && _mapPrefabId < PrefabName.Length)
                    {
                        if (_mapPrefabId != -1)
                        {
                            GameObject obj = (GameObject)GameManager.ResManager.LoadPrefabSync(MapPrefabPath, PrefabName[_mapPrefabId], typeof(GameObject));
                            obj.transform.SetParent(m_mapRoot.transform, false);
                            obj.transform.position = new Vector3(_minX + _i * MapPrefabSizeX, obj.transform.position.y, _minZ + _j * MapPrefabSizeZ);
                            _dataList.Add(obj.GetComponent<MapData>().Data);
                        }
                        _dataList.Add(-1);
                    }
                    else
                    {

                        Debug.Log(string.Format("SceneModule, DealMapData Error In row {0} col {1}", _i, _j));
                    }

                }
                m_mapData.Add(_dataList);
            }
        }

        private void OnEnable()
        {
            //Debug.Log("SceneModule | OnEnable");
            if(m_mapRoot != null)
                m_mapRoot.SetActive(true);
        }

        private void OnDisable()
        {
            //Debug.Log("SceneModule | Disable");
            if (m_mapRoot != null)
                m_mapRoot.SetActive(false);
        }

        private void OnDestroy()
        {
            //Debug.Log("SceneModule | OnDestroy");
            if (m_mapRoot != null)
                GameObject.Destroy(m_mapRoot);
        }

        public void ClearMap()
        {
            if(m_mapRoot != null)
            {
                for (int _i = 0; _i < m_mapRoot.transform.childCount; _i++)
                {
                    GameObject.Destroy(m_mapRoot.transform.GetChild(_i).gameObject);
                }
            }
            else
            {
                Debug.Log("SceneModule | m_mapRoot is Null");
            }
            ClearMapData();
        }

        private void ClearMapData()
        {
            if(m_mapData != null)
            {
                for (int _i = 0, _max = m_mapData.Count; _i < _max; _i++)
                {
                    m_mapData[_i].Clear();
                }
                m_mapData.Clear();
            }
            m_mapData = null;
        }

    }
}

