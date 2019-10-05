using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioFx : MonoBehaviour {

    private AudioSource m_FXAudio;

    public AudioClip A_chooseLv;
    public AudioClip A_click;
    public AudioClip A_ma;
    public AudioClip A_che;
    public AudioClip A_xiang;
    public AudioClip A_down;
    public AudioClip A_hit;

    //私有的静态实例
    private static AudioFx _instance = null;
    //共有的唯一的，全局访问点
    public static AudioFx Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<AudioFx>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("mAudioFx");
                    _instance = go.AddComponent<AudioFx>();
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
        m_FXAudio = GetComponent<AudioSource>();
    }

    public void clicktochooselv()
    {
        m_FXAudio.clip = A_chooseLv;
        m_FXAudio.Play();
    }

    public void clickMenu()
    {
        m_FXAudio.clip = A_click;
        m_FXAudio.Play();
    }

    public void clickskill(int skillid)
    {
        switch (skillid)
        {
            case 0:
                m_FXAudio.clip = A_che;
                break;
            case 1:
                m_FXAudio.clip = A_ma;
                break;
            case 3:
                m_FXAudio.clip = A_xiang;
                break;
            default:
                break;
        }
        m_FXAudio.Play();
    }

    public void pawndown()
    {
        m_FXAudio.clip = A_down;
        m_FXAudio.Play();
    }

    public void pawnhit()
    {
        m_FXAudio.clip = A_hit;
        m_FXAudio.Play();
    }

}
