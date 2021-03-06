﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.Content.Res;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using PlaneTracker.Shared.Models;
using PlaneTracker.Shared.Services;

namespace PlaneTracker
{
    
    [Activity(Label = "@string/app_name",  MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        ListView listView;

        public MainActivity()
        {
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.main_layout);

            ApiService.Instance.StartPooling();
            SupportActionBar.SetDisplayShowHomeEnabled(true);
            SupportActionBar.SetDisplayUseLogoEnabled(true);
            SupportActionBar.SetLogo(GetDrawable(Resource.Drawable.ic_action_icon));
            
            listView = FindViewById<ListView>(Resource.Id.mainListView);
            listView.Adapter =  new FlightAdapter(this);
            listView.ItemClick += ListView_ItemClick;      
            
        }

        private void ListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (e.Position < ApiService.Instance.Flights.Count)
            {
                var intent = new Intent(this, typeof(DetailActivity));
                intent.PutExtra("flight", ApiService.Instance.Flights[e.Position].ICAO24);

                StartActivity(intent);
            }
        }
    }
}

