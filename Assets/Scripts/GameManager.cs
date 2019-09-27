using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniProj
{
    public class GameManager : MonoBehaviour
    {

        private static GameObject SGameManagerObj = null;
        private static ResourceManager SResourceMgr = null;
        public static GameObject GameManagerObj { get { return SGameManagerObj; } }
        public static ResourceManager ResManager { get { return SResourceMgr; } }

        private List<Module> m_moduleList;
        private List<Module> m_freeModuleList;
        private static int m_sceneConfigId;
        public static int SceneConfigId
        {
            get { return m_sceneConfigId; }
        }
        public Transform UILayer;
        public Transform SceneLayer;
        //private int _test;

        private void Awake()
        {
            SGameManagerObj = CreateGameRootObject();
            SResourceMgr = CreateInstance<ResourceManager>();
            m_moduleList = new List<Module>();
            m_freeModuleList = new List<Module>();
            m_sceneConfigId = 1;
            LoadModule("SceneModule");
            //_test = 0;
        }

        public void LoadModule(string name)
        {
            for(int _i = 0, _max = m_moduleList.Count; _i < _max; _i++)
            {
                if (m_moduleList[_i].Name == name)
                {
                    Debug.Log(string.Format("GameManager {0} has already loaded !!!", name));
                    return;
                }
            }

            for(int _i = 0, _max = m_freeModuleList.Count; _i < _max; _i++)
            {
                if(m_freeModuleList[_i].Name == name)
                {
                    m_freeModuleList[_i].enabled = true;
                    Module _module = m_freeModuleList[_i];
                    m_freeModuleList.RemoveAt(_i);
                    m_moduleList.Add(_module);
                    return;
                }
            }
            ModuleId _id  = ModuleId.ErrorModule;
            ModuleName.NameToID.TryGetValue(name, out _id);
            LoadModuleByName(_id);
        }

        public void UnloadModule(string name)
        {
            
            for (int _i = 0, _max = m_moduleList.Count; _i < _max; _i++)
            {
                if (m_moduleList[_i].Name == name)
                {
                    Module _module = m_moduleList[_i];
                    m_moduleList.RemoveAt(_i);
                    _module.enabled = false;
                    m_freeModuleList.Add(_module);
                    return;
                }
            }
            Debug.Log(string.Format("GameManager | {0} did not load !!!", name));
            return;
        }
        
        public Module GetModuleByName(string name)
        {
            for (int _i = 0, _max = m_moduleList.Count; _i < _max; _i++)
            {
                if (m_moduleList[_i].Name == name)
                {
                    return m_moduleList[_i];
                }
            }
            Debug.Log(string.Format("GameManager | {0} did not load !!!", name));
            return null;
        }

        private void LoadModuleByName(ModuleId id)
        {
            switch(id)
            {
                case ModuleId.ErrorModule:
                    Debug.Log("Error Module Id");
                    break;
                case ModuleId.LoginModule:
                    LoginModule _loginModule = CreateInstance<LoginModule>();
                    m_moduleList.Add(_loginModule);
                    break;
                case ModuleId.MainMenuModule:
                    MainMenuModule _mainMenuModule = CreateInstance<MainMenuModule>();
                    m_moduleList.Add(_mainMenuModule);
                    break;
                case ModuleId.SceneModule:
                    SceneModule _sceneModule = CreateInstance<SceneModule>();
                    m_moduleList.Add(_sceneModule);
                    break;
                case ModuleId.RookieModule:
                    RookieModule _rookieModule = CreateInstance<RookieModule>();
                    m_moduleList.Add(_rookieModule);
                    break;
                default:
                    break;
            }
            return ;
        }

        private static GameObject CreateGameRootObject()
        {
            GameObject _pObj = GameObject.Find("GameManager");
            if (_pObj == null)
            {
                _pObj = new GameObject();
                _pObj.name = "GameManager";
            }
            DontDestroyOnLoad(_pObj);
            return _pObj;
        }

        private static T CreateInstance<T>() where T : Component
        {
            if (SGameManagerObj != null)
            {
                T tValue = SGameManagerObj.GetComponent<T>();
                if (tValue == null)
                {
                    tValue = SGameManagerObj.AddComponent<T>();
                }
                return tValue;
            }

            return default(T);
        }

        private static void DestroyInstance<T>() where T : Component
        {
            if(SGameManagerObj != null)
            {
                T tValue = SGameManagerObj.GetComponent<T>();
                if (tValue != null)
                {
                    Destroy(tValue);
                }
                return;
            }
        }

        private void Update()
        {
            //++_test;
            //if (_test == 100)
            //{
            //    Debug.Log("GameManager | Unload SceneModule");
            //    SceneModule _module = (SceneModule)GetModuleByName("SceneModule");
            //    _module.ClearMap();
            //    UnloadModule("SceneModule");
            //}
            TimerManager.UpdateTimerList();
            for (int _i = 0; _i < m_freeModuleList.Count; ++_i)
            {
                m_freeModuleList[_i].UpdateFreeTime(Time.deltaTime);
                if(m_freeModuleList[_i].TimeStamp > 10.0f)
                {
                    Destroy(m_freeModuleList[_i]);
                }
            }
        }
    }
}

