using ShahtyorGame.enums;

namespace ShahtyorGame.classes
{
    public partial class Game
    {
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

        public bool IsPlayerNearPit()
        {
            int x = Player.X;
            int y = Player.Y;

            int[] dx = { -1, 1, 0, 0 };
            int[] dy = { 0, 0, -1, 1 };

            for (int i = 0; i < 4; i++)
            {
                int nx = x + dx[i];
                int ny = y + dy[i];

                if (nx >= 0 && ny >= 0 && nx < SizeMap && ny < SizeMap)
                {
                    if (Map[nx, ny] == CellType.Pit)
                        return true;
                }
            }

            return false;
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