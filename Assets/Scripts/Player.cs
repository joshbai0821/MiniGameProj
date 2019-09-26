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
        private MapPos m_playerPos;
        public MapPos Pos
        {
            get { return m_playerPos; }
        }

        private SkillId m_skillId;
        private State m_state;

        private static float DiffX = 3.5f;
        private static float DiffZ = 5.0f;

        private void Awake()
        {
            m_skillId = SkillId.NONE;
            m_state = State.Idle;
            EventManager.RegisterEvent(HLEventId.USE_SKILL, this.GetHashCode(), UseSkill);
        }

        public void SetStartPosition(int row, int col)
        {
            m_playerPos.m_row = row;
            m_playerPos.m_col = col;
            transform.position = new Vector3(col * DiffX, 1.6f, row * DiffZ);
        }

        public bool IsReady()
        {
            return m_state != State.Move    ;
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
            if(targetData == MapDataType.NONE)
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
                        if(targetCol > m_playerPos.m_col)
                        {
                            for (int _i = m_playerPos.m_col + 1; _i <= targetCol; _i++)
                            {
                                if(_sceneModule.Data[m_playerPos.m_row][_i] == MapDataType.GAOTAI 
                                    || _sceneModule.Data[m_playerPos.m_row][_i] == MapDataType.NONE)
                                {
                                    return _ret; ;
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
                                    || _sceneModule.Data[m_playerPos.m_row][_i] == MapDataType.NONE)
                                {
                                    return _ret; ;
                                }
                            }
                            _ret = true;
                            return _ret;
                        }
                        
                    }
                    else if (targetCol == m_playerPos.m_col)
                    {
                        if(targetRow > m_playerPos.m_row)
                        {
                            for (int _i = m_playerPos.m_row + 1; _i <= targetRow; _i++)
                            {
                                if (_sceneModule.Data[_i][targetCol] == MapDataType.GAOTAI
                                    || _sceneModule.Data[_i][targetCol] == MapDataType.NONE)
                                {
                                    return _ret; ;
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
                                    || _sceneModule.Data[_i][targetCol] == MapDataType.NONE)
                                {
                                    return _ret; ;
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
                            if(_sceneModule.Data[m_playerPos.m_row][m_playerPos.m_col] == MapDataType.GAOTAI ||
                                _sceneModule.Data[m_playerPos.m_row + 1][m_playerPos.m_col] != MapDataType.GAOTAI)
                            {
                                _ret = true;
                            }
                        }
                    }
                    else if (targetRow == m_playerPos.m_row - 2)
                    {
                        if (targetCol == m_playerPos.m_col + 1 || targetCol == m_playerPos.m_col - 1)
                        {
                            if (_sceneModule.Data[m_playerPos.m_row][m_playerPos.m_col] == MapDataType.GAOTAI ||
                                _sceneModule.Data[m_playerPos.m_row - 1][m_playerPos.m_col] != MapDataType.GAOTAI)
                            {
                                _ret = true;
                            }
                        }
                    }

                    else if (targetCol == m_playerPos.m_col + 2)
                    {
                        if(targetRow == m_playerPos.m_row + 1 || targetRow == m_playerPos.m_row - 1)
                        {
                            if(_sceneModule.Data[m_playerPos.m_row][m_playerPos.m_col] == MapDataType.GAOTAI ||
                                _sceneModule.Data[m_playerPos.m_row][m_playerPos.m_col + 1] != MapDataType.GAOTAI)
                            {
                                _ret = true;
                            }
                        }
                    }
                    else if(targetCol == m_playerPos.m_col - 2)
                    {
                        if (targetRow == m_playerPos.m_row + 1 || targetRow == m_playerPos.m_row - 1)
                        {
                            if (_sceneModule.Data[m_playerPos.m_row][m_playerPos.m_col] == MapDataType.GAOTAI ||
                                _sceneModule.Data[m_playerPos.m_row][m_playerPos.m_col - 1] != MapDataType.GAOTAI)
                            {
                                _ret = true;
                            }
                        }
                    }
                    break;
                case SkillId.PAO:
                    break;
                case SkillId.XIANG:
                    if(targetRow == m_playerPos.m_row + 2 && targetCol == m_playerPos.m_col + 2)
                    {
                        if (_sceneModule.Data[m_playerPos.m_row][m_playerPos.m_col] == MapDataType.GAOTAI ||
                            _sceneModule.Data[m_playerPos.m_row + 1][m_playerPos.m_col + 1] != MapDataType.GAOTAI)
                        {
                            _ret = true;
                        }
                    }
                    else if(targetRow == m_playerPos.m_row + 2 && targetCol == m_playerPos.m_col - 2)
                    {
                        if (_sceneModule.Data[m_playerPos.m_row][m_playerPos.m_col] == MapDataType.GAOTAI ||
                            _sceneModule.Data[m_playerPos.m_row + 1][m_playerPos.m_col - 1] != MapDataType.GAOTAI)
                        {
                            _ret = true;
                        }
                    }
                    else if(targetRow == m_playerPos.m_row - 2 && targetCol == m_playerPos.m_col + 2)
                    {
                        if (_sceneModule.Data[m_playerPos.m_row][m_playerPos.m_col] == MapDataType.GAOTAI ||
                            _sceneModule.Data[m_playerPos.m_row - 1][m_playerPos.m_col + 1] != MapDataType.GAOTAI)
                        {
                            _ret = true;
                        }
                    }
                    else if(targetRow == m_playerPos.m_row - 2 && targetCol == m_playerPos.m_col - 2)
                    {
                        if (_sceneModule.Data[m_playerPos.m_row][m_playerPos.m_col] == MapDataType.GAOTAI ||
                            _sceneModule.Data[m_playerPos.m_row - 1][m_playerPos.m_col - 1] != MapDataType.GAOTAI)
                        {
                            _ret = true;
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
            if (Input.GetMouseButton(0) && m_skillId != SkillId.NONE)
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
                            
                            float _diffZ = (_data.Pos.m_row - m_playerPos.m_row) * DiffZ;
                            float _diffX = (_data.Pos.m_col - m_playerPos.m_col) * DiffX;
                            float _targetPosX = this.transform.position.x + _diffX;
                            float _targetPosZ = this.transform.position.z + _diffZ;
                            if(m_skillId == SkillId.JU)
                            {
                                Sequence _sequence = DOTween.Sequence();
                                _sequence.Append(transform.DOMove(new Vector3(_targetPosX, this.transform.position.y, _targetPosZ), 2));
                                _sequence.AppendCallback(MoveEnd);
                                _sequence.SetAutoKill(true);
                            }
                            else if(m_skillId == SkillId.MA || m_skillId == SkillId.PAO || m_skillId == SkillId.XIANG)
                            {
                                float _targetPosY = 1.6f;
                                if (_data.Data == MapDataType.GAOTAI)
                                {
                                    _targetPosY = 3.2f;
                                }
                                Sequence _sequence = DOTween.Sequence();
                                _sequence.Append(transform.DOJump(new Vector3(_targetPosX, _targetPosY, _targetPosZ), 1.5f * _targetPosY, 1, 2));
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

        private void MoveEnd()
        {
            EventManager.SendEvent(HLEventId.PLAYER_END_MOVE, null);
            m_state = State.Idle;
            
			SceneModule _sceneModule = (SceneModule)GameManager.GameManagerObj.GetComponent<GameManager>().GetModuleByName("SceneModule");
			_sceneModule.EnemyListUpdate();
        }

        private void UseSkill(EventArgs args)
        {
            m_state = State.UseSkill;
            m_skillId = (SkillId)((IntEventArgs)args).m_args;
            if(GameManager.SceneConfigId == 0)
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
    }

}

