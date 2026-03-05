using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32; // Для SaveFileDialog
using System.Windows.Media.Imaging; // Для JpegBitmapEncoder

namespace ImageAppWPF
{
    public partial class MainWindow : Window
    {
        private readonly string _baseUrl = "https://picsum.photos/600/400";
        private readonly HttpClient _httpClient = new HttpClient();
        private byte[] _currentImageBytes;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BtnLoad.IsEnabled = false;
                BtnDownload.IsEnabled = false;
                BtnDownload.Visibility = Visibility.Collapsed;
                TxtStatus.Text = "Загрузка изображения...";
                ImgDisplay.Source = null;
                _currentImageBytes = null;
                string uniqueUrl = $"{_baseUrl}?random={DateTime.Now.Ticks}";
                _currentImageBytes = await _httpClient.GetByteArrayAsync(uniqueUrl);
                using (var stream = new MemoryStream(_currentImageBytes))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = stream;
                    bitmap.EndInit();
                    bitmap.Freeze(); 

                    ImgDisplay.Source = bitmap;
                }
                TxtStatus.Text = "Изображение загружено успешно!";
                BtnDownload.IsEnabled = true;
                BtnDownload.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                TxtStatus.Text = "Ошибка загрузки.";
                MessageBox.Show($"Не удалось загрузить картинку.\nОшибка: {ex.Message}",
                    "Ошибка сети", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                BtnLoad.IsEnabled = true;
            }
        }
        private void BtnDownload_Click(object sender, RoutedEventArgs e)
        {
            if (_currentImageBytes == null)
            {
                MessageBox.Show("Сначала загрузите изображение!", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = $"random_image_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
            dlg.DefaultExt = ".jpg";
            dlg.Filter = "JPEG Images (.jpg)|*.jpg|All files (*.*)|*.*";
            dlg.Title = "Сохранить изображение как...";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                try
                {
                    TxtStatus.Text = "Сохранение файла...";
                    string filePath = dlg.FileName;
                    File.WriteAllBytes(filePath, _currentImageBytes);

                    TxtStatus.Text = $"Файл сохранен: {Path.GetFileName(filePath)}";
                    MessageBox.Show($"Картинка успешно сохранена!\nПуть: {filePath}",
                        "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    TxtStatus.Text = "Ошибка сохранения.";
                    MessageBox.Show($"Не удалось сохранить файл.\nОшибка: {ex.Message}",
                        "Ошибка диска", MessageBoxButton.OK, MessageBoxImage.Error);
                }ы
            }
        }
    }
}