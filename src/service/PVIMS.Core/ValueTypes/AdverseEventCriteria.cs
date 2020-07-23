﻿using System.ComponentModel;

namespace PVIMS.Core.ValueTypes
{
    public enum AdverseEventCriteria
    {
        [Description("Report Source")]
        ReportSource = 1,
        [Description("MedDRA Terminology")]
        MedDRATerminology = 2
    }

    public enum AdverseEventStratifyCriteria
    {
        [Description("Age Group")]
        AgeGroup = 1,
        Facility = 2,
        Drug = 3,
        Cohort = 4
    }
}
