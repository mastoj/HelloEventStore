using HelloEventStore.Domain.Commands;
using HelloEventStore.Domain.Exceptions;
using HelloEventStore.Infrastructure;

namespace HelloEventStore.Domain.Services
{
    public class UserService : IHandle<CreateUser>, IHandle<ChangeName>
    {
        private readonly IUserView _userView;
        private readonly IDomainRepository _domainRepository;

        public UserService(IUserView userView, IDomainRepository domainRepository)
        {
            _userView = userView;
            _domainRepository = domainRepository;
        }

        public IAggregate Handle(CreateUser command)
        {
            if (_userView.UserExist(command.UserName))
            {
                throw new UserNameTakenException("User with user name " + command.UserName + " already exists");   
            }
            var user = User.CreateUser(command.UserName, command.Name);
            return user;
        }

        public IAggregate Handle(ChangeName command)
        {
            var user = _domainRepository.GetById<User>(command.Id);
            user.ChangeName(command.NewName);
            return user;
        }
    }
}