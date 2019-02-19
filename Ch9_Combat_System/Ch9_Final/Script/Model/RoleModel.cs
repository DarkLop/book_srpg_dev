#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				RoleModel.cs
/// Author:					DarkRabbit
/// Create Time:			Fri, 06 Apr 2018 16:19:16 GMT
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
    public class RoleModel : ModelBase, IDictionary<int, UniqueRole>
    {
        #region Static
        /// <summary>
        /// 生成的下一个角色GUID
        /// </summary>
        public static ulong nextRoleGUID { get; private set; }
        #endregion

        #region Field
        private Dictionary<ClassType, MoveConsumption> m_MoveConsumptions;
        private Dictionary<int, Class> m_Classes;
        private Dictionary<int, Character> m_Characters;

        /// <summary>
        /// 独有角色
        /// </summary>
        private Dictionary<int, UniqueRole> m_UniqueRoles;

        /// <summary>
        /// 部下杂兵模版
        /// </summary>
        private Dictionary<int, RoleData> m_FollowingTemplates;

        /// <summary>
        /// 部下杂兵角色
        /// </summary>
        private Dictionary<ulong, FollowingRole> m_FollowingRoles;
        #endregion

        #region Property
        public int followingRolesCount
        {
            get { return m_FollowingRoles.Count; }
        }

        public Dictionary<ulong, FollowingRole>.KeyCollection followingRolesKeys
        {
            get { return m_FollowingRoles.Keys; }
        }

        public Dictionary<ulong, FollowingRole>.ValueCollection followingRolesValues
        {
            get { return m_FollowingRoles.Values; }
        }
        #endregion

        #region IDictionary<int, UniqueRole> Properties
        public ICollection<int> Keys
        {
            get { return m_UniqueRoles.Keys; }
        }

        public ICollection<UniqueRole> Values
        {
            get { return m_UniqueRoles.Values; }
        }

        public int Count
        {
            get { return m_UniqueRoles.Count; }
        }

        bool ICollection<KeyValuePair<int, UniqueRole>>.IsReadOnly
        {
            get { return ((ICollection<KeyValuePair<int, UniqueRole>>)m_UniqueRoles).IsReadOnly; }
        }

        public UniqueRole this[int characterId]
        {
            get { return GetOrCreateRole(characterId, RoleType.Unique) as UniqueRole; }
            set { throw new NotImplementedException("Readonly."); }
        }
        #endregion

        #region Load
        protected override void OnLoad()
        {
            m_MoveConsumptions = new Dictionary<ClassType, MoveConsumption>();
            m_Classes = new Dictionary<int, Class>();
            m_Characters = new Dictionary<int, Character>();

            nextRoleGUID = 1UL;
            m_UniqueRoles = new Dictionary<int, UniqueRole>();
            m_FollowingTemplates = new Dictionary<int, RoleData>();
            m_FollowingRoles = new Dictionary<ulong, FollowingRole>();
        }

        protected override void OnDispose()
        {
            m_MoveConsumptions = null;
            m_Classes = null;
            m_Characters = null;

            m_UniqueRoles = null;
            m_FollowingTemplates = null;
            m_FollowingRoles = null;
        }
        #endregion

        #region Get or Create Object
        /// <summary>
        /// 获取或创建移动消耗
        /// </summary>
        /// <param name="classType"></param>
        /// <returns></returns>
        public MoveConsumption GetOrCreateMoveConsumption(ClassType classType)
        {
            MoveConsumption consumption;
            if (!m_MoveConsumptions.TryGetValue(classType, out consumption))
            {
                MoveConsumptionInfoConfig config = MoveConsumptionInfoConfig.Get<MoveConsumptionInfoConfig>();
                MoveConsumptionInfo info = config[classType];
                if (info == null)
                {
                    Debug.LogErrorFormat(
                        "RoleModel -> MoveConsumption key `{0}` is not found.",
                        classType.ToString());
                    return null;
                }
                else
                {
                    consumption = new MoveConsumption(info);
                    m_MoveConsumptions.Add(classType, consumption);
                }
            }

            return consumption;
        }

        /// <summary>
        /// 获取或创建职业
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        public Class GetOrCreateClass(int classId)
        {
            Class cls;
            if (!m_Classes.TryGetValue(classId, out cls))
            {
                ClassInfoConfig config = ClassInfoConfig.Get<ClassInfoConfig>();
                ClassInfo info = config[classId];
                if (info == null)
                {
                    Debug.LogErrorFormat(
                        "RoleModel -> Class key `{0}` is not found.",
                        classId.ToString());
                    return null;
                }
                else
                {
                    cls = new Class(info);
                    m_Classes.Add(classId, cls);
                }

            }
            return cls;
        }

        /// <summary>
        /// 获取或创建独有人物
        /// </summary>
        /// <param name="characterId"></param>
        /// <returns></returns>
        public Character GetOrCreateCharacter(int characterId)
        {
            Character character;
            if (!m_Characters.TryGetValue(characterId, out character))
            {
                CharacterInfoConfig config = CharacterInfoConfig.Get<CharacterInfoConfig>();
                CharacterInfo info = config[characterId];
                if (info == null)
                {
                    Debug.LogErrorFormat(
                        "RoleModel -> CharacterInfo key `{0}` is not found.",
                        characterId.ToString());
                    return null;
                }
                else
                {
                    character = new Character(info);
                    m_Characters.Add(characterId, character);
                }
            }
            return character;
        }
        #endregion

        #region Get or Create Following Template
        /// <summary>
        /// 获取或创建杂兵模板
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        public RoleData GetOrCreateFollowingTemplate(int classId)
        {
            Class cls = GetOrCreateClass(classId);

            if (cls == null || cls.info == null)
            {
                return null;
            }

            ClassInfo info = cls.info;

            RoleData data;
            if (!m_FollowingTemplates.TryGetValue(info.id, out data))
            {
                data = new RoleData();
                data.classId = info.id;
                data.level = 1;
                data.exp = 0;
                data.fightProperties = FightProperties.Clamp(
                    cls.info.fightProperties,
                    cls.info.maxFightProperties);
                data.hp = data.fightProperties.hp;
                data.mp = data.fightProperties.mp;
                data.luk = 0;
                //data.money = character.info.money;
                data.movePoint = cls.info.movePoint;
                data.holding = false;

                // TODO 计算公式，计算npc出生数据
                m_FollowingTemplates.Add(data.classId, data);
            }

            return data;
        }
        #endregion

        #region Get or Create Role
        /// <summary>
        /// 设置独有角色
        /// </summary>
        /// <param name="role"></param>
        public void SetUniqueRole(UniqueRole role)
        {
            m_UniqueRoles[role.characterId] = role;
        }

        /// <summary>
        /// 获取或创建角色
        /// 独有角色：characterId
        /// 部下杂兵角色：classId
        /// </summary>
        /// <param name="characterId"></param>
        /// <returns></returns>
        public Role GetOrCreateRole(int id, RoleType roleType)
        {
            if (roleType == RoleType.Unique)
            {
                UniqueRole role;
                if (!m_UniqueRoles.TryGetValue(id, out role))
                {
                    role = CreateUniqueRole(id);
                    if (role != null)
                    {
                        m_UniqueRoles.Add(role.characterId, role);
                    }
                }
                return role;
            }
            else
            {
                FollowingRole role = CreateFollowingRole(id);
                if (role != null)
                {
                    m_FollowingRoles.Add(role.guid, role);
                }
                return role;
            }
        }

        /// <summary>
        /// 创建独有角色
        /// </summary>
        /// <param name="characterId"></param>
        /// <returns></returns>
        private UniqueRole CreateUniqueRole(int characterId)
        {
            UniqueRole role = new UniqueRole();
            RoleData data = CreateUniqueRoleData(characterId);
            if (!role.Load(data))
            {
                return null;
            }

            ItemModel model = ModelManager.models.Get<ItemModel>();
            for (int i = 0; i < role.character.info.items.Length; i++)
            {
                Item item = model.CreateItem(role.character.info.items[i]);
                role.AddItem(item);
            }

            return role;
        }

        /// <summary>
        /// 创建部下角色
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
        private FollowingRole CreateFollowingRole(int classId)
        {
            FollowingRole role = new FollowingRole(nextRoleGUID++);
            RoleData template = GetOrCreateFollowingTemplate(classId);
            if (!role.Load(template))
            {
                return null;
            }
            return role;
        }

        /// <summary>
        /// 创建独有角色数据
        /// </summary>
        /// <param name="characterId"></param>
        /// <returns></returns>
        private RoleData CreateUniqueRoleData(int characterId)
        {
            Character character = GetOrCreateCharacter(characterId);
            if (character == null)
            {
                return null;
            }

            Class cls = GetOrCreateClass(character.info.classId);
            if (cls == null)
            {
                return null;
            }

            RoleData self = new RoleData();
            self.characterId = characterId;
            self.classId = character.info.classId;
            self.level = Mathf.Clamp(character.info.level, 0, SettingVars.maxLevel);
            self.exp = 0;
            self.fightProperties = FightProperties.Clamp(
                character.info.fightProperties + cls.info.fightProperties,
                cls.info.maxFightProperties);
            self.hp = self.fightProperties.hp;
            self.mp = self.fightProperties.mp;
            self.luk = Mathf.Clamp(character.info.luk, 0, SettingVars.maxLuk);
            //self.money = character.info.money;
            self.movePoint = cls.info.movePoint;
            self.holding = false;

            return self;
        }

        /// <summary>
        /// 读取存档用
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public FollowingRole CreateFollowingRoleFromSaver(RoleData data)
        {
            if (data == null)
            {
                return null;
            }

            if (m_FollowingRoles.ContainsKey(data.guid))
            {
                Debug.LogErrorFormat(
                    "RoleModel -> Create FollowingRole from saver ERROR. GUID {0} is exist.",
                    data.guid);
                return null;
            }

            FollowingRole role = new FollowingRole(data.guid);
            if (!role.Load(data))
            {
                return null;
            }

            if (data.guid >= nextRoleGUID)
            {
                nextRoleGUID = data.guid + 1UL;
            }

            m_FollowingRoles.Add(role.guid, role);
            return role;
        }
        #endregion

        public bool TryGetFollowingRole(ulong guid, out FollowingRole role)
        {
            return m_FollowingRoles.TryGetValue(guid, out role);
        }

        #region IDictionary<int, UniqueRole> Methods
        void IDictionary<int, UniqueRole>.Add(int key, UniqueRole value)
        {
            throw new NotImplementedException("Not supported.");
        }

        public bool ContainsKey(int characterId)
        {
            return m_UniqueRoles.ContainsKey(characterId);
        }

        bool IDictionary<int, UniqueRole>.Remove(int characterId)
        {
            throw new NotImplementedException("Not supported.");
        }

        public bool TryGetValue(int characterId, out UniqueRole value)
        {
            return m_UniqueRoles.TryGetValue(characterId, out value);
        }

        void ICollection<KeyValuePair<int, UniqueRole>>.Add(KeyValuePair<int, UniqueRole> item)
        {
            throw new NotImplementedException("Not supported.");
        }

        void ICollection<KeyValuePair<int, UniqueRole>>.Clear()
        {
            throw new NotImplementedException("Not supported.");
        }

        bool ICollection<KeyValuePair<int, UniqueRole>>.Contains(KeyValuePair<int, UniqueRole> item)
        {
            return ((ICollection<KeyValuePair<int, UniqueRole>>)m_UniqueRoles).Contains(item);
        }

        void ICollection<KeyValuePair<int, UniqueRole>>.CopyTo(KeyValuePair<int, UniqueRole>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<int, UniqueRole>>)m_UniqueRoles).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<int, UniqueRole>>.Remove(KeyValuePair<int, UniqueRole> item)
        {
            throw new NotImplementedException("Not supported.");
        }

        public IEnumerator<KeyValuePair<int, UniqueRole>> GetEnumerator()
        {
            return m_UniqueRoles.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_UniqueRoles.GetEnumerator();
        }
        #endregion
    }
}