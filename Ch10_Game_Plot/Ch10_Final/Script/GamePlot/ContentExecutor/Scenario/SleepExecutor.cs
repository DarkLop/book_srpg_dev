#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2019 DarkRabbit(ZhangHan)
///
/// File Name:				SleepExecutor.cs
/// Author:					DarkRabbit
/// Create Time:			Sun, 13 Jan 2019 01:39:18 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

namespace DR.Book.SRPG_Dev.ScriptManagement
{
    public class SleepExecutor : ScenarioContentExecutor<SleepExecutor.SleepArgs>
    {
        public struct SleepArgs
        {
            public float sec;
        }

        public override string code
        {
            get { return "sleep"; }
        }

        public override bool ParseArgs(IScenarioContent content, ref SleepArgs args, out string error)
        {
            /// sleep 2000;
            /// sleep 5 2000;
            if (content.length != 2 && content.length != 3)
            {
                error = GetLengthErrorString(2, 3);
                return false;
            }

            int msec;
            if (!int.TryParse(content[1], out msec))
            {
                error = string.Format(
                    "{0} ParseArgs -> the argument of time is not a number.",
                    typeName);
                return false;
            }

            if (msec < 0)
            {
                error = string.Format(
                    "{0} ParseArgs -> the argument of time is can not be less than zero.",
                    typeName);
                return false;
            }

            args.sec = msec / 1000f;

            error = null;
            return true;
        }

        protected override ActionStatus Run(IGameAction gameAction, IScenarioContent content, SleepArgs args, out string error)
        {
            GameDirector.instance.StartTimer(-1, args.sec, gameAction.TimerTimeout);
            error = null;
            return ActionStatus.WaitTimerTimeout;
        }
    }
}