﻿using System.Collections.ObjectModel;
using Eto.Forms;
using LiveChartsCore.Defaults;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView.Eto;
using ViewModelsSamples.Events.AddPointOnClick;

namespace EtoFormsSample.Events.AddPointOnClick;

public class View : Panel
{
    private readonly ObservableCollection<ObservablePoint> _data;

    /// <summary>
    /// Initializes a new instance of the <see cref="View"/> class.
    /// </summary>
    public View()
    {
        var viewModel = new ViewModel();

        _data = viewModel.Data;

        var cartesianChart = new CartesianChart
        {
            Series = viewModel.SeriesCollection,
        };

        cartesianChart.MouseDown += CartesianChart_Click;

        Content = cartesianChart;
    }

    private void CartesianChart_Click(object sender, MouseEventArgs e)
    {
        var chart = (CartesianChart)sender;
        var p = new LvcPointD(e.Location.X, e.Location.Y);

        // scales the UI coordinates to the corresponding data in the chart.
        var dataCoordinates = chart.ScalePixelsToData(p);

        // finally add the new point to the data in our chart.
        _data.Add(new ObservablePoint(dataCoordinates.X, dataCoordinates.Y));

        // You can also get all the points or visual elements in a given location.
        var points = chart.GetPointsAt(new LvcPoint(p.X, p.Y));
        var visuals = chart.GetVisualsAt(new LvcPoint(p.X, p.Y));
    }
}
