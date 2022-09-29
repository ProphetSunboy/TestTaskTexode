using System.Windows;
using System.Windows.Controls;

namespace TestTask
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        MainWindowViewModel viewModel = new MainWindowViewModel();
        // Выводит пользователей в datagrid
        private void openFolder_Click(object sender, RoutedEventArgs e)
        {
            peopleGrid.ItemsSource = viewModel.Users;
            viewModel.GetUsers();
        }

        // Экспортирует статистику пользователя
        private void ExportUserData_Click(object sender, RoutedEventArgs e)
        {
            int currentRowIndex = peopleGrid.SelectedIndex;
            if (currentRowIndex == -1)
            {
                MessageBox.Show("Выберите пользователя", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            viewModel.ExportUserData(currentRowIndex);
        }

        // При изменении выбранного индекса перерисовывает график
        private void peopleGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int currentRowIndex = peopleGrid.SelectedIndex;
            if (currentRowIndex == -1)
            {
                MessageBox.Show("Выберите пользователя", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            cartesianChart.Series.Clear();
            cartesianChart.Visibility = Visibility.Visible;
            cartesianChart.Series.Add(viewModel.PrintGraph(currentRowIndex));
        }
    }
}
