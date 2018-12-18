#region ---------- File Info ----------
/// **********************************************************************
/// Copyright (C) 2018 DarkRabbit(ZhangHan)
///
/// File Name:				Class.cs
/// Author:					DarkRabbit
/// Create Time:			Thu, 06 Sep 2018 08:32:14 GMT
/// Modifier:
/// Module Description:
/// Version:				V1.0.0
/// **********************************************************************
#endregion ---------- File Info ----------

namespace DR.Book.SRPG_Dev.Models
{
    public class Class
    {
        public ClassInfo info { get; private set; }

        public MoveConsumption moveConsumption
        {
            get
            {
                RoleModel model = ModelManager.models.Get<RoleModel>();
                return model.GetOrCreateMoveConsumption(info.classType);
            }
        }

        public Class(ClassInfo info)
        {
            this.info = info;
        }
    }
}