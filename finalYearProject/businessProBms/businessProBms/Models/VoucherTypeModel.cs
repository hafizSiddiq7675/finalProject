using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace businessProBms.Models
{
    public enum VoucherTypeModel : int
    {
        CashRecipt=0,
        CashPayment=1,
        BankRecipet=2,
        BankPayment=3,
        Journal=4
    }
}