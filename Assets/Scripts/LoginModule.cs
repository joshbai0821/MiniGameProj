using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace MiniProj
{
    public class LoginModule : Module
    {
        public LoginModule():base("LoginModule")
        {

        }

		private GameObject m_obj;

		private static string LoginPrefabPath = "Prefabs/Login";

		private void Awake()
        {
			m_obj = (GameObject)GameManager.ResManager.LoadPrefabSync(LoginPrefabPath, "bg", typeof(GameObject));
			m_obj.transform.SetParent(GameManager.GameManagerObj.GetComponent<GameManager>().UILayer, false);
			m_obj.transform.Find("Button").GetComponent<Button>().onClick.AddListener(login);
		}

		public void login()
        {
        	GameManager.GameManagerObj.GetComponent<GameManager>().LoadModule("MainMenuModule");
        	GameObject.Destroy(m_obj);

		}

		//
    }
}

