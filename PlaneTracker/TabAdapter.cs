using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Java.Lang;

namespace PlaneTracker
{
    public class TabAdapter : FragmentStatePagerAdapter
    {
        private List<Android.Support.V4.App.Fragment> fragmentList = new List<Android.Support.V4.App.Fragment>();
        private List<string> fragmentTitleList = new List<string>();

        public override int Count => fragmentList.Count;

        public TabAdapter(Android.Support.V4.App.FragmentManager fm) : base(fm)
        {
        }

        public override Android.Support.V4.App.Fragment GetItem(int position)
        {
            return fragmentList[position];
        }

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            return CharSequence.ArrayFromStringArray(fragmentTitleList.ToArray())[position];
        }

        public void AddFragment(Android.Support.V4.App.Fragment fragment, string title)
        {
            fragmentList.Add(fragment);
            fragmentTitleList.Add(title);
        }
    }
}