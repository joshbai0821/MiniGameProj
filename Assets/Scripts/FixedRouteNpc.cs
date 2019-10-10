using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

namespace MiniProj
{
    public class FixedRouteNpc : MonoBehaviour
    {
        private static float DiffX = 1f;
        private static float DiffZ = -1.0f;
        private MapPos m_playerPos;
        public List<MapPos> m_routePosList;

        public int m_stepPerRound = 2;
        public int m_curStep = 0;
        private int m_curRoundStep = 0;

        private void Awake()
        {
            m_curStep = 1;
            EventManager.RegisterEvent(HLEventId.PLAYER_END_MOVE, this.GetHashCode(), FollowPlayer);
        }

        public void DestroyObj()
        {
            if (this != null)
            {
                GameObject.Destroy(this.gameObject);
            }
        }

        private void OnDestroy()
        {
            EventManager.UnregisterEvent(HLEventId.PLAYER_END_MOVE, this.GetHashCode());
        }

        public void SetPosition(int row, int col)
        {
            m_playerPos.m_row = row;
            m_playerPos.m_col = col;
            transform.position = new Vector3(row * DiffX, 0f, col * DiffZ);
        }

        private void FollowPlayer(EventArgs args)
        {
            DoOneStep();
        }

        private void DoOneStep()
        {
            if(m_curStep == m_routePosList.Count)
            {
                m_curRoundStep = 0;
                EventManager.SendEvent(HLEventId.NPC_END_MOVE, null);
            }
            SceneModule _sceneModule = (SceneModule)GameManager.GameManagerObj.GetComponent<GameManager>().GetModuleByName("SceneModule");
            if (m_curRoundStep < m_stepPerRound - 1)
            {
                MapPos _playPos = new MapPos();
                _sceneModule.GetPlayerPos(ref _playPos);
                int _col = m_routePosList[m_curStep].m_col;
                int _row = m_routePosList[m_curStep].m_row;
                if (_row == m_playerPos.m_row && _col == m_playerPos.m_col)
                {
                    ++m_curRoundStep;
                    ++m_curStep;
                    DoOneStep();
                }
                else if ((_col != _playPos.m_col || _row != _playPos.m_row) && _sceneModule.m_enemyList[_row][_col] == null && _sceneModule.Data[_row][_col] != MapDataType.NONE)
                {
                    _sceneModule.m_npcList[_row][_col] = _sceneModule.m_npcList[m_playerPos.m_row][m_playerPos.m_col];
                    _sceneModule.m_npcList[m_playerPos.m_row][m_playerPos.m_col] = null;
                    m_playerPos.m_row = _row;
                    m_playerPos.m_col = _col;
                    Sequence _sequence = DOTween.Sequence();
                    _sequence.Append(transform.DOMove(new Vector3(_row * DiffX, 0f, _col * DiffZ), 0.5f));
                    _sequence.onComplete += DoOneStep;
                    _sequence.SetAutoKill(true);
                    ++m_curRoundStep;
                    ++m_curStep;
                }
                else
                {
                    m_curRoundStep = 0;
                    EventManager.SendEvent(HLEventId.NPC_END_MOVE, null);                  
                }
                
            }
            else
            {
                MapPos _playPos = new MapPos();
                _sceneModule.GetPlayerPos(ref _playPos);
                int _col = m_routePosList[m_curStep].m_col;
                int _row = m_routePosList[m_curStep].m_row;
                if(_row == m_playerPos.m_row && _col == m_playerPos.m_col)
                {
                    m_curRoundStep = 0;
                    ++m_curStep;
                    EventManager.SendEvent(HLEventId.NPC_END_MOVE, null);
                }
                else if ((_col != _playPos.m_col || _row != _playPos.m_row) && _sceneModule.m_enemyList[_row][_col] == null && _sceneModule.Data[_row][_col] != MapDataType.NONE)
                {
                    _sceneModule.m_npcList[_row][_col] = _sceneModule.m_npcList[m_playerPos.m_row][m_playerPos.m_col];
                    _sceneModule.m_npcList[m_playerPos.m_row][m_playerPos.m_col] = null;
                    m_playerPos.m_row = _row;
                    m_playerPos.m_col = _col;
                    m_curRoundStep = 0;
                    Sequence _sequence = DOTween.Sequence();
                    _sequence.Append(transform.DOMove(new Vector3(_row * DiffX, 0f, _col * DiffZ), 0.5f));
                    _sequence.SetAutoKill(true);
                    _sequence.onComplete += NpcEndMoveCallBack;
                    ++m_curStep;
                }
                else
                {
                    m_curRoundStep = 0;
                    EventManager.SendEvent(HLEventId.NPC_END_MOVE, null);
                }
                
            }
            
        }

        private void NpcEndMoveCallBack()
        {
            EventManager.SendEvent(HLEventId.NPC_END_MOVE, null);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Enemy")
            {
                Enemy _enemy = other.gameObject.GetComponent<Enemy>();
                if(_enemy != null)
                {
                    MapPos _mapPos = _enemy.m_EnemyPosNew;
                    if(_mapPos.m_row == m_playerPos.m_row && _mapPos.m_col == m_playerPos.m_col)
                    {
                        Invoke("delayLoadFailpanel", 0.7f);
                        gameObject.SetActive(false);
                    }
                }
                
            }
        }

        private void delayLoadFailpanel()
        {
            SceneModule _sceneModule = (SceneModule)GameManager.GameManagerObj.GetComponent<GameManager>().GetModuleByName("SceneModule");
            if (!_sceneModule.m_sceneWin)
            {
                _sceneModule.LoadFailpanel("虞姬阵亡");
                DestroyObj();
            }
        }
    }

    

}
