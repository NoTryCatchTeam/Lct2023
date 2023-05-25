using System;
using System.Globalization;
using MvvmCross.Converters;

namespace Lct2023.Converters;

public class AnyExpressionConverter<TBindingType, TResult> : MvxValueConverter<TBindingType, TResult>
{
    private readonly Func<TBindingType, TResult> _convert;
    private readonly Func<TResult, TBindingType> _convertBack;

    public AnyExpressionConverter(Func<TBindingType, TResult> convert, Func<TResult, TBindingType> convertBack = null)
    {
        _convert = convert;
        _convertBack = convertBack;
    }

    protected override TResult Convert(TBindingType value, Type targetType, object parameter, CultureInfo culture) =>
        _convert.Invoke(value);

    protected override TBindingType ConvertBack(TResult value, Type targetType, object parameter, CultureInfo culture)
    {
        return _convertBack != null ? _convertBack(value) : base.ConvertBack(value, targetType, parameter, culture);
    }
}
