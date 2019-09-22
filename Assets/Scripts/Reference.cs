using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniProj
{
    public class Reference
    {
        protected int m_refCount;

        public bool IsUnused()
        {
            return m_refCount <= 0;
        }

        public void Retain()
        {
            m_refCount++;
        }

        public void Release()
        {
            m_refCount--;
        }
    }
}

