﻿using System.Collections;
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

        private static float DiffX = 1.0f;
        private static float DiffZ = -1.0f;
        private FollowerType m_type;
        private bool m_initial = false;
        private MapPos m_playerPos;
        public MapPos Pos
        {
            get { return m_playerPos; }
        }
        public AnimationCurve m_curve;
        

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
                    transform.position = new Vector3(row * DiffX, 10f, col * DiffZ);
                    Sequence _sequence = DOTween.Sequence();
                    _sequence.Append(transform.DOMove(new Vector3(transform.position.x, 0, transform.position.z), 0.5f));
                    _sequence.SetAutoKill(true);
                    m_playerPos.m_row = row;
                    m_playerPos.m_col = col;
                    m_initial = true;
                }
                if(m_type == FollowerType.JU)
                {
                    transform.position = new Vector3((row - 3) * DiffX, 0f, col * DiffZ);
                    Sequence _sequence = DOTween.Sequence();
                    _sequence.Append(transform.DOMove(new Vector3(row * DiffX, 0, this.transform.position.z), 1).SetEase(m_curve));
                    _sequence.SetAutoKill(true);
                    m_playerPos.m_row = row;
                    m_playerPos.m_col = col;
                    m_initial = true;
                }
                if(m_type == FollowerType.XIANG)
                {
                    transform.position = new Vector3((row - 2) * DiffX, 10f, (col - 2) * DiffZ);
                    Sequence _sequence = DOTween.Sequence();
                    _sequence.Append(transform.DOMove(new Vector3(transform.position.x, 0, transform.position.z), 1));
                    _sequence.Append(transform.DOJump(new Vector3(row * DiffX, 0f, col * DiffZ), 0.75f, 1,0.5f).SetEase(m_curve));
                    _sequence.SetAutoKill(true);
                    m_playerPos.m_row = row;
                    m_playerPos.m_col = col;
                    m_initial = true;
                }
            }
            else
            {
                if(m_type == FollowerType.MA || m_type == FollowerType.XIANG)
                {
                    m_playerPos.m_row = row;
                    m_playerPos.m_col = col;
                    Sequence _sequence = DOTween.Sequence();
                    _sequence.Append(transform.DOJump(new Vector3(row * DiffX, 0f, col * DiffZ), 0.4f, 1, 0.3f).SetEase(m_curve));
                    _sequence.SetAutoKill(true);
                }
                else
                {
                    m_playerPos.m_row = row;
                    m_playerPos.m_col = col;
                    Sequence _sequence = DOTween.Sequence();
                    _sequence.Append(transform.DOMove(new Vector3(row * DiffX, 0, col * DiffZ), 0.4f).SetEase(m_curve));
                    _sequence.SetAutoKill(true);
                }
            }
        }

        public void BeKilled()
        {
            GameObject.Destroy(gameObject);
        }


    }
}

