namespace QuizApp.Services.Authentication.Util
{
    public interface IUserNameValidate
    {
        /// <summary>
        /// abstraction for all the required check on the user name or use in general
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool Validate(string username);
    }
}
