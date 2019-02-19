#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				BattleAction.cs
/// Author:					DarkRabbit
/// Create Time:			Thu, 20 Dec 2018 01:42:31 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using UnityEngine;

namespace DR.Book.SRPG_Dev.CombatManagement
{
    public enum BattleActionType
    {
        Unknow,
        Prepare,
        Attack,
        MageAttack,
        Heal,

        // 其它自定义类型
    }

    public abstract class BattleAction : ScriptableObject
    {
        private string m_Message = "Unknow battle message.";

        public string message
        {
            get { return m_Message; }
            protected set { m_Message = value; }
        }

        public abstract BattleActionType actionType { get; }

        public abstract CombatStep CalcBattle(Combat combat, CombatVariable atkVal, CombatVariable defVal);

        public abstract bool IsBattleEnd(Combat combat, CombatVariable atkVal, CombatVariable defVal);

        public sealed override string ToString()
        {
            return m_Message;
        }
    }
}