#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				EditorTestMoving.cs
/// Author:					DarkRabbit
/// Create Time:			Wed, 04 Apr 2018 22:35:27 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DR.Book.SRPG_Dev.Maps.Testing
{
    [Obsolete("This class is OBSOLETED! Use `EditorTestPathFinding` instead.", true), RequireComponent(typeof(MapGraph))]
    public class EditorTestMoving : MonoBehaviour
    {
        public MapGraph m_Map;
        public MapClass m_ClsPrefab;
        public Vector3Int m_CreatePosition = new Vector3Int(1, 1, 0);

        private MapClass m_SelectedCls;
        private bool m_IsSelected = false;

#if UNITY_EDITOR
        #region Unity Callback
        private void Awake()
        {
            m_Map = GetComponent<MapGraph>();
            m_Map.InitMap();
        }

        private void Update()
        {
            if (m_SelectedCls == null)
            {
                return;
            }

            // 左键测试移动
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector3Int position = m_Map.grid.WorldToCell(world);

                if (m_IsSelected)
                {
                    if (m_SelectedCls.cellPosition != position)
                    {
                        StartCoroutine(Moving(position));
                    }
                }
                else
                {
                    if (m_SelectedCls.cellPosition == position)
                    {
                        m_IsSelected = true;
                        m_SelectedCls.animatorController.PlayMove();
                    }
                }
            }

            // 右键显示攻击范围
            if (Input.GetMouseButtonDown(1))
            {
                if (!m_IsSelected)
                {
                    return;
                }

                List<CellData> range = m_Map.SearchAttackRange(m_Map.GetCellData(m_SelectedCls.cellPosition), 2, 3);
                if (range != null)
                {
                    m_Map.ShowRangeCursors(range, MapCursor.CursorType.Attack);
                }
            }

            // 中间取消
            if (Input.GetMouseButtonDown(2))
            {
                m_IsSelected = false;
                m_SelectedCls.animatorController.StopMove();
                m_Map.HideRangeCursors();
            }
        }
        #endregion

        public MapClass CreateMapClass()
        {
            if (m_SelectedCls != null)
            {
                return m_SelectedCls;
            }
            MapClass cls = MapClass.Instantiate<MapClass>(m_ClsPrefab, m_Map.mapObjectPool.transform, false);
            cls.map = m_Map;
            cls.Load(0);
            cls.UpdatePosition(m_CreatePosition);
            m_SelectedCls = cls;
            return cls;
        }

        private IEnumerator Moving(Vector3Int position)
        {
            //yield return m_SelectedCls.MovingTo(position);
            m_SelectedCls.animatorController.StopMove();
            m_IsSelected = false;
            yield break;
        }
#endif
    }


#if UNITY_EDITOR
    [Obsolete("This `Editor` class is OBSOLETED!", true), CustomEditor(typeof(EditorTestMoving))]
    public class EditorMapGraphTestEditor : Editor
    {
        public EditorTestMoving map
        {
            get { return target as EditorTestMoving; }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Please start the game.", MessageType.Info);
                return;
            }

            if (GUILayout.Button("Create Class"))
            {
                map.CreateMapClass();
            }
        }

    }
#endif
}
