using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using L2Sql.DomainModel;
using L2Sql.DataAccessLayer;
using L2Sql.Dto;

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
        IList<CallSign> GetCallSignsFromLog(int Logid);
        CallSign GetCallSign(int CallSignId);

        void AddCabrilloInfo(params CabrilloInfo[] CabrilloInfos);
        CabrilloInfo GetCabrilloInfo(string ContestId, int CallSignId);


        IList<LogCategory> GetAllLogCategorys();
        void AddLogCategory(params LogCategory[] LogCategorys);

        Contest GetContest(string ContestId);

        void AddQso(params Qso[] Qsos);
        IList<Qso> GetQsos(int LogId);
        void UpdateQso(params Qso[] Qsos);
        IList<QsoAddPoinsMultsDTO> GetQsoPointsMults(int LogId);
        void UpdateQsoPointsMults(QsoUpdatePoinsMultsDTOCollextion QsoUpdatePoinsMultsDTOCollextion);

        void AddQsoExchangeNumber(params QsoExchangeNumber[] QsoExchangeNumbers);

    }
}
