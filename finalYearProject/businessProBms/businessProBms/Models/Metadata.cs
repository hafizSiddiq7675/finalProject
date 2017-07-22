using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace businessProBms.Models
{
    public class categoryMetadata
    {
        [Key]
        [Required]
        [Display(Name="Category Code")]
        public int code { get; set; }
        [Required]
        [Display(Name = "Category Name")]
        public string categoryName { get; set; }
        [Required]
        [Display(Name = "Discription")]
        public string discription { get; set; }
    }
    public class productMetadata
    {
        [Key]
        [Required]
        [Display(Name="Product Code")]
        public int code { get; set; }
        [Required]
        [Display(Name="Product Name")]
        public string name { get; set; }
        [Required]
        [Display(Name="Unit of Measure")]
        public string UOM { get; set; }
        [Required]
        [Display(Name="Sale Price")]
        public decimal price { get; set; }
        [Display(Name="Category Name")]
        [Required]
        public string categoryName { get; set; }
        [Required]
        [Display(Name="Category Code")]
        public int categoryCode { get; set; }
    }
    public class vendorMetadata
    {
        [Key]
        [Display(Name="Vendor Code")]
        [Required]
        public int code { get; set; }
        [Display(Name="Vendor Name")]
        [Required]
        public string name { get; set; }
        [Display(Name="Contact Information")]
        [Required]
        public string contact { get; set; }
        [Display(Name="Address")]
        [Required]
        public string address { get; set; }
        [Display(Name="Debit Limit(RS)")]
        public decimal debitLimit { get; set; }
    }
    public class customerMetadata
    {
        [Key]
        [Display(Name="Customer Code")]
        [Required]
        public int code { get; set; }
        [Display(Name="Customer Name")]
        [Required]
        public string name { get; set; }
        [Display(Name="Contact Information")]
        [Required]
        public string contact { get; set; }
        [Display(Name="Address")]
        [Required]
        public string address { get; set; }
        [Display(Name="Credit Limit (RS)")]
        [Required]
        public decimal creditLimit { get; set; }
    }
}