//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace businessProBms.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class SaleDetail
    {
        public int code { get; set; }
        public int saleDetailsId { get; set; }
        public string serialNo { get; set; }
        public int productCode { get; set; }
        public string productName { get; set; }
        public string unitOfMeasure { get; set; }
        public int quantity { get; set; }
        public decimal salePrice { get; set; }
        public decimal amount { get; set; }
    
        public virtual Sale Sale { get; set; }
    }
}
