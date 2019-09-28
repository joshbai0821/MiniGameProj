using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace MiniProj
{
    public class MainMenuModule : Module
    {
        public MainMenuModule():base("MainMenuModule")
        {

        }

		private GameObject m_obj;

		private static string MainPrefabPath = "Prefabs/MainMenu";

		private void Awake()
        {
            int _enterGame = PlayerPrefs.GetInt("EnterGame", 0);
            if(_enterGame == 0)
            {
                m_obj = (GameObject)GameManager.ResManager.LoadPrefabSync(MainPrefabPath, "MainMenuFirst", typeof(GameObject));
                m_obj.transform.SetParent(GameManager.GameManagerObj.GetComponent<GameManager>().UILayer, false);
                m_obj.transform.Find("Button").GetComponent<Button>().onClick.AddListener(EnterScene);
                PlayerPrefs.SetInt("EnterGame", 1);
            }
            else
            {
                m_obj = (GameObject)GameManager.ResManager.LoadPrefabSync(MainPrefabPath, "MainMenuFirst", typeof(GameObject));
                m_obj.transform.SetParent(GameManager.GameManagerObj.GetComponent<GameManager>().UILayer, false);
                m_obj.transform.Find("Button").GetComponent<Button>().onClick.AddListener(LoadChooseScene);
            }
            
        }

        //进入第一关
		private void EnterScene()
        {
            GameManager.SceneConfigId = 0;
        	GameManager.GameManagerObj.GetComponent<GameManager>().LoadModule("SceneModule");
        	GameObject.Destroy(m_obj);
		}

        //加载选关卡界面
        private void LoadChooseScene()
        {

        }

    }
}

