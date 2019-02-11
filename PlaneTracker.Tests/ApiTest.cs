using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlaneTracker.Shared;
using PlaneTracker.Shared.Services;
using System;
using System.Threading.Tasks;

namespace PlaneTracker.Tests
{
    [TestClass]
    public class ApiTest
    {
        const string username = "tomasprid";
        const string password = "IwJpXQJNanebfCAb";

        [TestMethod]
        public async Task GetFlights()
        {
            var restService = new ApiService(username, password);

            var begin = new DateTime(2018, 1, 29, 12, 0, 0, DateTimeKind.Utc);
            var end = new DateTime(2018, 1, 29, 13, 0, 0, DateTimeKind.Utc);

            var flights = await restService.GetFlightsAsync(begin, end);
        }
    }
}
