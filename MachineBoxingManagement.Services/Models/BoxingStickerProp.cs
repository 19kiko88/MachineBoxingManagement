using System;
using System.Collections.Generic;
using System.Text;

namespace MachineBoxingManagement.Services.Models
{
    public class BoxingStickerProp
    {
        public PropCoord BoxName { get; set; } = new PropCoord { X = 0, Y = 0 };
        public PropCoord BoxSerial { get; set; } = new PropCoord { X = 2, Y = 0 };
        public PropCoord OpDate { get; set; } = new PropCoord { X = 0, Y = 2 };
        public PropCoord Operator { get; set; } = new PropCoord { X = 1, Y = 4 };
        public PropCoord Location { get; set; } = new PropCoord { X = 2, Y = 2 };
        public PropCoord SPartNumber { get; set; } = new PropCoord { Y = 6, X = 0 };
        public PropCoord SModel { get; set; } = new PropCoord { Y = 6, X = 1 };
        public PropCoord SQuanty { get; set; } = new PropCoord { Y = 6, X = 2 };
        public PropCoord Subtotal { get; set; } = new PropCoord { Y = 28, X = 1 };
        public double RowHeight { get; set; } = 26.5;
        public int Rowspan { get; set; } = 29;
        public int Rowcapacity { get; set; } = 3;
        public int Colspan { get; set; } = 5;
        public int Colcapacity { get; set; } = 1;
        public int DefaultOffsetCol { get; set; } = 1;
    }

    public class NpiStickerProp : BoxingStickerProp
    {
        public PropCoord STakeoutDate { get; set; } = new PropCoord { Y = 6, X = 3 };
    }

    public class OaStickerProp : BoxingStickerProp
    {
        public PropCoord SCategory { get; set; } = new PropCoord { Y = 8, X = 2 };
        public PropCoord InStockDate { get; set; } = new PropCoord { Y = 8, X = 4 };
    }

    public class PropCoord
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string Value { get; set; }
    }
}
