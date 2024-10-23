﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using QuizApp.Data;

#nullable disable

namespace QuizApp.Migrations
{
    [DbContext(typeof(IdeaSpaceDBContext))]
    [Migration("20241023114407_Introduce-refreshtoken")]
    partial class Introducerefreshtoken
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("QuizApp.Model.Domain.Answer", b =>
                {
                    b.Property<Guid>("QuestionId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("answer_id");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("meta_answer_type");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("value");

                    b.HasKey("QuestionId");

                    b.HasIndex("Type");

                    b.ToTable("Answers");

                    b.HasData(
                        new
                        {
                            QuestionId = new Guid("9b90392c-e13b-4c88-acbf-16a845c620c9"),
                            Type = "MultipleChoice",
                            Value = "{choices:[{content: 'woof woff', isCorrect: true},{content: 'woof 1woff', isCorrect: false},{content: 'woof 2woff', isCorrect: false},{content: 'woof coff', isCorrect: false}]}"
                        });
                });

            modelBuilder.Entity("QuizApp.Model.Domain.Collection", b =>
                {
                    b.Property<Guid>("CollectionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("collection_id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("name");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("user_id");

                    b.HasKey("CollectionId");

                    b.HasIndex("UserId");

                    b.ToTable("Collections");

                    b.HasData(
                        new
                        {
                            CollectionId = new Guid("a0afe69c-3417-4cbf-9c54-b962204350d6"),
                            Name = "Economic",
                            UserId = new Guid("2a8c2fd1-4443-4e0e-ac39-062f7c1c75d3")
                        },
                        new
                        {
                            CollectionId = new Guid("22b7f73c-9be2-425c-a3cf-c8a34000ca80"),
                            Name = "Biolgy",
                            UserId = new Guid("2a8c2fd1-4443-4e0e-ac39-062f7c1c75d3")
                        });
                });

            modelBuilder.Entity("QuizApp.Model.Domain.MetaAnswer", b =>
                {
                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(450)")
                        .HasColumnName("type");

                    b.Property<string>("AnswerServiceType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("answer_service");

                    b.Property<string>("TestServiceType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("test_service");

                    b.Property<string>("ValidationServiceType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("validation_service");

                    b.HasKey("Type");

                    b.ToTable("MetaAnswers");

                    b.HasData(
                        new
                        {
                            Type = "MultipleChoice",
                            AnswerServiceType = "QuizApp.MultiChoiceAnswerExtractor",
                            TestServiceType = "QuizApp.MultiChoiceTestExtractor",
                            ValidationServiceType = "QuizApp.MultiChoiceAnswerValidator"
                        });
                });

            modelBuilder.Entity("QuizApp.Model.Domain.Question", b =>
                {
                    b.Property<Guid>("QuestionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("question_id");

                    b.Property<Guid>("CollectionId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("collection_id");

                    b.Property<DateOnly>("LastRevision")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("date")
                        .HasDefaultValue(new DateOnly(2024, 10, 23))
                        .HasColumnName("last_revision");

                    b.Property<int>("Level")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasDefaultValue(0);

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("value");

                    b.HasKey("QuestionId");

                    b.HasIndex("CollectionId");

                    b.ToTable("Questions");

                    b.HasData(
                        new
                        {
                            QuestionId = new Guid("9b90392c-e13b-4c88-acbf-16a845c620c9"),
                            CollectionId = new Guid("a0afe69c-3417-4cbf-9c54-b962204350d6"),
                            LastRevision = new DateOnly(1, 1, 1),
                            Level = 0,
                            Value = "What does a dog say?"
                        },
                        new
                        {
                            QuestionId = new Guid("11f4d460-ff85-40b0-ae94-55dbddaab8ff"),
                            CollectionId = new Guid("a0afe69c-3417-4cbf-9c54-b962204350d6"),
                            LastRevision = new DateOnly(1, 1, 1),
                            Level = 0,
                            Value = "What does a Cat say?"
                        });
                });

            modelBuilder.Entity("QuizApp.Model.Domain.RefreshToken", b =>
                {
                    b.Property<Guid>("RefreshTokenId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("token_id");

                    b.Property<DateTime>("Created")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("Expires")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsRevoked")
                        .HasColumnType("bit");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("RefreshTokenId");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("RefreshTokens");
                });

            modelBuilder.Entity("QuizApp.Model.Domain.User", b =>
                {
                    b.Property<Guid>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("user_id");

                    b.Property<string>("HashedPassword")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("password");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("user_name");

                    b.HasKey("UserId");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            UserId = new Guid("2a8c2fd1-4443-4e0e-ac39-062f7c1c75d3"),
                            HashedPassword = "test",
                            UserName = "June"
                        });
                });

            modelBuilder.Entity("QuizApp.Model.Domain.Answer", b =>
                {
                    b.HasOne("QuizApp.Model.Domain.Question", "Question")
                        .WithOne("Answer")
                        .HasForeignKey("QuizApp.Model.Domain.Answer", "QuestionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("QuizApp.Model.Domain.MetaAnswer", "MetaAnswer")
                        .WithMany("Answers")
                        .HasForeignKey("Type")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("MetaAnswer");

                    b.Navigation("Question");
                });

            modelBuilder.Entity("QuizApp.Model.Domain.Collection", b =>
                {
                    b.HasOne("QuizApp.Model.Domain.User", "User")
                        .WithMany("Collections")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("QuizApp.Model.Domain.Question", b =>
                {
                    b.HasOne("QuizApp.Model.Domain.Collection", "Collection")
                        .WithMany("Questions")
                        .HasForeignKey("CollectionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Collection");
                });

            modelBuilder.Entity("QuizApp.Model.Domain.RefreshToken", b =>
                {
                    b.HasOne("QuizApp.Model.Domain.User", "User")
                        .WithOne("RefreshToken")
                        .HasForeignKey("QuizApp.Model.Domain.RefreshToken", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("QuizApp.Model.Domain.Collection", b =>
                {
                    b.Navigation("Questions");
                });

            modelBuilder.Entity("QuizApp.Model.Domain.MetaAnswer", b =>
                {
                    b.Navigation("Answers");
                });

            modelBuilder.Entity("QuizApp.Model.Domain.Question", b =>
                {
                    b.Navigation("Answer")
                        .IsRequired();
                });

            modelBuilder.Entity("QuizApp.Model.Domain.User", b =>
                {
                    b.Navigation("Collections");

                    b.Navigation("RefreshToken")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
