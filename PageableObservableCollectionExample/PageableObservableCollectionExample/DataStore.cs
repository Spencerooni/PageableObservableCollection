
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PageableObservableCollectionExample
{
    public static class DataStore
    {
        static List<Person> _allFriends;

        static DataStore()
        {
            _allFriends = new List<Person>();
            
            // Create dummy data.
            foreach (var number in Enumerable.Range(1, 500))
            {
                _allFriends.Add(
                    new Person
                    {
                        Name = String.Format("Friend {0}", number)
                    });
            }
        }

        public static async Task<List<Person>> GetFriends(int page, int pageSize)
        {
            // Here is where you would normally do:
            //      await someApi.someRequest(x, y, z, etc);
            return _allFriends.Skip((page - 1) * pageSize).Take(pageSize).ToList();
        }
    }
}
