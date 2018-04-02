#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				JsonConfigFile.cs
/// Author:					DarkRabbit
/// Create Time:			Tue, 06 Mar 2018 03:11:54 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using System.Text;
using UnityEngine;

namespace DR.Book.SRPG_Dev.Framework
{
    public abstract class JsonConfigFile : ConfigFile
    {
        protected override void ConstructInfo(ref Info info)
        {
            info.relative = "Json/";
            info.name = "JsonConfig.json";
            info.loadType = LoadType.WWW;
            info.pathInAssetBundle = null;
        }

        protected sealed override void Format(Type type, byte[] bytes, ref ConfigFile config)
        {
            string json = Encoding.UTF8.GetString(bytes).Trim();
            FormatBuffer(json); 
        }

        /// <summary>
        /// Bytes转换成json后执行
        /// </summary>
        /// <param name="buffer"></param>
        protected virtual void FormatBuffer(string buffer)
        {
            JsonUtility.FromJsonOverwrite(buffer, this);
        }
    }
}