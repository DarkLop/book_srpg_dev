#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2019 DarkRabbit(ZhangHan)
///
/// File Name:				IScenarioContentContaner.cs
/// Author:					DarkRabbit
/// Create Time:			Sat, 05 Jan 2019 21:58:11 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;

namespace DR.Book.SRPG_Dev.ScriptManagement
{
    public interface IScenarioContentExecutorContainer
    {
        void LoadExecutors(params Type[] executorTypes);
        bool SetExecutor(IScenarioContentExecutor executor, bool existOverride = true);
        IScenarioContentExecutor GetExecutor(string code);
        IScenarioContentExecutor[] GetAllExecutors();
        bool RemoveExecutor(string code);
        void ClearExecutors();
    }
}