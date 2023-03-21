using MediatR;
using Microsoft.Extensions.Logging;
using OpenXmlPowerTools;
using PVIMS.API.Models;
using PVIMS.Core.Aggregates.DashboardAggregate;
using PVIMS.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PVIMS.API.Application.Queries.DashboardAggregate
{
    public class GenerateDashboardQueryHandler
        : IRequestHandler<GenerateDashboardQuery, List<ChartDto>>
    {
        private readonly IDashboardQueries _dashboardQueries;
        private readonly IRepositoryInt<Dashboard> _dashboardRepository;
        private readonly ILogger<GenerateDashboardQueryHandler> _logger;

        public GenerateDashboardQueryHandler(
            IDashboardQueries dashboardQueries,
            IRepositoryInt<Dashboard> dashboardRepository,
            ILogger<GenerateDashboardQueryHandler> logger)
        {
            _dashboardQueries = dashboardQueries ?? throw new ArgumentNullException(nameof(dashboardQueries));
            _dashboardRepository = dashboardRepository ?? throw new ArgumentNullException(nameof(dashboardRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<ChartDto>> Handle(GenerateDashboardQuery message, CancellationToken cancellationToken)
        {
            var dashboardFromRepo = await _dashboardRepository.GetAsync(d => d.Id == message.DashboardId, new string[] { 
                "Elements.SeriesElements",
                "Elements.VisualisationElements"
            });
            if (dashboardFromRepo == null)
            {
                throw new KeyNotFoundException($"Unable to locate dashboard {message.DashboardId}");
            }

            List<ChartDto> charts = new List<ChartDto>();

            foreach (var element in dashboardFromRepo.Elements)
            {
                foreach (var visualisation in element.VisualisationElements)
                {
                    if(visualisation.ChartTypeId == ChartType.Line.Id || visualisation.ChartTypeId == ChartType.Column.Id)
                    {
                        await PrepareAxisBasedChartAsync(element, visualisation, charts);
                    }
                    if (visualisation.ChartTypeId == ChartType.Pie.Id)
                    {
                        await PrepareNonAxisBasedChartAsync(element, visualisation, charts);
                    }
                }
            }

            return charts;
        }

        private async Task PrepareAxisBasedChartAsync(DashboardElement element, DashboardVisualisation visualisation, List<ChartDto> charts)
        {
            var chart = new ChartDto();

            chart.Chart = new ApexChart() { Type = ChartType.Map(visualisation.ChartTypeId), Height = 360 };
            chart.Colors = new string[] { "#33b2df", "#546E7A", "#d4526e", "#13d8aa", "#A5978B", "#2b908f", "#f9a3a4", "#90ee7e", "#f48024", "#69d2e7" };
            chart.DataLabels = new ApexDataLabel() { Enabled = false };
            chart.Legend = new ApexLegend() { HorizontalAlign = "left", OffsetX = 40 };
            chart.Title = new ApexTitle() { Align = "left", Text = element.Name };

            foreach (var series in element.SeriesElements.Where(ele => ele.ValueSQL != ""))
            {
                var stratResults = await _dashboardQueries.ExecuteValueBasedQuery(series.ValueSQL);
                chart.XAxis = new ApexXAxis() { Categories = stratResults.Select(res => res.Strat).ToArray() };
                chart.AxisSeries.Add(new ApexAxisSeries() { Name = series.Name, Data = stratResults.Select(res => res.StratValue).ToArray() });
            }
            charts.Add(chart);
        }

        private async Task PrepareNonAxisBasedChartAsync(DashboardElement element, DashboardVisualisation visualisation, List<ChartDto> charts)
        {
            var chart = new ChartDto();

            chart.Chart = new ApexChart() { Type = ChartType.Map(visualisation.ChartTypeId), Height = 360 };
            chart.Colors = new string[] { "#33b2df", "#546E7A", "#d4526e", "#13d8aa", "#A5978B", "#2b908f", "#f9a3a4", "#90ee7e", "#f48024", "#69d2e7" };
            chart.DataLabels = new ApexDataLabel() { Enabled = false };
            chart.Legend = new ApexLegend() { HorizontalAlign = "left", OffsetX = 40 };
            chart.Title = new ApexTitle() { Align = "left", Text = element.Name };

            foreach (var series in element.SeriesElements.Where(ele => ele.ValueSQL != ""))
            {
                var stratResults = await _dashboardQueries.ExecuteValueBasedQuery(series.ValueSQL);
                chart.Labels = stratResults.Select(res => res.Strat).ToArray();
                chart.NonAxisSeries = stratResults.Select(res => res.StratValue).ToArray();
            }
            charts.Add(chart);
        }
    }
}
