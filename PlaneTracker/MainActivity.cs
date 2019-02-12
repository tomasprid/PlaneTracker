using System;
using System.Collections.Generic;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using PlaneTracker.Shared.Models;

namespace PlaneTracker
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : ListActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var phoneNumbers = new Flight[] { };
            this.ListAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, phoneNumbers);
        }
	}

    public class FlightAdapter : BaseAdapter<Flight>
    {
        private readonly IEnumerable<Flight> flights;
        private readonly Activity context;

        public override Flight this[int psoition] => flights[position];
        public override int Count => flights.Count();

        public FlightAdapter(Activity context, IEnumerable<Flight> flights)
        {
            this.context = context;
            this.flights = flights;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var flight = flights[position];
            if (convertView == null) 
                convertView = context.LayoutInflater.Inflate(Resource.Layout.CustomView, null);
            var text1 = view.FindViewById<TextView>(Resource.Id.Text1);
            var text2 = view.FindViewById<TextView>(Resource.Id.Text2);

        }
    }
}

