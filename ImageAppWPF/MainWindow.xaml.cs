using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageAppWPF
{
    public partial class MainWindow : Window
    {
        // URL сервиса случайных картинок
        private readonly string _imageUrl = "https://picsum.photos/400/300";
        private readonly HttpClient _httpClient = new HttpClient();

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void BtnShow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TxtStatus.Text = "Загрузка...";
                ImgDisplay.Source = null; // Очистить предыдущее

                // Добавляем случайный параметр, чтобы браузер не кэшировал одну и ту же картинку
                string uniqueUrl = $"{_imageUrl}?random={DateTime.Now.Ticks}";

                // Скачиваем байты изображения
                byte[] imageBytes = await _httpClient.GetByteArrayAsync(uniqueUrl);

                // Преобразуем байты в источник изображения для WPF
                using (var stream = new System.IO.MemoryStream(imageBytes))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.StreamSource = stream;
                    bitmap.EndInit();
                    bitmap.Freeze(); // Замораживаем для безопасности потоков

                    ImgDisplay.Source = bitmap;
                }

                TxtStatus.Text = "Картинка успешно загружена!";
            }
            catch (Exception ex)
            {
                TxtStatus.Text = "Ошибка загрузки.";
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}