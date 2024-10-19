namespace QuizApp.Services.Operation.Provider
{
    /// <summary>
    /// Extract <some information> from an input of Type T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IExtractStrategy<T>
    {
        string Extract(T input);
    }
}
