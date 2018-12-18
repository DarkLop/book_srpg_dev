#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				EditorSrpgDataEditor.cs
/// Author:					DarkRabbit
/// Create Time:			Fri, 05 Oct 2018 13:46:38 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

#if UNITY_EDITOR
using UnityEngine;

namespace DR.Book.SRPG_Dev.Models
{
    [CreateAssetMenu(fileName = "EditorSrpgData.asset", menuName = "SRPG/Editor SRPG Data")]
    public class EditorSrpgData : ScriptableObject
    {
        public enum ConfigType
        {
            MoveConsumption,
            Class,
            Character,
            Item,
            Text
        }

        [SerializeField]
        public ConfigType currentConfig = ConfigType.MoveConsumption;

        [SerializeField]
        public MoveConsumptionInfoConfig moveConsumptionConfig;

        [SerializeField]
        public ClassInfoConfig classConfig;

        [SerializeField]
        public CharacterInfoConfig characterInfoConfig;

        [SerializeField]
        public ItemInfoConfig itemInfoConfig;

        [SerializeField]
        public TextInfoConfig textInfoConfig;

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEditorConfigSerializer GetCurConfig()
        {
            switch (currentConfig)
            {
                case ConfigType.MoveConsumption:
                    return moveConsumptionConfig;
                case ConfigType.Class:
                    return classConfig;
                case ConfigType.Character:
                    return characterInfoConfig;
                case ConfigType.Item:
                    return itemInfoConfig;
                case ConfigType.Text:
                    return textInfoConfig;
                default:
                    return null;
            }
        }
    }
}
#endif