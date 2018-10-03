#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				FightProperty.cs
/// Author:					DarkRabbit
/// Create Time:			Fri, 06 Apr 2018 21:30:39 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using System.Xml.Serialization;

namespace DR.Book.SRPG_Dev.Models.Old
{
    [Serializable]
    public struct FightProperty
    {
        /// <summary>
        /// 力量
        /// </summary>
        [XmlAttribute]
        public int str { get; set; }

        /// <summary>
        /// 魔力
        /// </summary>
        [XmlAttribute]
        public int mag { get; set; }

        /// <summary>
        /// 技巧
        /// </summary>
        [XmlAttribute]
        public int skl { get; set; }

        /// <summary>
        /// 速度
        /// </summary>
        [XmlAttribute]
        public int spd { get; set; }

        /// <summary>
        /// 防御
        /// </summary>
        [XmlAttribute]
        public int def { get; set; }

        /// <summary>
        /// 魔防
        /// </summary>
        [XmlAttribute]
        public int mdf { get; set; }

        public int this[FightPropertyType type]
        {
            get
            {
                switch (type)
                {
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
                        throw new IndexOutOfRangeException("Not Supported");
                }
            }
        }

        public static FightProperty operator +(FightProperty lhs, FightProperty rhs)
        {
            FightProperty fight = new FightProperty
            {
                str = lhs.str + rhs.str,
                mag = lhs.mag + rhs.mag,
                skl = lhs.skl + rhs.skl,
                spd = lhs.spd + rhs.spd,
                def = lhs.def + rhs.def,
                mdf = lhs.mdf + rhs.mdf,
            };
            return fight;
        }
    }
}