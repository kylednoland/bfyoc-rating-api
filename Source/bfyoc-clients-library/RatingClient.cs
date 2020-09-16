using bfyoc_objects_library;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bfyoc_clients_library
{
    public class RatingClient
    {
        private static string cosmosEndpointUri = "https://serverlessopenhack.documents.azure.com:443/";

        private static string primaryKey = "G0INAL2odWOocg60rgmogpJ4hxug2oY173JY6b3fGjtHAHQpQoWr9xlZnpqoMqrmdzZXtRscS23BUopcHk3L5w==";

        private static string databaseID = "bfyoc";

        private static string ratingContainerID = "Rating";
        
        private static string ratingPartitionKeyName = "locationName";

        private CosmosClient cosmosClient;
        public async Task CreateRatingAsync(Rating rating)
        {
            this.cosmosClient = new CosmosClient(cosmosEndpointUri, primaryKey);
            var container = cosmosClient.GetContainer(databaseID, ratingContainerID);
            await container.CreateItemAsync<Rating>(rating);
        }

        public async Task<Rating> RetrieveRatingAsync(Guid ratingId)
        {
            var queryText = string.Format("select * from c where c.id = \"{0}\"", ratingId.ToString());
            var ratings = await this.RetrieveItemsAsync<Rating>(ratingContainerID, queryText);
            return ratings.First();
        }

        public async Task<List<Rating>> RetrieveRatingsAsync(Guid userId)
        {
            var queryText = string.Format("select * from c where c.userId = \"{0}\"", userId.ToString());
            var ratings = await this.RetrieveItemsAsync<Rating>(ratingContainerID, queryText);
            return ratings;
        }

        private async Task<List<T>> RetrieveItemsAsync<T>(string containerId, string queryText)
        {
            this.cosmosClient = new CosmosClient(cosmosEndpointUri, primaryKey);
            var container = cosmosClient.GetContainer(databaseID, containerId);
            var queryDefinition = new QueryDefinition(queryText);

            var iterator = container.GetItemQueryIterator<T>(queryDefinition);

            var returnValue = new List<T>();
            while (iterator.HasMoreResults)
            {
                var resultSet = await iterator.ReadNextAsync();
                foreach (T t in resultSet)
                {
                    returnValue.Add(t);
                }
            }
            return returnValue;
        }
    }
}
