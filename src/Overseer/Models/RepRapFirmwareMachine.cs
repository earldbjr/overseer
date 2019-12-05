using Newtonsoft.Json;
using System.Collections.Generic;

namespace Overseer.Models
{
    public class RepRapFirmwareMachine : Machine, IRestMachine
    {
        public override MachineType MachineType => MachineType.RepRapFirmware;

        public bool RequiresPassword { get; set; }

        public string Password { get; set; }

        public string Url { get; set; }

        public string ClientCertificate { get; set; }

        [JsonIgnore]
        public Dictionary<string, string> Headers => new Dictionary<string, string>();
    }
}