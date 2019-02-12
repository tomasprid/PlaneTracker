using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.Linq;

namespace PlaneTracker.Shared.Models
{
    public class Flight
    {
        private const int ICAO24_ID = 0;
        private const int CALLSIGN_ID = 1;
        private const int ORIGIN_COUNTRY_ID = 2;
        private const int LAST_CONTACT_ID = 4;
        private const int BARO_ALTITUDE = 7;
        private const int ON_GORUND_ID = 8;
        private const int VELOCITY_ID = 9;
        private const int GEO_ALTITUDE = 13;
        private const int SQUAWK_ID = 14;

        public string ICAO24 { get; set; }
        public string Callsign { get; set; }
        public string OriginCountry { get; set; }
        public DateTime? LastContact { get; set; }
        public decimal? Altitude { get; set; }
        public bool OnGround { get; set; }
        public decimal? Velocity { get; set; }
        public string Squawk { get; set; }

        public Flight()
        { }

        public override int GetHashCode()
        {
            return ICAO24.GetHashCode();
        }

        public void Update(Flight flight)
        {
            LastContact = flight.LastContact;
            Altitude = flight.Altitude;
            OnGround = flight.OnGround;
            Velocity = flight.Velocity;
            Squawk = flight.Squawk;
        }

        /// <summary>
        /// Parse from sorted array
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Flight FromArray(string[] data)
        {
            return new Flight
            {
                ICAO24 = data[ICAO24_ID],
                Callsign = data[CALLSIGN_ID],
                OriginCountry = data[ORIGIN_COUNTRY_ID],
                LastContact = GetDateTime(data[LAST_CONTACT_ID]),
                Altitude = TryParseDecimal(data[BARO_ALTITUDE], data[GEO_ALTITUDE]),
                OnGround = Convert.ToBoolean(data[ON_GORUND_ID]),
                Velocity = TryParseDecimal(data[VELOCITY_ID]),
                Squawk = data[SQUAWK_ID]
            };
        }
        
        /// <summary>
        /// Try parse values to decimal
        /// </summary>
        /// <param name="values">String decimal values</param>
        /// <returns>First value that is successfully parsed, if no value is parsed returns null </returns>
        private static decimal? TryParseDecimal(params string[] values)
        {
            foreach (var value in values)
            {
                decimal output;
                if (decimal.TryParse(value, out output))
                {
                    return output;
                }
            }

            return null;
        }

        private static DateTime? GetDateTime(string value)
        {
            double unixTimeStamp;
            if (double.TryParse(value, out unixTimeStamp))
            {
                DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
                return dateTime;
            }
            else return null;
        }
    }
}
