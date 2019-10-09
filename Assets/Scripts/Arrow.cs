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

    //������״̬
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

        //�����Ƿ񴥷��»غϹ���
        public bool ArrowTrigger()
        {
            if (m_status != ArrowStatus.WAIT)
            {
                return false;
            }

            SceneModule _sceneModule = (SceneModule)GameManager.GameManagerObj.GetComponent<GameManager>().GetModuleByName("SceneModule");

            //���������Ƿ��ڴ�����λ����
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
                            //���ع�������������ʾ��Ϣ
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
                            //���ع�������������ʾ��Ϣ
                            SetType(ArrowStatus.ATTACK);
                            return true;
                        }
                    }
                }
                else
                {
                    //�������ͽ�ɫ�Ȳ���
                }
            }

            return false;
        }

        //������Ϸ�Ƿ����,����1�� �ݼ�2�� ��Ϸû����0, ����3
        public PlayerD ArrowAttack()
        {

            if (m_status != ArrowStatus.ATTACK)
            {
                SetType(ArrowStatus.ATTACK);
                return 0;
            }

            PlayerD Ret = 0;
            SceneModule _sceneModule = (SceneModule)GameManager.GameManagerObj.GetComponent<GameManager>().GetModuleByName("SceneModule");
            //���𹥻�λ�õ����е���;��ͬ���Ӳ�����һ��λ��
            MapPos pos = new MapPos();
            _sceneModule.GetPlayerPos(ref pos);
            bool YuJiExist = _sceneModule.UpdateYuJiPos();

            for (int _i = 0; _i < m_AttackArea.Count; _i++)
            {
                //�����ж�
                if (_sceneModule.m_enemyList[m_AttackArea[_i].m_row][m_AttackArea[_i].m_col] != null)
                {
                    //���˱�����ɱ���Ķ���

                    //������������
                    _sceneModule.m_enemyList[m_AttackArea[_i].m_row][m_AttackArea[_i].m_col].DestroyObj();
                }

                //�ݼ������ж�
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