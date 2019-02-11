using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlaneTracker.Shared
{
    public static class ApiHelper
    {
        const string root = "https://tomasprid:IwJpXQJNanebfCAb@opensky-network.org/api";

        public static void LoadFlightsByTime()
        {
            var client = new RestClient(root);

            var request = new RestRequest("flights/all");
            var start = DateTime.UtcNow.AddSeconds(-5);
            var end = DateTime.UtcNow;
            
            request.AddParameter("begin", start.ToUnixTimestamp());
            request.AddParameter("end", end.ToUnixTimestamp());

            var response = client.Get(request);

        }
    }
}
