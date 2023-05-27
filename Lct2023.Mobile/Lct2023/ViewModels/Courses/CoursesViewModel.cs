using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataModel.Definitions.Enums;
using DataModel.Responses.Courses;
using Lct2023.Business.RestServices.Courses;
using Lct2023.Definitions.Constants;
using Lct2023.Definitions.Types;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace Lct2023.ViewModels.Courses;

public class CoursesViewModel : BaseViewModel
{
    private readonly ICoursesRestService _coursesRestService;
    private readonly IConfiguration _configuration;

    public CoursesViewModel(
        ICoursesRestService coursesRestService,
        IConfiguration configuration,
        ILoggerFactory logFactory,
        IMvxNavigationService navigationService)
        : base(logFactory, navigationService)
    {
        _coursesRestService = coursesRestService;
        _configuration = configuration;

        CourseTapCommand = new MvxAsyncCommand<CourseItem>(CourseTapAsync);

        BannersCollection = new MvxObservableCollection<BannerItem>
        {
            new (),
            new (),
            new (),
            new (),
            new (),
            new (),
        };

        CoursesGroupsCollection = new MvxObservableCollection<CourseGroupItem>
        {
            new (
                CourseMajorType.Guitar,
                new[]
                {
                    new CourseItem(
                        "Гитара для начинающих",
                        new[]
                        {
                            new CourseTagItem("Платно", CourseTagItemType.Paid),
                            new CourseTagItem("Видеокурс", CourseTagItemType.Other),
                            new CourseTagItem("Онлайн", CourseTagItemType.Other),
                            new CourseTagItem("Lite", CourseTagItemType.Lite),
                        }),
                    new CourseItem(
                        "Гитара для ПРО",
                        new[]
                        {
                            new CourseTagItem("Платно", CourseTagItemType.Paid),
                            new CourseTagItem("Презентация", CourseTagItemType.Other),
                            new CourseTagItem("Оффлайн", CourseTagItemType.Other),
                            new CourseTagItem("Hard", CourseTagItemType.Hard),
                        })
                    {
                        IsPurchased = true,
                    },
                }),
            new (
                CourseMajorType.FrenchHorn,
                new[]
                {
                    new CourseItem(
                        "Что такое валторна?",
                        new[]
                        {
                            new CourseTagItem("Бесплатно", CourseTagItemType.Free),
                            new CourseTagItem("Видеокурс", CourseTagItemType.Other),
                            new CourseTagItem("Онлайн", CourseTagItemType.Other),
                            new CourseTagItem("Lite", CourseTagItemType.Lite),
                        }),
                    new CourseItem(
                        "Первый урок",
                        new[]
                        {
                            new CourseTagItem("Платно", CourseTagItemType.Paid),
                            new CourseTagItem("Презентация", CourseTagItemType.Other),
                            new CourseTagItem("Оффлайн", CourseTagItemType.Other),
                            new CourseTagItem("Hard", CourseTagItemType.Hard),
                        })
                    {
                        IsPurchased = true,
                    },
                }),
        };
    }

    public IMvxAsyncCommand<CourseItem> CourseTapCommand { get; }

    public MvxObservableCollection<BannerItem> BannersCollection { get; }

    public MvxObservableCollection<CourseGroupItem> CoursesGroupsCollection { get; }

    public override void ViewCreated()
    {
        base.ViewCreated();

        RunSafeTaskAsync(
            async () =>
            {
                var courses = await _coursesRestService.GetCoursesAsync(CancellationToken);

                var resourcesBaseUrl = $"{_configuration.GetValue<string>(ConfigurationConstants.AppSettings.HOST)}{_configuration.GetValue<string>(ConfigurationConstants.AppSettings.CMS_PATH)}"
                    .TrimEnd('/');

                var newGroup = new CourseGroupItem(
                    CourseMajorType.Drums,
                    courses.Select(course =>
                    {
                        // Tags
                        var tags = new List<CourseTagItem>();

                        tags.Add(new CourseTagItem(course.Item.Price == 0 ? "Бесплатно" : "Платно", course.Item.Price == 0 ? CourseTagItemType.Free : CourseTagItemType.Paid));

                        tags.Add(new CourseTagItem(course.Item.IsOnline ? "Онлайн" : "Оффлайн", CourseTagItemType.Other));

                        tags.Add(new CourseTagItem(course.Item.LevelType == CourseLevelType.Lite ? "Lite" : "Hard",
                            course.Item.LevelType == CourseLevelType.Lite ? CourseTagItemType.Lite : CourseTagItemType.Hard));

                        // Lessons
                        var lessons = course.Item.Lessons.Data.Select((lesson, lessonIter) =>
                        {
                            var result = new CourseLessonItem
                            {
                                Number = lesson.Item.Number,
                                Title = lesson.Item.Name,
                                Description = lesson.Item.Description,
                                AdditionalDescription = lesson.Item.AdditionalMaterial,
                                HomeworkDescription = lesson.Item.Homework,
                                Status = course.Item.Price == 0 && lessonIter == 0 ? CourseLessonStatus.Available : CourseLessonStatus.Locked,
                                SectionNumber = lesson.Item.Chapter,
                            };

                            if (lesson.Item.Attachment is { } attachment)
                            {
                                result.Attachment = new CourseLessonAttachment
                                {
                                    Name = attachment.Data.Item.Name,
                                    Extension = attachment.Data.Item.Extension,
                                    Mime = attachment.Data.Item.Mime,
                                    Url = attachment.Data.Item.Extension.Contains("pdf", StringComparison.InvariantCultureIgnoreCase) ?
                                        $"http://45.9.27.2:8081/viewer.html?file=files/{attachment.Data.Item.Url.Split("/").LastOrDefault()}" :
                                        $"{resourcesBaseUrl}{attachment.Data.Item.Url}",
                                };
                            }

                            return result;
                        });

                        return new CourseItem(course.Item.Name, tags)
                        {
                            Lessons = lessons,
                            IsPurchased = course.Item.Price == 0,
                        };
                    }));

                CoursesGroupsCollection.Insert(0, newGroup);
            });
    }

    private Task CourseTapAsync(CourseItem item)
    {
        return NavigationService.Navigate<CourseDetailsViewModel, CourseDetailsViewModel.NavParameter>(
            new CourseDetailsViewModel.NavParameter(item));
    }
}
