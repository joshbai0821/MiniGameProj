using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniProj
{
    public class BackGroundControl : MonoBehaviour
    {
        public float m_destroyTime;
        public bool m_click = false;
        private static string MapPrefabPath = "Prefabs/Map";
        // Use this for initialization
        void Start()
        {
            GameObject.Destroy(gameObject, m_destroyTime);
        }

        private void Update()
        {
            if (!m_click && Input.GetMouseButtonDown(0))
            {
                GameObject.Destroy(this.gameObject);
                m_click = true;
            }
        }

        private void OnDestroy()
        {
            if(GameManager.SceneConfigId == 0)
            {
                GameManager.GameManagerObj.GetComponent<GameManager>().LoadModule("RookieModule");
            }
            else if(GameManager.SceneConfigId == 1 && GameManager.firstv2)
            {
                GameManager.firstv2 = false;
                GameObject _introduce3 = (GameObject)GameManager.ResManager.LoadPrefabSync(MapPrefabPath, "IntroducePanel3", typeof(GameObject));
                _introduce3.transform.SetParent(GameManager.GameManagerObj.GetComponent<GameManager>().UILayer, false);
            }
            else if (GameManager.SceneConfigId == 2 && GameManager.firstv3)
            {
                GameManager.firstv3 = false;
                GameObject _introduce4 = (GameObject)GameManager.ResManager.LoadPrefabSync(MapPrefabPath, "IntroducePanel4", typeof(GameObject));
                _introduce4.transform.SetParent(GameManager.GameManagerObj.GetComponent<GameManager>().UILayer, false);
            }
            else if(GameManager.SceneConfigId == 4)
            {
                GameObject _obj = (GameObject)GameManager.ResManager.LoadPrefabSync("Prefabs/TipPanel", "TipPanel1", typeof(GameObject));
                _obj.transform.SetParent(GameManager.GameManagerObj.GetComponent<GameManager>().UILayer, false);
            }
        }
    }
}

