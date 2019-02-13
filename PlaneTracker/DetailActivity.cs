using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
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
        private string ICAO24;
        private DetailFragment detailFragment;
        private SupportMapFragment mapFragment;
        private GoogleMap map;
        private GroundOverlay planeOverlay;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            SetContentView(Resource.Layout.tab_layout);
            ICAO24 = Intent.GetStringExtra("flight");
            var flight = ApiService.Instance.Flights[ICAO24];

            SupportActionBar.Title = flight.Callsign;
            SupportActionBar.SetDisplayShowHomeEnabled(true);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayUseLogoEnabled(true);
            SupportActionBar.SetLogo(GetDrawable(Resource.Drawable.ic_action_icon));

            var tabLayout = FindViewById<TabLayout>(Resource.Id.tabLayout);
            var viewPager = FindViewById<ViewPager>(Resource.Id.viewPager);

            var adapter = new TabAdapter(SupportFragmentManager);
            detailFragment = new DetailFragment();
            detailFragment.Created += (sender, e) => UpdateDetailPage();

            mapFragment = SupportMapFragment.NewInstance();
            mapFragment.GetMapAsync(this);
            mapFragment.RetainInstance = true;

            adapter.AddFragment(detailFragment , GetString(Resource.String.details));
            adapter.AddFragment(mapFragment, GetString(Resource.String.map));
           
            viewPager.Adapter = adapter;
            
            ApiService.Instance.Flights.Updated += Flights_Updated;

            tabLayout.SetupWithViewPager(viewPager);      
        }

        private void Flights_Updated(object sender, EventArgs e)
        {
            RunOnUiThread(() =>
            {
                UpdateDetailPage();
                UpdateMapPage();
            });
        }

        private void UpdateMapPage()
        {
            if (ApiService.Instance.Flights.Contains(ICAO24))
            {
                var flight = ApiService.Instance.Flights[ICAO24];

                if (flight.Longitude != null && flight.Latitude != null && map != null)
                {
                    var position = new LatLng((double)flight.Latitude.Value, (double)flight.Longitude.Value);
                    var planeDimension = GetPlaneDimension((float)flight.Latitude.Value, map.CameraPosition.Zoom);
                    var image = CreateRotatedPlaneImage((float?)flight.Direction ?? 0);

                    if (planeOverlay == null)
                    {
                        GroundOverlayOptions groundOverlayOptions = new GroundOverlayOptions()
                            .Position(position, planeDimension)
                            .InvokeImage(image);

                        planeOverlay = map.AddGroundOverlay(groundOverlayOptions);
                    }
                    else
                    {
                        planeOverlay.Position = position;
                        planeOverlay.SetImage(image);
                        planeOverlay.SetDimensions(planeDimension);
                    }
                }
            }
        }

        public BitmapDescriptor CreateRotatedPlaneImage(float angle)
        {
            var bitmap = BitmapFactory.DecodeResource(Resources, Resource.Drawable.black_plane);
            bitmap = RotateBitmap(bitmap, 90);

            BitmapDescriptor image = BitmapDescriptorFactory.FromBitmap(bitmap);

            return image;
        }

        public static Bitmap RotateBitmap(Bitmap source, float angle)
        {
            Matrix matrix = new Matrix();
            matrix.PostRotate(angle);
            return Bitmap.CreateBitmap(source, 0, 0, source.Width, source.Height, matrix, true);
        }

        private void UpdateDetailPage()
        {
            if (ApiService.Instance.Flights.Contains(ICAO24))
            {
                var flight = ApiService.Instance.Flights[ICAO24];

                detailFragment.View.FindViewById<TextView>(Resource.Id.callsignLabel).Text = flight.Callsign;
                detailFragment.View.FindViewById<TextView>(Resource.Id.stateLabel).Text = flight.OnGround ? GetString(Resource.String.on_ground) : GetString(Resource.String.in_air);
                detailFragment.View.FindViewById<TextView>(Resource.Id.countryLabel).Text = flight.OriginCountry;
                var altitude = flight.GetAltitude(GetString(Resource.String.meters, Resource.String.miles));
                if (string.IsNullOrEmpty(altitude))
                    altitude = GetString(Resource.String.not_avalible);
                detailFragment.View.FindViewById<TextView>(Resource.Id.altitudeLabel).Text = altitude;
                var velocity = flight.GetVelocity();
                if (string.IsNullOrEmpty(velocity))
                    velocity = GetString(Resource.String.not_avalible);
                detailFragment.View.FindViewById<TextView>(Resource.Id.speedLabel).Text = velocity;
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
            planeOverlay?.SetDimensions(GetPlaneDimension((float)planeOverlay.Position.Latitude ,e.Position.Zoom));
        }

        private float GetPlaneDimension(float latitude, float zoom)
        {
            return (float)(156543.03392 * System.Math.Cos(latitude * System.Math.PI / 180) / System.Math.Pow(2, zoom)) * 30;
        }
    }
}