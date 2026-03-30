using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace financial_planner.Models
{
    public static class DataStorage
    {
        private static string _dataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "FinancialPlanner",
            "users.txt"
        );

        static DataStorage()
        {
            string directory = Path.GetDirectoryName(_dataPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        public static void SaveUsers(List<User> users)
        {
            List<string> lines = new List<string>();

            foreach (var user in users)
            {
                string line = $"{user.Id}|{user.Username}|{user.Password}|{user.Email}|{user.FullName}|{user.RegistrationDate}";
                lines.Add(line);
            }

            File.WriteAllLines(_dataPath, lines);
        }

        public static List<User> LoadUsers()
        {
            List<User> users = new List<User>();

            if (File.Exists(_dataPath))
            {
                var lines = File.ReadAllLines(_dataPath);
                foreach (var line in lines)
                {
                    var parts = line.Split('|');
                    if (parts.Length == 6)
                    {
                        users.Add(new User
                        {
                            Id = int.Parse(parts[0]),
                            Username = parts[1],
                            Password = parts[2],
                            Email = parts[3],
                            FullName = parts[4],
                            RegistrationDate = DateTime.Parse(parts[5])
                        });
                    }
                }
            }

            return users;
        }
    }
}