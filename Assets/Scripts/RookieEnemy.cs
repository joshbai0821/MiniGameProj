using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace MiniProj
{
    public class RookieEnemy : MonoBehaviour
    {
        private static float DiffX = 1.0f;
        private static float DiffZ = -1.0f;


        private int m_id;
        private bool m_initial = false;
        private MapPos m_playerPos;
        public MapPos Pos
        {
            get { return m_playerPos; }
        }

        public AnimationCurve m_curve;

        public void DestroyObj()
        {
            GameObject.Destroy(this.gameObject);
        }

        public void SetPosition(int row, int col)
        {
            m_playerPos.m_row = row;
            m_playerPos.m_col = col;
            if (!m_initial)
            {
                transform.position = new Vector3(row * DiffX, 0f, col * DiffZ);
                m_initial = true;
            }
            else
            {
                Sequence _sequence = DOTween.Sequence();
                _sequence.Append(transform.DOMove(new Vector3(row * DiffX, 0, col * DiffZ), 1.0f).SetEase(m_curve));
                _sequence.SetAutoKill(true);
            }
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
