#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				RuntimePoolObjectEditor.cs
/// Author:					DarkRabbit
/// Create Time:			Tue, 06 Mar 2018 01:07:32 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------


using UnityEditor;

namespace DR.Book.SRPG_Dev.Framework
{
    [CustomEditor(typeof(RuntimePrePoolObject)), CanEditMultipleObjects]
    public class RuntimePrePoolObjectEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // 判断Instance是不是来自Prefab
            PrefabType type = PrefabUtility.GetPrefabType(target);
            switch (type)
            {
                case PrefabType.None:
                    EditorGUILayout.HelpBox("This component can only add to the prefab or prefab instance.", MessageType.Error);
                    break;
                case PrefabType.MissingPrefabInstance:
                    EditorGUILayout.HelpBox("The prefab of this instance is missing.", MessageType.Error);
                    break;
                case PrefabType.DisconnectedPrefabInstance:
                    EditorGUILayout.HelpBox("The prefab of this instance is disconnected.", MessageType.Error);
                    break;
                case PrefabType.DisconnectedModelPrefabInstance:
                    EditorGUILayout.HelpBox("The prefab of this instance is disconnected.", MessageType.Error);
                    break;
                default:
                    break;
            }

            base.OnInspectorGUI();
        }
    }
}
