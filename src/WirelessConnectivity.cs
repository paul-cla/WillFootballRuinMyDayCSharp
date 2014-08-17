using Android.App;
using Android.Content;
using Android.Provider;
using Android.Widget;

namespace WillFootballRuinMyDay
{
    public class WirelessConnectivity
    {
        public static void ShowNoConnectionDialog(Context ctx1)
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

        public static void ShowNoConnectionToast(Context ctx1)
        {
            if (!_shownConnectionBox)
            {
                var ctx = ctx1;
                var toast = Toast.MakeText(ctx, "No Internet Connection", Toast.LENGTH_LONG);
                toast.Show();
            }
        }

        private static bool _shownConnectionBox;

        private static void DoNoConnectionPositiveClick(Context ctx)
        {
            ctx.StartActivity(new Intent(Settings.ACTION_WIRELESS_SETTINGS));
        }

        private static void DoNoConnectionNegativeClick()
        {
            _shownConnectionBox = true;
        }
    }
}