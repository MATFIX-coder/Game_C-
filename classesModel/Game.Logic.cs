using ShahtyorGame.enums;

namespace ShahtyorGame.classes
{
    public partial class Game
    {
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

            CellEffect(oldX, oldY, newX, newY); //реакция на клетку

            return true;
        }

        private void CellEffect(int oldX, int oldY, int newX, int newY) // что происходит когда игрок наступает на клетку
        {
            DetectedPlace[newX, newY] = true;

            switch (Map[newX, newY])
            {
                case CellType.Artifact: //реакция на артифакт
                    if (Player.PickaxeStrength >= 15)
                    {
                        Player.PickaxeStrength -= 15;
                        Player.Coins += 10;
                        CollectedArtifacts[newX, newY] = true; // помечаем что артефакт собран
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
                    Player.X = oldX;
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
    }
}