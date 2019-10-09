using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniProj
{
    public class IntroducePanelControl : MonoBehaviour
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
    }
}

