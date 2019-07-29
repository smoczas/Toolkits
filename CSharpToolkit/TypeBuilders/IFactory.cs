namespace CSharpToolkit.TypeBuilders
{
    public interface IFactory<out TResult>
    {
        TResult Create();
    }

    public interface IFactory<in T1, out TResult>
    {
        TResult Create(T1 arg1);
    }

    public interface IFactory<in T1, in T2, out TResult>
    {
        TResult Create(T1 arg1, T2 arg2);
    }

    public interface IFactory<in T1, in T2, in T3, out TResult>
    {
        TResult Create(T1 arg1, T2 arg2, T3 arg3);
    }
}
