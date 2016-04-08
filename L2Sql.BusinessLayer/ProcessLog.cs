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
using Logqso.mvc.common.Enum;



namespace L2Sql.BusinessLayer
{
    public class ProcessLogs
    {
        public IBusiness IBusiness;
        public CtyLib.CCtyObj CtyObj { get; set; }

        protected string CtyFile {get; set;}
        protected string LogFileDirectory { get; set; }
        private InputLog InputLog { get; set; }
        ContestTypeEnum ContestTypeEnum;
        private string SqlServerInstance { get; set; }
        private string SqlDatabase { get; set; }
        private string SqlQsoTable { get; set; }

        public ProcessLogs(string CtyFile, string LogFileDirectory, 
            string SqlServerInstance, string SqlDatabase, string SqlQsoTable)
        {
            this.CtyFile = CtyFile;
            this.LogFileDirectory = LogFileDirectory;
            this.SqlServerInstance = SqlServerInstance;
            this.SqlDatabase = SqlDatabase;
            this.SqlQsoTable = SqlQsoTable;


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

        }

        public bool CallsToDatabase(BackgroundWorker worker)
        {
            bool result = true;
            try
            {
                IBusiness = new Business();
                DirectoryInfo di = new DirectoryInfo(LogFileDirectory);
                IList<CallSign> ICurrentCallSigns;

                //create cty file
                CtyObj = new CtyLib.CCtyObj(CtyLib.CCtyObj.DatType.CtyDat, CtyFile);
                CtyObj.Load();
                string ContestId = di.Name.ToUpper();
                DateTime ContestYear;
                if (DateTime.TryParse("1,1," + di.Name.Substring(di.Name.Length - 4, 4), out ContestYear) == false)
                {
                    return false;
                }

                FileInfo[] rgFiles = di.GetFiles("*.log");

                //go through all logs and capture the callsigns
                //  sae changes after every log is orocessed.
                foreach (var item in rgFiles)
                {
                    //worker.ReportProgress(1,new InputLog(item.Name, item.Length) );
                   //get Callsigns already in DB
                    ICurrentCallSigns = IBusiness.GetAllCallsigns();
                    IList<CallSign> INewCallSigns = new List<CallSign>();
                    StreamReader TxtStream;
                    TxtStream = new StreamReader(item.FullName);
                    GetCabrilloCallsignInfo(TxtStream, ICurrentCallSigns, INewCallSigns);
                    //save all new callsigns
                    if (INewCallSigns.Count > 0)
                    {
                      worker.ReportProgress(1,new InputLog(item.Name, INewCallSigns.Count));
                      IBusiness.AddCallSign(INewCallSigns.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }


            return result;
        }


        public bool LogsToDatabase(BackgroundWorker worker)
        {
            bool result = true;
            try
            {
                IBusiness = new Business();
                DirectoryInfo di = new DirectoryInfo(LogFileDirectory);
                QsoModeTypeEnum QsoModeTypeEnum;
                IList<CallSign> ICurrentCallSigns;

                //create cty file
                CtyObj = new CtyLib.CCtyObj(CtyLib.CCtyObj.DatType.CtyDat, CtyFile);
                CtyObj.Load();
                string ContestId = di.Name.ToUpper();
                DateTime ContestYear;
                if (DateTime.TryParse("1,1," + di.Name.Substring(di.Name.Length - 4, 4), out ContestYear) == false)
                {
                    return false;
                }

                FileInfo[] rgFiles = di.GetFiles("*.log");

                //Create LogBase

                //get LogCategory
                IList<LogCategory> LogCategorys = IBusiness.GetAllLogCategorys();


                //refresh list
                ICurrentCallSigns = IBusiness.GetAllCallsigns();

                foreach (var item in rgFiles)
                {
                    Log Log = null;
                    //IList<Log> Logs = null;

                    //logCategory
                    LogCategory LogCategory = new DomainModel.LogCategory()
                        {
                            EntityState = EntityState.Added
                        };

                    //Create Cabrillo base
                    CabrilloLTagInfos CabInfo;
                    CabInfo = new CabrilloLTagInfos();
                    StreamReader TxtStream;
                    TxtStream = new StreamReader(item.FullName);
                    GetCabrilloInfo(TxtStream, LogCategory, CabInfo);

                    worker.ReportProgress(1, new InputLog(item.Name, item.Length));
                    CallSign CallSign = ICurrentCallSigns.Where(c => c.Call == CabInfo.Callsign).SingleOrDefault();
                    if (CallSign != null)
                    {
                        Log = IBusiness.GetLog(ContestId, CallSign.CallSignId);;
                    }
                    if (Log == null)
                    {

                        Log = new Log()
                        {
                            ContestYear = ContestYear,
                            ContestId = ContestId,
                            QsoDatabaseServerInstance = SqlServerInstance,
                            QsoDatabaseInstance = SqlDatabase,
                            QsoDatabaseTableName = SqlQsoTable,
                            EntityState = EntityState.Added
                        };

                        if (CabInfo.CallTxZone != 0  )
                        {//only set if contest has zone
                            Log.QsoExchangeNumber = CabInfo.CallTxZone;
                        }


                        SetLogCategory(LogCategory, out QsoModeTypeEnum, CabInfo);
                    
                        //find LogCategory, if it exists
                        LogCategory DBLogCategory = LogCategorys.Where(
                            l => l.CatAssistedEnum == LogCategory.CatAssistedEnum &&
                             l.CatBandEnum == LogCategory.CatBandEnum &&
                             l.CatNoOfTxEnum == LogCategory.CatNoOfTxEnum &&
                             l.CatOperatorEnum == LogCategory.CatOperatorEnum &&
                             l.CatOperatorOverlayEnum == LogCategory.CatOperatorOverlayEnum &&
                             l.CatPowerEnum == LogCategory.CatPowerEnum
                           ).SingleOrDefault();

                        if (DBLogCategory == null)
                        {//new entry update DB
                            IBusiness.AddLogCategory(LogCategory);
                            Log.LogCategoryId = LogCategory.LogCategoryId;
                        }
                        else
                        {
                            Log.LogCategoryId = DBLogCategory.LogCategoryId;
                        }



                        //check if the log callsign already exists in DB
                        CallSign = ICurrentCallSigns.Where(c => c.Call == CabInfo.Callsign).SingleOrDefault();
                        if (CallSign == null)
                        {
                            CallSign = new CallSign
                            {
                                Call = CabInfo.Callsign,
                                Accuracy = (short)googleutils.Geo.GAccuracyCode.G_Null,
                                Continent = (int)GetContinentEnum(CabInfo.Callsign),
                                EntityState = EntityState.Added
                            };
                            //need to assign CallsignId after saveChanges
                           // Log.CallSign = CallSign;
                            //add to cached list
                           //IList<CallSign> INewCallSigns = new List<CallSign>();

                           // INewCallSigns.Add(CallSign);
                           // IBusiness.AddCallSign(INewCallSigns.ToArray());
                            IBusiness.AddCallSign(CallSign);
                            //is this required
                            ICurrentCallSigns = IBusiness.GetAllCallsigns();

                            CallSign = ICurrentCallSigns.Where(c => c.Call == CabInfo.Callsign).SingleOrDefault();
                        }
                        Log.CallsignId = CallSign.CallSignId;
                    
                        //CabrilloInfo
                        CabrilloInfo CabrilloInfo = null;
                        CabrilloInfo = IBusiness.GetCabrilloInfo(ContestId, CallSign.CallSignId);
                        if (CabrilloInfo == null)
                        {// not in DB
                            CabrilloInfo = new CabrilloInfo
                            {
                                CallSignId = CallSign.CallSignId,
                                ContestId = ContestId,
                                ClaimedScore = CabInfo.ClaimedScore,
                                Club = CabInfo.Club,
                                Operators = CabInfo.Operators,
                                EntityState = EntityState.Added
                            };
                            IBusiness.AddCabrilloInfo(CabrilloInfo);

                        }
                    }

                    //Update Log


                    //get QSos
                    IList<CallSign> INewCallSigns = new List<CallSign>();

                    //Save Qsos;
                }
            }
            catch (Exception ex)
            {
                
                throw;
            }


            return result;
        }

        private ContinentEnum GetContinentEnum(string Call)
        {
            ContinentEnum ContinentEnum = Logqso.mvc.common.Enum.ContinentEnum.ALL;
            string continent = CtyObj.GetContinent(Call);
            Enum.TryParse(continent, out ContinentEnum);
            return ContinentEnum;
        }

        private bool GetCabrilloCallsignInfo(StreamReader TxtStream, IList<CallSign> ICurrentCallsigns, IList<CallSign> INewCallsigns)
        {
            bool bOK = true;
            string FoundCall;
            CallSign CallSign;
            try
            {
                if (TxtStream != null)
                {
                    using (TxtStream)
                    {
                        while (TxtStream.Peek() >= 0)
                        {
                            FoundCall = string.Empty;
                            string line = TxtStream.ReadLine();
                            if (line.Contains("QSO:"))
                            {
                                if (line.Contains("\t"))
                                {
                                    line = line.Replace('\t', ' ');
                                }
                                string[] Qcolumns = line.Split(" ".ToCharArray());
                                int i=0;
                                int j=0;
                                string[] QValidcolumns= new string[Qcolumns.Length] ;
                                while (i < Qcolumns.Length)
                                {
                                    if (Qcolumns[i] != "")
                                    {
                                        QValidcolumns[j++] = Qcolumns[i];
                                    }
                                    i++;
                                }
                                switch (ContestTypeEnum)
                                {
                                    case ContestTypeEnum.CQWW:
                                        FoundCall = QValidcolumns[8].Trim().ToUpper();
                                        break;
                                    case ContestTypeEnum.CQWPX:
                                        FoundCall = QValidcolumns[8].Trim().ToUpper();
                                       break;
                                    case ContestTypeEnum.CQ160:
                                       FoundCall = QValidcolumns[8].Trim().ToUpper();
                                        break;
                                    case ContestTypeEnum.RUSDXC:
                                        FoundCall = QValidcolumns[8].Trim().ToUpper();
                                       break;
                                    default:
                                        break;
                                }
                            }
                            else
                            {
                                string[] columns = line.Split(":".ToCharArray());

                                switch (columns[0].ToUpper())
                                {
                                    case "CALLSIGN":
                                        FoundCall = columns[1].Trim().ToUpper();
                                        break;
                                    default:
                                        break;
                                }
                            }
                            if (FoundCall != string.Empty)
                            {
                                //check if new call
                                CallSign = ICurrentCallsigns.Where(c => c.Call == FoundCall).SingleOrDefault();
                                if (CallSign == null)
                                {//it maybe a new call
                                    //check if it is new call already, ie. A qsp on a different band.
                                    CallSign = INewCallsigns.Where(c => c.Call == FoundCall).SingleOrDefault();
                                    if (CallSign == null)
                                    {//it is a new call
                                        CallSign = new CallSign
                                        {
                                            Call = FoundCall,
                                            Accuracy = (short)googleutils.Geo.GAccuracyCode.G_Null,
                                            Continent = (int)GetContinentEnum(FoundCall),
                                            EntityState = EntityState.Added
                                        };
                                        //add to new list
                                        INewCallsigns.Add(CallSign);
                                    }
                                }
                            }
                        }

                    }
                }
            }
            catch (ArgumentNullException ex)
            {
                //MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
            }

            return bOK;

        }


        private bool GetCabrilloInfo(StreamReader TxtStream, LogCategory LogCategory, CabrilloLTagInfos CInfo)
        {
            bool bOK = true;
            bool Version3 = true;
            try
            {
                if (TxtStream != null )
                {
                    using (TxtStream)
                    {
                        string line = TxtStream.ReadLine();
                        string[] columns = line.Split();
                        //if (line.Contains("BH4RNX") == true)
                        //{
                        //    var vale = 5;
                        //}
                        //get cabrillo version
                        if (columns.Length == 2)
                        {
                            if (columns[1].Contains("3.0"))
                            {
                                Version3 = true;
                            }
                            else
                            {
                                Version3 = false;  //version 2
                            }
                        }

                        if (TxtStream.BaseStream.CanSeek == true)
                        {
                            long posseek;
                            posseek = TxtStream.BaseStream.Seek(0, SeekOrigin.Begin); //rewind
                            TxtStream.DiscardBufferedData();
                        }

                        while (TxtStream.Peek() >= 0)
                        {
                            line = TxtStream.ReadLine();
                            if (line.Contains("QSO:") )
                            {
                                switch (ContestTypeEnum)
                                {
                                    case ContestTypeEnum.CQWW:
                                        CabrilloQSOCQWWTagInfos QsoInfo = new CabrilloQSOCQWWTagInfos();
                                        GetCabrilloCqwwQSOInfo(line, QsoInfo);
                                        CInfo.CallTxZone = QsoInfo.TxZone;
                                        break;
                                    case ContestTypeEnum.CQWPX:
                                        break;
                                    case ContestTypeEnum.CQ160:
                                        break;
                                    case ContestTypeEnum.RUSDXC:
                                        break;
                                    default:
                                        break;
                                }
                                break;
                            }
                            columns = line.Split(":".ToCharArray());

                            if (Version3)
                            {
                                switch (columns[0].ToUpper())
                                {
                                    case "CALLSIGN":
                                        CInfo.Callsign = columns[1].Trim().ToUpper();
                                        break;
                                    case "CATEGORY-ASSISTED":
                                        CInfo.CatAssisted = columns[1].Trim().ToUpper();                                        
                                        break;
                                    case "CATEGORY-BAND":
                                        CInfo.CatBand = columns[1].Trim().ToUpper();
                                        break;
                                    case "CATEGORY-MODE":
                                        CInfo.CatMode = columns[1].Trim().ToUpper();
                                        break;
                                    case "CATEGORY-OPERATOR":
                                        CInfo.CatOperator = columns[1].Trim().ToUpper();
                                        break;
                                    case "CATEGORY-POWER":
                                        CInfo.CatPower = columns[1].Trim().ToUpper();
                                        break;
                                    case "CATEGORY-STATION":
                                        CInfo.CatStation = columns[1].Trim().ToUpper();
                                        break;
                                    case "CATEGORY-TRANSMITTER":
                                        CInfo.CatTx = columns[1].Trim().ToUpper();
                                        break;
                                    case "CATEGORY-OVERLAY":
                                        CInfo.CatOverlay = columns[1].Trim().ToUpper();
                                        break;
                                    case "CLAIMED-SCORE":
                                        try
                                        {
                                            CInfo.ClaimedScore = int.Parse(columns[1].Trim());
                                        }
                                        catch
                                        {
                                            CInfo.ClaimedScore = 0;
                                        }
                                        break;
                                    case "CLUB":
                                        CInfo.Club = columns[1].Trim().ToUpper();
                                        break;
                                    case "CONTEST":
                                        CInfo.Contest = columns[1].Trim().ToUpper();
                                        break;
                                    case "CREATED-BY":
                                        CInfo.CreatedBy = columns[1].Trim().ToUpper();
                                        break;
                                    case "OPERATORS":
                                        if (!string.IsNullOrEmpty(CInfo.Operators))
                                        {
                                            columns[1] = columns[1].Replace(',', ' ');
                                            columns[1] = columns[1].Replace('&', ' ');
                                            CInfo.Operators += ", " + columns[1].Trim().ToUpper();
                                        }
                                        else
                                        {
                                            columns[1] = columns[1].Replace(',', ' ');
                                            columns[1] = columns[1].Replace('&', ' ');
                                            CInfo.Operators += columns[1].Trim().ToUpper();
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                            else   //Version 2
                            {
                                switch (columns[0].ToUpper())
                                {
                                    case "CALLSIGN":
                                        CInfo.Callsign = columns[1].Trim().ToUpper();
                                        break;
                                    case "CATEGORY":

                                        string[] catCol = columns[1].Split(" ".ToCharArray());
                                        int index = 0;
                                        while (catCol[index].Length == 0)
                                        {
                                            index++;
                                        }
                                        switch (catCol[index].Trim().ToUpper())
                                        {
                                            case "MULTI-ONE":
                                                CInfo.CatOperator = "MULTI-OP";
                                                CInfo.CatTx = "ONE";
                                                CInfo.CatAssisted = "ASSISTED";
                                                break;
                                            case "MULTI-TWO":
                                                CInfo.CatOperator = "MULTI-OP";
                                                CInfo.CatTx = "TWO";
                                                CInfo.CatAssisted = "ASSISTED";
                                                break;
                                            case "MULTI-MULTI":
                                                CInfo.CatOperator = "MULTI-OP";
                                                CInfo.CatTx = "UNLIMITED";
                                                CInfo.CatAssisted = "ASSISTED";
                                                break;
                                            case "SINGLE-OP":
                                                CInfo.CatOperator = "SINGLE-OP";
                                                CInfo.CatTx = "ONE";
                                                CInfo.CatAssisted = "NON-ASSISTED";
                                                break;
                                            case "SINGLE-OP-ASSISTED":
                                                CInfo.CatOperator = "SINGLE-OP";
                                                CInfo.CatTx = "ONE";
                                                CInfo.CatAssisted = "ASSISTED";
                                                break;

                                            default:
                                                break;
                                        }
                                        if (!string.IsNullOrEmpty(catCol[index + 1].Trim()))
                                        {
                                            CInfo.CatBand = catCol[index + 1].Trim().ToUpper();
                                        }
                                        if (!string.IsNullOrEmpty(catCol[index + 2].Trim()))
                                        {
                                            CInfo.CatPower = catCol[index + 2].Trim().ToUpper();
                                        }
                                        if (!string.IsNullOrEmpty(catCol[index + 3].Trim()))
                                        {
                                            if (catCol.Length >= 4)
                                            {
                                                CInfo.CatMode = catCol[index + 3].Trim().ToUpper();
                                            }
                                        }
                                        break;
                                    case "CATEGORY-ASSISTED":
                                        CInfo.CatAssisted = columns[1].Trim().ToUpper();
                                        break;
                                    case "CATEGORY-BAND":
                                        CInfo.CatBand = columns[1].Trim().ToUpper();
                                        break;
                                    case "CATEGORY-MODE":
                                        CInfo.CatMode = columns[1].Trim().ToUpper();
                                        break;
                                    case "CATEGORY-OPERATOR":
                                        CInfo.CatOperator = columns[1].Trim().ToUpper();
                                        break;
                                    case "CATEGORY-POWER":
                                        CInfo.CatPower = columns[1].Trim().ToUpper();
                                        break;
                                    case "CATEGORY-STATION":
                                        CInfo.CatStation = columns[1].Trim().ToUpper();
                                        break;
                                    case "CATEGORY-TRANSMITTER":
                                        CInfo.CatTx = columns[1].Trim().ToUpper();
                                        break;
                                    case "CATEGORY-OVERLAY":
                                        CInfo.CatOverlay = columns[1].Trim().ToUpper();
                                        break;
                                    case "CLAIMED-SCORE":
                                        try
                                        {
                                            CInfo.ClaimedScore = int.Parse(columns[1].Trim());
                                        }
                                        catch
                                        {
                                            CInfo.ClaimedScore = 0;
                                        }
                                        break;
                                    case "CLUB":
                                        CInfo.Club = columns[1].Trim().ToUpper();
                                        break;
                                    case "CONTEST":
                                        CInfo.Contest = columns[1].Trim().ToUpper();
                                        break;
                                    case "CREATED-BY":
                                        CInfo.CreatedBy = columns[1].Trim().ToUpper();
                                        break;
                                    case "OPERATORS":
                                        if (!string.IsNullOrEmpty(CInfo.Operators))
                                        {
                                            columns[1] = columns[1].Replace(',', ' ');
                                            columns[1] = columns[1].Replace('&', ' ');
                                            CInfo.Operators += " " + columns[1].Trim().ToUpper();
                                        }
                                        else
                                        {
                                            columns[1] = columns[1].Replace(',', ' ');
                                            columns[1] = columns[1].Replace('&', ' ');
                                            CInfo.Operators += columns[1].Trim().ToUpper();
                                        } break;
                                    default:
                                        break;
                                }
                            }
                        }
                        if (CInfo.Operators == CInfo.Callsign)
                        {
                            CInfo.Operators = "";
                        }
                        if (CInfo.CatOverlay == null)
                        {
                            //CInfo.CatOverlay = "NONE";
                        }
                        if (string.IsNullOrEmpty(CInfo.CatMode))
                        {
                            if (CInfo.Contest.Contains("SSB"))
                            {
                                CInfo.CatMode = "SSB";
                            }
                            else
                            {
                                CInfo.CatMode = "CW";
                            }
                        }

                    }
                }
            }
            catch (ArgumentNullException ex)
            {
                //MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
            }

            return bOK;

        }

        private void SetLogCategory(LogCategory LogCategory, out QsoModeTypeEnum QsoModeTypeEnum, CabrilloLTagInfos CabInfo)
        {
            //ALL HAVE TO BE CAST AS INT BECAUSE OF EF BUG
            //ALL names SHOULD MATCH 
            // LogCategory.CatOperator needs to be LogCategory.CatOperatorEnum
            //http://stackoverflow.com/questions/26692965/no-corresponding-object-layer-type-could-be-found-for-the-conceptual-type
            //http://stackoverflow.com/questions/13527400/entity-framework-5-rtm-code-first-enum-support-broken-enums-in-other-namespaces
            //QsoModeTypeEnum, ContestTypeEnum, QsoRadioTypeEnum are ok to map to enum
            QsoModeTypeEnum = Logqso.mvc.common.Enum.QsoModeTypeEnum.SSB;

            if (CabInfo.CatAssisted == "NON-ASSISTED")
            {
                LogCategory.CatAssistedEnum = CatAssistedEnum.NON_ASSISTED;
            }
            else
            {
                LogCategory.CatAssistedEnum = CatAssistedEnum.ASSISTED;
            }

            switch (CabInfo.CatBand)
            {
                case "10M":
                    LogCategory.CatBandEnum = CatBandEnum._10M;
                    break;
                case "15M":
                    LogCategory.CatBandEnum = CatBandEnum._15M;
                    break;
                case "20M":
                    LogCategory.CatBandEnum = CatBandEnum._20M;
                    break;
                case "40M":
                    LogCategory.CatBandEnum = CatBandEnum._40M;
                    break;
                case "80M":
                    LogCategory.CatBandEnum = CatBandEnum._80M;
                    break;
                case "160M":
                    LogCategory.CatBandEnum = CatBandEnum._160M;
                    break;
                case "ALL":
                    LogCategory.CatBandEnum = CatBandEnum.ALL;
                    break;
                default:
                    break;
            }

            switch (CabInfo.CatMode)
            {
                case "SSB":
                    QsoModeTypeEnum = QsoModeTypeEnum.SSB;
                    break;
                case "CW":
                    QsoModeTypeEnum = QsoModeTypeEnum.CW;
                    break;
                case "RTTY":
                    QsoModeTypeEnum = QsoModeTypeEnum.RTTY;
                    break;
                case "MIXED":
                    QsoModeTypeEnum = QsoModeTypeEnum.MIXED;
                    break;
                default:
                    break;
            }


            switch (CabInfo.CatOperator)
            {
                case "SINGLE-OP":
                    LogCategory.CatOperatorEnum = CatOperatorEnum.SINGLE_OP;
                    break;
                case "MULTI-OP":
                    LogCategory.CatOperatorEnum = CatOperatorEnum.MULTI_OP;
                    break;
                case "CHECKLOG":
                    LogCategory.CatOperatorEnum = CatOperatorEnum.CHECKLOG;
                    break;
                default:
                    break;
            }


            switch (CabInfo.CatPower)
            {
                case "HIGH":
                    LogCategory.CatPowerEnum = CatPowerEnum.HIGH;
                    break;
                case "LOW":
                    LogCategory.CatPowerEnum = CatPowerEnum.LOW;
                    break;
                case "QRP":
                    LogCategory.CatPowerEnum = CatPowerEnum.QRP;
                    break;
                default:
                    break;
            }

            if (!string.IsNullOrEmpty(CabInfo.CatOverlay) )
            {
                switch (CabInfo.CatOverlay)
                {
                    case "SINGLE_OP_CLASSIC":
                        LogCategory.CatOperatorOverlayEnum = CatOperatorOverlayEnum.SINGLE_OP_CLASSIC;
                        break;
                    case "SINGLE_OP_ROOKIE":
                        LogCategory.CatOperatorOverlayEnum = CatOperatorOverlayEnum.SINGLE_OP_ROOKIE;
                        break;
                    case "NONE":
                        LogCategory.CatOperatorOverlayEnum = CatOperatorOverlayEnum.NONE;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                LogCategory.CatOperatorOverlayEnum = CatOperatorOverlayEnum.NONE;
            }


            switch (CabInfo.CatTx)
            {
                case "ONE":
                    LogCategory.CatNoOfTxEnum = CatNoOfTxEnum.ONE;
                    break;
                case "TWO":
                    LogCategory.CatNoOfTxEnum = CatNoOfTxEnum.TWO;
                    break;
                case "UNLIMITED":
                    LogCategory.CatNoOfTxEnum = CatNoOfTxEnum.UNLIMITED;
                    break;
                default:
                    break;
            }



        }




        private void GetCabrilloCqwwQSOInfo(string line, CabrilloQSOCQWWTagInfos QsoInfo)
        {
            if (line.Contains("\t"))
            {
                line = line.Replace('\t', ' ');
            }
            string[] Qcolumns = line.Split(" ".ToCharArray());
            int i = 1;
            int j = 1; //ccqww field counter
            QsoInfo.Radio = 0;
            while (i < Qcolumns.Length)
            {
                if (Qcolumns[i] != "")
                {
                    switch (j)
                    {
                        case 1:
                            try
                            {
                                QsoInfo.QFreq = int.Parse(Qcolumns[i]);
                            }
                            catch
                            {
                            }
                            break;
                        case 2:
                            QsoInfo.Mode = Qcolumns[i];
                            break;
                        case 3:
                            QsoInfo.QDate = Qcolumns[i];
                            break;
                        case 4:
                            QsoInfo.QTime = Qcolumns[i];
                            break;
                        case 5:
                            QsoInfo.Call = Qcolumns[i];
                            break;
                        case 6:
                            try
                            {
                                QsoInfo.TxRpt = short.Parse(Qcolumns[i]);
                            }
                            catch
                            {
                            }
                            break;
                        case 7:
                            try
                            {
                                QsoInfo.TxZone = byte.Parse(Qcolumns[i]);
                            }
                            catch
                            {
                            }
                            break;
                        case 8:
                            QsoInfo.QCall = Qcolumns[i];
                            break;
                        case 9:
                            try
                            {
                                QsoInfo.QRpt = short.Parse(Qcolumns[i]);
                            }
                            catch
                            {
                            }
                            break;
                        case 10:
                            //zone I4
                            Qcolumns[i] = Qcolumns[i].Replace('I', '1'); //bad zone I4
                            try
                            {
                                QsoInfo.QZone = byte.Parse(Qcolumns[i]);
                            }
                            catch
                            {
                                QsoInfo.QZone = 0xff;
                            }
                            break;
                        case 11:
                            //radio
                            try
                            {
                                QsoInfo.Radio = byte.Parse(Qcolumns[i]);
                            }
                            catch
                            {
                                QsoInfo.Radio = 0;
                            }
                            break;
                        default:
                            break;
                    }
                    j++;
                }
                i++;
            }



        }


    
    }
}
