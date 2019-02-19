#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2019 DarkRabbit(ZhangHan)
///
/// File Name:				EditorTestGamePlot.cs
/// Author:					DarkRabbit
/// Create Time:			Sat, 12 Jan 2019 23:56:52 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DR.Book.SRPG_Dev.ScriptManagement.Testing
{
    using DR.Book.SRPG_Dev.Framework;
    using DR.Book.SRPG_Dev.Models;

    public class EditorTestGamePlot : MonoBehaviour
    {
        public bool m_DebugInfo = true;
        public string m_TestScript = "test";
        public bool m_IsTxt = true;

        #region Unity Callback
#if UNITY_EDITOR
        private void Awake()
        {
            ConfigLoader.rootDirectory = Application.streamingAssetsPath + "/Config";
            ConfigLoader.LoadConfig(typeof(TextInfoConfig));

            GameDirector.instance.debugInfo = m_DebugInfo;
            GameDirector.instance.firstScenario = m_TestScript;
            GameDirector.instance.firstScenarioIsTxt = m_IsTxt;
        }

        private void Start()
        {
            if (!GameDirector.instance.LoadFirstScenario())
            {
                Debug.LogError("Load first scenario error.");
                return;
            }

            GameDirector.instance.RunGameAction();
        }
#endif
        #endregion
    }
}