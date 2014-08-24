using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Android.App;
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
        private readonly Notifications _notifications;
        private readonly FixtureHelpers _fixtureHelpers;

        public MainActivity()
        {
            _notifications = new Notifications(this);
            _fixtureHelpers = new FixtureHelpers();
        }

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

        // ReSharper disable UnusedParameter.Local
        private void OnGetFixtures(object sender, DoWorkEventArgs doWorkEventArgs, bool forceRefresh = false)
        // ReSharper restore UnusedParameter.Local
        {
            var updater = new ListViewUpdater(_fixtureList);
            var fixtures = FootballService.GetFixtures(TeamId, forceRefresh);


            if (fixtures != null)
            {
                fixtures = _fixtureHelpers.LimitToHomeTeam(fixtures, TeamName);
                for (var i = 0; i < fixtures.Count; i++)
                {
                    _fixturesArray[i] = fixtures[i];
                }
                _notifications.DisplayNotificationIfNextGameIsToday(fixtures);
                var alarm = new FixtureAlarmService(this);
                alarm.StartFixtureCheckAlarm();
            }
            updater.Post();
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
