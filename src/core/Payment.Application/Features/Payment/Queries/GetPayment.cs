using MediatR;
using Payment.Application.Base.Models;
using Payment.Application.Constants;
using Payment.Application.Features.Dtos;
using Payment.Application.Interface;
using Payment.Ultils.Extensions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application.Features.Queries
{
    public class GetPayment : IRequest<BaseResultWithData<PaymentDtos>>
    {
        public string Id { get; set; } = string.Empty;
    }

    public class GetPaymentHandler : IRequestHandler<GetPayment, BaseResultWithData<PaymentDtos>>
    {
        private readonly ICurrentUserService currentUserService;
        private readonly ISqlService sqlService;
        private readonly IConnectionService connectionService;

        public GetPaymentHandler(ICurrentUserService currentUserService,
            ISqlService sqlService,
            IConnectionService connectionService)
        {
            this.currentUserService = currentUserService;
            this.sqlService = sqlService;
            this.connectionService = connectionService;
        }

        public Task<BaseResultWithData<PaymentDtos>> Handle(GetPayment request, 
            CancellationToken cancellationToken)
        {
            var result = new BaseResultWithData<PaymentDtos>();

            try
            {
                string connectionString = connectionService.Datebase ?? string.Empty;
                var paramters = new SqlParameter[]
                {
                    new SqlParameter("@PaymentId", request.Id),
                };
                (var data, string sqlError) = sqlService.FillDataTable(connectionString,
                    PaymentConstants.SelectByIdSprocName, paramters);
                var payment = data.AsListObject<PaymentDtos>()?.SingleOrDefault();

                if(payment != null)
                {
                    result.Set(true, MessageContants.OK, payment);
                }
                else
                {
                    result.Set(false, MessageContants.NotFound);
                }

            }
            catch(Exception ex)
            {
                result.Set(false, MessageContants.Error);
                result.Errors.Add(new BaseError()
                {
                    Code = MessageContants.Exception,
                    Message = ex.Message,
                });
            }

            return Task.FromResult(result); 
        }
    }
}
