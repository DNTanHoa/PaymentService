using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Payment.Application.Base.Models;
using Payment.Application.Features.Dtos;
using System.Net;

namespace Payment.Api.Controllers
{
    /// <summary>
    /// Api for check payment signature
    /// </summary>
    [Route("api/payment-transactions")]
    [ApiController]
    public class PaymentTransactionsController : ControllerBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public PaymentTransactionsController()
        {
                
        }

        /// <summary>
        /// Get transactions base on criteria
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(BaseResultWithData<List<PaymentTransactionDtos>>), 200)]
        [ProducesResponseType(400)]
        public IActionResult Get(string criteria)
        {
            var response = new BaseResultWithData<List<PaymentTransactionDtos>>();
            return Ok(response);
        }

        /// <summary>
        /// Get transactions paging
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("with-paging")]
        [ProducesResponseType(typeof(BaseResultWithData<BasePagingData<PaymentTransactionDtos>>), 200)]
        public IActionResult GetPaging([FromQuery] BasePagingQuery query)
        {
            var response = new BaseResultWithData<BasePagingData<PaymentTransactionDtos>>();
            return Ok(response);
        }

        /// <summary>
        /// Get one transaction by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(BaseResultWithData<PaymentTransactionDtos>), 200)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public IActionResult GetOne([FromRoute] string id)
        {
            var response = new BaseResultWithData<PaymentTransactionDtos>();
            return Ok(response);
        }
    }
}
