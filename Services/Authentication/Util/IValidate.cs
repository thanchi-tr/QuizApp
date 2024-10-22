namespace QuizApp.Services.Authentication.Util
{
    public interface IValidate<T>  where T : class
    {
        /// <summary>
        /// abstraction for all the required check on the user name or use in general
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool Validate(string username);
    }
}
