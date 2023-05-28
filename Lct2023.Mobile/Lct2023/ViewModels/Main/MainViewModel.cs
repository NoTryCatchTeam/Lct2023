using System.Collections.Generic;
using System.Linq;
using Lct2023.Business.RestServices.Stories;
using Lct2023.Definitions.Constants;
using Lct2023.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MvvmCross.Navigation;

namespace Lct2023.ViewModels.Main;

public class MainViewModel : BaseViewModel
{
    private readonly IConfiguration _configuration;
    private readonly IStoriesRestService _storiesRestService;

    public MainViewModel(
        IConfiguration configuration,
        IStoriesRestService storiesRestService,
        ILoggerFactory logFactory,
        IMvxNavigationService navigationService)
        : base(logFactory, navigationService)
    {
        _configuration = configuration;
        _storiesRestService = storiesRestService;
    }

    public IEnumerable<IStoryCardItemViewModel> StoryCards { get; private set; }

    public override void ViewCreated()
    {
        base.ViewCreated();

        RunSafeTaskAsync(async () =>
        {
            var storyQuizzes = (await _storiesRestService.GetStoryQuizzesAsync(CancellationToken))?.ToArray();

            if (storyQuizzes?.Any() != true)
            {
                return;
            }

            var resourcesBaseUrl = $"{_configuration.GetValue<string>(ConfigurationConstants.AppSettings.HOST)}{_configuration.GetValue<string>(ConfigurationConstants.AppSettings.CMS_PATH)}"
                .TrimEnd('/');

            StoryCards = storyQuizzes
                .Select(x =>
                {
                    var result = new StoryQuizItemViewModel(x.Item);

                    if (!string.IsNullOrEmpty(x.Item.Cover?.Data?.Item?.Url))
                    {
                        result.CoverUrl = $"{resourcesBaseUrl}{x.Item.Cover.Data.Item.Url}";
                    }

                    return result;
                })
                .OrderBy(storyQuiz => storyQuiz.Item.CreatedAt).ToArray();

            await RaisePropertyChanged(nameof(StoryCards));
        });
    }
}
