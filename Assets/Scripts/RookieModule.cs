using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace MiniProj
{
    public class RookieModule : Module
    {
        private static string MapPrefabPath = "Prefabs/Map";

        private MapPos m_playerPos;
        private List<GameObject> m_btnList;
        private Dictionary<int, RookieEnemy> m_rookieEnemyDic;
        private List<GameObject> m_followList;
        private MapPos[] m_enemyPosList =
        {
            new MapPos(3, 2), new MapPos(3, 3), new MapPos(3, 4), new MapPos(3, 5), new MapPos(3, 6),
            new MapPos(4, 2), new MapPos(4, 3), new MapPos(4, 5), new MapPos(4, 6),
            new MapPos(6, 2), new MapPos(6, 3), new MapPos(6, 4), new MapPos(6, 5), new MapPos(6, 6),
            new MapPos(7, 2), new MapPos(7, 3), new MapPos(7, 5), new MapPos(7, 6),
            new MapPos(9, 1), new MapPos(9, 2), new MapPos(9, 3), new MapPos(9, 4), new MapPos(9, 5),
        };
        private MapPos[] m_maPosList =
        {
            new MapPos(0,1), new MapPos(0,2), new MapPos(0,4), new MapPos(0,5),
        };

        public RookieModule() : base("RookieModule")
        {
        }

        private void Awake()
        {
            m_followList = new List<GameObject>();
            m_btnList = new List<GameObject>();
            m_rookieEnemyDic = new Dictionary<int, RookieEnemy>();
            Transform _tsfUIRoot = GameManager.GameManagerObj.GetComponent<GameManager>().UILayer;
            Transform _tsfPanel = _tsfUIRoot.Find("SkillPanel(Clone)");
            for(int _i = 0; _i < _tsfPanel.childCount; ++_i)
            {
                GameObject _btnObj = _tsfPanel.GetChild(_i).gameObject;
                m_btnList.Add(_btnObj);
                _btnObj.GetComponent<Button>().interactable = false;
            }
            LoadRookieEnemies();
            TimerManager.StartTimer(1000, false, null, ExecuteRookieEnemies, 0);
            TimerManager.StartTimer(1500, false, null, DelayActiveMaBtn, 0);
        }

        private void ExecuteRookieEnemies(EventArgs args)
        {
            ExecuteRookieEnemies();
        }

        private void DelayActiveMaBtn(EventArgs args)
        {
            LoadRookieMa();
            ActiveBtn(SkillId.MA);
        }

        private void LoadRookieMa()
        {
            SceneModule _sceneModule = (SceneModule)GameManager.GameManagerObj.GetComponent<GameManager>().GetModuleByName("SceneModule");
            for (int _i = 0; _i < m_maPosList.Length; ++_i)
            {
                GameObject _rookieMa = (GameObject)GameManager.ResManager.LoadPrefabSync(MapPrefabPath, "RookieMa", typeof(GameObject));
                _rookieMa.transform.SetParent(GameManager.GameManagerObj.GetComponent<GameManager>().SceneLayer);
                _rookieMa.GetComponent<Follower>().SetPosition(m_maPosList[_i].m_row, m_maPosList[_i].m_col);
                m_followList.Add(_rookieMa);
                _sceneModule.Data[m_maPosList[_i].m_row][m_maPosList[_i].m_col] = MapDataType.BUXIA;
            }
            
            
        }

        private void ActiveBtn(SkillId id)
        {
            for(int _i = 0; _i < m_btnList.Count; ++_i)
            {
                if(m_btnList[_i].GetComponent<SkillBtn>().Id == id)
                {
                    m_btnList[_i].GetComponent<Button>().interactable = true;
                }
            }
        }

        private void LoadRookieEnemies()
        {
            for(int _i = 0; _i < m_enemyPosList.Length; ++_i)
            {
                string _name = "RookieEnemy";
                GameObject _rookieEnemyObj = (GameObject)GameManager.ResManager.LoadPrefabSync(MapPrefabPath, _name, typeof(GameObject));
                _rookieEnemyObj.transform.SetParent(GameManager.GameManagerObj.GetComponent<GameManager>().SceneLayer);
                RookieEnemy _rookieEnemy = _rookieEnemyObj.GetComponent<RookieEnemy>();
                _rookieEnemy.SetId(_i);
                _rookieEnemy.SetPosition(m_enemyPosList[_i].m_row, m_enemyPosList[_i].m_col);
                m_rookieEnemyDic.Add(_i, _rookieEnemy);
            }
        }

        private void ExecuteRookieEnemies()
        {
            Dictionary<int, RookieEnemy>.Enumerator _enumerator = m_rookieEnemyDic.GetEnumerator();
            while (_enumerator.MoveNext())
            {
                _enumerator.Current.Value.Execute();

            }
            _enumerator.Dispose();
        }

        public void RemoveRookieEnemy(int id)
        {
            m_rookieEnemyDic.Remove(id);
        }


    }
}

