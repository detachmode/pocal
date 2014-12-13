using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;

namespace Pocal.Resources
{
    public partial class LiveTile : UserControl
    {
        public LiveTile()
        {
            InitializeComponent();
            tb1.Text = DateTime.Now.ToShortTimeString();
            LayoutRoot.UpdateLayout();
        }


        public static void UpdateTile()
        {
            
            var customTile = new LiveTile();
            customTile.Measure(new Size(336, 336));
            customTile.Arrange(new Rect(0, 0, 336, 336));

            var customTileWide = new LiveTile();
            customTileWide.Measure(new Size(691, 336));
            customTileWide.Arrange(new Rect(0, 0, 691, 336));

            var bmp = new WriteableBitmap(336, 336);
            bmp.Render(customTile, null);
            bmp.Invalidate();

            var bmp2 = new WriteableBitmap(691, 336);
            bmp2.Render(customTileWide, null);
            bmp2.Invalidate();

            const string filename = "/Shared/ShellContent/CustomTile.jpg";
            const string filenameWide = "/Shared/ShellContent/CustomTileWide.jpg";

            using (var isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!isf.DirectoryExists("/CustomLiveTiles"))
                {
                    isf.CreateDirectory("/CustomLiveTiles");
                }

                using (var stream = isf.OpenFile(filename, System.IO.FileMode.OpenOrCreate))
                {
                    bmp.SaveJpeg(stream, 336, 336, 0, 100);
                }

                using (var stream = isf.OpenFile(filenameWide, System.IO.FileMode.OpenOrCreate))
                {
                    bmp2.SaveJpeg(stream, 691, 336, 0, 100);
                }
            }
            try
            {

                FlipTileData tileData = new FlipTileData
                {
                    //Title = "CustomSecondaryTile", 
                    WideBackgroundImage = new Uri("isostore:" + filenameWide, UriKind.Absolute),
                    BackgroundImage = new Uri("isostore:" + filename, UriKind.Absolute),
                };

                //string tileUri = string.Concat("/MainPage.xaml?", "");
                ShellTile currentTile = ShellTile.ActiveTiles.FirstOrDefault();
                currentTile.Update(tileData);
                //ShellTile.Create(new Uri(tileUri, UriKind.Relative), tileData, true);
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message);
            }
        }
    }
}
