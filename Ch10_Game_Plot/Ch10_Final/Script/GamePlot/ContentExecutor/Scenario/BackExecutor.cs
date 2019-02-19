#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2019 DarkRabbit(ZhangHan)
///
/// File Name:				BackExecutor.cs
/// Author:					DarkRabbit
/// Create Time:			Tue, 22 Jan 2019 12:41:11 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

namespace DR.Book.SRPG_Dev.ScriptManagement
{
    public class BackExecutor : ScenarioContentExecutor
    {
        public override string code
        {
            get { return "back"; }
        }

        public override ActionStatus Execute(IGameAction gameAction, IScenarioContent content, out string error)
        {
            if (content.length != 1)
            {
                error = GetLengthErrorString(1);
                return ActionStatus.Error;
            }

            error = null;
            return ActionStatus.BackAction;
        }
    }
}