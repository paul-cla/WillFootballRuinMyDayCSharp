using System;
using System.Collections.Generic;
using System.ComponentModel;
using Android.App;
using Android.Content;
using Android.Os;
using Android.Util;
using Android.Widget;
using Dot42.Manifest;
using Java.Lang;
using Exception = System.Exception;

[assembly: Application("Will Football Ruin My Day?", Icon = "Icon")]
[assembly: UsesPermission(Android.Manifest.Permission.INTERNET)]

namespace WillFootballRuinMyDay
{
    [Activity]
    public class MainActivity : Activity
    {
        private ListView _fixtureList;
        private readonly List<Fixture> _fixtures = new List<Fixture> { new Fixture(), new Fixture(), new Fixture(), new Fixture(), new Fixture() };
        private ArrayAdapter<Fixture> _adapter;
        private Fixture[] _fixturesArray = new Fixture[5];

        /// <summary>
        /// Initialize activity
        /// </summary>
        protected override void OnCreate(Bundle savedInstance)
        {
            base.OnCreate(savedInstance);
            SetContentView(R.Layouts.MainLayout);

            _fixtureList = FindViewById<ListView>(R.Ids.fixtureList);

            _fixturesArray = _fixtures.ToArray();
            _adapter = new ArrayAdapter<Fixture>(this, Android.R.Layout.Simple_list_item_1, _fixturesArray);
            _fixtureList.SetAdapter(_adapter);
            _fixtureList.ItemClick += OnAirportClick;

            var worker = new BackgroundWorker();
            worker.DoWork += OnGetFixtures;
            worker.RunWorkerAsync();
        }

        private void OnGetFixtures(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            var updater = new ListViewUpdater(_fixtureList);
            var x = FootballService.GetFixtures(66);

            var i = 0;
            foreach (var fixture in x)
            {
                _fixturesArray[i] = fixture;
                i++;
            }
            updater.Post();
        }

        /// <summary>
        /// Show status of an airport.
        /// </summary>
        private void OnAirportClick(object sender, ItemClickEventArgs e)
        {
            var intent = new Intent(this, typeof(StatusActivity));
            //intent.PutExtra("code", airports[e.Position].Code);
            StartActivity(intent);
        }

        /// <summary>
        /// Helper used to update the listview.
        /// </summary>
        private class ListViewUpdater : IRunnable
        {
            private readonly ListView listView;

            public ListViewUpdater(ListView listView)
            {
                this.listView = listView;
            }

            /// <summary>
            /// Post an update request
            /// </summary>
            public void Post()
            {
                listView.Post(this);
            }

            /// <summary>
            /// Update now
            /// </summary>
            public void Run()
            {
                listView.InvalidateViews();
            }
        }
    }
}
