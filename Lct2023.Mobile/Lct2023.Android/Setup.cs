using Android.Widget;
using Google.Android.Material.Button;
using Google.Android.Material.Card;
using Google.Android.Material.ProgressIndicator;
using Lct2023.Android.Bindings;
using Lct2023.Android.Presenters;
using Lct2023.Android.Services;
using Lct2023.Factories;
using Lct2023.Services;
using Microsoft.Extensions.Logging;
using MvvmCross;
using MvvmCross.Binding.Bindings.Target.Construction;
using MvvmCross.IoC;
using MvvmCross.Platforms.Android.Binding.Target;
using MvvmCross.Platforms.Android.Core;
using MvvmCross.Platforms.Android.Presenters;

namespace Lct2023.Android;

public class Setup : MvxAndroidSetup<App>
{
    protected override void InitializeFirstChance(IMvxIoCProvider iocProvider)
    {
        base.InitializeFirstChance(iocProvider);

        Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IDialogService, DialogService>();
        Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IPlatformFileProvider, PlatformFileProvider>();
        Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IPlatformFileViewer, PlatformFileViewer>();
    }

    protected override void FillTargetFactories(IMvxTargetBindingFactoryRegistry registry)
    {
        base.FillTargetFactories(registry);

        registry.RegisterCustomBindingFactory<LinearProgressIndicator>(
            nameof(LinearIndicatorProgressBinding),
            v => new LinearIndicatorProgressBinding(v));

        registry.RegisterCustomBindingFactory<MaterialButton>(
            nameof(ButtonBackgroundByIdBinding),
            v => new ButtonBackgroundByIdBinding(v));

        registry.RegisterCustomBindingFactory<MaterialButton>(
            nameof(ButtonIconResourceBinding),
            v => new ButtonIconResourceBinding(v));

        registry.RegisterCustomBindingFactory<MaterialCardView>(
            nameof(CardViewBackgroundColorByHexBinding),
            v => new CardViewBackgroundColorByHexBinding(v));

        registry.RegisterCustomBindingFactory<ImageView>(
            nameof(MvxImageViewResourceNameTargetBinding),
            v => new MvxImageViewResourceNameTargetBinding(v));

        registry.RegisterCustomBindingFactory<ImageView>(
            nameof(MvxImageViewBitmapTargetBinding),
            v => new MvxImageViewBitmapTargetBinding(v));

        registry.RegisterCustomBindingFactory<MaterialCardView>(
            nameof(CardViewStrokeColorBinding),
            v => new CardViewStrokeColorBinding(v));

        registry.RegisterCustomBindingFactory<TextView>(
            nameof(TextViewTextColorBinding),
            v => new TextViewTextColorBinding(v));

        registry.RegisterCustomBindingFactory<TextView>(
            nameof(TextViewMaxLinesBinding),
            v => new TextViewMaxLinesBinding(v));

        registry.RegisterCustomBindingFactory<ImageView>(
            nameof(ImageViewByResIdBinding),
            v => new ImageViewByResIdBinding(v));
    }

    protected override IMvxAndroidViewPresenter CreateViewPresenter() =>
        new ExtendedAndroidViewPresenter(AndroidViewAssemblies);

    protected override IMvxIocOptions CreateIocOptions() =>
        new MvxIocOptions
        {
            PropertyInjectorOptions = MvxPropertyInjectorOptions.MvxInject,
        };

    protected override ILoggerProvider CreateLogProvider() =>
        null;

    protected override ILoggerFactory CreateLogFactory() =>
        SharedLoggerFactory.Create();
}
