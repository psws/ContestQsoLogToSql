using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using L2Sql.DomainModel;
using L2Sql.DataAccessLayer;

namespace L2Sql.BusinessLayer
{
    public class Business : IBusiness
    {

        private readonly ILogRepository LogRepository;
        private readonly IQsoRepository QsoRepository;
        private readonly ICallSignRepository CallSignRepository;
        private readonly ILogCategoryRepository LogCategoryRepository;
 
        public Business()
        {
            LogRepository = new LogRepository();
            QsoRepository = new QsoRepository();
            CallSignRepository = new CallSignRepository();
            LogCategoryRepository = new LogCategoryRepository();
        }

        public Business(ILogRepository LogRepository,
            IQsoRepository QsoRepository)
        {
            this.LogRepository = LogRepository;
            this.QsoRepository = QsoRepository;
        }

        public IList<Log> GetAllLogs(string ContestId)
        {
            return LogRepository.GetList(d => d.ContestId.Equals(ContestId), 
                d => d.CallSign ); //include related employees
        }


        public IList<LogCategory> GetAllLogCategorys()
        {
            return LogCategoryRepository.GetAll();
        }

        public void AddLog(params Log[] Logs)
        {
            /* Validation and error handling omitted */
            LogRepository.Add(Logs);
        }

        public void UpdateLog(params Log[] Logs)
        {
            /* Validation and error handling omitted */
            LogRepository.Update(Logs);
        }

        public void RemoveLog(params Log[] Logs)
        {
            /* Validation and error handling omitted */
            LogRepository.Remove(Logs);
        }
 
        //CallSigns
        public IList<CallSign> GetAllCallsigns()
        {
            return CallSignRepository.GetAll(); 
        }

        public void AddCallSign(params CallSign[] CallSigns)
        {
            /* Validation and error handling omitted */
            CallSignRepository.Add(CallSigns);
        }

        public void UpdateCallSign(params CallSign[] CallSigns)
        {
            /* Validation and error handling omitted */
            CallSignRepository.Update(CallSigns);
        }

        public void RemoveCallSign(params CallSign[] CallSigns)
        {
            /* Validation and error handling omitted */
            CallSignRepository.Remove(CallSigns);
        }

    }
}
