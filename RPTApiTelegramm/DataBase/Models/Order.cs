using System;
using System.Collections.Generic;
using System.Text;

namespace RPTApi.DataBase.Models
{
    public class Order
    {   
        public string Barcode { get; set; }
        public DateTime StartTracking { get; set; }
        public virtual List<Record> Records { get; set; }
        public virtual BotUser User { get; set; }
    }
}
