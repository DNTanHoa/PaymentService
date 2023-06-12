using MediatR;
using Microsoft.Extensions.Options;
using Payment.Application.Base.Models;
using Payment.Application.Constants;
using Payment.Application.Features.Dtos;
using Payment.Application.Interface;
using Payment.Service.VnPay.Config;
using Payment.Service.VnPay.Response;
using Payment.Ultils.Extensions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application.Features.Commands
{
    public class ProcessVnpayPaymentReturn : VnpayPayResponse,
        IRequest<BaseResultWithData<(PaymentReturnDtos, string)>>
    {

    }

    public class ProcessVnpayPaymentReturnHandler 
        : IRequestHandler<ProcessVnpayPaymentReturn, BaseResultWithData<(PaymentReturnDtos, string)>>
    {
        private readonly IConnectionService connectionService;
        private readonly ISqlService sqlService;
        private readonly VnpayConfig vnpayConfig;

        public ProcessVnpayPaymentReturnHandler(IConnectionService connectionService,
            ISqlService sqlService,
            IOptions<VnpayConfig> vnpayConfigOptions)
        {
            this.connectionService = connectionService;
            this.sqlService = sqlService;
            this.vnpayConfig = vnpayConfigOptions.Value;
        }
        public Task<BaseResultWithData<(PaymentReturnDtos, string)>> Handle(
            ProcessVnpayPaymentReturn request, CancellationToken cancellationToken)
        {
            string returnUrl = string.Empty;
            var result = new BaseResultWithData<(PaymentReturnDtos, string)>();
            
            try
            {
                var resultData = new PaymentReturnDtos();
                var isValidSignature = request.IsValidSignature(vnpayConfig.HashSecret);
                
                if (isValidSignature)
                {
                    string connectionString = connectionService.Datebase ?? string.Empty;
                    var paramters = new SqlParameter[]
                    {
                            new SqlParameter("@PaymentId", request.vnp_TxnRef),
                    };
                    (var data, string sqlError) = sqlService.FillDataTable(connectionString,
                        PaymentConstants.SelectByIdSprocName, paramters);
                    var payment = data.AsListObject<PaymentDtos>()?.SingleOrDefault();
                    
                    if(payment != null)
                    {
                        paramters = new SqlParameter[]
                        {
                            new SqlParameter("@Id", payment.MerchantId),
                        };
                        (data, sqlError) = sqlService.FillDataTable(connectionString,
                            MerchantContants.SelectByIdSprocName, paramters);
                        var merchant = data.AsListObject<MerchantDtos>()?.SingleOrDefault();
                        returnUrl = merchant?.MerchantReturnUrl ?? string.Empty;
                    }
                    else
                    {
                        resultData.PaymentStatus = "11";
                        resultData.PaymentMessage = "Can't find payment at payment service";
                    }

                    if (request.vnp_ResponseCode == "00")
                    {
                        resultData.PaymentStatus = "00";
                        resultData.PaymentId = payment.Id;
                        ///TODO: Make signature
                        resultData.Signature = Guid.NewGuid().ToString();
                    }
                    else
                    {
                        resultData.PaymentStatus = "10";
                        resultData.PaymentMessage = "Payment process failed";
                    }

                    result.Success = true;
                    result.Message = MessageContants.OK;
                    result.Data = (resultData, returnUrl);
                }
                else
                {
                    resultData.PaymentStatus = "99";
                    resultData.PaymentMessage = "Invalid signature in response"; 

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
