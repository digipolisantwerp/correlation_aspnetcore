using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Toolbox.Correlation;
using System.Net.Http;
using Microsoft.AspNet.Http;
using SampleApi1.ServiceAgents;

namespace SampleApi1.Controllers
{
    [Route("api/[controller]")]
    public class ValuesController : Controller
    {
        private ICorrelationContext _context;
        private SampleApi2Agent _sampleAgent;

        public ValuesController(ICorrelationContext context, SampleApi2Agent sampleAgent)
        {
            _context = context;
            _sampleAgent = sampleAgent;
        }

        // GET: api/values
        [HttpGet]
        public string Get()
        {
            return $"Result form SampleApi1: CorrelationId = {_context.CorrelationId.ToString()}, CorrelationSource = {_context.CorrelationSource}";
        }

        // To pass the correlation when calling another api using an HttpClient.
        // GET api/values
        [HttpGet("{id}")]
        public async Task<string> Get(int id)
        {
            //create request object
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri("http://localhost:5001/api/Values"),
                Method = HttpMethod.Get,
            };

            //set headers on the request using an extension method
            request.SetCorrelationValues(_context);

            //send the request
            var client = new HttpClient();
            var result = await client.SendAsync(request);

            return await result.Content.ReadAsStringAsync();
        }

        // To pass the correlation when calling another api using a serviceAgent. This is the preffered way!
        // GET api/values/UseServiceAgent
        [HttpGet("UseServiceAgent")]
        public async Task<string> UseServiceAgent()
        {

            //just use the service agent. The wiring for the correlation is done in the ConfigureServices method in Startup.cs.
            var result =  await _sampleAgent.GetMessage();

            return result.Value;
        }

    }
}
