#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				FightProperties.cs
/// Author:					DarkRabbit
/// Create Time:			Fri, 05 Oct 2018 01:46:34 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using System.Xml.Serialization;
using UnityEngine;

namespace DR.Book.SRPG_Dev.Models
{
    /// <summary>
    /// 战斗属性
    /// hp, mp, str, mag, skl, spd, def, mdf
    /// </summary>
    [Serializable]
    public struct FightProperties
    {
        /// <summary>
        /// 生命值
        /// </summary>
        [XmlAttribute]
        public int hp;

        /// <summary>
        /// 魔法值
        /// </summary>
        [XmlAttribute]
        public int mp;

        /// <summary>
        /// 力量
        /// </summary>
        [XmlAttribute]
        public int str;

        /// <summary>
        /// 魔力
        /// </summary>
        [XmlAttribute]
        public int mag;

        /// <summary>
        /// 技巧
        /// </summary>
        [XmlAttribute]
        public int skl;

        /// <summary>
        /// 速度
        /// </summary>
        [XmlAttribute]
        public int spd;

        /// <summary>
        /// 防御
        /// </summary>
        [XmlAttribute]
        public int def;

        /// <summary>
        /// 魔防
        /// </summary>
        [XmlAttribute]
        public int mdf;

        [XmlIgnore]
        public int this[FightPropertyType type]
        {
            get
            {
                switch (type)
                {
                    case FightPropertyType.HP:
                        return hp;
                    case FightPropertyType.MP:
                        return mp;
                    case FightPropertyType.STR:
                        return str;
                    case FightPropertyType.MAG:
                        return mag;
                    case FightPropertyType.SKL:
                        return skl;
                    case FightPropertyType.SPD:
                        return spd;
                    case FightPropertyType.DEF:
                        return def;
                    case FightPropertyType.MDF:
                        return mdf;
                    default:
                        return 0;
                        //throw new IndexOutOfRangeException("Not Supported");
                }
            }
            //set
            //{
            //    switch (type)
            //    {
            //        case FightPropertyType.STR:
            //            str = value;
            //            break;
            //        case FightPropertyType.MAG:
            //            mag = value;
            //            break;
            //        case FightPropertyType.SKL:
            //            skl = value;
            //            break;
            //        case FightPropertyType.SPD:
            //            spd = value;
            //            break;
            //        case FightPropertyType.DEF:
            //            def = value;
            //            break;
            //        case FightPropertyType.MDF:
            //            mdf = value;
            //            break;
            //        default:
            //            throw new IndexOutOfRangeException("Not Supported");
            //    }
            //}
        }

        public static FightProperties operator +(FightProperties lhs, FightProperties rhs)
        {
            FightProperties fight = new FightProperties
            {
                hp = lhs.hp + rhs.hp,
                mp = lhs.mp + rhs.mp,
                str = lhs.str + rhs.str,
                mag = lhs.mag + rhs.mag,
                skl = lhs.skl + rhs.skl,
                spd = lhs.spd + rhs.spd,
                def = lhs.def + rhs.def,
                mdf = lhs.mdf + rhs.mdf,
            };
            return fight;
        }

        public static FightProperties operator +(FightProperties lhs, int rhs)
        {
            FightProperties fight = new FightProperties
            {
                hp = lhs.hp + rhs,
                mp = lhs.mp + rhs,
                str = lhs.str + rhs,
                mag = lhs.mag + rhs,
                skl = lhs.skl + rhs,
                spd = lhs.spd + rhs,
                def = lhs.def + rhs,
                mdf = lhs.mdf + rhs,
            };
            return fight;
        }

        public static FightProperties operator *(FightProperties lhs, FightProperties rhs)
        {
            FightProperties fight = new FightProperties
            {
                hp = lhs.hp * rhs.hp,
                mp = lhs.mp * rhs.mp,
                str = lhs.str * rhs.str,
                mag = lhs.mag * rhs.mag,
                skl = lhs.skl * rhs.skl,
                spd = lhs.spd * rhs.spd,
                def = lhs.def * rhs.def,
                mdf = lhs.mdf * rhs.mdf,
            };
            return fight;
        }

        public static FightProperties operator *(FightProperties lhs, int rhs)
        {
            FightProperties fight = new FightProperties
            {
                hp = lhs.hp * rhs,
                mp = lhs.mp * rhs,
                str = lhs.str * rhs,
                mag = lhs.mag * rhs,
                skl = lhs.skl * rhs,
                spd = lhs.spd * rhs,
                def = lhs.def * rhs,
                mdf = lhs.mdf * rhs,
            };
            return fight;
        }

        public static FightProperties operator /(FightProperties lhs, int rhs)
        {
            if (rhs == 0)
            {
                return lhs;
            }

            FightProperties fight = new FightProperties
            {
                hp = lhs.hp / rhs,
                mp = lhs.mp / rhs,
                str = lhs.str / rhs,
                mag = lhs.mag / rhs,
                skl = lhs.skl / rhs,
                spd = lhs.spd / rhs,
                def = lhs.def / rhs,
                mdf = lhs.mdf / rhs,
            };
            return fight;
        }

        public static FightProperties Clamp(FightProperties value, FightProperties max)
        {
            FightProperties fight = new FightProperties
            {
                hp = Mathf.Clamp(value.hp, 1, max.hp),
                mp = Mathf.Clamp(value.mp, 0, max.mp),
                str = Mathf.Clamp(value.str, 0, max.str),
                mag = Mathf.Clamp(value.mag, 0, max.mag),
                skl = Mathf.Clamp(value.skl, 0, max.skl),
                spd = Mathf.Clamp(value.spd, 0, max.spd),
                def = Mathf.Clamp(value.def, 0, max.def),
                mdf = Mathf.Clamp(value.mdf, 0, max.mdf)
            };
            return fight;
        }
    }
}