using Android.Os;
using Android.Preference;
using Dot42.Manifest;

namespace WillFootballRuinMyDay
{
    [Activity]
    public class SettingsActivity : PreferenceActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            AddPreferencesFromResource(R.Xmls.Preferences);
        }
    }
}
