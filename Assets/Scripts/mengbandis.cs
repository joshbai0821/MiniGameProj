using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class mengbandis : MonoBehaviour
{

    //状态效果值
    public enum FadeStatuss
    {
        FadeIn,
        FadeOut
    }

    public Image m_Sprite;
    private float m_Alpha;
    private FadeStatuss m_Statuss;
    //效果更新的速度
    public float m_UpdateTime;

    // Use this for initialization
    void Start()
    {
        m_Statuss = FadeStatuss.FadeIn;
        m_Alpha = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Statuss == FadeStatuss.FadeIn)
        {
            m_Alpha += m_UpdateTime * Time.deltaTime;
        }
        else if (m_Statuss == FadeStatuss.FadeOut)
        {
            //m_Alpha -= m_UpdateTime * Time.deltaTime;
        }
        UpdateColorAlpha();
    }

    void UpdateColorAlpha()
    {
        Color ss = m_Sprite.color;
        ss.a = m_Alpha;
        m_Sprite.color = ss;
        if (m_Alpha > 1.5f)
        {
            //m_Alpha = 1f;
            Destroy(gameObject);
            //m_Statuss = FadeStatuss.FadeOut;
        }
        else if (m_Alpha < 0)
        {
            m_Statuss = FadeStatuss.FadeIn;
        }
    }
}
