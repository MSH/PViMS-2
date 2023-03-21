using PVIMS.Core.Exceptions;
using PVIMS.Core.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PVIMS.Core.Aggregates.DashboardAggregate
{
    public class ChartType
        : Enumeration
    {
        public static ChartType Line = new ChartType(1, "Line");
        public static ChartType Area = new ChartType(2, "Area");
        public static ChartType Column = new ChartType(3, "Column");
        public static ChartType StackedColumn = new ChartType(4, "StackedColumn");
        public static ChartType HorizontalBar = new ChartType(5, "HorizontalBar");
        public static ChartType VerticalBar = new ChartType(6, "VerticalBar");
        public static ChartType StackedBar = new ChartType(7, "StackedBar");
        public static ChartType RangeBar = new ChartType(8, "RangeBar");
        public static ChartType CandleStick = new ChartType(9, "CandleStick");
        public static ChartType Pie = new ChartType(10, "Pie");
        public static ChartType Radar = new ChartType(11, "Radar");
        public static ChartType Bubble = new ChartType(12, "Bubble");
        public static ChartType Scatter = new ChartType(13, "Scatter");
        public static ChartType HeatMap = new ChartType(14, "HeatMap");

        public ChartType(int id, string name)
            : base(id, name)
        {
        }

        public static IEnumerable<ChartType> List() =>
            new[] { Line, Area, Column, StackedColumn, HorizontalBar, VerticalBar, StackedBar, RangeBar, CandleStick, Pie, Radar, Bubble, Scatter, HeatMap };

        public static ChartType FromName(string name)
        {
            var state = List()
                .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

            if (state == null)
            {
                throw new DomainException($"Possible values for ChartType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static ChartType From(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new DomainException($"Possible values for ChartType: {String.Join(",", List().Select(s => s.Name))}");
            }

            return state;
        }

        public static string Map(int id)
        {
            var state = List().SingleOrDefault(s => s.Id == id);

            if (state == null)
            {
                throw new DomainException($"Possible values for ChartType: {String.Join(",", List().Select(s => s.Name))}");
            }

            switch (id)
            {
                case 1:
                    return "line";

                case 3:
                    return "bar";

                case 10:
                    return "pie";

                default:
                    break;
            }

            return "";
        }
    }
}