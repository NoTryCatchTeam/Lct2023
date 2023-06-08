using System;
using System.Globalization;
using Android.Views;
using Android.Widget;
using Lct2023.Converters;
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
            var subtitle = itemView.FindViewById<TextView>(Resource.Id.main_events_list_item_subtitle);
            _image = itemView.FindViewById<ImageView>(Resource.Id.main_events_list_item_image);

            this.DelayBind(() =>
            {
                var set = CreateBindingSet();

                set.Bind(title)
                    .For(x => x.Text)
                    .To(vm => vm.Title);

                set.Bind(subtitle)
                    .For(x => x.Text)
                    .To(vm => vm.EventDate)
                    .WithConversion(new AnyExpressionConverter<DateTime, string>(x =>
                        $"{x.Day} {new CultureInfo("ru-RU").DateTimeFormat.MonthGenitiveNames[x.Month - 1]}, {x:HH:mm}"));

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
