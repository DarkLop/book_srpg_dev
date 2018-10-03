#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				MapObject.cs
/// Author:					DarkRabbit
/// Create Time:			Wed, 04 Apr 2018 21:58:04 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using UnityEngine;

namespace DR.Book.SRPG_Dev.Maps
{
    using DR.Book.SRPG_Dev.Framework;

    [DisallowMultipleComponent]
    public abstract class MapObject : MonoBehaviour, IReusableComponent
    {
        #region Field
        [SerializeField]
        private SpriteRenderer m_Renderer;

        private Vector3Int m_CellPosition;

        private MapGraph m_Map;
        #endregion

        #region Property
        public new SpriteRenderer renderer
        {
            get { return m_Renderer; }
            set { m_Renderer = value; }
        }

        /// <summary>
        /// 地图网格中的位置
        /// </summary>
        public Vector3Int cellPosition
        {
            get { return m_CellPosition; }
            set
            {
                m_CellPosition = value;
                if (renderer != null)
                {
                    renderer.sortingOrder = MapObject.CalcSortingOrder(map, value);
                }
            }
        }

        /// <summary>
        /// 所属地图
        /// </summary>
        public MapGraph map
        {
            get { return m_Map; }
            internal set { m_Map = value; }
        }

        /// <summary>
        /// 地图对象类型
        /// </summary>
        public abstract MapObjectType mapObjectType { get; }
        #endregion

        #region Method
        public void InitMapObject(MapGraph map)
        {
            m_Map = map;
        }

        /// <summary>
        /// 更新位置
        /// </summary>
        /// <param name="world"></param>
        /// <param name="center"></param>
        public void UpdatePosition(bool world = true, bool center = false)
        {
            if (map == null)
            {
                Debug.LogError(name + " Map is null.");
                return;
            }

            Vector3 pos = map.GetCellPosition(cellPosition, world, center);
            if (world)
            {
                transform.position = pos;
            }
            else
            {
                transform.localPosition = pos;
            }
        }

        /// <summary>
        /// 更新位置
        /// </summary>
        /// <param name="cellPosition"></param>
        /// <param name="center"></param>
        /// <param name="world"></param>
        public void UpdatePosition(Vector3Int cellPosition, bool world = true, bool center = false)
        {
            this.cellPosition = cellPosition;
            UpdatePosition(world, center);
        }

        /// <summary>
        /// 计算sortingOrder
        /// </summary>
        /// <returns></returns>
        public static int CalcSortingOrder(MapGraph map, Vector3Int cellPosition)
        {
            if (map == null)
            {
                return 0;
            }

            // 零点偏移
            Vector3Int offset = Vector3Int.zero - map.leftDownPosition;

            // 相对零点坐标
            Vector3Int relative = cellPosition + offset;

            // 计算从右到左，从下到上渲染顺序（sortingOrder越大越后渲染）
            // 前y行的格子总数 = map.width * relative.y
            // 当前行（第y+1行）从右向左的格子数 = map.width - relative.x
            // 这样计算后是递增，范围 [1, map.width * map.height]
            // 加上负号后是递减，范围 [-(map.width * map.height), -1]
            // 举例：地图尺寸20x10
            //  右下角相对坐标，第20列第1行：
            //      cellPosition = (19, 0, 0)，sortingOrder = -(20 * 0 + (20 - 19)) = -1
            //  左上角相对坐标，第1列第10行：
            //      cellPosition = (0, 9, 0)，sortingOrder = -(20 * 9 + (20 - 0)) = -200
            //  测试相对坐标，第12列第6行：
            //      cellPosition = (11, 5, 0)，sortingOrder = -(20 * 5 + (20 - 11)) = -109
            return -(map.width * relative.y + (map.width - relative.x));
        }

        #endregion

        #region IReusableComponent 对象池组件
        public virtual void OnSpawn()
        {
        }

        public virtual void OnDespawn()
        {
            m_Map = null;
        }
        #endregion
    }
}