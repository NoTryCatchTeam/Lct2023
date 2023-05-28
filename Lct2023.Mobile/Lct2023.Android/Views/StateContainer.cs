using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;
using AndroidX.ConstraintLayout.Widget;
using AndroidX.Core.Content;
using Google.Android.Material.ProgressIndicator;
using Lct2023.Android.Definitions.Extensions;
using Lct2023.Commons.Extensions;
using Lct2023.Definitions.Enums;
using MvvmCross.Commands;
using ReactiveUI;

namespace Lct2023.Android.Views;

public class StateContainer: ConstraintLayout, INotifyPropertyChanged
{
    private IDisposable _disposable;
    private State _previousState;
    private CircularProgressIndicator _indicator;

    protected StateContainer(IntPtr javaReference, JniHandleOwnership transfer)
      : base(javaReference, transfer)
    {
        Init();
    }

    public StateContainer(Context context, IAttributeSet attrs)
      : base(context, attrs)
    {
        Init();
    }

    public StateContainer(Context context, IAttributeSet attrs, int defStyleAttr)
      : base(context, attrs, defStyleAttr)
    {
        Init();
    }

    public StateContainer(
        Context context,
        IAttributeSet attrs,
        int defStyleAttr,
        int defStyleRes)
        : base(context, attrs, defStyleAttr, defStyleRes)
    {
        Init();
    }

    public StateContainer(Context context)
        : base(context)
    {
        Init();
    }

    private void Init()
    {
        var changeStateCommand = new MvxCommand(OnStateChanged);

        _disposable = this
            .WhenAnyValue(c => c.State, c => c.States)
            .Where(c => c.Item2 != null)
            .InvokeCommand(changeStateCommand);
    }

    private void OnStateChanged()
    {
        if (States == null
            || State == _previousState
            || State == State.None
            || (!States.ContainsKey(State) && State != State.MinorLoading))
        {
            return;
        }

        if (_previousState == State.MinorLoading)
        {
            _indicator?.Then(RemoveView);
        }

        _previousState = State;
        
        if (State == State.MinorLoading)
        {
            _indicator ??= CreateProgressIndicator();
            using var layoutParams =
                new LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent)
                {
                    StartToStart = LayoutParams.ParentId,
                    EndToEnd = LayoutParams.ParentId,
                    TopToTop = LayoutParams.ParentId,
                    BottomToBottom = LayoutParams.ParentId,
                };
            
            AddView(_indicator, layoutParams);
            return;

            CircularProgressIndicator CreateProgressIndicator()
            {
                _indicator = new CircularProgressIndicator(Context)
                {
                    Indeterminate = true,
                    TrackColor = Color.Transparent
                };
                _indicator.SetIndicatorColor(ContextCompat.GetColor(Context, Resource.Color.lightPurple));

                return _indicator;
            }
        }

        var currentView = this.GetAllChildViews()?.FirstOrDefault();
        var newView = States[State]();

        if (ReferenceEquals(newView, currentView)
            || newView == null)
        {
            return;
        }

        currentView?.Then(RemoveView);

        Add(newView);
    }

    private void Add(View newView)
    {
        using var layoutParams =
            new LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent)
            {
                StartToStart = LayoutParams.ParentId,
                EndToEnd = LayoutParams.ParentId,
                TopToTop = LayoutParams.ParentId,
                BottomToBottom = LayoutParams.ParentId,
                VerticalBias = 0
            };
        
        AddView(newView, layoutParams);
    }

    public State State { get; set; }

    public IReadOnlyDictionary<State, Func<View>> States { get; set; }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _disposable?.Dispose();
            _disposable = null;
        }
        base.Dispose(disposing);
    }

    public event PropertyChangedEventHandler PropertyChanged;
}