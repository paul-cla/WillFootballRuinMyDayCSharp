using System;
using System.ComponentModel;
using Android.App;
using Android.Content;
using Android.Util;
using Dot42.Manifest;

namespace WillFootballRuinMyDay
{
    [Receiver(Process = ":remote")]
    public class FixtureTodayAlarmReceiver : BroadcastReceiver
    {
        private const int TeamId = 66;

        public override void OnReceive(Context context, Intent intent)
        {
            var worker = new BackgroundWorker();
            worker.DoWork += (o, args) => OnGetFixtures(o, args, context);
            worker.RunWorkerAsync();
        }

        // ReSharper disable UnusedParameter.Local
        private static void OnGetFixtures(object sender, DoWorkEventArgs doWorkEventArgs, Context context)
        // ReSharper restore UnusedParameter.Local
        {
            var fixtures = FootballService.GetFixtures(TeamId);

            if (fixtures == null) return;

            var notifications = new Notifications(context);
            notifications.DisplayNotificationIfNextGameIsToday(fixtures);
        }
    }
}