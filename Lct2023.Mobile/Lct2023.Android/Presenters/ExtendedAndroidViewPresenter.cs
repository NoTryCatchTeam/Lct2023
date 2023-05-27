using System.Collections.Generic;
using System.Reflection;
using MvvmCross.Platforms.Android.Presenters;
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
    }
}
