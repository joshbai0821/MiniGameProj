namespace MiniProj
{
    public enum HLEventId {
        GAME_BEGIN = 0,  // 游戏开始事件
        GAME_END,        // 游戏结束事件
        USE_SKILL,       // 点击按钮使用技能
        PLAYER_START_MOVE,        // 玩家开始移动
        PLAYER_END_MOVE,          // 玩家结束运动
        NPC_END_MOVE,     //NPC结束运动
        ROUND_FAIL,       //对局失败
        MAX_EVENT,       // 最大事件数量
    }
}
