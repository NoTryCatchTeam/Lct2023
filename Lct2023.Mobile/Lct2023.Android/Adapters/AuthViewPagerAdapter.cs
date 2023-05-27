using AndroidX.Fragment.App;
using AndroidX.ViewPager2.Adapter;
using Lct2023.Android.Fragments.Auth;
using Lct2023.ViewModels.Auth;

namespace Lct2023.Android.Adapters;

public class AuthViewPagerAdapter : FragmentStateAdapter
{
    private readonly Fragment[] _fragmentsArray;

    public AuthViewPagerAdapter(FragmentActivity activity, AuthViewModel authViewModel)
        : base(activity)
    {
        ItemCount = 2;

        _fragmentsArray = new Fragment[]
        {
            new AuthSignInFragment
            {
                DataContext = authViewModel,
            },
            new AuthSignUpFragment
            {
                DataContext = authViewModel,
            },
        };
    }

    public override int ItemCount { get; }

    public override Fragment CreateFragment(int position) => _fragmentsArray[position];
}
