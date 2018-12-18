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
        public MoveConsumptionInfo info { get; private set; }

        public ClassType classType
        {
            get { return info.classType; }
        }

        public float this[TerrainType terrainType]
        {
            get
            {
                if (terrainType == TerrainType.MaxLength)
                {
                    Debug.LogError("MoveConsumption -> TerrainType can not be MaxLength.");
                    return 255f;
                }
                return info.consumptions[terrainType.ToInteger()];
            }
        }

        public MoveConsumption(MoveConsumptionInfo info)
        {
            this.info = info;
        }
    }
}