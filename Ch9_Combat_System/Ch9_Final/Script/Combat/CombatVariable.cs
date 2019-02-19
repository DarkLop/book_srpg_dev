#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				CombatStep.cs
/// Author:					DarkRabbit
/// Create Time:			Fri, 09 Nov 2018 15:42:11 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;

namespace DR.Book.SRPG_Dev.CombatManagement
{
    [Serializable]
    public struct CombatVariable
    {
        /// <summary>
        /// 位置
        /// </summary>
        public int position;

        /// <summary>
        /// 生命值
        /// </summary>
        public int hp;

        /// <summary>
        /// 魔法值
        /// </summary>
        public int mp;

        /// <summary>
        /// 是否可攻击
        /// </summary>
        public bool canAtk;

        /// <summary>
        /// FE4武器耐久度（机器人大战技能数量）
        /// </summary>
        public int durability;

        /// <summary>
        /// 动画类型
        /// </summary>
        public CombatAnimaType animaType;

        /// <summary>
        /// 是否爆击
        /// </summary>
        public bool crit;

        /// <summary>
        /// 是否行动过
        /// </summary>
        public bool action;

        /// <summary>
        /// 是否已经死亡
        /// </summary>
        public bool isDead
        {
            get { return hp <= 0; }
        }

        public CombatVariable(int position, int hp, int mp, bool canAtk, CombatAnimaType animaType)
        {
            this.position = position;
            this.hp = hp;
            this.mp = mp;
            this.canAtk = canAtk;
            this.durability = 0;
            this.animaType = animaType;
            this.crit = false;
            this.action = false;
        }

        public CombatVariable(int position, int hp, int mp, bool canAtk, int durability, CombatAnimaType animaType)
        {
            this.position = position;
            this.hp = hp;
            this.mp = mp;
            this.canAtk = canAtk;
            this.durability = durability;
            this.animaType = animaType;
            this.crit = false;
            this.action = false;
        }

        public void ResetAnima()
        {
            this.animaType = CombatAnimaType.Unknow;
            this.crit = false;
        }

        public CombatVariable Clone()
        {
            CombatVariable variable = new CombatVariable()
            {
                position = position,
                hp = hp,
                mp = mp,
                canAtk = canAtk,
                durability = durability,
                animaType = animaType,
                crit = crit,
                action = action
            };
            return variable;
        }
    }
}