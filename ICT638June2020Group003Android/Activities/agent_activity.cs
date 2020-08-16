using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Essentials;

namespace ICT638June2020Group003Android.Activities
{
    [Activity(Label = "agent_activity")]
    public class agent_activity : Activity, IOnMapReadyCallback //BottomNavigationView.IOnNavigationItemSelectedListener
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Agent_Layout);
            var mapFrag = MapFragment.NewInstance();// mapOptions);


            FragmentManager.BeginTransaction()
                                      .Add(Resource.Id.mapContainer1, mapFrag, "map_fragment")
                                      .Commit();

            mapFrag.GetMapAsync(this);

            Button btn1 = FindViewById<Button>(Resource.Id.button1);
            btn1.Click += Btn1_Click; ;
            Button btn2 = FindViewById<Button>(Resource.Id.button2);
            btn2.Click += Btn2_Click; ;
            Button btnUser = FindViewById<Button>(Resource.Id.btnUser);
            btnUser.Click += BtnUser_Click;
            Button btnLog = FindViewById<Button>(Resource.Id.btnLogout);
            btnLog.Click += BtnLog_Click;
            //api connection

            try
            {
                TextView name = FindViewById<TextView>(Resource.Id.agent_name);
            TextView phoneNumber = FindViewById<TextView>(Resource.Id.agent_phoneNo);
            TextView email = FindViewById<TextView>(Resource.Id.agent_email);
           
                string url = "https://10.0.2.2:5001/api/Agents1/1";
                string result = "";
                var httpWebRequest = new HttpWebRequest(new Uri(url));
                httpWebRequest.ServerCertificateValidationCallback = delegate { return true; };
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "Get";
                HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse();
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                }
                Agent agent = new Agent();
                agent = Newtonsoft.Json.JsonConvert.DeserializeObject<Agent>(result);
                name.Text = agent.name;
                email.Text = agent.email;
                phoneNumber.Text = agent.phoneNumber;
            }

            catch(Exception e)
            {

            }

            //BottomNavigationView
            // BottomNavigationView navigation = FindViewById<BottomNavigationView>(Resource.Id.bottomNavigationView1);
            //navigation.SetOnNavigationItemSelectedListener(this);
        }

        private void BtnLog_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(RegisterActivity));
        }

        private void BtnUser_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(DetailActivity));
        }

        private async void Btn2_Click(object sender, System.EventArgs e)
        {
            TextView name = FindViewById<TextView>(Resource.Id.textView1);
            TextView phone = FindViewById<TextView>(Resource.Id.textView2);
            string text = "Hi, I am" + name.Text + " saw your details on the Rent - a - go app.Could you please send me details of more houses for rent in the same price range ?";

            try
            {
                var message = new SmsMessage(text, phone.Text);
                await Sms.ComposeAsync(message);
            }
            catch (FeatureNotSupportedException ex)
            {
                // Sms is not supported on this device.
            }
            catch (Exception ex)
            {
                // Other error has occurred.
            }
        }

        private async void Btn1_Click(object sender, System.EventArgs e)
        {
            TextView name = FindViewById<TextView>(Resource.Id.textView1);
            TextView phone = FindViewById<TextView>(Resource.Id.textView2);
            TextView email = FindViewById<TextView>(Resource.Id.textView3);
            await Share.RequestAsync(new ShareTextRequest
            {
                Text = "hi,i am an agent of property,i am" + name.Text + "my phone number is" + phone.Text + "my email is" + email.Text,
                Title = "Share Text"
            });
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }


        [System.Obsolete]
        public void OnMapReady(GoogleMap googleMap)
        {
            googleMap.MapType = GoogleMap.MapTypeNormal;
            googleMap.UiSettings.ZoomControlsEnabled = true;
            googleMap.UiSettings.CompassEnabled = true;

            getCurrentLoc(googleMap);
        }

        public async void getLastLocation(GoogleMap googleMap)
        {
            Console.WriteLine("Test - LastLoc");
            try
            {
                var location = await Geolocation.GetLastKnownLocationAsync();
                if (location != null)
                {
                    Console.WriteLine($"Last Loc - Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                    MarkerOptions curLoc = new MarkerOptions();
                    curLoc.SetPosition(new LatLng(location.Latitude, location.Longitude));
                    var address = await Geocoding.GetPlacemarksAsync(location.Latitude, location.Longitude);
                    var placemark = address?.FirstOrDefault();
                    var geocodeAddress = "";
                    if (placemark != null)
                    {
                        geocodeAddress =
                            $"AdminArea:       {placemark.AdminArea}\n" +
                            $"CountryCode:     {placemark.CountryCode}\n" +
                            $"CountryName:     {placemark.CountryName}\n" +
                            $"FeatureName:     {placemark.FeatureName}\n" +
                            $"Locality:        {placemark.Locality}\n" +
                            $"PostalCode:      {placemark.PostalCode}\n" +
                            $"SubAdminArea:    {placemark.SubAdminArea}\n" +
                            $"SubLocality:     {placemark.SubLocality}\n" +
                            $"SubThoroughfare: {placemark.SubThoroughfare}\n" +
                            $"Thoroughfare:    {placemark.Thoroughfare}\n";

                    }
                    curLoc.SetTitle("You were here" + geocodeAddress);
                    curLoc.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueAzure));
                    googleMap.AddMarker(curLoc);
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
                Toast.MakeText(this, "Feature Not Supported", ToastLength.Short);
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
                Toast.MakeText(this, "Feature Not Enabled", ToastLength.Short);
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
                Toast.MakeText(this, "Needs more permission", ToastLength.Short);
            }
            catch (Exception ex)
            {
                // Unable to get location
                Toast.MakeText(this, "Unable to get location", ToastLength.Short);
            }
        }

        public async void getCurrentLoc(GoogleMap googleMap)
        {
            Console.WriteLine("Test - CurrentLoc");
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Best);
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    Console.WriteLine($"current Loc - Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                    MarkerOptions curLoc = new MarkerOptions();
                    curLoc.SetPosition(new LatLng(location.Latitude, location.Longitude));


                    var address = await Geocoding.GetPlacemarksAsync(location.Latitude, location.Longitude);
                    var placemark = address?.FirstOrDefault();
                    var geocodeAddress = "";
                    if (placemark != null)
                    {
                        geocodeAddress =
                            $"AdminArea:       {placemark.AdminArea}\n" +
                            $"CountryCode:     {placemark.CountryCode}\n" +
                            $"CountryName:     {placemark.CountryName}\n" +
                            $"FeatureName:     {placemark.FeatureName}\n" +
                            $"Locality:        {placemark.Locality}\n";

                    }


                    curLoc.SetTitle("house location");// + geocodeAddress);
                    curLoc.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueAzure));

                    googleMap.AddMarker(curLoc);


                    CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
                    builder.Target(new LatLng(location.Latitude, location.Longitude));
                    builder.Zoom(18);
                    builder.Bearing(155);
                    builder.Tilt(65);

                    CameraPosition cameraPosition = builder.Build();

                    CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);

                    googleMap.MoveCamera(cameraUpdate);
                }
                else
                {
                    getLastLocation(googleMap);
                }
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
                Toast.MakeText(this, "Feature Not Supported", ToastLength.Short);
            }
            catch (FeatureNotEnabledException fneEx)
            {
                // Handle not enabled on device exception
                Toast.MakeText(this, "Feature Not Enabled", ToastLength.Short);
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
                Toast.MakeText(this, "Needs more permission", ToastLength.Short);
            }
            catch (Exception ex)
            {
                getLastLocation(googleMap);
            }
        }
        // [System.Obsolete]

        //navigationItemSelected click events
        /* public bool OnNavigationItemSelected(IMenuItem item)
         {
             agentFragment fg1 = new agentFragment();
             FrameLayout fragCon = FindViewById<FrameLayout>(Resource.Id.fragContainer);
             FragmentTransaction transaction;
             switch (item.ItemId)
             {
                 case Resource.Id.navigation_agent:
                     fragCon.RemoveAllViewsInLayout();
                     fragCon.RemoveViews(0, fragCon.ChildCount);
                     transaction = FragmentManager.BeginTransaction();
                     transaction.Replace(Resource.Id.fragContainer, fg1).AddToBackStack(null).Commit();
                     return true;
                 case Resource.Id.navigation_house:
                     transaction = FragmentManager.BeginTransaction();
                     transaction.Replace(Resource.Id.fragContainer, fg1).AddToBackStack(null).Commit();
                     return true;
                 case Resource.Id.navigation_user:
                     transaction = FragmentManager.BeginTransaction();
                     transaction.Replace(Resource.Id.fragContainer, fg1).AddToBackStack(null).Commit();
                     return true;
             }
             return false;


         }*/
    }
}