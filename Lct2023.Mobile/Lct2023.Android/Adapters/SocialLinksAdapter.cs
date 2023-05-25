using System;
using Android.Views;
using Android.Widget;
using DataModel.Definitions.Enums;
using Google.Android.Material.Button;
using Lct2023.Android.Bindings;
using Lct2023.Converters;
using Lct2023.ViewModels.Map;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.ValueConverters;
using MvvmCross.Platforms.Android.Binding;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace Lct2023.Android.Adapters;

public class SocialLinksAdapter : BaseRecyclerViewAdapter<SocialLinkItemViewModel, SocialLinksAdapter.SocialLinkViewHolder>
{
    public SocialLinksAdapter(IMvxAndroidBindingContext bindingContext)
        : base(bindingContext)
    {
    }

    protected override Func<View, IMvxAndroidBindingContext, SocialLinkViewHolder> BindableViewHolderCreator =>
        (v, c) => new SocialLinkViewHolder(v, c);

    public class SocialLinkViewHolder : BaseViewHolder
    {
        public SocialLinkViewHolder(View itemView, IMvxAndroidBindingContext context)
            : base(itemView, context)
        {
            var button = itemView.FindViewById<MaterialButton>(Resource.Id.social_link_button);

            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<SocialLinkViewHolder, SocialLinkItemViewModel>();

                set.Bind(button)
                    .For(v => v.BindClick())
                    .To(vm => vm.OpenSiteCommand)
                    .WithConversion<MvxCommandParameterValueConverter>(ViewModel.Url);
                
                set.Bind(button)
                    .For(nameof(ButtonIconResourceBinding))
                    .To(vm => vm.SocialLinkType)
                    .WithConversion(new AnyExpressionConverter<SocialLinkTypes, int>(type
                        => type switch
                        {
                            SocialLinkTypes.Vk => Resource.Drawable.ic_open_vk,
                            SocialLinkTypes.Youtube => Resource.Drawable.ic_open_youtube,
                        }));

                set.Apply();
            });
        }
    }
}
