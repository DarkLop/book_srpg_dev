#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				TestRuntime.cs
/// Author:					DarkRabbit
/// Create Time:			Sun, 01 Apr 2018 16:16:48 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace DR.Book.SRPG_Dev
{
    using DR.Book.SRPG_Dev.Directors;

	public class TestRuntime : MonoBehaviour 
	{
		#region Unity Callback
		private void Awake()
		{
			
		}
	
		private void Start () 
		{
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("a what;");
            sb.AppendLine("b the; // haha");
            sb.AppendLine("#b;");
            sb.AppendLine("c f***;");
            sb.AppendLine("// what the");
            sb.AppendLine("d ddddd;");

            TextScript script = new TextScript("Test", sb.ToString());

            Debug.Log(script.buffer);
            Debug.Log(script.ToString(true));
        }
	
		private void Update () 
		{
			
		}
		#endregion
	}
}