#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2019 DarkRabbit(ZhangHan)
///
/// File Name:				CalcExecutor.cs
/// Author:					DarkRabbit
/// Create Time:			Fri, 04 Jan 2019 02:16:18 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

namespace DR.Book.SRPG_Dev.ScriptManagement
{
    public class CalcExecutor : ScenarioContentExecutor<CalcExecutor.CalcArgs>
    {
        public struct CalcArgs
        {
            public string name;
            public string equalOp;
            public int value1;
            public string binaryOp;
            public int value2;
        }

        public override string code
        {
            get { return "calc"; }
        }

        public override bool ParseArgs(IScenarioContent content, ref CalcArgs args, out string error)
        {
            // calc var += 10;
            // calc var = var + 10;
            if (content.length != 4 && content.length != 6)
            {
                error = GetLengthErrorString(4, 6);
                return false;
            }

            // 在计算之前，变量必须存在
            if (!IsMatchVar(content[1], true, ref args.name, out error))
            {
                return false;
            }

            if (!IsMatchEqualOperator(content[2], ref args.equalOp, out error))
            {
                return false;
            }

            if (!ParseOrGetVarValue(content[3], ref args.value1, out error))
            {
                return false;
            }

            if (content.length == 4)
            {
                args.binaryOp = "+";
                args.value2 = 0;
            }
            else
            {
                if (!IsMatchBinaryOperator(content[4], ref args.binaryOp, out error))
                {
                    return false;
                }

                if (!ParseOrGetVarValue(content[5], ref args.value2, out error))
                {
                    return false;
                }
            }

            error = null;
            return true;
        }

        protected override ActionStatus Run(IGameAction gameAction, IScenarioContent content, CalcArgs args, out string error)
        {
            int binaryResult;
            if (!CalculateBinaryResult(args.binaryOp, args.value1, args.value2, out binaryResult, out error))
            {
                return ActionStatus.Error;
            }

            int equalResult;
            if (args.equalOp == "=")
            {
                equalResult = binaryResult;
            }
            else
            {
                int oldValue = ScenarioBlackboard.Get(args.name);
                if (!CalculateBinaryResult(args.equalOp, oldValue, binaryResult, out equalResult, out error))
                {
                    return ActionStatus.Error;
                }
            }

            ScenarioBlackboard.Set(args.name, equalResult);
            return ActionStatus.Continue;
        }

        #region Match Methods
        protected bool IsMatchEqualOperator(string opStr, ref string equalOp, out string error)
        {
            if (opStr == "=")
            {
                equalOp = opStr;
            }
            else
            {
                if (!IsMatchBinaryOperator(opStr.Substring(0, 1), ref equalOp, out error))
                {
                    error = GetMatchOperatorErrorString(
                        opStr,
                        "=",
                        "+=", "-=", "*=", "/=",
                        "&=", "|=", "^=");
                    return false;
                }
            }

            error = null;
            return true;
        }

        protected bool IsMatchBinaryOperator(string opStr, ref string binaryOp, out string error)
        {
            switch (opStr)
            {
                case "+":
                case "-":
                case "*":
                case "/":
                case "&":
                case "|":
                case "^":
                    binaryOp = opStr;
                    error = null;
                    return true;
                default:
                    error = GetMatchOperatorErrorString(
                        opStr,
                        "+", "-", "*", "/",
                        "&", "|", "^");
                    return false;
            }
        }
        #endregion

        #region Calculate Methods
        protected bool CalculateBinaryResult(string binaryOp, int value1, int value2, out int binaryResult, out string error)
        {
            binaryResult = 0;
            switch (binaryOp)
            {
                case "+":
                    binaryResult = value1 + value2;
                    break;
                case "-":
                    binaryResult = value1 - value2;
                    break;
                case "*":
                    binaryResult = value1 * value2;
                    break;
                case "/":
                    if (value2 == 0)
                    {
                        error = "CalcExecutor -> the dividend can not be zero.";
                        return false;
                    }
                    binaryResult = value1 / value2;
                    break;
                case "&":
                    binaryResult = value1 & value2;
                    break;
                case "|":
                    binaryResult = value1 | value2;
                    break;
                case "^":
                    binaryResult = value1 ^ value2;
                    break;
            }

            error = null;
            return true;
        }
        #endregion
    }
}