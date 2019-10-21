#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				FindRange.cs
/// Author:					DarkRabbit
/// Create Time:			Thu, 05 Apr 2018 22:37:07 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using UnityEngine;

namespace DR.Book.SRPG_Dev.Maps.FindPath
{
    [CreateAssetMenu(fileName = "FindRange.asset", menuName = "SRPG/How to find range")]
    public class FindRange : ScriptableObject, IHowToFind
    {
        public virtual CellData ChoseCell(PathFinding search)
        {
            if (search.reachable.Count == 0)
            {
                return null;
            }

            int index = search.reachable.Count - 1;
            CellData chose = search.reachable[index];
            search.reachable.RemoveAt(index);
            return chose;
        }

        public virtual bool IsFinishedOnChose(PathFinding search)
        {
            if (search.currentCell == null)
            {
                return true;
            }

            if (!search.IsCellInExpored(search.currentCell))
            {
                search.explored.Add(search.currentCell);
            }
            return false;
        }

        public virtual float CalcGPerCell(PathFinding search, CellData adjacent)
        {
            return 1f;
        }

        /// <summary>
        /// 用H计算攻击距离
        /// </summary>
        /// <param name="search"></param>
        /// <param name="adjacent"></param>
        /// <returns></returns>
        public virtual float CalcH(PathFinding search, CellData adjacent)
        {
            CellData startCell = search.startCell ?? search.endCell;
            Vector2 hVec;
            hVec.x = Mathf.Abs(adjacent.position.x - startCell.position.x);
            hVec.y = Mathf.Abs(adjacent.position.y - startCell.position.y);
            return hVec.x + hVec.y;
        }

        public virtual bool CanAddAdjacentToReachable(PathFinding search, CellData adjacent)
        {
            // 如果已经在关闭集
            if (search.IsCellInExpored(adjacent))
            {
                return false;
            }

            // 已经加入过开放集
            if (search.IsCellInReachable(adjacent))
            {
                return false;
            }

            //// 计算消耗 = 当前cell的消耗 + 邻居cell的消耗
            //float g = search.currentCell.g + CalcGPerCell(search, adjacent);

            // 计算攻击距离
            float h = CalcH(search, adjacent);

            //// 不在范围内
            //if (g < 0f || g > search.range.y)
            //{
            //    return false;
            //}

            // 攻击距离不在范围内
            if (h < 0f || h > search.range.y)
            {
                return false;
            }

            //adjacent.g = g;
            adjacent.h = h;

            return true;
        }

        public virtual void BuildResult(PathFinding search)
        {
            for (int i = 0; i < search.explored.Count; i++)
            {
                CellData cell = search.explored[i];
                //if (cell.g >= search.range.x && cell.g <= search.range.y)
                if (cell.h >= search.range.x && cell.h <= search.range.y)
                {
                    search.result.Add(cell);
                }
            }
        }
    }

}