using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService
{
    public class FunctionConfiguration
    {
        public string CosmosAccountEndpoint { get; }
        public string CosmosAccountKey { get; }
        public string CosmosDatabaseName { get; }

        public FunctionConfiguration(IConfiguration config)
        {
            CosmosAccountEndpoint = config["CosmosAccountEndpoint"];
            CosmosAccountKey = config["CosmosAccountKey"];
            CosmosDatabaseName = config["CosmosDatabaseName"];
        }
    }
}
