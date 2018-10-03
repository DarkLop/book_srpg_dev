#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				IHowToFind.cs
/// Author:					DarkRabbit
/// Create Time:			Fri, 06 Apr 2018 15:07:44 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

namespace DR.Book.SRPG_Dev.Maps.FindPath
{
    public interface IHowToFind
    {
        /// <summary>
        /// 获取检测的Cell
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        CellData ChoseCell(PathFinding search);

        /// <summary>
        /// 选择Cell后，是否结束
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        bool IsFinishedOnChose(PathFinding search);

        /// <summary>
        /// 计算移动到下一格的消耗
        /// </summary>
        /// <param name="search"></param>
        /// <param name="adjacent"></param>
        /// <returns></returns>
        float CalcGPerCell(PathFinding search, CellData adjacent);

        /// <summary>
        /// 无视范围，直接寻路用，计算预计消耗值，（这里用距离）
        /// </summary>
        /// <param name="search"></param>
        /// <param name="adjacent"></param>
        /// <returns></returns>
        float CalcH(PathFinding search, CellData adjacent);

        /// <summary>
        /// 是否能把邻居加入到检测列表中
        /// </summary>
        /// <param name="search"></param>
        /// <param name="adjacent"></param>
        /// <returns></returns>
        bool CanAddAdjacentToReachable(PathFinding search, CellData adjacent);

        /// <summary>
        /// 生成最终显示的范围
        /// </summary>
        /// <param name="search"></param>
        void BuildResult(PathFinding search);
    }
}