#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2019 DarkRabbit(ZhangHan)
///
/// File Name:				ScenarioActionStatus.cs
/// Author:					DarkRabbit
/// Create Time:			Sat, 05 Jan 2019 21:56:26 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

namespace DR.Book.SRPG_Dev.ScriptManagement
{
    public enum ActionStatus : int
    {
        #region Common
        /// <summary>
        /// 错误
        /// </summary>
        Error = -1,

        /// <summary>
        /// 继续
        /// </summary>
        Continue = 0,

        /// <summary>
        /// 下一帧
        /// </summary>
        NextFrame,

        /// <summary>
        /// 等待输入
        /// </summary>
        WaitInput,

        /// <summary>
        /// 返回上一个Action
        /// </summary>
        BackAction,
        #endregion

        #region Scenario
        /// <summary>
        /// 等待文本写入完成
        /// </summary>
        WaitWriteTextDone,

        /// <summary>
        /// 等待计时器结束
        /// </summary>
        WaitTimerTimeout,

        /// <summary>
        /// 等待菜单选择
        /// </summary>
        WaitMenuOption,

        /// <summary>
        /// 等待关卡结束
        /// </summary>
        WaitMapDone,
        #endregion

        #region Map
        /// <summary>
        /// 等待剧情
        /// </summary>
        WaitScenarioDone,

        /// <summary>
        /// 等待地图菜单
        /// </summary>
        WaitMapMenuDone,

        /// <summary>
        /// 等待地图动画
        /// </summary>
        WaitMapAnimaDone,
        #endregion
    }
}