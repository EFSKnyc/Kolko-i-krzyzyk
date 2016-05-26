using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace KolkoIKrzyzyk
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int[] box;
        private int kolkoczykrzyzyktemp;
        public int kolkoczykrzyzyk
        {
            get
            {
                return kolkoczykrzyzyktemp;
            }
            set
            {
                if (value == 1)
                    Wynik.Text = "Teraz krzyżyk";
                else
                    Wynik.Text = "Teraz kółko";
                kolkoczykrzyzyktemp = value;
            }
        }
        public BitmapSource pusty;// = new BitmapImage(new Uri(path + @"\\Source files\\pusty.bmp"));
        public BitmapSource kolko;// = new BitmapImage(new Uri(path + @"\\Source files\\kolko.bmp"));
        public BitmapSource krzyzyk;// = new BitmapImage(new Uri(path + @"\\Source files\\krzyzyk.bmp"));

            //1 = krzyzyk, 0 = kolko
        public int minrow;
        public int maxrow;
        public int mincolumn;
        public int maxcolumn;
        public int howmany = 0;
        public MainWindow()
        { 
            InitializeComponent();
            kolkoczykrzyzyk = 1;
            pusty = Bitmap2BitmapSource(Properties.Resources.pusty); 
            kolko = Bitmap2BitmapSource(Properties.Resources.kolko);
            krzyzyk = Bitmap2BitmapSource(Properties.Resources.krzyzyk);
            maxrow = 504;
            maxcolumn = 504;
            minrow = 500;
            mincolumn = 500;
            for (int i = minrow; i <= maxrow; i++)
            {
                StackPanel instance = new StackPanel();
                instance.Orientation = Orientation.Horizontal;
                for (int k = mincolumn; k <= maxcolumn; k++)
                {
                    myImage image = new myImage(new int[2]{ i, k });
                    image.Source = pusty;
                    image.Width = 80;
                    image.Height = 80;
                    image.MouseLeftButtonDown += Zdjecie_MouseLeftButtonDown;
                    image.MouseLeftButtonUp += Zdjecie_MouseLeftButtonUp;
                    instance.Children.Add(image);
                }
                MAIN.Children.Add(instance);
            }
        }


        private void Zdjecie_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            myImage image = sender as myImage;
            if (box == image.place)
            {
                if (kolkoczykrzyzyk==1)
                {
                    if (!image.locked)
                    {
                        image.Source = krzyzyk;
                        image.locked = true;
                        image.kolkoalbokrzyzyk = kolkoczykrzyzyk;
                        if (DetectWin(image))
                        {
                            Wynik.Text = "Krzyzyk wygral";
                            MessageBoxResult res = MessageBox.Show("Krzyzyk wygral. Restartujesz grę?", "End of the game", MessageBoxButton.YesNo);
                            switch(res)
                            {
                                case MessageBoxResult.Yes:
                                    System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                                    Application.Current.Shutdown();
                                    break;
                                case MessageBoxResult.No:
                                    break;
                            }
                        }
                        kolkoczykrzyzyk = 0;                        
                    }                    
                }
                else
                {
                    if (kolkoczykrzyzyk==0)
                        if (!image.locked)
                        {
                            image.Source = kolko;
                            image.locked = true;
                            image.kolkoalbokrzyzyk = kolkoczykrzyzyk;
                            if (DetectWin(image))
                            {
                                Wynik.Text = "Kolko wygralo";
                                MessageBoxResult res = MessageBox.Show("Kolko wygralo. Restartujesz grę?", "End of the game", MessageBoxButton.YesNo);
                                switch (res)
                                {
                                    case MessageBoxResult.Yes:
                                        System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                                        Application.Current.Shutdown();
                                        break;
                                    case MessageBoxResult.No:
                                        break;
                                }
                            }
                            kolkoczykrzyzyk = 1;
                        }
                }
                    
                
            }
        }

        private void Zdjecie_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            myImage image = sender as myImage;
            box = image.place;
        }

        private bool DetectWin(myImage image)
        {
            Task<bool> horizontal = CheckHorizontal(image);
            Task<bool> vertical = CheckVertical(image);
            Task<bool> fromleftup = CheckFromLeftUp(image);
            Task<bool> fromleftdown = CheckFromLeftDown(image);

            if (horizontal.Result)
                return true;

            if (vertical.Result)
                return true;

            if (fromleftup.Result)
                return true;
            if (fromleftdown.Result)
                return true;


            return false;
        }



        public class myImage : System.Windows.Controls.Image
        {
            public int[] place;
            public bool locked;
            public int kolkoalbokrzyzyk;
            public myImage (int[] placeSource)
            {
                locked = false;
                place = placeSource;
                kolkoalbokrzyzyk = 5;
            }
        }

        private void AddBottomRows(int n)
        {
            for (int i = maxrow + 1; i <= maxrow+n; i++)
            {
                StackPanel instance = new StackPanel();
                instance.Orientation = Orientation.Horizontal;
                for (int k = mincolumn; k <= maxcolumn; k++)
                {
                    myImage image = new myImage(new int[2] { i, k });
                    image.Source = pusty;
                    image.Width = 80;
                    image.Height = 80;
                    image.MouseLeftButtonDown += Zdjecie_MouseLeftButtonDown;
                    image.MouseLeftButtonUp += Zdjecie_MouseLeftButtonUp;
                    instance.Children.Add(image);
                }
                MAIN.Children.Add(instance);
            }
            maxrow += n;
        }

       
        private void AddTopRows(int n)
        {
            for (int i = minrow-1; i >= minrow - n; i--)
            {
                StackPanel instance = new StackPanel();
                instance.Orientation = Orientation.Horizontal;
                for (int k = mincolumn; k <= maxcolumn; k++)
                {
                    myImage image = new myImage(new int[2] { i, k });
                    image.Source = pusty;
                    image.Width = 80;
                    image.Height = 80;
                    image.MouseLeftButtonDown += Zdjecie_MouseLeftButtonDown;
                    image.MouseLeftButtonUp += Zdjecie_MouseLeftButtonUp;
                    instance.Children.Add(image);                    
                }
                MAIN.Children.Insert(0, instance);
            }
            minrow -= n;
        }

        private void AddRightColumns(int n)
        {
            for (int i = minrow; i <= maxrow; i++)
            {
                var temp = MAIN.Children[i-minrow] as StackPanel;
                for (int k = maxcolumn + 1; k <= maxcolumn + n; k++)
                {
                    myImage image = new myImage(new int[2] { i, k });
                    image.Source = pusty;
                    image.Width = 80;
                    image.Height = 80;
                    image.MouseLeftButtonDown += Zdjecie_MouseLeftButtonDown;
                    image.MouseLeftButtonUp += Zdjecie_MouseLeftButtonUp;
                    temp.Children.Add(image);
                }

            }
            maxcolumn += n;
        }

        private void AddLeftColumns (int n)
        {
            for (int i = minrow; i <= maxrow; i++)
            {
                var temp = MAIN.Children[i - minrow] as StackPanel;
                for (int k = mincolumn - 1; k >= mincolumn - n; k--)
                {
                    myImage image = new myImage(new int[2] { i, k });
                    image.Source = pusty;
                    image.Width = 80;
                    image.Height = 80;
                    image.MouseLeftButtonDown += Zdjecie_MouseLeftButtonDown;
                    image.MouseLeftButtonUp += Zdjecie_MouseLeftButtonUp;
                    temp.Children.Insert(0,image);
                }

            }
            mincolumn -= n;

        }




        private void Button_Left(object sender, RoutedEventArgs e)
        {
            AddLeftColumns(1);
        }

        private void Button_Top(object sender, RoutedEventArgs e)
        {
            AddTopRows(1);
        }

        private void Button_Right(object sender, RoutedEventArgs e)
        {
            AddRightColumns(1);
        }

        private void Button_Bottom(object sender, RoutedEventArgs e)
        {
            AddBottomRows(1);
        }


        private BitmapSource Bitmap2BitmapSource(Bitmap bitmap)
        {
            BitmapSource i = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                           bitmap.GetHbitmap(),
                           IntPtr.Zero,
                           Int32Rect.Empty,
                           BitmapSizeOptions.FromEmptyOptions());
            return i;
        }





        public async Task<bool> CheckHorizontal(myImage image)
        {
            int horizontal = 1;
            var sth = MAIN.Children[image.place[0] - minrow] as StackPanel;
            for (int i = 1; i < 5; i++)
            {
                try
                {
                    var temp = sth.Children[image.place[1] - mincolumn + i] as myImage;
                    if (temp.kolkoalbokrzyzyk != image.kolkoalbokrzyzyk)
                        break;
                    horizontal++;
                }
                catch (IndexOutOfRangeException)
                { }
                catch (NullReferenceException)
                { }
                catch (ArgumentOutOfRangeException)
                { }

            }
            for (int i = 1; i < 5; i++)
            {
                try
                {
                    var temp = sth.Children[image.place[1] - mincolumn - i];
                    var temp2 = temp as myImage;
                    if (temp2.kolkoalbokrzyzyk != image.kolkoalbokrzyzyk)
                        break;
                    horizontal++;
                }
                catch (IndexOutOfRangeException)
                { }
                catch (NullReferenceException)
                { }
                catch (ArgumentOutOfRangeException)
                { }
            }
            if (horizontal > 4)
                return true;
            return false;
        }

        public async Task<bool> CheckVertical (myImage image)
        {
            int vertical = 1;
            for (int i = 1; i < 5; i++)
            {
                try
                {
                    var sth2 = MAIN.Children[image.place[0] - minrow + i] as StackPanel;
                    var temp = sth2.Children[image.place[1] - mincolumn];
                    var temp2 = temp as myImage;
                    if (temp2.kolkoalbokrzyzyk != image.kolkoalbokrzyzyk)
                        break;
                    vertical++;
                }
                catch (IndexOutOfRangeException)
                { }
                catch (NullReferenceException)
                { }
                catch (ArgumentOutOfRangeException)
                { }
            }
            for (int i = 1; i < 5; i++)
            {
                try
                {
                    var sth2 = MAIN.Children[image.place[0] - minrow - i] as StackPanel;
                    var temp = sth2.Children[image.place[1] - mincolumn];
                    var temp2 = temp as myImage;
                    if (temp2.kolkoalbokrzyzyk != image.kolkoalbokrzyzyk)
                        break;
                    vertical++;
                }
                catch (IndexOutOfRangeException)
                { }
                catch (NullReferenceException)
                { }
                catch (ArgumentOutOfRangeException)
                { }

            }
            if (vertical > 4)
                return true;
            return false;
        }

        public async Task<bool> CheckFromLeftUp(myImage image)
        {
            int fromleftup = 1;
            for (int i = 1; i < 5; i++)
            {
                try
                {
                    var sth2 = MAIN.Children[image.place[0] - minrow + i] as StackPanel;
                    var temp = sth2.Children[image.place[1] - mincolumn - i] as myImage;
                    if (temp.kolkoalbokrzyzyk != image.kolkoalbokrzyzyk)
                        break;
                    fromleftup++;
                }
                catch (IndexOutOfRangeException)
                { }
                catch (NullReferenceException)
                { }
                catch (ArgumentOutOfRangeException)
                { }

            }
            for (int i = 1; i < 5; i++)
            {
                try
                {
                    var sth2 = MAIN.Children[image.place[0] - minrow - i] as StackPanel;
                    var temp = sth2.Children[image.place[1] - mincolumn + i];
                    var temp2 = temp as myImage;
                    if (temp2.kolkoalbokrzyzyk != image.kolkoalbokrzyzyk)
                        break;
                    fromleftup++;
                }
                catch (IndexOutOfRangeException)
                { }
                catch (NullReferenceException)
                { }
                catch (ArgumentOutOfRangeException)
                { }

            }
            if (fromleftup > 4)
                return true;
            return false;
        }


        public async Task<bool> CheckFromLeftDown(myImage image)
        {
            int fromleftdown = 1;
            for (int i = 1; i < 5; i++)
            {
                try
                {
                    var sth2 = MAIN.Children[image.place[0] - minrow - i] as StackPanel;
                    var temp = sth2.Children[image.place[1] - mincolumn - i];
                    var temp2 = temp as myImage;
                    if (temp2.kolkoalbokrzyzyk != image.kolkoalbokrzyzyk)
                        break;
                    fromleftdown++;
                }
                catch (IndexOutOfRangeException)
                { }
                catch (NullReferenceException)
                { }
                catch (ArgumentOutOfRangeException)
                { }

            }

            for (int i = 1; i < 5; i++)
            {
                try
                {
                    var sth2 = MAIN.Children[image.place[0] - minrow + i] as StackPanel;
                    var temp = sth2.Children[image.place[1] - mincolumn + i] as myImage;
                    if (temp.kolkoalbokrzyzyk != image.kolkoalbokrzyzyk)
                        break;
                    fromleftdown++;
                }
                catch (IndexOutOfRangeException)
                { }
                catch (NullReferenceException)
                { }
                catch (ArgumentOutOfRangeException)
                { }

            }
            if (fromleftdown > 4)
                return true;
            return false;
        }


    }




}
