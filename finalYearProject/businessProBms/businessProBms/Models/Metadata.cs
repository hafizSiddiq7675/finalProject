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
}