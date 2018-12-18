#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				MoveConsumptionInfo.cs
/// Author:					DarkRabbit
/// Create Time:			Thu, 06 Sep 2018 08:22:42 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using System;
using System.Xml.Serialization;

namespace DR.Book.SRPG_Dev.Models
{
    using DR.Book.SRPG_Dev.Framework;
    using DR.Book.SRPG_Dev.Maps;

    [Serializable]
    public class MoveConsumptionInfoConfig : BaseXmlConfig<ClassType, MoveConsumptionInfo>
    {
        protected override XmlConfigFile FormatBuffer(XmlConfigFile buffer)
        {
            // 长度要和TerrainType数量保持一致
            int length = (int)TerrainType.MaxLength;
            MoveConsumptionInfoConfig config = buffer as MoveConsumptionInfoConfig;
            for (int i = 0; i < config.datas.Length; i++)
            {
                if (config.datas[i].consumptions.Length != length)
                {
                    int oldLength = config.datas[i].consumptions.Length;
                    Array.Resize(ref config.datas[i].consumptions, length);
                    for (int j = oldLength; j < length; j++)
                    {
                        config.datas[i].consumptions[j] = 255f;
                    }
                }
            }

            return base.FormatBuffer(config);
        }
    }

    [Serializable]
    public class MoveConsumptionInfo : IConfigData<ClassType>
    {
        /// <summary>
        /// 职业类型
        /// </summary>
        [XmlAttribute]
        public ClassType classType;

        /// <summary>
        /// 在各个地形的移动消耗具体数值
        /// </summary>
        [XmlAttribute]
        public float[] consumptions;

        public ClassType GetKey()
        {
            return this.classType;
        }
    }
}