﻿namespace PVIMS.Core.Entities.Keyless
{
    public class ContingencyAnalysisItem
    {
        public string Drug { get; set; }
        public int Medication_Id { get; set; }
        public int ExposedCases { get; set; }
        public int ExposedNonCases { get; set; }
        public decimal ExposedPopulation { get; set; }
        public decimal ExposedIncidenceRate { get; set; }
        public int NonExposedCases { get; set; }
        public int NonExposedNonCases { get; set; }
        public decimal NonExposedPopulation { get; set; }
        public decimal NonExposedIncidenceRate { get; set; }
        public decimal UnadjustedRelativeRisk { get; set; }
        public decimal AdjustedRelativeRisk { get; set; }
        public decimal ConfidenceIntervalLow { get; set; }
        public decimal ConfidenceIntervalHigh { get; set; }
    }
}
