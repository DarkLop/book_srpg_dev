#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				ItemInfo.cs
/// Author:					DarkRabbit
/// Create Time:			Sat, 06 Oct 2018 03:53:36 GMT
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
    public class ItemInfoConfig : BaseXmlConfig<int, ItemInfo>
    {
    }

    [Serializable]
    public class ItemInfo : IConfigData<int>, ISerializationCallbackReceiver
    {
        /// <summary>
        /// 物品ID
        /// </summary>
        [XmlAttribute]
        public int id;

        /// <summary>
        /// 名称
        /// </summary>
        [XmlAttribute]
        public string name;

        /// <summary>
        /// 图标
        /// </summary>
        [XmlAttribute]
        public string icon;

        /// <summary>
        /// 价格，当价格是-1时，不可买卖与交易
        /// </summary>
        [XmlAttribute]
        public int price;

        /// <summary>
        /// 分类
        /// </summary>
        [XmlIgnore]
        public ItemType itemType;

        /// <summary>
        /// 物品独有信息
        /// </summary>
        [XmlChoiceIdentifier("itemType")]
        [XmlElement("Unknow", typeof(UniqueInfo), IsNullable = true)]
        [XmlElement("Weapon", typeof(WeaponUniqueInfo))]
        [XmlElement("Ornament", typeof(OrnamentUniqueInfo))]
        [XmlElement("Consumable", typeof(ConsumableUniqueInfo))]
        public UniqueInfo uniqueInfo;

        public int GetKey()
        {
            return this.id;
        }

#if UNITY_EDITOR
        [Serializable]
        public class EditorUniqueInfoCollection
        {
            public WeaponUniqueInfo weaponUniqueInfo = new WeaponUniqueInfo();
            public OrnamentUniqueInfo ornamentUniqueInfo = new OrnamentUniqueInfo();
            public ConsumableUniqueInfo consumableUniqueInfo = new ConsumableUniqueInfo();

            public UniqueInfo this[ItemType itemType]
            {
                get
                {
                    switch (itemType)
                    {
                        case ItemType.Weapon:
                            return weaponUniqueInfo;
                        case ItemType.Ornament:
                            return ornamentUniqueInfo;
                        case ItemType.Consumable:
                            return consumableUniqueInfo;
                    }
                    return null;
                }
                set
                {
                    if (value == null)
                    {
                        return;
                    }

                    Type type = value.GetType();

                    if (type == typeof(UniqueInfo))
                    {
                        return;
                    }

                    if (type == typeof(WeaponUniqueInfo))
                    {
                        weaponUniqueInfo = value as WeaponUniqueInfo;
                    }
                    else if (type == typeof(OrnamentUniqueInfo))
                    {
                        ornamentUniqueInfo = value as OrnamentUniqueInfo;
                    }
                    else if (type == typeof(ConsumableUniqueInfo))
                    {
                        consumableUniqueInfo = value as ConsumableUniqueInfo;
                    }
                }
            }
        }
        [SerializeField, XmlIgnore]
        private EditorUniqueInfoCollection editorUniqueInfo;
#endif

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
#if UNITY_EDITOR
            if (editorUniqueInfo != null)
            {
                editorUniqueInfo[itemType] = uniqueInfo;
            }
#endif
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
#if UNITY_EDITOR
            if (editorUniqueInfo != null)
            {
                uniqueInfo = editorUniqueInfo[itemType];
            }
#endif
        }
    }
}