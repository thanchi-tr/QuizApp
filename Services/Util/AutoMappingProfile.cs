using AutoMapper;
using QuizApp.Model.Domain;
using QuizApp.Model.DTO;

namespace QuizApp.Services.Util
{
    public class AutoMappingProfile : Profile
    {
        private int LevelToInt(Level level)
        {
            switch (level)
            {
                default: return 0;
                case Level.Level1:
                    return 1;
                case Level.Level2:
                    return 2;
                case Level.Level3:
                    return 3;
                case Level.Level4:
                    return 4;
                case Level.Level5:
                    return 5;
                case Level.Level6:
                    return 6;
                case Level.Level7:
                    return 7;
                case Level.Level8:
                    return 8;
                case Level.Level9:
                    return 9;
                case Level.Level10:
                    return 10;
            }
        }
        public AutoMappingProfile()
        {
            CreateMap<Collection, CollectionDTO>();
            CreateMap<CollectionDTO, Collection>();


            CreateMap<Question, QuestionDTO>()
                .ForMember(dest => dest.Level, opt => opt.MapFrom(src => LevelToInt(src.Level)))
                .ForMember(dest => dest.Format, opt => opt.Ignore()); // we map the format by our self
            CreateMap<QuestionDTO, Question>();
        }
    }
}
