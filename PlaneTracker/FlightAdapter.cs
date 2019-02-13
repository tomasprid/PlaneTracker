using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using PlaneTracker.Shared.Models;
using PlaneTracker.Shared.Services;

namespace PlaneTracker
{
    public class FlightAdapter : BaseAdapter<Flight>
    {
        private readonly Activity context;

        public override Flight this[int position] => ApiService.Instance.Flights[position];
        public override int Count => ApiService.Instance.Flights.Count;

        public FlightAdapter(Activity context)
        {
            this.context = context;
            ApiService.Instance.Flights.Changed += Flights_Changed;
        }

        ~FlightAdapter()
        {
            ApiService.Instance.Flights.Changed -= Flights_Changed;
        }

        private void Flights_Changed(object sender, EventArgs e)
        {
            context.RunOnUiThread(() => NotifyDataSetChanged());
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
                text1.SetTextColor(context.GetColorStateList(Resource.Color.colorHighlight));
            }

            text1.Text = flight.Callsign;

            var detail = new List<string>();

            if (flight.OnGround)
                detail.Add(context.GetString(Resource.String.on_ground));
            else
                detail.Add(context.GetString(Resource.String.in_air));

            detail.Add(flight.GetAltitude());
            
            if (flight.LastContact != null)
                detail.Add($"{context.GetString(Resource.String.last_contact)} {flight.LastContact.Value.ToString("hh:mm")}");

            text2.Text = string.Join(", ", detail);

            return convertView;
        }
    }
}