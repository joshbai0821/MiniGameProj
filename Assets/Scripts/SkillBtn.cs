using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace MiniProj
{
    public enum SkillId
    {
        NONE = -1,
        JU = 0,
        MA = 1,
        PAO = 2,
        XIANG = 3,
        SHI = 4,
        BING = 5,
    }
    public class SkillBtn : MonoBehaviour
    {
        public string[] SkillName =
        {
            "车","马","炮","相","士","兵",
        };

        private SkillId m_id;
        public SkillId Id
        {
            get { return m_id; }
        }
        private int m_count;
        public int Count
        {
            get { return m_count; }
        }

        private Text m_skillCountText;
        private Text m_skillNameText;

        private void Awake()
        {
            Button _btn = this.GetComponent<Button>();
            _btn.onClick.AddListener(UseSkill);
            Transform _tranCount = this.transform.Find("SkillCount");
            m_skillCountText = _tranCount.GetComponent<Text>();
            Transform _tranName = this.transform.Find("SkillName");
            m_skillNameText = _tranName.GetComponent<Text>();
            EventManager.RegisterEvent(HLEventId.PLAYER_START_MOVE, this.GetHashCode(), PlayerMove);
        }

        public void ShowNewModeTips()
        {

        }

        private void OnDestroy()
        {
            EventManager.UnregisterEvent(HLEventId.PLAYER_START_MOVE, this.GetHashCode());
        }

        public void Initial(SkillId id, int count)
        {
            m_id = id;
            m_count = count;
            m_skillCountText.text = m_count.ToString();
            m_skillNameText.text = SkillName[(int)m_id];
        }

        private void UseSkill()
        {
            SceneModule _sceneModule = (SceneModule)GameManager.GameManagerObj.GetComponent<GameManager>().GetModuleByName("SceneModule");
            if (_sceneModule.isPlayerReady() && m_count > 0)
            {
                IntEventArgs args = new IntEventArgs((int)m_id);
                EventManager.SendEvent(HLEventId.USE_SKILL, args);
            }
        }

        private void PlayerMove(EventArgs args)
        {
            SkillId _skillId = (SkillId)((IntEventArgs)args).m_args;
            if (_skillId == m_id)
            {
                m_count--;
                m_skillCountText.text = m_count.ToString();
                if(m_count == 0)
                {
                    this.GetComponent<Button>().interactable = false;
                }
            }

        }
    }
}


