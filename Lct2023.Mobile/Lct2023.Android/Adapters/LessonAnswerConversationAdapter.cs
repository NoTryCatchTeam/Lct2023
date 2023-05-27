using System;
using Android.Content.Res;
using Android.Views;
using Android.Widget;
using Google.Android.Material.Card;
using Lct2023.Android.Bindings;
using Lct2023.Android.Callbacks;
using Lct2023.Converters;
using Lct2023.ViewModels.Courses;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using Square.Picasso;

namespace Lct2023.Android.Adapters;

public class LessonAnswerConversationAdapter : BaseRecyclerViewAdapter<CourseLessonConversation, LessonAnswerConversationAdapter.ConversationViewHolder>
{
    public LessonAnswerConversationAdapter(IMvxAndroidBindingContext bindingContext)
        : base(bindingContext)
    {
    }

    protected override Func<View, IMvxAndroidBindingContext, ConversationViewHolder> BindableViewHolderCreator =>
        (v, c) => new ConversationViewHolder(v, c);

    public class ConversationViewHolder : BaseViewHolder
    {
        private readonly ImageView _image;

        public ConversationViewHolder(View itemView, IMvxAndroidBindingContext context)
            : base(itemView, context)
        {
            _image = itemView.FindViewById<ImageView>(Resource.Id.course_lesson_conversation_item_image);
            var name = itemView.FindViewById<TextView>(Resource.Id.course_lesson_conversation_item_name);
            var date = itemView.FindViewById<TextView>(Resource.Id.course_lesson_conversation_item_date);
            var badge = itemView.FindViewById<MaterialCardView>(Resource.Id.course_lesson_conversation_item_badge);
            var badgeText = itemView.FindViewById<TextView>(Resource.Id.course_lesson_conversation_item_badge_text);
            var text = itemView.FindViewById<TextView>(Resource.Id.course_lesson_conversation_item_text);

            this.DelayBind(() =>
            {
                var set = CreateBindingSet();

                set.Bind(name)
                    .For(x => x.Text)
                    .To(vm => vm.Author.Name);

                set.Bind(date)
                    .For(x => x.Text)
                    .To(vm => vm.CreatedAt)
                    .WithConversion(new AnyExpressionConverter<DateTimeOffset, string>(x => x.ToString("dd.MM.yyyy HH:mm")));

                set.Bind(badge)
                    .For(x => x.CardBackgroundColor)
                    .To(vm => vm.Author.IsStudent)
                    .WithConversion(new AnyExpressionConverter<bool, ColorStateList>(x => itemView.Context.Resources.GetColorStateList(x ? Resource.Color.bgBorder : Resource.Color.accent20)));

                set.Bind(badgeText)
                    .For(x => x.Text)
                    .To(vm => vm.Author.IsStudent)
                    .WithConversion(new AnyExpressionConverter<bool, string>(x => !x ? "Учитель" : "Ученик"));

                set.Bind(badgeText)
                    .For(nameof(TextViewTextColorBinding))
                    .To(vm => vm.Author.IsStudent)
                    .WithConversion(new AnyExpressionConverter<bool, ColorStateList>(x => itemView.Context.Resources.GetColorStateList(x ? Resource.Color.textPrimary : Resource.Color.accent)));

                set.Bind(text)
                    .For(x => x.Text)
                    .To(vm => vm.Text);

                set.Apply();
            });
        }

        public override void Bind()
        {
            base.Bind();

            Picasso.Get()
                .Load(ViewModel.Author.AvatarUrl)
                .Placeholder(Resource.Drawable.ic_profile_circle)
                .Error(Resource.Drawable.ic_profile_circle)
                .Into(
                    _image,
                    () => _image.ImageTintList = null,
                    _ =>
                    {
                    });
        }
    }
}
