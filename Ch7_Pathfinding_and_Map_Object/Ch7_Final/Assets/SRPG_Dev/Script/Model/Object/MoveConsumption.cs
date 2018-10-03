#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				MoveConsumption.cs
/// Author:					DarkRabbit
/// Create Time:			Thu, 06 Sep 2018 08:32:28 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using UnityEngine;

namespace DR.Book.SRPG_Dev.Models
{
    using DR.Book.SRPG_Dev.Maps;

    public class MoveConsumption
    {
        private MoveConsumptionInfo m_MoveConsumptionInfo;

        public float this[TerrainType terrainType]
        {
            get
            {
                if (terrainType == TerrainType.MaxLength)
                {
                    Debug.LogError("MoveConsuption -> TerrainType can not be MaxLength.");
                    return 0;
                }
                return m_MoveConsumptionInfo.consumptions[terrainType.ToInteger()];
            }
        }

        public MoveConsumption(ClassType classType)
        {
            // TODO Load from config file
            m_MoveConsumptionInfo = new MoveConsumptionInfo
            {
                type = classType,
                consumptions = new float[TerrainType.MaxLength.ToInteger()]
            };
            for (int i = 0; i < m_MoveConsumptionInfo.consumptions.Length; i++)
            {
                m_MoveConsumptionInfo.consumptions[i] = UnityEngine.Random.Range(0.5f, 3f);
            }
        }

        //TODO Other
    }
}