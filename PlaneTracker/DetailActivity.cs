using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
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
using static Android.Gms.Maps.GoogleMap;
using static Android.Views.View;

namespace PlaneTracker
{
    [Android.App.Activity()]
    public class DetailActivity : AppCompatActivity, IOnMapReadyCallback
    {
        private const double ZOOM_LVL1_M = 591657550.5;

        private string ICAO24;
        private TabFragment detailFragment;
        private SupportMapFragment mapFragment;
        private GoogleMap map;
        private GroundOverlay planeOverlay;

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
            detailFragment.Created += (sender, e) => UpdateDetailPage();

            mapFragment = SupportMapFragment.NewInstance();
            mapFragment.GetMapAsync(this);
            
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

                if (flight.Longitude != null && flight.Latitude != null && map != null)
                {
                    BitmapDescriptor image = BitmapDescriptorFactory.FromResource(Resource.Drawable.black_plane);
                    GroundOverlayOptions groundOverlayOptions = new GroundOverlayOptions()
                        .Position(new LatLng((double)flight.Latitude.Value, (double)flight.Longitude.Value), 5000)
                        .InvokeImage(image);
                    planeOverlay =  map.AddGroundOverlay(groundOverlayOptions);
                }
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

        public void OnMapReady(GoogleMap googleMap)
        {
            map = googleMap;
            map.CameraChange += Map_CameraChange;
            var max = new LatLng(ApiService.LATITUDE_MAX, ApiService.LONGITUDE_MAX);
            var min = new LatLng(ApiService.LATITUDE_MIN, ApiService.LONGITUDE_MIN);
            var bounds = new LatLngBounds(min, max);
            map.MoveCamera(CameraUpdateFactory.NewLatLngBounds(bounds, 0));
            map.SetLatLngBoundsForCameraTarget(bounds);
            map.SetMaxZoomPreference(50);
            map.SetMinZoomPreference(map.CameraPosition.Zoom);

            UpdateMapPage();
        }

        private void Map_CameraChange(object sender, CameraChangeEventArgs e)
        {
            ///TODO:
            planeOverlay?.SetDimensions((float)((((e.Position.Zoom - 50) * -1) * ZOOM_LVL1_M) / 1000000));
        }
    }
}