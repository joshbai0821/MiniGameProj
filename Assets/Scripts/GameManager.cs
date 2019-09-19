using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniProj
{
    public class GameManager : MonoBehaviour
    {

        private static GameObject m_GameManagerObj = null;
        private static ResourceManager m_sResourceMgr = null;
        public static GameObject GameManagerObj { get { return m_GameManagerObj; } }
        public static ResourceManager ResManager { get { return m_sResourceMgr; } }

        private List<Module> m_ModuleList;
        private List<Module> m_FreeModuleList;
        //private int _test;

        private void Awake()
        {
            m_GameManagerObj = CreateGameRootObject();
            m_sResourceMgr = CreateInstance<ResourceManager>();
            m_ModuleList = new List<Module>();
            m_FreeModuleList = new List<Module>();
            LoadModule("SceneModule");
            //_test = 0;
        }

        private void LoadModule(string name)
        {
            for(int _i = 0, _max = m_ModuleList.Count; _i < _max; _i++)
            {
                if (m_ModuleList[_i].Name == name)
                {
                    Debug.Log(string.Format("GameManager {0} has already loaded !!!", name));
                    return;
                }
            }

            for(int _i = 0, _max = m_FreeModuleList.Count; _i < _max; _i++)
            {
                if(m_FreeModuleList[_i].Name == name)
                {
                    m_FreeModuleList[_i].enabled = true;
                    Module _module = m_FreeModuleList[_i];
                    m_FreeModuleList.RemoveAt(_i);
                    m_ModuleList.Add(_module);
                    return;
                }
            }
            ModuleId _id  = ModuleId.ErrorModule;
            ModuleName.NameToID.TryGetValue(name, out _id);
            LoadModuleByName(_id);
        }

        private void UnloadModule(string name)
        {
            
            for (int _i = 0, _max = m_ModuleList.Count; _i < _max; _i++)
            {
                if (m_ModuleList[_i].Name == name)
                {
                    Module _module = m_ModuleList[_i];
                    m_ModuleList.RemoveAt(_i);
                    _module.enabled = false;
                    m_FreeModuleList.Add(_module);
                    return;
                }
            }
            Debug.Log(string.Format("GameManager | {0} did not load !!!", name));
            return;
        }
        
        private Module GetModuleByName(string name)
        {
            for (int _i = 0, _max = m_ModuleList.Count; _i < _max; _i++)
            {
                if (m_ModuleList[_i].Name == name)
                {
                    return m_ModuleList[_i];
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
                    m_ModuleList.Add(_loginModule);
                    break;
                case ModuleId.MainMenuModule:
                    MainMenuModule _mainMenuModule = CreateInstance<MainMenuModule>();
                    m_ModuleList.Add(_mainMenuModule);
                    break;
                case ModuleId.SceneModule:
                    SceneModule _sceneModule = CreateInstance<SceneModule>();
                    m_ModuleList.Add(_sceneModule);
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
            if (m_GameManagerObj != null)
            {
                T tValue = m_GameManagerObj.GetComponent<T>();
                if (tValue == null)
                {
                    tValue = m_GameManagerObj.AddComponent<T>();
                }
                return tValue;
            }

            return default(T);
        }

        private static void DestroyInstance<T>() where T : Component
        {
            if(m_GameManagerObj != null)
            {
                T tValue = m_GameManagerObj.GetComponent<T>();
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
            for (int _i = 0; _i < m_FreeModuleList.Count; ++_i)
            {
                m_FreeModuleList[_i].UpdateFreeTime(Time.deltaTime);
                if(m_FreeModuleList[_i].TimeStamp > 10.0f)
                {
                    Destroy(m_FreeModuleList[_i]);
                }
            }
        }
    }
}

