using System;
using MachineBoxingManagement.Web.Models.BoxIn;
using System.Collections.Generic;

namespace MachineBoxingManagement.Web.Models.Dto
{
    public class Favorite_PartNumber_Model_Desc
    {
        public int serial_No { get; set; }//流水號
        public int ID { get; set; }//DB KEY ID
        public string Part_Number { get; set; } = string.Empty;
        public string Boxing_Series { get; set; } = string.Empty;
        public int Boxing_Serial { get; set; }
        public int Turtle_Level { get; set; }
        public string Model { get; set; } = string.Empty;
        public string Desc { get; set; } = string.Empty;
    }
}
