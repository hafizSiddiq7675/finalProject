using System.ComponentModel.DataAnnotations;
using System;
namespace businessProBms.Models
{
    [MetadataType(typeof(categoryMetadata))]
    public partial class Category
    {
    }
    [MetadataType(typeof(productMetadata))]
    public partial class Product
    {
    }
    [MetadataType(typeof(vendorMetadata))]
    public partial class Vendor
    {
        public decimal openingCredit { get; set; }
    }
    [MetadataType(typeof(customerMetadata))]
    public partial class Customer 
    {
        public decimal openingDebit { get; set; }
    }
    [MetadataType(typeof(purchaseMetadata))]
    public partial class Purchase
    {
    }
    [MetadataType(typeof(purchaseDetailMetadata))]
    public partial class PurchaseDetail
    {
    }
    [MetadataType(typeof(saleMetadata))]
    public partial class Sale
    {
    }
    [MetadataType(typeof(saleDetailMetadata))]
    public partial class SaleDetail 
    {
    }
    [MetadataType(typeof(expenseAccountMetadata))]
    public partial class ExpenseAccount
    {
    }
    [MetadataType(typeof(voucherMetadata))]
    public partial class Voucher
    {
    }
    [MetadataType(typeof(voucherBodyMetadata))]
    public partial class VoucherBody
    {
    }
}