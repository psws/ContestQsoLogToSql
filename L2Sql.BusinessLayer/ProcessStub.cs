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

            string ContestId = di.Name.ToUpper();

            ContestTypeEnum ContestTypeEnum;
            CatOperatorEnum CatOperatorEnum;
            var Contest = IBusiness.GetContest(ContestId);
            ContestTypeEnum = Contest.ContestTypeEnum;


            //fixup JA1DCK and  calls containg W1AA??????
            //JHOKHR CQWWSSB2015 BAD QSOS LIKE W1? ? ?....
            result = IBusiness.FixupBadCallSignsContainingString(ContestId, "?");

            return result;
        }


    }
}
