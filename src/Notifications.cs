using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;

namespace WillFootballRuinMyDay
{
    public class Notifications
    {
        private MainActivity _mainActivity;

        public Notifications(MainActivity mainActivity)
        {
            _mainActivity = mainActivity;
        }

        public void DisplayNotificationIfFirstGameIsToday(IList<Fixture> fixtures)
        {
            var now = new DateTime(2014, 9, 14, 15, 00, 00);
            var fixture = fixtures[0];
            var notificationManager = (NotificationManager) _mainActivity.GetSystemService(Context.NOTIFICATION_SERVICE);

            if (fixture.Date.Date == now.Date && now <= fixture.Date)
            {
                var bigText = new Notification.BigTextStyle();
                bigText.BigText(fixture.HomeTeam + " v " + fixture.AwayTeam);

                var footballTodayMessage = "Football today at " + fixture.Date.ToString("HH:mm");

                bigText.SetBigContentTitle(footballTodayMessage);
                
                var notification = new Notification.Builder(_mainActivity)
                    .SetSmallIcon(R.Drawables.Icon)
                    .SetStyle(bigText)
                    .SetTicker(footballTodayMessage)
                    .Build();

                notificationManager.Notify(1, notification);
            }
            else
            {
                notificationManager.CancelAll();
            }
        }
    }
}