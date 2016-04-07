using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Objects;


namespace L2Sql.DataAccessLayer
{
    public static class DbContextExtensions
    {
        public static System.Data.Entity.Core.Objects.ObjectContext ToObjectContext(this DbContext dbContext)
        {
            return (dbContext as System.Data.Entity.Infrastructure.IObjectContextAdapter).ObjectContext;
        }
    }
}

