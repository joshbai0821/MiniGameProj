using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace MiniProj
{
    public class Follower : MonoBehaviour
    {
        private static float DiffX = 3.5f;
        private static float DiffZ = 5.0f;

        private MapPos m_playerPos;
        private bool m_initial = false;

        public void SetPosition(int row, int col)
        {
            if (m_initial == false)
            {
                transform.position = new Vector3(col * DiffX, 10f, row * DiffZ);
                Sequence _sequence = DOTween.Sequence();
                _sequence.Append(transform.DOMove(new Vector3(transform.position.x, 1, transform.position.z), 2));
                _sequence.SetAutoKill(true);
                m_initial = true;
            }
            else
            {
                m_playerPos.m_row = row;
                m_playerPos.m_col = col;
                transform.position = new Vector3(col * DiffX, 1f, row * DiffZ);
            }
        }
    }
}

