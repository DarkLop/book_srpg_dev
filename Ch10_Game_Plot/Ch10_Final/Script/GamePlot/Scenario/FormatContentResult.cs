#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2019 DarkRabbit(ZhangHan)
///
/// File Name:				FormatContentResult.cs
/// Author:					DarkRabbit
/// Create Time:			Tue, 01 Jan 2019 23:14:58 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

namespace DR.Book.SRPG_Dev.ScriptManagement
{
    /// <summary>
    /// 格式化结果
    /// </summary>
    public enum FormatContentResult
    {
        /// <summary>
        /// 成功
        /// </summary>
        Succeed,

        /// <summary>
        /// 失败
        /// </summary>
        Failure,

        /// <summary>
        /// 只有注释
        /// </summary>
        Commenting
    }
}