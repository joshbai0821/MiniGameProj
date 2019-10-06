using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniProj
{
    public class TipPanelControl : MonoBehaviour
    {
        public int m_id;
        public float m_destroyTime;
        private bool m_click = false;
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
            if(m_id == 2)
            {
                EventManager.SendEvent(HLEventId.PLAYER_END_MOVE, null);
            }
            else if(m_id == 3)
            {
                GameObject _obj = (GameObject)GameManager.ResManager.LoadPrefabSync("Prefabs/TipPanel", "TipPanel4", typeof(GameObject));
                _obj.transform.SetParent(GameManager.GameManagerObj.GetComponent<GameManager>().UILayer, false);
            }
            else if(m_id == 4)
            {

            }
        }
    }
}

