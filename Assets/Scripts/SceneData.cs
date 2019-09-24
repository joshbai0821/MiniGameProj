using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniProj
{
    public class SceneData : MonoBehaviour
    {
        //private GameObject m_mapRoot;

        public Transform m_mapRoot;
        [SerializeField]
        private int m_row;
        [SerializeField]
        private int m_col;

        //private static float MapPrefabSizeX = 1;
        //private static float MapPrefabSizeZ = 1;

        //private static string MapPrefabPath = "Prefabs/Map";

        private List<List<MapDataType>> m_mapData;

        public List<List<MapDataType>> Data
        {
            get { return m_mapData; }
        }

        public int Row
        {
            get { return m_row; }
        }

        public int Col
        {
            get { return m_col; }
        }

        private void Awake()
        {

            //m_mapRoot = new GameObject("MapRoot");
            //LoadMap("map1");
            LoadMap();
        }

        private void LoadMap()
        {
            m_mapData = new List<List<MapDataType>>(m_row);
            for (int _i = 0; _i < m_row; ++_i)
            {
                List<MapDataType> _list = new List<MapDataType>(m_col);
                for (int _j = 0; _j < m_col; ++_j)
                {
                    _list.Add(MapDataType.NONE);
                }
                m_mapData.Add(_list);
            }

            for(int _i = 0; _i < m_mapRoot.childCount; ++_i)
            {
                Transform _child = m_mapRoot.GetChild(_i);
                MapData _mapData = _child.GetComponent<MapData>();
                int _row = _mapData.Pos.m_row;
                int _col = _mapData.Pos.m_col;
                m_mapData[_row][_col] = _mapData.Data;
            }
        }

        //public void LoadMap(string name)
        //{
        //    ClearMapData();
        //    TextAsset _textAsset = Resources.Load<TextAsset>(name);
        //    string[] _rowString = _textAsset.text.Trim().Split('\n');
        //    DealSkillData(_rowString[0]);
        //    DealMapData(ref _rowString);

            
        //}

        //private void DealSkillData(string skillStr)
        //{
        //    Transform _skillPanel = GameManager.UIRoot.Find("SkillPanel");
        //    _skillPanel.gameObject.SetActive(true);
        //    string[] _str = skillStr.Split(',');
        //    for(int _i = 0; _i < _str.Length; _i += 2)
        //    {
        //        int _skillId = -1;
        //        int _skillCount = 0;
        //        if (int.TryParse(_str[_i], out _skillId) && int.TryParse(_str[_i + 1], out _skillCount))
        //        {
        //            GameObject obj = (GameObject)GameManager.ResManager.LoadPrefabSync(MapPrefabPath, "Skill", typeof(GameObject));
        //            obj.transform.SetParent(_skillPanel);
        //            obj.GetComponent<Skill>().InitialSkill(_skillId, _skillCount);
        //        }
        //        else
        //        {
        //            Debug.Log(string.Format("SceneModule, DealSkillData Error In row 0 col {0}", _i));
        //        }
        //    }

        //    return;
        //}

        //private void DealMapData(ref string[] rowString)
        //{
        //    float _minX = -((rowString.Length - 1) / 2.0f) * MapPrefabSizeX;
        //    m_mapData = new List<List<int>>(rowString.Length);
        //    for (int _i = 1; _i < rowString.Length; ++_i)
        //    {

        //        string[] _str = rowString[_i].Split(',');
        //        float _minZ = -(_str.Length / 2.0f) * MapPrefabSizeZ;
        //        List<int> _dataList = new List<int>(_str.Length);
        //        for (int _j = 0; _j < _str.Length; ++_j)
        //        {
        //            int _mapPrefabId = -1;
        //            if (int.TryParse(_str[_j], out _mapPrefabId) && _mapPrefabId >= -1 && _mapPrefabId < PrefabName.Length)
        //            {
        //                if (_mapPrefabId != -1)
        //                {
        //                    GameObject obj = (GameObject)GameManager.ResManager.LoadPrefabSync(MapPrefabPath, PrefabName[_mapPrefabId], typeof(GameObject));
        //                    obj.transform.SetParent(m_mapRoot.transform, false);
        //                    obj.transform.position = new Vector3(_minX + _i * MapPrefabSizeX, obj.transform.position.y, _minZ + _j * MapPrefabSizeZ);
        //                    _dataList.Add(obj.GetComponent<MapData>().Data);
        //                }
        //                _dataList.Add(-1);
        //            }
        //            else
        //            {

        //                Debug.Log(string.Format("SceneModule, DealMapData Error In row {0} col {1}", _i, _j));
        //            }

        //        }
        //        m_mapData.Add(_dataList);
        //    }
        //}

        //private void OnEnable()
        //{
        //    //Debug.Log("SceneModule | OnEnable");
        //    if(m_mapRoot != null)
        //        m_mapRoot.SetActive(true);
        //}

        //private void OnDisable()
        //{
        //    //Debug.Log("SceneModule | Disable");
        //    if (m_mapRoot != null)
        //        m_mapRoot.SetActive(false);
        //}

        //private void OnDestroy()
        //{
        //    //Debug.Log("SceneModule | OnDestroy");
        //    if (m_mapRoot != null)
        //        GameObject.Destroy(m_mapRoot);
        //}

        //public void ClearMap()
        //{
        //    if(m_mapRoot != null)
        //    {
        //        for (int _i = 0; _i < m_mapRoot.transform.childCount; _i++)
        //        {
        //            GameObject.Destroy(m_mapRoot.transform.GetChild(_i).gameObject);
        //        }
        //    }
        //    else
        //    {
        //        Debug.Log("SceneModule | m_mapRoot is Null");
        //    }
        //    ClearMapData();
        //}

        //private void ClearMapData()
        //{
        //    if(m_mapData != null)
        //    {
        //        for (int _i = 0, _max = m_mapData.Count; _i < _max; _i++)
        //        {
        //            m_mapData[_i].Clear();
        //        }
        //        m_mapData.Clear();
        //    }
        //    m_mapData = null;
        //}

    }
}

