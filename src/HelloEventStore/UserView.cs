using System;
using System.Collections.Generic;
using HelloEventStore.Domain.Services;

namespace HelloEventStore
{
    public class UserView : IUserView
    {
        private Dictionary<string, Guid> _userAccounts = new Dictionary<string, Guid>();
        private static UserView _instance;

        public static IUserView Instance
        {
            get
            {
                _instance = _instance ?? new UserView();
                return _instance;
            }
        }

        public bool UserExist(string userName)
        {
            return _userAccounts.ContainsKey(userName);
        }

        public void InsertUser(Guid id, string userName)
        {
            _userAccounts.Add(userName, id);
        }

        public Guid GetUserId(string userName)
        {
            return _userAccounts[userName];
        }
    }
}