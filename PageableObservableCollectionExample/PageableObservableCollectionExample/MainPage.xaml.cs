using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PageableObservableCollectionExample.Resources;

namespace PageableObservableCollectionExample
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();

            Friends = new PageableObservableCollection<Person>((int page, int pageSize) =>
            {
                // Here, provide the function which should be used to retrieve each page.
                return DataStore.GetFriends(page, pageSize);
            });

            Friends.LoadFirstPageAsync();
        }
        
        private PageableObservableCollection<Person> _friends;
        public PageableObservableCollection<Person> Friends
        {
            get
            {
                return _friends;
            }
            set
            {
                if (value != _friends)
                {
                    _friends = value;
                    NotifyPropertyChanged("Friends");
                }
            }
        }

        private void FriendsLongListSelector_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            // ItemRealized means the item in the event args has just been rendered. Items are preloaded by the framework
            // before you've scrolled down to them. Here we are telling the PageableObservableCollection which item has
            // been loaded, so it can check and load the next page if we're getting close to the end of list.
            Person friend = e.Container.Content as Person;
            if (friend != null)
            {
                Friends.ItemLoaded(friend);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}