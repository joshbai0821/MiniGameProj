using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
        private int m_waitCount;
        private int m_npcCount;

        public SceneConfig m_config;
        public Player m_player;
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
            "Enemy",
            "Enemy",
            "Enemy",
        };

        private static string MapPrefabPath = "Prefabs/Map";

        public MapPos YuJiPos;
        public List<List<Enemy>> m_enemyList;
        public List<List<FixedRouteNpc>> m_npcList;
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
            LoadEnemy();
            LoadNpc();

            EventManager.RegisterEvent(HLEventId.NPC_END_MOVE, this.GetHashCode(), NpcComplete);
        }

        public bool isPlayerReady()
        {
            return m_player.IsReady();
        }

        public void GetPlayerPos(ref MapPos pos)
        {
            pos = m_player.Pos;
        }

        private void LoadNpc()
        {
            ClearNpcData();
            int _row = m_config.SceneConfigList[GameManager.SceneConfigId].MapRow;
            int _col = m_config.SceneConfigList[GameManager.SceneConfigId].MapCol;
            m_npcList = new List<List<FixedRouteNpc>>(_row);
            for (int _i = 0; _i < _row; ++_i)
            {
                List<FixedRouteNpc> _lst = new List<FixedRouteNpc>(_col);
                for (int _j = 0; _j < _col; ++_j)
                {
                    _lst.Add(null);
                }
                m_npcList.Add(_lst);
            }
            for (int _j = 0; _j < m_config.SceneConfigList[GameManager.SceneConfigId].NpcPosData.Count; ++_j)
            {
                int _r = m_config.SceneConfigList[GameManager.SceneConfigId].NpcPosData[_j].m_row;
                int _c = m_config.SceneConfigList[GameManager.SceneConfigId].NpcPosData[_j].m_col;
                GameObject _obj = (GameObject)GameManager.ResManager.LoadPrefabSync(MapPrefabPath, "Npc", typeof(GameObject));
                m_npcList[_r][_c] = _obj.GetComponent<FixedRouteNpc>();
                if (m_npcList[_r][_c] != null)
                {
                    m_npcList[_r][_c].SetPosition(_r, _c);
                }
                else
                {
                    Debug.Log("SceneModule | LoadEnemy Error");
                }
            }
            m_npcCount = m_config.SceneConfigList[GameManager.SceneConfigId].NpcPosData.Count;
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

        public void WaitNpc()
        {
            m_waitCount = m_npcCount;
        }

        public void NpcComplete(EventArgs args)
        {
            --m_waitCount;
            if (m_waitCount == 0)
            {
                m_player.SetCanMove();
                EnemyListUpdate();
            }
        }

        //返回0 没有虞姬 ，返回1 有虞姬，已经更新位置
        private bool UpdateYuJiPos()
        {
            int _row = m_config.SceneConfigList[GameManager.SceneConfigId].MapRow;
            int _col = m_config.SceneConfigList[GameManager.SceneConfigId].MapCol;

            for (int _i = 0; _i < _row; ++_i)
            {

                for (int _j = 0; _j < _col; ++_j)
                {
                    if (m_npcList[_i][_j] != null)
                    {
                        YuJiPos.m_row = _i;
                        YuJiPos.m_col = _j;
                        return true;
                    }
                }
                
            }
            return false;
        }

        public void EnemyListUpdate()
        {
            bool YuJiExist = UpdateYuJiPos();
            //清空所有enemy change 标记
            for (int _i = 0; _i < m_enemyList.Count; _i++)
            {
                for (int _j = 0; _j < m_enemyList[_i].Count; _j++)
                {
                    if (m_enemyList[_i][_j] != null)
                    {
                        m_enemyList[_i][_j].PosIsChange = 0;
                    }
                }
            }

            int GameOver = 0;
            //找一个enemy
            for (int _i = 0; _i < m_enemyList.Count; _i++)
            {
                for (int _j = 0; _j < m_enemyList[_i].Count; _j++)
                {
                    if (m_enemyList[_i][_j] != null && m_enemyList[_i][_j].PosIsChange == 0)
                    {
                        //找出离player最近的可走的点为最后的结果
                        if ((GameOver = m_enemyList[_i][_j].GetEnemyNextPos(YuJiExist)) != 0)
                        {
                            //吃子特效写在这，1副子， 2主子**
                        }
                        //更新改变位置，ischange
                        m_enemyList[_i][_j].PosIsChange = 1;
                    }
                }
            }

            //遍历所有enemy,播位置变化的动画,update
            for (int _i = 0; _i < m_enemyList.Count; _i++)
            {
                for (int _j = 0; _j < m_enemyList[_i].Count; _j++)
                {
                    if (m_enemyList[_i][_j] != null)
                    {
                        m_enemyList[_i][_j].Update();
                    }
                }
            }

            //触发本局游戏结束
            if (GameOver != 0)
            {
                if (YuJiExist && GameOver == 1)
                {
                    //虞姬死了
                }
                //项羽死了

            }
        }

        private void LoadEnemy()
        {
            ClearEnemyData();
            int _row = m_config.SceneConfigList[GameManager.SceneConfigId].MapRow;
            int _col = m_config.SceneConfigList[GameManager.SceneConfigId].MapCol;
            m_enemyList = new List<List<Enemy>>(_row);
            for(int _i = 0; _i < _row; ++_i)
            {
                List<Enemy> _lst = new List<Enemy>(_col);
                for (int _j = 0; _j < _col; ++_j)
                {
                    _lst.Add(null);
                }
                m_enemyList.Add(_lst);
            }
            for(int _j = 0; _j < m_config.SceneConfigList[GameManager.SceneConfigId].EnemyData.Count; ++_j)
            {
                int _r = m_config.SceneConfigList[GameManager.SceneConfigId].EnemyData[_j].Pos.m_row;
                int _c = m_config.SceneConfigList[GameManager.SceneConfigId].EnemyData[_j].Pos.m_col;
                int _type = m_config.SceneConfigList[GameManager.SceneConfigId].EnemyData[_j].Type;
                GameObject _obj = (GameObject)GameManager.ResManager.LoadPrefabSync(MapPrefabPath, EnemyPrefabName[_type], typeof(GameObject));
                m_enemyList[_r][_c] = _obj.GetComponent<Enemy>();
                if (m_enemyList[_r][_c] != null)
                {
                    m_enemyList[_r][_c].SetType(_type);
                    m_enemyList[_r][_c].SetStartPos(_r, _c);
                }
                else
                {
                    Debug.Log("SceneModule | LoadEnemy Error");
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
            if (m_enemyList != null)
            {
                for (int _i = 0, _max = m_enemyList.Count; _i < _max; _i++)
                {
                    m_enemyList[_i].Clear();
                }
                m_enemyList.Clear();
            }

        }

        private void ClearNpcData()
        {
            if(m_npcList != null)
            {
                for (int _i = 0, _max = m_npcList.Count; _i < _max; _i++)
                {
                    m_npcList[_i].Clear();
                }
                m_npcList.Clear();
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

        private bool PosExitChess(int row, int col)
        {
            SceneModule _sceneModule = (SceneModule)GameManager.GameManagerObj.GetComponent<GameManager>().GetModuleByName("SceneModule");
            if (_sceneModule.m_enemyList[row][col] != null)
            {
                return true;
            }
            if(_sceneModule.m_npcList[row][col] != null)
            {
                return true;
            }
            return false;
        }

        public void ChangeMap(SkillId id, int playerRow, int playerCol)
        {
            if(m_waitCount > 0)
            {
                return;
            }
            int _mapRow = m_config.SceneConfigList[GameManager.SceneConfigId].MapRow;
            int _mapCol = m_config.SceneConfigList[GameManager.SceneConfigId].MapCol;
            RefreshMap();
            switch (id)
            {
                case SkillId.JU:
                    for (int _i = playerRow + 1; _i < _mapRow; ++_i)
                    {
                        if (m_mapData[_i][playerCol] != MapDataType.GAOTAI && m_mapData[_i][playerCol] != MapDataType.NONE
                            && m_npcList[_i][playerCol] == null)
                        {
                            Material _material = m_tsfMapList[_i * _mapCol + playerCol].GetComponent<MeshRenderer>().material;
                            m_originColorList.Add(_material.GetColor("_MainColor"));
                            _material.SetColor("_MainColor", Color.red);
                            m_matList.Add(_material);
                            if(m_enemyList[_i][playerCol] != null)
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    for (int _i = playerRow - 1; _i >= 0; --_i)
                    {
                        if (m_mapData[_i][playerCol] != MapDataType.GAOTAI && m_mapData[_i][playerCol] != MapDataType.NONE
                              && m_npcList[_i][playerCol] == null)
                        {
                            Material _material = m_tsfMapList[_i * _mapCol + playerCol].GetComponent<MeshRenderer>().material;
                            m_originColorList.Add(_material.GetColor("_MainColor"));
                            _material.SetColor("_MainColor", Color.red);
                            m_matList.Add(_material);
                            if(m_enemyList[_i][playerCol] != null)
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    for (int _j = playerCol + 1; _j < _mapCol; ++_j)
                    {
                        if (m_mapData[playerRow][_j] != MapDataType.GAOTAI && m_mapData[playerRow][_j] != MapDataType.NONE
                              && m_npcList[playerRow][_j] == null)
                        {
                            Material _material = m_tsfMapList[playerRow * _mapCol + _j].GetComponent<MeshRenderer>().material;
                            m_originColorList.Add(_material.GetColor("_MainColor"));
                            _material.SetColor("_MainColor", Color.red);
                            m_matList.Add(_material);
                            if(m_enemyList[playerRow][_j] != null)
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    for (int _j = playerCol - 1; _j >= 0; --_j)
                    {
                        if (m_mapData[playerRow][_j] != MapDataType.GAOTAI && m_mapData[playerRow][_j] != MapDataType.NONE
                              && m_npcList[playerRow][_j] == null)
                        {
                            Material _material = m_tsfMapList[playerRow * _mapCol + _j].GetComponent<MeshRenderer>().material;
                            m_originColorList.Add(_material.GetColor("_MainColor"));
                            _material.SetColor("_MainColor", Color.red);
                            m_matList.Add(_material);
                            if(m_enemyList[playerRow][_j] != null)
                            {
                                break;
                            }
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
                                && m_npcList[playerRow - 1][playerCol - 2] == null
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow][playerCol - 1] != MapDataType.GAOTAI)))
                            {
                                if(!PosExitChess(playerRow, playerCol - 1))
                                {
                                    Material _material = m_tsfMapList[(playerRow - 1) * _mapCol + playerCol - 2].GetComponent<MeshRenderer>().material;
                                    m_originColorList.Add(_material.GetColor("_MainColor"));
                                    _material.SetColor("_MainColor", Color.red);
                                    m_matList.Add(_material);
                                }
                                
                            }
                        }
                        if (playerCol + 2 < _mapCol)
                        {
                            if (m_mapData[playerRow - 1][playerCol + 2] != MapDataType.NONE
                                && m_npcList[playerRow - 1][playerCol + 2] == null
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow][playerCol + 1] != MapDataType.GAOTAI)))
                            {
                                if (!PosExitChess(playerRow, playerCol + 1))
                                {
                                    Material _material = m_tsfMapList[(playerRow - 1) * _mapCol + playerCol + 2].GetComponent<MeshRenderer>().material;
                                    m_originColorList.Add(_material.GetColor("_MainColor"));
                                    _material.SetColor("_MainColor", Color.red);
                                    m_matList.Add(_material);
                                }                                 
                            }
                        }
                    }
                    if (playerRow >= 2)
                    {
                        if (playerCol >= 1)
                        {
                            if (m_mapData[playerRow - 2][playerCol - 1] != MapDataType.NONE
                                && m_npcList[playerRow - 2][playerCol - 1] == null
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow - 1][playerCol] != MapDataType.GAOTAI)))
                            {
                                if(!PosExitChess(playerRow - 1, playerCol))
                                {
                                    Material _material = m_tsfMapList[(playerRow - 2) * _mapCol + playerCol - 1].GetComponent<MeshRenderer>().material;
                                    m_originColorList.Add(_material.GetColor("_MainColor"));
                                    _material.SetColor("_MainColor", Color.red);
                                    m_matList.Add(_material);
                                }
                            }
                        }
                        if (playerCol + 2 < _mapCol)
                        {
                            if (m_mapData[playerRow - 2][playerCol + 1] != MapDataType.NONE
                                && m_npcList[playerRow - 2][playerCol + 1] == null
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow - 1][playerCol] != MapDataType.GAOTAI)))
                            {
                                if(!PosExitChess(playerRow - 1, playerCol))
                                {
                                    Material _material = m_tsfMapList[(playerRow - 2) * _mapCol + playerCol + 1].GetComponent<MeshRenderer>().material;
                                    m_originColorList.Add(_material.GetColor("_MainColor"));
                                    _material.SetColor("_MainColor", Color.red);
                                    m_matList.Add(_material);
                                }
                                
                            }
                        }
                    }
                    if (playerRow + 1 < _mapRow)
                    {
                        if (playerCol >= 2)
                        {
                            if (m_mapData[playerRow + 1][playerCol - 2] != MapDataType.NONE
                                && m_npcList[playerRow + 1][playerCol - 2] == null
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow][playerCol - 1] != MapDataType.GAOTAI)))
                            {
                                if(!PosExitChess(playerRow, playerCol - 1))
                                {
                                    Material _material = m_tsfMapList[(playerRow + 1) * _mapCol + playerCol - 2].GetComponent<MeshRenderer>().material;
                                    m_originColorList.Add(_material.GetColor("_MainColor"));
                                    _material.SetColor("_MainColor", Color.red);
                                    m_matList.Add(_material);
                                }
                                
                            }
                        }
                        if (playerCol + 2 < _mapCol)
                        {
                            if (m_mapData[playerRow + 1][playerCol + 2] != MapDataType.NONE
                                && m_npcList[playerRow + 1][playerCol - 2] == null
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow][playerCol + 1] != MapDataType.GAOTAI)))
                            {
                                if(!PosExitChess(playerRow, playerCol + 1))
                                {
                                    Material _material = m_tsfMapList[(playerRow + 1) * _mapCol + playerCol + 2].GetComponent<MeshRenderer>().material;
                                    m_originColorList.Add(_material.GetColor("_MainColor"));
                                    _material.SetColor("_MainColor", Color.red);
                                    m_matList.Add(_material);
                                }
                            }
                        }
                    }
                    if (playerRow + 2 < _mapRow)
                    {
                        if (playerCol >= 1)
                        {
                            if (m_mapData[playerRow + 2][playerCol - 1] != MapDataType.NONE
                                && m_npcList[playerRow + 2][playerCol - 1] == null
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow + 1][playerCol] != MapDataType.GAOTAI)))
                            {
                                if(!PosExitChess(playerRow + 1, playerCol))
                                {
                                    Material _material = m_tsfMapList[(playerRow + 2) * _mapCol + playerCol - 1].GetComponent<MeshRenderer>().material;
                                    m_originColorList.Add(_material.GetColor("_MainColor"));
                                    _material.SetColor("_MainColor", Color.red);
                                    m_matList.Add(_material);
                                }
                                
                            }
                        }
                        if (playerCol + 1 < _mapCol)
                        {
                            if (m_mapData[playerRow + 2][playerCol + 1] != MapDataType.NONE
                                && m_npcList[playerRow + 2][playerCol + 1] == null
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow + 1][playerCol] != MapDataType.GAOTAI)))
                            {
                                if(!PosExitChess(playerRow + 1, playerCol))
                                {
                                    Material _material = m_tsfMapList[(playerRow + 2) * _mapCol + playerCol + 1].GetComponent<MeshRenderer>().material;
                                    m_originColorList.Add(_material.GetColor("_MainColor"));
                                    _material.SetColor("_MainColor", Color.red);
                                    m_matList.Add(_material);
                                }
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
                                && m_npcList[playerRow - 2][playerCol - 2] == null
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow - 1][playerCol - 1] != MapDataType.GAOTAI)))
                            {
                                if(!PosExitChess(playerRow-1, playerCol -1))
                                {
                                    Material _material = m_tsfMapList[(playerRow - 2) * _mapCol + playerCol - 2].GetComponent<MeshRenderer>().material;
                                    m_originColorList.Add(_material.GetColor("_MainColor"));
                                    _material.SetColor("_MainColor", Color.red);
                                    m_matList.Add(_material);
                                }
                                
                            }
                        }
                        if (playerCol + 2 < _mapCol)
                        {
                            if (m_mapData[playerRow - 2][playerCol + 2] != MapDataType.NONE
                                && m_npcList[playerRow - 2][playerCol + 2] == null
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow - 1][playerCol + 1] != MapDataType.GAOTAI)))
                            {
                                if(!PosExitChess(playerRow - 1, playerCol + 1))
                                {
                                    Material _material = m_tsfMapList[(playerRow - 2) * _mapCol + playerCol + 2].GetComponent<MeshRenderer>().material;
                                    m_originColorList.Add(_material.GetColor("_MainColor"));
                                    _material.SetColor("_MainColor", Color.red);
                                    m_matList.Add(_material);
                                }
                                
                            }
                        }
                    }
                    if (playerRow + 2 < _mapRow)
                    {
                        if (playerCol >= 2)
                        {
                            if (m_mapData[playerRow + 2][playerCol - 2] != MapDataType.NONE
                                && m_npcList[playerRow + 2][playerCol - 2] == null
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow + 1][playerCol - 1] != MapDataType.GAOTAI)))
                            {
                                if(!PosExitChess(playerRow + 1, playerCol - 1))
                                {
                                    Material _material = m_tsfMapList[(playerRow + 2) * _mapCol + playerCol - 2].GetComponent<MeshRenderer>().material;
                                    m_originColorList.Add(_material.GetColor("_MainColor"));
                                    _material.SetColor("_MainColor", Color.red);
                                    m_matList.Add(_material);
                                }
                                
                            }
                        }
                        if (playerCol + 2 < _mapCol)
                        {
                            if (m_mapData[playerRow + 2][playerCol + 2] != MapDataType.NONE
                                && m_npcList[playerRow + 2][playerCol + 2] == null
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow + 1][playerCol + 1] != MapDataType.GAOTAI)))
                            {
                                if(!PosExitChess(playerRow + 1, playerCol + 1))
                                {
                                    Material _material = m_tsfMapList[(playerRow + 2) * _mapCol + playerCol + 2].GetComponent<MeshRenderer>().material;
                                    m_originColorList.Add(_material.GetColor("_MainColor"));
                                    _material.SetColor("_MainColor", Color.red);
                                    m_matList.Add(_material);
                                }
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

        public Transform GetTsfMapData(int row, int col)
        {
            int _mapRow = m_config.SceneConfigList[GameManager.SceneConfigId].MapRow;
            int _mapCol = m_config.SceneConfigList[GameManager.SceneConfigId].MapCol;
            if(row > _mapRow || row < 0 || col > _mapCol || col < 0)
            {
                return null;
            }
            return m_tsfMapList[(row) * _mapCol + col];
        }
    }
}

