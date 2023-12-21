
using RefreshListView.Model;
using Syncfusion.Maui.DataSource;
using System.Collections.Specialized;

namespace RefreshListView
{
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
            if (inboxInfos.Name.ToLower().Contains(searchBar.Text.ToLower()) || inboxInfos.Subject.ToLower().Contains(searchBar.Text.ToLower()) || inboxInfos.Description.ToLower().Contains(searchBar.Text.ToLower()))
            {
                return true;                
            }
            else
            {
                return false;
            }
        }
    }

}
