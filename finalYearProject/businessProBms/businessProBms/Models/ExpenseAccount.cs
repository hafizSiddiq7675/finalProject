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
    
    public partial class ExpenseAccount
    {
        public int code { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int parentCode { get; set; }
        public bool isGroup { get; set; }
        public decimal openingDebit { get; set; }
        public decimal openingCredit { get; set; }
    }
}
