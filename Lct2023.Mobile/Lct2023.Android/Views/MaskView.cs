using System;
using System.Linq;
using Android.Animation;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Views.Animations;

namespace Lct2023.Android.Views;

public class MaskView : View, ValueAnimator.IAnimatorUpdateListener
{
    private Paint _transparentPaint;
    private Paint _backgroundPaint;
    private float[] _radiuses;
    private Path _path;
    private Rect _maskedRect;

    protected MaskView(IntPtr javaReference, JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
        Init();
    }

    public MaskView(Context context)
        : base(context)
    {
        Init();
    }

    public MaskView(Context context, IAttributeSet attrs)
        : base(context, attrs)
    {
        Init();
    }

    public void SetInitialRect(Rect rect)
    {
        _maskedRect = rect;
        PostInvalidate();
    }

    public void AnimateToRect(Rect newRect, long duration)
    {
        var animator = ValueAnimator.OfObject(new RectEvaluator(), _maskedRect, newRect);
        animator.SetDuration(duration);
        animator.SetInterpolator(new LinearInterpolator());
        animator.AddUpdateListener(this);

        animator.Start();
    }

    public override void Draw(Canvas canvas)
    {
        base.Draw(canvas);

        var newRect = new RectF(_maskedRect);

        _path.Reset();

        _path.AddRoundRect(newRect, _radiuses, Path.Direction.Cw);
        _path.SetFillType(Path.FillType.InverseEvenOdd);

        canvas.DrawRoundRect(newRect, _radiuses[0], _radiuses[1], _transparentPaint);

        canvas.DrawPath(_path, _backgroundPaint);
        canvas.ClipPath(_path);
        canvas.DrawColor(Color.ParseColor("#99000000"));
    }

    private void Init()
    {
        _transparentPaint = new Paint();
        _transparentPaint.SetColor(Color.Transparent);
        _transparentPaint.StrokeWidth = 10;

        _backgroundPaint = new Paint();
        _backgroundPaint.SetColor(Color.Transparent);
        _backgroundPaint.StrokeWidth = 10;

        _radiuses = Enumerable.Repeat(16f, 8).ToArray();

        _path = new Path();
        _maskedRect = new Rect();
    }

    public void OnAnimationUpdate(ValueAnimator animator)
    {
        _maskedRect = (Rect)animator.AnimatedValue;

        Invalidate();
    }
}
