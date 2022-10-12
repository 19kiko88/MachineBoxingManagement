using System;
using System.Collections.Generic;
using System.Text;

namespace MachineBoxingManagement.Services.Models
{
    public class QueryRule
    {
        public string PartNumber { get; set; }
        public string Model { get; set; }
        public string Sd_BoxIn { get; set; }
        public string Ed_BoxIn { get; set; }
        public string Sd_BoxOut { get; set; }
        public string Ed_BoxOut { get; set; }
        public List<int> LocationIds { get; set; }
        public List<int> OptionIds { get; set; }
        public List<int> StyleIds { get; set; }
        public List<int> Statuses { get; set; }
        /// <summary>
        /// 轉換暫存區查詢條件：0 =>全部選, 1 => 非暫存區, 2=>暫存區, 3=>全選
        /// </summary>
        public int BufferAreas { get; set; }
    }
}
