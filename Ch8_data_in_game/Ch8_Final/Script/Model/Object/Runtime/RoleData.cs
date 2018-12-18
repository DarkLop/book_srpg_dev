#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				RoleData.cs
/// Author:					DarkRabbit
/// Create Time:			Fri, 09 Nov 2018 18:23:59 GMT
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
    public class RoleData : RuntimeData<RoleData>
    {
        [XmlAttribute]
        public ulong guid;

        [XmlAttribute]
        public int characterId;

        [XmlAttribute]
        public int classId;

        [XmlAttribute]
        public AttitudeTowards attitudeTowards;

        [XmlAttribute]
        public int level = 1;

        [XmlAttribute]
        public int exp = 0;

        [XmlAttribute]
        public int hp = 1;

        [XmlAttribute]
        public int mp;

        [XmlElement]
        public FightProperties fightProperties;

        [XmlAttribute]
        public int luk;

        [XmlAttribute]
        public int money;

        [XmlAttribute]
        public float movePoint;

        public override void CopyTo(RoleData data)
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

            data.characterId = characterId;
            data.classId = classId;
            data.level = level;
            data.exp = exp;
            data.hp = hp;
            data.mp = mp;
            data.fightProperties = fightProperties;
            data.luk = luk;
            data.money = money;
            data.movePoint = movePoint;
        }
    }
}