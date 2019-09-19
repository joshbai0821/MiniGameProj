using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniProj
{
    public class Reference
    {
        protected int refCount;

        public bool IsUnused()
        {
            return refCount <= 0;
        }

        public void Retain()
        {
            refCount++;
        }

        public void Release()
        {
            refCount--;
        }
    }
}

