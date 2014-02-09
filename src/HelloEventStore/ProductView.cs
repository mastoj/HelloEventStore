namespace HelloEventStore
{
    public class ProductView : SimpleAggregateView
    {
        private static ProductView _instance;

        public static ProductView Instance
        {
            get
            {
                _instance = _instance ?? new ProductView();
                return _instance;
            }
        }        
    }
}