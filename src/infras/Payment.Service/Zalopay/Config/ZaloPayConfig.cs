using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Service.Zalopay.Config
{
    public class ZaloPayConfig
    {
        public static string ConfigName => "ZaloPay";
        public  string AppUser {get; set; }  = string.Empty;
        public  string PaymentUrl {get; set; } = string.Empty;
        public  string RedirectUrl {get; set; } = string.Empty;
        public  string IpnUrl {get; set; } = string.Empty;
        public  int AppId {get; set; } 
        public string Key1 { get; set; } = string.Empty;
        public string Key2 { get; set; } = string.Empty;

    }
}
