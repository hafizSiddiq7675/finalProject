﻿namespace businessProBms.Models
{
    public class voucherViewModel
    {
        
        public Voucher voucher { get; set; }
        public VoucherBody voucherDetail { get; set; }
        public decimal balance { get; set; }

        public VoucherTypeModel VoucherType { get; set; }
    }
}