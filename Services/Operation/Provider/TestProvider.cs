﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using QuizApp.Data;
using QuizApp.Model.Domain;
using QuizApp.Model.DTO;
using QuizApp.Model.DTO.External;
using QuizApp.Services.ConcreteStrategies.MultipleChoice;
using System.Text.Json;


namespace QuizApp.Services.Operation.Provider
{
    public class TestProvider : InformationProvider<TestDTO, Question>
    {
        private IMapper _mapper;
        // read data from the context and store it there
        private Collection? _collection;
        private List<Question> _questionWithAnswer = new List<Question>();

        public TestProvider(IdeaSpaceDBContext context, IMapper mapper) : base(context) => _mapper = mapper;



        /// <summary>
        /// Retrieve the collection value (information about the quiz) and 
        /// The answers.
        /// </summary>
        /// <param name="id"> Serialized Guid of the collection</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        protected override void GetRawData(string id)
        {
            _collection = _context.Collections
                .FirstOrDefault(q => q.CollectionId == new Guid(id));

            if (_collection != null)
            {
                _questionWithAnswer = _context.Questions
                    .AsNoTracking()
                    .Where(q => q.CollectionId == _collection.CollectionId)
                    .Include(q => q.Answer) // eager loading
                    .ToList();

            }
        }

        protected override void Process()
        {

            _information = new TestDTO
            {
                Title = JsonSerializer.Serialize(_mapper.Map<CollectionDTO>(_collection))
            };
            foreach (var question in _questionWithAnswer)
            {
                var QuestonDTO = _mapper.Map<QuestionDTO>(question);
                ///@todo: find a way  to solve the issue with routing the different question type strategy
                /// this implementation is high specific to multiple choice.
                /// will receive a string with queston

                var ConcreteTest = ExtractFromRawData(new ExtractMultipleChoiceTestStrategy(), question);
                _information.Questions.Add(ConcreteTest);
            }

        }
    }
}
