using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using L2Sql.DomainModel;
using L2Sql.DataAccessLayer;
using L2Sql.Dto;

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

        public Business()
        {
            ILogRepository = new LogRepository();
            IQsoRepository = new QsoRepository();
            ICallSignRepository = new CallSignRepository();
            ILogCategoryRepository = new LogCategoryRepository();
            ICabrilloInfoRepository = new CabrilloInfoRepository();
            IContestRepository = new ContestRepository();
            IQsoExchangeNumberRepository = new QsoExchangeNumberRepository();

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
                d => d.CallSign, d=>d.LogCategory ); //include related 
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
            catch (Exception ex )
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
            IQsoRepository.UpdateQsoPointsMults(QsoUpdatePoinsMultsDTOCollextion);
                //d => d.CallSign); //include related 
        }


        public IList<Qso> GetQsos(int LogId)
        {
            return IQsoRepository.GetList(d => d.LogId.Equals(LogId));
            //d => d.CallSign); //include related 
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

    }
}
