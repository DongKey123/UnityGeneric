using System;
using System.Threading;

    public class LazySingleton<T> where T : class
    {
        private static readonly Lazy<T>
            _instance = new Lazy<T>(CreateInstanceOf, LazyThreadSafetyMode.ExecutionAndPublication);

        private static T CreateInstanceOf()
        {
            return Activator.CreateInstance(typeof(T), true) as T;
        }

        public static T getInstance { get { return _instance.Value; } }

        protected LazySingleton()
        {
        }

        ~LazySingleton()
        {
        }
    }