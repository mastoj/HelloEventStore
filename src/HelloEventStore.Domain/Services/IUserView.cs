using System;

namespace HelloEventStore.Domain.Services
{
    public interface IUserView
    {
        bool Exist(string userName);
        Guid GetId(string userName);
        void Insert(Guid id, string userName);
    }
}