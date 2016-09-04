using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using L2Sql.BusinessLayer;
using CtyLib;
using System.IO;
using System.ComponentModel;
using L2Sql.DomainModel;
using Logqso.mvc.common;
using Logqso.mvc.common.Enum;
using L2Sql.Dto;
using System.Diagnostics;

namespace L2Sql.BusinessLayer
{
    public class ProcessUBNDX
    {
        public IBusiness IBusiness;
        public CtyLib.CCtyObj CtyObj { get; set; }

        protected string LogFileDirectory { get; set; }
        private InputLog InputLog { get; set; }
        ContestTypeEnum ContestTypeEnum;
        private string SqlServerInstance { get; set; }
        private string SqlDatabase { get; set; }
        private string SqlQsoTable { get; set; }


        public ProcessUBNDX(string LogFileDirectory, 
            string SqlServerInstance, string SqlDatabase, string SqlQsoTable)
        {
            this.LogFileDirectory = LogFileDirectory;
            this.SqlServerInstance = SqlServerInstance;
            this.SqlDatabase = SqlDatabase;
            this.SqlQsoTable = SqlQsoTable;


      }


        public bool UbndxToDatabase(BackgroundWorker worker)
        {
            bool result = true;
            IBusiness = new Business();
            DirectoryInfo di = new DirectoryInfo(LogFileDirectory);

            string ContestId = di.Name.ToUpper();

            ContestTypeEnum ContestTypeEnum;
            CatOperatorEnum CatOperatorEnum;
            var Contest = IBusiness.GetContest(ContestId);
            ContestTypeEnum = Contest.ContestTypeEnum;

#if false
            IList<Log> Logs = IBusiness.GetAllLogsWithCallsign(ContestId);      //includes CallSign , logcategory
#else
           // 1 Log for testing
            Log logtest = IBusiness.GetLog("CQWWSSB2015", 1);
            IList<Log> Logs = new List<Log>();
            Logs.Add(logtest);

#endif
            ////TESTING
            //int count = 1;
            //foreach (var log in Logs)
            //{ //for individual log2 testing
            //    var log2 = Logs.Where(x => x.LogId == 21387).FirstOrDefault();
            //    var Logcategory = IBusiness.GetLogCategory(log2.LogCategoryId);
            //    CatOperatorEnum = Logcategory.CatOperatorEnum;

            //    IList<QsoAddPoinsMultsDTO> QsoAddPoinsMultsDTOs = IBusiness.GetQsoPointsMults(log2.LogId);
            //    //CallSign CallSign = IBusiness.GetCallSign(log.CallsignId);
            //    worker.ReportProgress(1, new InputLog(log2.CallSign.Call, QsoAddPoinsMultsDTOs.Count, count++, DateTime.Now.ToString("HH:mm:ss")));
            //    if (QsoAddPoinsMultsDTOs.Count != 0)
            //    {
            //        //all NILS and Dupes and Bad calls and incorrect exchanges need to be processed before collecting UNiques
            //        //Bad calls 
            //        SetbnxdDTOs(Logs, Contest.ContestId, ContestTypeEnum, CatOperatorEnum, log2.LogId, log2.CallSign.Prefix, log2.CallSign.CallSignId);

            //    }
            //}

            int count = 1;
            foreach (var log in Logs)
            {
                var Logcategory = IBusiness.GetLogCategory(log.LogCategoryId);
                CatOperatorEnum = Logcategory.CatOperatorEnum;

                IList<QsoAddPoinsMultsDTO> QsoAddPoinsMultsDTOs = IBusiness.GetQsoPointsMults(log.LogId);
                //CallSign CallSign = IBusiness.GetCallSign(log.CallsignId);
                worker.ReportProgress(1, new InputLog(log.CallSign.Call, QsoAddPoinsMultsDTOs.Count, count++, DateTime.Now.ToString("HH:mm:ss") + "  " + log.CallSign.Call));
                if (QsoAddPoinsMultsDTOs.Count != 0)
                {
                    //Bad calls 
                    SetbDTOs(Logs, Contest.ContestId, ContestTypeEnum, CatOperatorEnum, log.LogId, log.CallSign.Prefix, log.CallSign.CallSignId);
                    //break;
                }
            }
            

            count =1;
            foreach (var log in Logs)
            {
                var Logcategory = IBusiness.GetLogCategory(log.LogCategoryId);
                CatOperatorEnum = Logcategory.CatOperatorEnum;

                IList<QsoAddPoinsMultsDTO> QsoAddPoinsMultsDTOs = IBusiness.GetQsoPointsMults(log.LogId);
                //CallSign CallSign = IBusiness.GetCallSign(log.CallsignId);
                worker.ReportProgress(1, new InputLog(log.CallSign.Call + "-2", QsoAddPoinsMultsDTOs.Count, count++, DateTime.Now.ToString("HH:mm:ss") + "  " + log.CallSign.Call + "-2"));
                if (QsoAddPoinsMultsDTOs.Count != 0)
                {
                    //all NILS and Dupes and Bad calls and incorrect exchanges need to be processed before collecting UNiques
                    SetbnxdDTOs(Logs, Contest.ContestId, ContestTypeEnum, CatOperatorEnum, log.LogId, log.CallSign.Prefix, log.CallSign.CallSignId);

                    //break;
                }
            }

            //Uniques
            count = 1;
            foreach (var log in Logs)
            {
                //all NILS and Dupes and Bad calls and incorrect exchanges need to be processed before collecting UNiques
                IList<QsoAddPoinsMultsDTO> QsoAddPoinsMultsDTOs = IBusiness.GetQsoPointsMults(log.LogId);
                //CallSign CallSign = IBusiness.GetCallSign(log.CallsignId);
                worker.ReportProgress(1, new InputLog(log.CallSign.Call + "-3", QsoAddPoinsMultsDTOs.Count, count++, DateTime.Now.ToString("HH:mm:ss") + "  " + log.CallSign.Call + "-3"));
                if (QsoAddPoinsMultsDTOs.Count != 0)
                {
                    //all NILS and Dupes and Bad calls and incorrect exchanges need to be processed before collecting UNiques
                    SetuDTOs(Contest.ContestId, log.LogId);

                    //break;
                }
            }


            //Check Uniques for similar QSOs on the same band , in the same country, +- 10 min
            //+- 10 min allows for the Op who only makes a few qsos spread in time.
            //if a criteria match is found:
            //Mark the Qso as a bad call in my log and remove the UNique qsp from the UbnUniques SQl table
            count = 1;
            foreach (var log in Logs)
            {
                //all NILS and Dupes and Bad calls and incorrect exchanges need to be processed before collecting UNiques
                IList<QsoAddPoinsMultsDTO> QsoAddPoinsMultsDTOs = IBusiness.GetQsoPointsMults(log.LogId);
                worker.ReportProgress(1, new InputLog(log.CallSign.Call + "-4", QsoAddPoinsMultsDTOs.Count, count++, DateTime.Now.ToString("HH:mm:ss") + "  " + log.CallSign.Call + "-4"));
                if (QsoAddPoinsMultsDTOs.Count != 0)
                {
                    //all NILS and Dupes and Bad calls and incorrect exchanges need to be processed before collecting UNiques
                    SetutoBadSameCountryDTOs(Logs, Contest.ContestId, log.LogId);

                    //break;
                }
            }


            //Check Uniques for similar QSOs on the same band , NOT in the same country, +- 5 min
            //by thie point there will be the minum nimber of uniques to check
            //+- 5 min allows for the Op who only makes a few qsos spread in time.
            //if a criteria match is found:
            //Mark the Qso as a bad call in my log and remove the UNique qsp from the UbnUniques SQl table
            count = 1;
            foreach (var log in Logs)
            {
                //all NILS and Dupes and Bad calls and incorrect exchanges need to be processed before collecting UNiques
                IList<QsoAddPoinsMultsDTO> QsoAddPoinsMultsDTOs = IBusiness.GetQsoPointsMults(log.LogId);
                worker.ReportProgress(1, new InputLog(log.CallSign.Call + "-5", QsoAddPoinsMultsDTOs.Count, count++, DateTime.Now.ToString("HH:mm:ss") + "  " + log.CallSign.Call + "-5"));
                if (QsoAddPoinsMultsDTOs.Count != 0)
                {
                    //all NILS and Dupes and Bad calls and incorrect exchanges need to be processed before collecting UNiques
                    SetutoBadDifferentCountryDTOs(Logs, Contest.ContestId, log.LogId);

                    //break;
                }
            }

            return result;

        }

        private void SetbDTOs(IList<Log> Logs, string ContestId, ContestTypeEnum ContestTypeEnum, CatOperatorEnum CatOperatorEnum, int LogId,
                    string LogPrefix, int CallSignId)
        {//1st pass bad
            IList<UbnIncorrectCall> UbnIncorrectCalls = new List<UbnIncorrectCall>();
            IList<UbnNotInLog> UbnNotInLogs = new List<UbnNotInLog>();
            IList<UbnDupe> UbnDupes = new List<UbnDupe>();


            //find bad call pass one
            IBusiness.GetBadCallsNils(Logs, ContestId, LogId, CallSignId, false, ref UbnIncorrectCalls, ref UbnNotInLogs, ref UbnDupes);
 
            if (UbnIncorrectCalls.Count > 0)
            {//store corrections in DB
                IBusiness.UpdateIncorrectCallsFromContest(UbnIncorrectCalls);
            }
        }

        private void SetbnxdDTOs( IList<Log> Logs, string ContestId, ContestTypeEnum ContestTypeEnum, CatOperatorEnum CatOperatorEnum, int LogId,
                    string LogPrefix, int CallSignId)
        {
            IList<UbnIncorrectCall> UbnIncorrectCalls = new List<UbnIncorrectCall>();
            IList<UbnNotInLog> UbnNotInLogs = new List<UbnNotInLog>();
            IList<UbnDupe> UbnDupes = new List<UbnDupe>();
            IList<UbnIncorrectExchange> UbnIncorrectExchanges = new List<UbnIncorrectExchange>();


            //find bad call pass 2 with corrections from Pass 1 above
            IBusiness.GetBadCallsNils(Logs, ContestId, LogId, CallSignId, true, ref UbnIncorrectCalls, ref UbnNotInLogs, ref UbnDupes);

            //check for callsigns in my log with no prefix (Country)
            IBusiness.GetBadQsosNoCountry(ContestId, LogId, ref UbnIncorrectCalls);


            IBusiness.GetDupesFromMyLog(ContestId, LogId, ref UbnIncorrectCalls, ref UbnNotInLogs, ref UbnDupes);


            IBusiness.GetBadXchgsFromMyLog(ContestId, ContestTypeEnum, CatOperatorEnum, LogId, 
                ref UbnIncorrectCalls, ref UbnNotInLogs, ref UbnDupes, ref UbnIncorrectExchanges);

            if (UbnIncorrectCalls.Count > 0)
            {//store in DB
                IBusiness.UpdateIncorrectCallsFromContest(UbnIncorrectCalls);
            }

            if (UbnNotInLogs.Count > 0)
            {//store in DB
                IBusiness.UpdateNilsFromContest(UbnNotInLogs);
            }

            if (UbnDupes.Count > 0)
            {//store in DB
                IBusiness.UpdateDupesFromContest(UbnDupes);
            }

            if (UbnIncorrectExchanges.Count > 0)
            {//store in DB
                IBusiness.UpdateIncorrectExchangesFromContest(UbnIncorrectExchanges);
            }



//return;

        }



        private void SetuDTOs( string ContestId, int LogId)
        {

            IList<UbnUnique> UbnUniques = new List<UbnUnique>();

            IList<UbnIncorrectCall> UbnIncorrectCalls = IBusiness.GetUbnIncorrectCalls(LogId);
            IList<UbnNotInLog> UbnNotInLogs = IBusiness.GetUbnNotInLogs(LogId);
            IList<UbnDupe> UbnDupes = IBusiness.GetUbnDupes(LogId);
            //IList<UbnIncorrectExchange> UbnIncorrectExchanges = IBusiness.GetUbnIncorrectExchanges(LogId);


            //Uniques
            IBusiness.GetUniquesFromContest(ContestId, LogId, ref UbnUniques,
                 ref  UbnIncorrectCalls, ref  UbnNotInLogs, ref  UbnDupes );
 //return;

            if (UbnUniques.Count > 0)
            {//store in DB
                IBusiness.UpdateUniquesFromContest(UbnUniques);
            }
        }


        private void SetutoBadSameCountryDTOs(IList<Log> Logs, string ContestId, int LogId)
        {

            IList<UbnIncorrectCall> UbnIncorrectCalls = IBusiness.GetUbnIncorrectCalls(LogId);

            
            //Check Uniques
           IBusiness.GetUniquesForBadCallCheckSameCountryFromContest(Logs, ContestId, LogId, ref  UbnIncorrectCalls);
            //return;

        }

        private void SetutoBadDifferentCountryDTOs(IList<Log> Logs, string ContestId, int LogId)
        {

            IList<UbnIncorrectCall> UbnIncorrectCalls = IBusiness.GetUbnIncorrectCalls(LogId);


            //Check Uniques
            IBusiness.GetUniquesForBadCallCheckForDifferrentCountryFromContest(Logs, ContestId, LogId, ref  UbnIncorrectCalls);
            //return;

        }



    }
}
