using System;
using Android.Views;
using Android.Widget;
using Lct2023.ViewModels.Map;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.BindingContext;
using Square.Picasso;
using Uri = Android.Net.Uri;

namespace Lct2023.Android.Adapters;

public class MainEventsAdapter : BaseRecyclerViewAdapter<EventItemViewModel, MainEventsAdapter.EventViewHolder>
{
    public MainEventsAdapter(IMvxAndroidBindingContext bindingContext)
        : base(bindingContext)
    {
    }

    protected override Func<View, IMvxAndroidBindingContext, EventViewHolder> BindableViewHolderCreator =>
        (v, c) => new EventViewHolder(v, c);

    public class EventViewHolder : BaseViewHolder
    {
        private readonly ImageView _image;

        public EventViewHolder(View itemView, IMvxAndroidBindingContext context)
            : base(itemView, context)
        {
            var title = itemView.FindViewById<TextView>(Resource.Id.main_events_list_item_title);
            _image = itemView.FindViewById<ImageView>(Resource.Id.main_events_list_item_image);

            this.DelayBind(() =>
            {
                var set = CreateBindingSet();

                set.Bind(title)
                    .For(x => x.Text)
                    .To(vm => vm.Title);

                set.Apply();
            });
        }

        public override void Bind()
        {
            base.Bind();

            if (!string.IsNullOrEmpty(ViewModel.ImageUrl))
            {
                Picasso.Get()
                    .Load(Uri.Parse(ViewModel.ImageUrl))
                    .Into(_image);
            }
        }
    }
}
