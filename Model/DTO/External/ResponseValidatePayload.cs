namespace QuizApp.Model.DTO.External
{
    public class ResponseValidatePayload
    {
        public string QuesitonId { get; set; }
        public string Correct { get; set; }
        public bool result { get; set; }


        /// <summary>
        /// Default null value
        /// </summary>
        public static ResponseValidatePayload Default { get; } = new ResponseValidatePayload
        {
            QuesitonId = "",
            Correct = "",
            result = false,
        };

        
    }
}
