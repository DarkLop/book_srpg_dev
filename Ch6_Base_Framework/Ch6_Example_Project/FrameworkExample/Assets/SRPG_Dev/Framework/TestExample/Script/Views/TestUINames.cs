#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				TestUINames.cs
/// Author:					DarkRabbit
/// Create Time:			Fri, 09 Mar 2018 22:40:29 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------


namespace DR.Book.SRPG_Dev.Framework.Test
{
    public static class TestUINames
    {
        #region Canvas
        /// <summary>
        /// 内部画布
        /// </summary>
        public const string Canvas_TestUICanvas = "TestUICanvas";
        #endregion

        #region Panel
        /// <summary>
        /// 测试用全局主面板
        /// </summary>
        public const string Panel_TestUIDefaultPanel = "TestUIDefaultPanel";

        /// <summary>
        /// Loading面板
        /// </summary>
        public const string Panel_TestUILoadingPanel = "TestUILoadingPanel";

        /// <summary>
        /// Pool面板，在AddScene中加载
        /// </summary>
        public const string Panel_TestUIPoolPanel = "TestUIPoolPanel";
        #endregion
    }
}