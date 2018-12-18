#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				AvailableWeapons.cs
/// Author:					DarkRabbit
/// Create Time:			Fri, 05 Oct 2018 11:21:02 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using System.Xml.Serialization;

namespace DR.Book.SRPG_Dev.Models
{
    /// <summary>
    /// 可用武器等级： 
    /// 0 不可用，
    /// 1 F，2 E，3 D，4 C，5 B，6 A，7 S，8 星
    /// </summary>
    [Serializable]
    public struct AvailableWeapons
    {
        /// <summary>
        /// 剑
        /// </summary>
        [XmlAttribute]
        public int sword;

        /// <summary>
        /// 枪
        /// </summary>
        [XmlAttribute]
        public int lance;

        /// <summary>
        /// 斧
        /// </summary>
        [XmlAttribute]
        public int axe;

        /// <summary>
        /// 弓
        /// </summary>
        [XmlAttribute]
        public int bow;

        /// <summary>
        /// 杖
        /// </summary>
        [XmlAttribute]
        public int staff;

        [XmlIgnore]
        public int this[WeaponType type]
        {
            get
            {
                switch (type)
                {
                    case WeaponType.Sword:
                        return sword;
                    // 其它由于没有动画，全部都禁用了Obsolete。
                    //case WeaponType.Lance:
                    //    return lance;
                    //case WeaponType.Axe:
                    //    return axe;
                    //case WeaponType.Bow:
                    //    return bow;
                    //case WeaponType.Staff:
                    //    return staff;
                    default:
                        return 0;
                        //throw new IndexOutOfRangeException("Not supported.");
                }
            }
        }
    }
}