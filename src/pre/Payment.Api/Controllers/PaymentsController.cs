using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Payment.Application.Base.Models;
using Payment.Application.Features.Commands;
using Payment.Application.Features.Dtos;
using Payment.Service.Momo.Request;
using Payment.Service.VnPay.Config;
using Payment.Service.VnPay.Response;
using Payment.Ultils.Extensions;
using System.Net;

namespace Payment.Api.Controllers
{
    /// <summary>
    /// Payment api endpoints
    /// </summary>
    [Route("api/payment")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IMediator mediator;
        private readonly VnpayConfig vnpayConfig;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mediator"></param>
        /// <param name="vnpayConfigOptions"></param>
        public PaymentsController(IMediator mediator,
            IOptions<VnpayConfig> vnpayConfigOptions)
        {
            this.mediator = mediator;
            this.vnpayConfig = vnpayConfigOptions.Value;
        }

        /// <summary>
        /// Create payment to get link
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(BaseResultWithData<PaymentLinkDtos>), 200)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreatePayment request)
        {
            var response = new BaseResultWithData<PaymentLinkDtos>();
            response = await mediator.Send(request);
            return Ok(response);
        }

        [HttpGet]
        [Route("vnpay-return")]
        public async Task<IActionResult> VnpayReturn([FromQuery]VnpayPayResponse response)
        {
            string returnUrl = string.Empty;
            var returnModel = new PaymentReturnDtos();
            var processResult = await mediator.Send(response.Adapt<ProcessVnpayPaymentReturn>());

            if (processResult.Success)
            {
                returnModel = processResult.Data.Item1 as PaymentReturnDtos;
                returnUrl = processResult.Data.Item2 as string;
            }

            if (returnUrl.EndsWith("/"))
                returnUrl = returnUrl.Remove(returnUrl.Length - 1, 1);
            return Redirect($"{returnUrl}?{returnModel.ToQueryString()}");
        }

        [HttpGet]
        [Route("momo-return")]
        public async Task<IActionResult> MomoReturn([FromQuery]MomoOneTimePaymentResultRequest response)
        {
            string returnUrl = string.Empty;
            var returnModel = new PaymentReturnDtos();
            var processResult = await mediator.Send(response.Adapt<ProcessMomoPaymentReturn>());

            if (processResult.Success)
            {
                returnModel = processResult.Data.Item1 as PaymentReturnDtos;
                returnUrl = processResult.Data.Item2 as string;
            }

            if (returnUrl.EndsWith("/"))
                returnUrl = returnUrl.Remove(returnUrl.Length - 1, 1);
            return Redirect($"{returnUrl}?{returnModel.ToQueryString()}");
        }
    }
}
