using System;
using Android.Runtime;
using Android.Util;
using System.ComponentModel;
using System.Reactive.Disposables;
using Google.Android.Material.Button;
using ReactiveUI;
using System.Reactive.Linq;
using Android.Content;
using Android.Widget;
using AndroidX.Lifecycle;
using DataModel.Definitions.Enums;
using Lct2023.Converters;
using Lct2023.Android.Definitions.Extensions;

namespace Lct2023.Android.Views
{
	public class SegmentedControl : MaterialButtonToggleGroup, INotifyPropertyChanged
    {
        private readonly CompositeDisposable _compositeDisposable = new();

        protected SegmentedControl(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer) => Init();

        public SegmentedControl(Context context, IAttributeSet attrs)
            : base(context, attrs) => Init();

        public SegmentedControl(Context context)
            : base(context) => Init();

        private void Init()
        {
            Observable.FromEventPattern<EventHandler<MaterialButtonToggleGroup.ButtonCheckedEventArgs>, MaterialButtonToggleGroup.ButtonCheckedEventArgs>(
                    h => ButtonChecked += h,
                    h => ButtonChecked -= h)
                .Where(_ => CheckedButtonId != -1)
                .Select(_ => FindViewById(CheckedButtonId))
                .WhereNotNull()
                .Subscribe(button => SelectedSegment = IndexOfChild(button))
                .DisposeWith(_compositeDisposable);

            this
                .WhenAnyValue(c => c.SelectedSegment)
                .Where(c => c != -1)
                .Select(_ => this.GetChildAt<MaterialButton>(SelectedSegment))
                .WhereNotNull()
                .Subscribe(rB => rB.Checked = true)
                .DisposeWith(_compositeDisposable);
        }

        public int SelectedSegment { get; set; } = -1;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _compositeDisposable?.Clear();
            }
            base.Dispose(disposing);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

