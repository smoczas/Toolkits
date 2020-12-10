using System;

namespace CSharpToolkit.Testing
{
    public class For<T> where T : class
    {
        public For(DependencyContainer dc)
        {
            _container = dc;
        }

        public void Register(Func<object[], T> factoryMethod)
        {
            _container.Registered[typeof(T)] = args => factoryMethod(args);
        }

        public void Register(T value)
        {
            Register(_ => value);
        }

        public T Get(object[] args)
        {
            var result = default(T);
            var dependencyType = typeof(T);
            if(_container.Registered.TryGetValue(dependencyType, out var factMethod))
            {
                result = (T)factMethod(args);
            }

            if (result != null)
            {
                return result;
            }

            result = (T)_container?.UnknownDependencyFactoryMethod(dependencyType, args);
            return result ?? throw new NotSupportedException($"Dependency for {dependencyType.Name} is not registered");
        }

        private readonly DependencyContainer _container;
    }
}
