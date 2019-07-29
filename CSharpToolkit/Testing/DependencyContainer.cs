using System;
using System.Collections.Generic;

namespace CSharpToolkit.Testing
{
    public class DependencyContainer
    {
        public DependencyContainer()
        {
            DependencyFactoryMethods = new Dictionary<Type, Func<object[], object>>();
        }

        public For<T> For<T>() where T : class
        {
            return new For<T>(this);
        }

        public Func<Type, object[], object> UnregisteredDependencyFactoryMethod { get; set; }

        internal IDictionary<Type, Func<object[], object>> DependencyFactoryMethods { get; }
    }
}
