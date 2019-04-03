using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApi.Common;

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
        [HttpPost]
        public IHttpActionResult Create([FromBody] ApplicationAPI.APIs.Product.CreateApi.Command command)
        {
            try
            {
                var model = _mediator.Send(command);
                return Json(model);           
            }
            catch (Exception e)
            {
                var response = new ApplicationAPI.APIs.Product.CreateApi.CommandResponse();
                response.Messages.Add(Constant.CommonError);
                response.Messages.Add(e.Message);
                response.Code = 500;
                return Json(response);
            }
        }

        // PUT api/values/5
        [HttpPut]
        public IHttpActionResult Edit([FromBody] ApplicationAPI.APIs.Product.UpdateApi.Command command)
        {
            try
            {
                var model = _mediator.Send(command);
                return Json(model);
            }
            catch (Exception e)
            {
                var response = new ApplicationAPI.APIs.Product.UpdateApi.CommandResponse();
                response.Messages.Add(Constant.CommonError);
                response.Messages.Add(e.Message);
                response.Code = 500;
                return Json(response);
            }
        }

        public IHttpActionResult Get(Guid id)
        {
            try
            {
                var model = _mediator.Send(new ApplicationAPI.APIs.Product.GetDetailApi.Query() { Id = id});
                return Json(model);
            }
            catch (Exception e)
            {
                var response = new ApplicationAPI.APIs.Product.GetDetailApi.Result();
                response.Messages.Add(Constant.CommonError);
                response.Messages.Add(e.Message);
                response.Code = 500;
                return Json(response);
            }
        }

        [HttpPost]
        public IHttpActionResult Search([FromBody] ApplicationAPI.APIs.Product.SearchApi.Query command)
        {
            try
            {
                var model = _mediator.Send(command);
                return Json(model);
            }
            catch (Exception e)
            {
                var response = new ApplicationAPI.APIs.Product.SearchApi.Result();
                response.Messages.Add(Constant.CommonError);
                response.Messages.Add(e.Message);
                response.Code = 500;
                return Json(response);
            }
        }
    }
}
