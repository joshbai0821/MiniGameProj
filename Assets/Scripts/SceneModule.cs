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
        private SceneConfig m_config;
        private Player m_player;
		private Enemy m_enemy;
        private List<Transform> m_tsfMapList;

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
			LoadEnemy("enemy");
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

		//卡马腿，象腿用的
		private bool PosExitChess(int row, int col)
        {
			if(m_EnemyList[row][col] != null )
			{
				return true;
			}

			if(row == m_player.m_playerPos.m_row && col == m_player.m_playerPos.m_col)
			{
				return true;
			}

			return false;
		}


		private void GetEnemyNextPos(Enemy enemy)
        {
			//判断,是否是可走的点
            int _mapRow = m_config.SceneConfigList[GameManager.SceneConfigId].MapRow;
            int _mapCol = m_config.SceneConfigList[GameManager.SceneConfigId].MapCol;
			
			int DistancePlayer = -1;
			//当前距离最近的点
			int minrow = 0;
			int mincol = 0;

			int temprow;
			int tempcol;

			//player的位置
			int PRow = m_player.m_playerPos.m_row;
			int PCol = m_player.m_playerPos.m_col;

			//enemy当前的位置
			int playerRow = enemy.m_EnemyPosNew.m_row;
			int playerCol = enemy.m_EnemyPosNew.m_col;
			
            switch (enemy.EnemyType)
            {
            	//马
                case 1:
                    if(playerRow >= 1)
                    {
                        if(playerCol >= 2)
                        {
                            if(m_mapData[playerRow - 1][playerCol - 2] != MapDataType.NONE
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow][playerCol - 1] != MapDataType.GAOTAI)))
                            {
                            	if(PosExitChess(playerRow,playerCol - 1) == false)
                            	{
                            		temprow = playerRow - 1;
									tempcol = playerCol - 2;
                            		//找一个最近的点
									if(DistancePlayer == -1)
									{
										DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
										minrow = temprow;
										mincol = tempcol;
									}
									else if(DistancePlayer > (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol))
									{
										DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
										minrow = temprow;
										mincol = tempcol;
									}
								}
                            	
                            }
                        }
                        if(playerCol + 2 < _mapCol)
                        {
                            if(m_mapData[playerRow - 1][playerCol + 2] != MapDataType.NONE
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow][playerCol + 1] != MapDataType.GAOTAI)))
                            {
                                if(PosExitChess(playerRow,playerCol + 1) == false)
								{
									temprow = playerRow - 1;
									tempcol = playerCol + 2;
                            		//找一个最近的点
									if(DistancePlayer == -1)
									{
										DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
										minrow = temprow;
										mincol = tempcol;
									}
									else if(DistancePlayer > (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol))
									{
										DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
										minrow = temprow;
										mincol = tempcol;
									}
								}
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
                                if(PosExitChess(playerRow - 1,playerCol) == false)
								{
									temprow = playerRow - 2;
									tempcol = playerCol - 1;
                            		//找一个最近的点
									if(DistancePlayer == -1)
									{
										DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
										minrow = temprow;
										mincol = tempcol;
									}
									else if(DistancePlayer > (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol))
									{
										DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
										minrow = temprow;
										mincol = tempcol;
									}
								}
                            }
                        }
                        if (playerCol + 2 < _mapCol)
                        {
                            if (m_mapData[playerRow - 2][playerCol + 1] != MapDataType.NONE
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow - 1][playerCol] != MapDataType.GAOTAI)))
                            {
								if(PosExitChess(playerRow - 1,playerCol) == false)
								{
									temprow = playerRow - 2;
									tempcol = playerCol + 1;
                            		//找一个最近的点
									if(DistancePlayer == -1)
									{
										DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
										minrow = temprow;
										mincol = tempcol;
									}
									else if(DistancePlayer > (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol))
									{
										DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
										minrow = temprow;
										mincol = tempcol;
									}
								}
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
                                if(PosExitChess(playerRow,playerCol - 1) == false)
								{
									temprow = playerRow + 1;
									tempcol = playerCol - 2;
                            		//找一个最近的点
									if(DistancePlayer == -1)
									{
										DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
										minrow = temprow;
										mincol = tempcol;
									}
									else if(DistancePlayer > (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol))
									{
										DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
										minrow = temprow;
										mincol = tempcol;
									}
								}
                            }
                        }
                        if (playerCol + 2 < _mapCol)
                        {
                            if (m_mapData[playerRow + 1][playerCol + 2] != MapDataType.NONE
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow][playerCol + 1] != MapDataType.GAOTAI)))
                            {
                                if(PosExitChess(playerRow,playerCol + 1) == false)
								{
									temprow = playerRow + 1;
									tempcol = playerCol + 2;
                            		//找一个最近的点
									if(DistancePlayer == -1)
									{
										DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
										minrow = temprow;
										mincol = tempcol;
									}
									else if(DistancePlayer > (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol))
									{
										DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
										minrow = temprow;
										mincol = tempcol;
									}
								}
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
                                if(PosExitChess(playerRow + 1,playerCol) == false)
								{
									temprow = playerRow + 2;
									tempcol = playerCol - 1;
                            		//找一个最近的点
									if(DistancePlayer == -1)
									{
										DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
										minrow = temprow;
										mincol = tempcol;
									}
									else if(DistancePlayer > (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol))
									{
										DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
										minrow = temprow;
										mincol = tempcol;
									}
								}
                            }
                        }
                        if (playerCol + 2 < _mapCol)
                        {
                            if (m_mapData[playerRow + 2][playerCol + 1] != MapDataType.NONE
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow + 1][playerCol] != MapDataType.GAOTAI)))
                            {
                                if(PosExitChess(playerRow + 1,playerCol) == false)
								{
									temprow = playerRow + 2;
									tempcol = playerCol + 1;
                            		//找一个最近的点
									if(DistancePlayer == -1)
									{
										DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
										minrow = temprow;
										mincol = tempcol;
									}
									else if(DistancePlayer > (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol))
									{
										DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
										minrow = temprow;
										mincol = tempcol;
									}
								}
                            }
                        }
                    }
                    break;
                case 2:
                    if(playerRow >= 2)
                    {
                        if(playerCol >= 2)
                        {
                            if (m_mapData[playerRow - 2][playerCol - 2] != MapDataType.NONE
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow - 1][playerCol - 1] != MapDataType.GAOTAI)))
                            {
                                if(PosExitChess(playerRow - 1,playerCol -1) == false)
								{
									temprow = playerRow - 2;
									tempcol = playerCol - 2;
                            		//找一个最近的点
									if(DistancePlayer == -1)
									{
										DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
										minrow = temprow;
										mincol = tempcol;
									}
									else if(DistancePlayer > (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol))
									{
										DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
										minrow = temprow;
										mincol = tempcol;
									}
								}
                            }
                        }
                        if(playerCol + 2 < _mapCol)
                        {
                            if (m_mapData[playerRow - 2][playerCol + 2] != MapDataType.NONE
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow - 1][playerCol + 1] != MapDataType.GAOTAI)))
                            {
								if(PosExitChess(playerRow - 1,playerCol + 1) == false)
								{
									temprow = playerRow - 2;
									tempcol = playerCol + 2;
									//找一个最近的点
									if(DistancePlayer == -1)
									{
										DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
										minrow = temprow;
										mincol = tempcol;
									}
									else if(DistancePlayer > (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol))
									{
										DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
										minrow = temprow;
										mincol = tempcol;
									}
								}

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
								if(PosExitChess(playerRow + 1,playerCol -1) == false)
								{
									temprow = playerRow + 2;
									tempcol = playerCol - 2;
									//找一个最近的点
									if(DistancePlayer == -1)
									{
										DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
										minrow = temprow;
										mincol = tempcol;
									}
									else if(DistancePlayer > (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol))
									{
										DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
										minrow = temprow;
										mincol = tempcol;
									}
								}

                            }
                        }
                        if (playerCol + 2 < _mapCol)
                        {
                            if (m_mapData[playerRow + 2][playerCol + 2] != MapDataType.NONE
                                && (m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (m_mapData[playerRow + 1][playerCol + 1] != MapDataType.GAOTAI)))
                            {
								if(PosExitChess(playerRow + 1,playerCol + 1) == false)
								{
									temprow = playerRow + 2;
									tempcol = playerCol + 2;
									//找一个最近的点
									if(DistancePlayer == -1)
									{
										DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
										minrow = temprow;
										mincol = tempcol;
									}
									else if(DistancePlayer > (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol))
									{
										DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
										minrow = temprow;
										mincol = tempcol;
									}
								}

                            }
                        }
                    }
                    break;
                case 3:
						if(playerCol >= 1 && m_mapData[playerRow][playerCol - 1] != MapDataType.GAOTAI && m_mapData[playerRow][playerCol - 1] != MapDataType.NONE && m_EnemyList[playerRow][playerCol - 1] == null)
						{
							temprow = playerRow;
							tempcol = playerCol - 1;
							//找一个最近的点
							if(DistancePlayer == -1)
							{
								DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
								minrow = temprow;
								mincol = tempcol;
							}
							else if(DistancePlayer > (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol))
							{
								DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
								minrow = temprow;
								mincol = tempcol;
							}
						}
						
						if(playerCol + 1 < _mapCol && m_mapData[playerRow][playerCol + 1] != MapDataType.GAOTAI && m_mapData[playerRow][playerCol + 1] != MapDataType.NONE && m_EnemyList[playerRow][playerCol + 1] == null)
						{
							temprow = playerRow;
							tempcol = playerCol + 1;
							//找一个最近的点
							if(DistancePlayer == -1)
							{
								DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
								minrow = temprow;
								mincol = tempcol;
							}
							else if(DistancePlayer > (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol))
							{
								DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
								minrow = temprow;
								mincol = tempcol;
							}

						}

						
						if(playerRow >= 1 && m_mapData[playerRow - 1][playerCol] != MapDataType.GAOTAI && m_mapData[playerRow - 1][playerCol] != MapDataType.NONE && m_EnemyList[playerRow - 1][playerCol] == null)
						{
							temprow = playerRow - 1;
							tempcol = playerCol;
							//找一个最近的点
							if(DistancePlayer == -1)
							{
								DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
								minrow = temprow;
								mincol = tempcol;
							}
							else if(DistancePlayer > (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol))
							{
								DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
								minrow = temprow;
								mincol = tempcol;
							}

						}

						if(playerRow + 1 < _mapRow && m_mapData[playerRow + 1][playerCol] != MapDataType.GAOTAI && m_mapData[playerRow + 1][playerCol] != MapDataType.NONE && m_EnemyList[playerRow + 1][playerCol] == null)
						{
							temprow = playerRow + 1;
							tempcol = playerCol;
							//找一个最近的点
							if(DistancePlayer == -1)
							{
								DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
								minrow = temprow;
								mincol = tempcol;
							}
							else if(DistancePlayer > (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol))
							{
								DistancePlayer = (PRow - temprow)*(PRow - temprow)+(tempcol - PCol)*(tempcol - PCol);
								minrow = temprow;
								mincol = tempcol;
							}

						}
						
                    break;
            }

			Debug.Log(string.Format("minx:{0}, miny:{1}",minrow, mincol));
			if(DistancePlayer == -1)
			{
				//没有点可以走
				enemy.MovePos(enemy.m_EnemyPosNew.m_row, enemy.m_EnemyPosNew.m_col);
			}
			else
			{
				m_EnemyList[minrow][mincol] = m_EnemyList[enemy.m_EnemyPosNew.m_row][enemy.m_EnemyPosNew.m_col];
				m_EnemyList[enemy.m_EnemyPosNew.m_row][enemy.m_EnemyPosNew.m_col] = null;
				enemy.MovePos(minrow, mincol);
			}

        }


		public void EnemyListUpdate()
        {
			//清空所有enemy change 标记
			for(int _i = 0; _i < m_EnemyList.Count; _i++)
			{
				for(int _j = 0; _j < m_EnemyList[_i].Count; _j++)
				{
					if(m_EnemyList[_i][_j] != null)
					{
						m_EnemyList[_i][_j].PosIsChange = 0;
					}
				}
			}
			//找一个enemy
			for(int _i = 0; _i < m_EnemyList.Count; _i++)
			{
				for(int _j = 0; _j < m_EnemyList[_i].Count; _j++)
				{
					if(m_EnemyList[_i][_j] != null && m_EnemyList[_i][_j].PosIsChange == 0)
					{
						//找出离player最近的可走的点为最后的结果
						GetEnemyNextPos(m_EnemyList[_i][_j]);
						Debug.Log(string.Format("111"));
						//更新改变位置，ischange
						m_EnemyList[_i][_j].PosIsChange = 1;
					}
				}
			}

			
			//遍历所有enemy,播位置变化的动画,update
			for(int _i = 0; _i < m_EnemyList.Count; _i++)
			{
				for(int _j = 0; _j < m_EnemyList[_i].Count; _j++)
				{
					if(m_EnemyList[_i][_j] != null)
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
						Debug.Log(string.Format("i={0},j={1},id={2}",_i,_j,_mapPrefabId));
						if (_mapPrefabId != 0)
						{
							GameObject obj = (GameObject)GameManager.ResManager.LoadPrefabSync(MapPrefabPath, EnemyPrefabName[_mapPrefabId], typeof(GameObject));
							m_enemy = obj.GetComponent<Enemy>();
							if(m_enemy != null)
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
			if(m_EnemyList != null)
			{
				for(int _i = 0, _max = m_EnemyList.Count; _i < _max; _i++)
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

