#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				TestEventNames.cs
/// Author:					DarkRabbit
/// Create Time:			Fri, 09 Mar 2018 22:33:25 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

namespace DR.Book.SRPG_Dev.Framework.Test
{
    /// <summary>
    /// 你也可以使用Enum枚举来标识名字。
    /// 使用ToString作为参数
    /// </summary>
    public static class TestEventNames
    {
        /// <summary>
        /// 初始化游戏
        /// </summary>
        public const string TestGameInit = "Event_TestGameInit";

        /// <summary>
        /// 异步读取场景Progress
        /// </summary>
        public const string TestLoadingProgress = "Event_LoadingProgress";
    }
}