#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2019 DarkRabbit(ZhangHan)
///
/// File Name:				ScenarioContentExecutor.cs
/// Author:					DarkRabbit
/// Create Time:			Thu, 03 Jan 2019 23:19:33 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using System.Collections.Generic;
using System.Linq;

namespace DR.Book.SRPG_Dev.ScriptManagement
{
    public abstract class ScenarioContentExecutor : IScenarioContentExecutor
    {
        public string typeName
        {
            get { return GetType().Name; }
        }

        public abstract string code { get; }

        public abstract ActionStatus Execute(IGameAction gameAction, IScenarioContent content, out string error);

        #region Helper Methods
        /// <summary>
        /// 判断变量格式是否正确，并赋值给`variable`。 
        ///     `isExist`: 指定变量在使用之前，是否存在。
        /// </summary>
        /// <param name="varStr"></param>
        /// <param name="isExist"></param>
        /// <param name="variable"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        protected bool IsMatchVar(string varStr, bool isExist, ref string variable, out string error)
        {
            if (!RegexUtility.IsMatchVariable(varStr))
            {
                error = GetMatchVariableErrorString(varStr);
                return false;
            }

            if (ScenarioBlackboard.Contains(varStr) != isExist)
            {
                error = GetVariableExistErrorString(varStr, !isExist);
                return false;
            }

            variable = varStr;
            error = null;
            return true;
        }

        /// <summary>
        /// 如果是数字，直接赋值；如果是变量，就获取变量
        /// </summary>
        /// <param name="numOrVar"></param>
        /// <param name="value"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        protected bool ParseOrGetVarValue(string numOrVar, ref int value, out string error)
        {
            if (!int.TryParse(numOrVar, out value))
            {
                if (!RegexUtility.IsMatchVariable(numOrVar))
                {
                    error = GetMatchVariableErrorString(numOrVar);
                    return false;
                }

                if (!ScenarioBlackboard.TryGet(numOrVar, out value))
                {
                    error = GetVariableExistErrorString(numOrVar, false);
                    return false;
                }
            }

            error = null;
            return true;
        }

        /// <summary>
        /// 转换Action
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inputAction"></param>
        /// <param name="outputAction"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        protected bool ParseAction<T>(IGameAction inputAction, out T outputAction, out string error)
            where T : class, IGameAction
        {
            if (inputAction is T)
            {
                outputAction = inputAction as T;
                error = null;
                return true;
            }

            outputAction = null;
            error = GetActionTypeErrorString(inputAction.GetType().Name, typeof(T).Name);
            return false;
        }
        #endregion

        #region Error String Methods
        protected string GetLengthErrorString(params int[] correctLength)
        {
            if (correctLength == null || correctLength.Length == 0)
            {
                return string.Format(
                    "{0} ParseArgs error: length of `content` is incorrect.",
                    typeName);
            }
            else
            {
                return string.Format(
                    "{0} ParseArgs error: length of `content` must be one of [{1}].",
                    typeName,
                    string.Join(", ", correctLength.Select(length => length.ToString()).ToArray()));
            }
        }

        protected string GetMatchVariableErrorString(string variable)
        {
            return string.Format(
                "{0} ParseArgs error: variable `{1}` match error.",
                typeName,
                variable);
        }

        protected string GetVariableExistErrorString(string variable, bool exist)
        {
            if (exist)
            {
                return string.Format(
                    "{0} ParseArgs error: variable `{1}` is already exist.",
                    typeName,
                    variable);
            }
            else
            {
                return string.Format(
                    "{0} ParseArgs error: variable `{1}` was not found.",
                    typeName,
                    variable);
            }
        }

        protected string GetMatchOperatorErrorString(string op, params string[] operators)
        {
            if (operators == null || operators.Length == 0)
            {
                return string.Format(
                    "{0} ParseArgs error: operator `{1}` is not supported.",
                    typeName,
                    op);
            }
            else
            {
                return string.Format(
                    "{0} ParseArgs error: operator `{1}` is not in [{2}]",
                    typeName,
                    op,
                    string.Join(", ", operators));
            }
        }

        protected string GetActionTypeErrorString(string currentActionType, string correctActionType = null)
        {
            if (string.IsNullOrEmpty(correctActionType))
            {
                return string.Format(
                    "{0} Execute error: action type `{1}` is incorrect.",
                    typeName,
                    currentActionType);
            }
            else
            {
                return string.Format(
                    "{0} Execute error: action type `{1}` does not inhert `{2}`.",
                    typeName,
                    currentActionType,
                    correctActionType);
            }
        }
        #endregion

        #region  Class EqualityComparer
        public class EqualityComparer : IEqualityComparer<IScenarioContentExecutor>
        {
            public bool Equals(IScenarioContentExecutor x, IScenarioContentExecutor y)
            {
                return x.code == y.code;
            }

            public int GetHashCode(IScenarioContentExecutor obj)
            {
                if (obj.code == null)
                {
                    return string.Empty.GetHashCode();
                }
                return obj.code.GetHashCode();
            }
        }
        #endregion

        #region Static Helper/Cache Methods
        private static Dictionary<Type, IScenarioContentExecutor> s_Cache;

        public static IScenarioContentExecutor CreateInstance(Type executorType, out string error)
        {
            if (executorType == null)
            {
                error = "ScenarioContentExecutor -> CreateInstance: type of executor is null.";
                return null;
            }

            if (executorType.IsAbstract)
            {
                error = string.Format(
                    "ScenarioContentExecutor -> CreateInstance: type `{0}` of executor is abstract or interface.",
                    executorType.FullName);
                return null;
            }

            if (!typeof(IScenarioContentExecutor).IsAssignableFrom(executorType))
            {
                error = string.Format(
                    "ScenarioContentExecutor -> CreateInstance: type `{0}` dose not inhert `IScenarioContentExecutor`.",
                    executorType.FullName);
                return null;
            }

            IScenarioContentExecutor executor = Activator.CreateInstance(executorType) as IScenarioContentExecutor;
            if (executor == null)
            {
                error = string.Format(
                    "ScenarioContentExecutor -> CreateInstance: type `{0}` create failure.",
                    executorType.FullName);
                return null;
            }

            if (executor.code == null)
            {
                error = string.Format(
                    "ScenarioContentExecutor -> CreateInstance: code of type `{0}` is null.",
                    executorType.FullName);
                return null;
            }

            error = null;
            return executor;
        }

        public static IScenarioContentExecutor CreateInstance(Type executorType)
        {
            string error;
            return CreateInstance(executorType, out error);
        }

        public static IScenarioContentExecutor GetOrCreateInstance(Type executorType, out string error, bool fromCache = true)
        {
            if (fromCache)
            {
                if (s_Cache == null)
                {
                    s_Cache = new Dictionary<Type, IScenarioContentExecutor>();
                }

                IScenarioContentExecutor executor;
                if (!s_Cache.TryGetValue(executorType, out executor))
                {
                    executor = CreateInstance(executorType, out error);
                    if (executor == null)
                    {
                        return null;
                    }
                    s_Cache.Add(executorType, executor);
                }

                error = null;
                return executor;
            }
            else
            {
                return CreateInstance(executorType, out error);
            }
        }

        public static IScenarioContentExecutor GetOrCreateInstance(Type executorType, bool fromCache = true)
        {
            string error;
            return GetOrCreateInstance(executorType, out error, fromCache);
        }

        public static T CreateInstance<T>(out string error)
            where T : class, IScenarioContentExecutor, new()
        {
            T executor = new T();

            if (executor.code == null)
            {
                error = string.Format(
                    "ScenarioContentExecutor -> CreateInstance: code of type `{0}` is null.",
                    executor.GetType().FullName);
                return null;
            }

            error = null;
            return executor;
        }

        public static T CreateInstance<T>()
            where T : class, IScenarioContentExecutor, new()
        {
            string error;
            return CreateInstance<T>(out error);
        }

        public static T GetOrCreateInstance<T>(out string error, bool fromCache = true)
            where T : class, IScenarioContentExecutor, new()
        {
            if (fromCache)
            {
                if (s_Cache == null)
                {
                    s_Cache = new Dictionary<Type, IScenarioContentExecutor>();
                }

                Type executorType = typeof(T);

                IScenarioContentExecutor executor;
                if (!s_Cache.TryGetValue(executorType, out executor))
                {
                    executor = CreateInstance<T>(out error);
                    if (executor == null)
                    {
                        return null;
                    }
                    s_Cache.Add(executorType, executor);
                }

                error = null;
                return executor as T;
            }
            else
            {
                return CreateInstance<T>(out error);
            }
        }

        public static T GetOrCreateInstance<T>(bool fromCache = true)
            where T : class, IScenarioContentExecutor, new()
        {
            string error;
            return GetOrCreateInstance<T>(out error, fromCache);
        }

        public static IScenarioContentExecutor[] GetOrCreateInstances(
            Type[] executorTypes,
            Action<int, string> triggerError = null,
            bool fromCache = true)
        {
            if (executorTypes == null)
            {
                return new IScenarioContentExecutor[0];
            }

            IScenarioContentExecutor[] executors = new IScenarioContentExecutor[executorTypes.Length];
            string error;
            for (int i = 0; i < executorTypes.Length; i++)
            {
                executors[i] = GetOrCreateInstance(executorTypes[i], out error, fromCache);
                if (!string.IsNullOrEmpty(error))
                {
                    if (triggerError != null)
                    {
                        triggerError(i, error);
                    }
                    error = null;
                }
            }

            return executors;
        }

        public static void GetOrCreateInstances(
            Type[] executorTypes,
            Action<int, IScenarioContentExecutor> loaded,
            Action<int, string> triggerError = null,
            bool fromCache = true,
            bool ignorEmpty = true)
        {
            if (executorTypes == null)
            {
                return;
            }

            string error;
            for (int i = 0; i < executorTypes.Length; i++)
            {
                IScenarioContentExecutor executor = GetOrCreateInstance(executorTypes[i], out error, fromCache);
                if (!string.IsNullOrEmpty(error))
                {
                    if (triggerError != null)
                    {
                        triggerError(i, error);
                    }
                    error = null;
                }

                if (executor == null && ignorEmpty)
                {
                    continue;
                }

                if (loaded != null)
                {
                    loaded(i, executor);
                }
            }
        }

        public static void ClearCachePool()
        {
            s_Cache.Clear();
            s_Cache = null;
        }
        #endregion
    }

    public abstract class ScenarioContentExecutor<T> : ScenarioContentExecutor, 
        IScenarioContentArgParser<T>
    {
        public sealed override ActionStatus Execute(IGameAction gameAction, IScenarioContent content, out string error)
        {
            T args = default(T);
            if (!ParseArgs(content, ref args, out error))
            {
                return ActionStatus.Error;
            }

            return Run(gameAction, content, args, out error);
        }

        public abstract bool ParseArgs(IScenarioContent content, ref T args, out string error);

        protected abstract ActionStatus Run(IGameAction gameAction, IScenarioContent content, T args, out string error);
    }
}