using System;
using Android.App;
using Android.Content;
using Android.Util;
using Dot42.Manifest;

namespace WillFootballRuinMyDay
{
    [Receiver(Process = ":remote")]
    public class FixtureTodayAlarmReceiver : BroadcastReceiver 
    {

        public override void OnReceive(Context context, Intent intent)
        {
            var notificationManager = (NotificationManager)context.GetSystemService(Context.NOTIFICATION_SERVICE);
            // Set the icon, scrolling text and timestamp
            var notification = new Notification(R.Drawables.Icon, "Test Alarm", Environment.TickCount);

            // The PendingIntent to launch our activity if the user selects this notification
            var contentIntent = PendingIntent.GetActivity(context, 0, new Intent(context, typeof (MainActivity)), 0);
            
            // Set the info for the views that show in the notification panel.
            notification.SetLatestEventInfo(context, "something", "This is a Test Alarm", contentIntent);
            
            // Send the notification.
            // We use a layout id because it is a unique number. We use it later to cancel.
            notificationManager.Notify(1, notification);
        }
    }
}