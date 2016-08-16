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
            var Contest = IBusiness.GetContest(ContestId);
            ContestTypeEnum = Contest.ContestTypeEnum;

            IList<Log> Logs = IBusiness.GetAllLogsWithCallsign(ContestId);      //includes CallSign , logcategory


            foreach (var log in Logs)
            {
                IList<QsoAddPoinsMultsDTO> QsoAddPoinsMultsDTOs = IBusiness.GetQsoPointsMults(log.LogId);  
                //CallSign CallSign = IBusiness.GetCallSign(log.CallsignId);
                worker.ReportProgress(1, new InputLog(log.CallSign.Call, QsoAddPoinsMultsDTOs.Count));
                if (QsoAddPoinsMultsDTOs.Count != 0)
                {

                    SetUbnxdDTOs(QsoAddPoinsMultsDTOs, Contest.ContestId, ContestTypeEnum, log.LogId, log.CallSign.Prefix, log.CallSign.CallSignId);
                    break;
                    //IBusiness.UpdateQsoUbnxds(QsoUpdateUbnxdDTOs);
                }

                QsoUpdateUbnxdDTO QsoUpdateUbnxdDTO = new QsoUpdateUbnxdDTO();
 
                //Uniques: get all calls inlg that only occur once in Db using contestid


            }

            return result;

        }


        private void SetUbnxdDTOs(IList<QsoAddPoinsMultsDTO> Qsos, string ContestId, ContestTypeEnum ContestTypeEnum, int LogId, string LogPrefix, int CallSignId)
        {
            //get all callsigns for this log
            //IList<CallSign> Callsigns = IBusiness.GetCallSignsFromLog(LogId);

            //IList<QsoUpdateUbnxdDTO> QsoUpdateUbnxdDTOs = new List<QsoUpdateUbnxdDTO>();
            // get allworked QSOs from worked contest log

            //UNIQUES
            QsoUpdateUniquNilDupeDTOCollextion QsoUpdateUniquNilDupeDTOCollextion = new QsoUpdateUniquNilDupeDTOCollextion();

            IList<UbnIncorrectCall> UbnIncorrectCalls = null;
            IList<UbnNotInLog> UbnNotInLogs = null;
            IList<UbnDupe> UbnDupes = null;
            IBusiness.GetBadCallsNils(ContestId, LogId, CallSignId, out UbnIncorrectCalls, out UbnNotInLogs, out UbnDupes);
UbnIncorrectCalls = UbnIncorrectCalls.OrderBy(x => x.QsoNo).ToList();
return;
           
            IList<UbnUnique> UbnUniques = new List<UbnUnique>();
            IList<short> UniqieCallsignIDs = IBusiness.GetUniquesFromContest(ContestId, LogId);

            //(UniqieCallsignIDs as List<int>).Sort();
            foreach (var item in UniqieCallsignIDs)
            {
                UbnUnique UbnUnique = new UbnUnique()
                {
                    LogId = LogId,
                    QsoNo = item,
                    EntityState = EntityState.Added
                };
                UbnUniques.Add(UbnUnique);
            }

            if (QsoUpdateUniquNilDupeDTOCollextion.Count > 0)
            {//store in DB
                IBusiness.UpdateUniquesFromContest(UbnUniques);
            }


            //Not In Log
            


        }


    }
}
