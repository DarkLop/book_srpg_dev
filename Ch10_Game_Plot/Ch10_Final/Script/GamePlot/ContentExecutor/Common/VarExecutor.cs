#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2019 DarkRabbit(ZhangHan)
///
/// File Name:				VarExecutor.cs
/// Author:					DarkRabbit
/// Create Time:			Thu, 03 Jan 2019 23:55:07 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

namespace DR.Book.SRPG_Dev.ScriptManagement
{
    /// <summary>
    /// 创建变量
    /// </summary>
	public class VarExecutor : ScenarioContentExecutor<VarExecutor.VarArgs>
    {
        public struct VarArgs
        {
            public string name;
            public int value;
        }

        public override string code
        {
            get { return "var"; }
        }

        public override bool ParseArgs(IScenarioContent content, ref VarArgs args, out string error)
        {
            // var a;
            // var b = 10;
            // var c = b;
            if (content.length != 2 && content.length != 4)
            {
                error = GetLengthErrorString(2, 4);
                return false;
            }

            // 变量名只能包含数字，字母（中文），下划线，且不能以数字开头
            if (!RegexUtility.IsMatchVariable(content[1]))
            {
                error = GetMatchVariableErrorString(content[1]);
                return false;
            }
            args.name = content[1];

            //// 你也可以使用这个方法，这里确保了变量必须不存在
            //if (!IsMatchVar(content[1], false, ref args.name, out error))
            //{
            //    return false;
            //}

            args.value = 0;

            if (content.length == 4)
            {
                if (content[2] != "=")
                {
                    error = GetMatchOperatorErrorString(content[2], "=");
                    return false;
                }

                if (!ParseOrGetVarValue(content[3], ref args.value, out error))
                {
                    return false;
                }
            }

            error = null;
            return true;
        }

        protected override ActionStatus Run(IGameAction gameAction, IScenarioContent content, VarArgs args, out string error)
        {
            ScenarioBlackboard.Set(args.name, args.value);
            error = null;
            return ActionStatus.Continue;
        }

    }
}