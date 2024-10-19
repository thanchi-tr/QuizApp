namespace QuizApp.Services.ConcreteStrategies
{
    public class Singleton<T> where T : new()
    {
        private static T _instance = default;
        private static readonly object _lock = new object();

        // Private constructor prevents external instantiation
        protected Singleton()
        {
        }

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new T();
                        }
                    }
                }
                return _instance;
            }
        }
    }
}
