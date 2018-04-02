#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				TestLogJsonConfig.cs
/// Author:					DarkRabbit
/// Create Time:			Sun, 11 Mar 2018 00:54:03 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------


namespace DR.Book.SRPG_Dev.Framework.Test
{
    public class TestLogJsonConfig : JsonConfigFile
    {
        public string data;

        protected override void ConstructInfo(ref Info info)
        {
            info.relative = "Json/";
            info.name = "TestLogJsonConfig.json"; //如果是使用Resources读取不要加后缀
            info.loadType = LoadType.WWW;
            info.pathInAssetBundle = string.Empty;
        }
    }
}