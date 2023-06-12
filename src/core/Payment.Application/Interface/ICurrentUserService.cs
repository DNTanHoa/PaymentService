using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application.Interface
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        string? IpAddress { get; }
    }
}
