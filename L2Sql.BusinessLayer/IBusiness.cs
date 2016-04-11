﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using L2Sql.DomainModel;
using L2Sql.DataAccessLayer;

namespace L2Sql.BusinessLayer
{
    public interface IBusiness
    {
        void AddLog(params Log[] Logs);
        void UpdateLog(params Log[] Logs);
        void RemoveLog(params Log[] Logs);
        IList<Log> GetAllLogs(string ContestId);
        Log GetLog(string ContestId, int CallsignId);

         void AddCallSign(params CallSign[] CallSigns);
        void UpdateCallSign(params CallSign[] CallSigns);
        void RemoveCallSign(params CallSign[] CallSigns);
        IList<CallSign> GetAllCallsigns();

        void AddCabrilloInfo(params CabrilloInfo[] CabrilloInfos);
        CabrilloInfo GetCabrilloInfo(string ContestId, int CallSignId);


        IList<LogCategory> GetAllLogCategorys();
        void AddLogCategory(params LogCategory[] LogCategorys);

        void AddQso(params Qso[] Qsos);
        IList<Qso> GetQso(int LogId);
    }
}
