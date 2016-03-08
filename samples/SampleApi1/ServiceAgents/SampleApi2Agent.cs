using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.OptionsModel;
using Toolbox.ServiceAgents;
using Toolbox.ServiceAgents.Settings;

namespace SampleApi1.ServiceAgents
{
    public class SampleApi2Agent : AgentBase
    {
        public SampleApi2Agent(IServiceProvider serviceProvider, IOptions<ServiceAgentSettings> options) 
            : base(serviceProvider, options, "SampleApi2Agent")
        {
            
        }

        public Task<SampleApi2Model> GetMessage()
        {
            return GetAsync<SampleApi2Model>("values");
        }
    }
}
