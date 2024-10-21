namespace QuizApp.Model.DTO.Internal
{
    public class BusinessToPresentationLayerDTO<T>(bool status, string message, T result)
    {
        public bool Status = status;
        public string Message = message;
        public T Data = result;
    }
}
