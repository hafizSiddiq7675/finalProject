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
    }
    [MetadataType(typeof(customerMetadata))]
    public partial class Customer 
    { 
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
}