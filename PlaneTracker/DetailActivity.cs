using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Java.Lang;
using PlaneTracker.Shared.Services;
using static Android.Views.View;

namespace PlaneTracker
{
    [Android.App.Activity()]
    public class DetailActivity : AppCompatActivity
    {
        private string ICAO24;
        private TabFragment detailFragment, mapFragment;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            SetContentView(Resource.Layout.tab_layout);
            ICAO24 = Intent.GetStringExtra("flight");

            SupportActionBar.Title = ICAO24;
            SupportActionBar.SetDisplayShowHomeEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayUseLogoEnabled(true);
            SupportActionBar.SetLogo(GetDrawable(Resource.Drawable.black_plane));

            var tabLayout = FindViewById<TabLayout>(Resource.Id.tabLayout);
            var viewPager = FindViewById<ViewPager>(Resource.Id.viewPager);

            var adapter = new TabAdapter(SupportFragmentManager);

            detailFragment = new TabFragment(Resource.Layout.detail_layout);
            mapFragment = new TabFragment(Resource.Layout.map_layout);

            detailFragment.Created += (sender, e) => UpdateDetailPage();
            mapFragment.Created += (sender, e) => UpdateMapPage();

            adapter.AddFragment(detailFragment , "Informace");
            adapter.AddFragment(mapFragment, "Mapa");
           
            viewPager.Adapter = adapter;
            
            ApiService.Instance.Flights.Updated += Flights_Updated;

            tabLayout.SetupWithViewPager(viewPager);      
        }

        private void Flights_Updated(object sender, EventArgs e)
        {
            UpdateDetailPage();
            UpdateMapPage();
        }

        private void UpdateMapPage()
        {
            if (ApiService.Instance.Flights.Contains(ICAO24))
            {
                var flight = ApiService.Instance.Flights[ICAO24];
            }
        }

        private void UpdateDetailPage()
        {
            if (ApiService.Instance.Flights.Contains(ICAO24))
            {
                var flight = ApiService.Instance.Flights[ICAO24];
                detailFragment.View.FindViewById<TextView>(Resource.Id.callsignLabel).Text = flight.Callsign;
                detailFragment.View.FindViewById<TextView>(Resource.Id.stateLabel).Text = flight.OnGround ? "Na zemi" : "Letí";
                detailFragment.View.FindViewById<TextView>(Resource.Id.countryLabel).Text = flight.OriginCountry;
                detailFragment.View.FindViewById<TextView>(Resource.Id.altitudeLabel).Text = flight.GetAltitude(false);
                detailFragment.View.FindViewById<TextView>(Resource.Id.speedLabel).Text = flight.GetVelocity();
                detailFragment.View.FindViewById<TextView>(Resource.Id.squawkLabel).Text = flight.Squawk;
            }
        }

        public override bool OnSupportNavigateUp()
        {
            ApiService.Instance.Flights.Updated -= Flights_Updated;
            OnBackPressed();
            return true;
        }
    }
}