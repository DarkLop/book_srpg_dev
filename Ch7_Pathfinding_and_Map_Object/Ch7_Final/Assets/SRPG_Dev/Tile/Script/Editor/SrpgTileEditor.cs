#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				SrpgTileEditor.cs
/// Author:					DarkRabbit
/// Create Time:			Wed, 24 Jan 2018 01:59:26 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using UnityEditor;

namespace DR.Book.SRPG_Dev.Maps
{
    [CustomEditor(typeof(SrpgTile))]
    [CanEditMultipleObjects]
    public class SrpgTileEditor : RuleTileEditor
    {
        /// <summary>
        /// 当前选择的 SRPG Tile
        /// </summary>
        public SrpgTile srpgTile
        {
            get { return target as SrpgTile; }
        }

        /// <summary>
        /// 重绘Inspector面板
        /// </summary>
        public override void OnInspectorGUI()
        {
            // 渲染新增的数据
            EditorGUI.BeginChangeCheck();
            srpgTile.terrainType = (TerrainType)EditorGUILayout.EnumPopup("Terrain Type", srpgTile.terrainType);
            srpgTile.avoidRate = EditorGUILayout.IntSlider("Avoid Rate", srpgTile.avoidRate, -100, 100);
            srpgTile.m_CheckAnyTile = EditorGUILayout.Toggle("Check Any Tile", srpgTile.m_CheckAnyTile);
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(srpgTile);
            }

            // 如果地形类型选择的是MaxLength，就提示Error，直接返回，不再渲染RuleTile的内容
            if (srpgTile.terrainType == TerrainType.MaxLength)
            {
                EditorGUILayout.HelpBox("Terrain Type is not supported, please change it.", MessageType.Error);
                return;
            }

            // 渲染RuleTile的内容
            EditorGUILayout.Space();
            base.OnInspectorGUI();
        }
    }
}