using System;
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
using System.Data;
using System.Windows.Markup;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Runtime.InteropServices;


namespace Uniterm
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AllocConsole();

        public Window1()
        {
            InitializeComponent();
            AllocConsole();
        }

        DataBase db;
        bool nowy = false, modified = false;
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            cDrawing.ClearAll();
        }

        private void ehMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            

            foreach (FontFamily f in System.Windows.Media.Fonts.SystemFontFamilies)
            {
                cbFonts.Items.Add(f);
            }
            if (cbFonts.Items.Count > 0)
                cbFonts.SelectedIndex = 0;

            for (int i = 8; i <= 40; i++)
            {
                cbfSize.Items.Add(i);
            }
            cbfSize.SelectedIndex = 4;


            db = new DataBase();
            DataTable dt = db.GetData();

            lbUniterms.SelectionChanged -= ehlbUNitermsSelectionChanged;
            lbUniterms.Items.Clear();

            Console.WriteLine("Zawartość bazy danych:");
            foreach (DataRow row in dt.Rows)
            {
                Console.WriteLine($"{row["sA"]}, {row["sB"]}, {row["sOp"]}, {row["eA"]}, {row["eB"]}, {row["eC"]}");
                lbUniterms.Items.Add(row["name"].ToString());
            }
            modified = false;
            nowy = false;
            lbUniterms.SelectionChanged += ehlbUNitermsSelectionChanged;
        }

        private void ehCBFontsChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                MyDrawing.fontFamily = new FontFamily(e.AddedItems[0].ToString());
                modified = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void ehcbfSizeChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                MyDrawing.fontsize = (int)e.AddedItems[0];
                modified = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddUniterm au = new AddUniterm();

            au.ShowDialog();

            if (au.tbA.Text.Length > 250 || au.tbB.Text.Length > 250)
            {
                MessageBox.Show("Zbyt długi tekst!\n Maksymalna długość tekstu to 250 znaków!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MyDrawing.sA = au.tbA.Text;
            MyDrawing.sB = au.tbB.Text;

            MyDrawing.sOp = au.rbSr.IsChecked == true ? " ; " : " , ";

            btnRedraw_Click(sender, e);

            modified = true;

        }

        private void btnAddEl_Click(object sender, RoutedEventArgs e)
        {
            AddElem ae = new AddElem();

            ae.ShowDialog();
            if (ae.tbA.Text.Length > 250 || ae.tbB.Text.Length > 250 || ae.tbC.Text.Length > 250)
            {
                MessageBox.Show("Zbyt długi tekst!\n Maksymalna długość tekstu to 250 znaków!", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            MyDrawing.eA = ae.tbA.Text;
            MyDrawing.eB = ae.tbB.Text;
            MyDrawing.eC = ae.tbC.Text;

            btnRedraw_Click(sender, e);
            modified = true;
        }

        private void btnRedraw_Click(object sender, RoutedEventArgs e)
        {
            cDrawing.ClearAll();

            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                MyDrawing md = new MyDrawing(dc);

                md.Redraw();
                dc.Close();
            }
            cDrawing.AddElement(dv);

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {

            char operacja = 'X';
            
            switch (MessageBox.Show("Co zamienić?\n [Tak]==A, [Nie]==B", "Zamień", MessageBoxButton.YesNoCancel, MessageBoxImage.Question))
            {
                case MessageBoxResult.Yes: operacja = 'A';
                
                    break;
                case MessageBoxResult.No: operacja = 'B';
                
                    break;
                case MessageBoxResult.Cancel: return;
            }

            cDrawing.ClearAll();
            MyDrawing.oper = operacja;
            btnRedraw_Click(sender, e);
            modified = true;
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
           // Int32 fontsize_1 = (Int32)MyDrawing.fontsize;
            try
            {
                if(db == null)
                {
                    db = new DataBase();
                }

                db.AddData(MyDrawing.sA, MyDrawing.sB, MyDrawing.sOp, MyDrawing.eA, MyDrawing.eB, MyDrawing.eC, MyDrawing.fontFamily.ToString(), MyDrawing.fontsize, MyDrawing.oper,tbName.Text, tbDescription.Text);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                MessageBox.Show(ex.Message, "Wystąpił błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Window_Loaded(sender, e);

            lbUniterms.SelectionChanged -= ehlbUNitermsSelectionChanged;
            lbUniterms.SelectedValue = tbName.Text;
            lbUniterms.SelectionChanged += ehlbUNitermsSelectionChanged;

          
        }

        private bool CheckSave(object sender, SelectionChangedEventArgs e)
        {

            if (!modified)
                return true;
            else
            {
                try
                {
                    switch (MessageBox.Show("Chcesz zapisać?", "Zapis", MessageBoxButton.YesNoCancel, MessageBoxImage.Question))
                    {
                        case MessageBoxResult.Yes:
                            {
                                if (tbName.Text == null || tbName.Text.Equals(""))
                                {
                                    throw new Exception("Nie podano nazwy!");
                                }
                                MenuItem_Click_1(sender, e);
                                modified = false;
                                nowy = false;
                                return true;
                            }
                        case MessageBoxResult.No:
                            {
                                ehNowyClick(sender, e);
                                modified = false;
                                nowy = false;
                                return true;
                            }
                        case MessageBoxResult.Cancel: return false;
                        default: return false;
                    }
                } catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Wystąpił błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
        }

        private void ehlbUNitermsSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine("Odczytywanie z Bazy");
            string name = lbUniterms.SelectedValue.ToString();

            if (CheckSave(sender, e))
            {
                DataRow dr;
                try
                {
                    
                    dr = db.GetDataByName(name).Rows[0];


                    MyDrawing.eA = dr.IsNull("eA") ? null : dr["eA"].ToString().Trim();
                    MyDrawing.eB = dr.IsNull("eB") ? null : dr["eB"].ToString().Trim();
                    MyDrawing.eC = dr.IsNull("eC") ? null : dr["eC"].ToString().Trim();

                    MyDrawing.sA = dr.IsNull("sA") ? null : dr["sA"].ToString().Trim();
                    MyDrawing.sB = dr.IsNull("sB") ? null : dr["sB"].ToString().Trim();
                    MyDrawing.sOp = dr.IsNull("sOp") ? null : " "+dr["sOp"].ToString().Trim()+" ";

                    // FontFamily
                    string fontName = dr.IsNull("fontFamily") ? "Arial" : dr["fontFamily"].ToString().Trim();
                    MyDrawing.fontFamily = new FontFamily(fontName);

                    // fontSize
                    MyDrawing.fontsize = dr.IsNull("fontSize") ? 12 : Convert.ToInt32(dr["fontSize"]);

                    // textboxy
                    tbName.Text = dr.IsNull("name") ? "" : dr["name"].ToString().Trim();
                    tbDescription.Text = dr.IsNull("description") ? "" : dr["description"].ToString().Trim();

                    // ComboBoxy
                    cbFonts.SelectedValue = MyDrawing.fontFamily;
                    cbfSize.SelectedValue = MyDrawing.fontsize;

                    // Canvas i rysowanie
                    cDrawing.ClearAll();

                    DrawingVisual dv = new DrawingVisual();
                    cDrawing.Width = 5000;
                    cDrawing.Height = 5000;

                    btnRedraw_Click(sender, e);

                    nowy = false;
                    modified = false;

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
            else
            {
                Console.WriteLine("Nie można załadować elementu, ponieważ nie zapisano zmian!");
            }
        }

        private void ehNowyClick(object sender, RoutedEventArgs e)
        {
            MyDrawing.ClearAll();
            cDrawing.ClearAll();
            nowy = true;
            modified = false;
        }

        private void tbDescKeyUP(object sender, KeyEventArgs e)
        {
            modified = true;
        }

        private void HorScroll_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            TranslateTransform tt = new TranslateTransform();
            tt.X = -HorScroll.Value;
            tt.Y = -VerScroll.Value;

            cDrawing.RenderTransform = tt;
        }





    }
}
