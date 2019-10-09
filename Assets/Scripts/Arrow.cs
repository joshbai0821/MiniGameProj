using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;


namespace MiniProj
{

    public enum PlayerD
    {
        NONE = 0,
        XIANGYU = 1,
        YUJI = 2,
        ALL = 3,
    }

    //弓箭的状态
    public enum ArrowStatus
    {
        WAIT = 1,
        ATTACK = 2,
        END = 3,
    }

    public class Arrow : MonoBehaviour
    {

        public List<MapPos> m_AttackArea;
        public List<MapPos> m_TriggerArea;
        public List<PlayerType> m_Trigger;

        public ArrowStatus m_status;

        public Arrow()
        {
            m_status = ArrowStatus.WAIT;
            m_AttackArea = new List<MapPos>();
            m_TriggerArea = new List<MapPos>();
            m_Trigger = new List<PlayerType>();
        }

        private void Awake()
        {
        }

        public void SetType(ArrowStatus iType)
        {
            m_status = iType;
        }

        public void DestroyObj()
        {
            GameObject.Destroy(this.gameObject);
        }

        //返回是否触发下回合攻击
        public bool ArrowTrigger()
        {
            if (m_status != ArrowStatus.WAIT)
            {
                return false;
            }

            SceneModule _sceneModule = (SceneModule)GameManager.GameManagerObj.GetComponent<GameManager>().GetModuleByName("SceneModule");

            //触发的人是否在触发的位置上
            for (int _i = 0; _i < m_Trigger.Count; _i++)
            {
                if (m_Trigger[_i] == PlayerType.XIANGYU)
                {
                    MapPos pos = new MapPos();
                    _sceneModule.GetPlayerPos(ref pos);

                    for (int _j = 0; _j < m_TriggerArea.Count; _j++)
                    {
                        if (m_TriggerArea[_j].m_row == pos.m_row && m_TriggerArea[_j].m_col == pos.m_col)
                        {
                            //返回弓箭被触发，提示信息
                            SetType(ArrowStatus.ATTACK);
                            return true;
                        }
                    }
                }
                else if (m_Trigger[_i] == PlayerType.YUJI && _sceneModule.UpdateYuJiPos())
                {
                    for (int _j = 0; _j < m_TriggerArea.Count; _j++)
                    {
                        if (m_TriggerArea[_j].m_row == _sceneModule.YuJiPos.m_row && m_TriggerArea[_j].m_col == _sceneModule.YuJiPos.m_col)
                        {
                            //返回弓箭被触发，提示信息
                            SetType(ArrowStatus.ATTACK);
                            return true;
                        }
                    }
                }
                else
                {
                    //其他类型角色先不做
                }
            }

            return false;
        }

        //返回游戏是否结束,项羽1， 虞姬2， 游戏没结束0, 都死3
        public PlayerD ArrowAttack()
        {

            if (m_status != ArrowStatus.ATTACK)
            {
                SetType(ArrowStatus.ATTACK);
                return 0;
            }

            PlayerD Ret = 0;
            SceneModule _sceneModule = (SceneModule)GameManager.GameManagerObj.GetComponent<GameManager>().GetModuleByName("SceneModule");
            //消灭攻击位置的所有的人;不同棋子不会在一个位置
            MapPos pos = new MapPos();
            _sceneModule.GetPlayerPos(ref pos);
            bool YuJiExist = _sceneModule.UpdateYuJiPos();

            for (int _i = 0; _i < m_AttackArea.Count; _i++)
            {
                //敌人判断
                if (_sceneModule.m_enemyList[m_AttackArea[_i].m_row][m_AttackArea[_i].m_col] != null)
                {
                    //敌人被弓箭杀死的动画

                    //敌人死亡处理
                    _sceneModule.m_enemyList[m_AttackArea[_i].m_row][m_AttackArea[_i].m_col].DestroyObj();
                }

                //虞姬项羽判断
                if ((Ret != PlayerD.XIANGYU || Ret != PlayerD.ALL) && pos.m_row == m_AttackArea[_i].m_row && pos.m_col == m_AttackArea[_i].m_col)
                {
                    Ret = Ret == PlayerD.YUJI ? PlayerD.ALL : PlayerD.XIANGYU;
                }

                if ((Ret != PlayerD.YUJI || Ret != PlayerD.ALL) && YuJiExist && _sceneModule.YuJiPos.m_row == m_AttackArea[_i].m_row && _sceneModule.YuJiPos.m_col == m_AttackArea[_i].m_col)
                {
                    Ret = Ret == PlayerD.XIANGYU ? PlayerD.ALL : PlayerD.YUJI;
                }

            }

            SetType(ArrowStatus.WAIT);
            return Ret;
        }

    }
}