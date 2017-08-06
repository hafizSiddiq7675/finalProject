using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace businessProBms.Models
{
    public class voucherLine
    {
        public int voucherNo { get; set; }
        public System.DateTime voucherDate { get; set; }
        public int accountNo { get; set; }
        public string accountName { get; set; }
        public string description { get; set; }
        public decimal debit { get; set; }
        public decimal credit { get; set; }
    }
}