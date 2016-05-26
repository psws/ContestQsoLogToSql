﻿using System;
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
using System.Diagnostics;



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
                IList<CallSign> INewCallSigns = new List<CallSign>();

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
                    StreamReader TxtStreambase = new StreamReader(item.FullName);
                    PeekingStreamReader PeekingStreamReader = new PeekingStreamReader(TxtStreambase.BaseStream);

                    GetCabrilloCallsignInfo(PeekingStreamReader, ICurrentCallSigns, INewCallSigns, CtyObj);
                    //save all new callsigns
                    if (INewCallSigns.Count > 0)
                    {
                        CallSign[] CallSigns;
                        worker.ReportProgress(1,new InputLog(item.Name, INewCallSigns.Count));
                        CallSigns = INewCallSigns.ToArray();
                        IBusiness.AddCallSign(CallSigns);
                    }
                    ICurrentCallSigns.Clear();
                    INewCallSigns.Clear();
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
                    LogCategory LogCategory = null;

                    //Create Cabrillo base
                    CabrilloLTagInfos CabInfo;
                    CabInfo = new CabrilloLTagInfos();
                    StreamReader TxtStreambase = new StreamReader(item.FullName);
                    PeekingStreamReader PeekingStreamReader = new PeekingStreamReader(TxtStreambase.BaseStream);
                    //if (item.FullName.Contains("ai4co"))
                    //{

                    //}   
                    if (PeekingStreamReader != null)
                    {
                        using (PeekingStreamReader)
                        {
                            try
                            {

                           GetCabrilloInfo(PeekingStreamReader, CabInfo);

                            worker.ReportProgress(1, new InputLog(item.Name, item.Length));
                            CallSign CallSign = ICurrentCallSigns.Where(c => c.Call == CabInfo.Callsign).SingleOrDefault();
                            if (CallSign != null)
                            {
                                Log = IBusiness.GetLog(ContestId, CallSign.CallSignId); ;
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

                                if (CabInfo.CallTxZone != 0)
                                {//only set if contest has zone
                                    Log.QsoExchangeNumber = CabInfo.CallTxZone;
                                }
                                //logCategory
                                LogCategory = new DomainModel.LogCategory();

                                SetLogCategory(LogCategory, out QsoModeTypeEnum, CabInfo);

                                //find LogCategory, if it exists
                                LogCategory DBLogCategory;
                                try
                                {
                                    DBLogCategory = LogCategorys.Where(
                                        l => l.CatAssistedEnum == LogCategory.CatAssistedEnum &&
                                         l.CatBandEnum == LogCategory.CatBandEnum &&
                                         l.CatNoOfTxEnum == LogCategory.CatNoOfTxEnum &&
                                         l.CatOperatorEnum == LogCategory.CatOperatorEnum &&
                                         l.CatOperatorOverlayEnum == LogCategory.CatOperatorOverlayEnum &&
                                         l.CatPowerEnum == LogCategory.CatPowerEnum
                                       ).SingleOrDefault();
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine(string.Format(" Problem in LogsToDatabase() LogCategorys: {0}"), ex.Message);
                                    throw;
                                }

                                try
                                {
                                    if (DBLogCategory == null)
                                    {//new entry update DB
                                        LogCategory.EntityState = EntityState.Added;
                                        IBusiness.AddLogCategory(LogCategory);
                                        LogCategorys = IBusiness.GetAllLogCategorys();
                                        Log.LogCategoryId = LogCategory.LogCategoryId;
                                    }
                                    else
                                    {
                                        Log.LogCategoryId = DBLogCategory.LogCategoryId;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine(string.Format(" Problem in LogsToDatabase() DBLogCategory: {0}"), ex.Message);
                                    throw;
                                }



                                //check if the log callsign already exists in DB
                                //CallSign = ICurrentCallSigns.Where(c => c.Call == CabInfo.Callsign).SingleOrDefault();
                                if (CallSign == null)
                                {
                                    CallSign = new CallSign
                                    {
                                        Call = CabInfo.Callsign,
                                        Accuracy = (short)googleutils.Geo.GAccuracyCode.G_Null,
                                        ContinentEnum = GetContinentEnum(CabInfo.Callsign),
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
                                    ICurrentCallSigns.Clear();
                                    ICurrentCallSigns = IBusiness.GetAllCallsigns();

                                    CallSign = ICurrentCallSigns.Where(c => c.Call == CabInfo.Callsign).SingleOrDefault();
                                }
                                Log.CallsignId = CallSign.CallSignId;

                                //Update Log
                                try
                                {
                                    IBusiness.AddLog(Log);
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine(string.Format(" Problem in LogsToDatabase() AddLog: {0} message {1}"),Log.CallsignId, ex.Message);
                                    throw;
                                }
                            }
                            //CabrilloInfo
                            CabrilloInfo CabrilloInfo = null;
                            CabrilloInfo = IBusiness.GetCabrilloInfo(ContestId, Log.CallsignId);
                            try
                            {
                                if (CabrilloInfo == null)
                                {// not in DB
                                    CabrilloInfo = new CabrilloInfo
                                    {
                                        CallSignId = CallSign.CallSignId,
                                        ContestId = ContestId,
                                        ClaimedScore = CabInfo.ClaimedScore,
                                        EntityState = EntityState.Added
                                    };
                                    if (!string.IsNullOrEmpty(CabInfo.Operators ) )
                                    {
                                        if (CabInfo.Operators.Length > 200)
                                        {
                                            CabInfo.Operators = CabInfo.Operators.Substring(1, 200);
                                        }
                                        CabrilloInfo.Operators = CabInfo.Operators;
                                    }
                                     if (!string.IsNullOrEmpty( CabInfo.Club ) )
                                    {
                                        if (CabInfo.Club.Length >20)
                                        {
                                            CabInfo.Club = CabInfo.Club.Substring(1, 20);
                                        }
                                        CabrilloInfo.Club = CabInfo.Club;
                                    }
                                   IBusiness.AddCabrilloInfo(CabrilloInfo);
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(string.Format(" Problem in LogsToDatabase() CabrilloInfo: {0} message {1}"), Log.CallsignId, ex.Message);
                                throw;
                            }


                            //get QSos

                            switch (ContestTypeEnum)
                            {
                                case ContestTypeEnum.CQWW:
                                    try
                                    {
                                        GetCqwwQSOInfo(PeekingStreamReader, Log);
                                    }
                                    catch (Exception)
                                    {
                                        
                                        throw;
                                    }
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
                        }//stream
                        catch (Exception ex)
                        {
                            Debug.WriteLine(string.Format(" Problem in LogsToDatabase() stream: {0}  message {1}"),item.FullName,  ex.Message);
                            throw;
                        }
                    }//stream 

                }//stream null
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

        private bool GetCabrilloCallsignInfo(PeekingStreamReader PeekingStreamReader, IList<CallSign> ICurrentCallsigns, IList<CallSign> INewCallsigns, CtyLib.CCtyObj CtyObj)
        {
            bool bOK = true;
            string FoundCall;
            CallSign CallSign;
            try
            {
                if (PeekingStreamReader != null)
                {
                    using (PeekingStreamReader)
                    {
                        while (PeekingStreamReader.Peek() >= 0)
                        {
                            FoundCall = string.Empty;
                            //No queing
                            string line = PeekingStreamReader.ReadLine();
                            if (line .Length >=6 && line.Substring(0,5).Contains("QSO:"))
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
                                if (QValidcolumns.Length >= 9)
                                {//QSO could be in comment field
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
                            }
                            else if (line.Length >=11 && line.Substring(0, 10).Contains("CALLSIGN:"))
                            {
#if DEBUG
                                Debug.WriteLine(line);
#endif
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
                                        string Prefix = CtyObj.GetCountryPrefix(FoundCall);
                                        if (string.IsNullOrEmpty(Prefix)  )
                                        {
                                            Prefix = "none";
                                        }
                                        CallSign = new CallSign
                                        {
                                            Call = FoundCall,
                                            Accuracy = (short)googleutils.Geo.GAccuracyCode.G_Null,
                                            ContinentEnum = GetContinentEnum(FoundCall),
                                            Prefix = Prefix,
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


        private bool GetCabrilloInfo(PeekingStreamReader PeekingStreamReader, CabrilloLTagInfos CInfo)
        {
            bool bOK = true;
            bool Version3 = true;
            try
            {
                if (PeekingStreamReader != null)
                {
                    string line = PeekingStreamReader.PeekReadLine();
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

                    if (PeekingStreamReader.BaseStream.CanSeek == true)
                    {
                        long posseek;
                        posseek = PeekingStreamReader.BaseStream.Seek(0, SeekOrigin.Begin); //rewind
                        PeekingStreamReader.DiscardBufferedData();
                        PeekingStreamReader.ClearQueue();
                    }

                    while (PeekingStreamReader.Peek() >= 0)
                    {
                        line = PeekingStreamReader.PeekReadLine();
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
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format(" Problem in GetCabrilloInfo() log. Call{0} Message {1}"),CInfo.Callsign, ex.Message);
                throw;
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
            try
            {
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
                        LogCategory.CatBandEnum = CatBandEnum.ALL;
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
                        QsoModeTypeEnum = QsoModeTypeEnum.SSB;
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
                        LogCategory.CatOperatorEnum = CatOperatorEnum.SINGLE_OP;
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
                        LogCategory.CatPowerEnum = CatPowerEnum.LOW;
                        break;
                }

                if (!string.IsNullOrEmpty(CabInfo.CatOverlay))
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
                            LogCategory.CatOperatorOverlayEnum = CatOperatorOverlayEnum.NONE;
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
                        LogCategory.CatNoOfTxEnum = CatNoOfTxEnum.ONE;
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format(" Problem in SetLogCategory {0} log ", CabInfo.Callsign));
                throw;
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


        private void GetCqwwQSOInfo(PeekingStreamReader PeekingStreamReader, Log Log)
        {
            string line;
            short QsoNum = 1;
            short NextDBQsoNum = 0;
            bool PartialLog = false;

            IList<CallSign> ICurrentCallSigns;
            IList<Qso> Qsos = new List<Qso>();
            IList<Qso> CurrentQsos;


             try
            {
                if (PeekingStreamReader != null)
                {
                    ICurrentCallSigns = IBusiness.GetAllCallsigns();
                    CurrentQsos = IBusiness.GetQso(Log.LogId);
                    if (CurrentQsos.Count != 0)
	                {//set to last qso in DB
                        NextDBQsoNum = (short)(CurrentQsos.Count + 1);
                        PartialLog = true;
	                }

                    while (PeekingStreamReader.Peek() >= 0)
                    {
                        Qso Qso = null;

                        line = PeekingStreamReader.ReadLine();
                        if (line.Contains("\t"))
                        {
                            line = line.Replace('\t', ' ');
                        }
                        if (line.Length >=6 && line.Substring(0,5).Contains("QSO:"))
                        {
                            if (PartialLog == true && QsoNum < NextDBQsoNum)
                            {
                                continue;  
                            }
                            Qso = new DomainModel.Qso()
                            {
                                LogId = Log.LogId,
                                QsoNo = QsoNum++,
                                QsoRadioTypeEnum = Logqso.mvc.common.Enum.QsoRadioTypeEnum.NONE,
                                EntityState = EntityState.Added
                            };

                            string[] Qcolumns = line.Split(" ".ToCharArray());
                            int i = 1;
                            int j = 1; //ccqww field counter
                            Qso.QsoRadioTypeEnum = QsoRadioTypeEnum.NONE;
                            try
                            {
                                while (i < Qcolumns.Length)
                                {
                                    if ((Qcolumns[i] != "") && (Qcolumns[i] != ":"))
                                    {
                                        switch (j)
                                        {
                                            case 1:
                                                int freq;
                                                if (int.TryParse(Qcolumns[i], out freq))
                                                {
                                                    Qso.Frequency = freq;
                                                }
                                                else
                                                {
                                                    Qso.Frequency = 1;
                                                }
                                                break;
                                            case 2:
                                                try
                                                {
                                                    switch (Qcolumns[i])
                                                    {
                                                        case "PH":
                                                            Qso.QsoModeTypeEnum = Logqso.mvc.common.Enum.QsoModeTypeEnum.SSB;
                                                            break;
                                                        case "CW":
                                                            Qso.QsoModeTypeEnum = Logqso.mvc.common.Enum.QsoModeTypeEnum.CW;
                                                            break;
                                                        case "RY":
                                                            Qso.QsoModeTypeEnum = Logqso.mvc.common.Enum.QsoModeTypeEnum.RTTY;
                                                            break;
                                                        default:
                                                            Qso.QsoModeTypeEnum = Logqso.mvc.common.Enum.QsoModeTypeEnum.MIXED;
                                                            break;
                                                    }
                                                }
                                                catch (Exception)
                                                {
                                                    Debug.WriteLine(string.Format(" bad Mode: {0} for {1} log ",
                                                        (Qcolumns[i]).ToString(), Log.CallSign.Call));
                                                    throw;
                                                }
                                                break;
                                            case 3:
                                                try
                                                {
                                                    DateTime DateTime;

                                                    if (DateTime.TryParse(Qcolumns[i], out DateTime) == true)
                                                    {
                                                        Qso.QsoDateTime = DateTime;
                                                    }
                                                    else
                                                    {
                                                        Qso.QsoDateTime = DateTime.Now;
                                                        Debug.WriteLine(string.Format(" bad Date: {0} for {1} log ",
                                                            (Qcolumns[i]).ToString(), Log.CallSign.Call));
                                                    }

                                                }
                                                catch (Exception)
                                                {
                                                    Debug.WriteLine(string.Format(" bad Date: {0} for {1} log ",
                                                        (Qcolumns[i]).ToString(), Log.CallSign.Call));
                                                    throw;
                                                }
                                                break;
                                            case 4:
                                                try
                                                {
                                                    DateTime DateTime;
                                                    string Date = Qso.QsoDateTime.ToShortDateString();
                                                    if (DateTime.TryParse(Date + " " + Qcolumns[i].Substring(0, 2) + ":" + Qcolumns[i].Substring(2, 2),
                                                        out DateTime) == true)
                                                    {
                                                        Qso.QsoDateTime = DateTime;
                                                    }
                                                    else
                                                    {
                                                        Debug.WriteLine(string.Format(" bad DateTime: {0} for {1} log ", DateTime,
                                                            (Qcolumns[i] + " " + Qcolumns[i + 1]).ToString(), Log.CallSign.Call));
                                                    }
                                                }
                                                catch (Exception)
                                                {
                                                    Debug.WriteLine(string.Format(" bad Time: {0} for {1} log ",
                                                        (Qcolumns[i]).ToString(), Log.CallSign.Call));
                                                    throw;
                                                }
                                                break;
                                            case 5:
                                                // station call is alrady in the log
                                                break;
                                            case 6:
                                                try
                                                {
                                                    short txrpt;
                                                    if (short.TryParse(Qcolumns[i], out txrpt))
                                                    {
                                                        Qso.TxRst = txrpt;
                                                    }
                                                    else
                                                    {
                                                        Qso.TxRst = 0xff;
                                                    }
                                                }
                                                catch
                                                {
                                                    Debug.WriteLine(string.Format(" bad TxRst: {0} for {1} log ",
                                                        (Qcolumns[i]).ToString(), Log.CallSign.Call));
                                                    throw;
                                                }
                                                break;
                                            case 7:
                                                //try
                                                //{
                                                //    Qso.QsoExchangeNumber = short.Parse(Qcolumns[i]);
                                                //}
                                                //catch
                                                //{
                                                //    Debug.WriteLine(string.Format(" bad QsoExchangeNumber: {0} for {1} log ",
                                                //        (Qcolumns[i]).ToString(), Log.CallSign.Call));
                                                //    throw;
                                                //}
                                                break;
                                            case 8:
                                                try
                                                {
                                                    if (Qcolumns[i].Contains("OH1LWZ/m"))
                                                    {

                                                    }
                                                    CallSign CallSign = ICurrentCallSigns.Where(c => c.Call == Qcolumns[i].ToUpper()).SingleOrDefault();
                                                    if (CallSign == null)
                                                    {//new call
                                                        CallSign = new CallSign
                                                        {
                                                            Call = Qcolumns[i],
                                                            Accuracy = (short)googleutils.Geo.GAccuracyCode.G_Null,
                                                            ContinentEnum = GetContinentEnum(Qcolumns[i]),
                                                            EntityState = EntityState.Added
                                                        };
                                                        //add to new list
                                                        ICurrentCallSigns.Add(CallSign);
                                                        //refresh
                                                        ICurrentCallSigns = IBusiness.GetAllCallsigns();
                                                        CallSign = ICurrentCallSigns.Where(c => c.Call == Qcolumns[i]).SingleOrDefault();
                                                    }
                                                    Qso.CallsignId = CallSign.CallSignId;
                                                }
                                                catch (Exception ex)
                                                {
                                                    Debug.WriteLine(string.Format(" bad CallSign: {0} for {1} log  message {2} ",
                                                       (Qcolumns[i]).ToString(), Log.CallsignId, ex.Message));
                                                    throw;
                                                }
                                                break;
                                            case 9:
                                                try
                                                {
                                                    short rxrpt;
                                                    if (short.TryParse(Qcolumns[i], out rxrpt))
                                                    {
                                                        Qso.RxRst = rxrpt;
                                                    }
                                                    else
                                                    {
                                                        Qso.RxRst = 0xff;
                                                    }
                                                }
                                                catch
                                                {
                                                    Debug.WriteLine(string.Format(" bad RxRst: {0} for {1} log ",
                                                        (Qcolumns[i]).ToString(), Log.CallSign.Call));
                                                    throw;
                                                }
                                                break;
                                            case 10:
                                                //zone I4
                                                //Qcolumns[i] = Qcolumns[i].Replace('I', '1'); //bad zone I4
                                                try
                                                {
                                                    short num;
                                                    if (short.TryParse(Qcolumns[i], out num))
                                                    {
                                                        Qso.QsoExchangeNumber = num;
                                                    }
                                                    else
                                                    {
                                                        Qso.QsoExchangeNumber = 0xff;
                                                    }
                                                }
                                                catch
                                                {
                                                    Debug.WriteLine(string.Format(" bad QsoExchangeNumber: {0} for {1} log ",
                                                        (Qcolumns[i]).ToString(), Log.CallSign.Call));
                                                    Qso.QsoExchangeNumber = 0xff;
                                                    throw;
                                                }
                                                break;
                                            case 11:
                                                //radio
                                                try
                                                {
                                                    byte val;
                                                    if (byte.TryParse(Qcolumns[i], out val))
                                                    {
                                                        switch (val)
                                                        {
                                                            case 0:
                                                                Qso.QsoRadioTypeEnum = QsoRadioTypeEnum.R1;
                                                                break;
                                                            case 1:
                                                                Qso.QsoRadioTypeEnum = QsoRadioTypeEnum.R2;
                                                                break;
                                                            default:
                                                                Qso.QsoRadioTypeEnum = Logqso.mvc.common.Enum.QsoRadioTypeEnum.NONE;
                                                                break;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Qso.QsoRadioTypeEnum = Logqso.mvc.common.Enum.QsoRadioTypeEnum.NONE;
                                                    }
                                                }
                                                catch
                                                {
                                                    Debug.WriteLine(string.Format(" bad QsoRadioTypeEnum: {0} for {1} log ",
                                                        (Qcolumns[i]).ToString(), Log.CallSign.Call));
                                                    throw;
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
                            catch (Exception ex )
                            {
                                
                                continue;
                            }
                        }
                        if (Qso != null)
                        {//add to DB if none or not already added
                            if (Qso.QsoNo >= NextDBQsoNum)
                            {
                                try
                                {
                                    Qsos.Add(Qso);
                                }
                                catch (Exception)
                                {
                                    throw;
                                }        
                            }
                        }
                    }
                    if (Qsos.Count != 0)
                    {//add to DB
                        try
                        {
                            IBusiness.AddQso(Qsos.ToArray());
                        }
                        catch (Exception)
                        {
                            
                            throw;
                        }
                    }
                }
             }
            catch (Exception ex)
            {
                
                Debug.WriteLine(string.Format(" Problem in {0} log ", Log.CallSign.Call ) );
                throw;
            }
  


        }
    }
}
