using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MvvmCross.Commands;
using MvvmCross.Navigation;

namespace Lct2023.ViewModels.Courses;

public class CourseOpenViewModel : BaseViewModel<CourseOpenViewModel.NavParameter>
{
    public CourseOpenViewModel(ILoggerFactory logFactory, IMvxNavigationService navigationService)
        : base(logFactory, navigationService)
    {
        OpenCommand = new MvxAsyncCommand(OpenAsync);
    }

    public IMvxAsyncCommand OpenCommand { get; }

    private Task OpenAsync()
    {
        NavigationParameter.CourseItem.IsPurchased = true;

        return NavigationService.Close(this);
    }

    public class NavParameter
    {
        public NavParameter(CourseItem courseItem)
        {
            CourseItem = courseItem;
        }

        public CourseItem CourseItem { get; }
    }
}
