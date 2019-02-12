using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using PlaneTracker.Shared.Models;
using PlaneTracker.Shared.Services;

namespace PlaneTracker
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : ListActivity
    {
        private readonly ApiService apiService = new ApiService();
        private FlightAdapter flightAdapter;

        public MainActivity()
        {
            apiService.FlightsReceived += ApiService_FlightsReceived;
            
        }

        private void ApiService_FlightsReceived(object sender, IEnumerable<Flight> e)
        {
            RunOnUiThread(() =>
            {
                flightAdapter?.Update(e);
            });
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.ListAdapter = flightAdapter = new FlightAdapter(this);
            apiService.StartPooling();
        }
	}

    public class FlightAdapter : BaseAdapter<Flight>
    {
        private static readonly ColorStateList HighlightColor = ColorStateList.ValueOf(Android.Graphics.Color.Red);

        private readonly Dictionary<string, Flight> flights = new Dictionary<string, Flight>();
        private readonly Activity context;

        public override Flight this[int position] => flights.Values.ToArray()[position];
        public override int Count => flights.Count;

        public FlightAdapter(Activity context)
        {
            this.context = context;
        }

        public void Update(IEnumerable<Flight> newFlights)
        {
            foreach (var newFlight in newFlights)
            {
                flights[newFlight.ICAO24] = newFlight;
            }

            NotifyDataSetChanged();
        }
        
        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var flight = this[position];

            convertView = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem2, null);

            var text1 = convertView.FindViewById<TextView>(Android.Resource.Id.Text1);
            var text2 = convertView.FindViewById<TextView>(Android.Resource.Id.Text2);

            if (flight.Callsign.StartsWith("OK") ||
                flight.Callsign.StartsWith("OM"))
            {
                text1.SetTextColor(HighlightColor);
            }

            text1.Text = flight.Callsign;

            var detail = new StringBuilder();

            if (flight.OnGround)
                detail.Append("Na zemi ");
            else
                detail.Append("ve vzduchu ");

            if (flight.Altitude != null)
                detail.Append($",{flight.Altitude} m");

            if (flight.LastContact != null)
                detail.Append($", poslední kontakt {flight.LastContact.Value.ToString("hh:mm")}");

            text2.Text = detail.ToString();

            return convertView;
        }
    }
}

