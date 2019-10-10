using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

namespace MiniProj
{
    enum State
    {
        Idle = 0,
        UseSkill = 1,
        Move = 2,
    }
    public class Player : MonoBehaviour
    {
        [SerializeField]
        public MapPos m_playerPos;
        private ParticleSystem m_playereff;
        private ParticleSystem m_hiteff;
        public MapPos Pos
        {
            get { return m_playerPos; }
        }

        private SkillId m_skillId;
        private State m_state;
        private bool m_move;

        private static float DiffX = 1f;
        private static float DiffZ = -1f;

        public AnimationCurve m_juCurve;
        public AnimationCurve m_maCurve1;
        public AnimationCurve m_maCurve2;
        public AnimationCurve m_xiangCurve;

        private void Awake()
        {
            m_move = true;
            m_skillId = SkillId.NONE;
            m_state = State.Idle;
            m_playereff = transform.Find("Pawndown").GetComponentInChildren<ParticleSystem>();
            m_hiteff = transform.Find("SoftFightAction2").GetComponent<ParticleSystem>();
            EventManager.RegisterEvent(HLEventId.USE_SKILL, this.GetHashCode(), UseSkill);
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
            EventManager.UnregisterEvent(HLEventId.USE_SKILL, this.GetHashCode());
        }

        public void SetStartPosition(int row, int col)
        {
            m_playerPos.m_row = row;
            m_playerPos.m_col = col;
            transform.position = new Vector3(row * DiffX, 0f, col * DiffZ);
        }

        public bool IsReady()
        {
            return m_state != State.Move    ;
        }

        private bool PosExitChess(int row, int col)
        {
            SceneModule _sceneModule = (SceneModule)GameManager.GameManagerObj.GetComponent<GameManager>().GetModuleByName("SceneModule");
            if (_sceneModule.m_enemyList[row][col] != null)
            {
                return true;
            }
            if (_sceneModule.m_npcList[row][col] != null)
            {
                return true;
            }
            return false;
        }

        private bool CheckCanMove(int targetRow, int targetCol, MapDataType targetData)
        {
            SceneModule _sceneModule = (SceneModule)GameManager.GameManagerObj.GetComponent<GameManager>().GetModuleByName("SceneModule");
            int _dataRow = _sceneModule.getMapDataRow();
            int _dataCol = _sceneModule.getMapDataCol();
            bool _ret = false;
            if(m_skillId == SkillId.NONE)
            {
                return _ret;
            }
            if(targetData == MapDataType.NONE || targetData == MapDataType.JUMATUI)
            {
                return _ret;
            }
            if(targetRow < 0 || targetRow >= _dataRow || targetCol < 0 || targetCol >= _dataCol)
            {
                return _ret;
            }
            switch(m_skillId)
            {
                case SkillId.JU:
                    if(targetRow == m_playerPos.m_row)
                    {
                        if (_sceneModule.Data[targetRow][targetCol] == MapDataType.NEWMODE)
                        {
                            return _ret;
                        }
                        if(targetCol > m_playerPos.m_col)
                        {
                            for (int _i = m_playerPos.m_col + 1; _i <= targetCol; _i++)
                            {
                                if(_sceneModule.Data[m_playerPos.m_row][_i] == MapDataType.GAOTAI 
                                    || _sceneModule.Data[m_playerPos.m_row][_i] == MapDataType.NONE
                                    || _sceneModule.Data[m_playerPos.m_row][_i] == MapDataType.JUMATUI
                                    || _sceneModule.m_npcList[m_playerPos.m_row][_i] != null)
                                {
                                    return _ret;
                                }
                                else
                                {
                                    if (_sceneModule.m_enemyList[m_playerPos.m_row][_i] != null && _i != targetCol)
                                    {
                                        return _ret;
                                    }
                                }
                            }
                            _ret = true;
                            return _ret;
                        }
                        else if(targetCol < m_playerPos.m_col)
                        {
                            for (int _i = m_playerPos.m_col - 1; _i >= targetCol; _i--)
                            {
                                if (_sceneModule.Data[m_playerPos.m_row][_i] == MapDataType.GAOTAI
                                    || _sceneModule.Data[m_playerPos.m_row][_i] == MapDataType.NONE
                                    || _sceneModule.Data[m_playerPos.m_row][_i] == MapDataType.JUMATUI
                                    || _sceneModule.m_npcList[m_playerPos.m_row][_i] != null)
                                {
                                    return _ret; ;
                                }
                                else
                                {
                                    if(_sceneModule.m_npcList[m_playerPos.m_row][_i] != null && _i != targetCol)
                                    {
                                        return _ret;
                                    }
                                }
                            }
                            _ret = true;
                            return _ret;
                        }
                        
                    }
                    else if (targetCol == m_playerPos.m_col)
                    {
                        if (_sceneModule.Data[targetRow][targetCol] == MapDataType.NEWMODE)
                        {
                            return _ret;
                        }
                        if (targetRow > m_playerPos.m_row)
                        {
                            for (int _i = m_playerPos.m_row + 1; _i <= targetRow; _i++)
                            {
                                if (_sceneModule.Data[_i][targetCol] == MapDataType.GAOTAI
                                    || _sceneModule.Data[_i][targetCol] == MapDataType.NONE
                                    || _sceneModule.Data[_i][targetCol] == MapDataType.JUMATUI
                                    || _sceneModule.m_npcList[_i][targetCol] != null)
                                {
                                    return _ret;
                                }
                                else
                                {
                                    if (_sceneModule.m_enemyList[_i][targetCol] != null && _i != targetRow)
                                    {
                                        return _ret;
                                    }
                                }
                            }
                            _ret = true;
                            return _ret;
                        }
                        else if (targetRow < m_playerPos.m_row)
                        {
                            for (int _i = m_playerPos.m_row - 1; _i >= targetRow; _i--)
                            {
                                if (_sceneModule.Data[_i][targetCol] == MapDataType.GAOTAI
                                    || _sceneModule.Data[_i][targetCol] == MapDataType.NONE
                                    || _sceneModule.Data[_i][targetCol] == MapDataType.JUMATUI
                                    || _sceneModule.m_npcList[_i][targetCol] != null)
                                {
                                    return _ret; ;
                                }
                                else
                                {
                                    if(_sceneModule.m_enemyList[_i][targetCol] != null && _i != targetRow)
                                    {
                                        return _ret;
                                    }
                                }
                            }
                            _ret = true;
                            return _ret;
                        }
                    }
                    break;
                case SkillId.MA:
                    if(targetRow == m_playerPos.m_row + 2)
                    {
                        if(targetCol == m_playerPos.m_col + 1 || targetCol == m_playerPos.m_col - 1)
                        {
                            if(_sceneModule.Data[targetRow][targetCol] != MapDataType.NEWMODE &&
                                (_sceneModule.Data[m_playerPos.m_row][m_playerPos.m_col] == MapDataType.GAOTAI ||
                                (_sceneModule.Data[m_playerPos.m_row + 1][m_playerPos.m_col] != MapDataType.GAOTAI
                                && _sceneModule.Data[m_playerPos.m_row + 1][m_playerPos.m_col] != MapDataType.JUMATUI)))
                            {
                                if(_sceneModule.m_npcList[targetRow][targetCol] == null && !PosExitChess(m_playerPos.m_row + 1, m_playerPos.m_col))
                                {
                                    _ret = true;
                                }
                            }
                        }
                    }
                    else if (targetRow == m_playerPos.m_row - 2)
                    {
                        if (targetCol == m_playerPos.m_col + 1 || targetCol == m_playerPos.m_col - 1)
                        {
                            if (_sceneModule.Data[targetRow][targetCol] != MapDataType.NEWMODE &&
                                (_sceneModule.Data[m_playerPos.m_row][m_playerPos.m_col] == MapDataType.GAOTAI ||
                                (_sceneModule.Data[m_playerPos.m_row - 1][m_playerPos.m_col] != MapDataType.GAOTAI
                                && _sceneModule.Data[m_playerPos.m_row - 1][m_playerPos.m_col] != MapDataType.JUMATUI)))
                            {
                                if (_sceneModule.m_npcList[targetRow][targetCol] == null && !PosExitChess(m_playerPos.m_row - 1, m_playerPos.m_col))
                                {
                                    _ret = true;
                                }
                            }
                        }
                    }

                    else if (targetCol == m_playerPos.m_col + 2)
                    {
                        if(targetRow == m_playerPos.m_row + 1 || targetRow == m_playerPos.m_row - 1)
                        {
                            if(_sceneModule.Data[targetRow][targetCol] != MapDataType.NEWMODE &&
                                (_sceneModule.Data[m_playerPos.m_row][m_playerPos.m_col] == MapDataType.GAOTAI ||
                                (_sceneModule.Data[m_playerPos.m_row][m_playerPos.m_col + 1] != MapDataType.GAOTAI
                                && _sceneModule.Data[m_playerPos.m_row][m_playerPos.m_col + 1] != MapDataType.JUMATUI)))
                            {
                                if (_sceneModule.m_npcList[targetRow][targetCol] == null && !PosExitChess(m_playerPos.m_row, m_playerPos.m_col + 1))
                                {
                                    _ret = true;
                                }
                            }
                        }
                    }
                    else if(targetCol == m_playerPos.m_col - 2)
                    {
                        if (targetRow == m_playerPos.m_row + 1 || targetRow == m_playerPos.m_row - 1)
                        {
                            if (_sceneModule.Data[targetRow][targetCol] != MapDataType.NEWMODE &&
                                (_sceneModule.Data[m_playerPos.m_row][m_playerPos.m_col] == MapDataType.GAOTAI ||
                                (_sceneModule.Data[m_playerPos.m_row][m_playerPos.m_col - 1] != MapDataType.GAOTAI
                                && _sceneModule.Data[m_playerPos.m_row][m_playerPos.m_col - 1] != MapDataType.JUMATUI)))
                            {
                                if (_sceneModule.m_npcList[targetRow][targetCol] == null && !PosExitChess(m_playerPos.m_row, m_playerPos.m_col - 1))
                                {
                                    _ret = true;
                                }
                            }
                        }
                    }
                    break;
                case SkillId.PAO:
                    break;
                case SkillId.XIANG:
                    if(targetRow == m_playerPos.m_row + 2 && targetCol == m_playerPos.m_col + 2)
                    {
                        if (_sceneModule.Data[targetRow][targetCol] != MapDataType.NEWMODE &&
                            (_sceneModule.Data[m_playerPos.m_row][m_playerPos.m_col] == MapDataType.GAOTAI ||
                            (_sceneModule.Data[m_playerPos.m_row + 1][m_playerPos.m_col + 1] != MapDataType.GAOTAI
                            && _sceneModule.Data[m_playerPos.m_row + 1][m_playerPos.m_col + 1] != MapDataType.JUMATUI)))
                        {
                            if (_sceneModule.m_npcList[targetRow][targetCol] == null && !PosExitChess(m_playerPos.m_row + 1, m_playerPos.m_col + 1))
                            {
                                _ret = true;
                            }
                        }
                    }
                    else if(targetRow == m_playerPos.m_row + 2 && targetCol == m_playerPos.m_col - 2)
                    {
                        if (_sceneModule.Data[targetRow][targetCol] != MapDataType.NEWMODE &&
                            (_sceneModule.Data[m_playerPos.m_row][m_playerPos.m_col] == MapDataType.GAOTAI ||
                            (_sceneModule.Data[m_playerPos.m_row + 1][m_playerPos.m_col - 1] != MapDataType.GAOTAI
                            && _sceneModule.Data[m_playerPos.m_row + 1][m_playerPos.m_col - 1] != MapDataType.JUMATUI)))
                        {
                            if (_sceneModule.m_npcList[targetRow][targetCol] == null && !PosExitChess(m_playerPos.m_row + 1, m_playerPos.m_col - 1))
                            {
                                _ret = true;
                            }
                        }
                    }
                    else if(targetRow == m_playerPos.m_row - 2 && targetCol == m_playerPos.m_col + 2)
                    {
                        if (_sceneModule.Data[targetRow][targetCol] != MapDataType.NEWMODE &&
                            (_sceneModule.Data[m_playerPos.m_row][m_playerPos.m_col] == MapDataType.GAOTAI ||
                            (_sceneModule.Data[m_playerPos.m_row - 1][m_playerPos.m_col + 1] != MapDataType.GAOTAI
                            && _sceneModule.Data[m_playerPos.m_row - 1][m_playerPos.m_col + 1] != MapDataType.JUMATUI)))
                        {
                            if (_sceneModule.m_npcList[targetRow][targetCol] == null && !PosExitChess(m_playerPos.m_row - 1, m_playerPos.m_col + 1))
                            {
                                _ret = true;
                            }
                        }
                    }
                    else if(targetRow == m_playerPos.m_row - 2 && targetCol == m_playerPos.m_col - 2)
                    {
                        if (_sceneModule.Data[targetRow][targetCol] != MapDataType.NEWMODE &&
                            (_sceneModule.Data[m_playerPos.m_row][m_playerPos.m_col] == MapDataType.GAOTAI ||
                            (_sceneModule.Data[m_playerPos.m_row - 1][m_playerPos.m_col - 1] != MapDataType.GAOTAI
                            && _sceneModule.Data[m_playerPos.m_row - 1][m_playerPos.m_col - 1] != MapDataType.JUMATUI)))
                        {
                            if (_sceneModule.m_npcList[targetRow][targetCol] == null && !PosExitChess(m_playerPos.m_row - 1, m_playerPos.m_col - 1))
                            {
                                _ret = true;
                            }
                        }
                    }
                    break;
                case SkillId.SHI:
                    break;
                case SkillId.BING:
                    break;
            }
            return _ret;
        }

        private void Update()
        {
            if (Input.GetMouseButton(0) && m_move && m_skillId != SkillId.NONE)
            {
                Ray _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit _hit;
                int _layerMask = 1 << 8;
                if (Physics.Raycast(_ray, out _hit, 100, _layerMask))
                {
                    if (_hit.collider.gameObject.tag.Equals("Plane"))
                    {
                        MapData _data = _hit.transform.GetComponent<MapData>();
                        if(CheckCanMove(_data.Pos.m_row, _data.Pos.m_col, _data.Data))
                        {
                            if(GameManager.SceneConfigId == 0)
                            {
                                RookieModule _rookieModule = (RookieModule)GameManager.GameManagerObj.GetComponent<GameManager>().GetModuleByName("RookieModule");
                                _rookieModule.RefreshMap();
                            }
                            else
                            {
                                SceneModule _sceneModule = (SceneModule)GameManager.GameManagerObj.GetComponent<GameManager>().GetModuleByName("SceneModule");
                                _sceneModule.RefreshMap();
                            }
                            
                            IntEventArgs args = new IntEventArgs((int)m_skillId);
                            EventManager.SendEvent(HLEventId.PLAYER_START_MOVE, args);
                            m_state = State.Move;
                            
                            float _targetPosX = _data.Pos.m_row * DiffX;
                            float _targetPosZ = _data.Pos.m_col * DiffZ;
                            if(m_skillId == SkillId.JU)
                            {
                                MissionList.Instance.jutimes++;
                                MissionList.Instance.Judgemission();
                                Sequence _sequence = DOTween.Sequence();
                                _sequence.Append(transform.DOMove(new Vector3(_targetPosX, this.transform.position.y, _targetPosZ), 0.4f).SetEase(m_juCurve));
                                _sequence.AppendCallback(MoveEnd);
                                _sequence.SetAutoKill(true);
                            }
                            else if(m_skillId == SkillId.MA || m_skillId == SkillId.PAO)
                            {
                                MissionList.Instance.matimes++;
                                MissionList.Instance.Judgemission();
                                float _targetPosY = 0f;
                                if (_data.Data == MapDataType.GAOTAI)
                                {
                                    _targetPosY = 1.0f;
                                }
                                float _midTargetPosX = 0.0f;
                                float _midTargetPosZ = 0.0f;
                                if(_data.Pos.m_row == m_playerPos.m_row + 2)
                                {
                                    _midTargetPosX = (m_playerPos.m_row + 1) * DiffX;
                                    _midTargetPosZ = this.transform.position.z;
                                }
                                else if(_data.Pos.m_row == m_playerPos.m_row - 2)
                                {
                                    _midTargetPosX = (m_playerPos.m_row - 1) * DiffX;
                                    _midTargetPosZ = this.transform.position.z;
                                }
                                else if(_data.Pos.m_col == m_playerPos.m_col + 2)
                                {
                                    _midTargetPosX = this.transform.position.x;
                                    _midTargetPosZ = (m_playerPos.m_col + 1) * DiffZ;
                                }
                                else if(_data.Pos.m_col == m_playerPos.m_col - 2)
                                {
                                    _midTargetPosX = this.transform.position.x;
                                    _midTargetPosZ = (m_playerPos.m_col - 1) * DiffZ;
                                }
                                Sequence _sequence = DOTween.Sequence();
                                _sequence.Append(transform.DOMove(new Vector3(_midTargetPosX, this.transform.position.y, _midTargetPosZ), 0.3f).SetEase(m_maCurve1));
                                _sequence.Append(transform.DOJump(new Vector3(_targetPosX, _targetPosY, _targetPosZ), 0.4f, 1, 0.3f).SetEase(m_maCurve2));
                                _sequence.AppendCallback(MoveEnd);
                                _sequence.SetAutoKill(true);
                            }
                            else if(m_skillId == SkillId.XIANG)
                            {
                                MissionList.Instance.xiangtimes++;
                                MissionList.Instance.Judgemission();
                                float _targetPosY = 0f;
                                if (_data.Data == MapDataType.GAOTAI)
                                {
                                    _targetPosY = 1.0f;
                                }
                                Sequence _sequence = DOTween.Sequence();
                                _sequence.Append(transform.DOJump(new Vector3(_targetPosX, _targetPosY, _targetPosZ), 0.4f, 1, 0.5f).SetEase(m_xiangCurve));
                                _sequence.AppendCallback(MoveEnd);
                                _sequence.SetAutoKill(true);
                            }
                            m_skillId = SkillId.NONE;
                            m_playerPos.m_row = _data.Pos.m_row;
                            m_playerPos.m_col = _data.Pos.m_col;
                        }
                    }
                }
            }
        }



        public void MoveEnd()
        {
            m_state = State.Idle;
            m_move = false;
            SceneModule _sceneModule = (SceneModule)GameManager.GameManagerObj.GetComponent<GameManager>().GetModuleByName("SceneModule");
			if (_sceneModule.m_enemyList[m_playerPos.m_row][m_playerPos.m_col] == null)
            {
                AudioFx.Instance.pawndown();
                m_playereff.Play();
            }
            else
            {
                MissionList.Instance.enemykilled++;
                AudioFx.Instance.pawnhit();
                m_playereff.Play();
                m_hiteff.Play();
                _sceneModule.m_enemyList[m_playerPos.m_row][m_playerPos.m_col].DestroyObj();
                _sceneModule.m_enemyList[m_playerPos.m_row][m_playerPos.m_col] = null;
            }
            bool _bWait = _sceneModule.WaitNpc();
            if(_bWait)
            EventManager.SendEvent(HLEventId.PLAYER_END_MOVE, null); 
        }

        private void UseSkill(EventArgs args)
        {            
            m_state = State.UseSkill;
            m_skillId = (SkillId)((IntEventArgs)args).m_args;
            AudioFx.Instance.clickskill((int)m_skillId);
            if (GameManager.SceneConfigId == 0)
            {
                RookieModule _rookieModule = (RookieModule)GameManager.GameManagerObj.GetComponent<GameManager>().GetModuleByName("RookieModule");
                _rookieModule.ChangeMap(m_skillId);
            }
            else
            {
                SceneModule _sceneModule = (SceneModule)GameManager.GameManagerObj.GetComponent<GameManager>().GetModuleByName("SceneModule");
                _sceneModule.ChangeMap(m_skillId, m_playerPos.m_row, m_playerPos.m_col);
            }
        }

        public void SetCanMove(bool bState)
        {
            m_move = bState;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.tag == "Enemy" && !m_move)
            {
                Invoke("delayLoadFailpanel", 0.7f);
                gameObject.SetActive(false);                
            }
        }

        private void delayLoadFailpanel()
        {
            SceneModule _sceneModule = (SceneModule)GameManager.GameManagerObj.GetComponent<GameManager>().GetModuleByName("SceneModule");
            if (!_sceneModule.m_sceneWin)
            {
                _sceneModule.LoadFailpanel("项羽阵亡");
                DestroyObj();
            }            
        }
    }

}

