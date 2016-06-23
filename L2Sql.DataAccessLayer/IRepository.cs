using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using L2Sql.DomainModel;
using L2Sql.Dto;

namespace L2Sql.DataAccessLayer
{
    public interface ICabrilloInfoRepository : IGenericDataRepository<CabrilloInfo> { }
    public interface ICallInfoRepository : IGenericDataRepository<CallInfo> { }
    public interface ICallSignRepository : IGenericDataRepository<CallSign> {
         IList<CallSign> GetCallSignsFromLog(int Logid);
    }
    public interface IContestRepository : IGenericDataRepository<Contest> { }
    public interface IContestTypeRepository : IGenericDataRepository<ContestType> { }
    public interface ILogRepository : IGenericDataRepository<Log> { }
    public interface ILogCategoryRepository : IGenericDataRepository<LogCategory> { }
    public interface IQsoRepository : IGenericDataRepository<Qso> {
        IList<QsoAddPoinsMultsDTO> GetQsoPointsMults(int LogId);
        void UpdateQsoPointsMults(QsoUpdatePoinsMultsDTOCollextion QsoUpdatePoinsMultsDTOCollextion);
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
    public interface IUbnIncorrectCallRepository : IGenericDataRepository<UbnIncorrectCall> { }
    public interface IUbnIncorrectExchangeRepository : IGenericDataRepository<UbnIncorrectExchange> { }
    public interface IUbnNotInLogRepository : IGenericDataRepository<UbnNotInLog> { }
    public interface IUbnSummaryRepository : IGenericDataRepository<UbnSummary> { }
    public interface IUbnUniqueRepository : IGenericDataRepository<UbnUnique> { }
}
