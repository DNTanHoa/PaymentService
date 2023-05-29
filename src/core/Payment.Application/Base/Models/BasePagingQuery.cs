using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application.Base.Models
{
    [BindProperties]
    public class BasePagingQuery
    {
        public string? Criteria { get; set; } = string.Empty;
        public int? PageSize { get; set;} = 20;
        public int? PageIndex { get; set; } = 1;
    }
}
