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
                },
                new Student()
                {
                    Id = 2,
                    Name = "Doe John"
                }
            };
        }

        public List<StudentCourse> GetSampleStudentCoures()
        {
            return new List<StudentCourse>
            {
                new StudentCourse()
                {
                    CourseId = 1,
                    StudentId = GetSampleStudents().First().Id
                },
                new StudentCourse()
                {
                    CourseId = 2,
                    StudentId = GetSampleStudents().First().Id
                }
            };
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
                    Id = 1,
                    GroupTitle = "StudyGroup 1",
                    CourseId = 1,
                    OwnerId = 1
                },
                new StudyGroup()
                {
                    Id = 2,
                    GroupTitle = "StudyGroup 2",
                    CourseId = 2
                },
                new StudyGroup()
                {
                    Id = 3,
                    GroupTitle = "StudyGroup 3",
                    CourseId = 2
                },
                new StudyGroup()
                {
                    Id = 4,
                    GroupTitle = "StudyGroup 4",
                    CourseId = 3
                }
            };
        }

        public List<StudentStudyGroup> GetSampleStudentStudyGroups()
        {
            return new List<StudentStudyGroup>()
            {
                new StudentStudyGroup()
                {
                    StudentId = 1,
                    StudyGroupId = 1
                },
                new StudentStudyGroup()
                {
                    StudentId = 1,
                    StudyGroupId = 2
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

        public StudyUpContext SetupDatabase()
        {
            var dbContextOptionsBuilder = new DbContextOptionsBuilder();
            InMemoryDbContextOptionsExtensions.UseInMemoryDatabase(dbContextOptionsBuilder, "test");
            var dbContext = new StudyUpContext(dbContextOptionsBuilder.Options);
            dbContext.Database.EnsureDeleted();
            dbContext.Students.AddRange(GetSampleStudents());
            dbContext.Courses.AddRange(GetSampleCourses());
            dbContext.StudentCourses.AddRange(GetSampleStudentCoures());
            dbContext.StudyGroups.AddRange(GetSampleStudyGroups());
            dbContext.StudentStudyGroups.AddRange(GetSampleStudentStudyGroups());
            dbContext.SaveChanges();

            return dbContext;
        }

        public StudyGroupController SetupController(StudyUpContext dbContext = null, int userId = 1)
        {
            if (dbContext == null) dbContext = SetupDatabase();

            var controller = new StudyGroupController(dbContext, new TestClock());

            var user = new Mock<ClaimsPrincipal>();
            user.Setup(p => p.Claims).Returns(new List<Claim>() { new Claim(ClaimTypes.NameIdentifier, userId.ToString()) });
            var httpContext = new Mock<HttpContext>();
            httpContext.SetupGet(ctx => ctx.User).Returns(user.Object);

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext.Object
            };

            return controller;
        }

        [Fact(DisplayName = "Find action returns valid model")]
        public void Find_Action_Return_Valid_Model() {
            var controller = SetupController();

            var result = controller.Find();

            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeOfType<FindViewModel>().Subject;
            // Contain is too greedy and fails if the collections do not have the same object reference
            //var testCollection = GetSampleCourseStudyGroups().Where(i => i.Course.EndDate > TestClock.StaticNow);
            //testCollection.Should().Contain(GetSampleCourseStudyGroups().Where(i => i.Course.EndDate > TestClock.StaticNow));
            model.Courses.Should().OnlyContain(i => i.Course.EndDate > TestClock.StaticNow);
        }

        [Fact(DisplayName = "Join action updates database")]
        public void Join_Action_Updates_Database()
        {
            var dbContext = SetupDatabase();
            var controller = SetupController(dbContext, 2);

            var result = controller.Join(1);

            dbContext.StudentStudyGroups.Should().Contain(p => p.StudentId == 2 && p.StudyGroupId == 1);
        }

        [Fact(DisplayName = "Leave action when not in group")]
        public void Leave_Action_Not_Member()
        {
            var dbContext = SetupDatabase();
            var controller = SetupController(dbContext, 2);

            var result = controller.Leave(3);
        }
        
        [Fact(DisplayName = "Create action displays courses without id")]
        public void Create_Action_Displays_Courses()
        {
            var dbContext = SetupDatabase();
            var controller = SetupController(dbContext);

            var result = controller.Create();
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeOfType<ChooseCourseViewModel>().Subject;

            model.Courses.Should().HaveCount(1).And.OnlyContain(i => i.Id == 2);
        }

        [Fact(DisplayName = "Create action displays form")]
        public void Create_Action_Displays_Form()
        {
            var controller = SetupController();

            var result = controller.Create(2);
            var viewResult = result.Should().BeOfType<ViewResult>().Subject;
            var model = viewResult.Model.Should().BeOfType<CreateViewModel>().Subject;
        }

        // *****************************
        // ***** Integration tests *****
        // *****************************

        [Fact(DisplayName = "Canceling course displays message on view")]
        public void Canceling_Displays_View_Message()
        {
            var controller = SetupController();

            var cancelResult = controller.Cancel(1);
            var viewResult = controller.View(1);

            var viewViewResult = viewResult.Should().BeOfType<ViewResult>().Subject;
            var model = viewViewResult.Model.Should().BeOfType<StudyGroupViewModel>().Subject;
            model.IsCanceled.Should().BeTrue();
        }

        [Fact(DisplayName = "Edit submit redirects to view")]
        public void Edit_Redirect_View()
        {
            var controller = SetupController();

            var model = new CreateViewModel()
            {
                Title = "StudyGroup 1 Changed",
                DateDay = TestClock.StaticNow.Day,
                DateMonth = TestClock.StaticNow.Month,
                DateYear = TestClock.StaticNow.Year,
                StartHour = TestClock.StaticNow.Hour,
                StartMin = TestClock.StaticNow.Minute,
                Duration = 1,
                Capacity = 3
            };
            var result = controller.Edit(1, model);

            var resultAction = result.Should().BeOfType<RedirectToActionResult>().Subject;
            resultAction.ActionName.Should().Be("View");
            resultAction.RouteValues["id"].Should().Be(1);
        }
    }
}
