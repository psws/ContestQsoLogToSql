using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using L2Sql.DomainModel;
using L2Sql.Dto;
using Logqso.mvc.common.Enum;

namespace L2Sql.DataAccessLayer
{
    public interface ICabrilloInfoRepository : IGenericDataRepository<CabrilloInfo> { }
    public interface ICallInfoRepository : IGenericDataRepository<CallInfo> { }
    public interface ICallSignRepository : IGenericDataRepository<CallSign> {
         IList<CallSign> GetCallSignsFromLog(int Logid);
         IList<CallSign> GetBadCallSignsContainingString(string PartialCall);
    }
    public interface IContestRepository : IGenericDataRepository<Contest> { }
    public interface IContestTypeRepository : IGenericDataRepository<ContestType> { }
    public interface ILogRepository : IGenericDataRepository<Log> { }
    public interface ILogCategoryRepository : IGenericDataRepository<LogCategory> { }
    public interface IQsoRepository : IGenericDataRepository<Qso>
    {
        IList<QsoAddPoinsMultsDTO> GetQsoPointsMults(int LogId);
        int GetQsoPointsMultsCount(int Logid);
        void UpdateQsoPointsMults(QsoUpdatePoinsMultsDTOCollextion QsoUpdatePoinsMultsDTOCollextion);
        void AddQsoInsertContacts(QsoInsertContactsDTOCollextion QsoInsertContactsDTOCollextion);
        IList<Qso> GetQsoContacts(int Logid);
        int GetQsoCount(int Logid);
        IList<QsoWorkedLogDTO> GetWorkedQsosFromContestInWorkesLog(string ContestId, int LogCallsignId, int NotCallsignId);
        // get all Qsos, with callsignId in ContestId, within freq range in QSo table, with no includes
        void GetQsosFromLogWithFreqRange(string ContestId, int LogId, decimal FreqLow, decimal FreqHigh,
                     out IList<QsoBadNilContact> QsoInMyLogBand, bool bPass2);
        // gest all Qsos, with callsignId for Logs in Log table, within freq range 
        void GetBandCallsInMyLogWithNoSubmittedLog(string ContestId, int LogId, int CallSignId, decimal FreqLow, decimal FreqHigh,
            out IList<QsoBadNilContact> QsoNotInMyLog, out IList<QsoBadNilContact> QsoBadOrNotInMyLog,
            out  IList<QsoBadNilContact> QsoNotInMyLogIntersection, bool bCorrections);
        //get all my qsos from their logs
        void GetAllQsosFromCallsignWithFreqRange(string ContestId, int LogId, int CallsignId, decimal FreqLow, decimal FreqHigh,
                     out IList<QsoBadNilContact> QsoInThereLogBand, bool bPass2);
        // get logid dupes of my call in there logs, with callsignId in ContestId, within freq range in QSo table
        void GetDupeQsosFromCallsignWithFreqRange(string ContestId, int LogId, int CallsignId, decimal FreqLow, decimal FreqHigh,
                    out IList<IGrouping<int, QsoBadNilContact>> QsoDupesBand);
        //get dupes in my log
        void GetDupeQsosFromLogWithFreqRange(string ContestId, int LogId, decimal FreqLow, decimal FreqHigh,
                    ref IList<IGrouping<int, QsoBadNilContact>> QsoDupesMyLogBand);
        //bad xchgs all bansd my log
        void GetBadXchgQsosFromLog(string ContestId, ContestTypeEnum ContestTypeEnum, CatOperatorEnum CatOperatorEnum, CatNoOfTxEnum CatNoOfTxEnum,
            int LogId, int CallSignId, ref IList<QsoBadNilContact> BadXchgQsos);
        void GetWPXBadXchgQsosFromLog(string ContestId, ContestTypeEnum ContestTypeEnum, CatOperatorEnum CatOperatorEnum, CatNoOfTxEnum CatNoOfTxEnum,
            int LogId, int CallSignId, decimal FreqLow, decimal FreqHigh, ref IList<QsoBadNilContact> BadXchgQsos);
        void GetBadQsosNoCountry(string ContestId, int LogId, ref IList<QsoBadNilContact> BadQsosNoCountry);

        IList<QsoBadNilContact> GetBandUbnIncorrectQsosForCall(string ContestId, string Call, decimal FreqLow, decimal FreqHigh);
        IList<UbnIncorrectCall> GetBandUbnIncorrectCallsForLog(string ContestId, int LogId, decimal FreqLow, decimal FreqHigh);
        IList<QsoBadNilContact> GetBandSnippetSameCountryQsosForQsoWithFreqRange(string ContestId, QsoBadNilContact Qso, decimal FreqLow,
                    decimal FreqHigh, int DeltaMinutes, int HoleMinutes);
        IList<QsoBadNilContact> GetBandSnippetDifferentCountryQsosForQsoWithFreqRange(string ContestId, QsoBadNilContact Qso,
                    decimal FreqLow, decimal FreqHigh, int DeltaMinutes, int HoleMinutes);
        Qso GetQsoFromLog(int Logid, int QsoNo);
        void AdjustBadCallsignIds(string ContestId, int BadCallSignId, int UpdatedCallSignId);
    }
    public interface IQsoExchangeAlphaRepository : IGenericDataRepository<QsoExchangeAlpha> { }
    public interface IQsoExchangeNumberRepository : IGenericDataRepository<QsoExchangeNumber> { }
    
    public interface IQsoExchangeTypeRepository : IGenericDataRepository<QsoExchangeType> { }
    public interface IQsoExtraDataRepository : IGenericDataRepository<QsoExtraData> { }
    public interface IQsoModeTypeRepository : IGenericDataRepository<QsoModeType> { }
    public interface IQsoRadioTypeRepository : IGenericDataRepository<QsoRadioType> { }
    public interface ISessionRepository : IGenericDataRepository<Session> { }
    public interface IStationRepository : IGenericDataRepository<Station> { }
    public interface IUbnDupeRepository : IGenericDataRepository<UbnDupe> { }
    public interface IUbnIncorrectCallRepository : IGenericDataRepository<UbnIncorrectCall> {
        IList<UbnIncorrectCall> GetBandUbnIncorrectCalls(string ContestId, decimal FreqLow, decimal FreqHigh);

    }
    public interface IUbnIncorrectExchangeRepository : IGenericDataRepository<UbnIncorrectExchange> { }
    public interface IUbnNotInLogRepository : IGenericDataRepository<UbnNotInLog> {   
        IList<UbnNotInLog> GetBandUbnNotInLogs(string ContestId, decimal FreqLow, decimal FreqHigh);
    }
    public interface IUbnSummaryRepository : IGenericDataRepository<UbnSummary> { }
    public interface IUbnUniqueRepository : IGenericDataRepository<UbnUnique> {
        IList<short> GetUniquesFromContest(string ContestId, int LogId, ref IList<UbnUnique> UbnUniques);
        bool? CheckCallIsUniqueInQsos(string ContestId, int CallsignId, decimal FreqLow, decimal FreqHigh);
        void GetAllUniqueBandQsosFromLog(string ContestId, int LogId, ref IList<QsoBadNilContact> AllUniqueQsosFromLog
            , decimal FreqLow, decimal FreqHigh);
    }
}
