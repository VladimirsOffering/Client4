using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using Client.ViewModels;
using System.Drawing.Imaging;
using System.Windows.Interop;

namespace Client.Models
{
    public  static class Temp 
    {
        public static BitmapImage BitmapToBitmapImage(this Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }
    }

    public class Card
    {
        public int value { get; private set; }
        public char suit { get; private set; }
        public BitmapImage img { get; private set; }
        public Card(int value, char suit)
        {
            this.value = value;
            this.suit = suit;
            this.img = Temp.BitmapToBitmapImage(GenerateCardImage(this));
        }


        Bitmap GenerateCardImage(Card card)
        {
            // Загружаем изображение целой карты
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", "cards1.png");
            Bitmap fullImage = new Bitmap(path);

            int width = 89;
            int height = 128;

            // Вычисляем координаты области для выделения карты
            int x = (card.value - 2) * width; // корды карты 
            int y = 0;
            switch (card.suit)
            {
                case 'C': y = 0 ; break;
                case 'D': y = height; break;
                case 'H': y = height * 2; break;
                case 'S': y = height * 3; break;
            }

            Bitmap segment = new Bitmap(width, height);
            var g = Graphics.FromImage(segment);
            g.DrawImage(fullImage, 0, 0, new Rectangle(x, y , width, height), GraphicsUnit.Pixel);
            return segment;
        }
    }
}
