using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PageableObservableCollectionExample
{
    public class PageableObservableCollection<T> : ObservableCollection<T>
    {
        /// <summary>
        /// The function to be called when the next page of data is required.
        /// </summary>
        private Func<int, int, Task<List<T>>> _loadMoreFunction;

        /// <summary>
        /// Default constructor which collection types must have in C#
        /// or you can get runtime errors during serialization.
        /// </summary>
        public PageableObservableCollection() : base()
        {
        }

        /// <summary>
        /// Constructor which accepts a function which will be called to get more items.
        /// The function which is passed to the constructor should accept two parameters,
        /// Page and PageSize (in that order).
        /// </summary>
        /// <param name="loadMoreFunction">The function to be called when the next page of data is required</param>
        /// <param name="pageSize">The number of items to request at once</param>
        public PageableObservableCollection(Func<int, int, Task<List<T>>> loadMoreFunction, int pageSize = 25) : base()
        {
            if (loadMoreFunction == null)
                throw new NullReferenceException("loadMoreFunction");

            _loadMoreFunction = loadMoreFunction;
            
            PageSize = pageSize;
            HasMore = true;
        }

        /// <summary>
        /// Checks if the next page should be loaded based off the
        /// position of the itemRealized in the currently loaded list.
        /// </summary>
        /// <param name="itemRealized">The item which has just been loaded by the LongListSelector</param>
        public async void ItemLoaded(T itemRealized)
        {
            if (!IsLoading && Items.Count - Items.IndexOf(itemRealized) <= PageSize)
            {
                await LoadNextPageAsync();
            }
        }

        /// <summary>
        /// Retrieves the first page of data. Any further page will be loaded via ItemLoaded
        /// </summary>
        public async void LoadFirstPageAsync()
        {
            await LoadNextPageAsync();
        }

        /// <summary>
        /// Load the next page of data and add the new items to the existing collection.
        /// </summary>
        /// <returns></returns>
        private async Task LoadNextPageAsync()
        {
            if (!HasMore)
                return;

            IsLoading = true;
            CurrentPage++;

            List<T> newItems = await _loadMoreFunction(CurrentPage, PageSize);

            if (newItems != null && newItems.Any())
            {
                foreach (var newItem in newItems)
                {
                    Add(newItem);
                }

                // If there were less items returned than we requested, then we've
                // reached the last page and there is no point doing any more requests
                // for a higher page number with the current value of PageSize.
                if (newItems.Count < PageSize)
                    HasMore = false;
            }
            else
            {
                HasMore = false;
            }

            IsLoading = false;
        }

        /// <summary>
        /// The number of items to request at one time.
        /// </summary>
        public int PageSize { get; private set; }

        /// <summary>
        /// The last page number which data was requested for.
        /// </summary>
        public int CurrentPage { get; private set; }

        /// <summary>
        /// Tells you whether there is a request in progress.
        /// </summary>
        public bool IsLoading { get; private set; }

        /// <summary>
        /// If the last request did not contain the same number of items as
        /// was requested, then HasMore will be set to false to signify there
        /// is no point doing any more requests beyond the current page using
        /// the current page size.
        /// </summary>
        public bool HasMore { get; private set; }
    }
}
