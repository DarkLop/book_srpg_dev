#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				XmlDocConfigFile.cs
/// Author:					DarkRabbit
/// Create Time:			Wed, 07 Mar 2018 01:26:58 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using System.Text;
using System.Xml;

namespace DR.Book.SRPG_Dev.Framework
{
    public abstract class XmlDocConfigFile : ConfigFile
    {
        protected override void ConstructInfo(ref Info info)
        {
            info.relative = "Xml/";
            info.name = "XmlDocConfig.xml";
            info.loadType = LoadType.WWW;
            info.pathInAssetBundle = null;
        }

        protected sealed override void Format(Type type, byte[] bytes, ref ConfigFile config)
        {
            string xml = Encoding.UTF8.GetString(bytes).Trim();
            XmlDocument buffer = new XmlDocument();
            buffer.LoadXml(xml);
            FormatBuffer(buffer);
        }

        /// <summary>
        /// Bytes转换成XmlDocument后执行
        /// </summary>
        /// <param name="buffer"></param>
        protected abstract void FormatBuffer(XmlDocument buffer);
    }
}