using bfyoc_objects_library;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace bfyoc_clients_library
{
    public class UserClient
    {
        private string userUrl = "https://serverlessohuser.trafficmanager.net/api/GetUser";
        private static HttpClient httpClient = new HttpClient();

        public async Task<User> RetrieveUserAsync(Guid userId)
        {
            var finalProductUrl = string.Format("{0}?userId={1}", userUrl, userId.ToString());
            var responseText = await httpClient.GetStringAsync(finalProductUrl);
            var user = JsonConvert.DeserializeObject<User>(responseText);
            return user;
        }
    }
}
