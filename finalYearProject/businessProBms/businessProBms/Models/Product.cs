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
    
    public partial class Product
    {
        public int code { get; set; }
        public string name { get; set; }
        public string UOM { get; set; }
        public decimal price { get; set; }
        public int categoryCode { get; set; }
        public Nullable<decimal> averagePrice { get; set; }
        public Nullable<decimal> lastPurchasePrice { get; set; }
        public Nullable<int> currentQuantity { get; set; }
        public string categoryName { get; set; }
    
        public virtual Category Category { get; set; }
    }
}
