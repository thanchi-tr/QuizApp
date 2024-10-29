using QuizApp.Services.ConcreteStrategies.MultipleChoice;

namespace QuizApp.Services.ConcreteStrategies
{
    /// <summary>
    /// @Todo:: this model is predecated :: need update
    /// Keep track of all the concrete Question type 
    /// report the actual format.
    /// </summary>
    public class FormatDeclarer : Singleton<FormatDeclarer>
    {

        private List<IFormatDeclare> declareList = new List<IFormatDeclare>();

        public FormatDeclarer()
        {
            declareList.Add(MutipleChoice.Instance);
        }


        public string GetRegisteredFormats()
        {
            string jsonString = @"{
                ""Format"": [
            ";

            foreach (IFormatDeclare format in declareList)
            {
                var declaredValues = format.Declare(); // Declare() returns a tuple (string, string)
                foreach ((string, string) val in declaredValues)
                {
                    jsonString += $@"{{ ""type"": ""{val.Item1}"", ""format"": ""{val.Item2}"" }},";

                }


            }
            // Remove the last comma and close the JSON array and object
            if (jsonString.EndsWith(","))
            {
                jsonString = jsonString.TrimEnd(',');
            }

            jsonString += @"
                ]
            }";

            return jsonString;
        }
    }
}
