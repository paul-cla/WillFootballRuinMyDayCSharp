using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Android.App;
using Android.Content;
using Android.Net;
using Android.Net.Http;
using Android.Os;
using Android.Util;
using Android.View;
using Android.Widget;
using Dot42.Manifest;
using Exception = System.Exception;

[assembly: Application("Will Football Ruin My Day?", Icon = "Icon")]
[assembly: UsesPermission(Android.Manifest.Permission.INTERNET)]
[assembly: UsesPermission(Android.Manifest.Permission.ACCESS_NETWORK_STATE)]

namespace WillFootballRuinMyDay
{
    [Activity]
    public class MainActivity : Activity
    {
        private const string Tag = "WillFootballRuinMyDay";
        private ListView _fixtureList;
        private readonly List<Fixture> _fixtures = new List<Fixture> { new Fixture(), new Fixture(), new Fixture(), new Fixture(), new Fixture() };
        private ArrayAdapter<Fixture> _adapter;
        private Fixture[] _fixturesArray = new Fixture[5];

        private const string TeamName = "Manchester United FC";
        private const int TeamId = 66;

        /// <summary>
        /// Inflate the menu resource into the given menu.
        /// </summary>
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(R.Menus.Menu, menu);
            return true;
        }

        /// <summary>
        /// Menu option has been clicked.
        /// </summary>
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.GetItemId())
            {
                case R.Ids.Refresh:
                    GetFixturesAsync(true);
                    break;

            }
            return base.OnOptionsItemSelected(item);
        }

        /// <summary>
        /// Initialize activity
        /// </summary>
        protected override void OnCreate(Bundle savedInstance)
        {
            base.OnCreate(savedInstance);

            try
            {
                var httpCacheDir = new Java.Io.File(GetCacheDir(), "http");
                const long httpCacheSize = 10 * 1024 * 1024; // 10 MB
                HttpResponseCache.Install(httpCacheDir, httpCacheSize);
            }
            catch (IOException ex)
            {
                Log.E(Tag, "HTTP response cache installation failed. " + ex.Message + " " + ex.StackTrace);
            }

            SetContentView(R.Layouts.MainLayout);
            _fixtureList = FindViewById<ListView>(R.Ids.fixtureList);

            _fixturesArray = _fixtures.ToArray();
            _adapter = new ArrayAdapter<Fixture>(this, Android.R.Layout.Simple_list_item_1, _fixturesArray);
            _fixtureList.SetAdapter(_adapter);

            GetFixturesAsync();


            var cache = HttpResponseCache.GetInstalled();
            Log.I(Tag, "Hit" + cache.GetHitCount().ToString());
            Log.I(Tag, "Network" + cache.GetNetworkCount().ToString());
            Log.I(Tag, "Request" + cache.GetRequestCount().ToString());

        }

        private void GetFixturesAsync(bool forceRefresh = false)
        {
            if (!IsNetworkConnected())
            {
                WirelessConnectivity.ShowNoConnectionToast(this);
            }
            var worker = new BackgroundWorker();
            worker.DoWork += (o, args) => OnGetFixtures(o, args, forceRefresh);
            worker.RunWorkerAsync();
        }


        public bool IsNetworkConnected()
        {
            var cm = (ConnectivityManager)GetSystemService(CONNECTIVITY_SERVICE);
            NetworkInfo ni;
            try
            {
                ni = cm.GetActiveNetworkInfo();
            }
            catch (Exception ex)
            {
                Log.E(Tag, "No wireless connectivity. " + ex);
                ni = null;
            }

            return ni != null;
        }

        private void OnGetFixtures(object sender, DoWorkEventArgs doWorkEventArgs, bool forceRefresh = false)
        {
            var updater = new ListViewUpdater(_fixtureList);
            var fixtures = FootballService.GetFixtures(TeamId, forceRefresh);


            if (fixtures != null)
            {
                fixtures = LimitToHomeTeam(fixtures);
                DisplayNotificationIfFirstGameIsToday(fixtures);
            }
            updater.Post();
        }

        private void DisplayNotificationIfFirstGameIsToday(IList<Fixture> fixtures)
        {
            var today = new DateTime(2014, 9, 14);
            var fixture = fixtures[0];
            var notificationManager = (NotificationManager)GetSystemService(NOTIFICATION_SERVICE);

            if (fixture.Date.Date == today.Date)
            {
                var bigText = new Notification.BigTextStyle();
                bigText.BigText(fixture.HomeTeam + " v " + fixture.AwayTeam);
                bigText.SetBigContentTitle("Football today at " + fixture.Date.ToString("HH:mm"));
                
                var notification = new Notification.Builder(this)
                    .SetSmallIcon(R.Drawables.Icon)
                    .SetStyle(bigText)
                    .Build();

                notificationManager.Notify(1, notification);
            }
            else
            {
                notificationManager.CancelAll();
            }
        }

        private List<Fixture> LimitToHomeTeam(IEnumerable<Fixture> fixtures)
        {
            var newFixtures = new List<Fixture>();
            var i = 0;
            foreach (var fixture in fixtures)
            {
                if (fixture.HomeTeam == TeamName)
                {
                    _fixturesArray[i] = fixture;
                    newFixtures.Add(fixture);
                    i++;
                }
            }
            return newFixtures;
        }

        protected new void OnPause()
        {
            var cache = HttpResponseCache.GetInstalled();

            if (cache != null)
            {
                cache.Flush();
            }
        }
    }
}
