using System;
using MachineBoxingManagement.Web.Models.BoxIn;
using System.Collections.Generic;

namespace MachineBoxingManagement.Web.Models.Dto
{
    public class PartNumber_Model_Desc
    {        
        public int serial_No { get; set; }//流水號
        public int ID { get; set; }//DB KEY ID
        public string Part_Number { get; set; } = string.Empty;
        public string SSN { get; set; } = string.Empty;
        public int Boxing_Location_Id { get; set; }
        public string Boxing_Location_Cn { get; set; } = string.Empty;
        public int Boxing_Option_Id { get; set; }
        public string Boxing_Option_Cn { get; set; } = string.Empty;
        public int Boxing_Style_Id { get; set; }
        public string Boxing_Series { get; set; } = string.Empty;
        public int Boxing_Serial { get; set; }
        public int Turtle_Level { get; set; }
        public int Status_Id { get; set; }
        public string Operator { get; set; } = string.Empty;
        public string? OperateTime { get; set; }
        public string TakerOut_Operator { get; set; } = string.Empty;
        public string? TakeOut_OperateTime { get; set; }
        public bool Is_Buffer_Area { get; set; }
        public bool? Is_Favorite { get; set; }

        //Data from WCF Service;
        public string Desc { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public bool? ToOA { get; set; }
        public bool? TearDown { get; set; }
        public bool? MbResale { get; set; }
        public string InsDate { get; set; }
    }
}
