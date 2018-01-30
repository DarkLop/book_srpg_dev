#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2017 DarkRabbit(ZhangHan)
///
/// File Name:				ScriptTitleChange.cs
/// Author:					DarkRabbit
/// Create Time:			
/// Modifier:
/// Module Description:
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace DR
{
    public class ScriptTitleChange : UnityEditor.AssetModificationProcessor
    {
        [MenuItem("DRTools/Helper/Change Script Title")]
        public static void ChangeScriptToDarkRabbit()
        {
            string[] paths = AssetDatabase.GetAllAssetPaths();
            EditorUtility.DisplayProgressBar("Change Title", "Changing...", 0f);

            for (int i = 0; i < paths.Length; i++)
            {
                float progress = (float)i / paths.Length;
                EditorUtility.DisplayProgressBar("Change Title", "Changing... (" + i + ", " + paths.Length + "): " + paths[i], progress);

                if (!paths[i].ToLower().EndsWith(".cs"))
                {
                    continue;
                }

                if (paths[i].ToLower().EndsWith("scripttitlechange.cs"))
                {
                    continue;
                }

                string path = paths[i];
                string allText = File.ReadAllText(path);
                if (allText.Contains("#AuthorName#") || allText.Contains("#CreateTime#") || allText.Contains("#CreateTimeYear#"))
                {
                    allText = allText.Replace("#AuthorName#", "DarkRabbit")
                    .Replace("#CreateTime#", DateTime.Now.ToString("R")).Replace("#CreateTimeYear#", DateTime.Now.Year.ToString());
                    using (StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.UTF8))
                    {
                        sw.Write(allText);
                        sw.Close();
                    }
                }
            }

            EditorUtility.ClearProgressBar();
        }
        
        public static void OnWillCreateAsset(string path)
        {
            path = path.Replace(".meta", "");
            if (path.ToLower().EndsWith(".cs"))
            {
                string allText = File.ReadAllText(path);
                allText = allText.Replace("#AuthorName#", "DarkRabbit")
                    .Replace("#CreateTime#", DateTime.Now.ToString("R")).Replace("#CreateTimeYear#", DateTime.Now.Year.ToString());
                using (StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.UTF8))
                {
                    sw.Write(allText);
                    sw.Close();
                }
            }
        }
    }
}
