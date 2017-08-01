using System.ComponentModel.DataAnnotations;

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
    public class purchaseMetadata
    {
        [Display(Name="Purchase ID")]
        [Required]
        [Key]
        public int purchaseId { get; set; }
        [Display(Name="Date")]
        [Required]
        [DataType(DataType.Date)]
        public System.DateTime purchaseDate { get; set; }
        [Display(Name="Vendor")]
        [Required]
        public int vendorCode { get; set; }
        [Required]
        [Display(Name="Vendor Name")]
        public string vendorName { get; set; }
    }
    public class saleMetadata
    {
        [Display(Name="Sale ID")]
        [Key]
        [Required]
        public int saleId { get; set; }
        [Display(Name="Date")]
        [Required]
        [DataType(DataType.Date)]
        public System.DateTime saleDate { get; set; }
        [Display(Name="Customer")]
        [Required]
        public int customerCode { get; set; }
    }
    public class saleDetailMetadata
    {
        [Display(Name="Sale Detail Code")]
        public int code { get; set; }
        [Display(Name="Sale Detail ID")]
        [Required]
        public int saleDetailsId { get; set; }
        [Display(Name="Serial Number")]
        public string serialNo { get; set; }
        [Display(Name="Product Code")]
        public int productCode { get; set; }
        [Display(Name="Product Name")]
        public string productName { get; set; }
        [Display(Name="Unit Of Measure")]
        public string unitOfMeasure { get; set; }
        [Display(Name="Quantity")]
        public int quantity { get; set; }
        [Display(Name="Sale Price")]
        public decimal salePrice { get; set; }
    }
    public class purchaseDetailMetadata
    {
        [Required]
        [Key]
        [Display(Name="Purchase Detail ID")]
        public int purchaseDetailsId { get; set; }
        public string serialNo { get; set; }
        [Display(Name="Product Code")]
        [Required]
        public int productCode { get; set; }
        [Required]
        public string productName { get; set; }
        [Display(Name="Quantity")]
        [Required]
        public int quantity { get; set; }
        [Display(Name="Price")]
        [Required]
        public decimal purchasePrice { get; set; }
    }
}