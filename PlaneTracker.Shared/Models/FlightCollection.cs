using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlaneTracker.Shared.Models
{
    public class FlightCollection
    {
        private class FlightComparer : IEqualityComparer<Flight>
        {
            public bool Equals(Flight x, Flight y)
            {
                return x.ICAO24 == y.ICAO24;
            }

            public int GetHashCode(Flight obj)
            {
                return obj.GetHashCode();
            }
        }

        private readonly Dictionary<string, Flight> flights = new Dictionary<string, Flight>();

        public event EventHandler Updated;
        public event EventHandler Changed;

        public Flight this[string ICAO24] => flights[ICAO24];
        public Flight this[int i] => flights.Values.ToArray()[i];
        public int Count => flights.Count;

        public bool Contains(string icao24)
        {
            return flights.ContainsKey(icao24);
        }

        public void Update(IEnumerable<Flight> newFlights)
        {
            var comparer = new FlightComparer();

            var removed = flights.Where(flight =>
            !newFlights.Contains(flight.Value, comparer)).
            Select(flight => flight.Key);

            foreach (var newFlight in newFlights)
            {
                if (flights.ContainsKey(newFlight.ICAO24))
                    flights[newFlight.ICAO24].Update(newFlight);
                else
                    flights.Add(newFlight.ICAO24, newFlight);
                Changed?.Invoke(this, EventArgs.Empty);
            }

            foreach (var remove in removed)
            {
                flights.Remove(remove);
                Changed?.Invoke(this, EventArgs.Empty);
            }

            Updated?.Invoke(this, EventArgs.Empty);
        }
    }
}
