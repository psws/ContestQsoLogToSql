using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using L2Sql.DomainModel;
using L2Sql.DataAccessLayer;

namespace L2Sql.BusinessLayer
{
    public interface IBusiness
    {
        void AddLog(params Log[] departments);
        void UpdateLog(params Log[] departments);
        void RemoveLog(params Log[] departments);
        IList<Log> GetAllLogs(string ContestId);

        void AddCallSign(params CallSign[] CallSigns);
        void UpdateCallSign(params CallSign[] CallSigns);
        void RemoveCallSign(params CallSign[] CallSigns);
        IList<CallSign> GetAllCallsigns();
        IList<LogCategory> GetAllLogCategorys();
    }
}
