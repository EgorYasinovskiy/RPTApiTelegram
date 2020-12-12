using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RPTApi.DataBase.Models
{
    public class Order
    {   
        public string Name { get; set; }
        [Key]
        public string Barcode { get; set; }
        public DateTime StartTracking { get; set; }
        public DateTime LastQuerry { get; set; }
        public int UserId { get; set; }
        public virtual List<Record> Records { get; set; }
        public virtual BotUser User { get; set; }
    }
}
