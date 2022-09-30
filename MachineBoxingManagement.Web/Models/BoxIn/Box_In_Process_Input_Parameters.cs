using MachineBoxingManagement.Web.Models.BoxIn;
using MachineBoxingManagement.Web.Models.Dto;
using System.Collections.Generic;

namespace MachineBoxingManagement.Web.Models.BoxIn
{
    public class Box_In_Process_Input_Parameters
    {
        public string UserName { get; set; } = string.Empty;
        public bool SaveTemp { get; set; }
        public string PartNumber { get; set; } = string.Empty;
        public string SSN { get; set; } = string.Empty;
        public int Location { get; set; }
        public int MachineOption { get; set; }
        public int MachineStyle { get; set; }
        public string BoxSeries { get; set; }
        public int BoxSerial { get; set; }
        public List<PartNumber_Model_Desc> PnDatas { get; set; }

    }
}
