using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniProj
{
    public class BackGroundControl : MonoBehaviour
    {
        public float m_destroyTime;
        // Use this for initialization
        void Start()
        {
            GameObject.Destroy(gameObject, m_destroyTime);
        }

        private void OnDestroy()
        {
            if(GameManager.SceneConfigId == 0)
            {
                GameManager.GameManagerObj.GetComponent<GameManager>().LoadModule("RookieModule");
            }
        }
    }
}

