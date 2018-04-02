#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				MapGraph.cs
/// Author:					DarkRabbit
/// Create Time:			Wed, 24 Jan 2018 15:53:18 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using Handles = UnityEditor.Handles;
using SceneView = UnityEditor.SceneView;
#endif

namespace DR.Book.SRPG_Dev.Maps
{
    [RequireComponent(typeof(Grid))]
    public class MapGraph : MonoBehaviour
    {

#if UNITY_EDITOR
        #region Gizmos
        [Header("Editor Gizmos")]
        public bool m_EditorDrawGizmos = true;
        public Color m_EditorBorderColor = Color.white;
        public Color m_EditorCellColor = Color.green;
        public Color m_EditorErrorColor = Color.red;

        private void OnDrawGizmos()
        {
            if (m_EditorDrawGizmos)
            {
                EditorDrawBorderGizmos();
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (m_EditorDrawGizmos)
            {
                EditorDrawCellGizmos();
            }
        }

        /// <summary>
        /// 绘制Border的Gizmos
        /// </summary>
        protected void EditorDrawBorderGizmos()
        {
            Color old = Gizmos.color;

            GUIStyle textStyle = new GUIStyle();
            textStyle.normal.textColor = m_EditorBorderColor;

            // 获取边框左下角与右上角的世界坐标
            Vector3 leftDown = grid.GetCellCenterWorld(leftDownPosition) - halfCellSize;
            Vector3 rightUp = grid.GetCellCenterWorld(rightUpPosition) + halfCellSize;

            // 绘制左下角Cell与右上角Cell的Position
            Handles.Label(leftDown, (new Vector2Int(leftDownPosition.x, leftDownPosition.y)).ToString(), textStyle);
            Handles.Label(rightUp, (new Vector2Int(rightUpPosition.x, rightUpPosition.y)).ToString(), textStyle);

            if (mapRect.width > 0 && mapRect.height > 0)
            {
                Gizmos.color = m_EditorBorderColor;

                // 边框的长与宽
                Vector3 size = Vector3.Scale(new Vector3(width, height), grid.cellSize);

                // 边框的中心坐标
                Vector3 center = leftDown + size / 2f;

                // 绘制边框
                Gizmos.DrawWireCube(center, size);
            }

            Gizmos.color = old;
        }

        /// <summary>
        /// 绘制Cell的Gizmos
        /// </summary>
        protected void EditorDrawCellGizmos()
        {
            // 用于获取鼠标位置
            Event e = Event.current;
            if (e.type != EventType.Repaint)
            {
                return;
            }

            // 获取当前操作Scene面板
            SceneView sceneView = SceneView.currentDrawingSceneView;
            if (sceneView == null)
            {
                return;
            }

            Color old = Gizmos.color;

            /// 获取鼠标世界坐标：
            /// Event是从左上角(Left Up)开始，
            /// 而Camera是从左下角(Left Down)，
            /// 需要转换才能使用Camera的ScreenToWorldPoint方法。
            Vector2 screenPosition = new Vector2(e.mousePosition.x, sceneView.camera.pixelHeight - e.mousePosition.y);
            Vector2 worldPosition = sceneView.camera.ScreenToWorldPoint(screenPosition);

            // 当前鼠标所在Cell的Position
            Vector3Int cellPostion = grid.WorldToCell(worldPosition);
            // 当前鼠标所在Cell的Center坐标
            Vector3 cellCenter = grid.GetCellCenterWorld(cellPostion);

            /// 绘制当前鼠标下的Cell边框与Position
            /// 如果包含Cell，正常绘制
            /// 如果不包含Cell，改变颜色，并多绘制一个叉
            if (Contains(cellPostion))
            {
                GUIStyle textStyle = new GUIStyle();
                textStyle.normal.textColor = m_EditorCellColor;
                Gizmos.color = m_EditorCellColor;

                Handles.Label(cellCenter - halfCellSize, (new Vector2Int(cellPostion.x, cellPostion.y)).ToString(), textStyle);
                Gizmos.DrawWireCube(cellCenter, grid.cellSize);
            }
            else
            {
                GUIStyle textStyle = new GUIStyle();
                textStyle.normal.textColor = m_EditorErrorColor;
                Gizmos.color = m_EditorErrorColor;

                Handles.Label(cellCenter - halfCellSize, (new Vector2Int(cellPostion.x, cellPostion.y)).ToString(), textStyle);
                Gizmos.DrawWireCube(cellCenter, grid.cellSize);

                // 绘制Cell对角线
                Vector3 from = cellCenter - halfCellSize;
                Vector3 to = cellCenter + halfCellSize;
                Gizmos.DrawLine(from, to);
                float tmpX = from.x;
                from.x = to.x;
                to.x = tmpX;
                Gizmos.DrawLine(from, to);
            }

            Gizmos.color = old;
        }
        #endregion
#endif

        #region Field
        [Header("Map Setting")]
        [SerializeField]
        private string m_MapName;
        [SerializeField]
        private RectInt m_MapRect = new RectInt(0, 0, 10, 10);

        private Grid m_Grid;
        #endregion

        #region Property
        /// <summary>
        /// 地图的名称
        /// </summary>
        public string mapName
        {
            get { return m_MapName; }
            set { m_MapName = value; }
        }

        /// <summary>
        /// 地图的矩形框
        /// </summary>
        public RectInt mapRect
        {
            get { return m_MapRect; }
            set { m_MapRect = value; }
        }

        /// <summary>
        /// 地图左下角Position
        /// </summary>
        public Vector3Int leftDownPosition
        {
            get { return new Vector3Int(m_MapRect.xMin, m_MapRect.yMin, 0); }
        }

        /// <summary>
        /// 地图右上角Position
        /// </summary>
        public Vector3Int rightUpPosition
        {
            get { return new Vector3Int(m_MapRect.xMax - 1, m_MapRect.yMax - 1, 0); }
        }

        /// <summary>
        /// 地图宽
        /// </summary>
        public int width
        {
            get { return m_MapRect.width; }
        }

        /// <summary>
        /// 地图高
        /// </summary>
        public int height
        {
            get { return m_MapRect.height; }
        }

        /// <summary>
        /// Grid组件
        /// </summary>
        public Grid grid
        {
            get
            {
                if (m_Grid == null)
                {
                    m_Grid = GetComponent<Grid>();
                }
                return m_Grid;
            }
        }

        /// <summary>
        /// 地图每个cell尺寸的一半
        /// </summary>
        public Vector3 halfCellSize
        {
            get { return grid.cellSize / 2f; }
        }
        #endregion

        #region Helper Method
        /// <summary>
        /// 地图是否包含Position
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool Contains(Vector3Int position)
        {
            return mapRect.Contains(new Vector2Int(position.x, position.y));
        }
        #endregion
    }
}