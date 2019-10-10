using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MiniProj;

public class winpanelcontrol : MonoBehaviour {

    private Text mission1;
    private Text mission2;
    private Text mission3;

    private Transform gou1;
    private Transform gou2;
    private Transform gou3;

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Lvmission()
    {
        mission1 = transform.GetChild(0).GetComponent<Text>();
        mission2 = transform.GetChild(1).GetComponent<Text>();
        mission3 = transform.GetChild(2).GetComponent<Text>();
        gou1 = mission1.transform.GetChild(0);
        gou2 = mission2.transform.GetChild(0);
        gou3 = mission3.transform.GetChild(0);

        MissionList.Instance.Judgemission();
        if (GameManager.SceneConfigId == 0)
        {
            mission1.text = MissionList.Instance.m_mission1[0];
            mission2.text = MissionList.Instance.m_mission1[1];
            mission3.text = MissionList.Instance.m_mission1[2];
            if (MissionList.Instance.m_mission1_1)
            {
                gou1.gameObject.SetActive(true);
            }
            if (MissionList.Instance.m_mission1_2)
            {
                gou2.gameObject.SetActive(true);
            }
            if (MissionList.Instance.m_mission1_3)
            {
                gou3.gameObject.SetActive(true);
            }
        }
        else if (GameManager.SceneConfigId == 1)
        {
            mission1.text = MissionList.Instance.m_mission2[0];
            mission2.text = MissionList.Instance.m_mission2[1];
            mission3.text = MissionList.Instance.m_mission2[2];
            if (MissionList.Instance.m_mission2_1)
            {
                gou1.gameObject.SetActive(true);
            }
            if (MissionList.Instance.m_mission2_2)
            {
                gou2.gameObject.SetActive(true);
            }
            if (MissionList.Instance.m_mission2_3)
            {
                gou3.gameObject.SetActive(true);
            }
        }
        else if (GameManager.SceneConfigId == 2)
        {
            mission1.text = MissionList.Instance.m_mission3[0];
            mission2.text = MissionList.Instance.m_mission3[1];
            mission3.text = MissionList.Instance.m_mission3[2];
            if (MissionList.Instance.m_mission3_1)
            {
                gou1.gameObject.SetActive(true);
            }
            if (MissionList.Instance.m_mission3_2)
            {
                gou2.gameObject.SetActive(true);
            }
            if (MissionList.Instance.m_mission3_3)
            {
                gou3.gameObject.SetActive(true);
            }
        }
        else if (GameManager.SceneConfigId == 3)
        {
            mission1.text = MissionList.Instance.m_mission4[0];
            mission2.text = MissionList.Instance.m_mission4[1];
            mission3.text = MissionList.Instance.m_mission4[2];
            if (MissionList.Instance.m_mission4_1)
            {
                gou1.gameObject.SetActive(true);
            }
            if (MissionList.Instance.m_mission4_2)
            {
                gou2.gameObject.SetActive(true);
            }
            if (MissionList.Instance.m_mission4_3)
            {
                gou3.gameObject.SetActive(true);
            }
        }
        else
        {
            mission1.text = MissionList.Instance.m_mission5[0];
            mission2.text = MissionList.Instance.m_mission5[1];
            mission3.text = MissionList.Instance.m_mission5[2];
            if (MissionList.Instance.m_mission5_1)
            {
                gou1.gameObject.SetActive(true);
            }

        }

    }
}
