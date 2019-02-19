#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2019 DarkRabbit(ZhangHan)
///
/// File Name:				SetFlagExecutor.cs
/// Author:					DarkRabbit
/// Create Time:			Fri, 04 Jan 2019 17:24:35 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

namespace DR.Book.SRPG_Dev.ScriptManagement
{
    public class SetFlagExecutor :  ScenarioContentExecutor<SetFlagExecutor.SetFlagArgs>
    {
        public struct SetFlagArgs
        {
            public string flag;
        }

        public override string code
        {
            get { return string.Empty; }
        }

        public override bool ParseArgs(IScenarioContent content, ref SetFlagArgs args, out string error)
        {
            // #flag0
            // 剧情标识符只能有一个参数
            if (content.length != 1)
            {
                error = GetLengthErrorString(1);
                return false;
            }

            args.flag = content.code;
            error = null;
            return true;
        }

        protected override ActionStatus Run(IGameAction gameAction, IScenarioContent content, SetFlagArgs args, out string error)
        {
            ScenarioAction action;
            if (!ParseAction<ScenarioAction>(gameAction, out action, out error))
            {
                return ActionStatus.Error;
            }

            return action.SetFlagCommand(args.flag, out error);
        }
    }
}