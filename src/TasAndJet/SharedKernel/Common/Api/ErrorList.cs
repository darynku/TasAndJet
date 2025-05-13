﻿using System.Collections;

namespace SharedKernel.Common.Api;

public class ErrorList(IEnumerable<Error> errors) : IEnumerable<Error>
{
    private readonly List<Error> _errors = [..errors];

    public IEnumerator<Error> GetEnumerator()
    {
        return _errors.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public static implicit operator ErrorList(List<Error> errors)
        => new(errors);
    
    public static implicit operator ErrorList(Error error)
        => new([error]);
}