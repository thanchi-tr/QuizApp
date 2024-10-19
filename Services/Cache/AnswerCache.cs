using Microsoft.Extensions.Caching.Memory;
using QuizApp.Model.Domain;
using QuizApp.Model.DTO;
using QuizApp.Services.ConcreteStrategies;

namespace QuizApp.Services.Cache
{
    /// <summary>
    /// MVP @15/10/24: lazy loading, invalidating cache
    /// </summary>
    public class AnswerCache :
            IInformationCache<AnswersDTO>
    {
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(20);

        public AnswerCache(IMemoryCache cache) : base()
        {
            _cache = cache;
        }

        public AnswersDTO? Get(string id)
        {
            if (!_cache.TryGetValue(id, out AnswersDTO? cachedProducts))
            {
                return null;

            }
            return cachedProducts;
        }
        public void Cache(AnswersDTO answer, string id)
        {
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _cacheExpiration,  // Expire after 10 minutes
                SlidingExpiration = TimeSpan.FromMinutes(5)  // Optional: refresh cache if accessed within 5 minutes
            };

            // Set the cache
            _cache.Set(id, answer, cacheOptions);
        }
    }



}
