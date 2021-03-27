using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;

namespace RabbitMQ.Producer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        

        private readonly ILogger<Product> _logger;
        private IRabbitManager _manager;


        public ProductController(ILogger<Product> logger, IRabbitManager manager)
        {
            _logger = logger;
            _manager = manager;
        }

        [HttpPost]
        [SwaggerOperation("ValidationProviderProfilePost")]
        [SwaggerResponse(statusCode: 200, type: typeof(Product), description: "Call is successful")]
        public IActionResult PostMessage([FromBody] Product body)
        {
            _manager.Publish(body, "demo.exchange.dotnetcore", "product-queue", "queue.durable.dotnetcore");
            return Ok("Message Published Successfully..!");
           
        }
    }
}
