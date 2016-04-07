using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using L2Sql.DomainModel;

namespace L2Sql.DataAccessLayer
{
    public class CabrilloInfoRepository : GenericDataRepository<CabrilloInfo>, ICabrilloInfoRepository
    {
    }
    public class CallInfoRepository : GenericDataRepository<CallInfo>, ICallInfoRepository
    {
    }
    public class CallSignRepository : GenericDataRepository<CallSign>, ICallSignRepository
    {
    }
    public class ContestRepository : GenericDataRepository<Contest>, IContestRepository
    {
    }
    public class ContestTypeRepository : GenericDataRepository<ContestType>, IContestTypeRepository
    {
    }
    public class LogRepository : GenericDataRepository<Log>, ILogRepository
    {
    }
    public class LogCategoryRepository : GenericDataRepository<LogCategory>, ILogCategoryRepository
    {
    }
    public class QsoRepository : GenericDataRepository<Qso>, IQsoRepository
    {
    }
    public class QsoExchangeAlphaepository : GenericDataRepository<QsoExchangeAlpha>, IQsoExchangeAlphaRepository
    {
    }
    public class QsoExchangeTypeRepository : GenericDataRepository<QsoExchangeType>, IQsoExchangeTypeRepository
    {
    }
    public class QsoExtraDataRepository : GenericDataRepository<QsoExtraData>, IQsoExtraDataRepository
    {
    }
    public class QsoModeTypeRepository : GenericDataRepository<QsoModeType>, IQsoModeTypeRepository
    {
    }
    public class QsoRadioTypeRepository : GenericDataRepository<QsoRadioType>, IQsoRadioTypeRepository
    {
    }
    public class SessionRepository : GenericDataRepository<Session>, ISessionRepository
    {
    }
    public class StationRepository : GenericDataRepository<Station>, IStationRepository
    {
    }
    public class UbnDupeRepository : GenericDataRepository<UbnDupe>, IUbnDupeRepository
    {
    }
    public class UbnIncorrectCallRepository : GenericDataRepository<UbnIncorrectCall>, IUbnIncorrectCallRepository
    {
    }
    public class UbnIncorrectExchangeRepository : GenericDataRepository<UbnIncorrectExchange>, IUbnIncorrectExchangeRepository
    {
    }
    public class UbnNotInLogRepository : GenericDataRepository<UbnNotInLog>, IUbnNotInLogRepository
    {
    }
    public class UbnSummaryRepository : GenericDataRepository<UbnSummary>, IUbnSummaryRepository
    {
    }
    public class UbnUniqueRepository : GenericDataRepository<UbnUnique>, IUbnUniqueRepository
    {
    }

}
