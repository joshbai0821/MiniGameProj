using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

namespace MiniProj
{
    public enum FollowerType
    {
        JU = 0,
        MA = 1,
        XIANG = 2,
    }
    public class Follower : MonoBehaviour
    {
        private static float DiffX = 3.5f;
        private static float DiffZ = 5.0f;
        private FollowerType m_type;
        private MapPos m_playerPos;
        public MapPos Pos
        {
            get { return m_playerPos; }
        }
        private bool m_initial = false;

        public void SetType(FollowerType type)
        {
            m_type = type;
        }

        public void SetPosition(int row, int col)
        {
            if (m_initial == false)
            {
                if(m_type == FollowerType.MA)
                {
                    transform.position = new Vector3(col * DiffX, 10f, row * DiffZ);
                    Sequence _sequence = DOTween.Sequence();
                    _sequence.Append(transform.DOMove(new Vector3(transform.position.x, 1, transform.position.z), 2));
                    _sequence.SetAutoKill(true);
                    m_playerPos.m_row = row;
                    m_playerPos.m_col = col;
                    m_initial = true;
                }
                if(m_type == FollowerType.JU)
                {
                    transform.position = new Vector3(col * DiffX, 1f, (row - 3) * DiffZ);
                    Sequence _sequence = DOTween.Sequence();
                    _sequence.Append(transform.DOMove(new Vector3(transform.position.x, 1, row * DiffZ), 2));
                    _sequence.SetAutoKill(true);
                    m_playerPos.m_row = row;
                    m_playerPos.m_col = col;
                    m_initial = true;
                }
                if(m_type == FollowerType.XIANG)
                {
                    transform.position = new Vector3((col - 2) * DiffX, 10f, (row - 2) * DiffZ);
                    Sequence _sequence = DOTween.Sequence();
                    _sequence.Append(transform.DOMove(new Vector3(transform.position.x, 1, transform.position.z), 2));
                    _sequence.Append(transform.DOJump(new Vector3(col * DiffX, 1, row * DiffZ), 1.5f, 1, 2.0f));
                    _sequence.SetAutoKill(true);
                    m_playerPos.m_row = row;
                    m_playerPos.m_col = col;
                    m_initial = true;
                }
            }
            else
            {
                m_playerPos.m_row = row;
                m_playerPos.m_col = col;
                transform.position = new Vector3(col * DiffX, 1f, row * DiffZ);
                m_playerPos.m_row = row;
                m_playerPos.m_col = col;
            }
        }

        public void BeKilled()
        {
            GameObject.Destroy(gameObject);
        }
    }
}

