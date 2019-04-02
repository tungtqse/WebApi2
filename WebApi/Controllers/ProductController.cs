using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApi.Controllers
{
    [Authorize]
    public class ProductController : ApiController
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // POST api/values
        public IHttpActionResult Post([FromBody]  ApplicationAPI.APIs.Product.CreateApi.Command command)
        {
            try
            {
                var model = _mediator.Send(command);
                return Ok();           
            }
            catch (Exception e)
            {
                var response = e.Message;
                return Json(response);
            }
        }
    }
}
