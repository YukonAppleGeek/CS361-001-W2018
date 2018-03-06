﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using StudyUp.Database;
using System;

namespace StudyUp.Migrations
{
    [DbContext(typeof(StudyUpContext))]
    [Migration("20180306022934_cancelFlag")]
    partial class cancelFlag
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125");

            modelBuilder.Entity("StudyUp.Database.Course", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("EndDate");

                    b.Property<string>("Name");

                    b.Property<DateTime?>("StartDate");

                    b.HasKey("Id");

                    b.ToTable("Courses");
                });

            modelBuilder.Entity("StudyUp.Database.Student", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Students");
                });

            modelBuilder.Entity("StudyUp.Database.StudentCourse", b =>
                {
                    b.Property<int>("StudentId");

                    b.Property<int>("CourseId");

                    b.HasKey("StudentId", "CourseId");

                    b.HasIndex("CourseId");

                    b.ToTable("StudentCourses");
                });

            modelBuilder.Entity("StudyUp.Database.StudentStudyGroup", b =>
                {
                    b.Property<int>("StudentId");

                    b.Property<int>("StudyGroupId");

                    b.HasKey("StudentId", "StudyGroupId");

                    b.HasIndex("StudyGroupId");

                    b.ToTable("StudentStudyGroups");
                });

            modelBuilder.Entity("StudyUp.Database.StudyGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Cancel");

                    b.Property<int>("Capacity");

                    b.Property<int?>("CourseId");

                    b.Property<TimeSpan>("Duration");

                    b.Property<string>("GroupTitle");

                    b.Property<string>("Location");

                    b.Property<string>("Objectives");

                    b.Property<int?>("OwnerId");

                    b.Property<DateTime>("StartTime");

                    b.HasKey("Id");

                    b.HasIndex("CourseId");

                    b.HasIndex("OwnerId");

                    b.ToTable("StudyGroups");
                });

            modelBuilder.Entity("StudyUp.Database.StudentCourse", b =>
                {
                    b.HasOne("StudyUp.Database.Course", "Course")
                        .WithMany()
                        .HasForeignKey("CourseId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("StudyUp.Database.Student", "Student")
                        .WithMany("Courses")
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("StudyUp.Database.StudentStudyGroup", b =>
                {
                    b.HasOne("StudyUp.Database.Student", "Student")
                        .WithMany("StudyGroups")
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("StudyUp.Database.StudyGroup", "StudyGroup")
                        .WithMany("Members")
                        .HasForeignKey("StudyGroupId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("StudyUp.Database.StudyGroup", b =>
                {
                    b.HasOne("StudyUp.Database.Course", "Course")
                        .WithMany()
                        .HasForeignKey("CourseId");

                    b.HasOne("StudyUp.Database.Student", "Owner")
                        .WithMany()
                        .HasForeignKey("OwnerId");
                });
#pragma warning restore 612, 618
        }
    }
}
