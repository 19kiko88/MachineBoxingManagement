using System;
using System.Collections.Generic;
using System.Text;

namespace MachineBoxingManagement.Services.Models
{
    public class PnXStatus
    {
        public string PartNumber { get; set; }
        public bool? ToOA { get; set; }
        public bool? TearDown { get; set; }
        public bool? MbResale { get; set; }
    }
}
