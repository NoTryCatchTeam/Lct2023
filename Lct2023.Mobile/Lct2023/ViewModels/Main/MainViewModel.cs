using System.Collections.Generic;
using System.Linq;
using Lct2023.Business.RestServices.Map;
using Lct2023.Business.RestServices.Stories;
using Lct2023.Definitions.Constants;
using Lct2023.Services;
using Lct2023.ViewModels.Map;
using Lct2023.ViewModels.ProfTest;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MvvmCross.Commands;
using MvvmCross.Navigation;

namespace Lct2023.ViewModels.Main;

public class MainViewModel : BaseViewModel
{
    private readonly IConfiguration _configuration;
    private readonly IMapRestService _mapRestService;
    private readonly IStoriesRestService _storiesRestService;
    private readonly string _resourcesBaseUrl;

    public MainViewModel(
        IConfiguration configuration,
        IMapRestService mapRestService,
        IStoriesRestService storiesRestService,
        ILoggerFactory logFactory,
        IMvxNavigationService navigationService)
        : base(logFactory, navigationService)
    {
        _configuration = configuration;
        _mapRestService = mapRestService;
        _storiesRestService = storiesRestService;

        StartProfTestCommand = new MvxAsyncCommand(() => NavigationService.Navigate<ProfTestStartViewModel>());

        _resourcesBaseUrl = $"{_configuration.GetValue<string>(ConfigurationConstants.AppSettings.HOST)}{_configuration.GetValue<string>(ConfigurationConstants.AppSettings.CMS_PATH)}"
            .TrimEnd('/');
    }

    public IMvxAsyncCommand StartProfTestCommand { get; }

    public IEnumerable<IStoryCardItemViewModel> StoryCards { get; private set; }

    public IEnumerable<EventItemViewModel> EventsCollection { get; private set; }

    public override void ViewCreated()
    {
        base.ViewCreated();

        // Stories
        RunSafeTaskAsync(async () =>
        {
            var storyQuizzes = (await _storiesRestService.GetStoryQuizzesAsync(CancellationToken))?.ToArray();

            if (storyQuizzes?.Any() != true)
            {
                return;
            }

            StoryCards = storyQuizzes
                .Select(x =>
                {
                    var result = new StoryQuizItemViewModel(x.Item);

                    if (!string.IsNullOrEmpty(x.Item.Cover?.Data?.Item?.Url))
                    {
                        result.CoverUrl = $"{_resourcesBaseUrl}{x.Item.Cover.Data.Item.Url}";
                    }

                    return result;
                })
                .OrderBy(storyQuiz => storyQuiz.Item.CreatedAt).ToArray();

            await RaisePropertyChanged(nameof(StoryCards));
        });

        // Events
        RunSafeTaskAsync(async () =>
        {
            var events = (await _mapRestService.GetMainEventsAsync(CancellationToken))?.ToArray();

            EventsCollection = events?
                .Select(x =>
                {
                    var result = new EventItemViewModel
                    {
                        Title = x.Item.Name,
                        Description = x.Item.Description,
                    };

                    if (!string.IsNullOrEmpty(x.Item.Cover?.Data?.Item?.Url))
                    {
                        result.ImageUrl = $"{_resourcesBaseUrl}{x.Item.Cover.Data.Item.Url}";
                    }

                    return result;
                })
                .ToList();

            await RaisePropertyChanged(nameof(EventsCollection));
        });
    }
}
