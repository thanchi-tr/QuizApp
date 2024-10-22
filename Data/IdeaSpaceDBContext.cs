using Microsoft.EntityFrameworkCore;
using QuizApp.Model.Domain;
using System.Reflection.Emit;
namespace QuizApp.Data
{
    public class IdeaSpaceDBContext : DbContext
    {
        public IdeaSpaceDBContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Collection> Collections { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<MetaAnswer> MetaAnswers { get; set; }
        public DbSet<Answer> Answers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // add key auto implementation
            modelBuilder.Entity<User>()
                .Property(u => u.UserId).ValueGeneratedOnAdd();
            modelBuilder.Entity<Collection>()
                .Property(c => c.CollectionId).ValueGeneratedOnAdd();
            modelBuilder.Entity<Question>()
                .Property(q => q.QuestionId).ValueGeneratedOnAdd();


            // Add relation relationship using fluent api
            modelBuilder.Entity<Collection>() //a user have many has many quizes 
                .HasOne(c => c.User)
                .WithMany(c => c.Collections)
                .HasForeignKey(c => c.UserId);
            modelBuilder.Entity<Question>() // A collection/quiz can have multiple question
                .HasOne(q => q.Collection)
                .WithMany(q => q.Questions)
                .HasForeignKey(q => q.CollectionId);
            modelBuilder.Entity<Question>()
                .HasOne(p => p.Answer)
                .WithOne(a => a.Question)
                .HasForeignKey<Answer>(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Answer>()
                .HasOne(a => a.MetaAnswer)
                .WithMany(a => a.Answers)
                .HasForeignKey(a => a.Type);

            // Add default value
            modelBuilder.Entity<Question>()
                .Property(q => q.Level)
                .HasDefaultValue(Level.Level0);
            modelBuilder.Entity<Question>()
                .Property(q => q.LastRevision)
                .HasDefaultValue(DateOnly.FromDateTime(DateTime.Now));

            // some initial entry for debug
            modelBuilder.Entity<User>().HasData(
                new User { UserId= new Guid("2a8c2fd1-4443-4e0e-ac39-062f7c1c75d3"), UserName = "June", HashedPassword="test"}
            );

            modelBuilder.Entity<Collection>().HasData(
                new Collection {CollectionId= new Guid("a0afe69c-3417-4cbf-9c54-b962204350d6"), Name = "Economic", UserId = new Guid("2a8c2fd1-4443-4e0e-ac39-062f7c1c75d3") },
                new Collection { CollectionId = new Guid("22b7f73c-9be2-425c-a3cf-c8a34000ca80"), Name = "Biolgy", UserId = new Guid("2a8c2fd1-4443-4e0e-ac39-062f7c1c75d3") }
            );

            modelBuilder.Entity<Question>().HasData(
                new Question{ 
                    Value="What does a dog say?",
                    CollectionId = new Guid("a0afe69c-3417-4cbf-9c54-b962204350d6"),
                    QuestionId = new Guid("9b90392c-e13b-4c88-acbf-16a845c620c9")
                },
                new Question
                {
                    Value = "What does a Cat say?",
                    CollectionId = new Guid("a0afe69c-3417-4cbf-9c54-b962204350d6"),
                    QuestionId = new Guid("11f4d460-ff85-40b0-ae94-55dbddaab8ff")
                }
            );
            modelBuilder.Entity<MetaAnswer>().HasData(
                new MetaAnswer { Type="MultipleChoice",
                        AnswerServiceType="QuizApp.MultiChoiceAnswerExtractor",
                        TestServiceType= "QuizApp.MultiChoiceTestExtractor",
                        ValidationServiceType= "QuizApp.MultiChoiceAnswerValidator"
                }
            );
            modelBuilder.Entity<Answer>().HasData(
                new Answer { 
                    QuestionId = new Guid("9b90392c-e13b-4c88-acbf-16a845c620c9"),
                    Value = "{choices:[{content: 'woof woff', isCorrect: true},{content: 'woof 1woff', isCorrect: false},{content: 'woof 2woff', isCorrect: false},{content: 'woof coff', isCorrect: false}]}",
                    Type = "MultipleChoice"
                }

            );
        }
    }
}
