﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

namespace MachineBoxingManagement.Repositories.Models
{
    public partial class BoxingStatus
    {
        public BoxingStatus()
        {
            MachineBoxingInfo = new HashSet<MachineBoxingInfo>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<MachineBoxingInfo> MachineBoxingInfo { get; set; }
    }
}