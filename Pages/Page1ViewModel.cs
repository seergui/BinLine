using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using System;
using System.ComponentModel;
using System.Windows.Media;

namespace BinLine.Pages
{
    class Page1ViewModel :INotifyPropertyChanged
    {
        public PlotModel Mymodel { get; set; }
        private double[] xline1;
        public double[] Xline1
        {
            get => xline1;
            set
            {
                xline1 = value;
                OnPropertyChanged(nameof(Xline1));
            }
        }
        private int datalength;
        public int Datelength
        {
            get => datalength;
            set
            {
                datalength = value;
                OnPropertyChanged(nameof(Datelength));
            }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string protertyname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(protertyname));
        }
        private double[] yline1;
        public double[] Yline1
        {
            get => yline1;
            set
            {
                yline1 = value;
                OnPropertyChanged(nameof(Yline1));
            }
        }
        public Page1ViewModel()
        {
            Mymodel = new PlotModel() { Title = "Spectral processing" };
            Mymodel.Axes.Add(new LinearAxis() { Title = "Spectral Wavelength", Position = AxisPosition.Bottom, IsAxisVisible = true });
            Mymodel.Axes.Add(new LinearAxis() { Title = "Spectral Intensity", Position = AxisPosition.Left, IsAxisVisible = true });
            Mymodel.Series.Add(new FunctionSeries((x) => { return Math.Sin(x); }, -10, 10, 0.01));
            Mymodel.Legends.Add(new Legend
            {
                LegendPlacement = LegendPlacement.Inside,
                LegendPosition = LegendPosition.RightTop,
                LegendOrientation = LegendOrientation.Horizontal,
                LegendBorderThickness = 10,
                LegendTextColor = OxyColors.LightGray
            });
            Xline1 = new double[Datelength];
            Yline1 = new double[Datelength];
        }
        public void Plot(double[] x,double[] y ) 
        {
            LineSeries line1 = new();
            line1.InterpolationAlgorithm = InterpolationAlgorithms.CanonicalSpline;
            for (int i = 0; i < x.Length; i++)
            {
                line1.Points.Add(new DataPoint(x[i], y[i]));
            }
            Mymodel.Series.Clear();
            Mymodel.Series.Add(line1);
            Mymodel.InvalidatePlot(true);
            Xline1 = x;
            Yline1 = y;
        }
        public void Draw(double[] x, double[] y, double lambda)
        {
            LineSeries line1 = new();
            line1.Title = "arPLS Parameter:" + lambda;
            line1.InterpolationAlgorithm = InterpolationAlgorithms.CanonicalSpline;
            for (int i = 0; i < x.Length; i++)
            {
                line1.Points.Add(new DataPoint(x[i], y[i]));
            }
            //Mymodel.Series.Clear();
            Mymodel.Series.Add(line1);
            Mymodel.InvalidatePlot(true);
            Xline1 = x;
            Yline1 = y;
        }
        public void Plot1(double[] x, double[] y,double[] z, double[] d)
        {
            LineSeries line1 = new();
            line1.InterpolationAlgorithm = InterpolationAlgorithms.CanonicalSpline;
            //line1.Color = OxyColors.Blue;
            LineSeries line2 = new();
            line2.InterpolationAlgorithm = InterpolationAlgorithms.CanonicalSpline;
            line2.Color = OxyColors.Red;
            LineSeries line3 = new();
            line3.InterpolationAlgorithm = InterpolationAlgorithms.CanonicalSpline;
            line3.Color = OxyColors.Blue;
            for (int i = 0; i < x.Length; i++)
            {
                line1.Points.Add(new DataPoint(x[i], y[i]));
                line2.Points.Add(new DataPoint(x[i], z[i]));
                line3.Points.Add(new DataPoint(x[i], d[i]));
            }
            Mymodel.Series.Clear();
            Mymodel.Series.Add(line1);
            Mymodel.Series.Add(line2);
            Mymodel.Series.Add(line3);
            Mymodel.InvalidatePlot(true);
            Xline1 = x;
            Yline1 = z;
        }
    }
}
