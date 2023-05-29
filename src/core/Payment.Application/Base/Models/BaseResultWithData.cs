using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application.Base.Models
{
    public class BaseResultWithData<T> : BaseResult
    {
        public T? Data { get; set; }
    }
}
