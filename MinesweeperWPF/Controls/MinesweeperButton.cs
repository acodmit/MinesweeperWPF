using MinesweeperWPF.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MinesweeperWPF.Controls
{
    internal class MinesweeperButton : Button
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public static int ButtonSize { get; set; } = 35; // Default value 35

        public MinesweeperButton(int row, int col) 
        {
            Row = row;
            Column = col;

            Height = ButtonSize;
            Width = ButtonSize;
            Margin = new System.Windows.Thickness(5);
            FontSize = 24;
            Background = Brushes.DimGray;
            Opacity = 0.3;
        }

        public void RefreshButton(Field field)
        {
            if (field.HasFlag)
            {
                Background = Brushes.Red;
            }
            else
            {
                Background = Brushes.DimGray;
            }

            if (!field.IsCovered)
            {
                Content = "";
                Background = Brushes.LightGray;
                IsEnabled = false;

                if (field.MinesAround > 0)
                {
                    Content = field.MinesAround.ToString();

                    switch (field.MinesAround)
                    {
                        case 1:
                            Foreground = Brushes.Blue;
                            break;
                        case 2:
                            Foreground = Brushes.Green;
                            break;
                        case 3:
                            Foreground = Brushes.Red;
                            break;
                        default:
                            Foreground = Brushes.DarkRed;
                            break;
                    }
                }
            }
        }
        public void ShowMine(Field field)
        {
            if (field.HasMine)
            {
                Image image = new Image();

                string relativePath = Path.Combine("..", "..", "Icons", "mine.ico");
                string fullPath = Path.Combine(Environment.CurrentDirectory, relativePath);

                image.Source = new BitmapImage(new Uri(fullPath));
                image.Stretch = Stretch.Uniform;
                Content = image;
            }
        }
    }
}

