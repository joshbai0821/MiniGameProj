using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniProj
{
    [System.Serializable]
    public class SkillData
    {
        [SerializeField]
        private SkillId m_id;

        [SerializeField]
        private int m_count;

        public SkillId Id
        {
            get
            {
                return m_id;
            }
        }

        public int Count
        {
            get
            {
                return m_count;
            }
        }
    }

    [System.Serializable]
    public class EnemyConfig
    {
        [SerializeField]
        private int m_type;

        [SerializeField]
        private MapPos m_pos;

        public int Type
        {
            get { return m_type; }
        }

        public MapPos Pos
        {
            get { return m_pos; }
        }
    }

    [System.Serializable]
    public class NpcConfig
    {
        [SerializeField]
        public List<MapPos> m_npcPosData;
    }

    [System.Serializable]
    public class ArrowConfig
    {
        //攻击区域
        [SerializeField]
        public List<MapPos> m_AttackArea;

        //触发区域
        [SerializeField]
        public List<MapPos> m_TriggerArea;

        //触发角色
        [SerializeField]
        public List<PlayerType> m_Trigger;
    }

    [System.Serializable]
    public class RockConfig
    {
        [SerializeField]
        public MapPos m_RockPos;
        //滚动方向
        [SerializeField]
        public int dir;
        //触发角色
        [SerializeField]
        public List<PlayerType> m_Trigger;
    }

    [System.Serializable]
    public class SingleSceneConfig
    {
        [SerializeField]
        private string m_prefabName;
        [SerializeField]
        private string m_backgroundName;
        [SerializeField]
        private string m_sceneTargetName;
        [SerializeField]
        private int m_mapRow;
        [SerializeField]
        private int m_mapCol;
        [SerializeField]
        private int m_playerStartRow;
        [SerializeField]
        private int m_playerStartCol;
        [SerializeField]
        private List<SkillData> m_skillData;
        [SerializeField]
        private List<EnemyConfig> m_enemyData;
        [SerializeField]
        private List<NpcConfig> m_npcPosData;
        [SerializeField]
        private List<ArrowConfig> m_Arrow;
        [SerializeField]
        private RockConfig m_Rock;

        public string PrefabName
        {
            get { return m_prefabName; }
        }
        public string BackGroundName
        {
            get { return m_backgroundName; }
        }
        public string SceneTargetName
        {
            get { return m_sceneTargetName; }
        }
        public int MapRow
        {
            get { return m_mapRow; }
        }
        public int MapCol
        {
            get { return m_mapCol; }
        }
        public int PlayerStartRow
        {
            get { return m_playerStartRow; }
        }
        public int PlayerStartCol
        {
            get { return m_playerStartCol; }
        }
        public List<SkillData> SkillData
        {
            get { return m_skillData; }
        }
        public List<EnemyConfig> EnemyData
        {
            get { return m_enemyData; }
        }
        public List<NpcConfig> NpcPosData
        {
            get { return m_npcPosData; }
        }

        public List<ArrowConfig> ArrowData
        {
            get { return m_Arrow; }
        }

        public RockConfig RockData
        {
            get { return m_Rock; }
        }
    }

    [System.Serializable]
    public class SceneConfig : ScriptableObject
    {
        [SerializeField]
        private List<SingleSceneConfig> m_sceneConfigList;
        public List<SingleSceneConfig> SceneConfigList
        {
            get { return m_sceneConfigList; }
        }
    }

}
