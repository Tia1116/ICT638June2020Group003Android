using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;

namespace ICT638June2020Group003Android
{
    [Activity(Label = "Navigation")]
    public class Navigation : Activity, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        private TextView textMessage;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Navigation navigation1 = this;
            Xamarin.Essentials.Platform.Init(navigation1, savedInstanceState);
            SetContentView(Resource.Layout.Navigation);

            textMessage = FindViewById<TextView>(Resource.Id.message);
            BottomNavigationView bNavView = FindViewById<BottomNavigationView>(Resource.Id.navigation);
            bNavView.SetOnNavigationItemSelectedListener(this);

        }
        public bool OnNavigationItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.navigation_House:
                    return true;
                case Resource.Id.navigation_Agent:
                    SetContentView(Resource.Layout.House);

                    return true;
                case Resource.Id.navigation_User:
                    StartActivity(typeof(houseActivity));
                    return true;
            }
            return false;
        }
    }
}