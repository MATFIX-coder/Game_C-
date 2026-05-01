using ShahtyorGame.enums;

namespace ShahtyorGame.classes
{
    public partial class Game
    {
        private class LevelConfig //настройки уровня
        {
            public int Size; //размер поля
            public int Mines; //количество мин
            public int Pits; //количество пропастей
            public int Artifacts; //количество артефактов
            public bool IsDark; //тёмный уровень или нет
        }

        private LevelConfig GetLevelConfig() //получаем настройки текущего уровня
        {
            return CurrentLevel switch
            {
                1 => new LevelConfig { Size = 6, Mines = 2, Pits = 0, Artifacts = 3, IsDark = true },
                2 => new LevelConfig { Size = 7, Mines = 4, Pits = 1, Artifacts = 3, IsDark = false },
                3 => new LevelConfig { Size = 7, Mines = 5, Pits = 3, Artifacts = 4, IsDark = true },
                4 => new LevelConfig { Size = 8, Mines = 7, Pits = 4, Artifacts = 4, IsDark = false },
                5 => new LevelConfig { Size = 8, Mines = 9, Pits = 5, Artifacts = 5, IsDark = true },
                _ => new LevelConfig { Size = 8, Mines = 9, Pits = 5, Artifacts = 5, IsDark = true }
            };
        }

        private void GenerateLevel() //генерация уровня
        {
            LevelConfig config = GetLevelConfig(); //беру настройки уровня

            SizeMap = config.Size; //меняю размер карты под уровень
            Map = new CellType[SizeMap, SizeMap]; //пересоздаю карту
            CollectedArtifacts = new bool[SizeMap, SizeMap]; // сброс при новом уровне
            DetectedPlace = new bool[SizeMap, SizeMap]; //сброс открытых клеток
            IsDarkLevel = config.IsDark; //запоминаю тёмный уровень
            SteppedOnMine = false; //сбрасываю флаг мины
            
            for (int i = 0; i < SizeMap; i++) //отчищаю поле после прошлого
            {
                for (int j = 0; j < SizeMap; j++)
                {
                    Map[i, j] = CellType.EmptyPoint;
                    DetectedPlace[i, j] = false;
                }
            }

            int artifactsCount = config.Artifacts; // количество артефактов на уровне
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

            int minesCount = config.Mines; //количество мин на уровне
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

            int pitsCount = config.Pits; //количество пропастей на уровне
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

        public void NextLevel() //оконачание уровня, сброс
        {
            Player.ResetPosition(); //сброс позиции
            CurrentLevel++; //левел некст
            Player.PickaxeStrength = 100; //обновляю кирку
            ActionMessage = "Начался новый уровень";
            GenerateLevel(); //запуск левела
        }
    }
}
