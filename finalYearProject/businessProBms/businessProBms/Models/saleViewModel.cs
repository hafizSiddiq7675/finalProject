﻿using System.Collections.Generic;
namespace businessProBms.Models
{
    public class saleViewModel
    {
        public Sale sales { get; set; }
        public List<SaleDetail> saleDetails { get; set; }
        public int saleId { get; set; }
        public System.DateTime saleDate { get; set; }
        public int customerCode { get; set; }
        public string customerName { get; set; }
        public int productCode { get; set; }
        public string productName { get; set; }
        public int quantity { get; set; }
        public decimal salePrice { get; set; }
        public int transactionType { get; set; }
        public int accountno { get; set; }
        public decimal netPayment { get; set; }
    }
}