#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				TxtConfigFile.cs
/// Author:					DarkRabbit
/// Create Time:			Tue, 06 Mar 2018 03:12:02 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using System.Text;

namespace DR.Book.SRPG_Dev.Framework
{
    public abstract class TxtConfigFile : ConfigFile
    {
        protected override void ConstructInfo(ref Info info)
        {
            info.relative = "Txt/";
            info.name = "TxtConfig.txt";
            info.loadType = LoadType.WWW;
            info.pathInAssetBundle = null;
        }

        protected sealed override void Format(Type type, byte[] bytes, ref ConfigFile config)
        {
            string text = Encoding.UTF8.GetString(bytes).Trim();
            FormatBuffer(text);
        }

        /// <summary>
        /// Bytes转换成text后执行
        /// </summary>
        /// <param name="buffer"></param>
        protected abstract void FormatBuffer(string buffer);
    }
}