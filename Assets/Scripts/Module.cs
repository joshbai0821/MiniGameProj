using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniProj
{
    public class Module : MonoBehaviour
    {
        protected float m_timeStamp;
        protected string m_name;
        public virtual string Name
        {
            get { return m_name; }
        }

        public float TimeStamp
        {
            get { return m_timeStamp; }
        }

        public void UpdateFreeTime(float delta)
        {
            m_timeStamp += delta;
        }

        public Module(string name = "Module")
        {
            m_timeStamp = 0.0f;
            m_name = name;
        }
    }
}

