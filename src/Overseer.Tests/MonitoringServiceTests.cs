﻿using Moq;
using NUnit.Framework;
using Overseer.Machines;
using Overseer.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Overseer.Tests
{
    [TestFixture]
    public class MonitoringServiceTests
    {
        public MonitoringService CreateMonitoringService()
        {
            var dataContext = new Data.UnitTestContext();
            var providerManager = new MachineProviderManager(machine => null);
            var machineManager = new Mock<IMachineManager>();
            var configurationManager = new Mock<IConfigurationManager>();
            configurationManager.Setup(x => x.GetApplicationSettings()).Returns(new ApplicationSettings());

            return new MonitoringService(machineManager.Object, configurationManager.Object, providerManager);
        }

        [Test]
        public void ShouldStartMonitoring()
        {
            var monitoringService = CreateMonitoringService();
            Assert.False(monitoringService.Enabled);

            monitoringService.StartMonitoring();
            Assert.True(monitoringService.Enabled);
        }
        
        [Test]
        public void ShouldStopMonitoring()
        {
            var monitoringService = CreateMonitoringService();
            monitoringService.StartMonitoring();
            Assert.True(monitoringService.Enabled);

            monitoringService.StopMonitoring();
            Assert.False(monitoringService.Enabled);
        }

        [Test]
        public async Task ShouldPollProviders()
        {
            var statuses = new List<MachineStatus>();
            var dataContext = new Data.UnitTestContext(); 
            var configurationManager = new Mock<IConfigurationManager>();

            var providerManager = new MachineProviderManager(machine =>
            {
                var mock = new Mock<IMachineProvider>();
                mock.Setup(x => x.GetStatus(It.IsAny<CancellationToken>()))
                    .Returns(() =>
                    {
                        return Task.FromResult(new MachineStatus { MachineId = machine.Id });
                    });

                return mock.Object;
            });

            var machineCount = 10;
            var machines = dataContext.GetRepository<Machine>();

            for (int i = 0; i < machineCount; i++)
            {
                machines.Create(new OctoprintMachine
                {
                    Name = "machine_" + i
                });
            }

            var monitoringService = new MonitoringService(new MachineManager(dataContext, providerManager), configurationManager.Object, providerManager);        
            monitoringService.StatusUpdate += (sender, args) =>
            {
                statuses.Add(args.Data);
            };

            await monitoringService.PollMachines();  

            Assert.AreEqual(machineCount, statuses.Count);
            for (var i = 1; i <= machineCount; i++)
            {
                Assert.True(statuses.Any(status => status.MachineId == i));
            }
        }
    }
}
