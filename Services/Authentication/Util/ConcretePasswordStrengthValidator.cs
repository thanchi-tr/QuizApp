namespace QuizApp.Services.Authentication.Util
{
    public class ConcretePasswordStrengthValidator : IValidate<String>
    {
        public bool Validate(string password)
        {
            /// 22/10/24@todo: implement a strength check
            /// Follow zero trust (cyber) architecture therefore need to checking before pass down
            return true;
        }
    }
}
