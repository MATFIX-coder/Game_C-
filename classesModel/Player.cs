namespace ShahtyorGame.classes
{
    public class Player
    {
        public int X { get; set; }            // позиция по X
        public int Y { get; set; }            // позиция по Y
        public int Health { get; set; }       // текущее здоровье
        public int MaxHealth { get; set; }    // максимальное здоровье
        public int PickaxeStrength { get; set; } // прочность кирки
        public int Coins { get; set; }        // монеты
        public bool HasDetector { get; set; } // куплен ли детектор мин
        public int Flashlights { get; set; }  // количество фонариков

        public Player()
        {
            X = 0;
            Y = 0;
            Health = 100;
            MaxHealth = 100;
            PickaxeStrength = 150;
            Coins = 0;
            HasDetector = false;
            Flashlights = 0;
        }

        public void ResetPosition()
        {
            X = 0;
            Y = 0;
        }

        public bool IsAlive()
        {
            return Health > 0;
        }

        public void UsePickaxe()
        {
            PickaxeStrength -= 1;
            if (PickaxeStrength < 0)
                PickaxeStrength = 0;
        }

        public bool HasPickaxe()
        {
            return PickaxeStrength > 0;
        }
    }
}