using System;

namespace HelloEventStore.Domain.Services
{
    public interface IUserView
    {
        bool UserExist(string userName);
        Guid GetUserId(string userName);
        void InsertUser(Guid id, string userName);
    }
}