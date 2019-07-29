namespace CSharpToolkit.TypeBuilders
{
    public interface IGetter<out R>
    {
        R Get();
    }

    public interface IGetter<in T1, out R>
    {
        R Get(T1 a1);
    }

    public interface IGetter<in T1, in T2, out R>
    {
        R Get(T1 a1, T2 a2);
    }
}
