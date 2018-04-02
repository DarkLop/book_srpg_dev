#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				DontDestroyGameObject.cs
/// Author:					DarkRabbit
/// Create Time:			Mon, 05 Mar 2018 19:22:18 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using UnityEngine;

namespace DR.Book.SRPG_Dev.Framework
{
    [AddComponentMenu("SRPG/Dont Destroy GameObject")]
    public sealed class DontDestroyGameObject : MonoBehaviour
    {

        #region Unity Callback
        private void Awake()
        {
            GameObject.DontDestroyOnLoad(gameObject);
        }
        #endregion

        public static void DontDestroy(GameObject gameObject)
        {
            if (gameObject == null)
            {
                return;
            }

            gameObject.AddComponent<DontDestroyGameObject>();
        }
    }
}