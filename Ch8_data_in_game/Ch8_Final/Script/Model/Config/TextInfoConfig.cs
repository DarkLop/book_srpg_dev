#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				TextInfoConfig.cs
/// Author:					DarkRabbit
/// Create Time:			Sun, 07 Oct 2018 23:40:09 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using UnityEngine;

namespace DR.Book.SRPG_Dev.Models
{
    [Serializable]
    public class TextInfoConfig : BaseTxtConfig<int, TextInfo>
    {

    }

    [Serializable]
    public class TextInfo : ITxtConfigData<int>
    {
        public int id;
        public string text;

        public bool FormatText(string line)
        {
            string[] words = line.Split('\t');
            if (words.Length != 2)
            {
                Debug.LogErrorFormat("{0} -> `FormatText` ERROR: Length error.", GetType().Name);
                return false;
            }

            try
            {
                id = int.Parse(words[0]);
                text = words[1].Trim();
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("{0} -> `FormatText` ERROR: {1}", GetType().Name, e.ToString());
                return false;
            }
            return true;
        }

        public int GetKey()
        {
            return this.id;
        }
    }
}