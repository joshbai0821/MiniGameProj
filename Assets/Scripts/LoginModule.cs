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
        private bool m_click = false;

		private static string LoginPrefabPath = "Prefabs/Login";

		private void Awake()
        {
#if UNITY_EDITOR 
            m_obj = (GameObject)GameManager.ResManager.LoadPrefabSync(LoginPrefabPath, "RawImage", typeof(GameObject));
            m_obj.transform.SetParent(GameManager.GameManagerObj.GetComponent<GameManager>().UILayer, false);
#else
            Handheld.PlayFullScreenMovie("cg8.mp4", Color.black, FullScreenMovieControlMode.CancelOnInput);
            GameManager.GameManagerObj.GetComponent<GameManager>().UnloadModule("LoginModule");
            GameManager.GameManagerObj.GetComponent<GameManager>().LoadModule("MainMenuModule");
#endif
        }

#if UNITY_EDITOR
        private void Update()
        {
            if(!m_click && Input.GetMouseButtonDown(0))
            {
                GoToMainMenu();
                m_click = true;
            }
        }
#endif

#if UNITY_EDITOR
        public void GoToMainMenu()
        {
            GameManager.GameManagerObj.GetComponent<GameManager>().UnloadModule("LoginModule");
            GameManager.GameManagerObj.GetComponent<GameManager>().LoadModule("MainMenuModule");
        	GameObject.Destroy(m_obj);
		}
#endif

        //
    }
}

