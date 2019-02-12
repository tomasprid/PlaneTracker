using PlaneTracker.Shared.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PlaneTracker.Shared.Services
{
    public class ApiService
    {
        private class StateResponse
        {
            public DateTime Time { get; set; }
            public IEnumerable<IEnumerable<string>> States { get; set; }
        }

        private const string API_URL = "https://opensky-network.org/api/";
        private const string STATES_ALL = "states/all";
        private const float LATITUDE_MIN = 48.73881f;
        private const float LATITUDE_MAX = 51.00369f;
        private const float LONGITUDE_MIN = 12.19499f;
        private const float LONGITUDE_MAX = 18.76458f;
        private const int REFRESH_DELAY = 15000;

        private readonly RestClient restClient;
        private readonly object sync = new object();

        private CancellationToken poolingCancellationToken;

        public event EventHandler<IEnumerable<Flight>> FlightsReceived;

        public bool IsPoolingData { get; private set; }
       
        public ApiService()
        {
            restClient = new RestClient(API_URL);
        }

        public void StartPooling()
        {
            if (!IsPoolingData)
            {
                poolingCancellationToken = new CancellationToken();
                Task.Run(PoolingTask,poolingCancellationToken);
            }
            else throw new Exception("Already pooling.");
        }

        public void StopPooling()
        {
            if (IsPoolingData)
            {
                try
                {
                    poolingCancellationToken.ThrowIfCancellationRequested();
                }
                catch (OperationCanceledException)
                {
                    IsPoolingData = false;
                }
            }
            else throw new Exception("Pooling is not running");
        }
        
        public async Task<IEnumerable<Flight>> GetFlightsAsync()
        {
            var request = new RestRequest(STATES_ALL);

            request.AddParameter("lamin", LATITUDE_MIN);
            request.AddParameter("lomin", LONGITUDE_MIN);
            request.AddParameter("lamax", LATITUDE_MAX);
            request.AddParameter("lomax", LONGITUDE_MAX);

            var response = await restClient.GetAsync<StateResponse>(request);

            if (response.States == null)
                return Enumerable.Empty<Flight>();

            var flightList = new List<Flight>();

            foreach (var state in response.States)
            {
                flightList.Add(Flight.FromArray(state.ToArray()));
            }

            return flightList;
        }

        private async Task PoolingTask()
        {
            while (true)
            {
                var flights = await GetFlightsAsync();

                lock (sync)
                {
                    FlightsReceived?.Invoke(this, flights);
                }

                await Task.Delay(REFRESH_DELAY);
            }
        }
    }
}
