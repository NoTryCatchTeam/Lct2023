using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Threading.Tasks;
using Android.Views;
using AndroidX.Fragment.App;
using MvvmCross.Platforms.Android.Presenters;
using MvvmCross.Platforms.Android.Views;
using MvvmCross.Presenters;

namespace Lct2023.Android.Presenters;

public class ExtendedAndroidViewPresenter : MvxAndroidViewPresenter
{
    public ExtendedAndroidViewPresenter(IEnumerable<Assembly> androidViewAssemblies)
        : base(androidViewAssemblies)
    {
    }

    public override void RegisterAttributeTypes()
    {
        base.RegisterAttributeTypes();

        AttributeTypesToActionsDictionary.Register<MvxRootActivityPresentationAttribute>(
            (type, presentation, request) =>
            {
                CurrentActivity?.FinishAffinity();

                return ShowActivity(type, presentation, request);
            },
            CloseActivity);

        AttributeTypesToActionsDictionary.Register<MvxFragmentAdapterChildItemPresentationAttribute>(
            (type, presentation, request) =>
            {
                Fragment fragmentByViewType = CurrentFragmentManager.Fragments?.ElementAtOrDefault(presentation.RootPosition);
                if (fragmentByViewType == null
                    || fragmentByViewType.GetType() != presentation.FragmentHostViewType)
                {
                    throw new InvalidOperationException("Fragment host not found when trying to show View " + type.Name + " as Nested Fragment");
                }
                PerformShowFragmentTransaction(fragmentByViewType.ChildFragmentManager, presentation, request);

                return Task.FromResult(result: true);
            },
            (vm, presentation) =>
            {
                Fragment fragmentByViewType = CurrentFragmentManager.Fragments?.ElementAtOrDefault(presentation.RootPosition);
                if (fragmentByViewType == null
                    || fragmentByViewType.GetType() != presentation.FragmentHostViewType)
                {
                    throw new InvalidOperationException("Fragment host not found when trying to show View " + presentation.FragmentHostViewType.Name + " as Nested Fragment");
                }

                if (fragmentByViewType != null && TryPerformCloseFragmentTransaction(fragmentByViewType.ChildFragmentManager, presentation))
                {
                    return Task.FromResult(result: true);
                }

                return Task.FromResult(result: false);
            });
    }
}
