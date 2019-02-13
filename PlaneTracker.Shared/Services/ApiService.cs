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
        private static ApiService instance;

        public static ApiService Instance => instance ?? (instance = new ApiService());

        private class StateResponse
        {
            public DateTime Time { get; set; }
            public IEnumerable<IEnumerable<string>> States { get; set; }
        }

        private const string API_URL = "https://opensky-network.org/api/";
        private const string STATES_ALL = "states/all";
        private const int REFRESH_DELAY = 15000;

        public const float LATITUDE_MIN = 48.73881f;
        public const float LATITUDE_MAX = 51.00369f;
        public const float LONGITUDE_MIN = 12.19499f;
        public const float LONGITUDE_MAX = 18.76458f;
        
        private readonly RestClient restClient;
        private readonly System.Timers.Timer poolTimer;
        public FlightCollection Flights { get; }
       
        private ApiService()
        {
            restClient = new RestClient(API_URL);
            Flights = new FlightCollection();
            poolTimer = new System.Timers.Timer(REFRESH_DELAY);
            poolTimer.AutoReset = true;
            poolTimer.Elapsed += PoolTimer_Elapsed;
        }

        public void StartPooling()
        {
            Task.Run(Pool);
            poolTimer.Start();
        }

        public void StopPooling()
        {
            poolTimer.Stop();
        }

        private void PoolTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Task.Run(Pool);
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

        private async Task Pool()
        {
            var flights = await GetFlightsAsync();

            Flights.Update(flights);
        }
    }
}
