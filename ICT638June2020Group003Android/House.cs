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

namespace ICT638June2020Group003Android
{
    class House
    {
        public int id { get; set; }
        public string rent { get; set; }

        public string bedroomnumber { get; set; }

        public string bathroomnumber { get; set; }

        public string Address { get; set; }
    }
}