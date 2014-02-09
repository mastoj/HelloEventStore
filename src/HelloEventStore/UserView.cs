using HelloEventStore.Domain.Services;

namespace HelloEventStore
{
    public class UserView : SimpleAggregateView, IUserView
    {
        private static UserView _instance;

        public static UserView Instance
        {
            get
            {
                _instance = _instance ?? new UserView();
                return _instance;
            }
        }
    }
}