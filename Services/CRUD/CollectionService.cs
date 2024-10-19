using AutoMapper;
using QuizApp.Data;
using QuizApp.Model.DTO;

namespace QuizApp.Services.CRUD
{
    public class CollectionService : ICRUDService<CollectionDTO>
    {
        private readonly IdeaSpaceDBContext _context;
        private readonly IMapper _mapper;

        public CollectionService(IdeaSpaceDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        /// <summary>
        /// get the list off all the DTO in the db
        /// </summary>
        /// <returns></returns>
        public List<CollectionDTO> GetAll()
        {
            var rawList = _context.Collections.ToList();
            List<CollectionDTO> collectionDTOs = new List<CollectionDTO>();
            foreach (var raw in rawList)
            {
                if(raw != null) 
                    collectionDTOs.Add(_mapper.Map<CollectionDTO>(raw));
            }
            return collectionDTOs;
        }
    }
}
