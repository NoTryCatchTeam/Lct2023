using System;
using MvvmCross.Platforms.Android.Presenters.Attributes;

namespace Lct2023.Android.Presenters
{
	public class MvxFragmentAdapterChildItemPresentationAttribute : MvxFragmentPresentationAttribute
    {
		public int RootPosition { get; set; }
	}
}

