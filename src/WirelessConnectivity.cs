using Android.Content;
using Android.Widget;

namespace WillFootballRuinMyDay
{
    public class WirelessConnectivity
    {
        public static void ShowNoConnectionToast(Context ctx1)
        {
            var ctx = ctx1;
            var toast = Toast.MakeText(ctx, "No Internet Connection", Toast.LENGTH_LONG);
            toast.Show();
        }
    }
}