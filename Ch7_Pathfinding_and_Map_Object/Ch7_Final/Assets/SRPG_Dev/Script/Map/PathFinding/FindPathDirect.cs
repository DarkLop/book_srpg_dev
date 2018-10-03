#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				FindPathDirect.cs
/// Author:					DarkRabbit
/// Create Time:			Thu, 06 Sep 2018 13:28:22 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System.Linq;
using UnityEngine;

namespace DR.Book.SRPG_Dev.Maps.FindPath
{
    [CreateAssetMenu(fileName = "FindPathDirect.asset", menuName = "SRPG/How to find path")]
    public class FindPathDirect : FindMoveRange
    {
        public override bool IsFinishedOnChose(PathFinding search)
        {
            // 如果开放集中已经空了，则说明没有达到目标点
            if (search.currentCell == null)
            {
                // 使用h最小值建立结果
                CellData minHCell = search.explored.First(cell => cell.h == search.explored.Min(c => c.h));
                search.BuildPath(minHCell, true);
                return true;
            }

            // 找到了目标点
            if (search.currentCell == search.endCell)
            {
                return true;
            }

            if (!search.IsCellInExpored(search.currentCell))
            {
                search.explored.Add(search.currentCell);
            }

            return false;
        }

        public override float CalcH(PathFinding search, CellData adjacent)
        {
            Vector2 hVec;
            hVec.x = Mathf.Abs(adjacent.position.x - search.endCell.position.x);
            hVec.y = Mathf.Abs(adjacent.position.y - search.endCell.position.y);
            return hVec.x + hVec.y;
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

            adjacent.g = g;
            adjacent.h = CalcH(search, adjacent);
            adjacent.previous = search.currentCell;
            return true;
        }

        public override void BuildResult(PathFinding search)
        {
            // 当没有达到目标点时，已经建立过结果
            if (search.result.Count > 0)
            {
                return;
            }

            search.BuildPath(search.endCell, true);
        }
    }
}