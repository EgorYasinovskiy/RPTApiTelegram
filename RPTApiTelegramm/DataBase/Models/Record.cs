using System;

namespace RPTApi.DataBase.Models
{
    public class Record
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public string Location { get; set; }
        public virtual Order Order { get; set; }
    }
}
