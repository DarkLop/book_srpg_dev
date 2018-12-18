#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				MoveConsumptionInfoConfigPropertyDrawer.cs
/// Author:					DarkRabbit
/// Create Time:			Sun, 07 Oct 2018 13:31:46 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using UnityEditor;
using UnityEngine;

namespace DR.Book.SRPG_Dev.Models
{
    [CustomPropertyDrawer(typeof(MoveConsumptionInfoConfig))]
    public class MoveConsumptionInfoConfigPropertyDrawer : PropertyDrawer
    {
        private const float k_Padding = 2f;
        private const float k_TabWidth = 16f;
        private const int k_ArraySize = (int)ClassType.MaxLength;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // 标题高度
            float height = EditorGUIUtility.singleLineHeight + k_Padding;

            if (property.isExpanded)
            {
                SerializedProperty datas = property.FindPropertyRelative("datas");

                // datas每一个属性高度
                for (int i = 0; i < datas.arraySize; i++)
                {
                    SerializedProperty data = datas.GetArrayElementAtIndex(i);
                    height += EditorGUI.GetPropertyHeight(data, true) + k_Padding;
                }
            }
            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // 设置初始位置与高度
            Rect rect = position;
            rect.height = EditorGUIUtility.singleLineHeight;

            // 渲染标题Foldout
            bool expanded = EditorGUI.PropertyField(rect, property, label);
            rect.y += EditorGUIUtility.singleLineHeight + k_Padding;

            // 设置长度与ClassType一样
            SerializedProperty datas = property.FindPropertyRelative("datas");
            if (datas.arraySize != k_ArraySize)
            {
                datas.arraySize = k_ArraySize;
            }

            if (expanded)
            {
                // 设置偏移
                rect.x += k_TabWidth;
                rect.width -= k_TabWidth;

                for (int i = 0; i < datas.arraySize; i++)
                {
                    SerializedProperty data = datas.GetArrayElementAtIndex(i);

                    // 保持数组顺序为ClassType的Enum顺序
                    SerializedProperty classType = data.FindPropertyRelative("classType");
                    classType.enumValueIndex = i;

                    // 渲染每一个MoveConsumpotionInfo
                    rect.height = EditorGUI.GetPropertyHeight(data, true);
                    EditorGUI.PropertyField(rect, data, true);
                    rect.y += rect.height + k_Padding;
                }
            }
        }
    }
}