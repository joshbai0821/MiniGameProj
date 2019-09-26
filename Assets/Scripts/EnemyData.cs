using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniProj
{
    [Serializable]
    public struct EnemyPos
    {
        public int m_row;
        public int m_col;
    }


    public class EnemyData : MonoBehaviour
    {
        
        [SerializeField]
        private int m_data;
        [SerializeField]
        private EnemyPos m_pos;

        public int Data
        {
            get { return m_data; }
        }

        public EnemyPos Pos
        {
            get { return m_pos; }
        }
    }

}

