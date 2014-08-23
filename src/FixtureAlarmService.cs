using Android.App;
using Android.Content;
using Android.Os;
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
            
            var calendar = Calendar.GetInstance();

            calendar.Add(Calendar.SECOND, 10);

            var alarmManager = (AlarmManager)_context.GetSystemService(Context.ALARM_SERVICE);

            alarmManager.SetInexactRepeating(AlarmManager.ELAPSED_REALTIME, 10000, 10000, _mAlarmSender);

        }
    }
}