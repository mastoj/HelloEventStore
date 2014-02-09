using System;

namespace HelloEventStore.Domain.Events
{
    public class UserCreated 
    {
        public Guid Id { get; private set; }
        public string UserName { get; private set; }
        public string Name { get; private set; }

        public UserCreated(Guid id, string userName, string name)
        {
            Id = id;
            UserName = userName;
            Name = name;
        }

        protected bool Equals(UserCreated other)
        {
            return Id.Equals(other.Id) && string.Equals(UserName, other.UserName) && string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (UserCreated)) return false;
            return Equals((UserCreated) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Id.GetHashCode();
                hashCode = (hashCode*397) ^ (UserName != null ? UserName.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Name != null ? Name.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return string.Format("Id: {0}, User name: {1}, Name: {2}", Id, UserName, Name);
        }
    }

    public class NameChanged
    {
        public Guid Id { get; private set; }
        public string NewName { get; private set; }

        public NameChanged(Guid id, string newName)
        {
            Id = id;
            NewName = newName;
        }

        protected bool Equals(NameChanged other)
        {
            return Id.Equals(other.Id) && string.Equals(NewName, other.NewName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((NameChanged) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Id.GetHashCode()*397) ^ (NewName != null ? NewName.GetHashCode() : 0);
            }
        }
    }
}