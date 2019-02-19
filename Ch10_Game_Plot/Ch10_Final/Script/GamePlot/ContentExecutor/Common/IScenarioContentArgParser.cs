#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2019 DarkRabbit(ZhangHan)
///
/// File Name:				IScenarioContentArgParser.cs
/// Author:					DarkRabbit
/// Create Time:			Thu, 03 Jan 2019 23:49:27 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

namespace DR.Book.SRPG_Dev.ScriptManagement
{
    public interface IScenarioContentArgParser<T>
    {
        /// <summary>
        /// 转换参数
        /// </summary>
        /// <param name="content"></param>
        /// <param name="args"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        bool ParseArgs(IScenarioContent content, ref T args, out string error);
    }

}