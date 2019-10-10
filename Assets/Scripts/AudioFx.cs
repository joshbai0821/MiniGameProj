using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioFx : MonoBehaviour {

    private AudioSource m_FXAudio;

    public AudioClip A_chooseLv;
    public float A_chooseLv_Volum;

    public AudioClip A_click;
    public float A_click_Volum;

    public AudioClip A_ma;
    public float A_ma_Volum;

    public AudioClip A_che;
    public float A_che_Volum;

    public AudioClip A_xiang;
    public float A_xiang_Volum;

    public AudioClip A_down;
    public float A_down_Volum;

    public AudioClip A_hit;
    public float A_hit_Volum;

    public AudioClip A_bridge;
    public float A_bridge_Volum;

    public AudioClip A_arrow;
    public float A_arrow_Volum;

    public AudioClip A_enemy;
    public float A_enemy_Volum;


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
        m_FXAudio.volume = A_chooseLv_Volum;
        m_FXAudio.Play();
    }

    public void clickMenu()
    {
        m_FXAudio.clip = A_click;
        m_FXAudio.volume = A_click_Volum;
        m_FXAudio.Play();
    }

    public void clickskill(int skillid)
    {
        switch (skillid)
        {
            case 0:
                m_FXAudio.clip = A_che;
                m_FXAudio.volume = A_che_Volum;
                break;
            case 1:
                m_FXAudio.clip = A_ma;
                m_FXAudio.volume = A_ma_Volum;
                break;
            case 3:
                m_FXAudio.clip = A_xiang;
                m_FXAudio.volume = A_xiang_Volum;
                break;
            default:
                break;
        }
        m_FXAudio.Play();
    }

    public void pawndown()
    {
        m_FXAudio.clip = A_down;
        m_FXAudio.volume = A_down_Volum;
        m_FXAudio.Play();
    }

    public void pawnhit()
    {
        m_FXAudio.clip = A_hit;
        m_FXAudio.volume = A_hit_Volum;
        m_FXAudio.Play();
    }

    public void enemymovesound()
    {
        m_FXAudio.clip = A_enemy;
        m_FXAudio.volume = A_enemy_Volum;
        m_FXAudio.Play();
    }


}
