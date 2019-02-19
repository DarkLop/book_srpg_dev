#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2019 DarkRabbit(ZhangHan)
///
/// File Name:				IScenarioContentExecutor.cs
/// Author:					DarkRabbit
/// Create Time:			Thu, 03 Jan 2019 23:49:18 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

namespace DR.Book.SRPG_Dev.ScriptManagement
{
    public interface IScenarioContentExecutor
    {
        /// <summary>
        /// 命令keycode
        /// </summary>
        string code { get; }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="action"></param>
        /// <param name="content"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        ActionStatus Execute(IGameAction gameAction, IScenarioContent content, out string error);
    }
}