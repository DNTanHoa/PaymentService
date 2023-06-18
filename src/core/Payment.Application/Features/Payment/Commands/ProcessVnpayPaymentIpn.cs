using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
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
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application.Features.Commands
{
    public class ProcessVnpayPaymentIpn : VnpayPayResponse,
        IRequest<BaseResultWithData<VnpayPayIpnResponse>>
    {
    }

    public class ProcessVnpayPaymentIpnHandler :
        IRequestHandler<ProcessVnpayPaymentIpn, BaseResultWithData<VnpayPayIpnResponse>>
    {
        private readonly IConnectionService connectionService;
        private readonly ISqlService sqlService;
        private readonly ICurrentUserService currentUserService;
        private readonly VnpayConfig vnpayConfig;

        public ProcessVnpayPaymentIpnHandler(IConnectionService connectionService,
            ISqlService sqlService,
            ICurrentUserService currentUserService,
            IOptions<VnpayConfig> vnpayConfigOptions)
        {
            this.connectionService = connectionService;
            this.sqlService = sqlService;
            this.currentUserService = currentUserService;
            this.vnpayConfig = vnpayConfigOptions.Value;
        }

        public Task<BaseResultWithData<VnpayPayIpnResponse>> Handle(
            ProcessVnpayPaymentIpn request, CancellationToken cancellationToken)
        {
            var result = new BaseResultWithData<VnpayPayIpnResponse>();
            var resultData = new VnpayPayIpnResponse();

            try
            {
                var isValidSignature = request.IsValidSignature(vnpayConfig.HashSecret);
                
                if (isValidSignature)
                {
                    /// Get payment request
                    string connectionString = connectionService.Datebase ?? string.Empty;
                    var paramters = new SqlParameter[]
                    {
                        new SqlParameter("@PaymentId", request.vnp_TxnRef),
                    };
                    (var data, string sqlError) = sqlService.FillDataTable(connectionString,
                        PaymentConstants.SelectByIdSprocName, paramters);
                    var payment = data.AsListObject<PaymentDtos>()?.SingleOrDefault();

                    if (payment != null)
                    {
                        if (payment.RequiredAmount == (request.vnp_Amount / 100))
                        {
                            if(payment.PaymentStatus != "0")
                            {
                                string message = "";
                                string status = "";

                                if(request.vnp_ResponseCode == "00" && 
                                   request.vnp_TransactionStatus == "00")
                                {
                                    status = "0";
                                    message = "Tran success";
                                }
                                else
                                {
                                    status = "-1";
                                    message = "Tran error";
                                }

                                /// Update database
                                paramters = new SqlParameter[]
                                {
                                    new SqlParameter("@Id", DateTime.Now.Ticks.ToString()),
                                    new SqlParameter("@TranMessage", message),
                                    new SqlParameter("@TranPayload", JsonConvert.SerializeObject(request)),
                                    new SqlParameter("@TranStatus", status),
                                    new SqlParameter("@TranAmount", request.vnp_Amount),
                                    new SqlParameter("@TranDate", DateTime.Now),
                                    new SqlParameter("@PaymentId", request.vnp_TxnRef),
                                    new SqlParameter("@InsertUser", currentUserService.UserId ?? string.Empty),
                                };
                                (var affectedRows, sqlError) = sqlService.ExecuteNonQuery(connectionString,
                                    PaymentTransactionContants.InsertSprocName, paramters);

                                /// Confirm success
                                if (affectedRows >= 1) 
                                {
                                    resultData.Set("00", "Confirm success");
                                }
                                else
                                {
                                    resultData.Set("99", "Input required data");
                                }
                            }
                            else
                            {
                                resultData.Set("02", "Order already confirmed");
                            }
                        }
                        else
                        {
                            resultData.Set("04", "Invalid amount");
                        }
                    }
                    else
                    {
                        resultData.Set("01", "Order not found");
                    }
                }
                else
                {
                    resultData.Set("97", "Invalid signature");
                }
            }
            catch (Exception ex)
            {
                /// TODO: process when exception
                resultData.Set("99", "Input required data");
            }

            result.Data = resultData;
            return Task.FromResult(result);
        }
    }
}
