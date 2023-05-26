using Android.Widget;
using Google.Android.Material.Button;
using Google.Android.Material.Card;
using Lct2023.Android.Bindings;
using Lct2023.Android.Services;
using Lct2023.Factories;
using Lct2023.Services;
using Microsoft.Extensions.Logging;
using MvvmCross;
using MvvmCross.Binding.Bindings.Target.Construction;
using MvvmCross.IoC;
using MvvmCross.Platforms.Android.Binding.Target;
using MvvmCross.Platforms.Android.Core;

namespace Lct2023.Android;

public class Setup : MvxAndroidSetup<App>
{
    protected override void InitializeFirstChance(IMvxIoCProvider iocProvider)
    {
        base.InitializeFirstChance(iocProvider);

        Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IDialogService, DialogService>();
        Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IFileProvider, FileProvider>();
    }

    protected override void FillTargetFactories(IMvxTargetBindingFactoryRegistry registry)
    {
        base.FillTargetFactories(registry);

        registry.RegisterCustomBindingFactory<MaterialButton>(
            nameof(ButtonIconResourceBinding),
            v => new ButtonIconResourceBinding(v));


        registry.RegisterCustomBindingFactory<MaterialCardView>(
            nameof(CardViewBackgroundColorByHexBinding),
            v => new CardViewBackgroundColorByHexBinding(v));
        
        registry.RegisterCustomBindingFactory<ImageView>(
            nameof(MvxImageViewResourceNameTargetBinding),
            v => new MvxImageViewResourceNameTargetBinding(v));

        registry.RegisterCustomBindingFactory<MaterialCardView>(
            nameof(CardViewStrokeColorBinding),
            v => new CardViewStrokeColorBinding(v));
    }

    protected override ILoggerProvider CreateLogProvider() =>
        null;

    protected override ILoggerFactory CreateLogFactory() =>
        SharedLoggerFactory.Create();
}
