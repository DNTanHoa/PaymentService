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
    [Route("api/payment-signatures")]
    [ApiController]
    public class PaymentSignaturesController : ControllerBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public PaymentSignaturesController()
        {
                
        }

        /// <summary>
        /// Get signatures base on criteria
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(BaseResultWithData<List<PaymentSignatureDtos>>), 200)]
        [ProducesResponseType(400)]
        public IActionResult Get(string criteria)
        {
            var response = new BaseResultWithData<List<PaymentSignatureDtos>>();
            return Ok(response);
        }

        /// <summary>
        /// Get signatures paging
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("with-paging")]
        [ProducesResponseType(typeof(BaseResultWithData<BasePagingData<PaymentSignatureDtos>>), 200)]
        public IActionResult GetPaging([FromQuery] BasePagingQuery query)
        {
            var response = new BaseResultWithData<BasePagingData<PaymentSignatureDtos>>();
            return Ok(response);
        }

        /// <summary>
        /// Get one signature by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(BaseResultWithData<PaymentSignatureDtos>), 200)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public IActionResult GetOne([FromRoute] string id)
        {
            var response = new BaseResultWithData<PaymentSignatureDtos>();
            return Ok(response);
        }
    }
}
