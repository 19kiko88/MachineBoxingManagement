namespace MachineBoxingManagement.Web.Models.BoxOut
{
    public class Object_QueryMachines
    {
        public string? pn { get; set; }
        public string? model { get; set; }
        //入庫日期(起)
        public string? take_in_dt_s { get; set; }
        //入庫日期(訖)
        public string? take_in_dt_e { get; set; }
        //取出日期(起)
        public string? take_out_dt_s { get; set; }
        //取出日期(訖)
        public string? take_out_dt_e { get; set; }
        //庫房
        public int[]? locations { get; set; }
        //機台選項
        public int[]? options { get; set; }
        //樣式選項
        public int[]? styles { get; set; }
        //機台狀態
        public int[]? statuses { get; set; }
        //暫存
        public int[]? favorites { get; set; }
        //暫存區
        public int[]? buffer_areas { get; set; }

    }
}
