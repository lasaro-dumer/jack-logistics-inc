using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JackLogisticsInc.API.Data;
using JackLogisticsInc.API.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace JackLogisticsInc.API.Services
{
    public class DeliveryProcessingService : BackgroundService
    {
        public IServiceProvider Services { get; set; }

        public DeliveryProcessingService(IServiceProvider services)
        {
            Services = services;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log.Information($"Starting {nameof(DeliveryProcessingService)} {nameof(ExecuteAsync)}");

            await Task.Yield();

            while (stoppingToken.IsCancellationRequested == false)
            {
                await Task.Delay(1000 * 60 * 1, stoppingToken);
                await DoWork(stoppingToken);
            }
        }

        private async Task DoWork(CancellationToken stoppingToken)
        {
            Log.Information($"Starting {nameof(DeliveryProcessingService)} {nameof(DoWork)}");
            try
            {
                using(var scope = Services.CreateScope())
                {
                    LogisticsDbContext dbContext = scope.ServiceProvider.GetService<LogisticsDbContext>();

                    List<Shipment> dueShipments = await dbContext.Shipments.Where(s => !s.DeliveredAt.HasValue &&
                        s.EstimatedTimeOfArrival <= DateTime.UtcNow).ToListAsync(stoppingToken);

                    Log.Information($"Delivering {dueShipments.Count} shipments");

                    foreach (var shipment in dueShipments)
                    {
                        shipment.DeliveredAt = DateTime.UtcNow;
                    }

                    Log.Information($"Saving shipments");

                    await dbContext.SaveChangesAsync(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"Error while processing shipments delivery: {ex.Message}", ex);
            }
            Log.Information($"Finishing {nameof(DeliveryProcessingService)} {nameof(DoWork)}");
        }
    }
}