using System;
using System.Threading.Tasks;
using Overseer.Models;

namespace Overseer
{
    public interface IMonitoringService : IDisposable
    {
        bool Enabled { get; }

        event EventHandler<EventArgs<MachineStatus>> StatusUpdate;
        
        Task PollMachines();

        void StartMonitoring();
        
        void StopMonitoring();
    }
}