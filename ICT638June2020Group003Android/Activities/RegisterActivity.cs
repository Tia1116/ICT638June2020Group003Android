using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace ICT638June2020Group003Android.Activities
{
    [Activity(Label = "RegisterActivity")]
    public class RegisterActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            SetContentView(Resource.Layout.register_layout);
            base.OnCreate(savedInstanceState);

            Button btn_add = FindViewById<Button>(Resource.Id.btn_register_add);
            btn_add.Click += Btn_add_Click;
            Button btn_back = FindViewById<Button>(Resource.Id.btn_home);
            btn_back.Click += Btn_back_Click;
        }

        private void Btn_back_Click(object sender, EventArgs e)
        {
            StartActivity(typeof(MainActivity));
        }

        public string getQuotedString(string str)
        {
            return "\"" + str + "\"";
        }

        private void Btn_add_Click(object sender, EventArgs e)
        {
            //Push Data to Web Api
            User user = new User();
            user.FirstName = FindViewById<EditText>(Resource.Id.txt_first_name_add).Text;
            user.LastName = FindViewById<EditText>(Resource.Id.txt_last_name_add).Text;
            user.Password = FindViewById<EditText>(Resource.Id.txt_password_add).Text;
            user.EmailAddress = FindViewById<EditText>(Resource.Id.txt_email_address_add).Text;
            user.PhoneNumber = FindViewById<EditText>(Resource.Id.txt_phone_number_add).Text;
            user.Address = FindViewById<EditText>(Resource.Id.txt_address_add).Text;
            user.Country = FindViewById<EditText>(Resource.Id.txt_country_add).Text;

            string url = "https://10.0.2.2:5001/api/Users";
            var httpWebRequest = new HttpWebRequest(new Uri(url));
            //var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ServerCertificateValidationCallback = delegate { return true; };
            //httpWebRequest.ServerCertificateCustomValidationCallback = delegate { return true; }
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = "{" +
                    getQuotedString("FirstName") + ":" + getQuotedString(user.FirstName) + "," +
                    getQuotedString("LastName") + ":" + getQuotedString(user.LastName) + "," +
                    getQuotedString("Password") + ":" + getQuotedString(user.Password) + "," +
                    getQuotedString("EmailAddress") + ":" + getQuotedString(user.EmailAddress) + "," +
                    getQuotedString("PhoneNumber") + ":" + getQuotedString(user.PhoneNumber) + "," +
                    getQuotedString("Address") + ":" + getQuotedString(user.Address) + "," +
                    getQuotedString("Country") + ":" + getQuotedString(user.Country) +
                               "}";

                streamWriter.Write(json);
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
            }
        }
    }
}