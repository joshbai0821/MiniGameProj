using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniProj
{
    [Serializable]
    public struct MapPos
    {
        public int m_row;
        public int m_col;

        public MapPos(int row, int col)
        {
            m_row = row;
            m_col = col;
        }
    }

    public enum MapDataType
    {
        NONE = -1,
        PINGDI = 0,
        GAOTAI = 1,
    }

    public class MapData : MonoBehaviour
    {
        
        [SerializeField]
        private MapDataType m_data;
        [SerializeField]
        private MapPos m_pos;

        public MapDataType Data
        {
            get { return m_data; }
        }

        public MapPos Pos
        {
            get { return m_pos; }
        }
    }

}
