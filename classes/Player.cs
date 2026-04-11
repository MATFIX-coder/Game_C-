namespace ShahtyorGame.classes
{
    public class Player
    {
        public int X {get; set; } //позиция по иксу
        public int Y {get; set; } //позиция по игрику
        public int Health {get; set; } //здоровье
        public int PickaxeStrength {get; set; } //прочность кирки
        public int Coins {get; set; } //кол-во монет

        public Player() //конструктор для игрока
        {
            X = 0;
            Y = 0;
            Health = 100;
            PickaxeStrength = 100;
            Coins = 0;
        }

        public void ResetPosition() //сброс позиции после завершения уровня
        {
            X = 0;
            Y = 0;
        }

        public bool IsAlive() //проверка, что жив
        {
            return Health > 0;
        }

        public void UsePickaxe()
        {
            PickaxeStrength -= 1;
            if (PickaxeStrength < 0)
                PickaxeStrength = 0;
        }
        
        public bool HasPickaxe() //проверка целости кирки
        {
            return PickaxeStrength > 0;
        }
    }
}