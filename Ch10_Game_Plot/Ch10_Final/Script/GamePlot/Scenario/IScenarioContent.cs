#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				Scenario.cs
/// Author:					DarkRabbit
/// Create Time:			Mon, 31 Dec 2018 03:46:39 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

namespace DR.Book.SRPG_Dev.ScriptManagement
{
    public interface IScenarioContent
    {
        /// <summary>
        /// 剧本内容类型
        /// </summary>
        ScenarioContentType type { get; }
        
        /// <summary>
        /// 关键字或剧情标识
        /// </summary>
        string code { get; }

        /// <summary>
        /// 参数数量
        /// </summary>
        int length { get; }

        /// <summary>
        /// 参数索引器
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        string this[int index] { get; }
    }
}