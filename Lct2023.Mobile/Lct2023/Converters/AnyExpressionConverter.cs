using System;
using System.Globalization;
using MvvmCross.Converters;

namespace Lct2023.Converters;

public class AnyExpressionConverter<TBindingType, TResult> : MvxValueConverter<TBindingType>
{
    private readonly Func<TBindingType, TResult> _expr;

    public AnyExpressionConverter(Func<TBindingType, TResult> expr)
    {
        _expr = expr;
    }

    protected override object Convert(TBindingType value, Type targetType, object parameter, CultureInfo culture) =>
        _expr.Invoke(value);
}
