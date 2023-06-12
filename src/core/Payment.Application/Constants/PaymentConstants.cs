using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application.Constants
{
    public class PaymentConstants
    {
        public static string InsertSprocName => "sproc_PaymentInsert";
        public static string SelectByIdSprocName => "sproc_PaymentSelectById";
    }
}
