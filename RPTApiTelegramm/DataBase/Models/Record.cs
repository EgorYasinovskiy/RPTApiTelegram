using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RPTApi.DataBase.Models
{
    public class Record
    {
        public DateTime DateTime { get; set; }
        public string Location { get; set; }
        [ForeignKey("OrderBarcode")]
        public string OrderBarcode { get; set; }
        public string OperationType { get; set; }
        public string OperationAttribute { get; set; }
        public virtual Order Order { get; set; }
    }
}
