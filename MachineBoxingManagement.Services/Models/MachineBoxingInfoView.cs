using System;
using System.Collections.Generic;
using System.Text;

namespace MachineBoxingManagement.Services.Models
{
    public class MachineBoxingInfoView
    {
        public int ID { get; set; }
        public string PartNumber { get; set; }
        public bool BufferArea { get; set; }
        public string BoxingName { get; set; }
        public int BoxingSerial { get; set; }
        public int? StackLevel { get; set; }
        public string Model { get; set; }
        public string Description { get; set; }
        public string SSN { get; set; }
        public string InStockDate { get; set; }
        public string Location { get; set; }
        public string BoxingOption { get; set; }
        public string Status { get; set; }
        public string Operator { get; set; }
        public DateTime OperationTime { get; set; }
        public bool Takeout { get; set; } = false;
        public string Takeouter { get; set; }
        public DateTime? TakeoutTime { get; set; }
    }
}
