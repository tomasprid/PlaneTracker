﻿using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.Linq;
using System.Globalization;

namespace PlaneTracker.Shared.Models
{
    public class Flight
    {
        private const int ICAO24_ID = 0;
        private const int CALLSIGN_ID = 1;
        private const int ORIGIN_COUNTRY_ID = 2;
        private const int LAST_CONTACT_ID = 4;
        private const int LONGITUDE_ID = 5;
        private const int LATITUDE_ID = 6;
        private const int BARO_ALTITUDE = 7;
        private const int ON_GORUND_ID = 8;
        private const int VELOCITY_ID = 9;
        private const int DIRECTION_ID = 10;
        private const int GEO_ALTITUDE = 13;
        private const int SQUAWK_ID = 14;

        public event EventHandler Updated;

        public string ICAO24 { get; set; }
        public string Callsign { get; set; }
        public string OriginCountry { get; set; }
        public DateTime? LastContact { get; set; }
        /// <summary>
        /// In meters
        /// </summary>
        public decimal? Altitude { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? Latitude { get; set; }
        public bool OnGround { get; set; }
        /// <summary>
        /// In m/s
        /// </summary>
        public decimal? Velocity { get; set; }
        public string Squawk { get; set; }
        public decimal? Direction { get; set; }

        public Flight()
        { }

        public override int GetHashCode()
        {
            return ICAO24.GetHashCode();
        }

        /// <summary>
        /// Update current Flight with newly received object
        /// </summary>
        /// <param name="flight">New object data</param>
        public void Update(Flight flight)
        {
            LastContact = flight.LastContact;
            if(flight.Latitude != null)
                Latitude = flight.Latitude;
            if(flight.Longitude != null)
                Longitude = flight.Longitude;
            if(flight.Altitude != null)
                Altitude = flight.Altitude;
            OnGround = flight.OnGround;
            if(flight.Velocity != null)
                Velocity = flight.Velocity;
            Squawk = flight.Squawk;

            Updated?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>Format altitude for current region info</summary>
        /// <param name="shortPostFix">Lenght of postfix e.g. for metric system short "m" long "meters""  </param>
        /// <returns>Formated altitude in meters or miles</returns>
        public string GetAltitude(string meters = null, string feets = null)
        {
            if (Altitude != null)
            {
                var regionInfo = RegionInfo.CurrentRegion;

                string postfix;
                if (regionInfo.IsMetric)
                {
                    postfix = string.IsNullOrEmpty(meters) ? 
                        "m" : meters;
                    return $"{Altitude} {postfix}";
                }
                else
                {
                    postfix = string.IsNullOrEmpty(feets) ? 
                        "ft" : feets;
                    return $"{Altitude} m,{ Math.Round((decimal)Altitude * 3.2808m, 0)} {postfix}";
                }
            }
            return null;
        }

        /// <summary>
        /// Format velocity for current region info
        /// </summary>
        /// <returns>Formated velocity in km/h or kt</returns>
        public string GetVelocity()
        {
            if (Velocity != null)
            {
                var regionInfo = RegionInfo.CurrentRegion;
                
                if (regionInfo.IsMetric)
                {
                    return $"{Math.Round((decimal)Velocity * 3.6m,2)} km/h";
                }
                else
                {
                    return $"{Math.Round((decimal)Velocity * 1.943844m, 2)} kt";
                }
            }
            return null;
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
                Longitude = TryParseDecimal(data[LONGITUDE_ID]),
                Latitude = TryParseDecimal(data[LATITUDE_ID]),
                Altitude = TryParseDecimal(data[BARO_ALTITUDE], data[GEO_ALTITUDE]),
                OnGround = Convert.ToBoolean(data[ON_GORUND_ID]),
                Velocity = TryParseDecimal(data[VELOCITY_ID]),
                Direction = TryParseDecimal(data[DIRECTION_ID]),
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

        /// <summary>
        /// Parse string unix value to DateTime
        /// </summary>
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
