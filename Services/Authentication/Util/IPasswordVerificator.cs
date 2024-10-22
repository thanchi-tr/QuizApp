namespace QuizApp.Services.Authentication.Util
{
    public interface IPasswordVerificate
    {
        /// <summary>
        /// compare the two password
        /// </summary>
        /// <param name="password"></param>
        /// <param name="authenticatedPassword"></param>
        /// <returns></returns>
        public bool Verify(string password, string authenticatedPassword);
    }
}
