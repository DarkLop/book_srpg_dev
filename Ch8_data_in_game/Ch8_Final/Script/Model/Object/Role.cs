#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				Role.cs
/// Author:					DarkRabbit
/// Create Time:			Fri, 02 Nov 2018 16:36:06 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using UnityEngine;

namespace DR.Book.SRPG_Dev.Models
{
    public abstract class Role
    {
        #region Field
        protected Weapon m_EquipedWeapon = null;
        protected readonly Item[] m_Items = new Item[SettingVars.k_RoleItemCount];
        #endregion

        #region Property
        protected RoleData self { get; set; }

        public ulong guid
        {
            get { return self.guid; }
        }

        public abstract RoleType roleType { get; }

        public int characterId
        {
            get { return self.characterId; }
        }

        public Character character
        {
            get
            {
                RoleModel model = ModelManager.models.Get<RoleModel>();
                return model.GetOrCreateCharacter(self.characterId);
            }
        }

        public int classId
        {
            get { return self.classId; }
        }

        public Class cls
        {
            get
            {
                RoleModel model = ModelManager.models.Get<RoleModel>();
                return model.GetOrCreateClass(self.characterId);
            }
        }

        public MoveConsumption moveConsumption
        {
            get { return cls.moveConsumption; }
        }

        public AttitudeTowards attitudeTowards
        {
            get { return self.attitudeTowards; }
        }

        public int level
        {
            get { return self.level; }
        }

        public virtual int exp
        {
            get { return self.exp; }
        }

        public FightProperties fightProperties
        {
            get { return self.fightProperties; }
        }

        public int maxHp
        {
            get { return self.fightProperties.hp; }
        }

        public int maxMp
        {
            get { return self.fightProperties.mp; }
        }

        public int hp
        {
            get { return self.hp; }
        }

        public int mp
        {
            get { return self.mp; }
        }

        public int luk
        {
            get { return self.luk; }
        }

        public int money
        {
            get { return self.money; }
        }

        public float movePoint
        {
            get { return self.movePoint; }
        }

        public virtual Weapon equipedWeapon
        {
            get { return m_EquipedWeapon; }
        }

        public Item[] items
        {
            get { return m_Items; }
        }

        public bool isDead
        {
            get { return self.hp <= 0; }
        }
        #endregion

        #region Combat Property
        /// <summary>
        /// 物理攻击力
        /// </summary>
        public int attack
        {
            get
            {
                if (equipedWeapon == null)
                {
                    return 0;
                }

                int atk = equipedWeapon.uniqueInfo.attack;
                atk += fightProperties[FightPropertyType.STR];
                atk += GetItemFightPropertySum(FightPropertyType.STR);
                return atk;
            }
        }

        /// <summary>
        /// 魔法攻击力
        /// </summary>
        public int mageAttack
        {
            get
            {
                if (equipedWeapon == null)
                {
                    return 0;
                }

                int mag = equipedWeapon.uniqueInfo.attack;
                mag += fightProperties[FightPropertyType.MAG];
                mag += GetItemFightPropertySum(FightPropertyType.MAG);
                return mag;
            }
        }

        /// <summary>
        /// 物理防御力
        /// </summary>
        public int defence
        {
            get
            {
                int def = fightProperties[FightPropertyType.DEF];
                def += GetItemFightPropertySum(FightPropertyType.DEF);
                return def;
            }
        }

        /// <summary>
        /// 魔法防御力
        /// </summary>
        public int mageDefence
        {
            get
            {
                int mdf = fightProperties[FightPropertyType.MDF];
                mdf += GetItemFightPropertySum(FightPropertyType.MDF);
                return mdf;
            }
        }

        /// <summary>
        /// 攻速
        /// </summary>
        public int speed
        {
            get
            {
                int spd = fightProperties[FightPropertyType.SPD];
                spd += GetItemFightPropertySum(FightPropertyType.SPD);
                if (equipedWeapon != null)
                {
                    spd -= equipedWeapon.uniqueInfo.weight;
                }
                return spd;
            }
        }

        /// <summary>
        /// 命中率
        /// </summary>
        public int hit
        {
            get
            {
                if (equipedWeapon == null)
                {
                    return 0;
                }

                int skl = fightProperties[FightPropertyType.SKL];
                skl += GetItemFightPropertySum(FightPropertyType.SKL);
                int hit = equipedWeapon.uniqueInfo.hit + skl * 2;
                return hit;
            }
        }

        /// <summary>
        /// 回避率
        /// </summary>
        public int avoidance
        {
            get
            {
                int spd = fightProperties[FightPropertyType.SPD];
                spd += GetItemFightPropertySum(FightPropertyType.SPD);
                int avd = spd * 2 + luk + GetItemLukSum();
                return avd;
            }
        }
        #endregion

        #region Constructor
        protected Role()
        {
            self = new RoleData();
        }

        protected Role(ulong guid) : this()
        {
            self.guid = guid;
        }
        #endregion

        #region Method
        public virtual bool Load(RoleData data)
        {
            if (data == null)
            {
                return false;
            }
            data.CopyTo(self);
            return true;
        }

        public virtual void LevelUp()
        {

        }

        /// <summary>
        /// 获取物品空位
        /// </summary>
        /// <returns></returns>
        protected int GetNullItemIndex()
        {
            int index = -1;
            for (int i = 0; i < SettingVars.k_RoleItemCount; i++)
            {
                if (items[i] == null)
                {
                    index = i;
                    break;
                }
            }

            return index;
        }

        /// <summary>
        /// 添加物品
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual int AddItem(Item item)
        {
            if (item == null)
            {
                return -1;
            }

            int index = GetNullItemIndex();

            if (index != -1)
            {
                items[index] = item;

                // 如果是武器，判断装备的武器是否为null
                if (item.itemType == ItemType.Weapon && equipedWeapon == null)
                {
                    m_EquipedWeapon = item as Weapon;
                }
            }

            return index;
        }

        /// <summary>
        /// 移除物品
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Item RemoveItem(int index)
        {
            if (index < 0 || index >= SettingVars.k_RoleItemCount || items[index] == null)
            {
                return null;
            }

            Item item = items[index];
            items[index] = null;

            // 如果是装备的武器
            if (item.itemType == ItemType.Weapon && equipedWeapon == item)
            {
                m_EquipedWeapon = null;

                for (int i = 0; i < SettingVars.k_RoleItemCount; i++)
                {
                    if (m_Items[i] != null && m_Items[i].itemType == ItemType.Weapon)
                    {
                        m_EquipedWeapon = m_Items[i] as Weapon;
                        break;
                    }
                }
            }

            return item;
        }

        /// <summary>
        /// 交换物品
        /// </summary>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        /// <returns></returns>
        public bool SwapItem(int index1, int index2)
        {
            if (index1 < 0 || index1 >= SettingVars.k_RoleItemCount
                || index2 < 0 || index2 >= SettingVars.k_RoleItemCount)
            {
                return false;
            }

            Item tmp = items[index1];
            items[index1] = items[index2];
            items[index2] = tmp;
            return true;
        }
        #endregion

        #region Helper
        public int GetItemFightPropertySum(FightPropertyType type)
        {
            if (type == FightPropertyType.MaxLength)
            {
                return 0;
            }

            int value = 0;

            if (equipedWeapon != null)
            {
                value += equipedWeapon.uniqueInfo.fightProperties[type];
            }

            foreach (Item item in items)
            {
                if (item != null && item.itemType == ItemType.Ornament)
                {
                    value += (item as Ornament).uniqueInfo.fightProperties[type];
                }
            }

            return value;
        }

        public int GetItemLukSum()
        {
            int value = 0;

            if (equipedWeapon != null)
            {
                value += equipedWeapon.uniqueInfo.luk;
            }

            foreach (Item item in items)
            {
                if (item != null && item.itemType == ItemType.Ornament)
                {
                    value += (item as Ornament).uniqueInfo.luk;
                }
            }

            return value;
        }
        #endregion
    }

    public class UniqueRole : Role
    {
        public sealed override RoleType roleType
        {
            get { return RoleType.Unique; }
        }

        public UniqueRole() : base()
        {

        }
    }

    public class FollowingRole : Role
    {
        public sealed override RoleType roleType
        {
            get { return RoleType.Following; }
        }

        public FollowingRole(ulong guid) : base(guid)
        {

        }
    }
}