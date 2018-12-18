#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				ItemInfoPropertyDrawer.cs
/// Author:					DarkRabbit
/// Create Time:			Sat, 06 Oct 2018 08:45:57 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using UnityEditor;
using UnityEngine;

namespace DR.Book.SRPG_Dev.Models
{
    using DR.Book.SRPG_Dev.Maps;

    [CustomPropertyDrawer(typeof(MoveConsumptionInfo))]
    public class MoveConsumptionInfoPropertyDrawer : PropertyDrawer
    {
        private const float k_Padding = 2f;
        private const float k_TabWidth = 16f;
        private const int k_ArraySize = (int)TerrainType.MaxLength;
        private static readonly GUIContent s_ClassTypeContent = new GUIContent("Class Type");

        /// <summary>
        /// 每一个ClassType的Foulout
        /// </summary>
        public static readonly bool[] s_ClassTypeFoldout = new bool[(int)ClassType.MaxLength];

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // 标题高度
            float height = EditorGUIUtility.singleLineHeight + k_Padding;

            // 如果有展开，加上 ClassType高度 + 单个消耗高度 * 消耗长度(k_ArraySize)
            if (property.isExpanded)
            {
                height += (k_ArraySize + 1) * (EditorGUIUtility.singleLineHeight + k_Padding);
            }
            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // 设置初始位置与高度
            Rect rect = position;
            rect.height = EditorGUIUtility.singleLineHeight;

            SerializedProperty classType = property.FindPropertyRelative("classType");

            // 渲染标题Foldout
            // EditorGUI.LabelField(rect, label);
            string tmpTitle = label.text;
            label.text = string.Format("{0} {1}", tmpTitle, EnumGUIContents.classTypeContents[classType.enumValueIndex].text);
            bool expanded = EditorGUI.PropertyField(rect, property, label);
            label.text = tmpTitle;
            rect.y += EditorGUIUtility.singleLineHeight + k_Padding;

            if (expanded)
            {
                // 设置偏移
                rect.x += k_TabWidth;
                rect.width -= k_TabWidth;
                
                // 渲染ClassType
                EditorGUI.PropertyField(rect, classType, s_ClassTypeContent);
                rect.y += EditorGUIUtility.singleLineHeight + k_Padding;

                // 强制保持长度与TerrainType一样
                SerializedProperty consumptions = property.FindPropertyRelative("consumptions");
                if (consumptions.arraySize != k_ArraySize)
                {
                    consumptions.arraySize = k_ArraySize;
                }

                // 渲染每一个消耗
                for (int i = 0; i < consumptions.arraySize; i++)
                {
                    SerializedProperty consumption = consumptions.GetArrayElementAtIndex(i);
                    EditorGUI.PropertyField(rect, consumption, EnumGUIContents.terrainTypeContents[i], true);
                    rect.y += EditorGUIUtility.singleLineHeight + k_Padding;
                }
            }
        }
    }
}