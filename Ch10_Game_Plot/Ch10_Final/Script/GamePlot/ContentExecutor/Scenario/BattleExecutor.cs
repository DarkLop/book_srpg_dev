#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2019 DarkRabbit(ZhangHan)
///
/// File Name:				MapExecutor.cs
/// Author:					DarkRabbit
/// Create Time:			Mon, 14 Jan 2019 02:25:02 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

namespace DR.Book.SRPG_Dev.ScriptManagement
{
    public class BattleExecutor : ScenarioContentExecutor<BattleExecutor.BattleArgs>
    {
        public struct BattleArgs
        {
            public string sceneName;
            public string scriptName;
        }

        public override string code
        {
            get { return "battle"; }
        }

        public override bool ParseArgs(IScenarioContent content, ref BattleArgs args, out string error)
        {
            // map Stage0Scene stage0script;
            if (content.length != 3)
            {
                error = GetLengthErrorString(3);
                return false;
            }

            args.sceneName = content[1];
            args.scriptName = content[2];

            error = null;
            return true;
        }

        protected override ActionStatus Run(IGameAction gameAction, IScenarioContent content, BattleArgs args, out string error)
        {
            error = null;
            ScenarioBlackboard.battleMapScene = args.sceneName;
            ScenarioBlackboard.mapScript = args.scriptName;
            if (!GameDirector.instance.LoadMap(args.sceneName))
            {
                return ActionStatus.Error;
            }

            ScenarioBlackboard.Set(args.sceneName, 0);
            error = null;
            return ActionStatus.WaitMapDone;
        }
    }
}