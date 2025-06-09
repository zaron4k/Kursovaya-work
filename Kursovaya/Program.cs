using System;
using System.Collections.Generic;
using System.Linq;

namespace DogShow
{
    class Program
    {
        static void Main(string[] args)
        {
            ShowOrganizer organizer = new ShowOrganizer();

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
                Console.WriteLine("0. Выход");

                string input = Console.ReadLine();
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
                    case "0": return;
                    default: Console.WriteLine("Некорректный ввод."); break;
                }
            }
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

            // Добавляем экспертов
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

            // Добавляем собак
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

            organizer.Dogs.Add(new Dog { Id = id, Name = name, Breed = breed, Age = age, Club = club, OwnerFullName = owner });
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
                Console.WriteLine($"ID: {dog.Id}, Кличка: {dog.Name}, Порода: {dog.Breed}, Медаль: {(string.IsNullOrWhiteSpace(dog.Medal) ? "нет" : dog.Medal)}");
            }

            Console.WriteLine($"\nЭксперты клуба \"{club}\":");
            foreach (var expert in experts)
            {
                Console.WriteLine($"ID: {expert.Id}, ФИО: {expert.FullName}, Специализация: {expert.Specialization}, Ринг: {expert.RingId}");
            }
        }

        static void ShowClubMedalsMenu(ShowOrganizer organizer)
        {
            Console.Write("Введите название клуба: ");
            var club = Console.ReadLine();
            var dogs = organizer.Dogs.Where(d => d.Club.Equals(club, StringComparison.OrdinalIgnoreCase)).ToList();

            var medals = dogs.Where(d => !string.IsNullOrWhiteSpace(d.Medal)).ToList();
            Console.WriteLine($"\nМедали клуба \"{club}\":");
            foreach (var dog in medals)
            {
                Console.WriteLine($"Собака: {dog.Name}, Порода: {dog.Breed}, Медаль: {dog.Medal}");
            }

            Console.WriteLine($"Всего медалей: {medals.Count}");
        }

        static void ShowRingsByOwnerMenu(ShowOrganizer organizer)
        {
            Console.Write("Введите ФИО хозяина: ");
            var owner = Console.ReadLine();
            var dogs = organizer.Dogs.Where(d => d.OwnerFullName.Equals(owner, StringComparison.OrdinalIgnoreCase)).Select(d => d.Id).ToHashSet();

            var rings = organizer.Rings.Where(r => r.DogIds.Any(id => dogs.Contains(id))).ToList();
            Console.WriteLine($"\nРинги, в которых участвуют собаки хозяина {owner}:");
            foreach (var ring in rings)
            {
                Console.WriteLine($"Ринг ID: {ring.Id}, Название: {ring.Title}, Адрес: {ring.Address}");
            }
        }

        static void ShowRingInfoMenu(ShowOrganizer organizer)
        {
            Console.Write("Введите ID ринга: ");
            if (!int.TryParse(Console.ReadLine(), out int ringId))
            {
                Console.WriteLine("Некорректный ID.");
                return;
            }

            var ring = organizer.Rings.FirstOrDefault(r => r.Id == ringId);
            if (ring == null)
            {
                Console.WriteLine("Ринг не найден.");
                return;
            }

            Console.WriteLine($"\nРинг ID: {ring.Id}\nНазвание: {ring.Title}\nАдрес: {ring.Address}\nКлуб: {ring.Club}");
            Console.WriteLine("Специализации пород: " + string.Join(", ", ring.Specializations));

            var experts = organizer.Experts.Where(e => ring.ExpertIds.Contains(e.Id)).ToList();
            Console.WriteLine("\nЭксперты, обслуживающие ринг:");
            foreach (var expert in experts)
            {
                Console.WriteLine($"  ID: {expert.Id}, ФИО: {expert.FullName}, Специализация: {expert.Specialization}");
            }

            var dogs = organizer.Dogs.Where(d => ring.DogIds.Contains(d.Id)).ToList();
            Console.WriteLine("\nСобаки, участвующие в ринге:");
            foreach (var dog in dogs)
            {
                Console.WriteLine($"  ID: {dog.Id}, Кличка: {dog.Name}, Порода: {dog.Breed}, Хозяин: {dog.OwnerFullName}, Медаль: {(string.IsNullOrWhiteSpace(dog.Medal) ? "нет" : dog.Medal)}");
            }
        }
    }

    class Dog
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Breed { get; set; }
        public int Age { get; set; }
        public string Club { get; set; }
        public string OwnerFullName { get; set; }
        public string Medal { get; set; } = "";
    }

    class Expert
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Specialization { get; set; }
        public int RingId { get; set; }
        public string Club { get; set; }
    }

    class Ring
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Address { get; set; }
        public List<string> Specializations { get; set; }
        public string Club { get; set; }
        public List<int> ExpertIds { get; set; }
        public List<int> DogIds { get; set; }
    }

    class ShowOrganizer
    {
        public List<Dog> Dogs { get; } = new List<Dog>();
        public List<Expert> Experts { get; } = new List<Expert>();
        public List<Ring> Rings { get; } = new List<Ring>();
    }
}

