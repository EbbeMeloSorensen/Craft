using System.Windows;
using Craft.Logging;
using Craft.Utils;

namespace Craft.ViewModels.Geometry2D.ScrollFree
{
    public class TimeSeriesViewModel : CoordinateSystemViewModel
    {
        private static TimeSpan _timeSpanOfOneDay;

        public static DateTime TimeAtOrigo { get; }

        static TimeSeriesViewModel()
        {
            // Dette er en konvention, vi opererer med
            TimeAtOrigo = DateTime.Now.Date;

            // Denne skal vi bruge igen og igen - den repræsenterer det time span, der svarer til enheded i koordinatsystemet
            // hvilket vi pr konvention sætter til 1 dag
            _timeSpanOfOneDay = TimeSpan.FromDays(1.0);
        }

        // Omregning af tidspunkt til x-værdi i koordinatsystemet
        public static double ConvertDateTimeToXValue(DateTime dateTime)
        {
            return (dateTime - TimeAtOrigo) / _timeSpanOfOneDay;
        }

        // Omregning af x-værdi i koordinatsystemet til tidspunkt 
        public static DateTime ConvertXValueToDateTime(double xValue)
        {
            return TimeAtOrigo + xValue * (_timeSpanOfOneDay);
        }

        private ILogger _logger;

        public ObservableObject<DateTime?> TimeAtMousePosition { get; }

        public TimeSeriesViewModel(
            Point worldWindowFocus,
            Size worldWindowSize,
            bool fitAspectRatio,
            double marginX,
            double marginY,
            double expansionFactor,
            XAxisMode xAxisMode,
            ILogger logger) : base(worldWindowFocus, worldWindowSize, fitAspectRatio, marginX, marginY, expansionFactor, xAxisMode)
        {
            _logger = logger;

            TimeAtMousePosition = new ObservableObject<DateTime?>();

            GeometryEditorViewModel.MousePositionWorld.PropertyChanged += (s, e) =>
            {
                TimeAtMousePosition.Object = GeometryEditorViewModel.MousePositionWorld.Object.HasValue
                    ? TimeAtOrigo + TimeSpan.FromDays(GeometryEditorViewModel.MousePositionWorld.Object.Value.X)
                    : null;
            };
        }

        protected override void DrawVerticalGridLinesAndOrLabels()
        {
            XAxisTickLabelViewModels.Clear();

            var x0 = GeometryEditorViewModel.WorldWindowUpperLeft.X;
            var x1 = GeometryEditorViewModel.WorldWindowUpperLeft.X + GeometryEditorViewModel.WorldWindowSize.Width;
            var y0 = -GeometryEditorViewModel.WorldWindowUpperLeft.Y - GeometryEditorViewModel.WorldWindowSize.Height;

            var dy = MarginBottom / GeometryEditorViewModel.Scaling.Height;

            var lineSpacingX_ViewPort_Min = 75.0;
            var lineSpacingX_World_Min = lineSpacingX_ViewPort_Min / GeometryEditorViewModel.Scaling.Width;

            var lineSpacingX_World = 1.0;
            var delta = TimeSpan.FromDays(1);
            var showSeconds = false;
            var showMinutes = false;
            var showHours = false;
            var modulo = 1;
            var maxDayLabel = 29;
            var x0ext = x0 - (x1 - x0);
            var x1ext = x1 + (x1 - x0);

            if (lineSpacingX_World_Min < 1.0 / 24.0 / 60.0 / 60.0)
            {
                // Operer med en linie pr sekund
                lineSpacingX_World = 1.0 / 24.0 / 60.0 / 60.0;
                delta = TimeSpan.FromSeconds(1);
                showSeconds = true;
            }
            else if (lineSpacingX_World_Min < 1.0 / 24.0 / 60.0 / 30.0)
            {
                // Operer med en linie pr 2 sekunder
                lineSpacingX_World = 1.0 / 24.0 / 60.0 / 30.0;
                delta = TimeSpan.FromSeconds(2);
                showSeconds = true;
            }
            else if (lineSpacingX_World_Min < 1.0 / 24.0 / 60.0 / 12.0)
            {
                // Operer med en linie pr 5 sekunder
                lineSpacingX_World = 1.0 / 24.0 / 60.0 / 12.0;
                delta = TimeSpan.FromSeconds(5);
                showSeconds = true;
            }
            else if (lineSpacingX_World_Min < 1.0 / 24.0 / 60.0 / 6.0)
            {
                // Operer med en linie pr 10 sekunder
                lineSpacingX_World = 1.0 / 24.0 / 60.0 / 6.0;
                delta = TimeSpan.FromSeconds(10);
                showSeconds = true;
            }
            else if (lineSpacingX_World_Min < 1.0 / 24.0 / 60.0 / 4.0)
            {
                // Operer med en linie pr 15 sekunder
                lineSpacingX_World = 1.0 / 24.0 / 60.0 / 4.0;
                delta = TimeSpan.FromSeconds(15);
                showSeconds = true;
            }
            else if (lineSpacingX_World_Min < 1.0 / 24.0 / 60.0 / 3.0)
            {
                // Operer med en linie pr 20 sekunder
                lineSpacingX_World = 1.0 / 24.0 / 60.0 / 3.0;
                delta = TimeSpan.FromSeconds(20);
                showSeconds = true;
            }
            else if (lineSpacingX_World_Min < 1.0 / 24.0 / 60.0 / 2.0)
            {
                // Operer med en linie pr 30 sekunder
                lineSpacingX_World = 1.0 / 24.0 / 60.0 / 2.0;
                delta = TimeSpan.FromSeconds(30);
                showSeconds = true;
            }
            else if (lineSpacingX_World_Min < 1.0 / 24.0 / 60.0)
            {
                // Operer med en linie pr minut
                lineSpacingX_World = 1.0 / 24.0 / 60.0;
                delta = TimeSpan.FromMinutes(1);
                showMinutes = true;
            }
            else if (lineSpacingX_World_Min < 1.0 / 24.0 / 30.0)
            {
                // Operer med en linie pr 2 minutter
                lineSpacingX_World = 1.0 / 24.0 / 30.0;
                delta = TimeSpan.FromMinutes(2);
                showMinutes = true;
            }
            else if (lineSpacingX_World_Min < 1.0 / 24.0 / 12.0)
            {
                // Operer med en linie pr 5 minutter
                lineSpacingX_World = 1.0 / 24.0 / 12.0;
                delta = TimeSpan.FromMinutes(5);
                showMinutes = true;
            }
            else if (lineSpacingX_World_Min < 1.0 / 24.0 / 6.0)
            {
                // Operer med en linie pr 10 minutter
                lineSpacingX_World = 1.0 / 24.0 / 6.0;
                delta = TimeSpan.FromMinutes(10);
                showMinutes = true;
            }
            else if (lineSpacingX_World_Min < 1.0 / 24.0 / 4.0)
            {
                // Operer med en linie pr 15 minutter
                lineSpacingX_World = 1.0 / 24.0 / 4.0;
                delta = TimeSpan.FromMinutes(15);
                showMinutes = true;
            }
            else if (lineSpacingX_World_Min < 1.0 / 24.0 / 2.0)
            {
                // Operer med en linie pr 30 minutter
                lineSpacingX_World = 1.0 / 24.0 / 2.0;
                delta = TimeSpan.FromMinutes(30);
                showMinutes = true;
            }
            else if (lineSpacingX_World_Min < 1.0 / 24.0)
            {
                // Operer med en linie pr time
                lineSpacingX_World = 1.0 / 24.0;
                delta = TimeSpan.FromHours(1);
                showHours = true;
            }
            else if (lineSpacingX_World_Min < 1.0 / 12.0)
            {
                // Operer med en linie pr 2 timer
                lineSpacingX_World = 1.0 / 12.0;
                delta = TimeSpan.FromHours(2);
                showHours = true;
            }
            else if (lineSpacingX_World_Min < 1.0 / 8.0)
            {
                // Operer med en linie pr 3 timer
                lineSpacingX_World = 1.0 / 8.0;
                delta = TimeSpan.FromHours(3);
                showHours = true;
            }
            else if (lineSpacingX_World_Min < 1.0 / 6.0)
            {
                // Operer med en linie pr 4 timer
                lineSpacingX_World = 1.0 / 6.0;
                delta = TimeSpan.FromHours(4);
                showHours = true;
            }
            else if (lineSpacingX_World_Min < 1.0 / 4.0)
            {
                // Operer med en linie pr 6 timer
                lineSpacingX_World = 1.0 / 4.0;
                delta = TimeSpan.FromHours(6);
                showHours = true;
            }
            else if (lineSpacingX_World_Min < 1.0 / 2.0)
            {
                // Operer med en linie pr 12 timer
                lineSpacingX_World = 1.0 / 2.0;
                delta = TimeSpan.FromHours(12);
                showHours = true;
            }
            else if (lineSpacingX_World_Min < 1.0)
            {
                // Operer med en linie pr dag
                lineSpacingX_World = 1.0;
                modulo = 1;
                maxDayLabel = 31;
            }
            else if (lineSpacingX_World_Min < 1.0 * 2.0)
            {
                // Operer med en linie pr 2 dage
                lineSpacingX_World = 1.0 * 2.0;
                modulo = 2;
            }
            else if (lineSpacingX_World_Min < 1.0 * 5.0)
            {
                // Operer med en linie pr 5 dage
                lineSpacingX_World = 1.0 * 5.0;
                modulo = 5;
            }
            else if (lineSpacingX_World_Min < 1.0 * 10.0)
            {
                // Operer med en linie pr 10 dage
                lineSpacingX_World = 1.0 * 10.0;
                modulo = 10;
            }
            else if (lineSpacingX_World_Min < 1.0 * 15.0)
            {
                // Operer med en linie pr 15 dage
                lineSpacingX_World = 1.0 * 15.0;
                modulo = 15;
            }
            else if (lineSpacingX_World_Min < 1.0 * 365.25 / 12)
            {
                // Operer med en linie pr måned
                lineSpacingX_World = 1.0 * 365.25 / 12;
                modulo = 1;
            }
            else if (lineSpacingX_World_Min < 365.25 / 6)
            {
                // Operer med en linie pr 2 måneder
                lineSpacingX_World = 365.25 / 6;
                modulo = 2;
            }
            else if (lineSpacingX_World_Min < 365.25 / 4)
            {
                // Operer med en linie pr 3 måneder
                lineSpacingX_World = 365.25 / 4;
                modulo = 3;
            }
            else if (lineSpacingX_World_Min < 365.25 / 3)
            {
                // Operer med en linie pr 4 måneder
                lineSpacingX_World = 365.25 / 3;
                modulo = 4;
            }
            else if (lineSpacingX_World_Min < 365.25 / 2)
            {
                // Operer med en linie pr 6 måneder
                lineSpacingX_World = 365.25 / 2;
                modulo = 6;
            }
            else if (lineSpacingX_World_Min < 365.25)
            {
                // Operer med en linie pr år
                lineSpacingX_World = 365.25;
                modulo = 1;
            }
            else if (lineSpacingX_World_Min < 365.25 * 2)
            {
                // Operer med en linie pr 2 år
                lineSpacingX_World = 365.25 * 2;
                modulo = 2;
            }
            else if (lineSpacingX_World_Min < 365.25 * 5)
            {
                // Operer med en linie pr 5 år
                lineSpacingX_World = 365.25 * 5;
                modulo = 5;
            }
            else if (lineSpacingX_World_Min < 365.25 * 10)
            {
                // Operer med en linie pr 10 år
                lineSpacingX_World = 365.25 * 10;
                modulo = 10;
            }
            else if (lineSpacingX_World_Min < 365.25 * 20)
            {
                // Operer med en linie pr 20 år
                lineSpacingX_World = 365.25 * 20;
                modulo = 20;
            }
            else if (lineSpacingX_World_Min < 365.25 * 50)
            {
                // Operer med en linie pr 50 år
                lineSpacingX_World = 365.25 * 50;
                modulo = 50;
            }
            else if (lineSpacingX_World_Min < 365.25 * 100)
            {
                // Operer med en linie pr 100 år
                lineSpacingX_World = 365.25 * 100;
                modulo = 100;
            }

            var labelWidth = lineSpacingX_World * GeometryEditorViewModel.Scaling.Width;
            var labelHeight = 20.0;

            // Find den største værdi for x, der ligger til venstre for World Window, og som repræsenterer en grid line
            var x = System.Math.Floor(x0ext / lineSpacingX_World) * lineSpacingX_World;

            var labelCount = 0;

            if (lineSpacingX_World > 365.24)
            {
                // Her opererer vi med at grid lines og labels knytter sig til ÅR

                var timeCorrespondingToX = TimeAtOrigo + TimeSpan.FromDays(x);
                var t = new DateTime(timeCorrespondingToX.Year, 1, 1);
                var t1 = TimeAtOrigo + TimeSpan.FromDays(x1ext);

                while (t < t1)
                {
                    x = (t - TimeAtOrigo) / TimeSpan.FromDays(1);

                    var year = t.Date.Year;

                    if (year % modulo == 0)
                    {
                        var label = $"{year}";
                        var point = new PointD(x, y0 + dy);

                        var labelViewModel = new LabelViewModel
                        {
                            Text = label,
                            Point = new PointD(point.X, GeometryEditorViewModel._yAxisFactor * point.Y),
                            Width = labelWidth,
                            Height = labelHeight,
                            Shift = new PointD(0, labelHeight / 2),
                            Opacity = 0.0,
                            FixedViewPortXCoordinate = null,
                            FixedViewPortYCoordinate = MarginBottomOffset
                        };

                        XAxisTickLabelViewModels.Add(labelViewModel);
                    }

                    labelCount++;

                    t = t.AddYears(1);
                }
            }
            else if (lineSpacingX_World > 30.0)
            {
                // Her opererer vi med at grid lines og labels knytter sig til MÅNEDER

                // Find det tidspunkt, der svarer til seneste ÅRSSKIFTE i forhold til den grid line, der kandiderer til at blive den første
                // (vi vil sikre, at der altid er en gridline for årsskifte og derudover for "legitime" måneder i året, såsom 1, 3, 5 osv)
                var timeCorrespondingToX = TimeAtOrigo + TimeSpan.FromDays(x);
                var t = new DateTime(timeCorrespondingToX.Year, 1, 1);
                var t1 = TimeAtOrigo + TimeSpan.FromDays(x1ext);

                while (t < t1)
                {
                    x = (t - TimeAtOrigo) / TimeSpan.FromDays(1);

                    var year = t.Date.Year;
                    var month = t.Date.Month;

                    var label = $"{month}/{year}";

                    if (month == 1 || (month - 1) % modulo == 0)
                    {
                        var point = new PointD(x, y0 + dy);

                        var labelViewModel = new LabelViewModel
                        {
                            Text = label,
                            Point = new PointD(point.X, GeometryEditorViewModel._yAxisFactor * point.Y),
                            Width = labelWidth,
                            Height = labelHeight,
                            Shift = new PointD(0, labelHeight / 2),
                            Opacity = 0.0,
                            FixedViewPortXCoordinate = null,
                            FixedViewPortYCoordinate = MarginBottomOffset
                        };

                        XAxisTickLabelViewModels.Add(labelViewModel);

                        labelCount++;
                    }

                    t = t.AddMonths(1);
                }
            }
            else if (lineSpacingX_World > 0.99)
            {
                // Her opererer vi med at grid lines og labels knytter sig til DAGE

                // Find det tidspunkt, der svarer til seneste MÅNEDSSKIFTE i forhold til den grid line, der kandiderer til at blive den første
                // (vi vil sikre, at der altid er en gridline for månedsskifte og derudover for "legitime" dage i måneden, såsom 1, 6, 11, 16 osv)
                var timeCorrespondingToX = TimeAtOrigo + TimeSpan.FromDays(x);
                var t = new DateTime(timeCorrespondingToX.Year, timeCorrespondingToX.Month, 1);
                var t1 = TimeAtOrigo + TimeSpan.FromDays(x1ext);

                while (t < t1)
                {
                    x = (t - TimeAtOrigo) / TimeSpan.FromDays(1);

                    var month = t.Date.Month;
                    var day = t.Date.Day;

                    var label = $"{day}/{month}";

                    if (day == 1 || (day - 1 ) % modulo == 0 && day <= maxDayLabel)
                    {
                        var point = new PointD(x, y0 + dy);

                        var labelViewModel = new LabelViewModel
                        {
                            Text = label,
                            Point = new PointD(point.X, GeometryEditorViewModel._yAxisFactor * point.Y),
                            Width = labelWidth,
                            Height = labelHeight,
                            Shift = new PointD(0, labelHeight / 2),
                            Opacity = 0.0,
                            FixedViewPortXCoordinate = null,
                            FixedViewPortYCoordinate = MarginBottomOffset
                        };

                        XAxisTickLabelViewModels.Add(labelViewModel);

                        labelCount++;
                    }

                    t = t.AddDays(1);
                }
            }
            else
            {
                // Her opererer vi med at grid lines og labels knytter sig til TIMER eller mindre enheder

                // Bemærk: x kan godt være negativ her - det afspejler bare, at World Window går hen over origo
                var t = TimeAtOrigo + RoundSeconds(TimeSpan.FromDays(x));

                while (x < x1ext)
                {
                    var hour = t.Hour;
                    var minute = t.Minute;
                    var second = t.Second;

                    var label = "x";

                    if (showSeconds)
                    {
                        label = $"{hour}:{minute.ToString().PadLeft(2, '0')}:{second.ToString().PadLeft(2, '0')}";
                    }
                    else if (showMinutes)
                    {
                        label = $"{hour}:{minute.ToString().PadLeft(2, '0')}";
                    }
                    else if (showHours)
                    {
                        label = $"{hour}:00";
                    }

                    if (ShowVerticalGridLines)
                    {
                        GeometryEditorViewModel.AddLine(
                            new PointD(x, _expandedWorldWindowUpperLeft.Y),
                            new PointD(x, _expandedWorldWindowUpperLeft.Y + _expandedWorldWindowSize.Height),
                            1,
                            _gridBrush);
                    }

                    if (ShowXAxisLabels)
                    {
                        var point = new PointD(x, y0 + dy);

                        var labelViewModel = new LabelViewModel
                        {
                            Text = label,
                            Point = new PointD(point.X, GeometryEditorViewModel._yAxisFactor * point.Y),
                            Width = labelWidth,
                            Height = labelHeight,
                            Shift = new PointD(0, labelHeight / 2),
                            Opacity = 0.0,
                            FixedViewPortXCoordinate = null,
                            FixedViewPortYCoordinate = MarginBottomOffset
                        };

                        XAxisTickLabelViewModels.Add(labelViewModel);

                        labelCount++;
                    }

                    x += lineSpacingX_World;
                    t += delta;
                }
            }

            //_logger?.WriteLine(LogMessageCategory.Information, $"TimeSeriesViewModel: Added {labelCount} labels");

            GenerateOverallDateLabels(x0, x1);
        }

        private void GenerateOverallDateLabels(
            double x0,
            double x1)
        {
            var t0 = TimeAtOrigo + TimeSpan.FromDays(x0);
            var t1 = TimeAtOrigo + TimeSpan.FromDays(x1);

            if (t0.Date == t1.Date)
            {
                // Same day
                //XAxisOverallLabel1 = t0.ToString("yyyy MMMM dd");
                XAxisOverallLabel1 = t0.ToString("D");
                XAxisOverallLabel2 = "";
                XAxisOverallLabel1Alignment = "Center";
            }
            else
            {
                XAxisOverallLabel1 = t0.ToString("D");
                XAxisOverallLabel2 = t1.ToString("D");
                XAxisOverallLabel1Alignment = "Left";
                XAxisOverallLabel2Alignment = "Right";
            }
            /*
            else if (t0.Year == t1.Year && t0.Month == t1.Month)
            {
                // Same month
                var monthName = t0.ToString("MMMM", CultureInfo.InvariantCulture);
                XAxisOverallLabel1 = $"{monthName} {t0.Year}";
                XAxisOverallLabel2 = "";
                XAxisOverallLabel1Alignment = "Center";
            }
            else if (t0.Year == t1.Year)
            {
                // Same year
                XAxisOverallLabel1 = $"{t0.Year}";
            }
            else
            {
                XAxisOverallLabel1 = "";
            }
            */
        }

        // Helper for rounding Timespan to closest timespan where second is zero
        // Det er for at kompensere for afrundingsfejl i forbindelse med konvertering fra x-værdier på x-aksen og tidspunkter
        private static TimeSpan RoundSeconds(TimeSpan span)
        {
            return TimeSpan.FromSeconds(System.Math.Round(span.TotalSeconds));
        }
    }
}
