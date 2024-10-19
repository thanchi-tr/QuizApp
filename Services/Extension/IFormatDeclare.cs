namespace QuizApp.Services.ConcreteStrategies
{
    public interface IFormatDeclare
    {
        /// <summary>
        /// Get the define format
        /// </summary>
        /// <returns> (questionType, Serialized Json of the Question) </returns>
        public List<(string, string)> Declare();
    }
}
