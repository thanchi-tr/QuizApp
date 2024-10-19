using Microsoft.EntityFrameworkCore;
using QuizApp.Data;

namespace QuizApp.Services.Operation.Provider
{
    /// <summary>
    /// Implement the template design patter:
    ///     to make sure a method only responsible for one operation (type of operation)
    ///     T: The format after complete modify, in which will be return to upper level of abstraction
    ///     U: the 
    /// </summary>
    public abstract class InformationProvider<T, U>
    {
        // Method Injection 
        protected readonly IdeaSpaceDBContext _context;
        protected InformationProvider(IdeaSpaceDBContext context) => _context = context;

        /// <summary>
        ///  Method injection: since an information have
        ///  different type (MChoice, short answer, code
        ///  base, etc)
        ///     the concrete strategy should only be pull at creatation (scoped)
        /// </summary>
        /// <param name="extractor">Concrete extraction strategy</param>
        public string ExtractFromRawData(IExtractStrategy<U> extractor, U input)
        {
            return extractor.Extract(input);
        }

        protected T? _information { get; set; }
        protected abstract void GetRawData(string id);
        protected abstract void Process();

        /// <summary>
        /// A template:
        ///     GetRawData: interaction with context
        ///     Process: turn the context return into a DTO that will be throw arround.
        /// </summary>
        /// <param name="id"> CollectionId : Serialized GUID equivalent to "QuizDb.collection.collection_id"</param>
        /// <returns></returns>
        public T? Get(string id)
        {
            GetRawData(id);
            Process();
            return _information;
        }

    }
}
