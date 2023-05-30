using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payment.Application.Base.Models;
using Payment.Application.Features.Dtos;
using Payment.Domain.Entities;
using System.Net;

namespace Payment.Api.Controllers
{
    /// <summary>
    /// Api for check payment notification
    /// </summary>
    [Route("api/payment-notifications")]
    [ApiController]
    public class PaymentNotificationsController : ControllerBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public PaymentNotificationsController()
        {
                
        }

        /// <summary>
        /// Get notifications base on criteria
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(BaseResultWithData<List<PaymentNotification>>), 200)]
        [ProducesResponseType(400)]
        public IActionResult Get(string criteria)
        {
            var response = new BaseResultWithData<List<PaymentNotification>>();
            return Ok(response);
        }

        /// <summary>
        /// Get notifications paging
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("with-paging")]
        [ProducesResponseType(typeof(BaseResultWithData<BasePagingData<PaymentNotification>>), 200)]
        public IActionResult GetPaging([FromQuery] BasePagingQuery query)
        {
            var response = new BaseResultWithData<BasePagingData<PaymentNotification>>();
            return Ok(response);
        }

        /// <summary>
        /// Get one notification by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(BaseResultWithData<PaymentNotification>), 200)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public IActionResult GetOne([FromRoute] string id)
        {
            var response = new BaseResultWithData<PaymentNotification>();
            return Ok(response);
        }
    }
}
