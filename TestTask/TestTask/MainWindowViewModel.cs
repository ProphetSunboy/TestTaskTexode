using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;
using Newtonsoft.Json;

namespace TestTask
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<User> users = new ObservableCollection<User>();
        public ObservableCollection<User> Users { get { return users; } set { users = value; OnPropertyChanged("Users"); } }

        // Получает список пользователей и их статистики
        public void GetUsers()
        {
            var manager = new UsersManager();
            var usersData = from user in manager.LoadData()
                            orderby user.AvgSteps descending
                            select user;

            ObservableCollection<User> sortedUsersData = new ObservableCollection<User>(usersData);

            foreach (var item in sortedUsersData)
            {
                item.Rank = sortedUsersData.IndexOf(item) + 1;
                item.Status = "Finished";
                Users.Add(item);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Возвращает график статистики пользователя
        public LineSeries PrintGraph(int index)
        {
            List<DaySteps> daySteps = Users[index].Steps;

            var series = new LineSeries()
            {
                Title = $"{Users[index].Name}",
                Values = new ChartValues<int>(daySteps.Select(x => x.Steps)),
            };

            return series;
        }

        // Получает user, filePath и экспортирует при помощи метода ToJson
        public void ExportUserData(int index)
        {
            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Json files(*.json)|*.json";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;
                    User selectedUser = Users[index];

                    try
                    {
                        ToJson(filePath, selectedUser);
                    }
                    catch (Exception e)
                    {
                        System.Windows.MessageBox.Show(e.Message, "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else System.Windows.MessageBox.Show("Не удаётся сохранить файл", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Записывает user в file .json по пути filepath.
        public void ToJson(string filePath, User user)
            => File.WriteAllText(filePath, JsonConvert.SerializeObject(user, Formatting.Indented));
    }

    // Класс для выделения пользователей.
    class MultiBindingColorConventer : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (((int)values[1] > (int)values[0] * 1.2) || ((int)values[2] < (int)values[0] / 1.2))
                return new SolidColorBrush(Colors.Yellow);
            else
                return new SolidColorBrush(Colors.White);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
