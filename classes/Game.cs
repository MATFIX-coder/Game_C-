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
        private Random random = new Random();

        public Game(int size = 6) //констурктор игры
        {
            SizeMap = size; 
            Map = new CellType[SizeMap, SizeMap];
            DetectedPlace = new bool[SizeMap, SizeMap];
            Player = new Player();
            CurrentLevel = 1;
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

            int artifactsCount = 3; //тестово устанавливаю 3 артифакта
            int setArtifactsCount = 0; //счетчик установленных

            while (setArtifactsCount < artifactsCount) //цикл установки
            {
                int x = random.Next(SizeMap);
                int y = random.Next(SizeMap);

                if ((x == 0 && y == 0) || Map[x, y] == CellType.Artifact) //проверка что это не начальная клетка и там не занято другим артифактом
                    continue;
                
                Map[x, y] = CellType.Artifact; //устанавливаю
                setArtifactsCount++;
            }
            DetectedPlace[0, 0] = true; //открываю начальную позицию
        }

        public bool TryMove(int goX, int goY) //возможно движение?
        {
            int newX = Player.X + goX;
            int newY = Player.Y + goY;

            if (newX < 0 || newY < 0 || newX >= SizeMap || newY >= SizeMap) //если не выходи за рамки
                return false;

            if (!Player.BrokePickaxe()) //если кирка еще не сломана
                return false;

            Player.X = newX;
            Player.Y = newY;
            DetectedPlace[newX, newY] = true;

            CellEffect(newX, newY); //реакция на клетку перемещение
            return true;
        }

        private void CellEffect(int newX, int newY)
        {
            switch(Map[newX, newY])
            {
                case CellType.Artifact: //реакция на артифакт
                    if (Player.PickaxeStrength >= 15)
                    {
                        Player.PickaxeStrength -= 15;
                        Player.Coins += 10;
                        Map[newX, newY] = CellType.EmptyPoint;
                    }
                    else
                    {
                        Player.UsePickaxe(); //если не хватает прочности, то просто переходим туда
                    }
                    break;

                case CellType.EmptyPoint: //если пустая клетка(обычная), то тратится одна прочность
                    Player.UsePickaxe();
                    break;
            }
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

        public void ResetLevel() //оконачание уровня, сброс
        {
            Player.ResetPosition();
            GenerateLevel();
        }
    }
}