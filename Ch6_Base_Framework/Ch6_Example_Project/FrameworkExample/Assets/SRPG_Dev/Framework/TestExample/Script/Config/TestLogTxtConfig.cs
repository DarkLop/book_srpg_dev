#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				TestLogTxtConfig.cs
/// Author:					DarkRabbit
/// Create Time:			Sun, 11 Mar 2018 00:54:11 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;

namespace DR.Book.SRPG_Dev.Framework.Test
{
    public class TestLogTxtConfig : TxtConfigFile
    {
        public string name { get; set; }
        public int hp { get; set; }
        public int atk { get; set; }
        public int def { get; set; }

        /// <summary>
        /// 文件Info构造器
        /// </summary>
        /// <param name="info"></param>
        protected override void ConstructInfo(ref Info info)
        {
            info.relative = "Txt";
            info.name = "TestLogTxtConfig.txt"; //如果是使用Resources读取不要加后缀
            info.loadType = LoadType.WWW;
            info.pathInAssetBundle = string.Empty;
        }

        protected override void FormatBuffer(string buffer)
        {
            try
            {
                string[] lines = buffer.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                name = lines[0];
                hp = int.Parse(lines[1]);
                atk = int.Parse(lines[2]);
                def = int.Parse(lines[3]);
            }
            catch
            {
                throw new Exception("Check the config file named 'TestLogTxtConfig.txt'.");
            }
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}, {2}, {3}", name, hp, atk, def);
        }
    }
}