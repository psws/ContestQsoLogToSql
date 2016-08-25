using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using L2Sql.DomainModel;
using L2Sql.DataAccessLayer;
using L2Sql.Dto;
using Logqso.mvc.common;
using Logqso.mvc.common.Enum;

namespace L2Sql.BusinessLayer
{
    public class Business : IBusiness
    {

        private readonly ILogRepository ILogRepository;
        private readonly IQsoRepository IQsoRepository;
        private readonly ICallSignRepository ICallSignRepository;
        private readonly ILogCategoryRepository ILogCategoryRepository;
        private readonly ICabrilloInfoRepository ICabrilloInfoRepository;
        private readonly IContestRepository IContestRepository;
        private readonly IQsoExchangeNumberRepository IQsoExchangeNumberRepository;
        private readonly IUbnUniqueRepository IUbnUniqueRepository;
        private readonly IUbnNotInLogRepository IUbnNotInLogRepository;
        private readonly IUbnDupeRepository IUbnDupeRepository;
        private readonly IUbnIncorrectCallRepository IUbnIncorrectCallRepository;
        private readonly IUbnIncorrectExchangeRepository IUbnIncorrectExchangeRepository;

        private readonly int DeltaMinutesMatch = 3;

        public Business()
        {
            ILogRepository = new LogRepository();
            IQsoRepository = new QsoRepository();
            ICallSignRepository = new CallSignRepository();
            ILogCategoryRepository = new LogCategoryRepository();
            ICabrilloInfoRepository = new CabrilloInfoRepository();
            IContestRepository = new ContestRepository();
            IQsoExchangeNumberRepository = new QsoExchangeNumberRepository();
            IUbnUniqueRepository = new UbnUniqueRepository();
            IUbnNotInLogRepository = new UbnNotInLogRepository();
            IUbnDupeRepository = new UbnDupeRepository();
            IUbnIncorrectCallRepository = new UbnIncorrectCallRepository();
            IUbnIncorrectExchangeRepository = new UbnIncorrectExchangeRepository();


        }

        public Business(ILogRepository LogRepository,
            IQsoRepository QsoRepository)
        {
            this.ILogRepository = LogRepository;
            this.IQsoRepository = QsoRepository;
        }

        public IList<Log> GetAllLogs(string ContestId)
        {
            return ILogRepository.GetList(d => d.ContestId.Equals(ContestId),
                d => d.CallSign, d => d.LogCategory); //include related 
        }

        public IList<Log> GetAllLogsWithCallsign(string ContestId)
        {
            return ILogRepository.GetList(d => d.ContestId.Equals(ContestId),
                d => d.CallSign); //include related 
        }

        public Log GetLog(string ContestId, int CallsignId)
        {
            return ILogRepository.GetSingle(d => d.ContestId.Equals(ContestId) &&
            d.CallsignId == CallsignId,
                d => d.CallSign); //include related 
        }


        public void AddLog(params Log[] Logs)
        {
            /* Validation and error handling omitted */
            ILogRepository.Add(Logs);
        }

        public void UpdateLog(params Log[] Logs)
        {
            /* Validation and error handling omitted */
            ILogRepository.Update(Logs);
        }

        public void RemoveLog(params Log[] Logs)
        {
            /* Validation and error handling omitted */
            ILogRepository.Remove(Logs);
        }


        //CallSigns
        public CallSign GetCallSign(int CallSignId)
        {
            return ICallSignRepository.GetSingle(d => d.CallSignId == CallSignId);
        }


        public IList<CallSign> GetAllCallsigns()
        {
            return ICallSignRepository.GetAll();
        }

        public void AddCallSign(params CallSign[] CallSigns)
        {
            /* Validation and error handling omitted */
            try
            {
                ICallSignRepository.AddRange(CallSigns);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public IList<CallSign> GetCallSignsFromLog(int Logid)
        {

            var Callsigns = ICallSignRepository.GetCallSignsFromLog(Logid);
            return Callsigns;
        }


        public void RemoveCallSign(params CallSign[] CallSigns)
        {
            /* Validation and error handling omitted */
            ICallSignRepository.Remove(CallSigns);
        }


        public void UpdateCallSign(params CallSign[] CallSigns)
        {
            /* Validation and error handling omitted */
            ICallSignRepository.Update(CallSigns);
        }


        public void AddQso(params Qso[] Qsos)
        {
            /* Validation and error handling omitted */
            IQsoRepository.AddRange(Qsos);
        }

        public IList<QsoAddPoinsMultsDTO> GetQsoPointsMults(int LogId)
        {
            return IQsoRepository.GetQsoPointsMults(LogId);
        }

        public void UpdateQsoPointsMults(QsoUpdatePoinsMultsDTOCollextion QsoUpdatePoinsMultsDTOCollextion)
        {
            if (QsoUpdatePoinsMultsDTOCollextion.Count > 0)
            {
                IQsoRepository.UpdateQsoPointsMults(QsoUpdatePoinsMultsDTOCollextion);
                //d => d.CallSign); //include related 
            }
        }


        public void AddQsoInsertContacts(QsoInsertContactsDTOCollextion QsoInsertContactsDTOCollextion)
        {
            IQsoRepository.AddQsoInsertContacts(QsoInsertContactsDTOCollextion);
            //d => d.CallSign); //include related 
        }

        
        public void GetBadCallsNils(IList<Log> Logs, string ContestId, int LogId, int CallSignId,
                        ref IList<UbnIncorrectCall> UbnIncorrectCalls, ref IList<UbnNotInLog> UbnNotInLogs, ref IList<UbnDupe> UbnDupes)

        {
            IList<QsoBadNilContact> QsoNotInMyLog = null;
            IList<QsoBadNilContact> QsoBadOrNotInMyLog = null;
            IList<QsoBadNilContact> QsoInMyLogBand = null;
            IList<QsoBadNilContact> QsoInThereLogBand = null;

            //160M
            IQsoRepository.GetBandCallsInMyLogWithNoSubmittedLog(ContestId, LogId, CallSignId, 1800m, 2000m,
                out QsoNotInMyLog, out QsoBadOrNotInMyLog);
            IQsoRepository.GetQsosFromLogWithFreqRange(ContestId, LogId, 1800m, 1800m, out QsoInMyLogBand);
            IQsoRepository.GetAllQsosFromCallsignWithFreqRange(ContestId, LogId, CallSignId, 1800m, 2000m, out QsoInThereLogBand);
            ProcessNilsDupesAndBads(Logs, ContestId, LogId, ref UbnIncorrectCalls, ref UbnNotInLogs, ref UbnDupes,
                         QsoNotInMyLog, QsoBadOrNotInMyLog, QsoInMyLogBand, QsoInThereLogBand, 1800m, 2000m);
            //8M
            IQsoRepository.GetBandCallsInMyLogWithNoSubmittedLog(ContestId, LogId, CallSignId, 3500m, 4000m,
                out QsoNotInMyLog, out QsoBadOrNotInMyLog);
            IQsoRepository.GetQsosFromLogWithFreqRange(ContestId, LogId, 3500m, 4000m, out QsoInMyLogBand);
            IQsoRepository.GetAllQsosFromCallsignWithFreqRange(ContestId, LogId, CallSignId, 3500m, 4000m, out QsoInThereLogBand);
            ProcessNilsDupesAndBads(Logs, ContestId, LogId, ref UbnIncorrectCalls, ref UbnNotInLogs, ref UbnDupes,
                         QsoNotInMyLog, QsoBadOrNotInMyLog, QsoInMyLogBand, QsoInThereLogBand, 3500m, 4000m);
            //40M
            IQsoRepository.GetBandCallsInMyLogWithNoSubmittedLog(ContestId, LogId, CallSignId, 7000m, 7300m,
                out QsoNotInMyLog, out QsoBadOrNotInMyLog);
            IQsoRepository.GetQsosFromLogWithFreqRange(ContestId, LogId, 7000m, 7300m, out QsoInMyLogBand);
            IQsoRepository.GetAllQsosFromCallsignWithFreqRange(ContestId, LogId, CallSignId, 7000m, 7300m, out QsoInThereLogBand);
            ProcessNilsDupesAndBads(Logs, ContestId, LogId, ref UbnIncorrectCalls, ref UbnNotInLogs, ref UbnDupes,
                         QsoNotInMyLog, QsoBadOrNotInMyLog, QsoInMyLogBand, QsoInThereLogBand, 7000m, 7300m);
            //20M
            IQsoRepository.GetBandCallsInMyLogWithNoSubmittedLog(ContestId, LogId, CallSignId, 14000m, 14350m,
                out QsoNotInMyLog, out QsoBadOrNotInMyLog);
           IQsoRepository.GetQsosFromLogWithFreqRange(ContestId, LogId, 14000m, 14350m, out QsoInMyLogBand);
           IQsoRepository.GetAllQsosFromCallsignWithFreqRange(ContestId, LogId, CallSignId, 14000m, 14350m, out QsoInThereLogBand);

            //IQsoRepository.GetDupeQsosFromCallsignWithFreqRange(ContestId, LogId, CallSignId, 14000m, 14350m, out QsoDupesBand);
           ProcessNilsDupesAndBads(Logs, ContestId, LogId, ref UbnIncorrectCalls, ref UbnNotInLogs, ref UbnDupes,
                        QsoNotInMyLog, QsoBadOrNotInMyLog, QsoInMyLogBand, QsoInThereLogBand, 14000m, 14350m);
            //15M
            IQsoRepository.GetBandCallsInMyLogWithNoSubmittedLog(ContestId, LogId, CallSignId, 21000m, 21450m,
                out QsoNotInMyLog, out QsoBadOrNotInMyLog);
            IQsoRepository.GetQsosFromLogWithFreqRange(ContestId, LogId, 21000m, 21450m, out QsoInMyLogBand);
            IQsoRepository.GetAllQsosFromCallsignWithFreqRange(ContestId, LogId, CallSignId, 21000m, 21450m, out QsoInThereLogBand);
            ProcessNilsDupesAndBads(Logs, ContestId, LogId, ref UbnIncorrectCalls, ref UbnNotInLogs, ref UbnDupes, 
                         QsoNotInMyLog, QsoBadOrNotInMyLog, QsoInMyLogBand, QsoInThereLogBand, 21000m, 21450m);
            //10M
            IQsoRepository.GetBandCallsInMyLogWithNoSubmittedLog(ContestId, LogId, CallSignId, 28000m, 29700m,
                out QsoNotInMyLog, out QsoBadOrNotInMyLog);
            IQsoRepository.GetQsosFromLogWithFreqRange(ContestId, LogId, 28000m, 29700m, out QsoInMyLogBand);
            IQsoRepository.GetAllQsosFromCallsignWithFreqRange(ContestId, LogId, CallSignId, 28000m, 29700m, out QsoInThereLogBand);
            //var call = QsoNotInMyLog.Where(x => x.Call == "K3LR").ToList();
            //var call1 = QsoBadOrNotInMyLog.Where(x => x.Call == "K3LR").ToList();
            //var call2 = QsoInMyLogBand.Where(x => x.Call == "K3LR").ToList();
            //var call3 = QsoInThereLogBand.Where(x => x.Call == "K3LR").ToList();
            ProcessNilsDupesAndBads(Logs, ContestId, LogId, ref UbnIncorrectCalls, ref UbnNotInLogs, ref UbnDupes, 
                         QsoNotInMyLog, QsoBadOrNotInMyLog, QsoInMyLogBand, QsoInThereLogBand, 28000m, 29700m);



            return;
        }

        public void GetDupesFromMyLog(string ContestId, int LogId, 
                        ref IList<UbnIncorrectCall> UbnIncorrectCalls, ref IList<UbnNotInLog> UbnNotInLogs,
                        ref IList<UbnDupe> UbnDupes)
        {
            IList<IGrouping<int, QsoBadNilContact>> QsoDupesMyLogBand = null;
            //160M
            IQsoRepository.GetDupeQsosFromLogWithFreqRange(ContestId, LogId, 1800m, 2000m, ref  QsoDupesMyLogBand);
            ProcessDupes( LogId, QsoDupesMyLogBand, ref UbnIncorrectCalls, ref UbnNotInLogs,ref UbnDupes);
            //80M
            IQsoRepository.GetDupeQsosFromLogWithFreqRange(ContestId, LogId, 3500m, 4000m, ref  QsoDupesMyLogBand);
            ProcessDupes( LogId, QsoDupesMyLogBand, ref UbnIncorrectCalls, ref UbnNotInLogs,ref UbnDupes);
            //40M
            IQsoRepository.GetDupeQsosFromLogWithFreqRange(ContestId, LogId, 7000m, 7300m, ref  QsoDupesMyLogBand);
            ProcessDupes( LogId, QsoDupesMyLogBand, ref UbnIncorrectCalls, ref UbnNotInLogs,ref UbnDupes);
            //20M
            IQsoRepository.GetDupeQsosFromLogWithFreqRange(ContestId, LogId, 14000m, 14350m, ref  QsoDupesMyLogBand);
            ProcessDupes( LogId, QsoDupesMyLogBand, ref UbnIncorrectCalls, ref UbnNotInLogs,ref UbnDupes);
            //15M
            IQsoRepository.GetDupeQsosFromLogWithFreqRange(ContestId, LogId, 21000m, 21450m, ref  QsoDupesMyLogBand);
            ProcessDupes( LogId, QsoDupesMyLogBand, ref UbnIncorrectCalls, ref UbnNotInLogs,ref UbnDupes);
            //10M
            IQsoRepository.GetDupeQsosFromLogWithFreqRange(ContestId, LogId, 28000m, 29700m, ref  QsoDupesMyLogBand);
            ProcessDupes( LogId, QsoDupesMyLogBand, ref UbnIncorrectCalls, ref UbnNotInLogs,ref UbnDupes);

        }

        public IList<Qso> GetQsos(int LogId)
        {
            return IQsoRepository.GetList(d => d.LogId.Equals(LogId));
            //d => d.CallSign); //include related 
        }

        public void GetUniquesFromContest(string ContestId, int LogId, ref IList<UbnUnique> UbnUniques,
                ref  IList<UbnIncorrectCall> UbnIncorrectCalls, ref IList<UbnNotInLog> UbnNotInLogs, ref IList<UbnDupe> UbnDupes)
        {
             IList<short> UniqieQSONos = IUbnUniqueRepository.GetUniquesFromContest(ContestId, LogId, ref UbnUniques);
            //(UniqieCallsignIDs as List<int>).Sort();
             foreach (var qsoNo in UniqieQSONos)
            {
                if (UbnIncorrectCalls.Where(x => x.LogId == LogId && x.QsoNo == qsoNo).FirstOrDefault() != null)
                {//not a dupe, its nas Call
                    continue;
                }
                else if (UbnNotInLogs.Where(x => x.LogId == LogId && x.QsoNo == qsoNo).FirstOrDefault() != null)
                {//not a dupe its NIL
                    continue;
                }
                else if (UbnDupes.Where(x => x.LogId == LogId && x.QsoNo == qsoNo).FirstOrDefault() != null)
                {//Already a dupe
                    continue;
                }
                else
                {
                    var entry = UbnUniques.Where(x => x.LogId == LogId && x.QsoNo == qsoNo).FirstOrDefault();
                    if (entry == null)
                    {
                        //This is a Unique
                        UbnUnique UbnUnique = new UbnUnique()
                        {
                            LogId = LogId,
                            QsoNo = qsoNo,
                            EntityState = EntityState.Added
                        };
                        UbnUniques.Add(UbnUnique);
                    }
                }
            }
        }

        public void GetBadXchgsFromMyLog(string ContestId, ContestTypeEnum ContestTypeEnum, CatOperatorEnum CatOperatorEnum, int LogId,
            ref IList<UbnIncorrectCall> UbnIncorrectCalls, ref IList<UbnNotInLog> UbnNotInLogs,
            ref IList<UbnDupe> UbnDupes, ref IList<UbnIncorrectExchange> UbnIncorrectExchanges)
        {
            IList<QsoBadNilContact> BadXchgQsos = null;
            IQsoRepository.GetBadXchgQsosFromLog(ContestId, ContestTypeEnum,  CatOperatorEnum, LogId, ref BadXchgQsos);
            foreach (var qso in BadXchgQsos)
            { 
                if (UbnIncorrectCalls.Where(x => x.LogId == LogId && x.QsoNo == qso.QsoNo).FirstOrDefault() != null)
                {//not a dupe, its nas Call
                    continue;
                }
                else if (UbnNotInLogs.Where(x => x.LogId == LogId && x.QsoNo == qso.QsoNo).FirstOrDefault() != null)
                {//not a dupe its NIL
                    continue;
                }
                else if (UbnDupes.Where(x => x.LogId == LogId && x.QsoNo == qso.QsoNo).FirstOrDefault() != null)
                {//Already a dupe
                    continue;
                }
                else
                {
                    var entry = UbnIncorrectExchanges.Where(x => x.LogId == LogId && x.QsoNo == qso.QsoNo).FirstOrDefault();
                    if (entry == null)
                    {
                        //This is a bad xchng
                        UbnIncorrectExchange UbnIncorrectExchange = new UbnIncorrectExchange()
                        {
                            LogId = LogId, //mylog
                            QsoNo = qso.QsoNo,
                            CorrectExchange = qso.QsoExchangeNumber.ToString(),
                            EntityState = EntityState.Added
                        };
                        UbnIncorrectExchanges.Add(UbnIncorrectExchange);
                    }
                }
            }


        }

        public void GetBadQsosNoCountry(string ContestId, int LogId, ref IList<UbnIncorrectCall> UbnIncorrectCalls)
        {
            IList<QsoBadNilContact> BadXchgQsos = null;
            IQsoRepository.GetBadQsosNoCountry(ContestId, LogId, ref BadXchgQsos);

            foreach (var qso in BadXchgQsos)
            {
                if (UbnIncorrectCalls.Where(x => x.LogId == LogId && x.QsoNo == qso.QsoNo).FirstOrDefault() != null)
                {//allready bad
                    continue;
                }
                else
                {
                    var entry = UbnIncorrectCalls.Where(x => x.LogId == LogId && x.QsoNo == qso.QsoNo).FirstOrDefault();
                    if (entry == null)
                    {
                        //This is a bad call
                        UbnIncorrectCall UbnIncorrectCall = new UbnIncorrectCall()
                        {
                            LogId = LogId, //mylog
                            QsoNo = qso.QsoNo,
                            CorrectCall = string.Empty,
                            EntityState = EntityState.Added
                        };
                        UbnIncorrectCalls.Add(UbnIncorrectCall);
                    }
                }
            }

        }


        public void UpdateIncorrectCallsFromContest(IList<UbnIncorrectCall> UbnIncorrectCalls)
        {
            if (UbnIncorrectCalls.Count > 0)
            {
                IUbnIncorrectCallRepository.Add(UbnIncorrectCalls.ToArray());
            }
        }


        public void UpdateIncorrectExchangesFromContest(IList<UbnIncorrectExchange> UbnIncorrectExchanges)
        {
            if (UbnIncorrectExchanges.Count > 0)
            {
                IUbnIncorrectExchangeRepository.Add(UbnIncorrectExchanges.ToArray());
            }
        }


        public void UpdateNilsFromContest(IList<UbnNotInLog> UbnNotInLogs)
        {
            if (UbnNotInLogs.Count > 0)
            {
                IUbnNotInLogRepository.Add(UbnNotInLogs.ToArray());
            }
        }

        public void UpdateDupesFromContest(IList<UbnDupe> UbnDupes)
        {
            if (UbnDupes.Count > 0)
            {
                IUbnDupeRepository.Add(UbnDupes.ToArray());
            }
        }

        public void UpdateUniquesFromContest(IList<UbnUnique> UbnUniques)
        {
            if (UbnUniques.Count > 0)
            {
                IUbnUniqueRepository.Add(UbnUniques.ToArray());
            }
        }



        public IList<UbnIncorrectCall> GetUbnIncorrectCalls(int LogId)
        {
            return IUbnIncorrectCallRepository.GetList(d => d.LogId.Equals(LogId) ).OrderBy(x=>x.QsoNo).ToList();
        }

        public IList<UbnIncorrectExchange> GetUbnIncorrectExchanges(int LogId)
        {
            return IUbnIncorrectExchangeRepository.GetList(d => d.LogId.Equals(LogId)).OrderBy(x=>x.QsoNo).ToList();
        }

        public IList<UbnNotInLog> GetUbnNotInLogs(int LogId)
        {
            return IUbnNotInLogRepository.GetList(d => d.LogId.Equals(LogId)).OrderBy(x=>x.QsoNo).ToList();
        }

        public IList<UbnNotInLog> GetBandUbnNotInLogs(int LogId, decimal FreqLow, decimal FreqHigh)
        {
            return IUbnNotInLogRepository.GetBandUbnNotInLogs(LogId, FreqLow, FreqHigh);

        }

        public IList<UbnDupe> GetUbnDupes(int LogId)
        {
            return IUbnDupeRepository.GetList(d => d.LogId.Equals(LogId)).OrderBy(x=>x.QsoNo).ToList();
        }

        public IList<UbnUnique> GetUbnUnique(int LogId)
        {
            return IUbnUniqueRepository.GetList(d => d.LogId.Equals(LogId)).OrderBy(x=>x.QsoNo).ToList();
        }



        public IList<QsoWorkedLogDTO> GetWorkedQsosFromContestInWorkesLog(string ContestId, int CallsignId, int NotCallsignId)
        {
            return IQsoRepository.GetWorkedQsosFromContestInWorkesLog(ContestId, CallsignId, NotCallsignId);
        }


        public IList<Qso> GetQsoContacts(int LogId)
        {
            return IQsoRepository.GetQsoContacts(LogId);
        }

        public void UpdateQso(params Qso[] Qsos)
        {
            /* Validation and error handling omitted */
            IQsoRepository.Update(Qsos);
        }


        //LogCategory
        public LogCategory GetLogCategory(int LogCategoryId)
        {
            return ILogCategoryRepository.GetSingle(d => d.LogCategoryId.Equals(LogCategoryId));
        }


        public IList<LogCategory> GetAllLogCategorys()
        {
            return ILogCategoryRepository.GetAll();
        }

        public void AddLogCategory(params LogCategory[] LogCategorys)
        {
            /* Validation and error handling omitted */
            try
            {
                ILogCategoryRepository.AddRange(LogCategorys);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public Contest GetContest(string ContestId)
        {
            return IContestRepository.GetSingle(d => d.ContestId.Equals(ContestId));
        }

        public void AddCabrilloInfo(params CabrilloInfo[] CabrilloInfos)
        {
            /* Validation and error handling omitted */
            try
            {
                ICabrilloInfoRepository.AddRange(CabrilloInfos);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public CabrilloInfo GetCabrilloInfo(string ContestId, int CallSignId)
        {
            return ICabrilloInfoRepository.GetSingle(d => d.ContestId.Equals(ContestId) &&
            d.CallSignId == CallSignId,
                d => d.CallSign); //include related )
        }

        public void AddQsoExchangeNumber(params QsoExchangeNumber[] QsoExchangeNumbers)
        {
            /* Validation and error handling omitted */
            IQsoExchangeNumberRepository.AddRange(QsoExchangeNumbers);
        }

        private void ProcessNilsDupesAndBads(IList<Log> Logs, string ContestId, int LogId,
                 ref IList<UbnIncorrectCall> UbnIncorrectCalls, ref IList<UbnNotInLog> UbnNotInLogs, ref IList<UbnDupe> UbnDupes,
                 IList<QsoBadNilContact> QsoNotInMyLog, IList<QsoBadNilContact> QsoBadOrNotInMyLog, IList<QsoBadNilContact> QsoInMyLogBand,
                    IList<QsoBadNilContact> QsoInThereLogBand, decimal FreqLow, decimal FreqHigh)
        {

            IList<IGrouping<int, QsoBadNilContact>> QsoDupesBand;
            QsoDupesBand = QsoInThereLogBand.GroupBy(x => x.CallsignId).Where(g => g.Count() > 1).ToList();
            int criteria = 2; //default match criteria
            IList<UbnNotInLog> UbnNotInLogsCurrent = null;

            UbnNotInLogsCurrent = IUbnNotInLogRepository.GetBandUbnNotInLogs(LogId, FreqLow, FreqHigh);

            IList<QsoBadNilContact> LogSnippet;
            bool bBadCall = false;

            if (QsoNotInMyLog.Count() != 0 && QsoBadOrNotInMyLog.Count() != 0)
            {//Check qsos we have log into for
                //Next we need to check every call in QsoNotInMyLog against a +- 3 minute window of QsoBadOrNotInMyLog
                //  A QSO in QsoNotInMyLog is searched for a near matching qso in QsoBadOrNotInMyLog
                //      if a near match is found then the QsoBadOrNotInMyLog is marked as bad call.
                //          The station that worked this bad Qso looses no points.
                //          My log looses points.
                //      if no near match is found then the QSO in QsoNotInMyLog is marked as NIL.
                //          The Station that made this Qso looses points
                //
                //All logs are processed. When finished you have all NILS and Bads marked in DB
                foreach (var item in QsoNotInMyLog)
                {
                    //if (item.Call == "CN2R")
                    //{

                    //}
                    bBadCall = false;
                    //get +- 3 minute window from QsoBadOrNotInMyLog
                    LogSnippet = QsoBadOrNotInMyLog.Where(x => x.QsoDateTime == item.QsoDateTime).ToList();
                    foreach (var itemQso in LogSnippet)
                    {
                        // look for bad call
                        bool? PartialMatchResult = PartialMatch(ContestId, item.Call, itemQso.Call, itemQso.CallsignId, criteria);
                        if (PartialMatchResult == null)
                        {
                            bBadCall = true;  //stops searn, No NIL or BAd
                            break;
                        }
                        else if (PartialMatchResult == true)
                        {
                            var log = Logs.Where(x => x.CallsignId == itemQso.CallsignId).FirstOrDefault();
                            if (log != null)
                            {
                                var entry = UbnIncorrectCalls.Where(x => x.LogId == LogId && x.QsoNo == itemQso.QsoNo).FirstOrDefault();
                                if (entry == null)
                                {
                                    //Mark QsoBadOrNotInMyLog as bad call
                                    UbnIncorrectCall UbnIncorrectCall = new UbnIncorrectCall()
                                    {
                                        QsoNo = itemQso.QsoNo,
                                        LogId = LogId, //mylog
                                        CorrectCall = item.Call,
                                        EntityState = EntityState.Added
                                    };
                                    UbnIncorrectCalls.Add(UbnIncorrectCall);
                                }
                                bBadCall = true;
                                break;
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }
                    if (bBadCall == false)
                    { //+/- DeltaMinutesMatch snippet
                        LogSnippet = QsoBadOrNotInMyLog.Where(
                            (x => x.QsoDateTime >= item.QsoDateTime.AddMinutes(-DeltaMinutesMatch) && x.QsoDateTime < item.QsoDateTime
                            ||
                            (x.QsoDateTime > item.QsoDateTime && x.QsoDateTime <= item.QsoDateTime.AddMinutes(DeltaMinutesMatch)))
                            )
                            .ToList();
                        foreach (var itemQso in LogSnippet)
                        {
                            // look for bad call
                            bool? PartialMatchResult = PartialMatch(ContestId, item.Call, itemQso.Call, itemQso.CallsignId, criteria);
                            if (PartialMatchResult == null)
                            {
                                bBadCall = true;  //stops searn, No NIL or BAd
                                break;
                            }
                            else if (PartialMatchResult == true)
                            {
                                 //check if he has a log.  If we had his log 
                                var log = Logs.Where(x => x.CallsignId == itemQso.CallsignId).FirstOrDefault();
                                if (log != null)
                                {

                                    var entry = UbnIncorrectCalls.Where(x => x.LogId == LogId && x.QsoNo == itemQso.QsoNo).FirstOrDefault();
                                    if (entry == null)
                                    {
                                        //Mark QsoBadOrNotInMyLog as bad call
                                        UbnIncorrectCall UbnIncorrectCall = new UbnIncorrectCall()
                                        {
                                            QsoNo = itemQso.QsoNo,
                                            LogId = LogId, //mylog
                                            CorrectCall = item.Call,
                                            EntityState = EntityState.Added
                                        };
                                        UbnIncorrectCalls.Add(UbnIncorrectCall);
                                    }
                                    bBadCall = true;
                                    break;
                                }
                                else
                                { //No log so we cannot verify this partoal match. Continue Snippet search
                                    continue;
                                }
                            }
                        }

                    }
                    if (bBadCall == false)
                    {
                        //now check if Calls in my log that have no submitted logs match snippet from Qsos table Not in my log
                        //K3RL in mylog, K3LR has CN2R for this qso and K3LR has 2 CN2R calls
                        //Calls that are Bad can only have one letter or number mismatch
                        //The criteria is 1.
                        foreach (var itemRev in QsoBadOrNotInMyLog)
                        {
                            //if (itemRev.Call == "CN2R")
                            //{

                            //}
                            var bBadThereCall = false;
                            //get +- 3 minute window from QsoBadOrNotInMyLog
                            LogSnippet = QsoNotInMyLog.Where(x => x.QsoDateTime == itemRev.QsoDateTime).ToList();
                            foreach (var itemQsoRev in LogSnippet)
                            {
                                // look for bad call
                                bool? PartialMatchResult = PartialMatch(ContestId, itemRev.Call, itemQsoRev.Call, itemQsoRev.CallsignId, 1);
                                if (PartialMatchResult == null)
                                {
                                    bBadThereCall = true;  //stops searn, No NIL or BAd
                                    break;
                                }
                                else if (PartialMatchResult == true)
                                {
                                    //Now check if The match differs by 2 or more
                                    // A71GO in my log A71CV log has CN2R at exact freq and ...
                                    //2 chars are different.
                                    var entry = UbnIncorrectCalls.Where(x => x.LogId == LogId && x.QsoNo == itemRev.QsoNo).FirstOrDefault();
                                    
                                    if (entry == null)
                                    {
                                        //Mark QsoBadOrNotInMyLog as bad call
                                        UbnIncorrectCall UbnIncorrectCall = new UbnIncorrectCall()
                                        {
                                            QsoNo = itemRev.QsoNo,
                                            LogId = LogId, //mylog
                                            CorrectCall = itemQsoRev.Call,
                                            EntityState = EntityState.Added
                                        };
                                        UbnIncorrectCalls.Add(UbnIncorrectCall);
                                    }
                                    bBadThereCall = true;
                                    break;
                                }
                            }
                            if (bBadThereCall == false)
                            { //+/- DeltaMinutesMatch snippet
                                LogSnippet = QsoNotInMyLog.Where(
                                    (x => x.QsoDateTime >= itemRev.QsoDateTime.AddMinutes(-DeltaMinutesMatch) && x.QsoDateTime < itemRev.QsoDateTime
                                    ||
                                    (x.QsoDateTime > itemRev.QsoDateTime && x.QsoDateTime <= itemRev.QsoDateTime.AddMinutes(DeltaMinutesMatch)))
                                    )
                                    .ToList();
                                foreach (var itemQsoRev in LogSnippet)
                                {
                                    // look for bad call
                                    bool? PartialMatchResult = PartialMatch(ContestId, itemRev.Call, itemQsoRev.Call, itemQsoRev.CallsignId, 1);
                                    if (PartialMatchResult == null)
                                    {
                                        bBadThereCall = true;  //stops searcn, No NIL or BAd
                                        break;
                                    }
                                    else if (PartialMatchResult == true)
                                    {
                                        var entry = UbnIncorrectCalls.Where(x => x.LogId == LogId && x.QsoNo == itemRev.QsoNo).FirstOrDefault();
                                        if (entry == null)
                                        {
                                            //Mark QsoBadOrNotInMyLog as bad call
                                            UbnIncorrectCall UbnIncorrectCall = new UbnIncorrectCall()
                                            {
                                                QsoNo = itemRev.QsoNo,
                                                LogId = LogId, //mylog
                                                CorrectCall = itemQsoRev.Call,
                                                EntityState = EntityState.Added
                                            };
                                            UbnIncorrectCalls.Add(UbnIncorrectCall);
                                        }
                                        bBadThereCall = true;
                                        break;
                                    }
                                }
                            }
                            if (bBadThereCall == false)
                            { //not in log of snippet left, no matches found must be NIL, if they submitted a log
                                //check if he has a log
                                var log = Logs.Where(x => x.CallsignId == itemRev.CallsignId).FirstOrDefault();
                                if (log != null)
                                {
                                    var entry = UbnNotInLogsCurrent.Where(x => x.LogId == LogId && x.QsoNo == itemRev.QsoNo).FirstOrDefault();
                                    if (entry == null)
                                    {
                                        UbnNotInLog UbnNotInLog = new UbnNotInLog()
                                        {
                                            LogId = LogId,
                                            QsoNo = itemRev.QsoNo,
                                            EntityState = EntityState.Added
                                        };
                                        UbnNotInLogs.Add(UbnNotInLog);
                                        UbnNotInLogsCurrent.Add(UbnNotInLog);
                                    }
                                    bBadThereCall = true;
                                    break;
                                }
                            }
                        }
                    }
                    if (bBadCall == false)
                    {//not in their log
                         ////expand snippet search to +- 5 minutes
                        //LogSnippet = QsoInMyLogBand.Where(
                        //     (x => x.QsoDateTime >= item.QsoDateTime.AddMinutes(-5) && x.QsoDateTime < item.QsoDateTime
                        //     ||
                        //     (x.QsoDateTime > item.QsoDateTime && x.QsoDateTime <= item.QsoDateTime.AddMinutes(5)))
                        //     )
                        //     .ToList();
                        // var min5Call = LogSnippet.Where(x=>x.CallsignId == item.CallsignId).FirstOrDefault();
                        //case 1 mylog qso in their log, but outside of 3 minute window
                        //case  2 Therelog has me mor then once and it eas a differebt qso
                        //
                        //CRAZY:  If Band Call times do not match in my log and there log, 
                        //This may be a dupe candiadate. The situation arises when My log has a Q that does not appear in their lofg at the same datetime
                        //If mycall appears in their log at a later time, This 1st Q in my log is not a  NIL in my log.
                        //The Qso in their log may or may not be in mylog.  Maybe when the other log made the QSO with my log, my Op 
                        //did not log it cuz he thougth it was a dupe, There op logged the QSO.

                        //look for other  QSos later or earlier in the their logs
                        var AnyTimeCalL = QsoInMyLogBand.Where(x => x.CallsignId == item.CallsignId).FirstOrDefault();
                        if (AnyTimeCalL == null)
                        {//no call found in there logs => it is not a dupe candidate, it is anull
                            var entry = UbnNotInLogsCurrent.Where(x => x.LogId == item.LogId && x.QsoNo == item.QsoNo).FirstOrDefault();
                            if (entry == null)
                            {
                                UbnNotInLog UbnNotInLog = new UbnNotInLog()
                                {
                                    LogId = item.LogId,
                                    QsoNo = item.QsoNo,
                                    EntityState = EntityState.Added
                                };
                                UbnNotInLogs.Add(UbnNotInLog);
                                UbnNotInLogsCurrent.Add(UbnNotInLog);
                            }
                            bBadCall = true;
                        }
                    }
                }
            }
            //CHECK QSOs wth my log but no QSO log was submitted
            //The bad call may be a dupe call in there log and one good and one bad call in my log? 
            //Ik2CYW was worked once IMYL IK2YCW. worked CN2R twice IK2CYW needs to be checked against log snippet
            //We need to check the full logsnippet.
            //ie: my log k2VMS  and  KC2VMS exists.
            //Because the Log exists KC2VMS is not in the original log snippet.
            //We need to create a new LogSnippey from the full mylog.
            foreach (var dupe in QsoDupesBand)
	        { //group dupes
                //VE5PV worked me twice, My log has 1st Qso as VE5DX and second as VE5PV
                //need to check 1st call is in mylog
                //The skipprd call below must be in my log
                foreach (var qso1 in dupe)
                {
                    //if (qso1.Call == "CN2R")
                    //{

                    //}
                    bBadCall = false;
                    // one snippet check
                    LogSnippet = QsoInMyLogBand.Where(
                        (x => x.QsoDateTime >= qso1.QsoDateTime.AddMinutes(-DeltaMinutesMatch) &&
                            (x.QsoDateTime <= qso1.QsoDateTime.AddMinutes(DeltaMinutesMatch)))
                        ).ToList();
                    foreach (var itemQso in LogSnippet)
                    { //check against snippet collection
                        // look for bad call
                        bool? PartialMatchResult = PartialMatch(ContestId, qso1.Call, itemQso.Call, itemQso.CallsignId, criteria);
                        if (PartialMatchResult == true)
                        {
                            //UT8AL UT1AA off by 2 chars but UT1AA is valid dupe
                            //We need to search in there log  UT!AA for dupe Qsos
                            var FoundQsos = QsoInThereLogBand.Where(x => x.Call == itemQso.Call &&
                                (x.Frequency >= FreqLow && x.Frequency <= FreqHigh)).ToList();
                            foreach (var item in FoundQsos)
                            {
                                if (item.QsoDateTime >= item.QsoDateTime.AddMinutes(-DeltaMinutesMatch) &&
                                    item.QsoDateTime <= item.QsoDateTime.AddMinutes(DeltaMinutesMatch))
                                { //same delta time match for UT1AA
                                    var entry = UbnNotInLogsCurrent.Where(x => x.LogId == qso1.LogId && x.QsoNo == qso1.QsoNo).FirstOrDefault();
                //if (item.Call == "PJ2T"  && item.QsoNo == 198)
                //{

                //}
                                    if (entry == null)
                                     {
                                         UbnNotInLog UbnNotInLog = new UbnNotInLog()
                                         {
                                             LogId = qso1.LogId,
                                             QsoNo = qso1.QsoNo,
                                             EntityState = EntityState.Added
                                         };
                                         UbnNotInLogs.Add(UbnNotInLog);
                                         UbnNotInLogsCurrent.Add(UbnNotInLog);
                                     }
                                    bBadCall = true;
                                    break;
                                }
                            }
                            if (bBadCall == false)
                            { 
                                var entry = UbnIncorrectCalls.Where(x => x.LogId == LogId && x.QsoNo == itemQso.QsoNo).FirstOrDefault();
                                if (entry == null)
                                {
                                    UbnIncorrectCall UbnIncorrectCall = new UbnIncorrectCall()
                                    {
                                        QsoNo = itemQso.QsoNo,
                                        LogId = LogId, //mylog
                                        CorrectCall = qso1.Call,
                                        EntityState = EntityState.Added
                                    };
                                    UbnIncorrectCalls.Add(UbnIncorrectCall);
                                }
                                bBadCall = true;
                            }
                        }
                        if ( bBadCall == true)
                        {
                            break;
                        }
                    }
                    if (bBadCall == true && dupe.Count() == 2)
                    {//only 1 dupe and one of dupe was actually a  bad call
                        break;
                    }
                }
                if (bBadCall == true)
                {
                    continue;
                }

                foreach (var qso in dupe.Skip(1))
                { //each dupe
                    bBadCall = false;
                    // one snippet check
                    LogSnippet = QsoInMyLogBand.Where(
                        (x => x.QsoDateTime >= qso.QsoDateTime.AddMinutes(-DeltaMinutesMatch) &&
                            (x.QsoDateTime <= qso.QsoDateTime.AddMinutes(DeltaMinutesMatch)))
                        ).ToList();
                    // look for bad call
                    foreach (var itemQso in LogSnippet)
                    { //check against snippet collection
                        // look for bad call
                        bool? PartialMatchResult = PartialMatch(ContestId, qso.Call, itemQso.Call, itemQso.CallsignId, criteria);
                        if (PartialMatchResult == null)
                        {
                            bBadCall = true;  //stops searn, No NIL or BAd
                            //This is a dupe
                            var entry = UbnDupes.Where(x => x.LogId == LogId && x.QsoNo == itemQso.QsoNo).FirstOrDefault();
                            if (entry == null)
                            {
                                UbnDupe UbnDupe = new UbnDupe()
                                {
                                    LogId = LogId, //mylog
                                    QsoNo = itemQso.QsoNo,
                                    EntityState = EntityState.Added
                                };
                                UbnDupes.Add(UbnDupe);
                            }
                            break;
                        }
                        else if (PartialMatchResult == true)
                        {
                            //UT8AL UT1AA off by 2 chars but UT1AA is valid dupe
                            //We need to search in there log  UT!AA for dupe Qsos
                            var FoundQsos = QsoInThereLogBand.Where(x => x.Call == itemQso.Call &&
                                (x.Frequency >= FreqLow && x.Frequency <= FreqHigh)).ToList();
                            foreach (var item in FoundQsos)
                            {
                                if (item.QsoDateTime >= item.QsoDateTime.AddMinutes(-DeltaMinutesMatch) &&
                                    item.QsoDateTime <= item.QsoDateTime.AddMinutes(DeltaMinutesMatch))
                                { //same delta time match for UT1AA
                                    var entry = UbnNotInLogsCurrent.Where(x => x.LogId == qso.LogId && x.QsoNo == qso.QsoNo).FirstOrDefault();
                                    if (entry == null)
                                    {
                                        UbnNotInLog UbnNotInLog = new UbnNotInLog()
                                         {
                                             LogId = qso.LogId,
                                             QsoNo = qso.QsoNo,
                                             EntityState = EntityState.Added
                                         };
                                        UbnNotInLogs.Add(UbnNotInLog);
                                        UbnNotInLogsCurrent.Add(UbnNotInLog);
                                    }
                                    bBadCall = true;
                                    break;
                                }
                            }
                            if (bBadCall == false)
                            {//UT1AA incorrect call
                                var entry = UbnIncorrectCalls.Where(x => x.LogId == LogId && x.QsoNo == itemQso.QsoNo).FirstOrDefault();
                                if (entry == null)
                                {
                                    //Mark QsoBadOrNotInMyLog as bad call
                                    UbnIncorrectCall UbnIncorrectCall = new UbnIncorrectCall()
                                    {
                                        QsoNo = itemQso.QsoNo,
                                        LogId = LogId, //mylog
                                        CorrectCall = qso.Call,
                                        EntityState = EntityState.Added
                                    };
                                    UbnIncorrectCalls.Add(UbnIncorrectCall);
                                }
                                bBadCall = true;
                                break;
                            }
                        }
                    }
                    if (bBadCall == false)
                    {//not in log => NIL qso in there  log
                        var entry = UbnNotInLogsCurrent.Where(x => x.LogId == qso.LogId && x.QsoNo == qso.QsoNo).FirstOrDefault();
                        if (entry == null)
                        {
                            UbnNotInLog UbnNotInLog = new UbnNotInLog()
                            {
                                LogId = qso.LogId,
                                QsoNo = qso.QsoNo,
                                EntityState = EntityState.Added
                            };
                            UbnNotInLogs.Add(UbnNotInLog);
                            UbnNotInLogsCurrent.Add(UbnNotInLog);
                        }
                        bBadCall = true;

                    }
                }
		 
	        }

        }




        private bool? PartialMatch(string ContestId, string CallNotInMyLog, string LogSnippetCall, int SnippetCallsignId, int criteria)
        {
            bool? results = false;
            char[] CallChars = CallNotInMyLog.ToCharArray();
            char[] CallCharsSnip = LogSnippetCall.ToCharArray();
            

            //they do not consider a missing /P to be a mismatch ie: G6BDV G6BDV/P
            //one call has portable not both
            //if (CallNotInMyLog.Contains(@"/P"))
            //{
            //    CallNotInMyLog = CallNotInMyLog.Replace(@"/P", "");
            //}
            //if (LogSnippetCall.Contains(@"/P"))
            //{
            //    LogSnippetCall = LogSnippetCall.Replace(@"/P", "");
            //}

            if (CallNotInMyLog.Contains('/') == false && LogSnippetCall.Contains('/') == false)
            {

                if (CallNotInMyLog.Length > LogSnippetCall.Length)
                { //PA3GDD PA1X not a match    9A5ADI 9A5I is a match
                    var charsToCompare = CallNotInMyLog.Zip(LogSnippetCall, (LeftChar, RightChar) => new { LeftChar, RightChar });
                    var matchingChars = charsToCompare.Select(pair => pair.LeftChar == pair.RightChar);
                    var NonMatchingCharsCount = matchingChars.Where(x => x == false).Count();
                    //VA3ZTZ VA3FP should not be a match
                    //SP2FWC  SP2JC  is a match
                    //if (CallNotInMyLog == "SP2FWC")
                    //{

                    //}
                    if (NonMatchingCharsCount <= criteria && ((CallNotInMyLog.Length - (LogSnippetCall.Length - NonMatchingCharsCount)) < CallNotInMyLog.Length / 2))
                    {
                        results = true;
                    }
                    else if (NonMatchingCharsCount <= criteria && ((LogSnippetCall.Length - (CallNotInMyLog.Length - NonMatchingCharsCount)) <= LogSnippetCall.Length / 2))
                    { //SP2FWC has CN2R qso bot in my log, My log has SP2JC which is unique. Mak SP2JC wrong
                        //Test unique
                        var bUnique = IUbnUniqueRepository.CheckCallIsUniqueInQsos(ContestId, SnippetCallsignId);
                        if (bUnique == true)
                        {//its unique
                            results = true;
                        }
                    }
                    else
                    {//  WP3E P3E 
                        //Check reverse
                        string CallNotInMyLogRev = StringOps.GetReverseString(CallNotInMyLog);
                        string LogSnippetCallRev = StringOps.GetReverseString(LogSnippetCall);
                        charsToCompare = CallNotInMyLogRev.Zip(LogSnippetCallRev, (LeftChar, RightChar) => new { LeftChar, RightChar });
                        matchingChars = charsToCompare.Select(pair => pair.LeftChar == pair.RightChar);
                        NonMatchingCharsCount = matchingChars.Where(x => x == false).Count();
                        if (NonMatchingCharsCount <= criteria)
                        {
                            results = true;
                        }

                    }
                }
                else if (CallNotInMyLog.Length < LogSnippetCall.Length)
                { // N3WMC  KN3WMC  
                    var charsToCompare = CallNotInMyLog.Zip(LogSnippetCall, (LeftChar, RightChar) => new { LeftChar, RightChar });
                    var matchingChars = charsToCompare.Select(pair => pair.LeftChar == pair.RightChar);
                    var NonMatchingCharsCount = matchingChars.Where(x => x == false).Count();

                    if (NonMatchingCharsCount <= criteria && ((LogSnippetCall.Length - (CallNotInMyLog.Length - NonMatchingCharsCount)) < LogSnippetCall.Length / 2))
                    {
                        results = true;
                    }
                    else if (NonMatchingCharsCount <= criteria && ((LogSnippetCall.Length - (CallNotInMyLog.Length - NonMatchingCharsCount)) <= LogSnippetCall.Length / 2))
                    { //SP2FWC has CN2R qso bot in my log, My log has SP2JC which is unique. Mak SP2JC wrong
                        //Test unique
                        var bUnique = IUbnUniqueRepository.CheckCallIsUniqueInQsos(ContestId, SnippetCallsignId);
                        if (bUnique == true)
                        {//its unique
                            results = true;
                        }
                    }
                    else
                    {//  N3WMC  KN3WMC 
                        //Check reverse
                        string CallNotInMyLogRev = StringOps.GetReverseString(CallNotInMyLog);
                        string LogSnippetCallRev = StringOps.GetReverseString(LogSnippetCall);
                        charsToCompare = CallNotInMyLogRev.Zip(LogSnippetCallRev, (LeftChar, RightChar) => new { LeftChar, RightChar });
                        matchingChars = charsToCompare.Select(pair => pair.LeftChar == pair.RightChar);
                        NonMatchingCharsCount = matchingChars.Where(x => x == false).Count();
                        if (NonMatchingCharsCount <= criteria && ((LogSnippetCall.Length - (CallNotInMyLog.Length - NonMatchingCharsCount)) < LogSnippetCall.Length / 2))
                        {
                            results = true;
                        }

                    }
                }
                else
                { // calls are equal length
                    //http://stackoverflow.com/questions/7879636/compare-the-characters-in-two-strings
                    var charsToCompare = CallNotInMyLog.Zip(LogSnippetCall, (LeftChar, RightChar) => new { LeftChar, RightChar });
                    var matchingChars = charsToCompare.Select(pair => pair.LeftChar == pair.RightChar);
                    var NonMatchingCharsCount = matchingChars.Where(x => x == false).Count();
                    if (NonMatchingCharsCount == 0 && CallNotInMyLog.Length == LogSnippetCall.Length)
                    {
                        results = null;
                    }
                    else if (NonMatchingCharsCount <= criteria)
                    {
                        results = true;
                    }
                }
            }
            else if(CallNotInMyLog.Contains('/') == true && LogSnippetCall.Contains('/') == true)
            { //handle portable calls by checking each side
                var charsToCompare = CallNotInMyLog.Zip(LogSnippetCall, (LeftChar, RightChar) => new { LeftChar, RightChar });
                var matchingChars = charsToCompare.Select(pair => pair.LeftChar == pair.RightChar);
                var NonMatchingCharsCount = matchingChars.Where(x => x == false).Count();

                if (NonMatchingCharsCount == 0 && CallNotInMyLog.Length == LogSnippetCall.Length)
                {
                    results = null;
                }
                else if (NonMatchingCharsCount <= criteria)
                {
                    results = true;
                }
                else if (CallNotInMyLog.Length < LogSnippetCall.Length)
                {// P3E/2  WP3E/2
                    //Check reverse
                    string CallNotInMyLogRev = CallNotInMyLog.Reverse().ToString();
                    string LogSnippetCallRev = LogSnippetCall.Reverse().ToString();
                    charsToCompare = CallNotInMyLogRev.Zip(LogSnippetCallRev, (LeftChar, RightChar) => new { LeftChar, RightChar });
                    matchingChars = charsToCompare.Select(pair => pair.LeftChar == pair.RightChar);
                    NonMatchingCharsCount = matchingChars.Where(x => x == false).Count();
                    if (NonMatchingCharsCount <= criteria)
                    {
                        results = true;
                    }
                }
                else
                {  //equal length
                    //handle portable calls by checking each side//IT9/K7ZSD  G7ZSD/IT9  
                    string[] CallNotInMyLogs = CallNotInMyLog.Split(new char[] { '/' });
                    string CallNotInMyLogLeft = CallNotInMyLogs[0];
                    string CallNotInMyLogRight = CallNotInMyLogs[1];

                    string[] LogSnippetCalls = LogSnippetCall.Split(new char[] { '/' });
                    string LogSnippetCallLeft = LogSnippetCalls[0];
                    string LogSnippetCallRight = LogSnippetCalls[1];

                    //now compare all combination
                    charsToCompare = CallNotInMyLogLeft.Zip(LogSnippetCallLeft, (LeftChar, RightChar) => new { LeftChar, RightChar });
                    matchingChars = charsToCompare.Select(pair => pair.LeftChar == pair.RightChar);
                    NonMatchingCharsCount = matchingChars.Where(x => x == false).Count();
                    if (NonMatchingCharsCount == 0 && CallNotInMyLogLeft.Length == LogSnippetCallLeft.Length)
                    {//left calls match
                        //now check right side partial match. They cannot exacly match
                        charsToCompare = CallNotInMyLogRight.Zip(LogSnippetCallRight, (LeftChar, RightChar) => new { LeftChar, RightChar });
                        matchingChars = charsToCompare.Select(pair => pair.LeftChar == pair.RightChar);
                        NonMatchingCharsCount = matchingChars.Where(x => x == false).Count();
                        if (NonMatchingCharsCount == 0 )
                        {
                            results = null;
                        }
                        else if (NonMatchingCharsCount <= criteria)
                        {
                            results = true;
                        }
                    }
                    else
                    { // test the cross //IT9/K7ZSD  G7ZSD/IT9  
                        charsToCompare = CallNotInMyLogLeft.Zip(LogSnippetCallRight, (LeftChar, RightChar) => new { LeftChar, RightChar });
                        matchingChars = charsToCompare.Select(pair => pair.LeftChar == pair.RightChar);
                        NonMatchingCharsCount = matchingChars.Where(x => x == false).Count();
                        if (NonMatchingCharsCount == 0 && CallNotInMyLogLeft.Length == LogSnippetCallRight.Length)
                        {//left cross right calls match
                            charsToCompare = CallNotInMyLogRight.Zip(LogSnippetCallLeft, (LeftChar, RightChar) => new { LeftChar, RightChar });
                            matchingChars = charsToCompare.Select(pair => pair.LeftChar == pair.RightChar);
                            NonMatchingCharsCount = matchingChars.Where(x => x == false).Count();
                            if (NonMatchingCharsCount == 0)
                            {
                                results = null;
                            }
                            else if (NonMatchingCharsCount <= criteria)
                            {
                                results = true;
                            }
                        }
                    }
                }
            }
            else if ((CallNotInMyLog.Contains('/') == true && LogSnippetCall.Contains('/') == false) ||
               (CallNotInMyLog.Contains('/') == false && LogSnippetCall.Contains('/') == true))
            { //handle portable calls on one side  WA3C WA3C/8
                //THIS check is not performed in the UBN results
                string[] CallNotInMyLogs = CallNotInMyLog.Split(new char[] { '/' });
                if (CallNotInMyLog.Contains('/') == true)
                {
                    int Index;
                    if (CallNotInMyLogs[0].Length > CallNotInMyLogs[1].Length == true)
                    {//check my log for call
                        Index = 1;
                    }
                    else
                    { //cty on left
                        Index = 0;
                    }
                    var charsToCompare = CallNotInMyLog.Zip(LogSnippetCall, (LeftChar, RightChar) => new { LeftChar, RightChar });
                    var matchingChars = charsToCompare.Select(pair => pair.LeftChar == pair.RightChar);
                    var NonMatchingCharsCount = matchingChars.Where(x => x == false).Count();
                    if (NonMatchingCharsCount == 0 && (CallNotInMyLogs[Index] == @"P" || CallNotInMyLogs[Index] == @"QRP" || CallNotInMyLogs[Index] == @"M"))
                    {// p and QRP  and M ignored
                        results = null;
                    }
                    else if (NonMatchingCharsCount <= criteria && (CallNotInMyLogs[Index] != @"P" && CallNotInMyLogs[Index] != @"QRP" || CallNotInMyLogs[Index] != @"M"))
                    {  //WA3C/8
                        results = true;
                    }
                    else
                    {    //Check for 9A7JZC/QRP in my log and 9A7JSC has CN2R in his log
                        if (CallNotInMyLogs[1] == "P" || CallNotInMyLogs[1] == "QRP" || CallNotInMyLogs[1] == "M"  )
                        {
                            charsToCompare = CallNotInMyLogs[0].Zip(LogSnippetCall, (LeftChar, RightChar) => new { LeftChar, RightChar });
                            matchingChars = charsToCompare.Select(pair => pair.LeftChar == pair.RightChar);
                            NonMatchingCharsCount = matchingChars.Where(x => x == false).Count();
                            if (NonMatchingCharsCount == 0)
                            {// p and QRP  and M ignored
                                results = null;
                            }

                            else if (NonMatchingCharsCount <= criteria)
                            {
                                results = true;
                            }
                        }
                    }
                    
                }
                else if (LogSnippetCall.Contains('/') == true)
                {
                    int Index;
                    string[] LogSnippetCalls = LogSnippetCall.Split(new char[] { '/' });
                    if (LogSnippetCalls[0].Length > LogSnippetCalls[1].Length == true)
                    {//check my log for call
                        Index = 1;
                    }
                    else
                    {
                        Index = 0;
                    }
                    var charsToCompare = CallNotInMyLog.Zip(LogSnippetCall, (LeftChar, RightChar) => new { LeftChar, RightChar });
                    var matchingChars = charsToCompare.Select(pair => pair.LeftChar == pair.RightChar);
                    var NonMatchingCharsCount = matchingChars.Where(x => x == false).Count();
                    if (NonMatchingCharsCount == 0 && (LogSnippetCalls[Index] == @"P" || LogSnippetCalls[Index] == @"QRP" || LogSnippetCalls[Index] == @"M"))
                    {// p and QRP ignored
                        results = null;
                    }
                    else if (NonMatchingCharsCount <= criteria && (LogSnippetCalls[Index] != @"P" && LogSnippetCalls[Index] != @"QRP" || LogSnippetCalls[Index] != @"M"))
                    {
                        results = true;
                    }
                }
            }

            return results;
        }


        public void ProcessDupes(int LogId, IList<IGrouping<int, QsoBadNilContact>> QsoDupesMyLogBand,
                        ref IList<UbnIncorrectCall> UbnIncorrectCalls, ref IList<UbnNotInLog> UbnNotInLogs,
                        ref IList<UbnDupe> UbnDupes)
        {
            foreach (var dupe in QsoDupesMyLogBand)
            { //group dupes
                //VE5PV worked me twice, My log has 1st Qso as VE3DX and second as VE5PV
                //need to check 1st call is in mylog
                //The skipprd call below must be in my log
                foreach (var qso in dupe.Skip(1))
                {
                    if (UbnIncorrectCalls.Where(x => x.LogId == LogId && x.QsoNo == qso.QsoNo).FirstOrDefault() != null)
                    {//not a dupe, its nas Call
                        break;
                    }
                    else if (UbnNotInLogs.Where(x => x.LogId == LogId && x.QsoNo == qso.QsoNo).FirstOrDefault() != null)
                    {//not a dupe its NIL
                        break;
                    }
                    else if (UbnDupes.Where(x => x.LogId == LogId && x.QsoNo == qso.QsoNo).FirstOrDefault() != null)
                    {//Already a dupe
                        break;
                    }
                    else
                    {
                        //This is a dupe
                        var entry = UbnDupes.Where(x => x.LogId == LogId && x.QsoNo == qso.QsoNo).FirstOrDefault();
                        if (entry == null)
                        {
                            UbnDupe UbnDupe = new UbnDupe()
                             {
                                 LogId = LogId, //mylog
                                 QsoNo = qso.QsoNo,
                                 EntityState = EntityState.Added
                             };
                            UbnDupes.Add(UbnDupe);
                        }
                        break;
                    }
                }
            }
        }


    }
}
