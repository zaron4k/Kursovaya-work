using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using System.Text;
using System.IO;
namespace DogShow
{
    public class Program
    {
        static ShowOrganizer organizer;

        static void Main(string[] args)
        {
            organizer = new ShowOrganizer();
            organizer.LoadData();
            organizer.SaveData();
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;

            Console.WriteLine("Работайте с программой. Для выхода выберите пункт 0.");
            
            while (true)
            {
                Console.WriteLine("\n--- Главное меню ---");
                Console.WriteLine("1. Добавить ринг");
                Console.WriteLine("2. Добавить эксперта");
                Console.WriteLine("3. Добавить собаку");
                Console.WriteLine("4. Показать ринги по хозяину");
                Console.WriteLine("5. Назначить эксперта или собаку в ринг");
                Console.WriteLine("6. Показать медали клуба (собаки и статистика)");
                Console.WriteLine("7. Показать собак клуба (ID, кличка, порода и экспертов)");
                Console.WriteLine("8. Вывести информацию о ринге");
                Console.WriteLine("9. Сохранить данные вручную");
                Console.WriteLine("0. Выход");
                Console.Write("Выберите пункт: ");
                ConsoleKeyInfo key = Console.ReadKey();
                Console.WriteLine(); // для переноса строки после ввода

                string input = key.KeyChar.ToString();


                switch (input)
                {
                    case "1": AddRingMenu(organizer); break;
                    case "2": AddExpertMenu(organizer); break;
                    case "3": AddDogMenu(organizer); break;
                    case "4": ShowRingsByOwnerMenu(organizer); break;
                    case "5": AssignToRingMenu(organizer); break;
                    case "6": ShowClubMedalsMenu(organizer); break;
                    case "7": ShowClubDogsMenu(organizer); break;
                    case "8": ShowRingInfoMenu(organizer); break;
                    case "9":
                        organizer.SaveData();
                        Console.WriteLine("Данные сохранены вручную (по пункту 9).");
                        break;

                    case "0":
                        Console.WriteLine("Выход из программы...");
                        organizer.SaveData();
                        return;

                 
                }
        }
        }

        private static void OnProcessExit(object sender, EventArgs e)
        {
            Console.WriteLine("Сохраняем данные перед выходом...");
            organizer.SaveData();
            Console.WriteLine("Данные сохранены.");
        }

        static void AddRingMenu(ShowOrganizer organizer)
        {
            Console.Write("ID ринга: ");
            int id = int.Parse(Console.ReadLine());
            Console.Write("Название ринга: ");
            string title = Console.ReadLine();
            Console.Write("Адрес: ");
            string address = Console.ReadLine();
            Console.Write("Клуб: ");
            string club = Console.ReadLine();
            Console.Write("Специализации (породы через запятую): ");
            List<string> specializations = Console.ReadLine().Split(',').Select(p => p.Trim()).ToList();

            var ring = new Ring
            {
                Id = id,
                Title = title,
                Address = address,
                Club = club,
                Specializations = specializations,
                ExpertIds = new List<int>(),
                DogIds = new List<int>()
            };

            Console.Write("Введите ID экспертов для добавления в ринг (через запятую), или пустую строку для пропуска: ");
            string expertsInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(expertsInput))
            {
                var expertIds = expertsInput.Split(',').Select(s => s.Trim()).Where(s => int.TryParse(s, out _)).Select(int.Parse).ToList();
                foreach (var expertId in expertIds)
                {
                    var expert = organizer.Experts.FirstOrDefault(e => e.Id == expertId);
                    if (expert != null)
                    {
                        ring.ExpertIds.Add(expertId);
                    }
                    else
                    {
                        Console.WriteLine($"Эксперт с ID {expertId} не найден и не добавлен.");
                    }
                }
            }

            Console.Write("Введите ID собак для добавления в ринг (через запятую), или пустую строку для пропуска: ");
            string dogsInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(dogsInput))
            {
                var dogIds = dogsInput.Split(',').Select(s => s.Trim()).Where(s => int.TryParse(s, out _)).Select(int.Parse).ToList();
                foreach (var dogId in dogIds)
                {
                    var dog = organizer.Dogs.FirstOrDefault(d => d.Id == dogId);
                    if (dog != null)
                    {
                        if (ring.Specializations.Contains(dog.Breed))
                        {
                            ring.DogIds.Add(dogId);
                        }
                        else
                        {
                            Console.WriteLine($"Собака с ID {dogId} имеет породу {dog.Breed}, которая не подходит рингу.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Собака с ID {dogId} не найдена и не добавлена.");
                    }
                }
            }

            organizer.Rings.Add(ring);

            Console.WriteLine("Ринг добавлен.");
        }

        static void AddExpertMenu(ShowOrganizer organizer)
        {
            Console.Write("ID эксперта: ");
            int id = int.Parse(Console.ReadLine());
            Console.Write("ФИО эксперта: ");
            string name = Console.ReadLine();
            Console.Write("Специализация: ");
            string spec = Console.ReadLine();
            Console.Write("ID ринга: ");
            int ringId = int.Parse(Console.ReadLine());
            Console.Write("Клуб: ");
            string club = Console.ReadLine();

            organizer.Experts.Add(new Expert { Id = id, FullName = name, Specialization = spec, RingId = ringId, Club = club });

            Console.WriteLine("Эксперт добавлен.");
        }

        static void AddDogMenu(ShowOrganizer organizer)
        {
            Console.Write("ID собаки: ");
            int id = int.Parse(Console.ReadLine());
            Console.Write("Кличка: ");
            string name = Console.ReadLine();
            Console.Write("Порода: ");
            string breed = Console.ReadLine();
            Console.Write("Возраст: ");
            int age = int.Parse(Console.ReadLine());
            Console.Write("Клуб: ");
            string club = Console.ReadLine();
            Console.Write("ФИО хозяина: ");
            string owner = Console.ReadLine();

            Console.Write("Есть ли у собаки медаль? (да/нет): ");
            string hasMedal = Console.ReadLine().Trim().ToLower();
            string medal = "";
            if (hasMedal == "да")
            {
                Console.Write("Введите тип медали (Золото/Серебро/Бронза): ");
                medal = Console.ReadLine().Trim();
            }

            var dog = new Dog
            {
                Id = id,
                Name = name,
                Breed = breed,
                Age = age,
                Club = club,
                OwnerFullName = owner,
                Medal = medal
            };

            organizer.Dogs.Add(dog);

            Console.WriteLine("Собака добавлена.");
        }

        static void AssignToRingMenu(ShowOrganizer organizer)
        {
            Console.Write("ID ринга: ");
            int ringId = int.Parse(Console.ReadLine());
            var ring = organizer.Rings.FirstOrDefault(r => r.Id == ringId);
            if (ring == null)
            {
                Console.WriteLine("Ринг не найден.");
                return;
            }

            Console.Write("Введите тип (expert/dog): ");
            string type = Console.ReadLine();

            if (type == "expert")
            {
                Console.Write("ID эксперта: ");
                int expertId = int.Parse(Console.ReadLine());
                if (!ring.ExpertIds.Contains(expertId))
                {
                    ring.ExpertIds.Add(expertId);
                    Console.WriteLine("Эксперт добавлен в ринг.");
                }
                else
                {
                    Console.WriteLine("Эксперт уже добавлен в ринг.");
                }
            }
            else if (type == "dog")
            {
                Console.Write("ID собаки: ");
                int dogId = int.Parse(Console.ReadLine());
                var dog = organizer.Dogs.FirstOrDefault(d => d.Id == dogId);
                if (dog == null || !ring.Specializations.Contains(dog.Breed))
                {
                    Console.WriteLine("Собака не найдена или её порода не подходит рингу.");
                    return;
                }
                if (!ring.DogIds.Contains(dogId))
                {
                    ring.DogIds.Add(dogId);
                    Console.WriteLine("Собака добавлена в ринг.");
                }
                else
                {
                    Console.WriteLine("Собака уже добавлена в ринг.");
                }
            }
            else
            {
                Console.WriteLine("Неверный тип.");
            }
        }

        static void ShowClubDogsMenu(ShowOrganizer organizer)
        {
            Console.Write("Введите название клуба: ");
            var club = Console.ReadLine();

            var dogs = organizer.Dogs.Where(d => d.Club.Equals(club, StringComparison.OrdinalIgnoreCase)).ToList();
            var experts = organizer.Experts.Where(e => e.Club.Equals(club, StringComparison.OrdinalIgnoreCase)).ToList();

            Console.WriteLine($"\nСобаки клуба \"{club}\":");
            foreach (var dog in dogs)
            {
                Console.WriteLine($"ID: {dog.Id}, Кличка: {dog.Name}, Порода: {dog.Breed}, Медаль: {(string.IsNullOrEmpty(dog.Medal) ? "Нет" : dog.Medal)}");
                var dogExperts = organizer.Experts.Where(e => organizer.Rings.Any(r => r.ExpertIds.Contains(e.Id) && r.DogIds.Contains(dog.Id))).ToList();
                if (dogExperts.Any())
                {
                    Console.WriteLine("  Эксперты, связанные с этой собакой:");
                    foreach (var expert in dogExperts)
                        Console.WriteLine($"   - {expert.FullName}");
                }
            }

            Console.WriteLine($"\nЭксперты клуба \"{club}\":");
            foreach (var expert in experts)
            {
                Console.WriteLine($"ID: {expert.Id}, ФИО: {expert.FullName}, Специализация: {expert.Specialization}");
            }
        }

        static void ShowClubMedalsMenu(ShowOrganizer organizer)
        {
            Console.Write("Введите название клуба: ");
            var club = Console.ReadLine();

            var dogs = organizer.Dogs.Where(d => d.Club.Equals(club, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrEmpty(d.Medal)).ToList();

            var goldCount = dogs.Count(d => d.Medal.Equals("Золото", StringComparison.OrdinalIgnoreCase));
            var silverCount = dogs.Count(d => d.Medal.Equals("Серебро", StringComparison.OrdinalIgnoreCase));
            var bronzeCount = dogs.Count(d => d.Medal.Equals("Бронза", StringComparison.OrdinalIgnoreCase));

            Console.WriteLine($"Медали клуба \"{club}\": Золото: {goldCount}, Серебро: {silverCount}, Бронза: {bronzeCount}");
        }

        static void ShowRingsByOwnerMenu(ShowOrganizer organizer)
        {
            Console.Write("Введите ФИО хозяина: ");
            var owner = Console.ReadLine();

            var rings = organizer.Rings.Where(r => r.DogIds.Any(dogId =>
            {
                var dog = organizer.Dogs.FirstOrDefault(d => d.Id == dogId);
                return dog != null && dog.OwnerFullName.Equals(owner, StringComparison.OrdinalIgnoreCase);
            })).ToList();

            Console.WriteLine($"\nРинги для хозяина \"{owner}\":");
            foreach (var ring in rings)
            {
                Console.WriteLine($"ID: {ring.Id}, Название: {ring.Title}, Адрес: {ring.Address}");
            }
        }

        static void ShowRingInfoMenu(ShowOrganizer organizer)
        {
            Console.Write("Введите ID ринга: ");
            int id = int.Parse(Console.ReadLine());

            var ring = organizer.Rings.FirstOrDefault(r => r.Id == id);
            if (ring == null)
            {
                Console.WriteLine("Ринг не найден.");
                return;
            }

            Console.WriteLine($"Ринг ID: {ring.Id}");
            Console.WriteLine($"Название: {ring.Title}");
            Console.WriteLine($"Адрес: {ring.Address}");
            Console.WriteLine($"Клуб: {ring.Club}");
            Console.WriteLine("Специализации: " + string.Join(", ", ring.Specializations));
            Console.WriteLine("Эксперты:");
            foreach (var expertId in ring.ExpertIds)
            {
                var expert = organizer.Experts.FirstOrDefault(e => e.Id == expertId);
                if (expert != null)
                {
                    Console.WriteLine($" - {expert.FullName} ({expert.Specialization})");
                }
            }
            Console.WriteLine("Собаки:");
            foreach (var dogId in ring.DogIds)
            {
                var dog = organizer.Dogs.FirstOrDefault(d => d.Id == dogId);
                if (dog != null)
                {
                    Console.WriteLine($" - {dog.Name} ({dog.Breed})");
                }
            }
        }
        public class ShowOrganizer
        {
            public string Club { get; set; } = "";

            public List<Ring> Rings { get; set; } = new List<Ring>();
            public List<Expert> Experts { get; set; } = new List<Expert>();
            public List<Dog> Dogs { get; set; } = new List<Dog>();

            private const string ClubFile = @"D:\DI\club.txt";
            private const string RingsFile = @"D:\DI\rings.txt";
            private const string ExpertsFile = @"D:\DI\experts.txt";
            private const string DogsFile = @"D:\DI\dogs.txt";

            public void SaveData()
            {
                SaveClub();
                SaveRings();
                SaveExperts();
                SaveDogs();
            }

            public void LoadData()
            {
                LoadClub();
                LoadRings();
                LoadExperts();
                LoadDogs();
            }

            private void SaveClub()
            {
                File.WriteAllText(ClubFile, Club ?? "", Encoding.UTF8);
            }

            private void LoadClub()
            {
                if (File.Exists(ClubFile))
                    Club = File.ReadAllText(ClubFile, Encoding.UTF8);
                else
                    Club = "";
            }

            // --- Далее без изменений, как и раньше ---

            private void SaveRings()
            {
                StringBuilder sb = new StringBuilder();
                foreach (var ring in Rings)
                {
                    sb.AppendLine($"{ring.Id}|{ring.Title}|{ring.Address}|{ring.Club}|{string.Join(",", ring.Specializations)}|{string.Join(",", ring.ExpertIds)}|{string.Join(",", ring.DogIds)}");
                }
                File.WriteAllText(RingsFile, sb.ToString(), Encoding.UTF8);
            }

            private void LoadRings()
            {
                Rings.Clear();
                if (!File.Exists(RingsFile)) return;
                var lines = File.ReadAllLines(RingsFile, Encoding.UTF8);
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    var parts = line.Split('|');
                    var ring = new Ring
                    {
                        Id = int.Parse(parts[0]),
                        Title = parts[1],
                        Address = parts[2],
                        Club = parts[3],
                        Specializations = parts[4].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList(),
                        ExpertIds = parts[5].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList(),
                        DogIds = parts[6].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList()
                    };
                    Rings.Add(ring);
                }
            }

            private void SaveExperts()
            {
                StringBuilder sb = new StringBuilder();
                foreach (var expert in Experts)
                {
                    sb.AppendLine($"{expert.Id}|{expert.FullName}|{expert.Specialization}|{expert.RingId}|{expert.Club}");
                }
                File.WriteAllText(ExpertsFile, sb.ToString(), Encoding.UTF8);
            }

            private void LoadExperts()
            {
                Experts.Clear();
                if (!File.Exists(ExpertsFile)) return;
                var lines = File.ReadAllLines(ExpertsFile, Encoding.UTF8);
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    var parts = line.Split('|');
                    var expert = new Expert
                    {
                        Id = int.Parse(parts[0]),
                        FullName = parts[1],
                        Specialization = parts[2],
                        RingId = int.Parse(parts[3]),
                        Club = parts[4]
                    };
                    Experts.Add(expert);
                }
            }

            private void SaveDogs()
            {
                StringBuilder sb = new StringBuilder();
                foreach (var dog in Dogs)
                {
                    sb.AppendLine($"{dog.Id}|{dog.Name}|{dog.Breed}|{dog.Age}|{dog.Club}|{dog.OwnerFullName}|{dog.Medal}");
                }
                File.WriteAllText(DogsFile, sb.ToString(), Encoding.UTF8);
            }

            private void LoadDogs()
            {
                Dogs.Clear();
                if (!File.Exists(DogsFile)) return;
                var lines = File.ReadAllLines(DogsFile, Encoding.UTF8);
                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    var parts = line.Split('|');
                    var dog = new Dog
                    {
                        Id = int.Parse(parts[0]),
                        Name = parts[1],
                        Breed = parts[2],
                        Age = int.Parse(parts[3]),
                        Club = parts[4],
                        OwnerFullName = parts[5],
                        Medal = parts[6]
                    };
                    Dogs.Add(dog);
                }
            }
        }

        public class Ring
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Address { get; set; }
            public string Club { get; set; }
            public List<string> Specializations { get; set; }
            public List<int> ExpertIds { get; set; }
            public List<int> DogIds { get; set; }
        }

        public class Expert
        {
            public int Id { get; set; }
            public string FullName { get; set; }
            public string Specialization { get; set; }
            public int RingId { get; set; }
            public string Club { get; set; }
        }

        public class Dog
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Breed { get; set; }
            public int Age { get; set; }
            public string Club { get; set; }
            public string OwnerFullName { get; set; }
            public string Medal { get; set; }
        }
      
    }
}
