using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PVIMS.API.Models
{
    /// <summary>
    /// A representation of a change in activity status
    /// </summary>
    [DataContract()]
    public class ChartDto
    {
        /// <summary>
        /// Axis or non axis based series data
        /// </summary>
        [DataMember]
        public List<ApexAxisSeries> AxisSeries { get; set; } = new List<ApexAxisSeries>();

        /// <summary>
        /// Axis or non axis based series data
        /// </summary>
        [DataMember]
        public int[] NonAxisSeries { get; set; }

        /// <summary>
        /// Chart configurations
        /// </summary>
        [DataMember]
        public ApexChart Chart { get; set; }

        /// <summary>
        /// The array of hex-based colours to be used when rendering charts
        /// </summary>
        [DataMember]
        public string[] Colors { get; set; }

        /// <summary>
        /// Data label configurations
        /// </summary>
        [DataMember]
        public ApexDataLabel DataLabels { get; set; }

        /// <summary>
        /// X-Axis configuration
        /// </summary>
        [DataMember]
        public ApexXAxis XAxis { get; set; }

        /// <summary>
        /// Legend configuration
        /// </summary>
        [DataMember]
        public ApexLegend Legend { get; set; }

        /// <summary>
        /// The array of labels for non axis based series
        /// </summary>
        [DataMember]
        public string[] Labels { get; set; }

        /// <summary>
        /// Title configuration
        /// </summary>
        [DataMember]
        public ApexTitle Title { get; set; }
    }

    [DataContract()]
    public class ApexAxisSeries
    {
        /// <summary>
        /// Name of the series
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// The values for the series
        /// </summary>
        [DataMember]
        public int[] Data { get; set; }
    }

    [DataContract()]
    public class ApexChart
    {
        /// <summary>
        /// Type of chart
        /// </summary>
        [DataMember]
        public string Type { get; set; }

        /// <summary>
        /// The height of the chart
        /// </summary>
        [DataMember]
        public int Height { get; set; }
    }

    [DataContract()]
    public class ApexDataLabel
    {
        /// <summary>
        /// Should data labels be enabled
        /// </summary>
        [DataMember]
        public bool Enabled { get; set; }
    }

    [DataContract()]
    public class ApexXAxis
    {
        /// <summary>
        /// Categories to be listed as part of X-Axis
        /// </summary>
        [DataMember]
        public string[] Categories { get; set; }
    }

    [DataContract()]
    public class ApexLegend
    {
        /// <summary>
        /// Horizontal alignment of the legend
        /// </summary>
        [DataMember]
        public string HorizontalAlign { get; set; }

        /// <summary>
        /// Offset 
        /// </summary>
        [DataMember]
        public int OffsetX { get; set; }
    }

    [DataContract()]
    public class ApexTitle
    {
        /// <summary>
        /// Title of the chart
        /// </summary>
        [DataMember]
        public string Text { get; set; }

        /// <summary>
        /// Offset 
        /// </summary>
        [DataMember]
        public string Align { get; set; }

    }
}
