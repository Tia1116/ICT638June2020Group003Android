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
    [Activity(Label = "DetailActivity")]
    public class DetailActivity : Activity, IOnMapReadyCallback
    {
        private EditText fname, lname, Phnum, Adress, Ctr, Mail;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetContentView(Resource.Layout.update_layout);
            base.OnCreate(savedInstanceState);

            fname = FindViewById<EditText>(Resource.Id.txt_first_name_up);
            lname = FindViewById<EditText>(Resource.Id.txt_last_name_up);
            Phnum = FindViewById<EditText>(Resource.Id.txt_phone_number_up);
            Adress = FindViewById<EditText>(Resource.Id.txt_address_up);
            Ctr = FindViewById<EditText>(Resource.Id.txt_country_up);
            Mail = FindViewById<EditText>(Resource.Id.txt_email_address_up);

            //Get Api
            string url = "https://10.0.2.2:5001/api/Users/1";
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

            User user = new User();
            user = Newtonsoft.Json.JsonConvert.DeserializeObject<User>(result);

            fname.Text = user.FirstName;
            lname.Text = user.LastName;
            Phnum.Text = user.PhoneNumber;
            Adress.Text = user.Address;
            Ctr.Text = user.Country;
            Mail.Text = user.EmailAddress;

            Button btn_Share = FindViewById<Button>(Resource.Id.btn_share);
            btn_Share.Click += Btn_Share_Click;

            Button btn_Send = FindViewById<Button>(Resource.Id.btn_send);
            btn_Send.Click += Btn_Send_Click;

            //Map
            var mapFrag = MapFragment.NewInstance();// mapOptions);

            FragmentManager.BeginTransaction()
                                    .Add(Resource.Id.map_Fragtag, mapFrag, "map_fragment")
                                   .Commit();

            mapFrag.GetMapAsync(this);
        }

        //Send Message
        private async void Btn_Send_Click(object sender, EventArgs e)
        {
            string messageText = "";
            string recipient = Phnum.Text;
            try
            {
                var message = new SmsMessage(messageText, new[] { recipient });
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

        //Share with Bluetooth
        private async void Btn_Share_Click(object sender, EventArgs e)
        {
            string text = lname.Text + "\r\n" +
                Mail.Text + Ctr.Text;
            await Share.RequestAsync(new ShareTextRequest
            {
                Text = text,
                Title = "Share Text"
            });
        }
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
                            $"Locality:        {placemark.Locality}\n" +
                            $"PostalCode:      {placemark.PostalCode}\n" +
                            $"SubAdminArea:    {placemark.SubAdminArea}\n" +
                            $"SubLocality:     {placemark.SubLocality}\n" +
                            $"SubThoroughfare: {placemark.SubThoroughfare}\n" +
                            $"Thoroughfare:    {placemark.Thoroughfare}\n";

                    }


                    curLoc.SetTitle("You are here");// + geocodeAddress);
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

    }
}