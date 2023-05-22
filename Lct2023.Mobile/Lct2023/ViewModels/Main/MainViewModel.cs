using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lct2023.Business.RestServices.Stories;
using Lct2023.Services;
using Microsoft.Extensions.Logging;
using MvvmCross.Navigation;

namespace Lct2023.ViewModels.Main;

public class MainViewModel : BaseViewModel
{
    private readonly IXamarinEssentialsWrapper _xamarinEssentialsWrapper;
    private readonly IStoriesRestService _storiesRestService;

    public MainViewModel(
        IStoriesRestService storiesRestService,
        ILoggerFactory logFactory,
        IMvxNavigationService navigationService,
        IXamarinEssentialsWrapper xamarinEssentialsWrapper)
        : base(logFactory, navigationService)
    {
        _xamarinEssentialsWrapper = xamarinEssentialsWrapper;
        _storiesRestService = storiesRestService;
    }
    
    public IEnumerable<IStoryCardItemViewModel> StoryCards { get; private set; }
    
    public override void ViewCreated()
    {
        base.ViewCreated();

        Task.Run(() => RunSafeTaskAsync(async () =>
        {
            var storyQuizzes = (await _storiesRestService.GetStoryQuizzesAsync(CancellationToken))?.ToArray();

            if (storyQuizzes?.Any() != true)
            {
                return;
            }

            _xamarinEssentialsWrapper.RunOnUi(() => StoryCards = storyQuizzes.Select(storyQuiz => new StoryQuizItemViewModel(storyQuiz.Item)).OrderBy(storyQuiz => storyQuiz.Item.CreatedAt).ToArray());
        }));
    }
}
