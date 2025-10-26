using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Captcha
{
    public partial class MainWindow : Window
    {
        private string captchaText = "";
        private readonly Random random = new();
        private int pixelCount = 100;
        private int lineCount = 10;
        private int captchaLenght = 5; // желательно от 4 до 10

        public MainWindow()
        {
            InitializeComponent();
            GenerateCaptcha();
        }

        private void GenerateCaptcha()
        {
            int width = 40 * captchaLenght;
            int height = 16 * captchaLenght;
            captchaText = GenerateRandomText(captchaLenght);

            using Bitmap bitmap = new(width, height);
            using Graphics g = Graphics.FromImage(bitmap);
            g.Clear(Color.LightGray);

            string[] fonts =
            {
                "Arial",
                "Times New Roman",
                "Calibri",
                "Tahoma",
                "Verdana",
                "Courier New",
                "Comic Sans MS",
                "Georgia"
            };
            for (int i = 0; i < captchaText.Length; i++)
            {
                string randomFont = fonts[random.Next(fonts.Length)];
                using Font font = new(randomFont, random.Next(24, 32));
                Brush brush = new SolidBrush(GetRandomColor());
                float x = 10 + i * 35 + random.Next(-5, 5);
                float y = random.Next(10, 30);
                g.DrawString(captchaText[i].ToString(), font, brush, x, y);
            }
            for (int i = 0; i < lineCount; i++)
            {
                System.Drawing.Point p1 = new(random.Next(width), random.Next(height));
                System.Drawing.Point p2 = new(random.Next(width), random.Next(height));
                g.DrawLine(Pens.Black, p1, p2);
            }
            for (int i = 0; i < pixelCount; i++)
            {
                int x = random.Next(width);
                int y = random.Next(height);
                bitmap.SetPixel(x, y, GetRandomColor());
            }
            CaptchaImage.Source = BitmapToImageSource(bitmap);
        }
        private string GenerateRandomText(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        private System.Drawing.Color GetRandomColor()
        {
            return System.Drawing.Color.FromArgb(
                (byte)random.Next(0, 150),
                (byte)random.Next(0, 150),
                (byte)random.Next(0, 150));
        }
        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using MemoryStream memory = new();
            bitmap.Save(memory, ImageFormat.Png);
            memory.Position = 0;

            BitmapImage bitmapimage = new();
            bitmapimage.BeginInit();
            bitmapimage.StreamSource = memory;
            bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapimage.EndInit();
            bitmapimage.Freeze();
            return bitmapimage;
        }
        private void Submit_Click(object sender, RoutedEventArgs e)
        {

            if (!string.IsNullOrEmpty(UserCaptchaInput.Text))
            {
                if(UserCaptchaInput.Text == captchaText)
                {
                    MessageBox.Show("Вы прошли!", "Успех!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Вы не прошли!", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
                    GenerateCaptcha();
                    UserCaptchaInput.Text = string.Empty;
                    return;
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите ответ", "Пустое поле ввода!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
