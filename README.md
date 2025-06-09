# Kursovaya-work
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp1
{
    public class Dog
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Breed { get; set; }
        public int Age { get; set; }
        public string Club { get; set; }
        public string OwnerFullName { get; set; }
        public string PedigreeDocNumber { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public DateTime LastVaccinationDate { get; set; }
        public string Medal { get; set; } // Золото / Серебро / Бронза / null

        // Метод для проверки наличия медали
        public bool HasMedal()
        {
            return !string.IsNullOrEmpty(Medal);
        }
    }

    public class Expert
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Specialization { get; set; }
        public int RingId { get; set; }
        public string Club { get; set; }
    }

    public class Ring
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Address { get; set; }
        public List<string> Specializations { get; set; } = new List<string>();
        public string Club { get; set; }
        public List<int> ExpertIds { get; set; } = new List<int>();
        public List<int> DogIds { get; set; } = new List<int>();
    }

    public class ShowOrganizer
    {
        public List<Dog> Dogs { get; set; } = new List<Dog>();
        public List<Expert> Experts { get; set; } = new List<Expert>();
        public List<Ring> Rings { get; set; } = new List<Ring>();

        public void AddDog(Dog dog)
        {
            Dogs.Add(dog);
        }

        public void AddExpert(Expert expert)
        {
            Experts.Add(expert);
        }

        public void AddDogToRing(int ringId, int dogId)
        {
            var ring = Rings.FirstOrDefault(r => r.Id == ringId);
            var dog = Dogs.FirstOrDefault(d => d.Id == dogId);
            if (ring == null)
            {
                Console.WriteLine("Ринг не найден.");
                return;
            }
            if (dog == null)
            {
                Console.WriteLine("Собака не найдена.");
                return;
            }
            if (!ring.Specializations.Contains(dog.Breed))
            {
                Console.WriteLine($"Порода собаки '{dog.Breed}' не подходит для этого ринга.");
                return;
            }
            if (!ring.DogIds.Contains(dogId))
            {
                ring.DogIds.Add(dogId);
                Console.WriteLine($"Собака '{dog.Name}' добавлена в ринг.");
            }
            else
            {
                Console.WriteLine("Собака уже присутствует в ринге.");
            }
        }

        public void AddExpertToRing(int ringId, int expertId)
        {
            var ring = Rings.FirstOrDefault(r => r.Id == ringId);
            var expert = Experts.FirstOrDefault(e => e.Id == expertId);
            if (ring == null)
            {
                Console.WriteLine("Ринг не найден.");
                return;
            }
            if (expert == null)
            {
                Console.WriteLine("Эксперт не найден.");
                return;
            }
            if (!ring.ExpertIds.Contains(expertId))
            {
                ring.ExpertIds.Add(expertId);
                Console.WriteLine($"Эксперт '{expert.FullName}' добавлен в ринг.");
            }
            else
            {
                Console.WriteLine("Эксперт уже присутствует в ринге.");
            }
        }

        public List<Ring> GetRingsByOwner(string ownerFullName)
        {
            var dogIds = Dogs.Where(d => d.OwnerFullName == ownerFullName).Select(d => d.Id).ToList();
            return Rings.Where(r => r.DogIds.Intersect(dogIds).Any()).ToList();
        }

        public void ShowRingInfo(int ringId)
        {
            var ring = Rings.FirstOrDefault(r => r.Id == ringId);
            if (ring == null)
            {
                Console.WriteLine("Ринг не найден.");
                return;
            }

            Console.WriteLine($"\nРинг ID: {ring.Id}");
            Console.WriteLine($"Название: {ring.Title}");
            Console.WriteLine($"Адрес: {ring.Address}");
            Console.WriteLine($"Клуб: {ring.Club}");
            Console.WriteLine("Специализации пород: " + string.Join(", ", ring.Specializations));

            var experts = Experts.Where(e => ring.ExpertIds.Contains(e.Id)).ToList();
            Console.WriteLine("\nЭксперты, обслуживающие ринг:");
            if (experts.Count == 0)
                Console.WriteLine("  Нет экспертов.");
            else
            {
                foreach (var expert in experts)
                {
                    Console.WriteLine($"  ID: {expert.Id}, ФИО: {expert.FullName}, Специализация: {expert.Specialization}");
                }
            }

            var dogs = Dogs.Where(d => ring.DogIds.Contains(d.Id)).ToList();
            Console.WriteLine("\nСобаки, участвующие в ринге:");
            if (dogs.Count == 0)
                Console.WriteLine("  Нет собак.");
            else
            {
                foreach (var dog in dogs)
                {
                    Console.WriteLine($"  ID: {dog.Id}, Кличка: {dog.Name}, Порода: {dog.Breed}, Владелец: {dog.OwnerFullName}, Медаль: {(dog.HasMedal() ? dog.Medal : "Нет медалей")}");
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var organizer = new ShowOrganizer();

            while (true)
            {
                Console.WriteLine("\n--- Главное меню ---");
                Console.WriteLine("1. Добавить ринг");
                Console.WriteLine("2. Добавить эксперта");
                Console.WriteLine("3. Добавить собаку");
                Console.WriteLine("4. Показать ринги по хозяину");
                Console.WriteLine("5. Назначить эксперта в ринг");
                Console.WriteLine("6. Назначить собаку в ринг");
                Console.WriteLine("7. Показать информацию о ринге");
                Console.WriteLine("0. Выход");
                Console.Write("Выберите действие: ");
                var input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        AddRingMenu(organizer);
                        break;
                    case "2":
                        AddExpertMenu(organizer);
                        break;
                    case "3":
                        AddDogMenu(organizer);
                        break;
                    case "4":
                        ShowRingsByOwnerMenu(organizer);
                        break;
                    case "5":
                        AssignExpertToRingMenu(organizer);
                        break;
                    case "6":
                        AssignDogToRingMenu(organizer);
                        break;
                    case "7":
                        ShowRingInfoMenu(organizer);
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Некорректный ввод.");
                        break;
                }
            }
        }

        static void AddRingMenu(ShowOrganizer organizer)
        {
            Console.Write("ID ринга: ");
            int id;
            while(!int.TryParse(Console.ReadLine(), out id))
            {
                Console.Write("Введите корректный ID ринга: ");
            }

            Console.Write("Название ринга: ");
            string title = Console.ReadLine();

            Console.Write("Адрес: ");
            string address = Console.ReadLine();

            Console.Write("Клуб: ");
            string club = Console.ReadLine();

            Console.Write("Специализации (породы через запятую): ");
            var specsInput = Console.ReadLine();
            var specs = specsInput.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();

            organizer.Rings.Add(new Ring
            {
                Id = id,
                Title = title,
                Address = address,
                Club = club,
                Specializations = specs
            });

            Console.WriteLine("Ринг добавлен.");
        }

        static void AddExpertMenu(ShowOrganizer organizer)
        {
            Console.Write("ID эксперта: ");
            int id;
            while (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.Write("Введите корректный ID эксперта: ");
            }

            Console.Write("ФИО эксперта: ");
            string name = Console.ReadLine();

            Console.Write("Специализация: ");
            string spec = Console.ReadLine();

            Console.Write("ID ринга: ");
            int ringId;
            while(!int.TryParse(Console.ReadLine(), out ringId))
            {
                Console.Write("Введите корректный ID ринга: ");
            }

            Console.Write("Клуб: ");
            string club = Console.ReadLine();

            var expert = new Expert
            {
                Id = id,
                FullName = name,
                Specialization = spec,
                RingId = ringId,
                Club = club
            };

            organizer.AddExpert(expert);
            Console.WriteLine("Эксперт добавлен.");
        }

        static void AddDogMenu(ShowOrganizer organizer)
        {
            Console.Write("ID собаки: ");
            int id;
            while (!int.TryParse(Console.ReadLine(), out id))
            {
                Console.Write("Введите корректный ID собаки: ");
            }

            Console.Write("Кличка: ");
            string name = Console.ReadLine();

            Console.Write("Порода: ");
            string breed = Console.ReadLine();

            Console.Write("Возраст: ");
            int age;
            while(!int.TryParse(Console.ReadLine(), out age))
            {
                Console.Write("Введите корректный возраст: ");
            }

            Console.Write("Клуб: ");
            string club = Console.ReadLine();

            Console.Write("ФИО хозяина: ");
            string owner = Console.ReadLine();

            Console.Write("Медаль (Золото/Серебро/Бронза или пусто): ");
            string medal = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(medal)) medal = null;

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

            organizer.AddDog(dog);
            Console.WriteLine("Собака добавлена.");
        }

        static void ShowRingsByOwnerMenu(ShowOrganizer organizer)
        {
            Console.Write("Введите ФИО хозяина: ");
            var owner = Console.ReadLine();

            var rings = organizer.GetRingsByOwner(owner);

            if (rings.Count == 0)
            {
                Console.WriteLine("Ринги для данного хозяина не найдены.");
            }
            else
            {
                Console.WriteLine($"\nРинги, в которых участвуют собаки хозяина {owner}:");
                foreach (var ring in rings)
                {
                    Console.WriteLine($"  ID: {ring.Id}, Название: {ring.Title}, Адрес: {ring.Address}");
                }
            }
        }

        static void AssignExpertToRingMenu(ShowOrganizer organizer)
        {
            Console.Write("ID ринга: ");
            int ringId;
            while(!int.TryParse(Console.ReadLine(), out ringId))
            {
                Console.Write("Введите корректный ID ринга: ");
            }

            Console.Write("ID эксперта: ");
            int expertId;
            while(!int.TryParse(Console.ReadLine(), out expertId))
            {
                Console.Write("Введите корректный ID эксперта: ");
            }

            organizer.AddExpertToRing(ringId, expertId);
        }

        static void AssignDogToRingMenu(ShowOrganizer organizer)
        {
            Console.Write("ID ринга: ");
            int ringId;
            while(!int.TryParse(Console.ReadLine(), out ringId))
            {
                Console.Write("Введите корректный ID ринга: ");
            }

            Console.Write("ID собаки: ");
            int dogId;
            while(!int.TryParse(Console.ReadLine(), out dogId))
            {
                Console.Write("Введите корректный ID собаки: ");
            }

            organizer.AddDogToRing(ringId, dogId);
        }

        static void ShowRingInfoMenu(ShowOrganizer organizer)
        {
            Console.Write("ID ринга: ");
            int ringId;
            while(!int.TryParse(Console.ReadLine(), out ringId))
            {
                Console.Write("Введите корректный ID ринга: ");
            }

            organizer.ShowRingInfo(ringId);
        }
    }
}
