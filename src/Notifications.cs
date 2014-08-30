using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;

namespace WillFootballRuinMyDay
{
    public class Notifications
    {
        private readonly Context _context;

        public Notifications(Context context)
        {
            _context = context;
        }

        public void DisplayNotificationIfNextGameIsToday(IList<Fixture> fixtures)
        {
            //var now = new DateTime(2014, 9, 14, 15, 00, 00);
            var now = DateTime.Now;
            var fixture = fixtures[0];
            var notificationManager = (NotificationManager)_context.GetSystemService(Context.NOTIFICATION_SERVICE);

            if (fixture.Date.Date == now.Date && now <= fixture.Date)
            {
                var bigText = new Notification.BigTextStyle();
                bigText.BigText(fixture.HomeTeam + " v " + fixture.AwayTeam);

                var footballTodayMessage = "Football today at " + fixture.Date.ToString("HH:mm");

                bigText.SetBigContentTitle(footballTodayMessage);
                
                var notification = new Notification.Builder(_context)
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