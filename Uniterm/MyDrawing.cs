﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Uniterm
{

    public class MyDrawing
    {

        #region Fields

        public static Pen pen
        {
            get
            {
                return new Pen(Brushes.SteelBlue, (int)Math.Log(fontsize, 3));
            }
        }

        private static Brush br = Brushes.White;

        public static FontFamily fontFamily = new FontFamily("Arial");

        public static /*double*/ Int32 fontsize = 12;

        public static string sA, sB, sOp;

        public static string eA, eB, eC;

        public static char oper = ' ';


        public static int FS = 12;

        public DrawingContext dc;
        #endregion

        #region Initalizers

        public MyDrawing(DrawingContext drawingContext)
        {
            dc = drawingContext;
        }

        #endregion

        #region Public Methods

        public void Redraw()
        {
            if (oper != ' ')          
            {
                DrawSwitched(new Point(20, fontsize + 30));
            }
            else
            {
                if (sA != "")
                {
                    DrawSek(new Point(30, fontsize + 30));
                }
                if (eA != "")
                {
                    DrawElim(new Point(30, fontsize * 3 + 30));
                    //DrawSek(new Point(30, fontsize + 30));
                }
            }
        }

        public static void ClearAll()
        {
            sA = sB = sOp = "";
            eA = eB = eC = "";
            oper = ' ';
        }

        public void DrawSek(Point pt)
        {
            if (sA == "" || sOp == "") return;
            int len = GetTextLength(sA + sOp + sB);

            DrawText(pt, sA + sOp + sB);
            DrawBezier(new Point(pt.X, pt.Y - 1), len);
        }

      public void DrawElim(Point pt)
        
      {
            if (eA == "" || eB == "" || eC == "") return;

            Point p2 = new Point(pt.X + 2, pt.Y);
            string text = eA + " ; " + eB +" ; " + eC;

            double l = GetTextLength(text);

            //DrawText(p2, text);
            //DrawVert(pt, (int)l);
            DrawText(pt, text);
            DrawElHorizontal(pt, (int)l);
        }

        public void DrawSwitched(Point pt)
        {
            if (sA == "" || sOp == "" || eA == "" || eB == "" || eC == "") return;


            string textElim = eA + " ; " + eB + " ; " + eC;

            int length = GetTextLength(textElim);

            string tmpOp = " " + sOp + " ";
            Console.WriteLine("sOp:"+ tmpOp);

            if (oper == 'A')
            {
                DrawText(new Point(pt.X + length + (fontsize / 3), pt.Y + 3), tmpOp + sB);
                DrawElim(new Point(pt.X + (fontsize / 3), pt.Y + 5));
                length += GetTextLength(tmpOp + sB) + (int)(fontsize / 3);
            }
            if (oper == 'B')
            {
                int sekTextLenght = GetTextLength(sA+tmpOp)+10;

                DrawText(new Point(pt.X+5,pt.Y), sA + tmpOp+ " ");
                DrawElim(new Point(pt.X + sekTextLenght + (fontsize / 3), pt.Y + 5));

                length += GetTextLength(sOp + sB) + (int)(fontsize / 3)+10;
            }

            DrawBezier(pt, length + 5);

        }
        #endregion

        #region Private Methods

        private void DrawVert(Point pt, int length)
        {
            dc.DrawLine(pen, pt, new Point { X = pt.X, Y = pt.Y + length });
            double b = (Math.Sqrt(length) / 2) + 2;

            dc.DrawLine(pen, new Point(pt.X - (b / 2), pt.Y), new Point(pt.X + (b / 2), pt.Y));
            dc.DrawLine(pen, new Point(pt.X - (b / 2), pt.Y + length), new Point(pt.X + (b / 2), pt.Y + length));

        }

        private void DrawBezier(Point p0, int length)
        {
            Point start = p0;
            Point p1 = new Point(), p2 = new Point(), p3 = new Point();

            p3.Y = p0.Y;
            p3.X = p0.X + length;

            int b = (int)Math.Sqrt(length) + 2;

            p1.X = p0.X + (int)(length * 0.25);
            p1.Y = p0.Y - b;

            p2.X = p0.X + (int)(length * 0.75);
            p2.Y = p0.Y - b;

            foreach (Point pt in GetBezierPoints(p0, p1, p2, p3))
            {
                dc.DrawLine(pen, start, pt);
                start = pt;
            }
        }

        private void DrawElHorizontal(Point p0,int length)
        {
            Point start = p0;
            Point end  = new Point();
            end.X = p0.X + length;
            end.Y = p0.Y;

            double b = (Math.Sqrt(length) / 2) + 4;

            dc.DrawLine(pen, new Point(start.X, start.Y - (b / 2)), new Point(start.X, start.Y + (b / 2)));
            dc.DrawLine(pen, new Point(end.X, end.Y - (b / 2)), new Point(end.X, end.Y + (b / 2)));

            dc.DrawLine(pen, start, end);

        }

        private void DrawText(Point point, string text)
        {
            dc.DrawText(GetFormattedText(text), point);
        }

        private int GetTextHeight(string text)
        {
            return (int)GetFormattedText(text).Height;
        }

        private int GetTextLength(string text)
        {
            return (int)GetFormattedText(text).Width;
        }

        private IEnumerable<Point> GetBezierPoints(Point A, Point B, Point C, Point D)
        {
            List<Point> points = new List<Point>();

            for (double t = 0.0d; t <= 1.0; t += 1.0 / 500)
            {
                double tbs = Math.Pow(t, 2);
                double tbc = Math.Pow(t, 3);
                double tas = Math.Pow((1 - t), 2);
                double tac = Math.Pow((1 - t), 3);

                points.Add(new Point
                {
                    Y = +tac * A.Y
                        + 3 * t * tas * B.Y
                        + 3 * tbs * (1 - t) * C.Y
                        + tbc * D.Y,
                   X = +tac * A.X
                        + 3 * t * tas * B.X
                        + 3 * tbs * (1 - t) * C.X
                        + tbc * D.X
                });
            }

            return points;
        }

        private FormattedText GetFormattedText(string text)
        {
            FontStyle style = FontStyles.Normal;

            style = FontStyles.Normal;
            Typeface typeface = new Typeface(fontFamily, style, FontWeights.Light, FontStretches.Medium);

            FormattedText formattedText = new FormattedText(text,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                typeface, fontsize, Brushes.Black);

            formattedText.TextAlignment = TextAlignment.Left;

            return formattedText;
        }

        #endregion
    }
}
