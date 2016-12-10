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
    public class ProcessStub
    {
        public IBusiness IBusiness;
        public CtyLib.CCtyObj CtyObj { get; set; }

        protected string LogFileDirectory { get; set; }
        private InputLog InputLog { get; set; }
        ContestTypeEnum ContestTypeEnum;
        private string SqlServerInstance { get; set; }
        private string SqlDatabase { get; set; }
        private string SqlQsoTable { get; set; }


        public ProcessStub(string LogFileDirectory, 
            string SqlServerInstance, string SqlDatabase, string SqlQsoTable)
        {
            this.LogFileDirectory = LogFileDirectory;
            this.SqlServerInstance = SqlServerInstance;
            this.SqlDatabase = SqlDatabase;
            this.SqlQsoTable = SqlQsoTable;


        }


        public bool StubFixup(BackgroundWorker worker)
        {
            bool result = true;
            IBusiness = new Business();
            DirectoryInfo di = new DirectoryInfo(LogFileDirectory);
            IList<CallSign> ICurrentCallSigns;

            string ContestId = di.Name.ToUpper();

            ContestTypeEnum ContestTypeEnum;
            CatOperatorEnum CatOperatorEnum;
            var Contest = IBusiness.GetContest(ContestId);
            ContestTypeEnum = Contest.ContestTypeEnum;

           if (LogFileDirectory.ToLower().Contains("cqww") )
	        {
		        ContestTypeEnum =Logqso.mvc.common.Enum.ContestTypeEnum.CQWW;
	        }else if (LogFileDirectory.ToLower().Contains("cqwpx") )
	        {
                ContestTypeEnum = Logqso.mvc.common.Enum.ContestTypeEnum.CQWPX;
            }
            else if (LogFileDirectory.ToLower().Contains("cq160"))
            {
                ContestTypeEnum = Logqso.mvc.common.Enum.ContestTypeEnum.CQ160;
            }
            if (LogFileDirectory.ToLower().Contains("russiandx"))
            {
                ContestTypeEnum = Logqso.mvc.common.Enum.ContestTypeEnum.RUSDXC;
            }
#if false
            //fixup JA1DCK and  calls containg W1AA??????
            //JHOKHR CQWWSSB2015 BAD QSOS LIKE W1? ? ?....
            result = IBusiness.FixupBadCallSignsContainingString(ContestId, "?");
#endif

#if false
            //refresh list
            ICurrentCallSigns = IBusiness.GetAllCallsigns();
            int count = 1;

            //fixup bad WPX Sent exchanges
            FileInfo[] rgFiles = di.GetFiles("*.log");

            foreach (var item in rgFiles)
            {
                Log Log = null;
                //IList<Log> Logs = null;
                LogCategory LogCategory = null;

                //Create Cabrillo base
                CabrilloLTagInfos CabInfo;
                CabInfo = new CabrilloLTagInfos();
                StreamReader TxtStreambase = new StreamReader(item.FullName);
                PeekingStreamReader PeekingStreamReader = new PeekingStreamReader(TxtStreambase.BaseStream);

                if (PeekingStreamReader != null)
                {
                    using (PeekingStreamReader)
                    {
                        try
                        {

                            ProcessUtils.GetCabrilloInfo(PeekingStreamReader, ContestTypeEnum, CabInfo);

                            worker.ReportProgress(1, new InputLog(item.Name, item.Length, count++, DateTime.Now.ToString("HH:mm:ss")));
                            CallSign CallSign = ICurrentCallSigns.Where(c => c.Call == CabInfo.Callsign).SingleOrDefault();
                            if (CallSign != null)
                            {
                                Log = IBusiness.GetLog(ContestId, CallSign.CallSignId); 
                            }
                            LogCategory = IBusiness.GetLogCategory(Log.LogCategoryId);

                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(string.Format(" Problem in StubFixup() Log: {0} message {1}"),Log.CallsignId, ex.Message);
                            throw;
                        }

                        //GetQSOInfo(PeekingStreamReader, Log, Log.LogCategory, ContestTypeEnum);


                    }

                }
            }


            //FixupWPXSentExchange(string ContestId)
#endif
            return result;
        }

    }
}
