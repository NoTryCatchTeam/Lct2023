using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using Android.Content;
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
            _indicator ??= new CircularProgressIndicator(Context)
            {
                Indeterminate = true,
                TrackColor = ContextCompat.GetColor(Context, Resource.Color.lightPurple)
            };
            Add(_indicator);
            return;
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
        AddView(newView);

        using var constraintSet = new ConstraintSet();
        constraintSet.Clone(this);

        var newViewId = newView.Id;
        constraintSet.Connect(newViewId, ConstraintSet.Top, ConstraintSet.ParentId, ConstraintSet.Top);
        constraintSet.Connect(newViewId, ConstraintSet.Start, ConstraintSet.ParentId, ConstraintSet.Start);
        constraintSet.Connect(newViewId, ConstraintSet.End, ConstraintSet.ParentId, ConstraintSet.End);
        constraintSet.Connect(newViewId, ConstraintSet.Bottom, ConstraintSet.ParentId, ConstraintSet.Bottom);
        constraintSet.ApplyTo(this);
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