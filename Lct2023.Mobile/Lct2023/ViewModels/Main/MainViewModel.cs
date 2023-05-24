using System.Collections.Generic;
using System.Linq;
using Lct2023.Business.RestServices.Stories;
using Lct2023.Services;
using Microsoft.Extensions.Logging;
using MvvmCross.Navigation;

namespace Lct2023.ViewModels.Main;

public class MainViewModel : BaseViewModel
{
    private readonly IStoriesRestService _storiesRestService;

    public MainViewModel(
        IStoriesRestService storiesRestService,
        ILoggerFactory logFactory,
        IMvxNavigationService navigationService,
        IXamarinEssentialsWrapper xamarinEssentialsWrapper)
        : base(logFactory, navigationService)
    {
        _storiesRestService = storiesRestService;
    }

    public IEnumerable<IStoryCardItemViewModel> StoryCards { get; private set; }

    public string Image { get; private set; } =
        "https://media.newyorker.com/photos/59095bb86552fa0be682d9d0/master/w_2560%2Cc_limit/Monkey-Selfie.jpg";

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

            StoryCards = storyQuizzes.Select(storyQuiz => new StoryQuizItemViewModel(storyQuiz.Item)).OrderBy(storyQuiz => storyQuiz.Item.CreatedAt).ToArray();

            await RaisePropertyChanged(nameof(StoryCards));
        });
    }
}
