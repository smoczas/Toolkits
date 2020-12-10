using System;
using System.Collections.Generic;

namespace CSharpToolkit.Testing
{
    public class DependencyContainer
    {
        internal DependencyContainer()
        {
            Registered = new Dictionary<Type, Func<object[], object>>();
        }

        public For<T> For<T>() where T : class
        {
            return new For<T>(this);
        }

        public Func<Type, object[], object> UnknownDependencyFactoryMethod { get; set; }

        internal IDictionary<Type, Func<object[], object>> Registered { get; }
    }
}
