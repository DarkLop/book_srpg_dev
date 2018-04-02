#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				OnLoadSceneArgs.cs
/// Author:					DarkRabbit
/// Create Time:			Mon, 05 Mar 2018 17:28:23 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using UnityEngine.SceneManagement;

namespace DR.Book.SRPG_Dev.Framework
{
    public class OnLoadSceneArgs : MessageArgs
    {
        public Scene activeScene;
        public int buildIndex;
        public string sceneName;
        public LoadSceneType type;
        public LoadSceneMode mode;
        public bool async;
    }
}