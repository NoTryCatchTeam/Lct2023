using System;
using Android.Views;
using Android.Widget;
using DataModel.Definitions.Enums;
using Lct2023.Android.Bindings;
using Lct2023.Converters;
using Lct2023.ViewModels.Map;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.ValueConverters;
using MvvmCross.Platforms.Android.Binding;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace Lct2023.Android.Adapters;

public class EventsAdapter : BaseRecyclerViewAdapter<EventItemViewModel, EventsAdapter.EventViewHolder>
{
    public EventsAdapter(IMvxAndroidBindingContext bindingContext)
        : base(bindingContext)
    {
    }

    protected override Func<View, IMvxAndroidBindingContext, EventViewHolder> BindableViewHolderCreator =>
        (v, c) => new EventViewHolder(v, c);

    public class EventViewHolder : BaseViewHolder
    {
        public EventViewHolder(View itemView, IMvxAndroidBindingContext context)
            : base(itemView, context)
        {
            var layout = itemView.FindViewById(Resource.Id.afisha_item_layout);
            var image = itemView.FindViewById<ImageView>(Resource.Id.afisha_item_image);
            var title = itemView.FindViewById<TextView>(Resource.Id.afisha_item_title);
            var description = itemView.FindViewById<TextView>(Resource.Id.afisha_item_description);

            this.DelayBind(() =>
            {
                var set = this.CreateBindingSet<EventViewHolder, EventItemViewModel>();

                set.Bind(layout)
                    .For(v => v.BindClick())
                    .To(vm => vm.OpenSiteCommand)
                    .WithConversion<MvxCommandParameterValueConverter>(ViewModel.Url);
                
                set.Bind(image)
                    .For(nameof(ImageViewByUrlBinding))
                    .To(vm => vm.ImageUrl);
                
                set.Bind(title)
                    .For(v => v.Text)
                    .To(vm => vm.Title);
                
                set.Bind(description)
                    .For(v => v.Text)
                    .To(vm => vm.Description);

                set.Apply();
            });
        }
    }
}
