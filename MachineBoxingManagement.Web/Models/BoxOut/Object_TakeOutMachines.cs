using System.Collections.Generic;

namespace MachineBoxingManagement.Web.Models.BoxOut
{
    public class Object_TakeOutMachines
    {
        public string userName { get; set; }
        public int[] ids { get; set; }
        public List<Object_BoxOut> BoxOut_Item { get; set; }
    }

    public class Object_BoxOut
    {
        public int Id { get; set; }
        public bool IsBufferArea { get; set; }
    }
}
