PageableObservableCollection
============================
A component which extends ObservableCollection to enable continuous/infinite scrolling for Windows Phone. This is an example project which uses the component. You can simply copy/paste it into your project.
 
 
Usage
============================
Create an instance of the PageableObserveableCollection and provide it with a function which you would like to execute to retrieve your items (web service call or whatever). The Func must accept two int parameters for page and page size. These will be provided to the fuction when the next page of data is required.

When a page is retrieved, the new items will be automatically added to the list collection, so your LongListSelector will automatically update.

```c#
Friends = new PageableObservableCollection<Person>((int page, int pageSize) =>
{
    // Here, provide the async function which should be used to retrieve each page.
    return DataStore.GetFriends(page, pageSize);
});
```

The page size is an optional third parameter and defaults to 25.

Begin the loading like so:

```c#
Friends.LoadFirstPageAsync();
```

You will then need to hook up the ItemRealized event from the LongListSelector to an event in the code behind. Pass this item to the ItemLoaded method of the PageableObserveableCollection and it will decide if it needs to load another page.

```c#
private void FriendsLongListSelector_ItemRealized(object sender, ItemRealizationEventArgs e)
{
    Person friend = e.Container.Content as Person;
    if (friend != null)
    {
        Friends.ItemLoaded(friend);
    }
}
```

In the XAML you will have to hook up the ItemRealized event:

```xaml
<phone:LongListSelector ItemsSource="{Binding Friends}" x:Name="FriendsLongListSelector" ItemRealized="FriendsLongListSelector_ItemRealized">
```

And that's it! As you scroll down, more and more chunks of data will be loaded and you can keep on scrollin'! 
