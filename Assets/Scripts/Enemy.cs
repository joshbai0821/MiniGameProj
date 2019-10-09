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
		public int EnemyType;     //���˵�����
		public int PosIsChange;   //λ���ڱ����Ƿ��Ѿ��ı����
		public MapPos m_EnemyPosOld;   //�ϵ�λ��
		public MapPos m_EnemyPosNew;    //�µ�λ��
        public bool m_EnemyIsMove;   //���Ŷ���

        private static float DiffX = 1f;
        private static float DiffZ = -1f;

        private struct JuUse
        {
            public JuUse(int begin, int bround, int change)
            {
                m_BeginPos = begin;
                m_BroundPos = bround;
                m_Change = change;
            }
            public int m_BeginPos;
            public int m_BroundPos;
            public int m_Change;
        }

        private void Awake()
		{
		}

		public void SetType(int iType)
		{
			EnemyType = iType;
		}

        public void DestroyObj()
        {
            GameObject.Destroy(this.gameObject);
        }

        public void SetStartPos(int row, int col)
		{
			m_EnemyPosOld.m_row = row;
			m_EnemyPosOld.m_col = col;

			m_EnemyPosNew.m_row = row;
			m_EnemyPosNew.m_col = col;

			transform.position = new Vector3(row * DiffX, 0f, col * DiffZ);
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
			//transform.position = new Vector3(m_EnemyPosNew.m_row * DiffX, 0f, m_EnemyPosNew.m_col * DiffZ);
		}

		//�����ȣ������õ�
		private bool PosExitChess(int row, int col, bool YuJiExist)
        {
            //enemy��
            SceneModule _sceneModule = (SceneModule)GameManager.GameManagerObj.GetComponent<GameManager>().GetModuleByName("SceneModule");
			if(_sceneModule.m_enemyList[row][col] != null )
			{
				return true;
			}

            //player��
            MapPos _pos = new MapPos();
            _sceneModule.GetPlayerPos(ref _pos);

            if (row == _pos.m_row && col == _pos.m_col)
			{
				return true;
			}

            //�ݼ���
            if (YuJiExist && _sceneModule.YuJiPos.m_row == row && _sceneModule.YuJiPos.m_col == col)
            {
                return true;
            }

            //����
            if(_sceneModule.m_mapData[row][col] == MapDataType.JUMATUI)
            {
                return true;
            }


            return false;
		}

        public void EnemyMove(int row, int col)
        {
            m_EnemyIsMove = true;
            SceneModule _sceneModule = (SceneModule)GameManager.GameManagerObj.GetComponent<GameManager>().GetModuleByName("SceneModule");
            float DiffX = 1f;
            float DiffZ = -1f;
            float _targetPosX = m_EnemyPosNew.m_row * DiffX;
            float _targetPosZ = m_EnemyPosNew.m_col * DiffZ;
            float _targetPosY = 0f;
            Sequence _sequence = DOTween.Sequence();

            switch (EnemyType)
            {
                case (int)ChessType.MA:
                    {
                        _targetPosY = 0f;
                        if (_sceneModule.m_mapData[row][col] == MapDataType.GAOTAI)
                        {
                            _targetPosY = 1.0f;
                        }
                        float _midTargetPosX = 0.0f;
                        float _midTargetPosZ = 0.0f;
                        if (m_EnemyPosNew.m_row == m_EnemyPosOld.m_row + 2)
                        {
                            _midTargetPosX = (m_EnemyPosOld.m_row + 1) * DiffX;
                            _midTargetPosZ = this.transform.position.z;
                        }
                        else if (m_EnemyPosNew.m_row == m_EnemyPosOld.m_row - 2)
                        {
                            _midTargetPosX = (m_EnemyPosOld.m_row - 1) * DiffX;
                            _midTargetPosZ = this.transform.position.z;
                        }
                        else if (m_EnemyPosNew.m_col == m_EnemyPosOld.m_col + 2)
                        {
                            _midTargetPosX = this.transform.position.x;
                            _midTargetPosZ = (m_EnemyPosOld.m_col + 1) * DiffZ;
                        }
                        else if (m_EnemyPosNew.m_col == m_EnemyPosOld.m_col - 2)
                        {
                            _midTargetPosX = this.transform.position.x;
                            _midTargetPosZ = (m_EnemyPosOld.m_col - 1) * DiffZ;
                        }
                        _sequence.Append(transform.DOMove(new Vector3(_midTargetPosX, this.transform.position.y, _midTargetPosZ), 0.3f).SetEase(_sceneModule.m_player.m_maCurve1));
                        _sequence.Append(transform.DOJump(new Vector3(_targetPosX, _targetPosY, _targetPosZ), 0.4f, 1, 0.5f).SetEase(_sceneModule.m_player.m_maCurve2));
                    }
                    break;
                case (int)ChessType.XIANG:
                    _sequence.Append(transform.DOJump(new Vector3(_targetPosX, _targetPosY, _targetPosZ), 0.4f, 1, 0.5f).SetEase(_sceneModule.m_player.m_xiangCurve));
                    break;
                case (int)ChessType.SHI:
                    _sequence.Append(transform.DOMove(new Vector3(_targetPosX, _targetPosY, _targetPosZ), 0.4f).SetEase(_sceneModule.m_player.m_juCurve));
                    break;
                case (int)ChessType.JU:
                    _sequence.Append(transform.DOMove(new Vector3(_targetPosX, _targetPosY, _targetPosZ), 0.4f).SetEase(_sceneModule.m_player.m_juCurve));
                    break;
            }

            _sequence.AppendCallback(MoveEnd);
            _sequence.SetAutoKill(true);

        }

        private void MoveEnd()
        {
            //m_state = State.Idle;
            //m_move = false;
            //SceneModule _sceneModule = (SceneModule)GameManager.GameManagerObj.GetComponent<GameManager>().GetModuleByName("SceneModule");
            //_sceneModule.WaitNpc();
            //EventManager.SendEvent(HLEventId.PLAYER_END_MOVE, null);
        }

        //�����ܷ��ߵ�ʱ������ж��ݼ��Ƿ�ס��
        //���˳Ե�ʱ�򣬳��ݼ��������ȼ�һ���ߣ����ǿ����ݼ�
        //1 �����ӣ� 2�Ը��ӣ� 0��û����
        public int GetEnemyNextPos(bool YuJiExist)
        {
        	SceneModule _sceneModule = (SceneModule)GameManager.GameManagerObj.GetComponent<GameManager>().GetModuleByName("SceneModule");
			//�ж�,�Ƿ��ǿ��ߵĵ�
            int _mapRow = _sceneModule.m_config.SceneConfigList[GameManager.SceneConfigId].MapRow;
            int _mapCol = _sceneModule.m_config.SceneConfigList[GameManager.SceneConfigId].MapCol;
			
			int DistancePlayer = -1;
			//��ǰ��������ĵ�
			int minrow = 0;
			int mincol = 0;

            //����Ҫ�ߵ�λ��
			int temprow;
			int tempcol;

            MapPos _pos = new MapPos();
            _sceneModule.GetPlayerPos(ref _pos);

            //������
            int PRow = -1;
            int PCol = -1;

            //�����ӣ����ݼ�ʱ�������
            int SecondRow = -1;
            int SecondCol = -1;

            int RetValue = 0;

            if (YuJiExist)
            {
                PRow = _sceneModule.YuJiPos.m_row;
                PCol = _sceneModule.YuJiPos.m_col;

                SecondRow = _pos.m_row;
                SecondCol = _pos.m_col;
            }
            else
            {
                //��player��λ��
                PRow = _pos.m_row;
			    PCol = _pos.m_col;
            }


			//enemy��ǰ��λ��
			int playerRow = m_EnemyPosNew.m_row;
			int playerCol = m_EnemyPosNew.m_col;
			
            switch (EnemyType)
            {
            	//��
                case (int)ChessType.MA:
                    if(playerRow >= 1)
                    {
                        if(playerCol >= 2)
                        {
                            if ((_sceneModule.m_mapData[playerRow - 1][playerCol - 2] != MapDataType.NONE
                                && _sceneModule.m_enemyList[playerRow - 1][playerCol - 2] == null
                                && _sceneModule.m_mapData[playerRow - 1][playerCol - 2] != MapDataType.JUMATUI)
                                && (_sceneModule.m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (_sceneModule.m_mapData[playerRow][playerCol - 1] != MapDataType.GAOTAI)))
                            {
                            	if(PosExitChess(playerRow,playerCol - 1, YuJiExist) == false)
                            	{
                            		temprow = playerRow - 1;
									tempcol = playerCol - 2;

                                    //�ܷ�Ը���
                                    if (YuJiExist && temprow == SecondRow && tempcol == SecondCol)
                                    {
                                        RetValue = 2;
                                        DistancePlayer = (PRow - temprow) * (PRow - temprow) + (tempcol - PCol) * (tempcol - PCol);
                                        minrow = temprow;
                                        mincol = tempcol;
                                        break;
                                    }
									else if(DistancePlayer == -1)
									{
                                        //��һ������ĵ�
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
                            if((_sceneModule.m_mapData[playerRow - 1][playerCol + 2] != MapDataType.NONE
                                && _sceneModule.m_enemyList[playerRow - 1][playerCol + 2] == null
                                && _sceneModule.m_mapData[playerRow - 1][playerCol + 2] != MapDataType.JUMATUI)
                                && (_sceneModule.m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (_sceneModule.m_mapData[playerRow][playerCol + 1] != MapDataType.GAOTAI)))
                            {
                                if(PosExitChess(playerRow,playerCol + 1, YuJiExist) == false)
								{
									temprow = playerRow - 1;
									tempcol = playerCol + 2;
                                    //��һ������ĵ�
                                    //�ܷ�Ը���
                                    if (YuJiExist && temprow == SecondRow && tempcol == SecondCol)
                                    {
                                        RetValue = 2;
                                        DistancePlayer = (PRow - temprow) * (PRow - temprow) + (tempcol - PCol) * (tempcol - PCol);
                                        minrow = temprow;
                                        mincol = tempcol;
                                        break;
                                    }
                                    else if (DistancePlayer == -1)
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
                            if ((_sceneModule.m_mapData[playerRow - 2][playerCol - 1] != MapDataType.NONE
                                && _sceneModule.m_enemyList[playerRow - 2][playerCol - 1] == null
                                && _sceneModule.m_mapData[playerRow - 2][playerCol - 1] != MapDataType.JUMATUI)
                                && (_sceneModule.m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (_sceneModule.m_mapData[playerRow - 1][playerCol] != MapDataType.GAOTAI)))
                            {
                                if(PosExitChess(playerRow - 1,playerCol, YuJiExist) == false)
								{
									temprow = playerRow - 2;
									tempcol = playerCol - 1;
                                    //��һ������ĵ�
                                    //�ܷ�Ը���
                                    if (YuJiExist && temprow == SecondRow && tempcol == SecondCol)
                                    {
                                        RetValue = 2;
                                        DistancePlayer = (PRow - temprow) * (PRow - temprow) + (tempcol - PCol) * (tempcol - PCol);
                                        minrow = temprow;
                                        mincol = tempcol;
                                        break;
                                    }
                                    else if (DistancePlayer == -1)
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
                        if (playerCol + 1 < _mapCol)
                        {
                            if ((_sceneModule.m_mapData[playerRow - 2][playerCol + 1] != MapDataType.NONE
                                && _sceneModule.m_enemyList[playerRow - 2][playerCol + 1] == null
                                && _sceneModule.m_mapData[playerRow - 2][playerCol + 1] != MapDataType.JUMATUI)
                                && (_sceneModule.m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (_sceneModule.m_mapData[playerRow - 1][playerCol] != MapDataType.GAOTAI)))
                            {
								if(PosExitChess(playerRow - 1,playerCol, YuJiExist) == false)
								{
									temprow = playerRow - 2;
									tempcol = playerCol + 1;
                                    //��һ������ĵ�
                                    //�ܷ�Ը���
                                    if (YuJiExist && temprow == SecondRow && tempcol == SecondCol)
                                    {
                                        RetValue = 2;
                                        DistancePlayer = (PRow - temprow) * (PRow - temprow) + (tempcol - PCol) * (tempcol - PCol);
                                        minrow = temprow;
                                        mincol = tempcol;
                                        break;
                                    }
                                    else if (DistancePlayer == -1)
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
                            if ((_sceneModule.m_mapData[playerRow + 1][playerCol - 2] != MapDataType.NONE
                                && _sceneModule.m_enemyList[playerRow + 1][playerCol - 2] == null
                                && _sceneModule.m_mapData[playerRow + 1][playerCol - 2] != MapDataType.JUMATUI)
                                && (_sceneModule.m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (_sceneModule.m_mapData[playerRow][playerCol - 1] != MapDataType.GAOTAI)))
                            {
                                if(PosExitChess(playerRow,playerCol - 1, YuJiExist) == false)
								{
									temprow = playerRow + 1;
									tempcol = playerCol - 2;
                                    //��һ������ĵ�
                                    //�ܷ�Ը���
                                    if (YuJiExist && temprow == SecondRow && tempcol == SecondCol)
                                    {
                                        RetValue = 2;
                                        DistancePlayer = (PRow - temprow) * (PRow - temprow) + (tempcol - PCol) * (tempcol - PCol);
                                        minrow = temprow;
                                        mincol = tempcol;
                                        break;
                                    }
                                    else if (DistancePlayer == -1)
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
                            if ((_sceneModule.m_mapData[playerRow + 1][playerCol + 2] != MapDataType.NONE
                                && _sceneModule.m_enemyList[playerRow + 1][playerCol + 2] == null
                                && _sceneModule.m_mapData[playerRow + 1][playerCol + 2] != MapDataType.JUMATUI)
                                && (_sceneModule.m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (_sceneModule.m_mapData[playerRow][playerCol + 1] != MapDataType.GAOTAI)))
                            {
                                if(PosExitChess(playerRow,playerCol + 1, YuJiExist) == false)
								{
									temprow = playerRow + 1;
									tempcol = playerCol + 2;
                                    //��һ������ĵ�
                                    //�ܷ�Ը���
                                    if (YuJiExist && temprow == SecondRow && tempcol == SecondCol)
                                    {
                                        RetValue = 2;
                                        DistancePlayer = (PRow - temprow) * (PRow - temprow) + (tempcol - PCol) * (tempcol - PCol);
                                        minrow = temprow;
                                        mincol = tempcol;
                                        break;
                                    }
                                    else if (DistancePlayer == -1)
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
                            if ((_sceneModule.m_mapData[playerRow + 2][playerCol - 1] != MapDataType.NONE
                                && _sceneModule.m_enemyList[playerRow + 2][playerCol - 1] == null
                                && _sceneModule.m_mapData[playerRow + 2][playerCol - 1] != MapDataType.JUMATUI)
                                && (_sceneModule.m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (_sceneModule.m_mapData[playerRow + 1][playerCol] != MapDataType.GAOTAI)))
                            {
                                if(PosExitChess(playerRow + 1,playerCol, YuJiExist) == false)
								{
									temprow = playerRow + 2;
									tempcol = playerCol - 1;
                                    //��һ������ĵ�
                                    //�ܷ�Ը���
                                    if (YuJiExist && temprow == SecondRow && tempcol == SecondCol)
                                    {
                                        RetValue = 2;
                                        DistancePlayer = (PRow - temprow) * (PRow - temprow) + (tempcol - PCol) * (tempcol - PCol);
                                        minrow = temprow;
                                        mincol = tempcol;
                                        break;
                                    }
                                    else if (DistancePlayer == -1)
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
                        if (playerCol + 1 < _mapCol)
                        {
                            if ((_sceneModule.m_mapData[playerRow + 2][playerCol + 1] != MapDataType.NONE
                                && _sceneModule.m_enemyList[playerRow + 2][playerCol + 1] == null
                                && _sceneModule.m_mapData[playerRow + 2][playerCol + 1] != MapDataType.JUMATUI)
                                && (_sceneModule.m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (_sceneModule.m_mapData[playerRow + 1][playerCol] != MapDataType.GAOTAI)))
                            {
                                if(PosExitChess(playerRow + 1,playerCol, YuJiExist) == false)
								{
									temprow = playerRow + 2;
									tempcol = playerCol + 1;
                                    //��һ������ĵ�
                                    //�ܷ�Ը���
                                    if (YuJiExist && temprow == SecondRow && tempcol == SecondCol)
                                    {
                                        RetValue = 2;
                                        DistancePlayer = (PRow - temprow) * (PRow - temprow) + (tempcol - PCol) * (tempcol - PCol);
                                        minrow = temprow;
                                        mincol = tempcol;
                                        break;
                                    }
                                    else if (DistancePlayer == -1)
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
                case (int)ChessType.XIANG:
                    if(playerRow >= 2)
                    {
                        if(playerCol >= 2)
                        {
                            if ((_sceneModule.m_mapData[playerRow - 2][playerCol - 2] != MapDataType.NONE
                                && _sceneModule.m_enemyList[playerRow - 2][playerCol - 2] == null
                                && _sceneModule.m_mapData[playerRow - 2][playerCol - 2] != MapDataType.JUMATUI)
                                && (_sceneModule.m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (_sceneModule.m_mapData[playerRow - 1][playerCol - 1] != MapDataType.GAOTAI)))
                            {
                                if(PosExitChess(playerRow - 1,playerCol -1, YuJiExist) == false)
								{
									temprow = playerRow - 2;
									tempcol = playerCol - 2;
                                    //��һ������ĵ�
                                    //�ܷ�Ը���
                                    if (YuJiExist && temprow == SecondRow && tempcol == SecondCol)
                                    {
                                        RetValue = 2;
                                        DistancePlayer = (PRow - temprow) * (PRow - temprow) + (tempcol - PCol) * (tempcol - PCol);
                                        minrow = temprow;
                                        mincol = tempcol;
                                        break;
                                    }
                                    else if (DistancePlayer == -1)
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
                            if ((_sceneModule.m_mapData[playerRow - 2][playerCol + 2] != MapDataType.NONE
                                && _sceneModule.m_enemyList[playerRow - 2][playerCol + 2] == null
                                && _sceneModule.m_mapData[playerRow - 2][playerCol + 2] != MapDataType.JUMATUI)
                                && (_sceneModule.m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (_sceneModule.m_mapData[playerRow - 1][playerCol + 1] != MapDataType.GAOTAI)))
                            {
								if(PosExitChess(playerRow - 1,playerCol + 1, YuJiExist) == false)
								{
									temprow = playerRow - 2;
									tempcol = playerCol + 2;
                                    //��һ������ĵ�
                                    //�ܷ�Ը���
                                    if (YuJiExist && temprow == SecondRow && tempcol == SecondCol)
                                    {
                                        RetValue = 2;
                                        DistancePlayer = (PRow - temprow) * (PRow - temprow) + (tempcol - PCol) * (tempcol - PCol);
                                        minrow = temprow;
                                        mincol = tempcol;
                                        break;
                                    }
                                    else if (DistancePlayer == -1)
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
                        if (playerCol >= 2)
                        {
                            if ((_sceneModule.m_mapData[playerRow + 2][playerCol - 2] != MapDataType.NONE
                                && _sceneModule.m_enemyList[playerRow + 2][playerCol - 2] == null
                                && _sceneModule.m_mapData[playerRow + 2][playerCol - 2] != MapDataType.JUMATUI)
                                && (_sceneModule.m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (_sceneModule.m_mapData[playerRow + 1][playerCol - 1] != MapDataType.GAOTAI)))
                            {
								if(PosExitChess(playerRow + 1,playerCol -1, YuJiExist) == false)
								{
									temprow = playerRow + 2;
									tempcol = playerCol - 2;
                                    //��һ������ĵ�
                                    //�ܷ�Ը���
                                    if (YuJiExist && temprow == SecondRow && tempcol == SecondCol)
                                    {
                                        RetValue = 2;
                                        DistancePlayer = (PRow - temprow) * (PRow - temprow) + (tempcol - PCol) * (tempcol - PCol);
                                        minrow = temprow;
                                        mincol = tempcol;
                                        break;
                                    }
                                    else if (DistancePlayer == -1)
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
                            if ((_sceneModule.m_mapData[playerRow + 2][playerCol + 2] != MapDataType.NONE
                                && _sceneModule.m_enemyList[playerRow + 2][playerCol + 2] == null
                                && _sceneModule.m_mapData[playerRow + 2][playerCol + 2] != MapDataType.JUMATUI)
                                && (_sceneModule.m_mapData[playerRow][playerCol] == MapDataType.GAOTAI ||
                                (_sceneModule.m_mapData[playerRow + 1][playerCol + 1] != MapDataType.GAOTAI)))
                            {
								if(PosExitChess(playerRow + 1,playerCol + 1, YuJiExist) == false)
								{
									temprow = playerRow + 2;
									tempcol = playerCol + 2;
                                    //��һ������ĵ�
                                    //�ܷ�Ը���
                                    if (YuJiExist && temprow == SecondRow && tempcol == SecondCol)
                                    {
                                        RetValue = 2;
                                        DistancePlayer = (PRow - temprow) * (PRow - temprow) + (tempcol - PCol) * (tempcol - PCol);
                                        minrow = temprow;
                                        mincol = tempcol;
                                        break;
                                    }
                                    else if (DistancePlayer == -1)
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
                case (int)ChessType.SHI:
                    if (playerRow >= 1 && _sceneModule.m_mapData[playerRow - 1][playerCol] != MapDataType.GAOTAI && _sceneModule.m_mapData[playerRow - 1][playerCol] != MapDataType.JUMATUI && _sceneModule.m_mapData[playerRow - 1][playerCol] != MapDataType.NONE && _sceneModule.m_enemyList[playerRow - 1][playerCol] == null)
                    {
                        temprow = playerRow - 1;
                        tempcol = playerCol;
                        //��һ������ĵ�
                        //�ܷ�Ը���
                        if (YuJiExist && temprow == SecondRow && tempcol == SecondCol)
                        {
                            RetValue = 2;
                            DistancePlayer = (PRow - temprow) * (PRow - temprow) + (tempcol - PCol) * (tempcol - PCol);
                            minrow = temprow;
                            mincol = tempcol;
                            break;
                        }
                        else if (DistancePlayer == -1)
                        {
                            DistancePlayer = (PRow - temprow) * (PRow - temprow) + (tempcol - PCol) * (tempcol - PCol);
                            minrow = temprow;
                            mincol = tempcol;
                        }
                        else if (DistancePlayer > (PRow - temprow) * (PRow - temprow) + (tempcol - PCol) * (tempcol - PCol))
                        {
                            DistancePlayer = (PRow - temprow) * (PRow - temprow) + (tempcol - PCol) * (tempcol - PCol);
                            minrow = temprow;
                            mincol = tempcol;
                        }

                    }

                    if (playerRow + 1 < _mapRow && _sceneModule.m_mapData[playerRow + 1][playerCol] != MapDataType.GAOTAI && _sceneModule.m_mapData[playerRow + 1][playerCol] != MapDataType.JUMATUI && _sceneModule.m_mapData[playerRow + 1][playerCol] != MapDataType.NONE && _sceneModule.m_enemyList[playerRow + 1][playerCol] == null)
                    {
                        temprow = playerRow + 1;
                        tempcol = playerCol;
                        //��һ������ĵ�
                        //�ܷ�Ը���
                        if (YuJiExist && temprow == SecondRow && tempcol == SecondCol)
                        {
                            RetValue = 2;
                            DistancePlayer = (PRow - temprow) * (PRow - temprow) + (tempcol - PCol) * (tempcol - PCol);
                            minrow = temprow;
                            mincol = tempcol;
                            break;
                        }
                        else if (DistancePlayer == -1)
                        {
                            DistancePlayer = (PRow - temprow) * (PRow - temprow) + (tempcol - PCol) * (tempcol - PCol);
                            minrow = temprow;
                            mincol = tempcol;
                        }
                        else if (DistancePlayer > (PRow - temprow) * (PRow - temprow) + (tempcol - PCol) * (tempcol - PCol))
                        {
                            DistancePlayer = (PRow - temprow) * (PRow - temprow) + (tempcol - PCol) * (tempcol - PCol);
                            minrow = temprow;
                            mincol = tempcol;
                        }

                    }

                    if (playerCol >= 1 && _sceneModule.m_mapData[playerRow][playerCol - 1] != MapDataType.GAOTAI && _sceneModule.m_mapData[playerRow][playerCol - 1] != MapDataType.JUMATUI && _sceneModule.m_mapData[playerRow][playerCol - 1] != MapDataType.NONE && _sceneModule.m_enemyList[playerRow][playerCol - 1] == null)
						{
							temprow = playerRow;
							tempcol = playerCol - 1;
                        //��һ������ĵ�
                        //�ܷ�Ը���
                            if (YuJiExist && temprow == SecondRow && tempcol == SecondCol)
                            {
                                RetValue = 2;
                                DistancePlayer = (PRow - temprow) * (PRow - temprow) + (tempcol - PCol) * (tempcol - PCol);
                                minrow = temprow;
                                mincol = tempcol;
                                break;
                            }
                            else if (DistancePlayer == -1)
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
						
						if(playerCol + 1 < _mapCol && _sceneModule.m_mapData[playerRow][playerCol + 1] != MapDataType.GAOTAI && _sceneModule.m_mapData[playerRow][playerCol + 1] != MapDataType.JUMATUI && _sceneModule.m_mapData[playerRow][playerCol + 1] != MapDataType.NONE && _sceneModule.m_enemyList[playerRow][playerCol + 1] == null)
						{
							temprow = playerRow;
							tempcol = playerCol + 1;
                        //��һ������ĵ�
                        //�ܷ�Ը���
                            if (YuJiExist && temprow == SecondRow && tempcol == SecondCol)
                            {
                                RetValue = 2;
                                DistancePlayer = (PRow - temprow) * (PRow - temprow) + (tempcol - PCol) * (tempcol - PCol);
                                minrow = temprow;
                                mincol = tempcol;
                                break;
                            }
                            else if (DistancePlayer == -1)
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
                case (int)ChessType.JU:
                    //���ߵı߽��
                    JuUse TempJu1 = new JuUse(m_EnemyPosNew.m_row, 0, -1);
                    JuUse TempJu2 = new JuUse(m_EnemyPosNew.m_row, _mapRow - 1, 1);
                    JuUse TempJu3 = new JuUse(m_EnemyPosNew.m_col, 0, -1);
                    JuUse TempJu4 = new JuUse(m_EnemyPosNew.m_col, _mapCol - 1, 1);

                    List<JuUse> Pos = new List<JuUse>();
                    Pos.Add(TempJu1);
                    Pos.Add(TempJu2);
                    Pos.Add(TempJu3);
                    Pos.Add(TempJu4);
                    //���ݵз����Ӻ͵����ж�row��col����ֵ,��4�������һ����
                    for (int _i = 0; _i < Pos.Count; _i++)
                    {
                        JuUse temp = Pos[_i];
                        for (temp.m_BeginPos += temp.m_Change; temp.m_BeginPos * temp.m_Change <= temp.m_BroundPos; temp.m_BeginPos += temp.m_Change)
                        {
                            //row�䣬col��������
                            if (_i < 2 && (_sceneModule.m_enemyList[temp.m_BeginPos][m_EnemyPosNew.m_col] != null || _sceneModule.m_mapData[temp.m_BeginPos][m_EnemyPosNew.m_col] != _sceneModule.m_mapData[m_EnemyPosNew.m_row][m_EnemyPosNew.m_col]))
                            {
                                //����ͼ���ߵ����赲��������ı�row
                                temp.m_BeginPos -= temp.m_Change;
                                break;
                            }
                            //col�䣬row��������
                            if (_i >= 2 && (_sceneModule.m_enemyList[m_EnemyPosNew.m_row][temp.m_BeginPos] != null || _sceneModule.m_mapData[m_EnemyPosNew.m_row][temp.m_BeginPos] != _sceneModule.m_mapData[m_EnemyPosNew.m_row][m_EnemyPosNew.m_col]))
                            {
                                //����ͼ���ߵ����赲��������ı�col
                                temp.m_BeginPos -= temp.m_Change;
                                break;
                            }
                        }
                        //�߽�����ߵ����
                        if (temp.m_BeginPos * temp.m_Change > temp.m_BroundPos)
                        {
                            temp.m_BeginPos = temp.m_BroundPos;
                        }
                        Pos[_i] = temp;
                    }

                    //�ݼ������ܳԸ��ӵ������col�������߻���  row��������
                    if (YuJiExist && ((SecondRow == m_EnemyPosNew.m_row && SecondCol >= Pos[2].m_BeginPos && SecondCol <= Pos[3].m_BeginPos) || (SecondCol == m_EnemyPosNew.m_col && SecondRow >= Pos[0].m_BeginPos && SecondRow <= Pos[1].m_BeginPos)))
                    {
                        temprow = SecondRow;
                        tempcol = SecondCol;
                        RetValue = 2;
                        DistancePlayer = 0;
                        minrow = temprow;
                        mincol = tempcol;
                        break;
                    }
                    else
                    {
                        //���Ը��ӵ������һ�������������λ��
                        //1 �ı�row���������һ�������
                        temprow = PRow;
                        tempcol = m_EnemyPosNew.m_col;
                        if (temprow >= m_EnemyPosNew.m_row)
                        {
                            temprow = temprow > Pos[1].m_BeginPos ? Pos[1].m_BeginPos : temprow;
                        }
                        else
                        {
                            temprow = temprow < Pos[0].m_BeginPos ? Pos[0].m_BeginPos : temprow;
                        }
                        DistancePlayer = (PRow - temprow) * (PRow - temprow) + (tempcol - PCol) * (tempcol - PCol);
                        minrow = temprow;
                        mincol = tempcol;
                        //2 �ı�col���������һ�������
                        temprow = m_EnemyPosNew.m_row;
                        tempcol = PCol;
                        if (tempcol >= m_EnemyPosNew.m_col)
                        {
                            tempcol = tempcol > Pos[3].m_BeginPos ? Pos[3].m_BeginPos : tempcol;
                        }
                        else
                        {
                            tempcol = tempcol < Pos[2].m_BeginPos ? Pos[2].m_BeginPos : tempcol;
                        }

                        if (DistancePlayer > (PRow - temprow) * (PRow - temprow) + (tempcol - PCol) * (tempcol - PCol))
                        {
                            DistancePlayer = (PRow - temprow) * (PRow - temprow) + (tempcol - PCol) * (tempcol - PCol);
                            minrow = temprow;
                            mincol = tempcol;
                        }
                    }
                   
                    break;
            }

            //Debug.Log(string.Format("minx:{0}, miny:{1}",minrow, mincol));
            if (DistancePlayer == -1)
            {
                //û�е������
                MovePos(m_EnemyPosNew.m_row, m_EnemyPosNew.m_col);
            }
            else
            {
                _sceneModule.m_enemyList[minrow][mincol] = _sceneModule.m_enemyList[m_EnemyPosNew.m_row][m_EnemyPosNew.m_col];
                _sceneModule.m_enemyList[m_EnemyPosNew.m_row][m_EnemyPosNew.m_col] = null;
                MovePos(minrow, mincol);
                EnemyMove(minrow, mincol);
                PosIsChange = 1;
                if (RetValue == 2)
                {
                    return 2;
                }
                else if (DistancePlayer == 0)
                {
                    return 1;
                }
            }

            return 0;

        }
		
	}

}


