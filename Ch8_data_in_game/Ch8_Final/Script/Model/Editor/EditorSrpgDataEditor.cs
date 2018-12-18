#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				EditorSrpgDataEditor.cs
/// Author:					DarkRabbit
/// Create Time:			Sat, 06 Oct 2018 10:37:55 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using UnityEngine;
using UnityEditor;

namespace DR.Book.SRPG_Dev.Models
{
    [CustomEditor(typeof(EditorSrpgData))]
    public class EditorSrpgDataEditor : Editor
    {
        #region Property
        public EditorSrpgData srpgData
        {
            get { return target as EditorSrpgData; }
        }
        #endregion

        #region Unity Callback
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginDisabledGroup(true);
            base.OnInspectorGUI();
            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("Edit Datas"))
            {
                EditorSrpgDataEditorWindow window = EditorSrpgDataEditorWindow.OpenEditorSrpgDataEditorWindow();
                window.srpgData = srpgData;
            }
        }
        #endregion
    }
}