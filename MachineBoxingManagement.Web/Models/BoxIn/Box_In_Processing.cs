using MachineBoxingManagement.Web.Models.Dto;
using MachineBoxingManagement.Web.Models.BoxIn;
using System.Collections.Generic;

namespace MachineBoxingManagement.Web.Models.BoxIn
{
    public class Box_In_Processing
    {
        public PartNumber_Model_Desc PartNumber_Model_Desc { get; set; }
        public Stocking_Info Stocking_Info { get; set; }
    }
}
