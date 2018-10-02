using System;
using System.Collections.Generic;
using Lykke.Job.OrderbooksBridge.Domain;
using Lykke.Job.OrderbooksBridge.Sql.Models;
using Lykke.Service.DataBridge.Data.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Lykke.Job.OrderbooksBridge.Sql
{
    public class DataContext : DbContextExt
    {
        private const int MaxStringFieldsLength = 255;

        private readonly Dictionary<Type, List<string>> _typesDict = new Dictionary<Type, List<string>>
        {
            { typeof(Orderbook), new List<string>{ "OrderBooks" } },
            { typeof(OrderBookForSqlDb), new List<string>{ "OrderBooks" } },
        };

        public const string OrderbooksTable = "OrderBooks";

        public virtual DbSet<OrderBookForSqlDb> Orderbooks { get; set; }

        public DataContext()
            : base(new DbContextOptionsBuilder<DataContext>().Options)
        {
            Database.SetCommandTimeout(TimeSpan.FromMinutes(15));
        }

        public DataContext(DbContextOptions options)
            : base(options)
        {
            Database.SetCommandTimeout(TimeSpan.FromMinutes(15));
        }

        public override List<string> GetTableNames(Type type)
        {
            if (!_typesDict.ContainsKey(type))
                throw new NotSupportedException($"Type {type.Name} is not supported!");

            return _typesDict[type];
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderBookForSqlDb>(entity =>
            {
                entity.Property(e => e.Id).UseSqlServerIdentityColumn().HasColumnType("bigint");
                entity.Property(e => e.AssetPair).IsRequired().HasColumnType($"varchar({MaxStringFieldsLength})");
                entity.Property(e => e.IsBuy).HasColumnType("bit");
                entity.Property(e => e.Timestamp).HasColumnType("datetime");
                entity.Property(i => i.BestPrice).HasColumnType("float");
                entity.ToTable(OrderbooksTable);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
