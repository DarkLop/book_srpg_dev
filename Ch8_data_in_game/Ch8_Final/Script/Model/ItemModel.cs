#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				ItemModel.cs
/// Author:					DarkRabbit
/// Create Time:			Sun, 14 Oct 2018 14:00:30 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DR.Book.SRPG_Dev.Models
{
    public class ItemModel : ModelBase, IDictionary<ulong, Item>
    {
        #region Static
        /// <summary>
        /// 生成的下一个物品的GUID
        /// </summary>
        public static ulong nextItemGUID { get; private set; }
        #endregion

        #region Field
        private Dictionary<ulong, Item> m_Items;
        private Dictionary<int, ItemData> m_ItemTemplates;
        #endregion

        #region IDictionary<int, Item> Properties
        public ICollection<ulong> Keys
        {
            get { return m_Items.Keys; }
        }

        public ICollection<Item> Values
        {
            get { return m_Items.Values; }
        }

        public int Count
        {
            get { return m_Items.Count; }
        }

        bool ICollection<KeyValuePair<ulong, Item>>.IsReadOnly
        {
            get { return ((ICollection<KeyValuePair<ulong, Item>>)m_Items).IsReadOnly; }
        }

        public Item this[ulong guid]
        {
            get
            {
                Item data;
                if (!m_Items.TryGetValue(guid, out data))
                {
                    return null;
                }

                return data;
            }
            set { throw new NotImplementedException("Readonly."); }
        }
        #endregion

        #region Load
        protected override void OnLoad()
        {
            nextItemGUID = 1UL;
            m_Items = new Dictionary<ulong, Item>();
            m_ItemTemplates = new Dictionary<int, ItemData>();
        }

        protected override void OnDispose()
        {
            ulong[] keys = m_Items.Keys.ToArray();
            for (int i = 0; i < keys.Length; i++)
            {
                m_Items[keys[i]].Dispose();
            }
            m_Items = null;
            m_ItemTemplates = null;
        }
        #endregion

        #region IDictionary<int, Item> Method
        void IDictionary<ulong, Item>.Add(ulong guid, Item value)
        {
            throw new NotImplementedException("Readonly.");
        }

        public bool ContainsKey(ulong guid)
        {
            return m_Items.ContainsKey(guid);
        }

        public bool Remove(ulong guid)
        {
            return m_Items.Remove(guid);
        }

        public bool TryGetValue(ulong guid, out Item value)
        {
            return m_Items.TryGetValue(guid, out value);
        }

        void ICollection<KeyValuePair<ulong, Item>>.Add(KeyValuePair<ulong, Item> item)
        {
            throw new NotImplementedException("Readonly.");
        }

        void ICollection<KeyValuePair<ulong, Item>>.Clear()
        {
            throw new NotImplementedException("Readonly.");
        }

        bool ICollection<KeyValuePair<ulong, Item>>.Contains(KeyValuePair<ulong, Item> item)
        {
            return ((ICollection<KeyValuePair<ulong, Item>>)m_Items).Contains(item);
        }

        void ICollection<KeyValuePair<ulong, Item>>.CopyTo(KeyValuePair<ulong, Item>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<ulong, Item>>)m_Items).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<ulong, Item>>.Remove(KeyValuePair<ulong, Item> item)
        {
            return ((ICollection<KeyValuePair<ulong, Item>>)m_Items).Remove(item);
        }

        public IEnumerator<KeyValuePair<ulong, Item>> GetEnumerator()
        {
            return m_Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_Items.GetEnumerator();
        }
        #endregion

        #region Get or Create
        /// <summary>
        /// 获取物品信息
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        private ItemInfo GetItemInfo(int itemId)
        {
            ItemInfoConfig config = ItemInfoConfig.Get<ItemInfoConfig>();
            ItemInfo info = config[itemId];

            if (info == null)
            {
                Debug.LogErrorFormat(
                    "ItemModel -> ItemInfo key `{0}` is not found.",
                    itemId.ToString());
                return null;
            }
            return info;
        }

        /// <summary>
        /// 获取物品数据模版，如果不存在则创建后获取
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public ItemData GetOrCreateItemTemplate(int itemId)
        {
            ItemData data;
            if (!m_ItemTemplates.TryGetValue(itemId, out data))
            {
                ItemInfo info = GetItemInfo(itemId);
                if (info == null)
                {
                    return null;
                }

                data = new ItemData()
                {
                    itemId = info.id
                };

                switch (info.itemType)
                {
                    case ItemType.Weapon:
                        WeaponUniqueInfo weapon = info.uniqueInfo as WeaponUniqueInfo;
                        data.durability = weapon.durability;
                        break;
                    case ItemType.Ornament:
                        break;
                    case ItemType.Consumable:
                        ConsumableUniqueInfo consumable = info.uniqueInfo as ConsumableUniqueInfo;
                        data.durability = consumable.stackingNumber == 1 ? consumable.amountUsed : consumable.stackingNumber;
                        break;
                    default:
                        break;
                }

                m_ItemTemplates.Add(itemId, data);
            }
            return data;
        }

        /// <summary>
        /// 创建物品
        /// </summary>
        /// <param name="template"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        private Item CreateItem(ItemInfo info, ItemData data, bool isTemplate)
        {
            Item item;
            switch (info.itemType)
            {
                case ItemType.Weapon:
                    item = new Weapon(info, isTemplate ? nextItemGUID++ : data.guid);
                    break;
                case ItemType.Ornament:
                    item = new Ornament(info, isTemplate ? nextItemGUID++ : data.guid);
                    break;
                case ItemType.Consumable:
                    item = new Consumable(info, isTemplate ? nextItemGUID++ : data.guid);
                    break;
                default:
                    Debug.LogError("ItemModel -> Create item: unknow type.");
                    item = null;
                    break;
            }

            if (item != null)
            {
                item.Load(data);
            }
            
            return item;
        }

        /// <summary>
        /// 创建物品
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="ownerType"></param>
        /// <param name="ownerId"></param>
        /// <returns></returns>
        public Item CreateItem(int itemId)
        {
            ItemInfo info = GetItemInfo(itemId);
            if (info == null)
            {
                return null;
            }

            ItemData template = GetOrCreateItemTemplate(itemId);
            if (template == null)
            {
                return null;
            }

            Item item = CreateItem(info, template, true);
            m_Items.Add(item.guid, item);
            return item;
        }

        /// <summary>
        /// 读取存档用
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public Item CreateItemFromSaver(ItemData data)
        {
            if (data == null)
            {
                return null;
            }

            ItemInfo info = GetItemInfo(data.itemId);
            if (info == null)
            {
                return null;
            }

            if (m_Items.ContainsKey(data.guid))
            {
                Debug.LogErrorFormat(
                    "ItemModel -> Create item from saver ERROR. GUID {0} is exist.",
                    data.guid);
                return null;
            }

            // 判断guid
            if (data.guid >= nextItemGUID)
            {
                nextItemGUID = data.guid + 1UL;
            }

            Item item = CreateItem(info, data, false);
            m_Items.Add(item.guid, item);
            return item;
        }
        #endregion
    }
}