using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniProj
{
    public class RookieEnemy : MonoBehaviour
    {
        private static float DiffX = 1.0f;
        private static float DiffZ = -1.0f;

        private int m_id;
        private bool m_active;
        private MapPos m_playerPos;
        public MapPos Pos
        {
            get { return m_playerPos; }
        }

        public void DestroyObj()
        {
            GameObject.Destroy(this.gameObject);
        }

        public void SetPosition(int row, int col)
        {
            m_playerPos.m_row = row;
            m_playerPos.m_col = col;
            transform.position = new Vector3(row * DiffX, 0f, col * DiffZ);
        }

        public void Execute()
        {
            SetPosition(m_playerPos.m_row - 1, m_playerPos.m_col);
        }

        public void SetId(int id)
        {
            m_id = id;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.tag == "Player")
            {
                RookieModule _rookieModule = (RookieModule)GameManager.GameManagerObj.GetComponent<GameManager>().GetModuleByName("RookieModule");
                _rookieModule.RemoveRookieEnemy(m_id);
                GameObject.Destroy(gameObject);
            }
        }
    }

}
