# load-sflistview-and-search-bar-in-.net-maui-pull-to-refresh

This sample demonstrates how to load and wire up the Syncfusion SfListView together with a SearchBar inside the Syncfusion SfPullToRefresh control in a .NET MAUI application.

## Overview

This repository contains a small MAUI sample that shows a common pattern in mobile apps:

- A SearchBar placed at the top of the page to filter the list.
- A pull-to-refresh container (`SfPullToRefresh`) that hosts the list content and enables pull-to-refresh UX.
- A Syncfusion `SfListView` configured with item templates, group headers, and swipe templates.

The sample uses a ViewModel-backed ItemsSource (`InboxInfos`) bound to the `SfListView`. The SearchBar updates a filter function on the `ListView.DataSource` so the view updates immediately as the user types.

For the official documentation and additional details, please refer:
- [Getting Started with .NET MAUI PullToRefresh](https://help.syncfusion.com/maui/pull-to-refresh/getting-started)
- [Getting Started with .NET MAUI ListView](https://help.syncfusion.com/maui/listview/getting-started)


## How it works (short)

- The `SearchBar` raises `TextChanged` events which are handled in the page's code-behind. The handler stores a reference to the `SearchBar` and sets a filter method on the list's `DataSource`.
- The filter (`FilterContacts`) checks the typed text against multiple fields on the model (Name, Subject, Description) and returns true for items that should remain visible.
- `SfPullToRefresh` wraps the list and provides pull-to-refresh capability; the sample also demonstrates using a small custom behavior to wire refresh events (see `Helper/ListViewPullToRefreshBehavior.cs`).

## XAML 

Below is a compacted excerpt from `MainPage.xaml` showing the SearchBar, the `SfPullToRefresh` wrapper and the `SfListView` configuration with templates.

```
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
						 xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
						 xmlns:ListView="clr-namespace:Syncfusion.Maui.ListView;assembly=Syncfusion.Maui.ListView"
						 xmlns:pulltoRefresh="clr-namespace:Syncfusion.Maui.PullToRefresh;assembly=Syncfusion.Maui.PullToRefresh"
						 x:Class="RefreshListView.MainPage">
	<SearchBar Placeholder="Search Here" TextChanged="SearchBar_TextChanged" />
	<pulltoRefresh:SfPullToRefresh x:Name="pullToRefresh">
		<pulltoRefresh:SfPullToRefresh.PullableContent>
			<Grid>
				<Label Text="Inbox" />
				<ListView:SfListView x:Name="listView" ItemsSource="{Binding InboxInfos}" AutoFitMode="Height">
					<!-- ItemTemplate, GroupHeaderTemplate and Swipe templates omitted for brevity -->
				</ListView:SfListView>
			</Grid>
		</pulltoRefresh:SfPullToRefresh.PullableContent>
	</pulltoRefresh:SfPullToRefresh>
</ContentPage>
```

## C# 

The code-behind wires the SearchBar to the list filtering logic. The two important pieces are `SearchBar_TextChanged` and `FilterContacts`.

```
using RefreshListView.Model;
using Syncfusion.Maui.DataSource;

public partial class MainPage : ContentPage
{
		SearchBar? searchBar = null;

		public MainPage()
		{
				InitializeComponent();
		}

		private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
		{
				searchBar = sender as SearchBar;
				if (listView.DataSource != null)
				{
						listView.DataSource.Filter = FilterContacts;
						listView.DataSource.RefreshFilter();
				}
		}

		private bool FilterContacts(object obj)
		{
				if (searchBar == null || searchBar.Text == null)
						return true;
				var inboxInfos = obj as InboxInfo;
				return inboxInfos.Name.ToLower().Contains(searchBar.Text.ToLower())
							 || inboxInfos.Subject.ToLower().Contains(searchBar.Text.ToLower())
							 || inboxInfos.Description.ToLower().Contains(searchBar.Text.ToLower());
		}
}
```

##### Conclusion

I hope you enjoyed learning about how to load the .NET MAUI SfListView and SearchBar in .NET MAUI Pull To Refresh (SfPullToRefresh).