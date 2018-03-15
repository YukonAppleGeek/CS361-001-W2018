using System;
using Moq;
using Xunit;
using StudyUp.Database;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using StudyUp.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using StudyUp.Models;
using System.Linq;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace StudyUp.Tests
{
    public class StudyUpControllerTests
    {
        public List<Student> GetSampleStudents()
        {
            return new List<Student>
            {
                new Student()
                {
                    Id = 1,
                    Name = "John Doe"
                }
            };
        }

        public List<StudentCourse> GetSampleStudentCoures()
        {
            return GetSampleCourses().Select(course => new StudentCourse()
            {
                CourseId = course.Id,
                StudentId = GetSampleStudents().First().Id
            }).ToList();
        }

        public List<Course> GetSampleCourses()
        {
            return new List<Course>
            {
                new Course()
                {
                    Id = 1,
                    Name = "Course 1 (CS_1)",
                    StartDate = null,
                    EndDate = new DateTime(2018, 1, 1)
                },
                new Course()
                {
                    Id = 2,
                    Name = "Course 2 (CS_2)",
                    StartDate = null,
                    EndDate = new DateTime(2018, 6, 6)
                },
                new Course()
                {
                    Id = 3,
                    Name = "Course 3 (CS_3)",
                    StartDate = null,
                    EndDate = new DateTime(2018, 7, 1)
                }
            };
        }

        public List<StudyGroup> GetSampleStudyGroups()
        {
            return new List<StudyGroup>()
            {
                new StudyGroup()
                {
                    GroupTitle = "StudyGroup 1",
                    CourseId = 1
                },
                new StudyGroup()
                {
                    GroupTitle = "StudyGroup 2",
                    CourseId = 2
                },
                new StudyGroup()
                {
                    GroupTitle = "StudyGroup 3",
                    CourseId = 2
                },
                new StudyGroup()
                {
                    GroupTitle = "StudyGroup 4",
                    CourseId = 3
                }
            };
        }

        public List<FindViewModel.CourseStudyGroups> GetSampleCourseStudyGroups()
        {
            return GetSampleCourses().Select(course => new FindViewModel.CourseStudyGroups()
            {
                Course = course,
                StudyGroups = GetSampleStudyGroups().Where(sg => sg.CourseId == course.Id).ToList()
            }).ToList();
        }


        [Fact(DisplayName = "Test find model result")]
        public void TestFind() {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder();
            InMemoryDbContextOptionsExtensions.UseInMemoryDatabase(dbContextOptionsBuilder, "test");
            var dbContext = new StudyUpContext(dbContextOptionsBuilder.Options);
            dbContext.Students.AddRange(GetSampleStudents());
            dbContext.Courses.AddRange(GetSampleCourses());
            dbContext.StudentCourses.AddRange(GetSampleStudentCoures());
            dbContext.StudyGroups.AddRange(GetSampleStudyGroups());
            dbContext.SaveChanges();

            var controller = new StudyGroupController(dbContext, new TestClock());

            var user = new Mock<ClaimsPrincipal>();
            user.Setup(p => p.Claims).Returns(new List<Claim>(){ new Claim(ClaimTypes.NameIdentifier, "1")});
            var httpContext = new Mock<HttpContext>();
            httpContext.SetupGet(ctx => ctx.User).Returns(user.Object);
            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = httpContext.Object;

            var result = controller.Find();

            var okResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = okResult.Model.Should().BeOfType<FindViewModel>().Subject;
            // Contain is too greedy and fails if the collections do not have the same object reference
            //var testCollection = GetSampleCourseStudyGroups().Where(i => i.Course.EndDate > TestClock.StaticNow);
            //testCollection.Should().Contain(GetSampleCourseStudyGroups().Where(i => i.Course.EndDate > TestClock.StaticNow));
            model.Courses.Should().OnlyContain(i => i.Course.EndDate > TestClock.StaticNow);
        }
    }
}
