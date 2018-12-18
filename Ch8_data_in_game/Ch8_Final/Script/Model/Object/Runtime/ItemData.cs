#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				ItemData.cs
/// Author:					DarkRabbit
/// Create Time:			Sun, 14 Oct 2018 14:02:53 GMT
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
    [Serializable]
    public class ItemData : RuntimeData<ItemData>
    {
        [XmlAttribute]
        public ulong guid;

        [XmlAttribute]
        public int itemId;

        [XmlAttribute]
        public int durability;

        [XmlAttribute]
        public Item.OwnerType ownerType;

        /// <summary>
        /// 所有者ID。
        /// 未知：可能在虚空中
        /// 角色：角色id，哪个角色
        /// 商店：商店id，哪个商店
        /// 地图：地图坐标的一位数组index，地图上哪个位置（暂定）
        /// </summary>
        [XmlAttribute]
        public int ownerId;

        public override ItemData Clone()
        {
            ItemData data = new ItemData()
            {
                itemId = itemId,
                durability = durability
            };
            return data;
        }

        public override void CopyTo(ItemData data)
        {
            if (data == null)
            {
                Debug.LogError("RuntimeData -> CopyTo: data is null.");
                return;
            }

            if (data == this)
            {
                return;
            }

            data.itemId = itemId;
            data.durability = durability;
        }
    }
}