using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace MiniProj
{
    public class MainMenuModule : Module
    {
        public MainMenuModule():base("MainMenuModule")
        {

        }

		private GameObject m_mainMenuObj;
        private GameObject m_chooseSceneObj;

		private static string MainPrefabPath = "Prefabs/MainMenu";

		private void Awake()
        {
            LoadMainMenu();
        }

        //进入第一关
		private void EnterFirstScene()
        {
            if (m_mainMenuObj != null)
            {
                GameObject.Destroy(m_mainMenuObj);
            }
            if(m_chooseSceneObj != null)
            {
                GameObject.Destroy(m_chooseSceneObj);
            }
            GameManager.SceneConfigId = 0;
            SceneManager.sceneLoaded += GameManager.GameManagerObj.GetComponent<GameManager>().OnMapSceneLoad;
            SceneManager.LoadScene(1);
		}

        private void LoadMainMenu()
        {
            if(m_chooseSceneObj != null)
            {
                m_chooseSceneObj.SetActive(false);
            }
            if (m_mainMenuObj == null)
            {
                m_mainMenuObj = (GameObject)GameManager.ResManager.LoadPrefabSync(MainPrefabPath, "MainMenuPanel", typeof(GameObject));
                m_mainMenuObj.transform.SetParent(GameManager.GameManagerObj.GetComponent<GameManager>().UILayer, false);
                m_mainMenuObj.transform.Find("Button").GetComponent<Button>().onClick.AddListener(EnterFirstScene);
                m_mainMenuObj.transform.Find("Button1").GetComponent<Button>().onClick.AddListener(LoadChooseScene);
            }
            else
            {
                m_mainMenuObj.SetActive(true);
            }
        }

        //加载选关卡界面
        private void LoadChooseScene()
        {
            if (m_mainMenuObj != null)
            {
                m_mainMenuObj.SetActive(false);
            }
            if(m_chooseSceneObj == null)
            {
                m_chooseSceneObj = (GameObject)GameManager.ResManager.LoadPrefabSync(MainPrefabPath, "ChooseScene", typeof(GameObject));
                m_chooseSceneObj.transform.SetParent(GameManager.GameManagerObj.GetComponent<GameManager>().UILayer, false);
                m_chooseSceneObj.transform.Find("Viewport/Content/Button").GetComponent<Button>().onClick.AddListener(()=> { LoadOneMapScene(0); });
                m_chooseSceneObj.transform.Find("Viewport/Content/Button1").GetComponent<Button>().onClick.AddListener(() => { LoadOneMapScene(1); });
                m_chooseSceneObj.transform.Find("Viewport/Content/Button2").GetComponent<Button>().onClick.AddListener(() => { LoadOneMapScene(2); });
            }
            else
            {
                m_chooseSceneObj.SetActive(true);
            }
        }

        private void LoadOneMapScene(int id)
        {
            if (m_mainMenuObj != null)
            {
                GameObject.Destroy(m_mainMenuObj);
            }
            if (m_chooseSceneObj != null)
            {
                GameObject.Destroy(m_chooseSceneObj);
            }
            GameManager.SceneConfigId = id;
            SceneManager.sceneLoaded += GameManager.GameManagerObj.GetComponent<GameManager>().OnMapSceneLoad;
            SceneManager.LoadScene(id + 1);
        }

    }
}

