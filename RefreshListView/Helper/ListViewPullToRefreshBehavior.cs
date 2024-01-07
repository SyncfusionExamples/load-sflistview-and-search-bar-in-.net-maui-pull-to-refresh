using Syncfusion.Maui.DataSource.Extensions;
using Syncfusion.Maui.DataSource;
using Syncfusion.Maui.ListView;
using Syncfusion.Maui.PullToRefresh;
using RefreshListView.Model;
using RefreshListView.ViewModel;
using Microsoft.Maui.Platform;

namespace RefreshListView.Helper
{
    /// <summary>
    /// Base generic class for user-defined behaviors that can respond to conditions and events.
    /// </summary>
    public class ListViewPullToRefreshBehavior : Behavior<ContentPage>
    {
        private SfPullToRefresh? pullToRefresh;
        private SfListView? ListView;
        private ListViewInboxInfoViewModel? ViewModel;

        /// <summary>
        /// You can override this method to subscribe to AssociatedObject events and initialize properties.
        /// </summary>
        /// <param name="bindable">ContentPage type parameter named as bindable.</param>
        protected override void OnAttachedTo(ContentPage bindable)
        {
            ViewModel = new ListViewInboxInfoViewModel();
            bindable.BindingContext = ViewModel;
            pullToRefresh = bindable.FindByName<SfPullToRefresh>("pullToRefresh");
            ListView = bindable.FindByName<SfListView>("listView");
            pullToRefresh.Refreshing += PullToRefresh_Refreshing;
            pullToRefresh.Refreshed += PullToRefresh_Refreshed;
            ListView.ItemTapped += ListView_ItemTapped;
            ListView.QueryItemSize += ListView_QueryItemSize;
            ListView!.DataSource!.SortDescriptors.Add(new SortDescriptor()
            {
                PropertyName = "Date",
                Direction = Syncfusion.Maui.DataSource.ListSortDirection.Descending,
            });
            ListView.DataSource.GroupDescriptors.Add(new GroupDescriptor()
            {
                PropertyName = "Date",
                KeySelector = (obj) =>
                {
                    var groupName = ((InboxInfo)obj).Date;
                    return GetKey(groupName);
                },
                Comparer = new CustomGroupComparer(),
            });
            ListView.DataSource.LiveDataUpdateMode = LiveDataUpdateMode.AllowDataShaping;

            base.OnAttachedTo(bindable);
        }

        private void PullToRefresh_Refreshed(object? sender, EventArgs e)
        {
#if ANDROID
        (this.ListView!.Handler!.PlatformView as Android.Views.View)!.InvalidateMeasure(this.ListView);
#endif
        }

        /// <summary>
        /// Fired when pullToRefresh View is refreshing
        /// </summary>
        /// <param name="sender">PullToRefresh_Refreshing event sender</param>
        /// <param name="e">PullToRefresh_Refreshing event args</param>
        private async void PullToRefresh_Refreshing(object? sender, EventArgs e)
        {
            pullToRefresh!.IsRefreshing = true;
            await Task.Delay(2500);
            ViewModel!.AddItemsRefresh(3);
            pullToRefresh.IsRefreshing = false;
        }

        /// <summary>
        /// Fired when taps the listview item.
        /// </summary>
        /// <param name="sender">ItemTapped sender.</param>
        /// <param name="e">Represents the event args.</param>
        private void ListView_ItemTapped(object? sender, Syncfusion.Maui.ListView.ItemTappedEventArgs e)
        {
            (e.DataItem as InboxInfo)!.IsOpened = true;
        }

        /// <summary>
        /// Fires whenever an item comes to view.
        /// </summary>
        /// <param name="sender">ListView_QueryItemSize event sender.</param>
        /// <param name="e">Represent ListView_QueryItemSize event args.</param>
        private void ListView_QueryItemSize(object? sender, QueryItemSizeEventArgs e)
        {
            if (e.ItemType == ItemType.GroupHeader && e.ItemIndex == 0)
            {
                var groupName = e.DataItem as GroupResult;

                if (groupName != null && (GroupName)groupName.Key == GroupName.Today)
                {
                    e.ItemSize = 0;
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// You can override this method while View was detached from window
        /// </summary>
        /// <param name="bindable">ContentPage type parameter named as bindable</param>
        protected override void OnDetachingFrom(ContentPage bindable)
        {
            ListView!.ItemTapped -= ListView_ItemTapped;
            ListView.QueryItemSize -= ListView_QueryItemSize;
            pullToRefresh!.Refreshing -= PullToRefresh_Refreshing;
            pullToRefresh = null;
            ListView = null;
            ViewModel = null;
            base.OnDetachingFrom(bindable);
        }

        /// <summary>
        /// Helper method to get the key value for the GroupHeader name based on Data.
        /// </summary>
        /// <param name="groupName">Date of an item.</param>
        /// <returns>Returns specific group name.</returns>
        private GroupName GetKey(DateTime groupName)
        {
            int compare = groupName.Date.CompareTo(DateTime.Now.Date);

            if (compare == 0)
            {
                return GroupName.Today;
            }
            else if (groupName.Date.CompareTo(DateTime.Now.AddDays(-1).Date) == 0)
            {
                return GroupName.Yesterday;
            }
            else if (IsLastWeek(groupName))
            {
                return GroupName.LastWeek;
            }
            else if (IsThisWeek(groupName))
            {
                return GroupName.ThisWeek;
            }
            else if (IsThisMonth(groupName))
            {
                return GroupName.ThisMonth;
            }
            else if (IsLastMonth(groupName))
            {
                return GroupName.LastMonth;
            }
            else
            {
                return GroupName.Older;
            }
        }

        /// <summary>
        /// Helper method to check whether particular date is in this week or not.
        /// </summary>
        /// <param name="groupName">Date of an item.</param>
        /// <returns>Returns true if the mentioned date is in this week.</returns>
        private bool IsThisWeek(DateTime groupName)
        {
            var groupWeekSunDay = groupName.AddDays(-(int)groupName.DayOfWeek).Day;
            var currentSunday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek).Day;

            var groupMonth = groupName.Month;
            var currentMonth = DateTime.Today.Month;

            var isCurrentYear = groupName.Year == DateTime.Today.Year;

            return currentSunday == groupWeekSunDay && (groupMonth == currentMonth || groupMonth == currentMonth - 1) && isCurrentYear;
        }

        /// <summary>
        /// Helper method to check whether particular date is in last week or not.
        /// </summary>
        /// <param name="groupName">Date of an item.</param>
        /// <returns>Returns true if the mentioned date is in last week.</returns>
        private bool IsLastWeek(DateTime groupName)
        {
            var groupWeekSunDay = groupName.AddDays(-(int)groupName.DayOfWeek).Day;
            var lastSunday = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek).Day - 7;

            var groupMonth = groupName.Month;
            var currentMonth = DateTime.Today.Month;

            var isCurrentYear = groupName.Year == DateTime.Today.Year;

            return lastSunday == groupWeekSunDay && (groupMonth == currentMonth || groupMonth == currentMonth - 1) && isCurrentYear;
        }

        /// <summary>
        /// Helper method to check whether particular date is in this month or not.
        /// </summary>
        /// <param name="groupName">Date of an item.</param>
        /// <returns>Returns true if the mentioned date is in this month.</returns>
        private bool IsThisMonth(DateTime groupName)
        {
            var groupMonth = groupName.Month;
            var currentMonth = DateTime.Today.Month;

            var isCurrentYear = groupName.Year == DateTime.Today.Year;

            return groupMonth == currentMonth && isCurrentYear;
        }

        /// <summary>
        /// Helper method to check whether particular date is in last month or not.
        /// </summary>
        /// <param name="groupName">Date of an item.</param>
        /// <returns>Returns true if the mentioned date is in last month.</returns>
        private bool IsLastMonth(DateTime groupName)
        {
            var groupMonth = groupName.Month;
            var currentMonth = DateTime.Today.AddMonths(-1).Month;

            var isCurrentYear = groupName.Year == DateTime.Today.Year;

            return groupMonth == currentMonth && isCurrentYear;
        }
    }

    public enum GroupName
    {
        Today = 0,
        Yesterday,
        ThisWeek,
        LastWeek,
        ThisMonth,
        LastMonth,
        Older
    }
}
