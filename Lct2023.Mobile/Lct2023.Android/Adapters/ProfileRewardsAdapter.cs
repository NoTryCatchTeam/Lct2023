using System;
using Android.Content.Res;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.Card;
using Lct2023.Android.Bindings;
using Lct2023.Android.Helpers;
using Lct2023.Converters;
using Lct2023.ViewModels.Profile;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.Target;

namespace Lct2023.Android.Adapters;

public class ProfileRewardsAdapter : BaseRecyclerViewAdapter<ProfileRewardItem, ProfileRewardsAdapter.RewardViewHolder>
{
    public ProfileRewardsAdapter(IMvxAndroidBindingContext bindingContext)
        : base(bindingContext)
    {
    }

    protected override Func<View, IMvxAndroidBindingContext, RewardViewHolder> BindableViewHolderCreator =>
        (v, c) => new RewardViewHolder(v, c);

    public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
    {
        var holder = base.OnCreateViewHolder(parent, viewType);

        if (holder.ItemView is { } view)
        {
            var itemLayoutParams = (ViewGroup.MarginLayoutParams)view.LayoutParameters;
            itemLayoutParams.Width =
                (parent.MeasuredWidth -
                 parent.PaddingLeft -
                 parent.PaddingRight -
                 DimensUtils.DpToPx(parent.Context, 4) * 6) / 7;

            view.LayoutParameters = itemLayoutParams;
        }

        return holder;
    }

    public class RewardViewHolder : BaseViewHolder
    {
        public RewardViewHolder(View itemView, IMvxAndroidBindingContext context)
            : base(itemView, context)
        {
            var parent = itemView.FindViewById<MaterialCardView>(Resource.Id.profile_daily_reward_item_layout);
            var icon = itemView.FindViewById<ImageView>(Resource.Id.profile_daily_reward_item_icon);
            var text = itemView.FindViewById<TextView>(Resource.Id.profile_daily_reward_item_value);

            this.DelayBind(() =>
            {
                var set = CreateBindingSet();

                set.Bind(parent)
                    .For(x => x.CardBackgroundColor)
                    .To(vm => vm.IsCollected)
                    .WithConversion(new AnyExpressionConverter<bool, ColorStateList>(
                        x => itemView.Context.GetColorStateList(x ? Resource.Color.accent : Resource.Color.bgIslandInner)));

                set.Bind(parent)
                    .For(x => x.StrokeWidth)
                    .To(vm => vm.IsAvailable)
                    .WithConversion(new AnyExpressionConverter<bool, int>(x => x ? 2 : 0));

                set.Bind(icon)
                    .For(x => x.ImageTintList)
                    .To(vm => vm.IsCollected)
                    .WithConversion(new AnyExpressionConverter<bool, ColorStateList>(
                        x => x ? null : itemView.Context.GetColorStateList(Resource.Color.accent)));

                set.Bind(icon)
                    .For(nameof(MvxImageViewResourceNameTargetBinding))
                    .To(vm => vm.IsCollected)
                    .WithConversion(new AnyExpressionConverter<bool, int>(
                        x => x ? Resource.Drawable.ic_reward_checkbox : Resource.Drawable.ic_crown));

                set.Bind(text)
                    .For(x => x.Text)
                    .To(vm => vm.Points)
                    .WithConversion(new AnyExpressionConverter<int, string>(x => $"+{x}"));

                set.Bind(text)
                    .For(nameof(TextViewTextColorBinding))
                    .To(vm => vm.IsCollected)
                    .WithConversion(new AnyExpressionConverter<bool, ColorStateList>(
                        x => itemView.Context.GetColorStateList(x ? Resource.Color.textLight : Resource.Color.textPrimary)));

                set.Apply();
            });
        }
    }
}
