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

        private readonly ILogRepository ILogRepository;
        private readonly IQsoRepository IQsoRepository;
        private readonly ICallSignRepository ICallSignRepository;
        private readonly ILogCategoryRepository ILogCategoryRepository;
        private readonly ICabrilloInfoRepository ICabrilloInfoRepository;

        public Business()
        {
            ILogRepository = new LogRepository();
            IQsoRepository = new QsoRepository();
            ICallSignRepository = new CallSignRepository();
            ILogCategoryRepository = new LogCategoryRepository();
            ICabrilloInfoRepository = new CabrilloInfoRepository();
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
                d => d.CallSign ); //include related 
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
            catch (Exception ex )
            {
                
                throw;
            }
        }

        public void AddQso(params Qso[] Qsos)
        {
            /* Validation and error handling omitted */
                IQsoRepository.AddRange(Qsos);
        }

        public IList<Qso> GetQso(int LogId)
        {
            return IQsoRepository.GetList(d => d.LogId.Equals(LogId)); //include related 
        }


        public void UpdateCallSign(params CallSign[] CallSigns)
        {
            /* Validation and error handling omitted */
            ICallSignRepository.Update(CallSigns);
        }

        public void RemoveCallSign(params CallSign[] CallSigns)
        {
            /* Validation and error handling omitted */
            ICallSignRepository.Remove(CallSigns);
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


    }
}
