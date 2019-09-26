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
        private int m_step = 0;

        private MapPos m_playerPos;
        private List<GameObject> m_btnList;
        private Dictionary<int, RookieEnemy> m_rookieEnemyDic;
        private List<GameObject> m_followList;
        private MapPos[] m_enemyPosList =
        {
            new MapPos(3, 2), new MapPos(3, 3), new MapPos(3, 4), new MapPos(3, 5), new MapPos(3, 6),
            new MapPos(4, 2), new MapPos(4, 3), new MapPos(4, 5), new MapPos(4, 6),
            new MapPos(5, 2), new MapPos(5, 3), new MapPos(5, 4), new MapPos(5, 5), new MapPos(5, 6),
            new MapPos(6, 2), new MapPos(6, 3), new MapPos(6, 5), new MapPos(6, 6),
            new MapPos(8, 0), new MapPos(8, 1), new MapPos(8, 2), new MapPos(8, 3), new MapPos(8, 4),
        };
        private MapPos[] m_maPosList =
        {
            new MapPos(0,1), new MapPos(0,2), new MapPos(0,4), new MapPos(0,5),
        };

        private MapPos[] m_juPosList =
        {
            new MapPos(2,2), new MapPos(2,3), new MapPos(2,5), new MapPos(2,6),
        };

        private MapPos[] m_xiangPosList =
        {
            new MapPos(3,2), new MapPos(3,3), new MapPos(3,5), new MapPos(3,6),
        };

        private List<Material> m_matList;
        private List<Color> m_originColorList;

        public RookieModule() : base("RookieModule")
        {
        }

        private void Awake()
        {
            m_matList = new List<Material>();
            m_originColorList = new List<Color>();
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
            TimerManager.StartTimer(1000, false, null, DelayExecuteRookieEnemies, 0);
            TimerManager.StartTimer(1500, false, null, DelayActiveMaBtn, 0);
            EventManager.RegisterEvent(HLEventId.PLAYER_END_MOVE, this.GetHashCode(), FollowPlayer);
        }

        private void OnDestroy()
        {
            m_matList.Clear();
            m_originColorList.Clear();
            m_followList.Clear();
            m_followList.Clear();
            m_btnList.Clear();
            m_rookieEnemyDic.Clear();
        }

        private void DelayExecuteRookieEnemies(EventArgs args)
        {
            ExecuteRookieEnemies();
        }

        private void DelayActiveMaBtn(EventArgs args)
        {
            LoadRookieMa();
            ActiveBtn(SkillId.MA);
        }

        private void FollowPlayer(EventArgs args)
        {
            if(m_step == 0)
            {
                for(int _i = 0; _i < m_followList.Count; ++_i)
                {
                    Follower _follower = m_followList[_i].GetComponent<Follower>();
                    int _row = _follower.Pos.m_row;
                    int _col = _follower.Pos.m_col;
                    _follower.SetPosition(_row + 2, _col + 1);
                }
                ++m_step;
                TimerManager.StartTimer(1000, false, null, DelayDestroyMaFollow, 0);
            }
            else if(m_step == 1)
            {
                for (int _i = 0; _i < m_followList.Count; ++_i)
                {
                    Follower _follower = m_followList[_i].GetComponent<Follower>();
                    int _row = _follower.Pos.m_row;
                    int _col = _follower.Pos.m_col;
                    _follower.SetPosition(_row + 1, _col);
                }
                ++m_step;
                TimerManager.StartTimer(1000, false, null, DelayDestroyJuFollow, 0);
            }
            else if(m_step == 2)
            {
                for (int _i = 0; _i < m_followList.Count; ++_i)
                {
                    Follower _follower = m_followList[_i].GetComponent<Follower>();
                    int _row = _follower.Pos.m_row;
                    int _col = _follower.Pos.m_col;
                    _follower.SetPosition(_row + 2, _col - 2);
                }
                ++m_step;
            }
        }

        private void DelayDestroyMaFollow(EventArgs args)
        {
            for (int _i = 0; _i < m_followList.Count; ++_i)
            {
                GameObject.Destroy(m_followList[_i]);
            }
            m_followList.Clear();
            ExecuteRookieEnemies();
            TimerManager.StartTimer(1000, false, null, DelayLoadRookieJu, 0);
        }

        private void DelayDestroyJuFollow(EventArgs args)
        {
            for (int _i = 0; _i < m_followList.Count; ++_i)
            {
                GameObject.Destroy(m_followList[_i]);
            }
            m_followList.Clear();
            ExecuteRookieEnemies();
            TimerManager.StartTimer(1000, false, null, DelayLoadRookieXiang, 0);
        }

        private void DelayLoadRookieJu(EventArgs args)
        {
            ActiveBtn(SkillId.JU);
            for (int _i = 0; _i < m_juPosList.Length; ++_i)
            {
                GameObject _rookieJu = (GameObject)GameManager.ResManager.LoadPrefabSync(MapPrefabPath, "RookieJu", typeof(GameObject));
                _rookieJu.transform.SetParent(GameManager.GameManagerObj.GetComponent<GameManager>().SceneLayer);
                Follower _follower = _rookieJu.GetComponent<Follower>();
                _follower.SetType(FollowerType.JU);
                _follower.SetPosition(m_juPosList[_i].m_row, m_juPosList[_i].m_col);

                m_followList.Add(_rookieJu);
            }
        }

        private void DelayLoadRookieXiang(EventArgs args)
        {
            ActiveBtn(SkillId.XIANG);
            for (int _i = 0; _i < m_xiangPosList.Length; ++_i)
            {
                GameObject _rookieXiang = (GameObject)GameManager.ResManager.LoadPrefabSync(MapPrefabPath, "RookieXiang", typeof(GameObject));
                _rookieXiang.transform.SetParent(GameManager.GameManagerObj.GetComponent<GameManager>().SceneLayer);
                Follower _follower = _rookieXiang.GetComponent<Follower>();
                _follower.SetType(FollowerType.XIANG);
                _follower.SetPosition(m_xiangPosList[_i].m_row, m_xiangPosList[_i].m_col);
                m_followList.Add(_rookieXiang);
            }
        }

        public void ChangeMap(SkillId id)
        {
            SceneModule _sceneModule = (SceneModule)GameManager.GameManagerObj.GetComponent<GameManager>().GetModuleByName("SceneModule");
            if (id == SkillId.MA)
            {
                Transform _tsf1 = _sceneModule.GetTsfMapData(2, 2);
                Material _material1 = _tsf1.GetComponent<MeshRenderer>().material;
                m_originColorList.Add(_material1.GetColor("_MainColor"));
                _material1.SetColor("_MainColor", Color.red);
                m_matList.Add(_material1);
                Transform _tsf2 = _sceneModule.GetTsfMapData(2, 4);
                Material _material2 = _tsf2.GetComponent<MeshRenderer>().material;
                m_originColorList.Add(_material2.GetColor("_MainColor"));
                _material2.SetColor("_MainColor", Color.red);
                m_matList.Add(_material2);
            }
            if(id == SkillId.JU)
            {
                //
                Transform _tsf1 = _sceneModule.GetTsfMapData(0, 4);
                Material _material1 = _tsf1.GetComponent<MeshRenderer>().material;
                m_originColorList.Add(_material1.GetColor("_MainColor"));
                _material1.SetColor("_MainColor", Color.red);
                m_matList.Add(_material1);
                //
                Transform _tsf2 = _sceneModule.GetTsfMapData(1, 4);
                Material _material2 = _tsf2.GetComponent<MeshRenderer>().material;
                m_originColorList.Add(_material2.GetColor("_MainColor"));
                _material2.SetColor("_MainColor", Color.red);
                m_matList.Add(_material2);
                //
                Transform _tsf3 = _sceneModule.GetTsfMapData(3, 4);
                Material _material3 = _tsf3.GetComponent<MeshRenderer>().material;
                m_originColorList.Add(_material3.GetColor("_MainColor"));
                _material3.SetColor("_MainColor", Color.red);
                m_matList.Add(_material3);
            }
            if(id == SkillId.XIANG)
            {
                //
                Transform _tsf1 = _sceneModule.GetTsfMapData(1, 2);
                Material _material1 = _tsf1.GetComponent<MeshRenderer>().material;
                m_originColorList.Add(_material1.GetColor("_MainColor"));
                _material1.SetColor("_MainColor", Color.red);
                m_matList.Add(_material1);
                //
                Transform _tsf2 = _sceneModule.GetTsfMapData(1, 6);
                Material _material2 = _tsf2.GetComponent<MeshRenderer>().material;
                m_originColorList.Add(_material2.GetColor("_MainColor"));
                _material2.SetColor("_MainColor", Color.red);
                m_matList.Add(_material2);
                //
                Transform _tsf3 = _sceneModule.GetTsfMapData(5, 2);
                Material _material3 = _tsf3.GetComponent<MeshRenderer>().material;
                m_originColorList.Add(_material3.GetColor("_MainColor"));
                _material3.SetColor("_MainColor", Color.red);
                m_matList.Add(_material3);
                //
                Transform _tsf4 = _sceneModule.GetTsfMapData(5, 6);
                Material _material4 = _tsf4.GetComponent<MeshRenderer>().material;
                m_originColorList.Add(_material4.GetColor("_MainColor"));
                _material4.SetColor("_MainColor", Color.red);
                m_matList.Add(_material4);
            }
        }

        private void LoadRookieMa()
        {
            for (int _i = 0; _i < m_maPosList.Length; ++_i)
            {
                GameObject _rookieMa = (GameObject)GameManager.ResManager.LoadPrefabSync(MapPrefabPath, "RookieMa", typeof(GameObject));
                _rookieMa.transform.SetParent(GameManager.GameManagerObj.GetComponent<GameManager>().SceneLayer);
                Follower _follower = _rookieMa.GetComponent<Follower>();
                _follower.SetType(FollowerType.MA);
                _follower.SetPosition(m_maPosList[_i].m_row, m_maPosList[_i].m_col);
                
                m_followList.Add(_rookieMa);
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

        public void RefreshMap()
        {
            if (m_matList != null)
            {
                for (int _i = 0; _i < m_matList.Count; ++_i)
                {
                    m_matList[_i].SetColor("_MainColor", m_originColorList[_i]);
                }
                m_matList.Clear();
            }
        }
    }
}

