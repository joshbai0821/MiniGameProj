using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio_BGM : MonoBehaviour {

    private AudioSource m_BGMAudio;

    public AudioClip A_BGMAll;
    public AudioClip A_lv1;
    public AudioClip A_lv2;
    public AudioClip A_lv3;
    public AudioClip A_lv4;

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
                    GameObject go = new GameObject("mAudioBGM");
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
                break;
            case 1:
                m_BGMAudio.clip = A_lv1;
                break;
            case 2:
                m_BGMAudio.clip = A_lv2;
                break;
            case 3:
                m_BGMAudio.clip = A_lv3;
                break;
            default:
                m_BGMAudio.clip = A_lv4;
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

