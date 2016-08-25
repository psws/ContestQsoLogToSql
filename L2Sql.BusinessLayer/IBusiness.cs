using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using L2Sql.DomainModel;
using L2Sql.DataAccessLayer;
using L2Sql.Dto;
using Logqso.mvc.common.Enum;


namespace L2Sql.BusinessLayer
{
    public interface IBusiness
    {
        void AddLog(params Log[] Logs);
        void UpdateLog(params Log[] Logs);
        void RemoveLog(params Log[] Logs);
        IList<Log> GetAllLogs(string ContestId);
        IList<Log> GetAllLogsWithCallsign(string ContestId);
        Log GetLog(string ContestId, int CallsignId);

         void AddCallSign(params CallSign[] CallSigns);
        void UpdateCallSign(params CallSign[] CallSigns);
        void RemoveCallSign(params CallSign[] CallSigns);
        IList<CallSign> GetAllCallsigns();
        IList<CallSign> GetCallSignsFromLog(int Logid);
        CallSign GetCallSign(int CallSignId);

        void AddCabrilloInfo(params CabrilloInfo[] CabrilloInfos);
        CabrilloInfo GetCabrilloInfo(string ContestId, int CallSignId);

        LogCategory GetLogCategory(int LogCategoryId);
        IList<LogCategory> GetAllLogCategorys();
        void AddLogCategory(params LogCategory[] LogCategorys);

        Contest GetContest(string ContestId);

        void AddQso(params Qso[] Qsos);
        IList<Qso> GetQsoContacts(int Logid);
        IList<Qso> GetQsos(int LogId);
        void UpdateQso(params Qso[] Qsos);
        IList<QsoAddPoinsMultsDTO> GetQsoPointsMults(int LogId);
        void UpdateQsoPointsMults(QsoUpdatePoinsMultsDTOCollextion QsoUpdatePoinsMultsDTOCollextion);
        void AddQsoInsertContacts(QsoInsertContactsDTOCollextion QsoInsertContactsDTOCollextion);
        void AddQsoExchangeNumber(params QsoExchangeNumber[] QsoExchangeNumbers);

        void GetUniquesFromContest(string ContestId, int LogId, ref IList<UbnUnique> UbnUniques,
                ref  IList<UbnIncorrectCall> UbnIncorrectCalls, ref IList<UbnNotInLog> UbnNotInLogs, ref IList<UbnDupe> UbnDupes);
        void GetBadCallsNils(IList<Log> Logs, string ContestId, int LogId, int CallSignId,
                        ref IList<UbnIncorrectCall> UbnIncorrectCalls, ref IList<UbnNotInLog> UbnNotInLogs, ref IList<UbnDupe> UbnDupes);
        void GetBadQsosNoCountry(string ContestId, int LogId, ref IList<UbnIncorrectCall> UbnIncorrectCalls);
        void GetDupesFromMyLog(string ContestId, int LogId,
                        ref IList<UbnIncorrectCall> UbnIncorrectCalls, ref IList<UbnNotInLog> UbnNotInLogs, ref IList<UbnDupe> UbnDupes);
        void GetBadXchgsFromMyLog(string ContestId, ContestTypeEnum ContestTypeEnum, CatOperatorEnum CatOperatorEnum, int LogId,
            ref IList<UbnIncorrectCall> UbnIncorrectCalls, ref IList<UbnNotInLog> UbnNotInLogs,
            ref IList<UbnDupe> UbnDupes, ref IList<UbnIncorrectExchange> UbnIncorrectExchanges);

        void UpdateIncorrectCallsFromContest(IList<UbnIncorrectCall> UbnIncorrectCalls);
        void UpdateIncorrectExchangesFromContest(IList<UbnIncorrectExchange> UbnIncorrectExchanges);
        void UpdateDupesFromContest(IList<UbnDupe> UbnDupes);
        void UpdateNilsFromContest(IList<UbnNotInLog> UbnNotInLogs);

        void UpdateUniquesFromContest(IList<UbnUnique> UbnUniques);

        IList<UbnIncorrectCall> GetUbnIncorrectCalls(int LogId);
        IList<UbnIncorrectExchange> GetUbnIncorrectExchanges(int LogId);
        IList<UbnNotInLog> GetUbnNotInLogs(int LogId);
        IList<UbnNotInLog> GetBandUbnNotInLogs(int LogId, decimal FreqLow, decimal FreqHigh );
        IList<UbnDupe> GetUbnDupes(int LogId);
        IList<UbnUnique> GetUbnUnique(int LogId);


    }
}
