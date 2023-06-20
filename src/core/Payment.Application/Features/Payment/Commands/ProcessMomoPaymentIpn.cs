using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Payment.Application.Base.Models;
using Payment.Application.Constants;
using Payment.Application.Features.Dtos;
using Payment.Application.Interface;
using Payment.Service.Momo.Config;
using Payment.Service.Momo.Request;
using Payment.Ultils.Extensions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Application.Features.Commands
{
    public class ProcessMomoPaymentIpn : MomoOneTimePaymentResultRequest,
        IRequest<BaseResult>
    {
    }

    public class ProcessMomoPaymentIpnHandler
        : IRequestHandler<ProcessMomoPaymentIpn, BaseResult>
    {
        private readonly IConnectionService connectionService;
        private readonly ISqlService sqlService;
        private readonly ICurrentUserService currentUserService;
        private readonly MomoConfig momoConfig;

        public ProcessMomoPaymentIpnHandler(IConnectionService connectionService,
            ISqlService sqlService,
            ICurrentUserService currentUserService,
            IOptions<MomoConfig> momoConfigOptions)
        {
            this.connectionService = connectionService;
            this.sqlService = sqlService;
            this.currentUserService = currentUserService;
            this.momoConfig = momoConfigOptions.Value;
        }

        public Task<BaseResult> Handle(ProcessMomoPaymentIpn request, CancellationToken cancellationToken)
        {
            var result = new BaseResult();

            try
            {
                var isValidSignature = request.IsValidSignature(momoConfig.AccessKey, momoConfig.SecretKey);

                if (isValidSignature)
                {
                    /// Get payment request
                    string connectionString = connectionService.Datebase ?? string.Empty;
                    var paramters = new SqlParameter[]
                    {
                        new SqlParameter("@PaymentId", request.orderId),
                    };
                    (var data, string sqlError) = sqlService.FillDataTable(connectionString,
                        PaymentConstants.SelectByIdSprocName, paramters);
                    var payment = data.AsListObject<PaymentDtos>()?.SingleOrDefault();

                    if (payment != null)
                    {
                        if (payment.RequiredAmount == request.amount)
                        {
                            if (payment.PaymentStatus != "0")
                            {
                                string message = "";
                                string status = "";

                                if (request.resultCode == 0)
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
                                    new SqlParameter("@TranAmount", request.amount),
                                    new SqlParameter("@TranDate", DateTime.Now),
                                    new SqlParameter("@PaymentId", request.orderId),
                                    new SqlParameter("@InsertUser", currentUserService.UserId ?? string.Empty),
                                };
                                (var affectedRows, sqlError) = sqlService.ExecuteNonQuery(connectionString,
                                    PaymentTransactionContants.InsertSprocName, paramters);

                                /// Confirm success
                                if (affectedRows >= 1)
                                {
                                    result.Set(true, "Confirm success");
                                }
                                else
                                {
                                    result.Set(false, "Input required data");
                                }
                            }
                            else
                            {
                                result.Set(false, "Payment already confirmed");
                            }
                        }
                        else
                        {
                            result.Set(false, "Invalid amount");
                        }
                    }
                    else
                    {
                        result.Set(false, "Payment not found");
                    }
                }
                else
                {
                    result.Set(false, "Invalid signature");
                }
            }
            catch (Exception ex)
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
