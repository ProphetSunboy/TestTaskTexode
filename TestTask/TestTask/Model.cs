using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace TestTask
{
    // Класс полученной информации из .json файла
    internal class Data
    {
        public string User { get; }
        public int Steps { get; }
        public int Day { get; private set; }
        public void SetDay(int day) => Day = day;

        public Data(string user, int steps)
            => (User, Steps) = (user, steps);
    }

    // Класс отвечает за количество шагов в день
    internal class DaySteps
    {
        public int Day { get; }
        public int Steps { get; }

        public DaySteps(int day, int steps) => (Day, Steps) = (day, steps);
    }

    // Класс отвечает за информацию о пользователе
    internal class User
    {
        public int Rank { get; set; }
        public string Name { get; }
        public string Status { get; set; }
        [JsonIgnore]
        public List<DaySteps> Steps { get; }
        public User(string name) => (Name, Steps) = (name, new List<DaySteps>());
        public bool HasADay(int day) => Steps.Any(x => x.Day == day);

        public int AvgSteps { get { return (int)Steps.Select(x => x.Steps).Average(); } }
        public int MaxSteps { get { return Steps.Select(x => x.Steps).Max(); } }
        public int MinSteps { get { return (from s in Steps where s.Steps != 0 select s.Steps).Min(); } }
    }

    // Отвечает за преобразование .json файлов в класс User
    class UsersManager
    {
        // Преобразует .json в Data
        private IEnumerable<Data> GetData()
        {
            using (var folderBrowserDialog = new FolderBrowserDialog())
            {
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    int count = 1;
                    while (true)
                    {
                        string data;
                        string filename = folderBrowserDialog.SelectedPath + $@"\day{count}.json";
                        try
                        {
                            data = File.ReadAllText(filename);
                        }
                        catch (FileNotFoundException)
                        {
                            break;
                        }

                        var result = JsonConvert.DeserializeObject<Data[]>(data);

                        foreach (var item in result)
                        {
                            item.SetDay(count);
                            yield return item;
                        }

                        count++;
                    }
                }
                else MessageBox.Show("Не удаётся открыть папку", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Преобразует Data в User
        public List<User> LoadData()
        {
            List<User> users = new List<User>();
            foreach (var data in GetData())
            {
                if (!users.Any(x => x.Name == data.User))
                {
                    var user = new User(data.User);
                    user.Steps.Add(new DaySteps(data.Day, data.Steps));
                    users.Add(user);
                }
                else
                {
                    var user = users.FirstOrDefault(x => x.Name == data.User);
                    user.Steps.Add(new DaySteps(data.Day, data.Steps));
                }
            }

            return users;
        }
    }
}
