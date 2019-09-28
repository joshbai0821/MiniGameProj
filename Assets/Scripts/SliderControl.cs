using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MiniProj
{
    public class SliderControl : MonoBehaviour
    {

        public Scrollbar m_scrollbar;

        private float m_targetValue;

        private bool m_needMove = false;

        private const float MOVE_SPEED = 1F;

        private const float SMOOTH_TIME = 0.2F;

        private float m_moveSpeed = 0f;

        public void OnPointerDown()
        {
            m_needMove = false;
        }

        public void OnPointerUp()
        {
            // 判断当前位于哪个区间，设置自动滑动至的位置
            if (m_scrollbar.value <= 0.33f)
            {
                m_targetValue = 0;
            }
            else if (m_scrollbar.value <= 0.66f)
            {
                m_targetValue = 0.5f;
            }
            else
            {
                m_targetValue = 1f;
            }

            m_needMove = true;
            m_moveSpeed = 0;
        }

        void Update()
        {
            if (m_needMove)
            {
                if (Mathf.Abs(m_scrollbar.value - m_targetValue) < 0.01f)
                {
                    m_scrollbar.value = m_targetValue;
                    m_needMove = false;
                    return;
                }
                m_scrollbar.value = Mathf.SmoothDamp(m_scrollbar.value, m_targetValue, ref m_moveSpeed, SMOOTH_TIME);
            }
        }
    }
}

