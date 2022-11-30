﻿// The MIT License(MIT)
//
// Copyright(c) 2021 Alberto Rodriguez Orozco & LiveCharts Contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using System.Collections.Generic;
using System;
using System.Linq;

namespace LiveChartsCore.Kernel;

internal struct ChartPointCleanupContext
{
    public int ToDeleteCnt { get; private set; }


    public void RemovePoint(ChartPoint point)
    {
        if (point.RemoveOnCompleted) // protect against deleting same point again
        {
            ToDeleteCnt--;
            point.RemoveOnCompleted = false;
        }
    }


    public static ChartPointCleanupContext For(HashSet<ChartPoint> points)
    {
        InvalidateAllPoints(points);
        return new ChartPointCleanupContext() { ToDeleteCnt = points.Count };
    }

    internal static void InvalidateAllPoints(HashSet<ChartPoint> points)
    {
        foreach (var point in points)
            point.RemoveOnCompleted = true;
    }

    internal static void RemoveInvalidPoints(HashSet<ChartPoint> points, IChartView chartView, Scaler primaryScale, Scaler secondaryScale, Action<ChartPoint, Scaler, Scaler> disposeAction)
    {
        // It would be nice to have System.Buffers installed to use rented buffer
        // Or we can probably use single cached buffer since all calculations are running on GUI thread
        // At least we don't allocate when there is nothing to remove
        // And allocate only as much as we need to remove
        var toDeletePoints = points.Where(p => p.RemoveOnCompleted).ToArray();
        foreach (var p in toDeletePoints)
        {
            if (p.Context.Chart != chartView) continue;
            disposeAction(p, primaryScale, secondaryScale);
            _ = points.Remove(p);
        }
    }

    internal static void RemoveInvalidPoints(HashSet<ChartPoint> points, IChartView chartView, PolarScaler scale, Action<ChartPoint, PolarScaler> disposeAction)
    {
        // It would be nice to have System.Buffers installed to use rented buffer
        // Or we can probably use single cached buffer since all calculations are running on GUI thread
        // At least we don't allocate when there is nothing to remove
        // And allocate only as much as we need to remove
        var toDeletePoints = points.Where(p => p.RemoveOnCompleted).ToArray();
        foreach (var p in toDeletePoints)
        {
            if (p.Context.Chart != chartView) continue;
            disposeAction(p, scale);
            _ = points.Remove(p);
        }
    }
}
