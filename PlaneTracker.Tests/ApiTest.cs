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
            var restService = new ApiService();
            
            var flights = await restService.GetFlightsAsync();
        }
    }
}
