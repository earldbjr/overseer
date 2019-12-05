using Newtonsoft.Json;
using System.Collections.Generic;

namespace Overseer.Models
{
    public class OctoprintMachine : Machine, IRestMachine
    {
        public override MachineType MachineType => MachineType.Octoprint;

        public string ApiKey { get; set; }

        public string Profile { get; set; }

        public Dictionary<string, string> AvailableProfiles { get; set; } = new Dictionary<string, string>();

        public string Url { get; set; }

        public string ClientCertificate { get; set; }

        [JsonIgnore]
        public Dictionary<string, string> Headers => new Dictionary<string, string> { { "X-Api-Key", ApiKey } };
    }
}