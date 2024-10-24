namespace QuizApp.Services.Authentication.Util
{
    public interface IHash
    {
        
        public string Hash(string password); 
        public bool Verify(string password, string authenticatedPassword);
    }
}
