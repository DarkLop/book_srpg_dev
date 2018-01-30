#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2017 DarkRabbit(ZhangHan)
///
/// File Name:				ChangeAllFileToUTF8.cs
/// Author:					DarkRabbit
/// Create Time:			Wed, 28 Jun 2017 07:33:25 GMT
/// Modifier:
/// Module Description:
/// **********************************************************************
#endregion ---------- File Info ----------

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace DR
{

    public class ChangeMonoScriptsToUTF8
    {
        [MenuItem("DRTools/Helper/Change MonoScripts to UTF8")]
        static void Change()
        {
            if (!EditorUtility.DisplayDialog("Change MonoScripts", "Are you sure?", "Sure", "Not Sure"))
            {
                return;
            }

            string[] allPath = AssetDatabase.GetAllAssetPaths();

            EditorUtility.DisplayProgressBar("Change File", "Changing...", 0f);

            for (int i = 0; i < allPath.Length; i++)
            {
                float progress = (float)i / allPath.Length;
                EditorUtility.DisplayProgressBar("Change File", "Changing... (" + i + ", " + allPath.Length + "): " + allPath[i], progress);

                if (!allPath[i].Contains("Assets"))
                {
                    continue;
                }

                UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(allPath[i]);
                if (obj == null)
                {
                    continue;
                }

                if (obj is MonoScript)
                {
                    string path = System.Environment.CurrentDirectory.Replace("\\", "/") + "/" + allPath[i];
                    try
                    {
                        string text = File.ReadAllText(path);
                        using (StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.UTF8))
                        {
                            sw.Write(text);
                            sw.Close();
                        }
                    }
                    catch
                    {
                        Debug.LogWarning("Change Failure. Path: " + path);
                        continue;
                    }
                }

            }

            EditorUtility.ClearProgressBar();
        }
    }
}
