using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuizApp.Data;
using QuizApp.Model.Domain;

namespace QuizApp.Services.CRUD
{
    public class QuestionService
    {
        private readonly IdeaSpaceDBContext _context;

        public QuestionService(IdeaSpaceDBContext context)
        {
            _context = context;
        }

        // Map integer to Domain.Level enum
        private Level MapIntToLevel(int level)
        {
            if (Enum.IsDefined(typeof(Level), level))
            {
                return (Level)level;
            }

            throw new ArgumentException($"Invalid level: {level}");
        }

        /// <summary>
        /// The value will also include
        /// </summary>
        /// <param name="collectionId"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<Question> CreateQuestion(Guid collectionId, string value)
        {
            var level = MapIntToLevel(0); // Convert int to Level enum

            var newQuestion = new Question
            {
                QuestionId = Guid.NewGuid(),
                CollectionId = collectionId,
                Value = value,
                Level = level,
                LastRevision = DateOnly.FromDateTime(DateTime.Now)
            };

            _context.Questions.Add(newQuestion);
            await _context.SaveChangesAsync();
            return newQuestion;
        }

        
        
        // Update Question value
        public async Task<Question> UpdateQuestionValue(Guid questionId, string newValue)
        {
            var question = await _context.Questions.FirstOrDefaultAsync(q => q.QuestionId == questionId);

            if (question != null)
            {
                question.Value = newValue;
                question.LastRevision = DateOnly.FromDateTime(DateTime.Now); // Update last revision
                await _context.SaveChangesAsync();
            }

            return question;
        }

        // Delete a Question (and cascade delete the related Answer)
        public async Task<bool> DeleteQuestion(Guid questionId)
        {
            var question = await _context.Questions
                                         .Include(q => q.Answer) // Include the Answer to ensure it gets deleted
                                         .FirstOrDefaultAsync(q => q.QuestionId == questionId);

            if (question != null)
            {
                _context.Questions.Remove(question);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        
    }


}
