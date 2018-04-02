#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				XmlConfigFile.cs
/// Author:					DarkRabbit
/// Create Time:			Tue, 06 Mar 2018 03:11:44 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using System.IO;
using System.Xml.Serialization;

namespace DR.Book.SRPG_Dev.Framework
{
    public abstract class XmlConfigFile : ConfigFile
    {
        protected override void ConstructInfo(ref Info info)
        {
            info.relative = "Xml/";
            info.name = "XmlConfig.xml";
            info.loadType = LoadType.WWW;
            info.pathInAssetBundle = null;
        }

        protected sealed override void Format(Type type, byte[] bytes, ref ConfigFile config)
        {
            XmlConfigFile buffer;
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                XmlSerializer xs = new XmlSerializer(type);
                buffer = xs.Deserialize(ms) as XmlConfigFile;
            }

            if (buffer != null)
            {
                config = FormatBuffer(buffer);
            }
        }

        /// <summary>
        /// 反序列化完成后执行
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        protected abstract XmlConfigFile FormatBuffer(XmlConfigFile buffer);
    }
}