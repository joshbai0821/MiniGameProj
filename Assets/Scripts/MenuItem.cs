using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MiniProj;

namespace SwipeMenu
{
    /// <summary>
    /// Attach to any menu item. 
    /// </summary>
    public class MenuItem : MonoBehaviour
    {
        public int gameindex;

        const float dragtime = 0.2f;
        float timer = 0;
        bool isDown = false;

        /// <summary>
        /// The behaviour to be invoked when the menu item is selected.
        /// </summary>
        public Button.ButtonClickedEvent OnClick;

        /// <summary>
        /// The behaviour to be invoked when another menu item is selected.
        /// </summary>
        public Button.ButtonClickedEvent OnOtherMenuClick;

        void Update()
        {
            if (isDown)
            {
                timer += Time.deltaTime;
            }

        }

        void OnMouseDown()
        {
            isDown = true;
        }

        void OnMouseUp()
        {
            isDown = false;
            if (timer < dragtime)
            {
                AudioFx.Instance.clicktochooselv();
                Camera.main.GetComponent<WaterWaveEffect>().enabled = true;
                GameManager.GameManagerObj.GetComponent<MainMenuModule>().LoadEffect();

                Invoke("LoadMapScene", 0.8f);
                //SceneManager.LoadScene(gameindex);
                timer = 0;
            }
            timer = 0;
        }

        void LoadMapScene()
        {
            GameManager.GameManagerObj.GetComponent<MainMenuModule>().LoadOneMapScene(gameindex);
        }
    }
}