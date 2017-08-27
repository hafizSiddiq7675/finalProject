using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace businessProBms.Models
{
    public class purchaseViewModel
    {
        public Purchase purchases { get; set; }
        public List<PurchaseDetail> purchaseDetails { get; set; }
        public int purchaseId { get; set; }
        public System.DateTime purchaseDate { get; set; }
        public int vendorCode { get; set; }
        public string vendorName { get; set; }
        public int productCode { get; set; }
        public string productName { get; set; }
        public int quantity { get; set; }
        public decimal purchasePrice { get; set; }
        public int transactionType { get; set; }
        public int accountno { get; set; }
        public decimal netPayment { get; set; }
    }
}