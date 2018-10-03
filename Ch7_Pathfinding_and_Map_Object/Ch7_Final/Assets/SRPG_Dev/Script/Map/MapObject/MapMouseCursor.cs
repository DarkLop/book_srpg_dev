#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				MapMouseCursor.cs
/// Author:					DarkRabbit
/// Create Time:			Wed, 05 Sep 2018 01:20:36 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

using UnityEngine;

namespace DR.Book.SRPG_Dev.Maps
{
    [AddComponentMenu("SRPG/Map Object/Map Mouse Cursor")]
    public class MapMouseCursor : MapCursor
    {
        public override MapObjectType mapObjectType
        {
            get { return MapObjectType.MouseCursor; }
        }

        public override CursorType cursorType
        {
            set
            {
                if (value != CursorType.Mouse)
                {
                    return;
                }
                base.cursorType = value;
            }
        }

        public override void OnSpawn()
        {
            base.OnSpawn();

            cursorType = CursorType.Mouse;
        }
    }
}