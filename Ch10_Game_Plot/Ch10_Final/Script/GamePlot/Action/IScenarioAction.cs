#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2019 DarkRabbit(ZhangHan)
///
/// File Name:				IScenarioAction.cs
/// Author:					DarkRabbit
/// Create Time:			Sat, 05 Jan 2019 21:55:29 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

namespace DR.Book.SRPG_Dev.ScriptManagement
{
    public interface IScenarioAction : IGameAction
    {
        /// <summary>
        /// 当前剧本
        /// </summary>
        IScenario scenario { get; }

        /// <summary>
        /// 剧本状态
        /// </summary>
        ActionStatus status { get; }

        /// <summary>
        /// 运行到哪一条命令
        /// </summary>
        int token { get; }

        /// <summary>
        /// 读取剧本
        /// </summary>
        /// <param name="scenario"></param>
        /// <returns></returns>
        bool LoadScenario(IScenario scenario);
    }
}