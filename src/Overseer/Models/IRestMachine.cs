using System.Collections.Generic;

namespace Overseer.Models
{
    public interface IRestMachine
    {
        string Url { get; set; }

        string ClientCertificate { get; set; }

        Dictionary<string, string> Headers { get; }
    }
}