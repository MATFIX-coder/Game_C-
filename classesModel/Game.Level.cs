using ShahtyorGame.enums;

namespace ShahtyorGame.classes
{
    public partial class Game
    {
        private void GenerateLevel() //генерация уровня
        {
            CollectedArtifacts = new bool[SizeMap, SizeMap]; // сброс при новом уровне
            
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