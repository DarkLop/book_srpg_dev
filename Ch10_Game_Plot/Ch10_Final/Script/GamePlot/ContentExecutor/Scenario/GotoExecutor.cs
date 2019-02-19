#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2019 DarkRabbit(ZhangHan)
///
/// File Name:				GotoExecutor.cs
/// Author:					DarkRabbit
/// Create Time:			Thu, 03 Jan 2019 23:54:49 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

namespace DR.Book.SRPG_Dev.ScriptManagement
{
    public class GotoExecutor : ScenarioContentExecutor<GotoExecutor.GotoArgs>
	{
        public struct GotoArgs
        {
            public string flag;
        }

        public override string code
        {
            get { return "goto"; }
        }

        public override bool ParseArgs(IScenarioContent content, ref GotoArgs args, out string error)
        {
            // goto #flag;
            if (content.length != 2)
            {
                error = GetLengthErrorString(2);
                return false;
            }

            args.flag = content[1];
            error = null;
            return true;
        }

        protected override ActionStatus Run(IGameAction gameAction, IScenarioContent content, GotoArgs args, out string error)
        {
            ScenarioAction action;
            if (!ParseAction<ScenarioAction>(gameAction, out action, out error))
            {
                return ActionStatus.Error;
            }

            return action.GotoCommand(args.flag, out error);
        }
    }
}