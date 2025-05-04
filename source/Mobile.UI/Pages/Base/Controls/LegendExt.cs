using Syncfusion.Maui.Toolkit.Charts;

namespace Mobile.UI.Pages.Base.Controls;

public class LegendExt : ChartLegend
{
    protected override double GetMaximumSizeCoefficient()
    {
        return 0.5;
    }
}