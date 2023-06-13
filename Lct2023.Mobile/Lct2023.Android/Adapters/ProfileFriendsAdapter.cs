using System;
using Android.Views;
using Android.Widget;
using Lct2023.ViewModels.Profile;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Binding.BindingContext;

namespace Lct2023.Android.Adapters;

public class ProfileFriendsAdapter : BaseRecyclerViewAdapter<FriendItem, ProfileFriendsAdapter.FriendViewHolder>
{
    public ProfileFriendsAdapter(IMvxAndroidBindingContext bindingContext)
        : base(bindingContext)
    {
    }

    protected override Func<View, IMvxAndroidBindingContext, FriendViewHolder> BindableViewHolderCreator =>
        (v, c) => new FriendViewHolder(v, c);

    public class FriendViewHolder : BaseViewHolder
    {
        public FriendViewHolder(View itemView, IMvxAndroidBindingContext context)
            : base(itemView, context)
        {
            var name = itemView.FindViewById<TextView>(Resource.Id.profile_friend_item_name);
            var rating = itemView.FindViewById<TextView>(Resource.Id.profile_friend_item_rating_value);

            this.DelayBind(() =>
            {
                var set = CreateBindingSet();

                set.Bind(name)
                    .For(x => x.Text)
                    .To(vm => vm.Name);

                set.Bind(rating)
                    .For(x => x.Text)
                    .To(vm => vm.Rating);

                set.Apply();
            });
        }
    }
}