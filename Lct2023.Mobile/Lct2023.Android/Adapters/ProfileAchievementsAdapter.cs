using System;
using Android.Views;
using Android.Widget;
using Lct2023.Converters;
using Lct2023.ViewModels.Profile;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace Lct2023.Android.Adapters;

public class ProfileAchievementsAdapter : BaseRecyclerViewAdapter<AchievementItem, ProfileAchievementsAdapter.AchievementViewHolder>
{
    public ProfileAchievementsAdapter(IMvxAndroidBindingContext bindingContext)
        : base(bindingContext)
    {
    }

    protected override Func<View, IMvxAndroidBindingContext, AchievementViewHolder> BindableViewHolderCreator =>
        (v, c) => new AchievementViewHolder(v, c);

    public class AchievementViewHolder : BaseViewHolder
    {
        private readonly ImageView _image;

        public AchievementViewHolder(View itemView, IMvxAndroidBindingContext context)
            : base(itemView, context)
        {
            _image = itemView.FindViewById<ImageView>(Resource.Id.achievement_item_image);
            var text = itemView.FindViewById<TextView>(Resource.Id.achievement_item_title);

            this.DelayBind(() =>
            {
                var set = CreateBindingSet();

                set.Bind(text)
                    .For(x => x.Text)
                    .To(vm => vm.Number)
                    .WithConversion(new AnyExpressionConverter<int, string>(
                        x => x switch
                        {
                            0 => "Художник",
                            1 => "Дружелюбный",
                            2 => "Игрок",
                            3 => "Настроен как струна",
                            4 => "Хореограф",
                        }));

                set.Apply();
            });
        }

        public override void Bind()
        {
            base.Bind();

            _image.SetImageResource(ViewModel.Number switch
            {
                0 => Resource.Drawable.image_achievement_artist,
                1 => Resource.Drawable.image_achievement_friendly,
                2 => Resource.Drawable.image_achievement_player,
                3 => Resource.Drawable.image_achievement_setted_up,
                4 => Resource.Drawable.image_achievement_choreographer,
            });
        }
    }
}