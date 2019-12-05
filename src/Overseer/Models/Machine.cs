using Overseer.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Overseer.Models
{
    public abstract class Machine : IEntity
    {
        static readonly Lazy<ConcurrentDictionary<MachineType, Type>> _machineTypeMap = new Lazy<ConcurrentDictionary<MachineType, Type>>(() =>
        {
            var mapping = typeof(Machine).GetAssignableTypes()
                .ToDictionary(type => ((Machine)Activator.CreateInstance(type)).MachineType);

            return new ConcurrentDictionary<MachineType, Type>(mapping);
        });

        public int Id { get; set; }

        public string Name { get; set; }

        public bool Disabled { get; set; }

        public string WebCamUrl { get; set; }

        public WebCamOrientation WebCamOrientation { get; set; }

        public string SnapshotUrl { get; set; }

        public IEnumerable<MachineTool> Tools { get; set; } = new List<MachineTool>();

        public abstract MachineType MachineType { get; }

        public int SortIndex { get; set; }

        public MachineTool GetHeater(int heaterIndex)
        {
            return GetTool(MachineToolType.Heater, heaterIndex);
        }

        public MachineTool GetExtruder(int extruderIndex)
        {
            return GetTool(MachineToolType.Extruder, extruderIndex);
        }

        public MachineTool GetTool(MachineToolType machineToolType, int index)
        {
            return Tools.FirstOrDefault(tool => tool.ToolType == machineToolType && tool.Index == index);
        }

        public static Type GetMachineType(string machineTypeName)
        {
            var machineType = (MachineType)Enum.Parse(typeof(MachineType), machineTypeName, ignoreCase: true);
            if (_machineTypeMap.Value.TryGetValue(machineType, out Type type)) return type;

            throw new InvalidOperationException("Invalid Machine Type");
        }
    }
}