using System;
using Android.Views;
using Android.Widget;
using Lct2023.ViewModels.Main;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using Square.Picasso;
using Uri = Android.Net.Uri;

namespace Lct2023.Android.Adapters;

public class MainStoriesAdapter : BaseRecyclerViewAdapter<StoryQuizItemViewModel, MainStoriesAdapter.StoryViewHolder>
{
    public MainStoriesAdapter(IMvxAndroidBindingContext bindingContext)
        : base(bindingContext)
    {
    }

    protected override Func<View, IMvxAndroidBindingContext, StoryViewHolder> BindableViewHolderCreator =>
        (v, c) => new StoryViewHolder(v, c);

    public class StoryViewHolder : BaseViewHolder
    {
        private readonly ImageView _image;

        public StoryViewHolder(View itemView, IMvxAndroidBindingContext context)
            : base(itemView, context)
        {
            var text = itemView.FindViewById<TextView>(Resource.Id.main_story_item_text);
            _image = itemView.FindViewById<ImageView>(Resource.Id.main_story_item_image);

            this.DelayBind(() =>
            {
                var set = CreateBindingSet();

                set.Bind(text)
                    .For(x => x.Text)
                    .To(vm => vm.Title);

                set.Apply();
            });
        }

        public override void Bind()
        {
            base.Bind();

            Picasso.Get()
                .Load(Uri.Parse(ViewModel.CoverUrl))
                .Into(_image);
        }
    }
}
