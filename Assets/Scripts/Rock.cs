using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;


namespace MiniProj
{

    public class Rock : MonoBehaviour
    {

        public MapPos m_RockPos;
        public int m_dir;
        public List<PlayerType> m_Trigger;
        public bool m_IsEnd;
        public Rock()
        { }

        private void Awake()
        {
            m_IsEnd = false;
        }

        public void DestroyObj()
        {
            GameObject.Destroy(this.gameObject);
        }

        public void Trigger()
        {
            SceneModule _sceneModule = (SceneModule)GameManager.GameManagerObj.GetComponent<GameManager>().GetModuleByName("SceneModule");
            MapPos pos = new MapPos();
            _sceneModule.GetPlayerPos(ref pos);
            //触发
            for (int _i = 0; _i < m_Trigger.Count; _i++)
            {
                if(m_Trigger[_i] == PlayerType.XIANGYU && pos.m_row == m_RockPos.m_row && pos.m_col == m_RockPos.m_col)
                {
                    //攻击
                    Attack();
                }
                else if(m_Trigger[_i] == PlayerType.YUJI && _sceneModule.UpdateYuJiPos())
                {
                    if(_sceneModule.YuJiPos.m_row == m_RockPos.m_row && _sceneModule.YuJiPos.m_col == m_RockPos.m_col)
                    {
                        //攻击
                        Attack();
                    }

                }
            }
        }

        public void Attack()
        {
            //石头滚动并消灭敌人

            m_IsEnd = true;
        }



    }
}