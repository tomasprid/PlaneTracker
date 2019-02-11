using PlaneTracker.Shared.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PlaneTracker.Shared.Services
{
    public class ApiService
    {
        private const string API_URL = "https://{0}:{1}@opensky-network.org/api";
        private const string FLIGHTS_REQUEST = "flights/all";

        private readonly string username;
        private readonly string password;
        private readonly RestClient restClient;
        

        public ApiService(string username, string password)
        {
            this.username = username;
            this.password = password;

            restClient = new RestClient(GetUrl());
        }

        public async Task<IEnumerable<Flight>> GetFlightsAsync(DateTime begin, DateTime end)
        {
            var request = new RestRequest(FLIGHTS_REQUEST);

            request.AddParameter("begin", begin.ToUnixTimestamp());
            request.AddParameter("end", end.ToUnixTimestamp());

            var response = await restClient.GetAsync<List<Flight>>(request);

            return response;
        }

        private string GetUrl()
        {
            return string.Format(API_URL, username, password);
        }
    }
}
