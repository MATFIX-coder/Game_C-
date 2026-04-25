using System;
using ShahtyorGame.enums;

namespace ShahtyorGame.classes
{
    public partial class Game
    {
        public int SizeMap { get; private set; } //размер карты
        public CellType[,] Map { get; private set; } //заполняемость карты
        public bool[,] DetectedPlace { get; private set; } //отображение скрыта ли место на поле
        public Player Player { get; private set; } //игрок
        public int CurrentLevel { get; private set; } //текущий левел
        public string ActionMessage { get; set; } //свойства для метки события
        private Random random = new Random(); // генератор случайных чисел
        public bool SteppedOnMine { get; set; } // флаг игрок наступил на мину
        public bool[,] CollectedArtifacts { get; private set; } // клетки где артефакт уже собран
        // список всех анаграмм
        private (string anagram, string correct, string hint)[] anagrams =
        {
            ("АКМЯИ", "МЫШКА", "Маленький грызун"),
            ("НОСЛЕЦ", "СОЛНЦЕ", "Светит днём"),
            ("АГНИК", "КНИГА", "Источник знаний"),
            ("РОБАСТА", "РАБОТА", "То, что делаешь каждый день"),
            ("ТОМОЛОК", "МОЛОТОК", "Инструмент для забивания"),
        };

        public Game(int size = 6) //констурктор игры
        {
            SizeMap = size;
            Map = new CellType[SizeMap, SizeMap];
            CollectedArtifacts = new bool[SizeMap, SizeMap];
            DetectedPlace = new bool[SizeMap, SizeMap];
            Player = new Player();
            CurrentLevel = 1;
            ActionMessage = "Игра началась";
            GenerateLevel();
        }

        // берём случайную анаграмму из списка
        public (string anagram, string correct, string hint) GetRandomAnagram()
        {
            int index = random.Next(anagrams.Length); // случайный индекс
            return anagrams[index]; // возвращаем данные для анаграммы
        }
    }
}