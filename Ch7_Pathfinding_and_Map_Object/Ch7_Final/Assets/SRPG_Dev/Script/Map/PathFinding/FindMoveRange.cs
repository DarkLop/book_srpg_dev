#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				FindMoveRange.cs
/// Author:					DarkRabbit
/// Create Time:			Sun, 08 Apr 2018 01:35:04 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using UnityEngine;

namespace DR.Book.SRPG_Dev.Maps.FindPath
{
    [CreateAssetMenu(fileName = "FindMoveRange.asset", menuName = "SRPG/How to find move range")]
    public class FindMoveRange : FindRange
    {
        public override CellData ChoseCell(PathFinding search)
        {
            if (search.reachable.Count == 0)
            {
                return null;
            }

            /// 取得f最小的节点（因为我们没有计算h，这里就是g）
            /// 当你在寻找路径有卡顿时，请一定使用更好的查找方式，
            /// 例如可以改用二叉树的方式，
            /// 也可将PathFinding里面reachable.Add(adjacent)的方法改成边排序边加入的方法
            search.reachable.Sort((cell1, cell2) => -cell1.f.CompareTo(cell2.f));
            int index = search.reachable.Count - 1;
            CellData chose = search.reachable[index];
            search.reachable.RemoveAt(index);
            return chose;
        }

        public override float CalcGPerCell(PathFinding search, CellData adjacent)
        {
            // 获取邻居的Tile
            SrpgTile tile = search.map.GetTile(adjacent.position);

            // 返回本格子的消耗
            return search.GetMoveConsumption(tile.terrainType);
        }

        public override bool CanAddAdjacentToReachable(PathFinding search, CellData adjacent)
        {
            //// 没有Tile
            //if (!adjacent.hasTile)
            //{
            //    return false;
            //}

            //// 已经有对象了
            //if (adjacent.hasMapObject)
            //{
            //    return false;
            //}

            // 是否可移动
            if (!adjacent.canMove)
            {
                return false;
            }

            // 如果已经在关闭集
            if (search.IsCellInExpored(adjacent))
            {
                return false;
            }

            // 计算消耗 = 当前cell的消耗 + 邻居cell的消耗
            float g = search.currentCell.g + CalcGPerCell(search, adjacent);

            // 已经加入过开放集
            if (search.IsCellInReachable(adjacent))
            {
                // 如果新消耗更低
                if (g < adjacent.g)
                {
                    adjacent.g = g;
                    adjacent.previous = search.currentCell;
                }
                return false;
            }

            // 不在范围内
            if (g < 0f || g > search.range.y)
            {
                return false;
            }

            adjacent.g = g;
            adjacent.previous = search.currentCell;

            return true;
        }

        public override void BuildResult(PathFinding search)
        {
            for (int i = 0; i < search.explored.Count; i++)
            {
                CellData cell = search.explored[i];
                if (cell.g >= search.range.x && cell.g <= search.range.y)
                {
                    search.result.Add(cell);
                }
            }
        }
    }
}