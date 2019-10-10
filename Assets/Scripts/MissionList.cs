using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionList : MonoBehaviour {

    //私有的静态实例
    private static MissionList _instance = null;
    //共有的唯一的，全局访问点
    public static MissionList Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<MissionList>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("MissionList");
                    _instance = go.AddComponent<MissionList>();
                }
            }
            return _instance;
        }
    }

    public string[] m_mission1 = new string[3];
    public string[] m_mission2 = new string[3];
    public string[] m_mission3 = new string[3];
    public string[] m_mission4 = new string[3];
    public string[] m_mission5 = new string[3];

    public bool m_mission1_1 = false;
    public bool m_mission1_2 = false;
    public bool m_mission1_3 = false;

    public bool m_mission2_1 = false;
    public bool m_mission2_2 = false;
    public bool m_mission2_3 = false;

    public bool m_mission3_1 = false;
    public bool m_mission3_2 = false;
    public bool m_mission3_3 = false;

    public bool m_mission4_1 = false;
    public bool m_mission4_2 = false;
    public bool m_mission4_3 = false;

    public bool m_mission5_1 = false;

    public int enemykilled = 0;
    public int jutimes = 0;
    public int matimes = 0;
    public int xiangtimes = 0;


    // Use this for initialization
    void Start () {
        m_mission1[0] = "项羽抵达终点";
        m_mission1[1] = "击杀三名敌军";
        m_mission1[2] = "使用三种棋子";

        m_mission2[0] = "项羽抵达终点";
        m_mission2[1] = "击杀全部敌军";
        m_mission2[2] = "不使用相过关";

        m_mission3[0] = "保护虞姬抵达终点";
        m_mission3[1] = "击杀至少七名敌军";
        m_mission3[2] = "使用不超过一个车";

        m_mission4[0] = "保护虞姬抵达终点";
        m_mission4[1] = "不使用车过关";
        m_mission4[2] = "不击杀任何敌军";

        m_mission5[0] = "保护虞姬";
        m_mission5[1] = "力战到底";
        m_mission5[2] = "乌江自刎";

    }

    public void Judgemission()
    {
        if (enemykilled == 3)
        {
            m_mission2_2 = true;
        }
        else
        {
            m_mission2_2 = false;
        }
        if (xiangtimes == 0)
        {
            m_mission2_3 = true;
        }
        else
        {
            m_mission2_3 = false;
        }


        if (enemykilled >= 7)
        {
            m_mission3_2 = true;
        }
        else
        {
            m_mission3_2 = false;
        }
        if (jutimes <= 1)
        {
            m_mission3_3 = true;
        }
        else
        {
            m_mission3_3 = false;
        }


        
        if (jutimes == 0)
        {
            m_mission4_2 = true;
        }
        else
        {
            m_mission4_2 = false;
        }
        if (enemykilled == 0)
        {
            m_mission4_3 = true;
        }
        else
        {
            m_mission4_3 = false;
        }
    }

    public void Cleardata()
    {
        enemykilled = 0;
        jutimes = 0;
        matimes = 0;
        xiangtimes = 0;
    }
}
