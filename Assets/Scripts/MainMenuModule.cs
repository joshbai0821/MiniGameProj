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
        private GameObject m_mengban;

		private static string MainPrefabPath = "Prefabs/MainMenu";

		private void Awake()
        {
            //LoadMainMenu();
        }

        private void OnEnable()
        {
            LoadMainMenu();
        }

        private void OnDisable()
        {
            if (m_mainMenuObj != null)
            {
                GameObject.Destroy(m_mainMenuObj);
            }
            if (m_chooseSceneObj != null)
            {
                GameObject.Destroy(m_chooseSceneObj);
            }
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
            GameManager.GameManagerObj.GetComponent<GameManager>().UnloadModule("MainMenuModule");
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
                m_mainMenuObj.transform.Find("Button").GetComponent<Button>().onClick.AddListener(LoadChooseScene);
            }
            else
            {
                m_mainMenuObj.SetActive(true);
            }
        }

        //加载选关卡界面
        private void LoadChooseScene()
        {
            AudioFx.Instance.clicktochooselv();

            if (m_mainMenuObj != null)
            {
                m_mainMenuObj.SetActive(false);
            }
            if(m_chooseSceneObj == null)
            {
                m_chooseSceneObj = (GameObject)GameManager.ResManager.LoadPrefabSync(MainPrefabPath, "ChooseMenuCamera", typeof(GameObject));
                Camera.main.GetComponent<CameraFilterPack_Blur_Movie>().enabled = true;
            }
            else
            {
                m_chooseSceneObj.SetActive(true);
            }
        }

        public void LoadEffect()
        {
            if (m_mengban == null)
            {
                m_mengban = (GameObject)GameManager.ResManager.LoadPrefabSync(MainPrefabPath, "mengban", typeof(GameObject));
                m_mengban.transform.SetParent(GameManager.GameManagerObj.GetComponent<GameManager>().UILayer, false);
            }
            else
            {
                m_mengban.SetActive(true);
            }

            if (m_chooseSceneObj != null)
            {
                m_chooseSceneObj.SetActive(false);
            }
            Camera.main.GetComponent<CameraFilterPack_Blur_Movie>().enabled = false;
        }

        public void LoadOneMapScene(int id)
        {
            if (m_mainMenuObj != null)
            {
                GameObject.Destroy(m_mainMenuObj);
            }
            if (m_chooseSceneObj != null)
            {
                GameObject.Destroy(m_chooseSceneObj);
            }
            GameManager.GameManagerObj.GetComponent<GameManager>().LoadBGM(id + 1);
            Audio_BGM.Instance.LvBGM(id + 1);
            GameManager.GameManagerObj.GetComponent<GameManager>().UnloadModule("MainMenuModule");
            GameManager.SceneConfigId = id;
            SceneManager.sceneLoaded += GameManager.GameManagerObj.GetComponent<GameManager>().OnMapSceneLoad;
            SceneManager.LoadScene(SceneModule.ConfigIdToSceneIdx[GameManager.SceneConfigId]);
            Camera.main.GetComponent<CameraFilterPack_Blur_Movie>().enabled = false;
            Camera.main.GetComponent<WaterWaveEffect>().enabled = false;
        }

    }
}

