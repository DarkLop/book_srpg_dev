#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				Item.cs
/// Author:					DarkRabbit
/// Create Time:			Sun, 14 Oct 2018 14:23:14 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;

namespace DR.Book.SRPG_Dev.Models
{
    public abstract class Item : IDisposable
    {
        #region enum OwnerType
        [Serializable]
        public enum OwnerType
        {
            /// <summary>
            /// 未知
            /// </summary>
            Unknow = 0,

            /// <summary>
            /// 角色
            /// </summary>
            Role,

            /// <summary>
            /// 商店
            /// </summary>
            Shop,

            /// <summary>
            /// 地图
            /// </summary>
            Map
        }

        #endregion

        #region Property
        protected ItemData self { get; set; }

        public ItemInfo info { get; private set; }

        public UniqueInfo uniqueInfo
        {
            get { return info.uniqueInfo; }
        }

        public abstract ItemType itemType { get; }

        public ulong guid
        {
            get { return self.guid; }
        }

        public int itemId
        {
            get { return self.itemId; }
        }

        public int durability
        {
            get { return self.durability; }
        }

        public bool isBroken
        {
            get { return self.durability <= 0; }
        }

        public OwnerType ownerType
        {
            get { return self.ownerType; }
            set { self.ownerType = value; }
        }

        public int ownerId
        {
            get { return self.ownerId; }
            set { self.ownerId = value; }
        }
        #endregion

        #region Constructor
        protected Item(ItemInfo info, ulong guid)
        {
            this.info = info;
            this.self = new ItemData
            {
                guid = guid
            };
        }
        #endregion

        #region Load
        public virtual void Load(ItemData data)
        {
            data.CopyTo(this.self);
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            if (ModelManager.models.ContainsKey(typeof(ItemModel)))
            {
                ItemModel model = ModelManager.models.Get<ItemModel>();
                if (model.ContainsKey(guid))
                {
                    model.Remove(guid);
                }
            }

            self = null;
        }
        #endregion
    }

    public class Weapon : Item
    {
        public sealed override ItemType itemType
        {
            get { return ItemType.Weapon; }
        }

        public new WeaponUniqueInfo uniqueInfo
        {
            get { return info.uniqueInfo as WeaponUniqueInfo; }
        }

        #region Constructor
        public Weapon(ItemInfo info, ulong guid) : base(info, guid)
        {

        }
        #endregion
    }

    public class Consumable : Item
    {
        public sealed override ItemType itemType
        {
            get { return ItemType.Consumable; }
        }

        public new ConsumableUniqueInfo uniqueInfo
        {
            get { return info.uniqueInfo as ConsumableUniqueInfo; }
        }

        #region Constructor
        public Consumable(ItemInfo info, ulong guid) : base(info, guid)
        {

        }
        #endregion
    }

    public class Ornament : Item
    {
        public sealed override ItemType itemType
        {
            get { return ItemType.Ornament; }
        }

        public new OrnamentUniqueInfo uniqueInfo
        {
            get { return info.uniqueInfo as OrnamentUniqueInfo; }
        }

        #region Constructor
        public Ornament(ItemInfo info, ulong guid) : base(info, guid)
        {

        }
        #endregion
    }
}