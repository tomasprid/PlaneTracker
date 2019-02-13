using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace PlaneTracker
{
    public class TabFragment : Android.Support.V4.App.Fragment
    {
        private readonly int layout;

        public event EventHandler Created;

        public TabFragment(int layout)
        {
            this.layout = layout;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(layout, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            Created?.Invoke(this, EventArgs.Empty);
        }
    }
}