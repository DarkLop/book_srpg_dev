#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				EnumGUIContents.cs
/// Author:					DarkRabbit
/// Create Time:			Mon, 08 Oct 2018 11:06:43 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEngine;

namespace DR.Book.SRPG_Dev
{
    using DR.Book.SRPG_Dev.Maps;

    public static class EnumGUIContents
    {
        public static readonly GUIContent[] terrainTypeContents = GetEnumContents(typeof(TerrainType));
        public static readonly GUIContent[] classTypeContents = GetEnumContents(typeof(ClassType));

        private static GUIContent[] GetEnumContents(Type enumType)
        {
            return Enum.GetNames(enumType).Select(n => new GUIContent(n)).ToArray();
        }
    }
}
#endif