using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application.Base.Models
{
    public class BaseResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; } = string.Empty;
        public List<BaseError> Errors { get; set; } = new List<BaseError>();

        public void Set(bool success, string message)
        {
            this.Success = success;
            this.Message = message; 
        }
    }
}
