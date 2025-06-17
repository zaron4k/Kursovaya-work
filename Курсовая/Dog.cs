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
    }

    // Класс "Эксперт"
    public class Expert
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Specialization { get; set; }
        public int RingId { get; set; }
        public string Club { get; set; }
    }

    // Класс "Ринг"
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

    // Основной класс управления
    public class ShowOrganizer
    {
        public List<Dog> Dogs { get; set; } = new List<Dog>();
        public List<Expert> Experts { get; set; } = new List<Expert>();
        public List<Ring> Rings { get; set; } = new List<Ring>();

        // Добавить новую собаку
        public void AddDog(Dog dog)
        {
            Dogs.Add(dog);
        }

        // Добавить нового эксперта
        public void AddExpert(Expert expert)
        {
            Experts.Add(expert);
        }

        // Удалить эксперта и заменить
        public void ReplaceExpert(int oldExpertId, Expert newExpert)
        {
            var expert = Experts.FirstOrDefault(e => e.Id == oldExpertId);
            if (expert != null)
            {
                Experts.Remove(expert);
                AddExpert(newExpert);
            }
        }

        // Отстранить собаку
        public void RemoveDog(int dogId)
        {
            var dog = Dogs.FirstOrDefault(d => d.Id == dogId);
            if (dog != null)
            {
                Dogs.Remove(dog);
                foreach (var ring in Rings)
                {
                    ring.DogIds.Remove(dogId);
                }
            }
        }

        // 🔎 Запросы:

        // На каком ринге выступает заданный хозяин
        public List<Ring> GetRingsByOwner(string ownerFullName)
        {
            var dogIds = Dogs.Where(d => d.OwnerFullName == ownerFullName).Select(d => d.Id).ToList();
            return Rings.Where(r => r.DogIds.Intersect(dogIds).Any()).ToList();
        }

        // Породы, представленные клубом
        public List<string> GetBreedsByClub(string club)
        {
            return Dogs.Where(d => d.Club == club).Select(d => d.Breed).Distinct().ToList();
        }

        // Сколько медалей у клуба
        public Dictionary<string, int> GetMedalsByClub(string club)
        {
            return Dogs
                .Where(d => d.Club == club && !string.IsNullOrEmpty(d.Medal))
                .GroupBy(d => d.Medal)
                .ToDictionary(g => g.Key, g => g.Count());
        }

        // Эксперты по породе
        public List<Expert> GetExpertsByBreed(string breed)
        {
            return Experts.Where(e => e.Specialization == breed).ToList();
        }

        // Специализации рингов
        public Dictionary<string, List<string>> GetRingSpecializations()
        {
            return Rings.ToDictionary(r => r.Title, r => r.Specializations);
        }

        // Рекордсмены по породам
        public Dictionary<string, Dog> GetTopDogsByBreed()
        {
            return Dogs
                .Where(d => !string.IsNullOrEmpty(d.Medal))
                .GroupBy(d => d.Breed)
                .ToDictionary(
                    g => g.Key,
                    g => g.OrderByDescending(d => d.Medal == "Золото" ? 3 : d.Medal == "Серебро" ? 2 : 1).First()
                );
        }
    }
}