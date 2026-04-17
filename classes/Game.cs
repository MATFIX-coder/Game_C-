using System;
using ShahtyorGame.enums;

namespace ShahtyorGame.classes
{
    public class Game
    {
        public int SizeMap {get; private set; } //размер карты
        public CellType[,] Map {get; private set; } //заполняемость карты
        public bool[,] DetectedPlace {get; private set;} //отображение скрыта ли место на поле
        public Player Player {get; private set; } //игрок
        public int CurrentLevel {get; private set; } //текущий левел
        public string ActionMessage { get; set; } //свойства для метки события
        private Random random = new Random(); // генератор случайных чисел
        public bool SteppedOnMine { get; set; } // флаг игрок наступил на мину

        // список всех анаграмм
        private (string anagram, string correct, string hint)[] anagrams =
        {
            ("АКМЯИ", "МЫШКА", "Маленький грызун"),
            ("НОСЛЕЦ", "СОЛНЦЕ", "Светит днём"),
            ("АГНИК", "КНИГА", "Источник знаний"),
            ("РОБАСТА", "РАБОТА", "То, что делаешь каждый день"),
            ("ТОМОЛОК", "МОЛОТОК", "Инструмент для забивания"),
        };

        // берём случайную анаграмму из списка    
        public (string anagram, string correct, string hint) GetRandomAnagram()
        {
            int index = random.Next(anagrams.Length); // случайный индекс
            return anagrams[index]; // возвращаем данные для анаграммы
        }

        public Game(int size = 6) //констурктор игры
        {
            SizeMap = size; 
            Map = new CellType[SizeMap, SizeMap];
            DetectedPlace = new bool[SizeMap, SizeMap];
            Player = new Player();
            CurrentLevel = 1;
            ActionMessage = "Игра началась";
            GenerateLevel();
        }

        private void GenerateLevel() //генерация уровня
        {
            for (int i = 0; i < SizeMap; i++) //отчищаю поле после прошлого
            {
                for (int j = 0; j < SizeMap; j++)
                {
                    Map[i, j] = CellType.EmptyPoint;
                    DetectedPlace[i, j] = false;
                }
            }
            int artifactsCount = 3; // количество артефактов на уровне
            int setArtifactsCount = 0; //счетчик установленных

            while (setArtifactsCount < artifactsCount) //цикл установки
            {
                int x = random.Next(SizeMap);
                int y = random.Next(SizeMap);

                if ((x == 0 && y == 0) || Map[x, y] != CellType.EmptyPoint) //проверка что это не начальная клетка и там не занято другим
                    continue;
                
                Map[x, y] = CellType.Artifact; //устанавливаю
                setArtifactsCount++;
            }

            int minesCount = 2;
            int setMinesCount = 0;

            while (setMinesCount < minesCount)
            {
                int x = random.Next(SizeMap);
                int y = random.Next(SizeMap);

                // Нельзя ставить мину на старт, артефакт или другую мину
                if ((x == 0 && y == 0) || Map[x, y] != CellType.EmptyPoint)
                    continue;

                Map[x, y] = CellType.Mine;
                setMinesCount++;
            }

            int pitsCount = 2;
            int setPitsCount = 0;

            while (setPitsCount < pitsCount)
            {
                int x = random.Next(SizeMap);
                int y = random.Next(SizeMap);

                if ((x == 0 && y == 0) || Map[x, y] != CellType.EmptyPoint)
                    continue;

                Map[x, y] = CellType.Pit;
                setPitsCount++;
            }

            DetectedPlace[0, 0] = true; //открываю начальную позицию
        }

        public bool TryMove(int goX, int goY) //возможно движение?
        {
            int oldX = Player.X;
            int oldY = Player.Y;

            int newX = Player.X + goX;
            int newY = Player.Y + goY;

            if (newX < 0 || newY < 0 || newX >= SizeMap || newY >= SizeMap) //если не выходи за рамки
                return false;

            if (!Player.HasPickaxe()) //если кирка еще не сломана
                return false;

            Player.X = newX;
            Player.Y = newY;
            DetectedPlace[newX, newY] = true;

            CellEffect(oldX, oldY, newX, newY); //реакция на клетку перемещение
            return true;
        }

        private void CellEffect(int oldX, int oldY, int newX, int newY) // что происходит когда игрок наступает на клетку
        {
            switch(Map[newX, newY])
            {
                case CellType.Artifact: //реакция на артифакт
                    if (Player.PickaxeStrength >= 15)
                    {
                        Player.PickaxeStrength -= 15;
                        Player.Coins += 10;
                        Map[newX, newY] = CellType.EmptyPoint;
                        ActionMessage = "Артефакт добыт: +10 монет";
                    }
                    else
                    {
                        Player.UsePickaxe(); //если не хватает прочности, то просто переходим туда
                        ActionMessage = "Найден артефакт, но кирка уже слишком сломанна";
                    }
                    break;

                case CellType.Mine:
                    SteppedOnMine = true; //наступили на мину
                    ActionMessage = "⚠️ Мина!";
                    break;

                case CellType.Pit:
                    DetectedPlace[newX, newY] = true;

                    Player.X = oldX; //отбрасываем назад
                    Player.Y = oldY;

                    Player.Health /= 2; //текущее здоровье режем пополам

                    Player.MaxHealth -= 10; //максимум здоровья уменьшаем на 10
                    if (Player.MaxHealth < 20)
                        Player.MaxHealth = 20;

                    if (Player.Health > Player.MaxHealth)
                        Player.Health = Player.MaxHealth;

                    ActionMessage = "🕳 Пропасть! -50% здоровья и -10 к максимуму";
                    break;

                case CellType.EmptyPoint: //если пустая клетка(обычная), то тратится одна прочность
                    Player.UsePickaxe();
                    ActionMessage = "Пустая клетка";
                    break;
            }
        }

        public void DefuseMine() //обезвреживаем мину
        {
            Map[Player.X, Player.Y] = CellType.EmptyPoint;
        }

        public bool IsPlayerNearPit()
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    if (dx == 0 && dy == 0)
                        continue;

                    int nx = Player.X + dx;
                    int ny = Player.Y + dy;

                    if (nx < 0 || ny < 0 || nx >= SizeMap || ny >= SizeMap)
                        continue;

                    if (Map[nx, ny] == CellType.Pit)
                        return true;
                }
            }

            return false;
        }

        public bool CollectedAllArtifacts() //проверка, что собраны все артифакты(победа)
        {
            for (int i = 0; i < SizeMap; i++)
            {
                for (int j = 0; j < SizeMap; j++)
                {
                    if (Map[i, j] == CellType.Artifact)
                        return false;
                }
            }
            return true;
        }

        public void NextLevel() //оконачание уровня, сброс
        {
            Player.ResetPosition(); //сброс позиции
            CurrentLevel++; //левел некст
            Player.PickaxeStrength = 100; //обновляю кирку
            ActionMessage = "Начался новый уровень";
            GenerateLevel(); //запуск левела
        }

        public int CountMinesAround(int x, int y) //считаем количество мин вокруг клетки
        {
            int count = 0;

            for (int dx = -1; dx <= 1; dx++) // перебираем смещения по строке
            {
                for (int dy = -1; dy <= 1; dy++) // перебираем смещения по столбцу
                {
                    if (dx == 0 && dy == 0) continue; // пропускаем саму клетку

                    int nx = x + dx; // координата соседа
                    int ny = y + dy;

                    // проверяем что не вышли за границы поля
                    if (nx < 0 || ny < 0 || nx >= SizeMap || ny >= SizeMap) continue;

                    if (Map[nx, ny] == CellType.Mine)
                        count++; //нахожу мину
                }
            }
            return count;
        }
    }
}