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

namespace ICT638June2020Group003Android.Activities
{
    [Activity(Label = "UserProfile")]
    public class UserProfile : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Button btlg = FindViewById<Button>(Resource.Id.logout);
            btlg.Click += Btlg_Click;
            Button BtnU = FindViewById<Button>(Resource.Id.btusers);
            BtnU.Click += BtnU_Click;
            Button btagent = FindViewById<Button>(Resource.Id.btagent);
            btagent.Click += Btagent_Click;

            // Create your application here
        }

        private void Btagent_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent();
            intent.SetClass(this, typeof(agent_activity));
            StartActivity(intent);
        }

        private void BtnU_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent();
            intent.SetClass(this, typeof(UserProfile));
            StartActivity(intent);
        }

        private void Btlg_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent();
            intent.SetClass(this, typeof(MainActivity));
            StartActivity(intent);
        }
    }
}