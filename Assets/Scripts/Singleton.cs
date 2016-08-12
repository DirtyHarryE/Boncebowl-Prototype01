public class Singleton<T> 
    where T : 
        Singleton<T>, 
        new()
{
    private static readonly T instance = new T();
    public static T Instance
    {
        get
        {
            return instance;
        }
    }
}