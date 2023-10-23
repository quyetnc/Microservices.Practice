﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.SeedWork;

public class ApiErrorResult<T> : ApiResult<T>
{
    public ApiErrorResult ( string message ) : base(false, message)
    {

    }
    public ApiErrorResult ( ) : this("Something wrong happened. Please try again later")
    {

    }

    public ApiErrorResult ( List<string> errors ) : base(false)
    {
        Errors = errors;
    }

    public List<string> Errors { get; set; }
}
