using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio_BGM : MonoBehaviour {

    private AudioSource m_BGMAudio;

    public AudioClip A_BGMAll;
    public float mainMenuVolum;

    public AudioClip A_lv1;
    public float lv1_Volum;

    public AudioClip A_lv2;
    public float lv2_Volum;

    public AudioClip A_lv3;
    public float lv3_Volum;

    public AudioClip A_lv4;
    public float lv4_Volum;

    public AudioClip A_lv5;
    public float lv5_Volum;

    //私有的静态实例
    private static Audio_BGM _instance = null;
    //共有的唯一的，全局访问点
    public static Audio_BGM Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<Audio_BGM>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("All_BGM");
                    _instance = go.AddComponent<Audio_BGM>();
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(gameObject);
        m_BGMAudio = GetComponent<AudioSource>();

    }

    public void LvBGM(int lvnum)
    {
        switch (lvnum)
        {
            case 0:
                m_BGMAudio.clip = null;
                m_BGMAudio.volume = mainMenuVolum;
                break;
            case 1:
                m_BGMAudio.clip = A_lv1;
                m_BGMAudio.volume = lv1_Volum;
                break;
            case 2:
                m_BGMAudio.clip = A_lv2;
                m_BGMAudio.volume = lv2_Volum;
                break;
            case 3:
                m_BGMAudio.clip = A_lv3;
                m_BGMAudio.volume = lv3_Volum;
                break;
            case 4:
                m_BGMAudio.clip = A_lv4;
                m_BGMAudio.volume = lv4_Volum;
                break;
            case 5:
                m_BGMAudio.clip = A_lv5;
                m_BGMAudio.volume = lv5_Volum;
                break;
            default:
                //m_BGMAudio.clip = A_lv4;
                break;
        }
        m_BGMAudio.Play();
    }

    public void BGMStart()
    {
        m_BGMAudio.clip = A_BGMAll;
        m_BGMAudio.Play();
    }

}

