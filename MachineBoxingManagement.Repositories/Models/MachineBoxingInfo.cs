// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace MachineBoxingManagement.Repositories.Models
{
    public partial class MachineBoxingInfo
    {
        public int Id { get; set; }
        public string PartNumber { get; set; }
        public string Ssn { get; set; }
        public int BoxingLocationId { get; set; }
        public int BoxingOptionId { get; set; }
        public int BoxingSerial { get; set; }
        public int? StackLevel { get; set; }
        public int StatusId { get; set; }
        public string Operator { get; set; }
        public DateTime OperateTime { get; set; }
        public string TakeOutor { get; set; }
        public DateTime? TakeOutTime { get; set; }
        public int BoxingStyleId { get; set; }
        public string BoxingName { get; set; }
        public bool BufferArea { get; set; }

        public virtual BoxingLocation BoxingLocation { get; set; }
        public virtual BoxingOption BoxingOption { get; set; }
        public virtual BoxingStyle BoxingStyle { get; set; }
        public virtual BoxingStatus Status { get; set; }
    }
}