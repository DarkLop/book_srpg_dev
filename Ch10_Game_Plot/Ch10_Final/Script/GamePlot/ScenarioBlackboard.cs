#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2019 DarkRabbit(ZhangHan)
///
/// File Name:				ScenarioBlackboard.cs
/// Author:					DarkRabbit
/// Create Time:			Fri, 04 Jan 2019 01:53:47 GMT
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
    public static class ScenarioBlackboard
    {
        [Serializable]
        public struct VarValuePair
        {
            public string name;
            public int value;

            public VarValuePair(string name, int value)
            {
                this.name = name;
                this.value = value;
            }
        }

        private static string s_LastScenarioScene;
        private static string s_BattleMapScene;
        private static string s_MapScript;
        private readonly static Dictionary<string, VarValuePair> s_VarValues = new Dictionary<string, VarValuePair>();

        public static string lastScenarioScene
        {
            get { return s_LastScenarioScene; }
            set { s_LastScenarioScene = value; }
        }

        public static string battleMapScene
        {
            get { return s_BattleMapScene; }
            set { s_BattleMapScene = value; }
        }

        public static string mapScript
        {
            get { return s_MapScript; }
            set { s_MapScript = value; }
        }

        public static bool Contains(string name)
        {
            return s_VarValues.ContainsKey(name);
        }

        public static void Set(string name, int value)
        {
            s_VarValues[name] = new VarValuePair(name, value);
        }

        public static bool TryGet(string name, out int value)
        {
            value = 0;
            if (!s_VarValues.ContainsKey(name))
            {
                return false;
            }

            value = s_VarValues[name].value;
            return true;
        }

        public static int Get(string name, int defaultValue = 0)
        {
            int value = defaultValue;
            if (!TryGet(name, out value))
            {
                Set(name, value);
            }

            return value;
        }

        public static bool Remove(string name)
        {
            return s_VarValues.Remove(name);
        }

        public static void Clear()
        {
            s_VarValues.Clear();
        }

        public static VarValuePair[] ToArray()
        {
            return s_VarValues.Values.ToArray();
        }
    }
}