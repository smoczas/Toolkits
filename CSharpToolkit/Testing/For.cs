using System;

namespace CSharpToolkit.Testing
{
    public class For<T> where T : class
    {
        public For(DependencyContainer dc)
        {
            _dependencyContainer = dc;
        }

        public void Register(Func<object[], T> factoryMethod)
        {
            _dependencyContainer.DependencyFactoryMethods[typeof(T)] = args => factoryMethod(args);
        }

        public void Register(T value)
        {
            Register(_ => value);
        }

        public T Get(object[] args)
        {
            if(_dependencyContainer.DependencyFactoryMethods.TryGetValue(typeof(T), out var factMethod))
            {
                return (T)factMethod(args);
            }

            var result = (T)_dependencyContainer?.UnregisteredDependencyFactoryMethod(typeof(T), args);
            return result ?? throw new NotSupportedException($"Dependency for {typeof(T).Name} is not registered");
        }

        private readonly DependencyContainer _dependencyContainer;
    }
}
