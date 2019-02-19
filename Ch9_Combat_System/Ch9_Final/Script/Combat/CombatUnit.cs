#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				CombatUnit.cs
/// Author:					DarkRabbit
/// Create Time:			Fri, 09 Nov 2018 01:58:19 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;

namespace DR.Book.SRPG_Dev.CombatManagement
{
    using DR.Book.SRPG_Dev.Models;
    using DR.Book.SRPG_Dev.Maps;

    public class CombatUnit : IDisposable
    {
        public MapClass mapClass { get; private set; }

        public Role role { get { return mapClass.role; } }

        /// <summary>
        /// 战斗中的位置
        /// </summary>
        public int position { get; private set; }

        /// <summary>
        /// 生命值
        /// </summary>
        public int hp { get; private set; }

        /// <summary>
        /// 最大生命值
        /// </summary>
        public int maxHp { get; private set; }

        /// <summary>
        /// 魔法值
        /// </summary>
        public int mp { get; private set; }

        /// <summary>
        /// 最大魔法值
        /// </summary>
        public int maxMp { get; private set; }

        /// <summary>
        /// 攻击
        /// </summary>
        public int atk { get; private set; }

        /// <summary>
        /// 魔法攻击
        /// </summary>
        public int mageAtk { get; private set; }

        /// <summary>
        /// 防御
        /// </summary>
        public int def { get; private set; }

        /// <summary>
        /// 魔法防御
        /// </summary>
        public int mageDef { get; private set; }

        /// <summary>
        /// 攻速
        /// </summary>
        public int speed { get; private set; }

        /// <summary>
        /// 命中率
        /// </summary>
        public int hit { get; private set; }

        /// <summary>
        /// 爆击率
        /// </summary>
        public int crit { get; private set; }

        /// <summary>
        /// 回避率
        /// </summary>
        public int avoidance { get; private set; }

        /// <summary>
        /// 武器类型
        /// </summary>
        public WeaponType weaponType { get; private set; }

        /// <summary>
        /// 武器耐久度
        /// </summary>
        public int durability { get; private set; }

        public CombatUnit(int position)
        {
            this.position = position;
        }

        public bool Load(MapClass mapClass)
        {
            if (mapClass == null)
            {
                return false;
            }


            if (mapClass.role == null)
            {
                return false;
            }

            this.mapClass = mapClass;

            this.hp = role.hp;
            this.mp = role.mp;
            this.maxHp = role.maxHp;
            this.maxMp = role.maxMp;
            this.atk = role.attack;
            this.mageAtk = role.mageAttack;
            this.def = role.defence;
            this.mageDef = role.mageDefence;
            this.speed = role.speed;
            this.hit = role.hit;
            //this.crit = role.crit;
            this.avoidance = role.avoidance;
            if (role.equipedWeapon == null)
            {
                this.weaponType = WeaponType.Unknow;
                this.durability = 0;
            }
            else
            {
                this.weaponType = role.equipedWeapon.uniqueInfo.weaponType;
                this.durability = role.equipedWeapon.durability;
            }
            return true;
        }

        public void Dispose()
        {
            this.mapClass = null;
            this.position = -1;
        }

        public void ClearMapClass()
        {
            this.mapClass = null;
        }
    }
}