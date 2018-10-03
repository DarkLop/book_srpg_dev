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
using System.Collections.Generic;
using UnityEngine;

namespace DR.Book.SRPG_Dev.Models.Old
{
    public class RoleModel : ModelBase
    {
        #region Field
        private MoveConsume[] m_Consumes;
        private Dictionary<int, Class> m_ClassDict;
        private Dictionary<int, Character> m_CharacterDict;
        #endregion

        #region Get
        public MoveConsume GetMoveConsume(ClassType type)
        {
            if (type < 0 || type >= ClassType.MaxLength)
            {
                throw new IndexOutOfRangeException("Class Type can not be 'MaxLenght'.");
            }

            return m_Consumes[(int)type];
        }

        public Class GetClass(int id)
        {
            Class cls;
            if (!m_ClassDict.TryGetValue(id, out cls))
            {
                ClassInfo info = ClassConfig.GetData(id);
                cls = new Class(info);
                m_ClassDict.Add(id, cls);
            }
            return cls;
        }

        public Character GetCharacter(int id)
        {
            Character character;
            if (!m_CharacterDict.TryGetValue(id, out character))
            {
                CharacterInfo info = CharacterConfig.GetData(id);
                character = new Character(info);
                m_CharacterDict.Add(id, character);
            }
            return character;
        }
        #endregion

        protected override void OnLoad()
        {
            m_Consumes = new MoveConsume[(int)ClassType.MaxLength];
            m_ClassDict = new Dictionary<int, Class>();
            m_CharacterDict = new Dictionary<int, Character>();

            MoveConsumeConfig consumeConfig = MoveConsumeConfig.Get<MoveConsumeConfig>();
            if (consumeConfig != null)
            {
                foreach (var info in consumeConfig)
                {
                    if (info.Key == ClassType.MaxLength)
                    {
                        Debug.LogWarning("Some Class Type is 'MaxLenght'. Ignor!");
                        continue;
                    }

                    if (m_Consumes[(int)info.Key] != null)
                    {
                        Debug.LogWarning("Exist Consume. Type: " + info.Key.ToString());
                        continue;
                    }
                    MoveConsume consume = new MoveConsume(info.Value);
                    m_Consumes[(int)info.Key] = consume;
                }
            }
            MoveConsumeConfig.Release<MoveConsumeConfig>();

            ClassConfig classConfig = ClassConfig.Get<ClassConfig>();
            if (classConfig != null)
            {
                foreach (var info in classConfig)
                {
                    Class cls = new Class(info.Value);
                    m_ClassDict.Add(info.Key, cls);
                }
            }
            ClassConfig.Release<ClassConfig>();

            CharacterConfig characterConfig = CharacterConfig.Get<CharacterConfig>();
            if (characterConfig != null)
            {
                foreach (var info in characterConfig)
                {
                    Character character = new Character(info.Value);
                    m_CharacterDict.Add(info.Key, character);
                }
            }
            CharacterConfig.Release<CharacterConfig>();
        }
    }
}