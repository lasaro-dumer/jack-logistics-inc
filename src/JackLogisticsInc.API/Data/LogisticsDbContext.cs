using System.Collections.Generic;
using System.Linq;
using JackLogisticsInc.API.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace JackLogisticsInc.API.Data
{
    public class LogisticsDbContext : DbContext
    {
        public const string MIGRATIONS_TABLE = "__EFMigrationsHistory";
        public const string SCHEMA = "logistics";

        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Shipment> Shipments { get; set; }

        public LogisticsDbContext(DbContextOptions<LogisticsDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(SCHEMA);

            modelBuilder.Entity<Package>()
                .HasOne(p => p.Location)
                .WithOne(l => l.Package)
                .HasForeignKey<Package>(p => p.LocationId);

            IEnumerable<IMutableForeignKey> cascadeFKs = modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetForeignKeys())
                .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (IMutableForeignKey fk in cascadeFKs)
                fk.DeleteBehavior = DeleteBehavior.Restrict;

            modelBuilder.Entity<Warehouse>().HasData(new Warehouse() { Id = 1, Name = "Main Street Deposit", AddressData = "F38M+QM Glória, Estrela - RS" });

            modelBuilder.Entity<Location>().HasData(new Location() { Id = 1, WarehouseId = 1, Building = "1A", Floor = "1", Corridor = "A", Shelf = "001" });
            modelBuilder.Entity<Location>().HasData(new Location() { Id = 2, WarehouseId = 1, Building = "1A", Floor = "1", Corridor = "A", Shelf = "002" });
            modelBuilder.Entity<Location>().HasData(new Location() { Id = 3, WarehouseId = 1, Building = "1A", Floor = "1", Corridor = "B", Shelf = "001" });
            modelBuilder.Entity<Location>().HasData(new Location() { Id = 4, WarehouseId = 1, Building = "1A", Floor = "1", Corridor = "B", Shelf = "002" });

            base.OnModelCreating(modelBuilder);
        }
    }
}