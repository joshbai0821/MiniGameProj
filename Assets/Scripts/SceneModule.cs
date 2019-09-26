using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniProj
{
    public class SceneModule : Module
    {
        /*
			MA = 1,
			XIANG = 2,
			SHI = 3,
			JU = 4,
			PAO = 5,
		*/

        private GameObject m_mapRoot;
        public SceneConfig m_config;
        public Player m_player;
        private Enemy m_enemy;
        private List<Transform> m_tsfMapList;
        private List<Material> m_matList;
        private List<Color> m_originColorList;

        public SceneModule() : base("SceneModule")
        {
        }

        //1 马   2 象  3 士
        private static string[] EnemyPrefabName =
        {
            "null",
            "Cube 1",
            "Cube 1",
            "Cube 1",
        };

        private static string MapPrefabPath = "Prefabs/Map";

        private static float MapPrefabSizeX = 1;
        private static float MapPrefabSizeZ = 1;
        public List<List<Enemy>> m_EnemyList;
        public List<List<MapDataType>> m_mapData;
        public List<List<MapDataType>> Data
        {
            get { return m_mapData; }
        }

        private void Awake()
        {
            m_config = Resources.Load<SceneConfig>("SceneConfig");
            m_mapRoot = new GameObject("MapRoot");
            m_mapRoot.transform.SetParent(GameManager.GameManagerObj.GetComponent<GameManager>().SceneLayer);
            string _name = m_config.SceneConfigList[GameManager.SceneConfigId].PrefabName;
            GameObject _mapPrefab = (GameObject)GameManager.ResManager.LoadPrefabSync(MapPrefabPath, _name, typeof(GameObject));
            _mapPrefab.transform.SetParent(m_mapRoot.transform, false);
            m_matList = new List<Material>();
            m_originColorList = new List<Color>();
            LoadMap();
            LoadPlayer();
            LoadSkillBtn();
            LoadRookieModule();
            LoadEnemy("enemy");
        }

        public bool isPlayerReady()
        {
            return m_player.IsReady();
        }

        private void LoadRookieModule()
        {
            if (GameManager.SceneConfigId == 0)
            {
                GameManager.GameManagerObj.GetComponent<GameManager>().LoadModule("RookieModule");
            }
        }

        private void LoadSkillBtn()
        {
            GameObject _skillPanel = (GameObject)GameManager.ResManager.LoadPrefabSync(MapPrefabPath, "SKillPanel", typeof(GameObject));
            _skillPanel.transform.SetParent(GameManager.GameManagerObj.GetComponent<GameManager>().UILayer, false);
            for (int _i = 0; _i < m_config.SceneConfigList[GameManager.SceneConfigId].SkillData.Count; ++_i)
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
            _playerPrefab.transform.SetParent(GameManager.GameManagerObj.GetComponent<GameManager>().SceneLayer);
            m_player = _playerPrefab.GetComponent<Player>();
            if (m_player != null)
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
                if (_mapData != null)
                {
                    int _r = _mapData.Pos.m_row;
                    int _c = _mapData.Pos.m_col;
                    m_mapData[_r][_c] = _mapData.Data;
                }
            }
        }


        public void EnemyListUpdate()
        {
            //清空所有enemy change 标记
            for (int _i = 0; _i < m_EnemyList.Count; _i++)
            {
                for (int _j = 0; _j < m_EnemyList[_i].Count; _j++)
                {
                    if (m_EnemyList[_i][_j] != null)
                    {
                        m_EnemyList[_i][_j].PosIsChange = 0;
                    }
                }
            }
            //找一个enemy
            for (int _i = 0; _i < m_EnemyList.Count; _i++)
            {
                for (int _j = 0; _j < m_EnemyList[_i].Count; _j++)
                {
                    if (m_EnemyList[_i][_j] != null && m_EnemyList[_i][_j].PosIsChange == 0)
                    {
                        //找出离player最近的可走的点为最后的结果
                        m_EnemyList[_i][_j].GetEnemyNextPos();
                        Debug.Log(string.Format("111"));
                        //更新改变位置，ischange
                        m_EnemyList[_i][_j].PosIsChange = 1;
                    }
                }
            }


            //遍历所有enemy,播位置变化的动画,update
            for (int _i = 0; _i < m_EnemyList.Count; _i++)
            {
                for (int _j = 0; _j < m_EnemyList[_i].Count; _j++)
                {
                    if (m_EnemyList[_i][_j] != null)
                    {
                        m_EnemyList[_i][_j].Update();
                    }
                }
            }


        }


        private void DealEnemyData(ref string[] rowString)
        {
            m_EnemyList = new List<List<Enemy>>(rowString.Length);
            for (int _i = 0; _i < rowString.Length; ++_i)
            {
                string[] _str = rowString[_i].Split(',');
                float _minZ = -(_str.Length / 2.0f) * MapPrefabSizeZ;
                List<Enemy> _dataList = new List<Enemy>(_str.Length);
                for (int _j = 0; _j < _str.Length; ++_j)
                {
                    int _mapPrefabId = 0;
                    if (int.TryParse(_str[_j], out _mapPrefabId) && _mapPrefabId >= -1 && _mapPrefabId < EnemyPrefabName.Length)
                    {
                        Debug.Log(string.Format("i={0},j={1},id={2}", _i, _j, _mapPrefabId));
                        if (_mapPrefabId != 0)
                        {
                            GameObject obj = (GameObject)GameManager.ResManager.LoadPrefabSync(MapPrefabPath, EnemyPrefabName[_mapPrefabId], typeof(GameObject));
                            m_enemy = obj.GetComponent<Enemy>();
                            if (m_enemy != null)
                            {
                                m_enemy.SetType(_mapPrefabId);
                                m_enemy.SetStartPos(_i, _j);
                                _dataList.Add(m_enemy);
                            }
                            else
                            {
                                Debug.Log("load enemy error");
                            }
                        }
                        else
                        {
                            _dataList.Add(null);
                        }

                    }
                    else
                    {

                        Debug.Log(string.Format("SceneModule, DealMapData Error In row {0} col {1}", _i, _j));
                    }

                }
                m_EnemyList.Add(_dataList);
            }

        }

        private void LoadEnemy(string name)
        {
            ClearEnemyData();

            TextAsset _textAsset = Resources.Load<TextAsset>(name);
            string[] _rowString = _textAsset.text.Trim().Split('\n');
            DealEnemyData(ref _rowString);
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
            ClearMapData();
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

        private void ClearEnemyData()
        {
            if (m_EnemyList != null)
            {
                for (int _i = 0, _max = m_EnemyList.Count; _i < _max; _i++)
                {
                    m_EnemyList[_i].Clear();
                }
                m_EnemyList.Clear();
            }

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
            if (m_tsfMapList != null)
            {
                m_tsfMapList.Clear();
            }
            if (m_matList != null)
            {
                m_matList.Clear();
            }
            if (m_originColorList != null)
            {
                m_originColorList.Clear();
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
            RefreshMap();
            switch (id)
            {
                case SkillId.JU:
                    for (int _i = playerRow + 1; _i < _mapRow; ++_i)
                    {
                        if (m_mapData[_i][playerCol] != MapDataType.GAOTAI && m_mapData[_i][playerCol] != MapDataType.NONE)
                        {
                            Material _material = m_tsfMapList[_i * _mapCol + playerCol].GetComponent<MeshRenderer>().material;
                            m_originColorList.Add(_material.GetColor("_MainColor"));
                            _material.SetColor("_MainColor", Color.red);
                            m_matList.Add(_material);

                        }
                        else
                        {
                            break;
                        }
                    }
                    for (int _i = playerRow - 1; _i >= 0; --_i)
                    {
                        if (m_mapData[_i][playerCol] != MapDataType.GAOTAI && m_mapData[_i][playerCol] != MapDataType.NONE)
                        {
                            Material _material = m_tsfMapList[_i * _mapCol + playerCol].GetComponent<MeshRenderer>().material;
                            m_originColorList.Add(_material.GetColor("_MainColor"));
                            _material.SetColor("_MainColor", Color.red);
                            m_matList.Add(_material);
                        }
                        else
                        {
                            break;
                        }
                    }
                    for (int _j = playerCol + 1; _j < _mapCol; ++_j)
                    {
                        if (m_mapData[playerRow][_j] != MapDataType.GAOTAI && m_mapData[playerRow][_j] != MapDataType.NONE)
                        {
                            Material _material = m_tsfMapList[playerRow * _mapCol + _j].GetComponent<MeshRenderer>().material;
                            m_originColorList.Add(_material.GetColor("_MainColor"));
                            _material.SetColor("_MainColor", Color.red);
                            m_matList.Add(_material);
                        }
                        else
                        {
                            break;
                        }
                    }
                    for (int _j = playerCol - 1; _j >= 0; --_j)
                    {
                        if (m_mapData[playerRow][_j] != MapDataType.GAOTAI && m_mapData[playerRow][_j] != MapDataType.NONE)
                        {
                            Material _material = m_tsfMapList[playerRow * _mapCol + _j].GetComponent<MeshRenderer>().material;
                            m_originColorList.Add(_material.GetColor("_MainColor"));
                            _material.SetColor("_MainColor", Color.red);
                            m_matList.Add(_material);
                        }
                        else
                        {
                            break;
                        }
                    }
                    break;
                case SkillId.MA:
                    if (playerRow >= 1)
                    {
                        if (playerCol >= 2)
                        {
                            if (m_mapData[playerRow - 1][playerCol - 2] != MapDataType.NONE
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow][playerCol - 1] != MapDataType.GAOTAI)))
                            {
                                Material _material = m_tsfMapList[(playerRow - 1) * _mapCol + playerCol - 2].GetComponent<MeshRenderer>().material;
                                m_originColorList.Add(_material.GetColor("_MainColor"));
                                _material.SetColor("_MainColor", Color.red);
                                m_matList.Add(_material);
                            }
                        }
                        if (playerCol + 2 < _mapCol)
                        {
                            if (m_mapData[playerRow - 1][playerCol + 2] != MapDataType.NONE
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow][playerCol + 1] != MapDataType.GAOTAI)))
                            {
                                Material _material = m_tsfMapList[(playerRow - 1) * _mapCol + playerCol + 2].GetComponent<MeshRenderer>().material;
                                m_originColorList.Add(_material.GetColor("_MainColor"));
                                _material.SetColor("_MainColor", Color.red);
                                m_matList.Add(_material);
                            }
                        }
                    }
                    if (playerRow >= 2)
                    {
                        if (playerCol >= 1)
                        {
                            if (m_mapData[playerRow - 2][playerCol - 1] != MapDataType.NONE
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow - 1][playerCol] != MapDataType.GAOTAI)))
                            {
                                Material _material = m_tsfMapList[(playerRow - 2) * _mapCol + playerCol - 1].GetComponent<MeshRenderer>().material;
                                m_originColorList.Add(_material.GetColor("_MainColor"));
                                _material.SetColor("_MainColor", Color.red);
                                m_matList.Add(_material);
                            }
                        }
                        if (playerCol + 2 < _mapCol)
                        {
                            if (m_mapData[playerRow - 2][playerCol + 1] != MapDataType.NONE
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow - 1][playerCol] != MapDataType.GAOTAI)))
                            {
                                Material _material = m_tsfMapList[(playerRow - 2) * _mapCol + playerCol + 1].GetComponent<MeshRenderer>().material;
                                m_originColorList.Add(_material.GetColor("_MainColor"));
                                _material.SetColor("_MainColor", Color.red);
                                m_matList.Add(_material);
                            }
                        }
                    }
                    if (playerRow + 1 < _mapRow)
                    {
                        if (playerCol >= 2)
                        {
                            if (m_mapData[playerRow + 1][playerCol - 2] != MapDataType.NONE
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow][playerCol - 1] != MapDataType.GAOTAI)))
                            {
                                Material _material = m_tsfMapList[(playerRow + 1) * _mapCol + playerCol - 2].GetComponent<MeshRenderer>().material;
                                m_originColorList.Add(_material.GetColor("_MainColor"));
                                _material.SetColor("_MainColor", Color.red);
                                m_matList.Add(_material);
                            }
                        }
                        if (playerCol + 2 < _mapCol)
                        {
                            if (m_mapData[playerRow + 1][playerCol + 2] != MapDataType.NONE
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow][playerCol + 1] != MapDataType.GAOTAI)))
                            {
                                Material _material = m_tsfMapList[(playerRow + 1) * _mapCol + playerCol + 2].GetComponent<MeshRenderer>().material;
                                m_originColorList.Add(_material.GetColor("_MainColor"));
                                _material.SetColor("_MainColor", Color.red);
                                m_matList.Add(_material);
                            }
                        }
                    }
                    if (playerRow + 2 < _mapRow)
                    {
                        if (playerCol >= 1)
                        {
                            if (m_mapData[playerRow + 2][playerCol - 1] != MapDataType.NONE
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow + 1][playerCol] != MapDataType.GAOTAI)))
                            {
                                Material _material = m_tsfMapList[(playerRow + 2) * _mapCol + playerCol - 1].GetComponent<MeshRenderer>().material;
                                m_originColorList.Add(_material.GetColor("_MainColor"));
                                _material.SetColor("_MainColor", Color.red);
                                m_matList.Add(_material);
                            }
                        }
                        if (playerCol + 1 < _mapCol)
                        {
                            if (m_mapData[playerRow + 2][playerCol + 1] != MapDataType.NONE
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow + 1][playerCol] != MapDataType.GAOTAI)))
                            {
                                Material _material = m_tsfMapList[(playerRow + 2) * _mapCol + playerCol + 1].GetComponent<MeshRenderer>().material;
                                m_originColorList.Add(_material.GetColor("_MainColor"));
                                _material.SetColor("_MainColor", Color.red);
                                m_matList.Add(_material);
                            }
                        }
                    }
                    break;
                case SkillId.PAO:
                    break;
                case SkillId.XIANG:
                    if (playerRow >= 2)
                    {
                        if (playerCol >= 2)
                        {
                            if (m_mapData[playerRow - 2][playerCol - 2] != MapDataType.NONE
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow - 1][playerCol - 1] != MapDataType.GAOTAI)))
                            {
                                Material _material = m_tsfMapList[(playerRow - 2) * _mapCol + playerCol - 2].GetComponent<MeshRenderer>().material;
                                m_originColorList.Add(_material.GetColor("_MainColor"));
                                _material.SetColor("_MainColor", Color.red);
                                m_matList.Add(_material);
                            }
                        }
                        if (playerCol + 2 < _mapCol)
                        {
                            if (m_mapData[playerRow - 2][playerCol + 2] != MapDataType.NONE
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow - 1][playerCol + 1] != MapDataType.GAOTAI)))
                            {
                                Material _material = m_tsfMapList[(playerRow - 2) * _mapCol + playerCol + 2].GetComponent<MeshRenderer>().material;
                                m_originColorList.Add(_material.GetColor("_MainColor"));
                                _material.SetColor("_MainColor", Color.red);
                                m_matList.Add(_material);
                            }
                        }
                    }
                    if (playerRow + 2 < _mapRow)
                    {
                        if (playerCol >= 2)
                        {
                            if (m_mapData[playerRow + 2][playerCol - 2] != MapDataType.NONE
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow + 1][playerCol - 1] != MapDataType.GAOTAI)))
                            {
                                Material _material = m_tsfMapList[(playerRow + 2) * _mapCol + playerCol - 2].GetComponent<MeshRenderer>().material;
                                m_originColorList.Add(_material.GetColor("_MainColor"));
                                _material.SetColor("_MainColor", Color.red);
                                m_matList.Add(_material);
                            }
                        }
                        if (playerCol + 2 < _mapCol)
                        {
                            if (m_mapData[playerRow + 2][playerCol + 2] != MapDataType.NONE
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow + 1][playerCol + 1] != MapDataType.GAOTAI)))
                            {
                                Material _material = m_tsfMapList[(playerRow + 2) * _mapCol + playerCol + 2].GetComponent<MeshRenderer>().material;
                                m_originColorList.Add(_material.GetColor("_MainColor"));
                                _material.SetColor("_MainColor", Color.red);
                                m_matList.Add(_material);
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

        public void RefreshMap()
        {
            if (m_matList != null)
            {
                for (int _i = 0; _i < m_matList.Count; ++_i)
                {
                    m_matList[_i].SetColor("_MainColor", m_originColorList[_i]);
                }
                m_matList.Clear();
            }
        }
    }
}

