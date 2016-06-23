using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using L2Sql.DomainModel;
using L2Sql.Dto;

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
        public IList<CallSign> GetCallSignsFromLog(int Logid)
        {
            using (var context = new ContestqsoDataEntities())
            {
                 IQueryable<Qso>QsoQuery = context.Set<Qso>().AsNoTracking();
                 IQueryable<CallSign> CallSignQuery = context.Set<CallSign>().AsNoTracking();
                 var Callsigns = (from lc in CallSignQuery
                                  join lq in QsoQuery on lc.CallSignId equals lq.CallsignId
                                 where lq.LogId == Logid
                                 select lc).Distinct().ToList();
                return Callsigns;
            }
        }

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
        public IList<QsoAddPoinsMultsDTO> GetQsoPointsMults(int Logid)
        {
            using (var context = new ContestqsoDataEntities())
            {
                IQueryable<Qso> QsoQuery = context.Set<Qso>().AsNoTracking();
                var Qsos = (from lc in QsoQuery
                            where lc.LogId == Logid
                            select new QsoAddPoinsMultsDTO
                            {
                                QsoNo = lc.QsoNo,
                                LogId = lc.LogId,
                                CallsignId = lc.CallsignId,
                                Frequency = lc.Frequency,
                                QsoExchangeNumber = lc.QsoExchangeNumber,
                                QsoModeTypeEnum = lc.QsoModeTypeEnum,
                            }).ToList();
                return Qsos;
            }
        }

        //TVP method for updating QsoPointsMults using stored Proc
        public void UpdateQsoPointsMults(QsoUpdatePoinsMultsDTOCollextion QsoUpdatePoinsMultsDTOCollextion)
        {
            //.Net will call QsoUpdatePoinsMultsDTOCollextion.GetEnumerator() for each record.
            using (var context = new ContestqsoDataEntities())
            {
                var sqp = new SqlParameter("sqp", SqlDbType.Structured);
                sqp.Value = QsoUpdatePoinsMultsDTOCollextion;
                sqp.TypeName = "udt_QsoPointsMults";

                string command = "EXEC " + "CQD_sp_QsoUpdatePointsMults" + " @sqp";
                context.Database.ExecuteSqlCommand(command, sqp);
            }
        }
    }
    public class QsoExchangeAlphaRepository : GenericDataRepository<QsoExchangeAlpha>, IQsoExchangeAlphaRepository
    {
    }
    public class QsoExchangeNumberRepository : GenericDataRepository<QsoExchangeNumber>, IQsoExchangeNumberRepository
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
