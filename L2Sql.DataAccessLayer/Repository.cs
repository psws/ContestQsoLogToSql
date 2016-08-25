using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using L2Sql.DomainModel;
using L2Sql.Dto;
using Logqso.mvc.common.Enum;

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
        public IList<Qso> GetQsoContacts(int Logid)
        {
            using (var context = new ContestqsoDataEntities())
            {
                IQueryable<Qso> QsoQuery = context.Set<Qso>().AsNoTracking();
                var Qsos = (from lc in QsoQuery
                            where lc.LogId == Logid
                            select lc).ToList();
                return Qsos;
            }
        }


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


        public IList<QsoWorkedLogDTO> GetWorkedQsosFromContestInWorkesLog(string ContestId, int LogCallsignId, int NotCallsignId)
        {
            using (var context = new ContestqsoDataEntities())
            {
                IQueryable<Qso> QsoQuery = context.Set<Qso>().AsNoTracking();
                IQueryable<Log> LogQuery = context.Set<Log>().AsNoTracking();
                IQueryable<QsoExchangeAlpha> QsoExchangeAlphaQuery = context.Set<QsoExchangeAlpha>().AsNoTracking();

                var QsoWorkedLogDTOs = (from lq in QsoQuery
                                        join ll in LogQuery on lq.LogId equals ll.LogId
                                        where ll.ContestId == ContestId &&
                                lq.CallsignId == LogCallsignId
                                        select new QsoWorkedLogDTO
                                        {
                                            LogId = ll.LogId,
                                            QsoDateTime = lq.QsoDateTime,
                                            Frequency = lq.Frequency,
                                            CallsignId = ll.CallsignId,
                                            QsoExchangeNumber = lq.QsoExchangeNumber,
                                            QsoExchangeAlpha = (from le in QsoExchangeAlphaQuery
                                                                where le.LogId == lq.LogId && le.QsoNo == lq.QsoNo
                                                                select le.QsoExhangeAlphaValue).FirstOrDefault(),
                                        }).ToList();
                return QsoWorkedLogDTOs;
            }
        }


        //TVP method for updating QsoInsertContactsDTO using stored Proc
        public void AddQsoInsertContacts(QsoInsertContactsDTOCollextion QsoInsertContactsDTOCollextion)
        {
            //.Net will call QsoInsertContactsDTOCollextion.GetEnumerator() for each record.
            using (var context = new ContestqsoDataEntities())
            {
                var sqp = new SqlParameter("sqp", SqlDbType.Structured);
                sqp.Value = QsoInsertContactsDTOCollextion;
                sqp.TypeName = "udt_QsoContacts";

                string command = "EXEC " + "CQD_sp_QsoInsertContacts" + " @sqp";
                context.Database.ExecuteSqlCommand(command, sqp);
            }
        }

        public void GetQsosFromLogWithFreqRange(string ContestId, int LogId, decimal FreqLow, decimal FreqHigh,
                    out IList<QsoBadNilContact> QsoInMyLogBand)
        {
            using (var context = new ContestqsoDataEntities())
            {
                IQueryable<Qso> QsoQuery = context.Set<Qso>().AsNoTracking();
                IQueryable<CallSign> CallsignQuery = context.Set<CallSign>().AsNoTracking();
                QsoInMyLogBand = (from lq in QsoQuery
                                      join lc in CallsignQuery on lq.CallsignId equals lc.CallSignId
                                      where lq.LogId == LogId
                                          && (lq.Frequency >= FreqLow && lq.Frequency <= FreqHigh)
                                      select new QsoBadNilContact
                                      {
                                          CallsignId = lq.CallsignId,
                                          Call = lc.Call,
                                          Frequency = lq.Frequency,
                                          QsoDateTime = lq.QsoDateTime,
                                          QsoExchangeNumber = lq.QsoExchangeNumber,
                                          QsoNo = lq.QsoNo,
                                          LogId = 0

                                      })
                              .ToList();

                
            }
        }

        public void GetAllQsosFromCallsignWithFreqRange(string ContestId, int LogId, int CallsignId, decimal FreqLow, decimal FreqHigh,
                    out IList<QsoBadNilContact> QsoInThereLogBand)
        {
            using (var context = new ContestqsoDataEntities())
            {
                IQueryable<Qso> QsoQuery = context.Set<Qso>().AsNoTracking();
                IQueryable<Log> LogQuery = context.Set<Log>().AsNoTracking();
                IQueryable<CallSign> CallsignQuery = context.Set<CallSign>().AsNoTracking();

                //All Qsos with submitted logs
                QsoInThereLogBand = (from lq in QsoQuery
                                join ll in LogQuery on lq.LogId equals ll.LogId
                                join lc in CallsignQuery on lq.CallsignId equals lc.CallSignId
                                where ll.ContestId == ContestId && lq.CallsignId == CallsignId
                                    && ll.LogId != LogId
                                    && (lq.Frequency >= FreqLow && lq.Frequency <= FreqHigh)
                                select new QsoBadNilContact
                                {
                                    CallsignId = ll.CallsignId,
                                    Call = (from cl in CallsignQuery
                                            where cl.CallSignId == ll.CallsignId
                                            select cl.Call).FirstOrDefault(),
                                    Frequency = lq.Frequency,
                                    QsoDateTime = lq.QsoDateTime,
                                    QsoExchangeNumber = ll.QsoExchangeNumber,
                                    QsoNo = lq.QsoNo,
                                    LogId = ll.LogId
                                }).OrderBy(x => x.QsoDateTime).ToList();
            }
      }


        public void GetDupeQsosFromCallsignWithFreqRange(string ContestId, int LogId, int CallsignId, decimal FreqLow, decimal FreqHigh,
                    out IList<IGrouping<int, QsoBadNilContact>> QsoDupesBand)
        {
            //dupes inlude my call QSOs
            using (var context = new ContestqsoDataEntities())
            {
                IQueryable<Qso> QsoQuery = context.Set<Qso>().AsNoTracking();
                IQueryable<Log> LogQuery = context.Set<Log>().AsNoTracking();
                IQueryable<CallSign> CallsignQuery = context.Set<CallSign>().AsNoTracking();

                //All Qsos with submitted logs
                var AllQsoWithLogsFromLog = (from lq in QsoQuery
                                             join ll in LogQuery on lq.LogId equals ll.LogId
                                             join lc in CallsignQuery on lq.CallsignId equals lc.CallSignId
                                             where ll.ContestId == ContestId && lq.CallsignId == CallsignId
                                                 && ll.LogId != LogId
                                                 && (lq.Frequency >= FreqLow && lq.Frequency <= FreqHigh)
                                             select new QsoBadNilContact
                                             {
                                                 CallsignId = ll.CallsignId,
                                                 Call = (from cl in CallsignQuery
                                                         where cl.CallSignId == ll.CallsignId
                                                         select cl.Call).FirstOrDefault(),
                                                 Frequency = lq.Frequency,
                                                 QsoDateTime = lq.QsoDateTime,
                                                 QsoExchangeNumber = ll.QsoExchangeNumber,
                                                 QsoNo = lq.QsoNo,
                                                 LogId = ll.LogId
                                             }).OrderBy(x => x.QsoDateTime)
                                             .AsQueryable();
                QsoDupesBand = AllQsoWithLogsFromLog.GroupBy(x => x.CallsignId).Where(g => g.Count() > 1).ToList();
            }
        }

        public void GetDupeQsosFromLogWithFreqRange(string ContestId, int LogId, decimal FreqLow, decimal FreqHigh,
                    ref IList<IGrouping<int, QsoBadNilContact>> QsoDupesBand)
        {
            //dupes inlude my call QSOs
            using (var context = new ContestqsoDataEntities())
            {
                IQueryable<Qso> QsoQuery = context.Set<Qso>().AsNoTracking();
                IQueryable<Log> LogQuery = context.Set<Log>().AsNoTracking();
                IQueryable<CallSign> CallsignQuery = context.Set<CallSign>().AsNoTracking();

                //All Qsos with submitted logs
                var AllQsoWithLogsFromLog = (from lq in QsoQuery
                                             join ll in LogQuery on lq.LogId equals ll.LogId
                                             join lc in CallsignQuery on lq.CallsignId equals lc.CallSignId
                                             where ll.ContestId == ContestId && lq.LogId == LogId
                                                 && (lq.Frequency >= FreqLow && lq.Frequency <= FreqHigh)
                                             select new QsoBadNilContact
                                             {
                                                 CallsignId = lq.CallsignId,
                                                 Call = (from cl in CallsignQuery
                                                         where cl.CallSignId == lq.CallsignId
                                                         select cl.Call).FirstOrDefault(),
                                                 Frequency = lq.Frequency,
                                                 QsoDateTime = lq.QsoDateTime,
                                                 QsoExchangeNumber = ll.QsoExchangeNumber,
                                                 QsoNo = lq.QsoNo,
                                                 LogId = ll.LogId
                                             }).OrderBy(x=>x.QsoDateTime)
                                             .AsQueryable();
                QsoDupesBand = AllQsoWithLogsFromLog.GroupBy(x => x.CallsignId).Where(g => g.Count() > 1).ToList();
            }
        }



        public void GetBadXchgQsosFromLog(string ContestId, ContestTypeEnum ContestTypeEnum, CatOperatorEnum CatOperatorEnum, int LogId,
                    ref IList<QsoBadNilContact> BadXchgQsos)
        {
            using (var context = new ContestqsoDataEntities())
            {
                IQueryable<Qso> QsoQuery = context.Set<Qso>().AsNoTracking();
                IQueryable<Log> LogQuery = context.Set<Log>().AsNoTracking();
                IQueryable<CallSign> CallsignQuery = context.Set<CallSign>().AsNoTracking();

                if (ContestTypeEnum == Logqso.mvc.common.Enum.ContestTypeEnum.CQWPX && CatOperatorEnum == Logqso.mvc.common.Enum.CatOperatorEnum.MULTI_OP)
                {
                    IQueryable<QsoExchangeNumber> QsoExchangeNumberQuery = context.Set<QsoExchangeNumber>().AsNoTracking();
                    //All Qsos with submitted logs
                    BadXchgQsos = (from lq in QsoQuery
                                   join ll in LogQuery on lq.CallsignId equals ll.CallsignId
                                   join lc in CallsignQuery on lq.CallsignId equals lc.CallSignId
                                   join xc in QsoExchangeNumberQuery on lq.LogId equals xc.LogId
                                   where ll.ContestId == ContestId && lq.LogId == LogId
                                       && lq.QsoNo == xc.QsoNo
                                       && xc.QsoExhangeNumberValue != ll.QsoExchangeNumber
                                   select new QsoBadNilContact
                                   {
                                       CallsignId = lq.CallsignId,
                                       Call = (from cl in CallsignQuery
                                               where cl.CallSignId == lq.CallsignId
                                               select cl.Call).FirstOrDefault(),
                                       Frequency = lq.Frequency,
                                       QsoDateTime = lq.QsoDateTime,
                                       QsoExchangeNumber = ll.QsoExchangeNumber, //corected xchng
                                       //QsoExchangeNumber = (from xc in QsoExchangeNumberQuery
                                       //                     where xc.LogId == LogId &&
                                       //                     xc.QsoNo == lq.QsoNo
                                       //                     select xc.QsoExhangeNumberValue).FirstOrDefault(),
                                       QsoNo = lq.QsoNo,
                                       LogId = ll.LogId
                                   }).OrderBy(x => x.QsoDateTime)
                                                 .ToList();
                }
                else 
                {
                    //All Qsos with submitted logs
                    BadXchgQsos = (from lq in QsoQuery
                                   join ll in LogQuery on lq.CallsignId equals ll.CallsignId
                                   join lc in CallsignQuery on lq.CallsignId equals lc.CallSignId
                                   where ll.ContestId == ContestId && lq.LogId == LogId
                                       && lq.QsoExchangeNumber != ll.QsoExchangeNumber
                                   select new QsoBadNilContact
                                   {
                                       CallsignId = lq.CallsignId,
                                       Call = (from cl in CallsignQuery
                                               where cl.CallSignId == lq.CallsignId
                                               select cl.Call).FirstOrDefault(),
                                       Frequency = lq.Frequency,
                                       QsoDateTime = lq.QsoDateTime,
                                       QsoExchangeNumber = ll.QsoExchangeNumber,
                                       QsoNo = lq.QsoNo,
                                       LogId = ll.LogId
                                   }).OrderBy(x => x.QsoDateTime)
                                                 .ToList();
                }
            }
        }



        public void GetBadQsosNoCountry(string ContestId, int LogId, ref IList<QsoBadNilContact> BadQsosNoCountry)
        {
            using (var context = new ContestqsoDataEntities())
            {
                IQueryable<Qso> QsoQuery = context.Set<Qso>().AsNoTracking();
                IQueryable<CallSign> CallsignQuery = context.Set<CallSign>().AsNoTracking();

                BadQsosNoCountry = (from lc in CallsignQuery
                                   join lq in QsoQuery on lc.CallSignId equals lq.CallsignId
                                   where lc.Prefix == "none" && lq.LogId == LogId
                                   select new QsoBadNilContact
                                   {
                                       CallsignId = lq.CallsignId,
                                       Call = lc.Call,
                                       Frequency = lq.Frequency,
                                       QsoDateTime = lq.QsoDateTime,
                                       QsoExchangeNumber = lq.QsoExchangeNumber,
                                       QsoNo = lq.QsoNo,
                                       LogId = LogId
                                   }).ToList();
            }

        }

        public void GetBandCallsInMyLogWithNoSubmittedLog(string ContestId, int LogId, int CallSignId, decimal FreqLow, decimal FreqHigh,
                        out IList<QsoBadNilContact> QsoNotInMyLog, out IList<QsoBadNilContact>  QsoBadOrNotInMyLog)
        {
            //the return value represents all QSOs in my log that have not been verified with All contest Qso with mycall in the Qso table.
            //This returned list represents the potential NIls and Bad Qsos in my log.
            /*SELECT
              y.CallsignId,x.CallsignId, y.Call, y.QsoDateTime
	            from 
              (select l.callsignid, l.LogId
			            from Qso q
			             INNER JOIN Log l on q.LogId = l.LogId
			             where l.ContestId = 'CQWWSSB2015'  and q.CallsignId = 1 and l.LogId <> 14001
			             and q.Frequency between 14000 and 14350) as x
			             right join
			            ( select q.CallsignId,c.Call, q.QsoDateTime
			            from Qso q 
			             --INNER JOIN Log l on l.CallsignId = q.CallsignId
			             INNER JOIN Callsign c on q.CallsignId = c.CallsignId
			            where q.LogId = 14001 and q.Frequency between 14000 and 14350) as y
			
			            on x.CallsignId = y.CallsignId
			             where x.LogId  IS NULL
			             order by y.QsoDateTime */

            using (var context = new ContestqsoDataEntities())
            {
                IQueryable<Qso> QsoQuery = context.Set<Qso>().AsNoTracking();
                IQueryable<Log> LogQuery = context.Set<Log>().AsNoTracking();
                IQueryable<CallSign> CallsignQuery = context.Set<CallSign>().AsNoTracking();

                //All Qsos in table with my call(via submitted logs) excluding mt QSOs
                var AllQsoWithLogsFromLog = (from lq in QsoQuery
                                             join ll in LogQuery on lq.LogId equals ll.LogId
                                             join lc in CallsignQuery on lq.CallsignId equals lc.CallSignId
                                             where ll.ContestId == ContestId && lq.CallsignId == CallSignId
                                                 && ll.LogId != LogId
                                                 && (lq.Frequency >= FreqLow && lq.Frequency <= FreqHigh)
                                             select new QsoBadNilContact
                                             {
                                                 CallsignId = ll.CallsignId,
                                                 Call = (from cl in CallsignQuery
                                                         where cl.CallSignId == ll.CallsignId
                                                         select cl.Call).FirstOrDefault(),
                                                Frequency = lq.Frequency,
                                                QsoDateTime = lq.QsoDateTime,
                                                QsoExchangeNumber = ll.QsoExchangeNumber, 
                                                QsoNo = lq.QsoNo,
                                                LogId = ll.LogId
                                             })
                                             .AsEnumerable();
                //var AllQsoWithLogsFromLog2 = AllQsoWithLogsFromLog.Where(x => x.Call == "CN2R").ToList();
                //All Qsos with my call
                var AllQsosFromLog = (from lq in QsoQuery
                                      join lc in CallsignQuery on lq.CallsignId equals lc.CallSignId
                                      where lq.LogId == LogId
                                          && (lq.Frequency >= FreqLow && lq.Frequency <= FreqHigh)
                                      select new QsoBadNilContact
                                      {
                                          CallsignId = lq.CallsignId,
                                          Call = lc.Call,
                                          Frequency = lq.Frequency,
                                          QsoDateTime = lq.QsoDateTime,
                                          QsoExchangeNumber = lq.QsoExchangeNumber,
                                          QsoNo = lq.QsoNo,
                                          LogId = 0

                                      })
                                      .AsEnumerable();
                //var AllQsosFromLog2 = AllQsosFromLog.Where(x => x.Call == "CN2R").ToList();

                //Left Exclude join  CQD_GetNotInMyLog.sql
                //QsoNotInMyLog = AllQsoWithLogsFromLog.Except(AllQsosFromLog, (x, y) => x.Call == y.Call).OrderBy(x => x.Call).ToList();
                //Left Exclude join  CQD_GetNotInMyLog.sql
                //we need time because K4LR could have worked us more then once on a band.
                //We nned to catch the K3LR QSO that is not in myLog, since my log has only one K3LR band Qso. K3RL is a bad call in my log
                QsoNotInMyLog = AllQsoWithLogsFromLog.Except(AllQsosFromLog, (x, y) => x.Call == y.Call &&
                    (x.QsoDateTime >= y.QsoDateTime.AddMinutes(-3) && x.QsoDateTime <= y.QsoDateTime.AddMinutes(3))).OrderBy(x => x.Call).ToList();
                //Right Exclude join  CQD_GetNoLogsWithBadCallsJoin.sql
                QsoBadOrNotInMyLog = AllQsosFromLog.Except(AllQsoWithLogsFromLog, (x, y) => x.CallsignId == y.CallsignId).OrderBy(x => x.Call).ToList();

                //var QsoNotInMyLog2 = QsoNotInMyLog.Where(x => x.Call == "CN2R").ToList();
                //var QsoBadOrNotInMyLog2 = QsoBadOrNotInMyLog.Where(x => x.Call == "CN2R").ToList();
          
                
            }


            return;
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

        public IList<UbnNotInLog> GetBandUbnNotInLogs(int LogId, decimal FreqLow, decimal FreqHigh)
        {
            List<UbnNotInLog> UbnNotInLogs = null;
            using (var context = new ContestqsoDataEntities())
            {
                IQueryable<UbnNotInLog> UbnNotInLogQuery = context.Set<UbnNotInLog>().AsNoTracking();
                IQueryable<Qso> QsoQuery = context.Set<Qso>().AsNoTracking();

                UbnNotInLogs = (from Nil in UbnNotInLogQuery
                                join qq in QsoQuery on Nil.QsoNo equals (qq.QsoNo)
                                where Nil.LogId == qq.LogId &&
                                    (qq.Frequency >= FreqLow && qq.Frequency <= FreqHigh) 
                                select Nil 
                                ).OrderBy(x=>x.QsoNo).ToList();

            }

            return UbnNotInLogs;
        }

    }
    public class UbnSummaryRepository : GenericDataRepository<UbnSummary>, IUbnSummaryRepository
    {
    }
    public class UbnUniqueRepository : GenericDataRepository<UbnUnique>, IUbnUniqueRepository
    {
        public IList<short> GetUniquesFromContest(string ContestId, int LogId, ref IList<UbnUnique> UbnUniques)
        {
            //http://stackoverflow.com/questions/1406962/entity-framework-t-sql-having-equivalent 
            List<short> QsoNos = null;
            using (var context = new ContestqsoDataEntities())
            {
                IQueryable<Qso> QsoQuery = context.Set<Qso>().AsNoTracking();
                IQueryable<Log> LogQuery = context.Set<Log>().AsNoTracking();

                try
                {
                    QsoNos = (from lqq in QsoQuery
                              join ly in LogQuery on lqq.LogId equals ly.LogId
                              where ly.ContestId == ContestId &&
                                ((from lq in QsoQuery  //all unique callsignid in ContestId
                                  join ll in LogQuery on lq.LogId equals ll.LogId
                                  where ll.ContestId == ContestId
                                  group lq.QsoNo by lq.CallsignId into grp
                                  where grp.Count() == 1
                                  select grp.Key)

                                                .Intersect

                                    (from lq in QsoQuery //all uniques CallsignId in logid
                                     join ll in LogQuery on lq.LogId equals ll.LogId
                                     where lq.LogId == LogId
                                     group lq.QsoNo by lq.CallsignId into grp
                                     where grp.Count() == 1
                                     select grp.Key))
                                .Contains(lqq.CallsignId)
                              select lqq.QsoNo)
                                        .ToList();
                }
                catch (Exception ex)
                {

                    //throw;
                }


            }
            return QsoNos;

        }


        public bool? CheckCallIsUniqueInQsos(string ContestId, int CallsignId)
        {
            //http://stackoverflow.com/questions/1406962/entity-framework-t-sql-having-equivalent 
            bool? results = null;
            using (var context = new ContestqsoDataEntities())
            {
                IQueryable<Qso> QsoQuery = context.Set<Qso>().AsNoTracking();
                IQueryable<Log> LogQuery = context.Set<Log>().AsNoTracking();

                try
                {
                    var Qsos =  (from lq in QsoQuery  //all unique callsignid in ContestId
                                 join ll in LogQuery on lq.LogId equals ll.LogId
                                 where ll.ContestId == ContestId &&
                                    lq.CallsignId == CallsignId 
                                    select CallsignId ).Count();
                    if (Qsos == 1 )
                    {
                        results = true;
                    }
                    else if (Qsos > 1)
                    {
                        results = false;
                    }
                }
                catch (Exception ex)
                {

                    //throw;
                }
            }
            return results;
        }

    }

}
