using Android.Widget;
using Java.Lang;

namespace WillFootballRuinMyDay
{
    /// <summary>
    /// Helper used to update the listview.
    /// </summary>
    internal class ListViewUpdater : IRunnable
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