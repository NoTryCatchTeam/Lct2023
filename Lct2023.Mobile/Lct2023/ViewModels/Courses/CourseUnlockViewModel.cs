using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MvvmCross.Commands;
using MvvmCross.Navigation;

namespace Lct2023.ViewModels.Courses;

public class CourseUnlockViewModel : BaseViewModel<CourseUnlockViewModel.NavParameter, CourseUnlockViewModel.NavBackParameter>
{
    public CourseUnlockViewModel(ILoggerFactory logFactory, IMvxNavigationService navigationService)
        : base(logFactory, navigationService)
    {
        OpenCommand = new MvxAsyncCommand(OpenAsync);
    }

    public IMvxAsyncCommand OpenCommand { get; }

    private Task OpenAsync()
    {
        NavigationParameter.CourseItem.IsUnlocked = NavigationParameter.CourseItem.IsPurchased = true;

        return NavigationService.Close(this, new NavBackParameter());
    }

    protected override Task NavigateBackAction() =>
        NavigationService.Close(this);

    public class NavParameter
    {
        public NavParameter(CourseItem courseItem)
        {
            CourseItem = courseItem;
        }

        public CourseItem CourseItem { get; }
    }

    public class NavBackParameter
    {
    }
}
