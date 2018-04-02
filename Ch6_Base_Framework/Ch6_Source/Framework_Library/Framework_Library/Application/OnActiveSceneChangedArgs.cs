#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				OnActiveSceneChangedArgs.cs
/// Author:					DarkRabbit
/// Create Time:			Mon, 05 Mar 2018 17:08:27 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using UnityEngine.SceneManagement;

namespace DR.Book.SRPG_Dev.Framework
{
    public class OnActiveSceneChangedArgs : MessageArgs
    {
        public Scene scene1;
        public Scene scene2;
    }
}