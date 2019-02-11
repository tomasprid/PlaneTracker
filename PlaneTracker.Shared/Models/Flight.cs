using System;
using System.Collections.Generic;
using System.Text;

namespace PlaneTracker.Shared.Models
{
    public class Flight
    {
        public string ICAO24 { get; set; }
        public DateTime FirstSeen { get; set; }
        public DateTime LastSeen { get; set; }
        public string EstDepartureAirport { get; set; }
        public string EstArrivalAirport { get; set; }
        public string Callsign { get; set; }
        public int EstDepartureAirportHorizDistance { get; set; }
        public int EstDepartureAirportVertDistance { get; set; }
        public int EstArrivalAirportHorizDistance { get; set; }
        public int EstArrivalAirportVertDistance { get; set; }
        public int DepartureAirportCandidatesCount {get; set;}
        public int ArrivalAirportCandidatesCount { get; set; }
    }
}
