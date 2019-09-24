using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniProj
{
    public class SceneModule : Module
    {
        private GameObject m_mapRoot;
        private SceneConfig m_config;
        private Player m_player;
        private List<Transform> m_tsfMapList;

        public SceneModule() : base("SceneModule")
        {
        }

        private static string MapPrefabPath = "Prefabs/Map";

        private List<List<MapDataType>> m_mapData;
        public List<List<MapDataType>> Data
        {
            get { return m_mapData; }
        }

        private void Awake()
        {
            m_config = Resources.Load<SceneConfig>("SceneConfig");
            m_mapRoot = new GameObject("MapRoot");
            string _name = m_config.SceneConfigList[GameManager.SceneConfigId].PrefabName;
            GameObject _mapPrefab = (GameObject)GameManager.ResManager.LoadPrefabSync(MapPrefabPath, _name , typeof(GameObject));
            _mapPrefab.transform.SetParent(m_mapRoot.transform, false);
            LoadMap();
            LoadPlayer();
            LoadSkillBtn();
        }

        public bool isPlayerReady()
        {
            return m_player.IsReady();
        }

        private void LoadSkillBtn()
        {
            GameObject _skillPanel = (GameObject)GameManager.ResManager.LoadPrefabSync(MapPrefabPath, "SKillPanel", typeof(GameObject));
            _skillPanel.transform.SetParent(GameManager.GameManagerObj.GetComponent<GameManager>().UILayer, false);
            for(int _i = 0; _i < m_config.SceneConfigList[GameManager.SceneConfigId].SkillData.Count; ++_i)
            {
                GameObject _skillBtn = (GameObject)GameManager.ResManager.LoadPrefabSync(MapPrefabPath, "SkillBtn", typeof(GameObject));
                _skillBtn.transform.SetParent(_skillPanel.transform, false);
                _skillBtn.GetComponent<SkillBtn>().Initial(
                    m_config.SceneConfigList[GameManager.SceneConfigId].SkillData[_i].Id,
                    m_config.SceneConfigList[GameManager.SceneConfigId].SkillData[_i].Count);
            }

        }

        private void LoadPlayer()
        {
            string _name = "Player";
            GameObject _playerPrefab = (GameObject)GameManager.ResManager.LoadPrefabSync(MapPrefabPath, _name, typeof(GameObject));
            m_player = _playerPrefab.GetComponent<Player>();
            if(m_player != null)
            {
                m_player.SetStartPosition(
                m_config.SceneConfigList[GameManager.SceneConfigId].PlayerStartRow,
                m_config.SceneConfigList[GameManager.SceneConfigId].PlayerStartCol);
            }
            else
            {
                Debug.Log("SceneModule | LoadPlayer Error");
            }
            
        }

        private void LoadMap()
        {
            
            ClearMapData();
            Transform _tsfMapDataRoot = m_mapRoot.transform.GetChild(0);
            int _row = m_config.SceneConfigList[GameManager.SceneConfigId].MapRow;
            int _col = m_config.SceneConfigList[GameManager.SceneConfigId].MapCol;
            m_mapData = new List<List<MapDataType>>(_row);
            for (int _i = 0; _i < _row; ++_i)
            {
                List<MapDataType> _list = new List<MapDataType>(_col);
                for (int _j = 0; _j < _col; ++_j)
                {
                    _list.Add(MapDataType.NONE);
                }
                m_mapData.Add(_list);
            }
            m_tsfMapList = new List<Transform>();
            for (int _i = 0; _i < _tsfMapDataRoot.childCount; ++_i)
            {
                Transform _child = _tsfMapDataRoot.GetChild(_i);
                MapData _mapData = _child.GetComponent<MapData>();
                m_tsfMapList.Add(_child);
                if(_mapData != null)
                {
                    int _r = _mapData.Pos.m_row;
                    int _c = _mapData.Pos.m_col;
                    m_mapData[_r][_c] = _mapData.Data;
                }
            }
        }

        private void OnEnable()
        {
            //Debug.Log("SceneModule | OnEnable");
            if (m_mapRoot != null)
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
            //Resources.UnloadAsset(m_config);
            m_config = null;
            if (m_mapRoot != null)
                GameObject.Destroy(m_mapRoot);
        }

        public void ClearMap()
        {
            if (m_mapRoot != null)
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
            if (m_mapData != null)
            {
                for (int _i = 0, _max = m_mapData.Count; _i < _max; _i++)
                {
                    m_mapData[_i].Clear();
                }
                m_mapData.Clear();
            }
            if(m_tsfMapList != null)
            {
                m_tsfMapList.Clear();
            }
            m_mapData = null;
        }

        public int getMapDataRow()
        {
            return m_config.SceneConfigList[GameManager.SceneConfigId].MapRow;
        }

        public int getMapDataCol()
        {
            return m_config.SceneConfigList[GameManager.SceneConfigId].MapCol;
        }

        public void ChangeMap(SkillId id, int playerRow, int playerCol)
        {
            int _mapRow = m_config.SceneConfigList[GameManager.SceneConfigId].MapRow;
            int _mapCol = m_config.SceneConfigList[GameManager.SceneConfigId].MapCol;
            switch (id)
            {
                case SkillId.JU:
                    for(int _i = 0; _i < _mapRow; ++_i)
                    {
                        if(_i != playerRow)
                        {
                            if(m_mapData[_i][playerCol] != MapDataType.GAOTAI && m_mapData[_i][playerCol] != MapDataType.NONE)
                            {
                                //m_tsfMapList[_i * _mapCol + playerCol].GetComponent<Renderer>().material.color = Color.red;
                            }
                        }
                    }
                    for(int _j = 0; _j < _mapCol; ++_j)
                    {
                        if(_j != playerCol)
                        {
                            if(m_mapData[playerRow][_j] != MapDataType.GAOTAI && m_mapData[playerRow][_j] != MapDataType.NONE)
                            {
                                //m_tsfMapList[playerRow * _mapCol + _j].GetComponent<Renderer>().material.color = Color.red;
                            }
                        }
                    }

                    break;
                case SkillId.MA:
                    if(playerRow >= 1)
                    {
                        if(playerCol >= 2)
                        {
                            if(m_mapData[playerRow - 1][playerCol - 2] != MapDataType.NONE
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow][playerCol - 1] != MapDataType.GAOTAI)))
                            {
                                //m_tsfMapList[playerRow * _mapCol + _j].GetComponent<Renderer>().material.color = Color.red;
                            }
                        }
                        if(playerCol + 2 < _mapCol)
                        {
                            if(m_mapData[playerRow - 1][playerCol + 2] != MapDataType.NONE
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow][playerCol + 1] != MapDataType.GAOTAI)))
                            {
                                //m_tsfMapList[(playerRow - 1) * _mapCol + playerCol + 2].GetComponent<Renderer>().material.color = Color.red;
                            }
                        }
                    }
                    if(playerRow >= 2)
                    {
                        if (playerCol >= 1)
                        {
                            if (m_mapData[playerRow - 2][playerCol - 1] != MapDataType.NONE
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow - 1][playerCol] != MapDataType.GAOTAI)))
                            {
                                //m_tsfMapList[playerRow * _mapCol + _j].GetComponent<Renderer>().material.color = Color.red;
                            }
                        }
                        if (playerCol + 2 < _mapCol)
                        {
                            if (m_mapData[playerRow - 2][playerCol + 1] != MapDataType.NONE
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow - 1][playerCol] != MapDataType.GAOTAI)))
                            {
                                //m_tsfMapList[(playerRow - 1) * _mapCol + playerCol + 2].GetComponent<Renderer>().material.color = Color.red;
                            }
                        }
                    }
                    if(playerRow + 1 < _mapRow)
                    {
                        if(playerCol >= 2)
                        {
                            if (m_mapData[playerRow + 1][playerCol - 2] != MapDataType.NONE
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow][playerCol - 1] != MapDataType.GAOTAI)))
                            {
                                //m_tsfMapList[playerRow * _mapCol + _j].GetComponent<Renderer>().material.color = Color.red;
                            }
                        }
                        if (playerCol + 2 < _mapCol)
                        {
                            if (m_mapData[playerRow + 1][playerCol + 2] != MapDataType.NONE
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow][playerCol + 1] != MapDataType.GAOTAI)))
                            {
                                //m_tsfMapList[(playerRow - 1) * _mapCol + playerCol + 2].GetComponent<Renderer>().material.color = Color.red;
                            }
                        }
                    }
                    if(playerRow + 2 < _mapRow)
                    {
                        if (playerCol >= 1)
                        {
                            if (m_mapData[playerRow + 2][playerCol - 1] != MapDataType.NONE
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow + 1][playerCol] != MapDataType.GAOTAI)))
                            {
                                //m_tsfMapList[playerRow * _mapCol + _j].GetComponent<Renderer>().material.color = Color.red;
                            }
                        }
                        if (playerCol + 2 < _mapCol)
                        {
                            if (m_mapData[playerRow + 2][playerCol + 1] != MapDataType.NONE
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow + 1][playerCol] != MapDataType.GAOTAI)))
                            {
                                //m_tsfMapList[(playerRow - 1) * _mapCol + playerCol + 2].GetComponent<Renderer>().material.color = Color.red;
                            }
                        }
                    }
                    break;
                case SkillId.PAO:
                    break;
                case SkillId.XIANG:
                    if(playerRow >= 2)
                    {
                        if(playerCol >= 2)
                        {
                            if (m_mapData[playerRow - 2][playerCol - 2] != MapDataType.NONE
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow - 1][playerCol - 1] != MapDataType.GAOTAI)))
                            {

                            }
                        }
                        if(playerCol + 2 < _mapCol)
                        {
                            if (m_mapData[playerRow - 2][playerCol + 2] != MapDataType.NONE
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow - 1][playerCol + 1] != MapDataType.GAOTAI)))
                            {

                            }
                        }
                    }
                    if(playerRow + 2 < _mapCol)
                    {
                        if (playerCol >= 2)
                        {
                            if (m_mapData[playerRow + 2][playerCol - 2] != MapDataType.NONE
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow + 1][playerCol - 1] != MapDataType.GAOTAI)))
                            {

                            }
                        }
                        if (playerCol + 2 < _mapCol)
                        {
                            if (m_mapData[playerRow + 2][playerCol + 2] != MapDataType.NONE
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow + 1][playerCol + 1] != MapDataType.GAOTAI)))
                            {

                            }
                        }
                    }
                    break;
                case SkillId.SHI:
                    break;
                case SkillId.BING:
                    break;
            }
        }
    }
}

