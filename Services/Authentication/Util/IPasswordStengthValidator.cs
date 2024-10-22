namespace QuizApp.Services.Authentication.Util
{
    public interface IPasswordStengthValidate
    {
        public bool Validate(string password);
    }
}
