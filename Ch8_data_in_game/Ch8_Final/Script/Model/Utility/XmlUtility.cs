#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				XmlUtility.cs
/// Author:					DarkRabbit
/// Create Time:			Fri, 05 Oct 2018 12:05:29 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace DR.Book.SRPG_Dev.Models
{
    using DR.Book.SRPG_Dev.Framework;

    public static class XmlUtility
    {
        public static T ObjectFromXmlBytes<T>(byte[] xml) where T : XmlConfigFile, new()
        {
            return ObjectFromXmlBytes(xml, typeof(T)) as T;
        }

        public static XmlConfigFile ObjectFromXmlBytes(byte[] xml, Type type)
        {
            if (xml == null)
            {
                return null;
            }

            try
            {
                XmlConfigFile config;
                using (MemoryStream ms = new MemoryStream(xml))
                {
                    using (StreamReader sr = new StreamReader(ms, Encoding.UTF8))
                    {
                        XmlSerializer xs = new XmlSerializer(type);
                        config = xs.Deserialize(sr) as XmlConfigFile;
                    }
                }
                return config;
            }
            catch
            {
                return null;
            }
        }

        public static byte[] ObjectToXmlBytes(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            try
            {
                byte[] bytes;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (StreamWriter sw = new StreamWriter(ms, Encoding.UTF8))
                    {
                        XmlSerializer xs = new XmlSerializer(obj.GetType());
                        XmlSerializerNamespaces xsn = new XmlSerializerNamespaces();
                        xsn.Add("", "");
                        xs.Serialize(sw, obj, xsn);
                        bytes = ms.ToArray();
                    }
                }
                return bytes;
            }
            catch
            {
                return null;
            }
        }
    }
}