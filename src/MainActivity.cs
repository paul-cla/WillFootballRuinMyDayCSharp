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
using Java.Lang;
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

        private static bool _shownConnectionBox;


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
                Log.E(Tag, "HTTP response cache installation failed:" + ex);
            }

           // if (IsNetworkConnected())
           // {
                SetContentView(R.Layouts.MainLayout);
                _fixtureList = FindViewById<ListView>(R.Ids.fixtureList);

                _fixturesArray = _fixtures.ToArray();
                _adapter = new ArrayAdapter<Fixture>(this, Android.R.Layout.Simple_list_item_1, _fixturesArray);
                _fixtureList.SetAdapter(_adapter);

                var worker = new BackgroundWorker();
                worker.DoWork += OnGetFixtures;
                worker.RunWorkerAsync();
           // }
            //else
            //{
            //    ShowNoConnectionDialog(this);
            //}
        }

        private static void ShowNoConnectionDialog(Context ctx1)
        {
            if (!_shownConnectionBox)
            {
                var ctx = ctx1;
                var builder = new AlertDialog.Builder(ctx);
                builder.SetCancelable(true);
                builder.SetMessage("No connection");
                builder.SetTitle("No connection");
                builder.SetPositiveButton("OK",
                                          (sender, dialogInterfaceClickEventArgs) => DoNoConnectionPositiveClick(ctx));
                builder.SetNegativeButton("Cancel",
                                          (sender, dialogInterfaceClickEventArgs) => DoNoConnectionNegativeClick());
                builder.Show();
            }
        }


        private static void DoNoConnectionPositiveClick(Context ctx)
        {
            ctx.StartActivity(new Intent(Android.Provider.Settings.ACTION_WIRELESS_SETTINGS));
        }

        private static void DoNoConnectionNegativeClick()
        {
            _shownConnectionBox = true;
        }

        private bool IsNetworkConnected()
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

        private void OnGetFixtures(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            var updater = new ListViewUpdater(_fixtureList);
            var fixtures = FootballService.GetFixtures(TeamId);

            var i = 0;
            foreach (var fixture in fixtures)
            {
                if (fixture.HomeTeam == TeamName)
                {
                    _fixturesArray[i] = fixture;
                    i++;
                }
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
