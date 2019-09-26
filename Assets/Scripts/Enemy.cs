using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;


namespace MiniProj
{
	public class Enemy : MonoBehaviour
	{
		//[SerializeField]
		public int EnemyType;     //敌人的类型
		public int PosIsChange;   //位置在本轮是否已经改变过了
		public MapPos m_EnemyPosOld;   //老的位置
		public MapPos m_EnemyPosNew;	//新的位置

		private static float DiffX = 3.5f;
        private static float DiffZ = 5.0f;

		private void Awake()
		{
		}

		public void SetType(int iType)
		{
			EnemyType = iType;
		}

		
		public void SetStartPos(int row, int col)
		{
			m_EnemyPosOld.m_row = row;
			m_EnemyPosOld.m_col = col;

			m_EnemyPosNew.m_row = row;
			m_EnemyPosNew.m_col = col;

			transform.position = new Vector3(col * DiffX, 1.6f, row * DiffZ);
		}

		public void MovePos(int row, int col)
		{
			m_EnemyPosOld.m_row = m_EnemyPosNew.m_row;
			m_EnemyPosOld.m_col = m_EnemyPosNew.m_col;

			m_EnemyPosNew.m_row = row;
			m_EnemyPosNew.m_col = col;
		}

		public void Update()
		{
			transform.position = new Vector3(m_EnemyPosNew.m_col * DiffX, 1.6f, m_EnemyPosNew.m_row * DiffZ);
		}

				//卡马腿，象腿用的
		private bool PosExitChess(int row, int col)
        {
            SceneModule _sceneModule = (SceneModule)GameManager.GameManagerObj.GetComponent<GameManager>().GetModuleByName("SceneModule");
			if(_sceneModule.m_EnemyList[row][col] != null )
			{
				return true;
			}

			if(row == _sceneModule.m_player.m_playerPos.m_row && col == _sceneModule.m_player.m_playerPos.m_col)
			{
				return true;
			}

			return false;
		}

		public void GetEnemyNextPos()
        {
        	SceneModule _sceneModule = (SceneModule)GameManager.GameManagerObj.GetComponent<GameManager>().GetModuleByName("SceneModule");
			//判断,是否是可走的点
            int _mapRow = _sceneModule.m_config.SceneConfigList[GameManager.SceneConfigId].MapRow;
            int _mapCol = _sceneModule.m_config.SceneConfigList[GameManager.SceneConfigId].MapCol;
			
			int DistancePlayer = -1;
			//当前距离最近的点
			int minrow = 0;
			int mincol = 0;

			int temprow;
			int tempcol;

			//player的位置
			int PRow = _sceneModule.m_player.m_playerPos.m_row;
			int PCol = _sceneModule.m_player.m_playerPos.m_col;

			//enemy当前的位置
			int playerRow = m_EnemyPosNew.m_row;
			int playerCol = m_EnemyPosNew.m_col;
			
            switch (EnemyType)
            {
            	//马
                case 1:
                    if(playerRow >= 1)
                    {
                        if(playerCol >= 2)
                        {
                            if(_sceneModule.m_mapData[playerRow - 1][playerCol - 2] != MapDataType.NONE
                                && (_sceneModule.m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (_sceneModule.m_mapData[playerRow][playerCol - 1] != MapDataType.GAOTAI)))
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
                            if(_sceneModule.m_mapData[playerRow - 1][playerCol + 2] != MapDataType.NONE
                                && (_sceneModule.m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (_sceneModule.m_mapData[playerRow][playerCol + 1] != MapDataType.GAOTAI)))
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
                            if (_sceneModule.m_mapData[playerRow - 2][playerCol - 1] != MapDataType.NONE
                                && (_sceneModule.m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (_sceneModule.m_mapData[playerRow - 1][playerCol] != MapDataType.GAOTAI)))
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
                            if (_sceneModule.m_mapData[playerRow - 2][playerCol + 1] != MapDataType.NONE
                                && (_sceneModule.m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (_sceneModule.m_mapData[playerRow - 1][playerCol] != MapDataType.GAOTAI)))
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
                            if (_sceneModule.m_mapData[playerRow + 1][playerCol - 2] != MapDataType.NONE
                                && (_sceneModule.m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (_sceneModule.m_mapData[playerRow][playerCol - 1] != MapDataType.GAOTAI)))
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
                            if (_sceneModule.m_mapData[playerRow + 1][playerCol + 2] != MapDataType.NONE
                                && (_sceneModule.m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (_sceneModule.m_mapData[playerRow][playerCol + 1] != MapDataType.GAOTAI)))
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
                            if (_sceneModule.m_mapData[playerRow + 2][playerCol - 1] != MapDataType.NONE
                                && (_sceneModule.m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (_sceneModule.m_mapData[playerRow + 1][playerCol] != MapDataType.GAOTAI)))
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
                            if (_sceneModule.m_mapData[playerRow + 2][playerCol + 1] != MapDataType.NONE
                                && (_sceneModule.m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (_sceneModule.m_mapData[playerRow + 1][playerCol] != MapDataType.GAOTAI)))
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
                            if (_sceneModule.m_mapData[playerRow - 2][playerCol - 2] != MapDataType.NONE
                                && (_sceneModule.m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (_sceneModule.m_mapData[playerRow - 1][playerCol - 1] != MapDataType.GAOTAI)))
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
                            if (_sceneModule.m_mapData[playerRow - 2][playerCol + 2] != MapDataType.NONE
                                && (_sceneModule.m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (_sceneModule.m_mapData[playerRow - 1][playerCol + 1] != MapDataType.GAOTAI)))
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
                            if (_sceneModule.m_mapData[playerRow + 2][playerCol - 2] != MapDataType.NONE
                                && (_sceneModule.m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (_sceneModule.m_mapData[playerRow + 1][playerCol - 1] != MapDataType.GAOTAI)))
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
                            if (_sceneModule.m_mapData[playerRow + 2][playerCol + 2] != MapDataType.NONE
                                && (_sceneModule.m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (_sceneModule.m_mapData[playerRow + 1][playerCol + 1] != MapDataType.GAOTAI)))
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
						if(playerCol >= 1 && _sceneModule.m_mapData[playerRow][playerCol - 1] != MapDataType.GAOTAI && _sceneModule.m_mapData[playerRow][playerCol - 1] != MapDataType.NONE && _sceneModule.m_EnemyList[playerRow][playerCol - 1] == null)
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
						
						if(playerCol + 1 < _mapCol && _sceneModule.m_mapData[playerRow][playerCol + 1] != MapDataType.GAOTAI && _sceneModule.m_mapData[playerRow][playerCol + 1] != MapDataType.NONE && _sceneModule.m_EnemyList[playerRow][playerCol + 1] == null)
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

						
						if(playerRow >= 1 && _sceneModule.m_mapData[playerRow - 1][playerCol] != MapDataType.GAOTAI && _sceneModule.m_mapData[playerRow - 1][playerCol] != MapDataType.NONE && _sceneModule.m_EnemyList[playerRow - 1][playerCol] == null)
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

						if(playerRow + 1 < _mapRow && _sceneModule.m_mapData[playerRow + 1][playerCol] != MapDataType.GAOTAI && _sceneModule.m_mapData[playerRow + 1][playerCol] != MapDataType.NONE && _sceneModule.m_EnemyList[playerRow + 1][playerCol] == null)
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
				MovePos(m_EnemyPosNew.m_row, m_EnemyPosNew.m_col);
			}
			else
			{
				_sceneModule.m_EnemyList[minrow][mincol] = _sceneModule.m_EnemyList[m_EnemyPosNew.m_row][m_EnemyPosNew.m_col];
				_sceneModule.m_EnemyList[m_EnemyPosNew.m_row][m_EnemyPosNew.m_col] = null;
				MovePos(minrow, mincol);
			}

        }
		
	}

}


