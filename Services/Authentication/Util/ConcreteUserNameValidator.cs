using QuizApp.Model.Domain;

namespace QuizApp.Services.Authentication.Util
{
    public class ConcreteUserNameValidator : IValidate<User>
    {
        public bool Validate(string username)
        {
            /*22/10/24@todo */
            return true;
        }
    }
}
