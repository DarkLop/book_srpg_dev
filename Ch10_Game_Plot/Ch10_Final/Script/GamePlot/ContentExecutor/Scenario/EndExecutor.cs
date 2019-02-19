#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2019 DarkRabbit(ZhangHan)
///
/// File Name:				EndExecutor.cs
/// Author:					DarkRabbit
/// Create Time:			Mon, 14 Jan 2019 11:26:48 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using UnityEngine.SceneManagement;

namespace DR.Book.SRPG_Dev.ScriptManagement
{
    public class EndExecutor : ScenarioContentExecutor
    {
        public override string code
        {
            get { return "end"; }
        }

        public override ActionStatus Execute(IGameAction gameAction, IScenarioContent content, out string error)
        {
            if (content.length > 1)
            {
                error = GetLengthErrorString(1);
                return ActionStatus.Error;
            }

            error = null;
            GameDirector.instance.GameActionRunOver();
            GameMain.instance.LoadScene(GameMain.instance.startScene, LoadSceneMode.Single);
            return ActionStatus.NextFrame;
        }
    }
}