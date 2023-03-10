using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Factorization;
using Microsoft.Win32;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Org.BouncyCastle.Crypto.Encodings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Navigation;

namespace BinLine
{
    internal class MainViewModel : INotifyPropertyChanged
    {
        public Pages.Page1ViewModel page1 = new Pages.Page1ViewModel();
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private string filename;
        public string Filename
        {
            get => filename;
            set
            {
                filename = value;
                OnPropertyChanged(nameof(Filename));
            }
        }
        private double processtime;
        public double Processtime
        {
            get => processtime;
            set
            {
                processtime = value;
                OnPropertyChanged(nameof(Processtime));
            }
        }
        private double rmse;
        public double RMSE
        {
            get => rmse;
            set
            {
                rmse = value;
                OnPropertyChanged(nameof(RMSE));
            }
        }
        private string s;
        public string S
        {
            get => s;
            set
            {
                s = value;
                OnPropertyChanged(nameof(S));
            }
        }
        private double[] xdada;
        public double[] Xdata
        {
            get => xdada;
            set
            {
                xdada = value;
                OnPropertyChanged(nameof(Xdata));
            }
        }
        private double[] ydada;
        public double[] Ydata
        {
            get => ydada;
            set
            {
                ydada = value;
                OnPropertyChanged(nameof(Ydata));
            }
        }
        public List<double> Lambda { get; set; }
        private double lambdaSelected = 5;
        public double LambdaSelected
        {
            get
            {
                return lambdaSelected;
            }
            set
            {
                lambdaSelected = value;
                OnPropertyChanged(nameof(LambdaSelected));
                //Line(2,Xfdata(S),Simulate(S,Base(S)));
                Realdata(Xdata ,Ydata);
            }
        }
        public MainViewModel()
        {
            Filename = "No file are selected here !";
            Processtime = 0.000;
            RMSE = 0.000;
            Lambda  = new List<double> { 3,3.5,4,4.5,5,5.5,6,6.5,7,7.5,8,8.5,9 };
            lambdaSelected = 5;
        }
        public void OpenFile()
        {
            OpenFileDialog open = new()
            {
                Filter = string.Format("表格|*.xlsx;*.xls;*.csv;*.txt;")
            };
            if (open.ShowDialog() == true)
            {
                string path = open.FileName;
                Filename = path;
                ExcelReader(Filename);
            }
        }
        public void ExcelReader(string filepath)
        {
            DataTable dt = new();
            string fileName = filepath;
            string sheetName = "sheet1";//Excel的工作表名称
            bool isColumnName = true;//判断第一行是否为标题列
            IWorkbook workbook;//创建一个工作薄接口
            string fileExt = Path.GetExtension(fileName).ToLower();//获取文件的拓展名
            //创建一个文件流
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                //if (fileExt == ".xlsx")
                //{
                //    workbook = new XSSFWorkbook(fs);
                //}
                //else
                //{
                //    workbook = null;
                //}
                workbook = new XSSFWorkbook(fs);
                //实例化sheet
                ISheet sheet = null;
                if (sheetName != null && sheetName != "")//判断是否存在sheet
                {
                    sheet = workbook.GetSheet(sheetName);
                    if (sheet == null)
                    {
                        sheet = workbook.GetSheetAt(0);//从第一个开始读取，0位索引
                    }
                    else
                    {
                        sheet = workbook.GetSheetAt(0);
                    }
                }
                //获取表头
                IRow header = sheet.GetRow(sheet.FirstRowNum);
                int startRow = 0;//数据的第一行索引
                if (isColumnName)//表示第一行是列名
                {
                    startRow = sheet.FirstRowNum;//数据从第二行开始读

                    //遍历表的第一行，即所有的列名
                    for (int i = header.FirstCellNum; i < header.LastCellNum; i++)
                    {
                        ICell cell = header.GetCell(i);
                        if (cell != null)
                        {
                            //获取列名的值
                            string cellValue = cell.ToString();
                            if (cellValue != null)
                            {
                                DataColumn col = new DataColumn(cellValue);
                                dt.Columns.Add(col);
                            }
                            else
                            {
                                DataColumn col = new DataColumn();
                                dt.Columns.Add(col);
                            }
                        }
                    }
                }
                //读取数据
                for (int i = startRow; i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null)
                    {
                        continue;
                    }
                    DataRow dr = dt.NewRow();
                    for (int j = row.FirstCellNum; j < row.LastCellNum; j++)
                    {
                        if (row.GetCell(j) != null)
                        {
                            dr[j] = row.GetCell(j).ToString();
                        }

                    }
                    dt.Rows.Add(dr);
                }
                List<string> ls1 = new();
                List<string> ls2 = new();
                foreach (DataRow dr in dt.Rows)
                {
                    ls1.Add((string)dr[0]);
                    ls2.Add((string)dr[1]);
                }
                string[] ls11 = ls1.ToArray();
                string[] ls22 = ls2.ToArray();
                double[] x = new double[ls11.Length];
                double[] y = new double[ls11.Length];
                for (int i = 0; i < ls11.Length; i++)
                {
                    
                    x[i] = double.Parse(ls11[i]);
                    y[i] = double.Parse(ls22[i]);
                }
                page1.Datelength = x.Length;    
                page1.Xline1 = x;
                page1.Yline1 = y;
            }
            Xdata = page1.Xline1;
            Ydata = page1.Yline1;   
            page1.Plot(page1.Xline1, page1.Yline1);
        }
        public double[] Xfdata(string input)
        {
            if (input == "")
            {
                MessageBox.Show("Please enter the parameters!");
                return Array.Empty<double>();
            }
            else
            {
                string[] strArray = input.Split(',');
                double[] simulate = Array.ConvertAll<string, double>(strArray, s => double.Parse(s));
                double[] x = new double[(int)simulate[0]];
                double[] b = new double[x.Length];
                for (int i = 0; i < x.Length; i++)
                {
                    x[i] = i + simulate[1];
                }
                return x;
            }
        }
        public double[] Base(string input)
        {
            if (input == "")
            {
                MessageBox.Show("Please enter the parameters!");
                return Array.Empty<double>();
            }
            else
            {
                string[] strArray = input.Split(',');
                double[] simulate = Array.ConvertAll<string, double>(strArray, s => double.Parse(s));
                double[] x = new double[(int)simulate[0]];
                double[] b = new double[x.Length];
                for (int i = 0; i < x.Length; i++)
                {
                    x[i] = i + simulate[1];
                    b[i] = 5000.0 * Math.Sin((x[i] / 200)+30) + 5000.0;
                }
                return b;
            }
        }
        public double[] Simulate(string input, double[] b)
        {
            if (input == "")
            {
                MessageBox.Show("Please enter the parameters!");
                return Array.Empty<double>();
            }
            else{
                string[] strArray = input.Split(',');
                double[] simulate = Array.ConvertAll<string, double>(strArray, s => double.Parse(s));
                double[] x = new double[(int)simulate[0]];
                double[] y = new double[x.Length];
                int num = (int)simulate[2];
                for (int i = 0; i < x.Length; i++)
                {
                    x[i] = i + simulate[1];
                    for (int j = 0; j < num; j++)
                    {
                        y[i] += simulate[4 + j * 3] * simulate[5 + j * 3] * simulate[5 + j * 3] / (4 * Math.Pow((simulate[3 + j * 3] - x[i]), 2) + simulate[5 + j * 3] * simulate[5 + j * 3]);
                    }
                    y[i] = y[i] + b[i];
                }
                page1.Plot(x, y);
                return y;
            }
        }
        public double[] Polyfit(double[]x, double[]y)
        {
            double[] results = new double[y.Length];
            List<double> x3 = new();
            List<double> y3 = new();
            for (int i = 0; i < y.Length; i+=10)
            {
                var set = y.Skip(i).Take(10).ToArray();
                var min = set.Min();
                int index = 0;
                for (int j = 0; j < set.Length; j++)
                {
                    if (set[j]==min)
                    {
                       index = j+i;   
                    }
                }
                x3.Add(x[index]);
                y3.Add(min);
            }
            var xx = x3.ToArray();
            var yy = y3.ToArray();
            List<double> po1 = new();
            List<double> po2 = new();
            for (int i = 1; i < yy.Length-1; i++)
            {
                var d1 = yy[i] - yy[i-1];
                var d2 = yy[i + 1] - yy[i]; 
                if (d1/d2<1.5 || d1/d2 >0.5)
                {
                    po1.Add(xx[i]);
                    po2.Add(yy[i]);
                }
            }
            //double[] X = new double[yy.Length];
            //for (int i = 0; i <yy.Length; i++)
            //{
            //    X[i] = i+1;
            //}
            double[] parameters = Fit.Polynomial(po1.ToArray(), po2.ToArray(), 2);
            for (int i = 0; i < x.Length; i++)
            {
                //results[i] = parameters[0] + parameters[1] * x[i] + parameters[2] * x[i] * x[i]+ parameters[3] * Math.Pow(x[i],3) + parameters[4] * Math.Pow(x[i], 4)+ parameters[5] * Math.Pow(x[i], 5)- 0.01*(po2.Max() - po2.Min());
                results[i] = parameters[0] + parameters[1] * x[i] + parameters[2] * x[i] * x[i] - 0.1*(po2.Max() - po2.Min());
                //results[i] = parameters[0] + parameters[1] * x[i]-(po2.Max()-po2.Min());
            }
            return results;
        }
        public void Poly(double[] x, double[] y)
        {
            var b = Base(S);
            var z = Polyfit(x,y);
            page1.Plot1(x, y, z, b);
        }
        public void Realdata(double[] x, double[] y)
        {
            page1.Draw(x, y, 0);
            for (int i = 3; i < 8; i++)
            {
                double[] z = arPLSPlus(y, i, 2);
                page1.Draw(x,z,i);
            }
            
            //double[] z = arPLSPlus(y, lambdaSelected, 2);
            //double[] b = arPLSPlus(y, 8, 2);
            //RMSE = Rmse(z, b);

            //double[] d = new double[x.Length];
            //d = Sub(z, b);
            //page1.Plot1(x, y, z, b);

        }
        public void Line(int type, double[]x ,double[] y)
        {
            //double[] x = new double[y.Length];
            //for (int i = 0; i < x.Length; i++)
            //{
            //    x[i] = (double)i;
            //}
            //double[] y = new double[200];
            //double lambda1 = 121;
            //double Pi1 = 16;
            //double wi1 = 20;
            //double lambda2 = 151;
            //double Pi2 = 63;
            //double wi2 = 10;
            //for (int i = 0; i < x.Length; i++)
            //{
            //    x[i] = (double)i;
            //    //y[i] = 100.0 * Math.Exp(-1.0 * Math.Pow(((x[i] - 300) / 15), 2)) + 200.0 * Math.Exp(-1.0 * Math.Pow(((x[i] - 750) / 30), 2)) + 100 * Math.Exp(-1.0 * Math.Pow(((x[i] - 800) / 15), 2))+ 10 * Math.Sin(x[i] / 100) + 100.0;
            //    y[i] = Pi1*wi1*wi1/(4*Math.Pow((lambda1 - x[i]),2)+wi1*wi1)+ Pi2 * wi2 * wi2 / (4 * Math.Pow((lambda2 - x[i]), 2) + wi2 * wi2) + 10 * Math.Sin(x[i] / 100) + 10.0;
            //}
            if (type == 0)
            {
                page1.Plot(x, y); 
            }
            if (type == 1)
            {
                Noise(0, x, y,Base(S));
            }
            if (type == 2)
            {
                Noise(1, x, y,Base(S));
            }
        }
        //private double Random()
        //{
        //    var seed = Guid.NewGuid().GetHashCode();
        //    Random r = new(seed);
        //    int i = r.Next(0, 100000);
        //    return (double)i / 100000;
        //}
        public void Noise(int type,double[] x, double[] y, double[] b)
        {
            Random rd = new();
            for (int i = 0; i < x.Length; i++)
            {
                /*y[i] = y[i] + b[i]*/;
                y[i] = y[i] + .1 * y[i] * rd.Next(-1,1);
            }
            if (type == 0)
            {
                page1.Plot(x, y);
            }
            if (type == 1)
            {
                //double[] z = arPLS(y,1000,0.1);
                double[] z = arPLSPlus(y, lambdaSelected, 2);
                //double[] z = arPLSUltra(y);
                //double[] z = arPLSS(y, 2, b);
                RMSE = Rmse(z,b);
                double[] d = new double[x.Length];
                d = Sub(z,b);
                page1.Plot1(x, y, z, d);
            }
        }
        public double[] arPLSUltra(double[] x,double[] y)
        {

            int n = y.Length;
            var yy = DenseVector.OfArray(y);
            double[] z = new double[n];
            DiagonalMatrix A = new DiagonalMatrix(n, n, 1);
            double[,] B = returndiff(A.ToArray());
            var C = DenseMatrix.OfArray(B);
            double c = Math.Pow(10,5);
            var D = c * (C.Transpose() * C);
            var w = DenseVector.Create(n, 1);
            double r = 0.0;
            do
            {
                DiagonalMatrix W = new(n, n, 1);
                W.SetDiagonal(w);
                var E = W + D;
                var zz = E.Cholesky().Solve(W * yy);
                z = zz.ToArray();
                double[] d = Sub(y, z);
                double m = Mean(d);
                double s = STD(d);
                double[] wt = Logi(d, m, s);
                r = R(wt, w.ToArray());
                w = wt;
            } while (r > 0.01);
            return z;
        }
        public double[] arPLSPlus(double[] y, double lambda, double ratio)
        {
            int n = y.Length;
            var yy = DenseVector.OfArray(y);
            double[] z = new double[n];
            DiagonalMatrix A = new DiagonalMatrix(n, n, 1);
            double[,] B = returndiff(A.ToArray());
            var C = DenseMatrix.OfArray(B);
            double c = Math.Pow(10, lambda);
            var D =c*(C.Transpose() * C);
            var w = DenseVector.Create(n, 1);
            //var www = DenseVector.Create(n, 1);
            int i = 0;
            double r = 0.0;
            double rr = Math.Pow(10, -1.0 * ratio);
            do
            { 
                DiagonalMatrix W = new(n, n, 1);
                W.SetDiagonal(w);
                //var WW = Broadcasting(www);
                //var WWD = WW.Multiply(D);
                //var E = W + WWD;
                var E = W + D;
                var zz = E.Cholesky().Solve(W * yy);
                z = zz.ToArray();
                double[] d = Sub(y, z);
                double m = Mean(d);
                double s = STD(d);
                double[] wt = Logi(d, m, s);
                //double[] wwt = Logi2(d, m, s);
                r = R(wt, w.ToArray());
                w = wt;
                //www = wwt;
                i++;
            } while (r > rr||i < 10);
            return z;
        }
        public double[] arPLSS(double[] y, double ratio, double[]b)
        {
            int n = y.Length;
            var yy = DenseVector.OfArray(y);
            double[] z = new double[n];
            DiagonalMatrix A = new DiagonalMatrix(n,n,1);
            double[,] B = returndiff(A.ToArray());
            var C = DenseMatrix.OfArray(B);
            var w = DenseVector.Create(n,1);
            double r = 0.0;
            double lambda = 5.0;
            //double rm1 = 100.0;
            //double rm2 = 100.0;
            do
            {
                var D = Math.Pow(10, lambda) * (C.Transpose() * C);
                do
                {
                    DiagonalMatrix W = new(n, n, 1);
                    W.SetDiagonal(w);
                    var E = W + D;
                    var zz = E.Cholesky().Solve(W * yy);
                    z = zz.ToArray();
                    double[] d = Sub(y, z);
                    double m = Mean(d);
                    double s = STD(d);
                    double[] wt = Logi(d, m, s);
                    r = R(wt, w.ToArray());
                    w = wt;                   
                } while (r > Math.Pow(10, -1.0 * ratio));
                RMSE = Rmse(z, b);
                lambda = lambda + 0.5;
                //rm1 = RMSE;
                //if (rm1 <= rm2)
                //{
                //    lambda =lambda + 0.1;
                //    rm2 = rm1;
                //}
                //else
                //{
                //    lambda =lambda - 0.05;
                //    rm2 = rm1;
                //}
            } while ( RMSE > 0.5);
            lambda = lambda - 0.5;
            MessageBox.Show("the lambda is" + lambda.ToString());
            return z;
        }

        public double Rmse(double[] x, double[] y)
        {
         double rmse=0.0;
            for (int i = 0; i < x.Length ; i++)
            {
                rmse += (x[i] - y[i])* (x[i] - y[i])/x.Length;
            }
            rmse = Math.Sqrt(rmse);
            return rmse;
        }
        public double[] arPLS(double[] y, double lambda, double ratio)
        {
            int n = y.Length;
            double[,] D = returndiff(Speye(n, One(n, 1)));
            double[,] H = valueArrMultiply(lambda, arrMultiply(transpose(D),D));
            double[] w = One(n, 1);
            double[] z = new double[n];
            double r = 0.0;
            do
            {
                double[,] W = Speye(n, w);
                double[,] C = Add(W, H);
                var matrix = DenseMatrix.OfArray(C);
                var vector = DenseVector.OfArray(y);
                var x = DenseMatrix.OfArray(arrMultiply(matrix.Inverse().ToArray(), W));
                z = (x * vector).ToArray();
                double[] d = Sub(y, z);
                double m = Mean(d);
                double s = STD(d);
                double[] wt = Logi(d,m,s);
                r = R(wt,w);
                w = wt;
            } while (r > ratio);
            return z;
        }
        public double R(double[] wt,double[] w )
        {
            double r = 0.0;
            double norm1 = 0.0;
            double norm2 = 0.0;
            for (int i = 0; i < wt.Length; i++)
            {
                norm1 += (wt[i] - w[i])* (wt[i] - w[i]);
                norm2 += w[i] * w[i];
            }
            r = Math.Sqrt(norm1)/ Math.Sqrt(norm2);
            return r;
        }
        public double[] Logi(double[] input,double m, double s)
        {
            double[] logi = new double[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                logi[i] = 1.0 / (1 + Math.Exp(2 * (input[i] - (2 * s - m)) / s));
            }
            return logi;
        }
        public double[] Logi2(double[] input, double m, double s)
        {
            double[] logi = new double[input.Length];
            for (int i = 0; i < input.Length; i++)
            {
                logi[i] =1.0 - (1.0 / (1 + Math.Exp(2 * (input[i] - (2 * s - m)) / s)));
            }
            return logi;
        }
        public double[] One(int N ,double value)
        { 
            double[] one = new double[N];
            for (int i = 0; i < N; i++)
            {
                one[i] = value;
            }
            return one;
        }
        public double Mean(double[] input)
        {
            List<double> num = new List<double>();
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] < 0.0)
                {
                    num.Add(input[i]);
                }
            }
            double avg = 0.0;
            if (!num.Any()) //序列中包含数据
                return avg;
            avg = num.Average();
            return avg;
        }
        public double STD(double[] input)
        {
            List<double>  num = new List<double>();
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] < 0.0)
                {
                    num.Add(input[i]);
                }
            }
            double ret = 0.0;
            if (!num.Any()) //序列中包含数据
                return ret;
            int n = num.Count;
            //  计算平均数   
            double avg = num.Average();
            //  计算各数值与平均数的差值的平方，然后求和 
            double sumx = num.Sum(d => Math.Pow(d - avg, 2));
            //  除以数量，然后开方
            if (n > 0)
                ret = Math.Sqrt(sumx / n);
            return ret;
        }
        public double[] Sub(double[] input1,double[] input2)
        { 
            double [] sub = new double[input1.Length];
            for (int i = 0; i < input1.Length; i++)
            {
                sub[i] = input1[i]-input2[i];
            }
            return sub;
        }
        public double[,] Speye(int n,double[] y)
        {
            double[,] speye = new double[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (i == j)
                    {
                        speye[i, j] = y[i];
                    }
                    else speye[i, j] = 0.0;
                }
            }
            return speye;
        }
        public static double[,] returndiff(double[,] inputd)
        {
            int n = inputd.GetUpperBound(0) - 1;//因为差分一次行数少一
            int m = inputd.GetUpperBound(1) + 1;
            double[,] temparray = new double[n, m];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    temparray[i, j] = (inputd[i + 2, j] - inputd[i + 1, j])-(inputd[i + 1, j] - inputd[i, j]);
                }
            }
            return temparray;
        }
        public static double[,] transpose(double[,] inpute)
        {
            int x = inpute.GetUpperBound(0) + 1;//获得行数
            int y = inpute.GetUpperBound(1) + 1;//获得列数
            double[,] transarray = new double[y, x];
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y; j++)
                {
                    transarray[j, i] = inpute[i, j];
                }
            }
            return transarray;
        }
        public static double[,] arrMultiply(double[,] input1, double[,] input2)
        {
            int input1x = input1.GetUpperBound(0)+1;//获得行数
            int input1y = input1.GetUpperBound(1)+1;//获得列数
            int input2x = input2.GetUpperBound(0)+1;//获得行数
            int input2y = input2.GetUpperBound(1)+1;//获得列数
            double tempvale = 0;
            double[,] retarr = new double[input1x, input2y];
            //Parallel.For(0, input1x, k =>//可以用这个来解决运算慢的问题
            //{
            //    for (int i = 0; i < input2y; i++)
            //    {
            //        for (int j = 0; j < input2x; j++)
            //        {
            //            tempvale += input1[k, j] * input2[j, i];
            //        }
            //        retarr[k, i] = tempvale;
            //        tempvale = 0;
            //    }
            //});
            for (int k = 0; k < input1x; k++)
            {
                for (int i = 0; i < input2y; i++)
                {
                    for (int j = 0; j < input2x; j++)
                    {
                        tempvale += input1[k, j] * input2[j, i];
                    }
                    retarr[k, i] = tempvale;
                    tempvale = 0;
                }
            }
            return retarr;
        }
        public static double[,] valueArrMultiply(double input1, double[,] input2)
        {
            int input2x = input2.GetUpperBound(0) + 1;//获得行数
            int input2y = input2.GetUpperBound(1) + 1;//获得列数
            double[,] arraytempdd = new double[input2x, input2y];
            for (int i = 0; i < input2x; i++)
            {
                for (int j = 0; j < input2y; j++)
                {
                    arraytempdd[i, j] = input1 * input2[i, j];
                }
            }
            return arraytempdd;
        }
        public double[,] Add(double[,] input1, double[,] input2)
        {
            int inputx = input1.GetUpperBound(0)+1;//获得行数
            int inputy = input1.GetUpperBound(1)+1;//获得列数
            double[,] retarr = new double[inputx, inputy];
            for (int i = 0; i < inputx; i++)
            {
                for (int j = 0; j < inputy; j++)
                {
                    retarr[i, j] = input1[i,j] + input2[i, j];
                }
            }
            return retarr ;
        }

        public DenseMatrix Broadcasting(DenseVector x)
        {
            var xx = x.ToArray();
            DenseMatrix re = new(xx.Length,xx.Length);
            double[,] output = new double[xx.Length, xx.Length];
            for (int i = 0; i < xx.Length; i++)
            {
                for (int j = 0; j < xx.Length; j++)
                {
                    output[i, j] = xx[i];
                }
            }
            re = DenseMatrix.OfArray(output);
            return re;
        }
    }
}
