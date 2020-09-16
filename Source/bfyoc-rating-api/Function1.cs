using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace bfyoc_rating_api
{
    public static class Function1
    {
        [FunctionName("GetRatings")]
        public static async Task<IActionResult> GetUserRatings(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "user/{id}/ratings")] HttpRequest req,
            ILogger log,
            string id)
        {
            if(Guid.TryParse(id, out Guid userId))
            {
                // Check user Id.
                // Retrieve ratings
            }
            else
            {
                return new BadRequestObjectResult("The user id is not a GUID.");
            }

            // string name = req.Query["name"];

            // string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            // dynamic data = JsonConvert.DeserializeObject(requestBody);
            // name = name ?? data?.name;

            // string responseMessage = string.IsNullOrEmpty(name)
            //     ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
            //     : $"Hello, {name}. This HTTP triggered function executed successfully.";

            return new OkResult();//OkObjectResult(responseMessage);
        }
    }
}
