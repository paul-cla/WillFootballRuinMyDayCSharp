using System;
using Android.App;
using Android.Content;
using Java.Util;

namespace WillFootballRuinMyDay
{
    public class FixtureAlarmService
    {
        private readonly Context _context;
        private readonly PendingIntent _mAlarmSender; 
        
        public FixtureAlarmService(Context context)
        {
            _context = context;
            var intent = new Intent(_context, typeof(FixtureTodayAlarmReceiver));
            _mAlarmSender = PendingIntent.GetBroadcast(context, 0, intent, 0);
        }

        public void StartFixtureCheckAlarm()
        {
            var alarmManager = (AlarmManager)_context.GetSystemService(Context.ALARM_SERVICE);

            // Set the alarm to start at approximately 10:00 a.m.
            var calendar = Calendar.GetInstance();
            calendar.SetTimeInMillis(DateTime.Now.CurrentTimeMillis());
            calendar.Set(Calendar.HOUR_OF_DAY, 10);

            alarmManager.SetInexactRepeating(AlarmManager.RTC_WAKEUP, calendar.GetTimeInMillis(), AlarmManager.INTERVAL_DAY, _mAlarmSender);

        }
    }
}