#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				IScenario.cs
/// Author:					DarkRabbit
/// Create Time:			Mon, 31 Dec 2018 03:58:38 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

namespace DR.Book.SRPG_Dev.ScriptManagement
{
    /// <summary>
    /// 剧本接口
    /// </summary>
    public interface IScenario
    {
        /// <summary>
        /// 剧本名称
        /// </summary>
        string name { get; }

        /// <summary>
        /// 错误信息
        /// </summary>
        string formatError { get; }

        /// <summary>
        /// 是否读取过剧本文本
        /// </summary>
        bool isLoaded { get; }

        /// <summary>
        /// 内容数量
        /// </summary>
        int contentCount { get; }

        /// <summary>
        /// 获取内容
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IScenarioContent GetContent(int index);

        /// <summary>
        /// 格式化剧本
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="scriptText"></param>
        /// <returns></returns>
        bool Load(string fileName, string scriptText);

        void Reset();
    }
}