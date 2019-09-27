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
			m_obj = (GameObject)GameManager.ResManager.LoadPrefabSync(MainPrefabPath, "MainMenuPanel", typeof(GameObject));
			m_obj.transform.SetParent(GameManager.GameManagerObj.GetComponent<GameManager>().UILayer, false);

			
			
			m_obj.transform.Find("Scroll View/Viewport/Content/Button1").GetComponent<Button>().onClick.AddListener(mainmenu);
		}

		public void mainmenu()
        {
        	Debug.Log("mainmenu");
        	GameManager.GameManagerObj.GetComponent<GameManager>().LoadModule("SceneModule");
        	GameObject.Destroy(m_obj);

		}

    }
}

