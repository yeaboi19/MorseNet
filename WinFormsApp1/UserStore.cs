using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace WinFormsApp1 {
    internal class UserStore {
        private static readonly string Filepath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "users.json"
            );

        private record UserEntry(string username, string passwordHash);

        private static List<UserEntry> LoadAll() {
            if (!File.Exists(Filepath)) return new List<UserEntry>();
            try {
                string json = File.ReadAllText(Filepath);
                return JsonSerializer.Deserialize<List<UserEntry>>(json);

            } catch {
                return new List<UserEntry>();
            }
        }


        private static void SaveAll(List<UserEntry> list) {
            string json = JsonSerializer.Serialize(list,
                new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(Filepath, json);
        }

        private static string Hash(string input) {
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexString(bytes);
        }

        public static bool Authenticate(string username, string password) {
            string hash = Hash(password);
            return LoadAll().Any(u =>
                u.username.Equals(username, StringComparison.OrdinalIgnoreCase) &&
                u.passwordHash == hash
            );
        }

        public static bool CreateUser(string username, string password) {
            if (UserExists(username)) return false;
            var users = LoadAll();
            users.Add(new UserEntry(username.Trim(), Hash(password)));
            SaveAll(users);
            return true;
        }

        public static bool UserExists(string username) => LoadAll().Any(u => u.username.Equals(username, StringComparison.OrdinalIgnoreCase));

        public static bool HasAnyUsers() => LoadAll().Count() > 0;
    }
}

