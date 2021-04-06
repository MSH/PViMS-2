﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PVIMS.Core.CustomAttributes
{
    public class CustomAttributeValidationException : Exception
    {
        public CustomAttributeValidationException(string format, params object[] args)
            :base(string.Format(format, args))
        { 
        }
    }
}
