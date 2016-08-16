using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using L2Sql.DomainModel;
using L2Sql.DataAccessLayer;
using L2Sql.Dto;
using Logqso.mvc.common;

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

        private readonly int DeltaMinutesMatch = 5;

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
        public void UpdateQsoUbnxds(IList<QsoUpdateUbnxdDTO> QsoUpdateUbnxdDTOs)
        {

        }
        public void GetBadCallsNils(string ContestId, int LogId, int CallSignId,
                        out IList<UbnIncorrectCall> UbnIncorrectCalls, out IList<UbnNotInLog> UbnNotInLogs, out IList<UbnDupe> UbnDupes)

        {
            IList<QsoBadNilContact> QsoNotInMyLog = null;
            IList<QsoBadNilContact> QsoBadOrNotInMyLog = null;
            IList<QsoBadNilContact> QsoInMyLogBand = null;
            IList<IGrouping<int, QsoBadNilContact>> QsoDupesBand = null;

            //160M
            ////IQsoRepository.GetBandCallsInMyLogWithNoSubmittedLog(ContestId, LogId, 1800m, 2000m,
            ////    out QsoNotInMyLog, out QsoBadOrNotInMyLog);
            ////ProcessNilsAndBads(LogId, out UbnIncorrectCalls, out UbnNotInLogs, QsoNotInMyLog, QsoBadOrNotInMyLog);
            //////8M
            ////IQsoRepository.GetBandCallsInMyLogWithNoSubmittedLog(ContestId, LogId, 3500m, 4000m,
            ////    out QsoNotInMyLog, out QsoBadOrNotInMyLog);
            ////ProcessNilsAndBads(LogId, out UbnIncorrectCalls, out UbnNotInLogs, QsoNotInMyLog, QsoBadOrNotInMyLog);
            //////40M
            ////IQsoRepository.GetBandCallsInMyLogWithNoSubmittedLog(ContestId, LogId, 7000m, 7300m,
            ////    out QsoNotInMyLog, out QsoBadOrNotInMyLog);
            ////ProcessNilsAndBads(LogId, out UbnIncorrectCalls, out UbnNotInLogs, QsoNotInMyLog, QsoBadOrNotInMyLog);
            //20M
            IQsoRepository.GetBandCallsInMyLogWithNoSubmittedLog(ContestId, LogId, 14000m, 14350m,
                out QsoNotInMyLog, out QsoBadOrNotInMyLog);
           IQsoRepository.GetQsosFromLogWithFreqRange(ContestId, LogId, 14000m, 14350m, out QsoInMyLogBand);
           IQsoRepository.GetDupeQsosFromCallsignWithFreqRange(ContestId, LogId, CallSignId, 14000m, 14350m, out QsoDupesBand);
           ProcessNilsDupesAndBads(LogId, out UbnIncorrectCalls, out UbnNotInLogs, out UbnDupes,
                        QsoNotInMyLog, QsoBadOrNotInMyLog, QsoInMyLogBand, QsoDupesBand );
            //////15M
            ////IQsoRepository.GetBandCallsInMyLogWithNoSubmittedLog(ContestId, LogId, 21000m, 21450m,
            ////    out QsoNotInMyLog, out QsoBadOrNotInMyLog);
            ////ProcessNilsAndBads(LogId, out UbnIncorrectCalls, out UbnNotInLogs, QsoNotInMyLog, QsoBadOrNotInMyLog);
            //////10M
            ////IQsoRepository.GetBandCallsInMyLogWithNoSubmittedLog(ContestId, LogId, 28000m, 29700m,
            ////    out QsoNotInMyLog, out QsoBadOrNotInMyLog);
            ////ProcessNilsAndBads(LogId, out UbnIncorrectCalls, out UbnNotInLogs, QsoNotInMyLog, QsoBadOrNotInMyLog);



            return;
        }


        public IList<Qso> GetQsos(int LogId)
        {
            return IQsoRepository.GetList(d => d.LogId.Equals(LogId));
            //d => d.CallSign); //include related 
        }

        public IList<short> GetUniquesFromContest(string ContestId, int LogId)
        {
            return IUbnUniqueRepository.GetUniquesFromContest(ContestId, LogId);
        }

        public void UpdateUniquesFromContest(IList<UbnUnique> UbnUniques)
        {
            if (UbnUniques.Count > 0)
            {
                IUbnUniqueRepository.Add(UbnUniques.ToArray());
            }
        }


        public void UpdateNilsFromContest(IList<UbnNotInLog> UbnNotInLogs)
        {
            if (UbnNotInLogs.Count > 0)
            {
                IUbnNotInLogRepository.Add(UbnNotInLogs.ToArray());
            }
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

        private void ProcessNilsDupesAndBads(int LogId,
                 out IList<UbnIncorrectCall> UbnIncorrectCalls, out IList<UbnNotInLog> UbnNotInLogs, out IList<UbnDupe> UbnDupes ,
                 IList<QsoBadNilContact> QsoNotInMyLog, IList<QsoBadNilContact> QsoBadOrNotInMyLog, IList<QsoBadNilContact> QsoInMyLogBand,
                    IList<IGrouping<int, QsoBadNilContact>> QsoDupesBand)
        {
            UbnIncorrectCalls = new List<UbnIncorrectCall>();
            UbnNotInLogs = new List<UbnNotInLog>();
            UbnDupes = new List<UbnDupe>();

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
                    //if (item.Call == "G6BDV")
                    //{

                    //}
                    bBadCall = false;
                    //get +- 3 minute window from QsoBadOrNotInMyLog
                    LogSnippet = QsoBadOrNotInMyLog.Where(x => x.QsoDateTime == item.QsoDateTime).ToList();
                    foreach (var itemQso in LogSnippet)
                    {
                        // look for bad call
                        bool? PartialMatchResult = PartialMatch(item.Call, itemQso.Call);
                        if (PartialMatchResult == null)
                        {
                            bBadCall = true;  //stops searn, No NIL or BAd
                            break;
                        }
                        else if (PartialMatchResult == true)
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
                            bBadCall = true;
                            break;
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
                            bool? PartialMatchResult = PartialMatch(item.Call, itemQso.Call);
                            if (PartialMatchResult == null)
                            {
                                bBadCall = true;  //stops searn, No NIL or BAd
                                break;
                            }
                            else if (PartialMatchResult == true)
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
                                bBadCall = true;
                                break;
                            }
                        }

                    }
                    if (bBadCall == false)
                    {//not in there log
                        UbnNotInLog UbnNotInLog = new UbnNotInLog()
                        {
                            LogId = item.LogId,
                            QsoNo = item.QsoNo,
                            EntityState = EntityState.Added
                        };
                         UbnNotInLogs.Add(UbnNotInLog);
                    }
                }
            }
            //CHECK QSOs wth my log but no QSO log was submitted
            //The bad call may be a dupe call in there log and one good and one bad call in my log? 
            //Ik2CYW was worked once IMYL IK2YCW. worked CN2R twice IK2CYW needs to be checked against log snippet
            //We need to check the full logsnippet.
            //ie: my log k2VMS  and  KC2VMS exists.
            //Because the Log exists KC2VMS is not in the original log snippet.
            //We need to vreate a new LogSnippey from the full mylog.
            foreach (var dupe in QsoDupesBand)
	        { //group dupes
                foreach (var qso in dupe.Skip(1))
                { //each dupe
                    bBadCall = false;
                    LogSnippet = QsoInMyLogBand.Where( x => x.QsoDateTime == qso.QsoDateTime).ToList();
                    // look for bad call
                    foreach (var itemQso in LogSnippet)
                    { //check against snippet collection
                        // look for bad call
                        bool? PartialMatchResult = PartialMatch(qso.Call, itemQso.Call);
                        if (PartialMatchResult == null)
                        {
                            bBadCall = true;  //stops searn, No NIL or BAd
                            //This is a dupe
                            UbnDupe UbnDupe = new UbnDupe()
                            {
                                LogId = LogId, //mylog
                                QsoNo = itemQso.QsoNo,
                                EntityState = EntityState.Added
                            };
                            UbnDupes.Add(UbnDupe);
                            break;
                        }
                        else if (PartialMatchResult == true)
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
                            bBadCall = true;
                            break;
                        }
                    }
                    if (bBadCall == false)
                    {
                        LogSnippet = QsoInMyLogBand.Where(
                            (x => x.QsoDateTime >= qso.QsoDateTime.AddMinutes(-DeltaMinutesMatch) && x.QsoDateTime < qso.QsoDateTime
                            ||
                            (x.QsoDateTime > qso.QsoDateTime && x.QsoDateTime <= qso.QsoDateTime.AddMinutes(DeltaMinutesMatch)))
                            )
                            .ToList();
                        foreach (var itemQso in LogSnippet)
                        {
                            bool? PartialMatchResult = PartialMatch(qso.Call, itemQso.Call);
                            if (PartialMatchResult == null)
                            {
                                bBadCall = true;  //stops searn, No NIL or BAd
                                //This is a dupe
                                UbnDupe UbnDupe = new UbnDupe()
                                {
                                    LogId = LogId, //mylog
                                    QsoNo = itemQso.QsoNo,
                                    EntityState = EntityState.Added
                                };
                                UbnDupes.Add(UbnDupe);
                                break;
                            }
                            else if (PartialMatchResult == true)
                            {
                                //UT8AL UT1AA off by 2 chars but UT1AA is valid dupe
                                //We need to search in there log  UT!AA for dupe Qsos
                                var dupecall = LogSnippet.Where(x=>x.Call == itemQso.Call)
                                //Mark QsoBadOrNotInMyLog as bad call
                                UbnIncorrectCall UbnIncorrectCall = new UbnIncorrectCall()
                                {
                                    QsoNo = itemQso.QsoNo,
                                    LogId = LogId, //mylog
                                    CorrectCall = qso.Call,
                                    EntityState = EntityState.Added
                                };
                                UbnIncorrectCalls.Add(UbnIncorrectCall);
                                bBadCall = true;
                                break;
                            }
                        }
                    }
                    if (bBadCall == false)
                    {//not in log => NIL qso in there  log
                        UbnNotInLog UbnNotInLog = new UbnNotInLog()
                        {
                            LogId = qso.LogId,
                            QsoNo = qso.QsoNo,
                            EntityState = EntityState.Added
                        };
                        UbnNotInLogs.Add(UbnNotInLog);
                        bBadCall = true;

                    }
                }
		 
	        }

        }


        private bool? PartialMatch(string CallNotInMyLog, string LogSnippetCall)
        {
            bool? results = false;
            char[] CallChars = CallNotInMyLog.ToCharArray();
            char[] CallCharsSnip = LogSnippetCall.ToCharArray();
            int criteria = 2;

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
                { //PA1X PA3GDD or 9A5I 9A5ADI
                    var charsToCompare = CallNotInMyLog.Zip(LogSnippetCall, (LeftChar, RightChar) => new { LeftChar, RightChar });
                    var matchingChars = charsToCompare.Select(pair => pair.LeftChar == pair.RightChar);
                    var NonMatchingCharsCount = matchingChars.Where(x => x == false).Count();

                    if (NonMatchingCharsCount <= criteria  && ((CallNotInMyLog.Length - (LogSnippetCall.Length - NonMatchingCharsCount) ) <= CallNotInMyLog.Length/2 )  )
                    {
                        results = true;
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
                else
                {
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
                        if (NonMatchingCharsCount <= criteria)
                        {
                            results = true;
                        }
                        else
                        {
                            results = null;
                        }
                    }
                    else
                    { // test the cross
                        charsToCompare = CallNotInMyLogLeft.Zip(LogSnippetCallRight, (LeftChar, RightChar) => new { LeftChar, RightChar });
                        matchingChars = charsToCompare.Select(pair => pair.LeftChar == pair.RightChar);
                        NonMatchingCharsCount = matchingChars.Where(x => x == false).Count();
                        if (NonMatchingCharsCount == 0 && CallNotInMyLogLeft.Length == LogSnippetCallRight.Length)
                        {//left cross right calls match
                            charsToCompare = CallNotInMyLogRight.Zip(LogSnippetCallLeft, (LeftChar, RightChar) => new { LeftChar, RightChar });
                            matchingChars = charsToCompare.Select(pair => pair.LeftChar == pair.RightChar);
                            NonMatchingCharsCount = matchingChars.Where(x => x == false).Count();
                            if (NonMatchingCharsCount <= criteria)
                            {
                                results = true;
                            }
                            else
                            {
                                results = null;
                            }
                        }
                    }
                }
            }
            else if ((CallNotInMyLog.Contains('/') == true && LogSnippetCall.Contains('/') == false) ||
               (CallNotInMyLog.Contains('/') == false && LogSnippetCall.Contains('/') == true))
            { //handle portable calls on one side  WA3C WA3C/8
                //THIS check is not performed in the UBN results
                if (CallNotInMyLog.Contains('/') == true)
                {
                    int Index;
                    string[] CallNotInMyLogs = CallNotInMyLog.Split(new char[] { '/' });
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
                    if (NonMatchingCharsCount == 0 && (CallNotInMyLogs[Index] == @"P" || CallNotInMyLogs[Index] == @"QRP"))
                    {// p and QRP ignored
                        results = null;
                    }
    
                    if (NonMatchingCharsCount <= criteria && (CallNotInMyLogs[Index] != @"P" && CallNotInMyLogs[Index] != @"QRP"))
                    {
                        results = true;
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
                    if (NonMatchingCharsCount == 0 && (LogSnippetCalls[Index] == @"P" || LogSnippetCalls[Index] == @"QRP"))
                    {// p and QRP ignored
                        results = null;
                    }
                    if (NonMatchingCharsCount <= criteria && (LogSnippetCalls[Index] != @"P" && LogSnippetCalls[Index] != @"QRP"))
                    {
                        results = true;
                    }
                }
            }

            return results;
        }

    }
}
