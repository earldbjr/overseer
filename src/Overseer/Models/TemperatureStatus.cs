namespace Overseer.Models
{
    public class TemperatureStatus
    {
        public int HeaterIndex { get; set; }

        public float Actual { get; set; }

        public float Target { get; set; }
    }
}