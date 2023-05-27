using System;
using System.Collections.Generic;
using FluentValidation.Results;

namespace Lct2023.Definitions.MvxIntercationResults;

public class ValidationInteractionResult : BaseInteractionResult
{
    public ValidationInteractionResult(Type validatedType, bool isSuccess = false)
        : base(isSuccess)
    {
        ValidatedType = validatedType;
    }

    public Type ValidatedType { get; }

    public IEnumerable<ValidationFailure> ValidationResults { get; set; }
}
