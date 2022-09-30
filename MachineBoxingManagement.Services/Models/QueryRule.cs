using System;
using System.Collections.Generic;
using System.Text;

namespace MachineBoxingManagement.Services.Models
{
    public class QueryRule
    {
        public string PartNumber { get; set; }
        public string Model { get; set; }
        public List<int> LocationIds { get; set; }
        public List<int> OptionIds { get; set; }
        public List<int> StyleIds { get; set; }
        public List<int> Statuses { get; set; }
        public List <bool> BufferAreas { get; set; }
    }
}
