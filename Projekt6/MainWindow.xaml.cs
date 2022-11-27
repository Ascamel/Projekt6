using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Projekt6
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Ellipse elip = new Ellipse();
        private Point anchorPoint;
        private List<Ellipse> ellipses = new();
        private List<Line> helpingLines = new();
        private Point[,] BezierPoints = new Point[50, 1000];

        //  private List<Ellipse> BezierEllipses = new();

        private Ellipse[,] BezierEllipses = new Ellipse[50, 1000];

        private static float[] Factorial = new float[]
        {
            1.0f,
            1.0f,
            2.0f,
            6.0f,
            24.0f,
            120.0f,
            720.0f,
            5040.0f,
            40320.0f,
            362880.0f,
            3628800.0f,
            39916800.0f,
            479001600.0f,
            6227020800.0f,
            87178291200.0f,
            1307674368000.0f,
            20922789888000.0f,
        };

        private static float Binomial(int n, int i)
        {
            float ni;
            float a1 = Factorial[n];
            float a2 = Factorial[i];
            float a3 = Factorial[n - i];
            ni = a1 / (a2 * a3);
            return ni;
        }

        public MainWindow()
        {
            InitializeComponent();

        }

        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DrawCircle(e);

            if (ellipses.Count > 1)
            {
                DrawHelpLine();
            }

            if (helpingLines.Count % 2 == 0 && helpingLines.Count != 0)
            {
                float t = 0F;
                while (t <= 1)
                {
                    Ellipse newElipse = new()
                    {
                        Stroke = Brushes.Black,
                        StrokeThickness = 1,
                        Fill = Brushes.Black
                    };

                    Point Point00 = new()
                    {
                        X = helpingLines[helpingLines.Count - 2].X1,
                        Y = helpingLines[helpingLines.Count - 2].Y1,
                    };
                    Point Point01 = new()
                    {
                        X = helpingLines[helpingLines.Count - 2].X2,
                        Y = helpingLines[helpingLines.Count - 2].Y2,
                    };
                    Point Point02 = new()
                    {
                        X = helpingLines[helpingLines.Count - 1].X2,
                        Y = helpingLines[helpingLines.Count - 1].Y2,
                    };

                    float P20X = (float)(Binomial(2, 0) * Point00.X * Math.Pow(1-t, 2) + Binomial(2, 1) * Point01.X * t * (1-t) + Binomial(2, 2) * Point02.X * t * t);
                    float P20Y = (float)(Binomial(2, 0) * Point00.Y * Math.Pow(1-t, 2) + Binomial(2, 1) * Point01.Y * t * (1-t) + Binomial(2, 2) * Point02.Y * t * t);

                    Point P20 = new(P20X, P20Y);

                    BezierPoints[helpingLines.Count - 2, (int)(t *1000)] = P20;

                    BezierEllipses[helpingLines.Count - 2, (int)(t *1000)] = newElipse;

                    canvas.Children.Add(newElipse);

                    Canvas.SetTop(newElipse, P20.Y);
                    Canvas.SetLeft(newElipse, P20.X);

                    t += 0.001F;
                }
            }
        }

        private void DrawHelpLine()
        {
            Line line = new()
            {
                Stroke = Brushes.Black,
                StrokeThickness = .1,
            };
            Ellipse start = ellipses[ellipses.Count - 2];
            Ellipse end = ellipses[ellipses.Count - 1];

            line.X1 = Canvas.GetLeft(start) + 2;
            line.X2 = Canvas.GetLeft(end) + 2;
            line.Y1 = Canvas.GetTop(start) + 2;
            line.Y2 = Canvas.GetTop(end) + 2;

            helpingLines.Add(line);

            canvas.Children.Add(line);
        }

        private void DrawCircle(MouseButtonEventArgs e)
        {
            canvas.CaptureMouse();

            anchorPoint = e.MouseDevice.GetPosition(canvas);
            elip = new Ellipse
            {
                Stroke = Brushes.Red,
                StrokeThickness = 10,
                Fill = Brushes.Red
            };
            canvas.Children.Add(elip);

            ellipses.Add(elip);

            Canvas.SetTop(elip, anchorPoint.Y);
            Canvas.SetLeft(elip, anchorPoint.X);

            canvas.ReleaseMouseCapture();

            Label label = new();
            label.Content = (ellipses.Count);

            StackPanel stackPanel = new()
            {
                Orientation = Orientation.Horizontal
            };

            TextBox textBox1 = new()
            {
                Text = Math.Round(anchorPoint.X).ToString()
            };
            TextBox textBox2 = new()
            {
                Text = Math.Round(anchorPoint.Y).ToString()
            };

            stackPanel.Children.Add(textBox1);
            stackPanel.Children.Add(textBox2);

            MyStackPanel.Children.Add(label);
            MyStackPanel.Children.Add(stackPanel);

        }
        DependencyObject element;

        private void canvas_LeftDown(object sender, MouseButtonEventArgs e)
        {
            var canvas = sender as Canvas;
            if (canvas == null)
                return;

            HitTestResult hitTestResult = VisualTreeHelper.HitTest(canvas, e.GetPosition(canvas));
            element = hitTestResult.VisualHit;

        }

        private void canvas_LeftUp(object sender, MouseButtonEventArgs e)
        {
            if (element != null && element is Ellipse)
            {

                var tmp = element as Ellipse;
                Point MoveAnchorPoint = e.MouseDevice.GetPosition(canvas);

                int startingLineIndex = helpingLines.FindIndex(l => l.X1 == Canvas.GetLeft(tmp) + 2);

                if (startingLineIndex >= 0)
                {
                    Line startingLine = helpingLines[startingLineIndex];

                    if (startingLineIndex - 1 >= 0)
                    {
                        int beforeIndex = startingLineIndex - 1;

                        if (beforeIndex >= 0)
                        {
                            Canvas.SetTop(tmp, MoveAnchorPoint.Y);
                            Canvas.SetLeft(tmp, MoveAnchorPoint.X);

                            if (startingLineIndex % 2 != 0)
                            {
                                helpingLines[startingLineIndex].X1 = Canvas.GetLeft(tmp) + 2;
                                helpingLines[startingLineIndex].Y1 = Canvas.GetTop(tmp) + 2;

                                helpingLines[beforeIndex].X2 = Canvas.GetLeft(tmp) + 2;
                                helpingLines[beforeIndex].Y2 = Canvas.GetTop(tmp) + 2;

                                ReDrawBezier(startingLineIndex);
                            }
                            else
                            {
                                int endLineIndex = helpingLines.FindIndex(l => l.X1 == startingLine.X2);

                                if (endLineIndex > 0)
                                {
                                    Line endLine = helpingLines[endLineIndex];

                                    Canvas.SetTop(tmp, MoveAnchorPoint.Y);
                                    Canvas.SetLeft(tmp, MoveAnchorPoint.X);

                                    helpingLines[startingLineIndex].X1 = Canvas.GetLeft(tmp) + 2;
                                    helpingLines[startingLineIndex].Y1 = Canvas.GetTop(tmp) + 2;

                                    ReDrawBezier(endLineIndex);
                                }

                                int endLineIndex2 = endLineIndex - 2;
                                int startingLineIndex2 = startingLineIndex - 2;

                                if (endLineIndex2 > 0)
                                {
                                    Line endLine = helpingLines[endLineIndex2];

                                    Canvas.SetTop(tmp, MoveAnchorPoint.Y);
                                    Canvas.SetLeft(tmp, MoveAnchorPoint.X);

                                    helpingLines[endLineIndex2].X2 = Canvas.GetLeft(tmp) + 2;
                                    helpingLines[endLineIndex2].Y2 = Canvas.GetTop(tmp) + 2;

                                    ReDrawBezier(endLineIndex2);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (helpingLines[startingLineIndex + 1] != null)
                        {
                            int endLineIndex = helpingLines.FindIndex(l => l.X1 == startingLine.X2);

                            if (endLineIndex > 0)
                            {
                                Line endLine = helpingLines[endLineIndex];

                                Canvas.SetTop(tmp, MoveAnchorPoint.Y);
                                Canvas.SetLeft(tmp, MoveAnchorPoint.X);

                                helpingLines[startingLineIndex].X1 = Canvas.GetLeft(tmp) + 2;
                                helpingLines[startingLineIndex].Y1 = Canvas.GetTop(tmp) + 2;

                                ReDrawBezier(endLineIndex);
                            }
                        }
                    }
                }
                else
                {
                    int endLineIndex = helpingLines.FindIndex(l => l.X2 == Canvas.GetLeft(tmp) + 2);

                    if (endLineIndex > 0)
                    {
                        Line endLine = helpingLines[endLineIndex];
                        startingLineIndex = helpingLines.FindIndex(l => l.X2 == endLine.X2);

                        Canvas.SetTop(tmp, MoveAnchorPoint.Y);
                        Canvas.SetLeft(tmp, MoveAnchorPoint.X);

                        helpingLines[endLineIndex].X2 = Canvas.GetLeft(tmp) + 2;
                        helpingLines[endLineIndex].Y2 = Canvas.GetTop(tmp) + 2;

                        ReDrawBezier(endLineIndex);
                    }

                }
            }
        }

        private void ReDrawBezier(int index)
        {
            if (index > 0)
            {
                for (int k = 0; k < 1000; k++)
                {
                    Ellipse ellipse = BezierEllipses[index - 1, k];
                    canvas.Children.Remove(ellipse);
                }

                float t = 0F;
                while (t <= 1)
                {
                    Ellipse newElipse = new()
                    {
                        Stroke = Brushes.Black,
                        StrokeThickness = 1,
                        Fill = Brushes.Black
                    };

                    Point Point00 = new()
                    {
                        X = helpingLines[index - 1].X1,
                        Y = helpingLines[index - 1].Y1,
                    };
                    Point Point01 = new()
                    {
                        X = helpingLines[index - 1].X2,
                        Y = helpingLines[index - 1].Y2,
                    };
                    Point Point02 = new()
                    {
                        X = helpingLines[index].X2,
                        Y = helpingLines[index].Y2,
                    };

                    float P20X = (float)(Binomial(2, 0) * Point00.X * Math.Pow(1-t, 2) + Binomial(2, 1) * Point01.X * t * (1-t) + Binomial(2, 2) * Point02.X * t * t);
                    float P20Y = (float)(Binomial(2, 0) * Point00.Y * Math.Pow(1-t, 2) + Binomial(2, 1) * Point01.Y * t * (1-t) + Binomial(2, 2) * Point02.Y * t * t);

                    Point P20 = new(P20X, P20Y);


                    BezierPoints[index - 1, (int)(t *1000)] = P20;

                    canvas.Children.Add(newElipse);

                    BezierEllipses[index - 1, (int)(t *1000)] = newElipse;

                    Canvas.SetTop(newElipse, P20.Y);
                    Canvas.SetLeft(newElipse, P20.X);

                    t += 0.001F;
                }
            }
        }
    }
}
