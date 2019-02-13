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
        private static readonly ColorStateList HighlightColor = ColorStateList.ValueOf(Android.Graphics.Color.Red);

        private readonly Activity context;

        public override Flight this[int position] => ApiService.Instance.Flights[position];
        public override int Count => ApiService.Instance.Flights.Count;

        public FlightAdapter(Activity context)
        {
            this.context = context;
            ApiService.Instance.Flights.Updated += Flights_Updated;
        }

        ~FlightAdapter()
        {
            ApiService.Instance.Flights.Updated -= Flights_Updated;
        }

        private void Flights_Updated(object sender, EventArgs e)
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
                text1.SetTextColor(HighlightColor);
            }

            text1.Text = flight.Callsign;

            var detail = new StringBuilder();

            if (flight.OnGround)
                detail.Append("Na zemi");
            else
                detail.Append("ve vzduchu");

            detail.AppendFormat(" ,{0}", flight.GetAltitude());
            
            if (flight.LastContact != null)
                detail.Append($", poslední kontakt {flight.LastContact.Value.ToString("hh:mm")}");

            text2.Text = detail.ToString();

            return convertView;
        }
    }
}