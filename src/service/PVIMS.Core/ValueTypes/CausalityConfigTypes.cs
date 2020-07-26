﻿using System.ComponentModel;

namespace PVIMS.Core.ValueTypes
{
    public enum CausalityConfigType
    {
        [Description("Both Scales")]
        BothScales = 1,
        [Description("Naranjo Scale")]
        NaranjoScale = 2,
        [Description("WHO Scale")]
        WHOScale = 3
    }
}
