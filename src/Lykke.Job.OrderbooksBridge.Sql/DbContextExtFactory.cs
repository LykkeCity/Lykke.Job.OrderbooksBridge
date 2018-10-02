using Lykke.Service.DataBridge.Data.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Lykke.Job.OrderbooksBridge.Sql
{
    public class DbContextExtFactory : IDbContextExtFactory
    {
        public DbContextExt CreateInstance(DbContextOptions options)
        {
            return new DataContext(options);
        }

        public DataContext CreateInstance(string connectionString)
        {
            var optionsBiuilder = new DbContextOptionsBuilder<DataContext>();
            optionsBiuilder.UseSqlServer(connectionString, opts =>
            {
                opts.EnableRetryOnFailure();
                opts.CommandTimeout(900);
            });
            return new DataContext(optionsBiuilder.Options);
        }
    }
}
