using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace bfyoc_rating_api
{
    [JsonObject(MemberSerialization.OptIn)]
    public class BatchTracker
    {
        [JsonProperty("value")]
        public int Value { get; set; }

        public void Add()
        {
            this.Value += 1;
        }

        public Task Reset()
        {
            this.Value = 0;
            return Task.CompletedTask;
        }

        public Task<int> Get()
        {
            return Task.FromResult(this.Value);
        }

        public void Delete()
        {
            Entity.Current.DeleteState();
        }

        [FunctionName("BatchTracker")]
        public static Task Run([EntityTrigger] IDurableEntityContext ctx)
            => ctx.DispatchAsync<BatchTracker>();
    }
}
