using NUnit.Framework;
using NUnit.Framework.Legacy;
using ShahtyorGame.classes;
using ShahtyorGame.enums;

namespace ShahtyorGame.Tests
{
    [TestFixture]
    public class PlayerTests
    {
        private Player player;

        [SetUp]
        public void SetUp() // запускается перед каждым тестом — создаём свежего игрока
        {
            player = new Player();
        }

        [Test]
        public void Player_StartsWithCorrectValues() // игрок создаётся с правильными начальными значениями
        {
            Assert.That(player.X, Is.EqualTo(0));
            Assert.That(player.Y, Is.EqualTo(0));
            Assert.That(player.Health, Is.EqualTo(100));
            Assert.That(player.MaxHealth, Is.EqualTo(100));
            Assert.That(player.PickaxeStrength, Is.EqualTo(100));
            Assert.That(player.Coins, Is.EqualTo(0));
        }

        [Test]
        public void Player_IsAlive_WhenHealthAboveZero() // жив если здоровье больше нуля
        {
            player.Health = 50;
            Assert.That(player.IsAlive(), Is.True);
        }

        [Test]
        public void Player_IsNotAlive_WhenHealthIsZero() // мёртв если здоровье равно нулю
        {
            player.Health = 0;
            Assert.That(player.IsAlive(), Is.False);
        }

        [Test]
        public void Player_UsePickaxe_ReducesStrengthByOne() // кирка теряет 1 прочность за использование
        {
            player.UsePickaxe();
            Assert.That(player.PickaxeStrength, Is.EqualTo(99));
        }

        [Test]
        public void Player_UsePickaxe_DoesNotGoBelowZero() // прочность не уходит в минус
        {
            player.PickaxeStrength = 0;
            player.UsePickaxe();
            Assert.That(player.PickaxeStrength, Is.EqualTo(0));
        }

        [Test]
        public void Player_HasPickaxe_ReturnsFalse_WhenBroken() // HasPickaxe возвращает false когда сломана
        {
            player.PickaxeStrength = 0;
            Assert.That(player.HasPickaxe(), Is.False);
        }

        [Test]
        public void Player_HasPickaxe_ReturnsTrue_WhenIntact() // HasPickaxe возвращает true когда цела
        {
            Assert.That(player.HasPickaxe(), Is.True);
        }

        [Test]
        public void Player_ResetPosition_ReturnsToStart() // сброс позиции возвращает на 0,0
        {
            player.X = 3;
            player.Y = 4;
            player.ResetPosition();
            Assert.That(player.X, Is.EqualTo(0));
            Assert.That(player.Y, Is.EqualTo(0));
        }
    }

    [TestFixture]
    public class GameTests
    {
        private Game game;

        [SetUp]
        public void SetUp() // создаём игру перед каждым тестом
        {
            game = new Game(6);
        }

        [Test]
        public void Game_StartsAtLevelOne() // игра начинается с первого уровня
        {
            Assert.That(game.CurrentLevel, Is.EqualTo(1));
        }

        [Test]
        public void Game_PlayerStartsAtOrigin() // игрок стартует на 0,0
        {
            Assert.That(game.Player.X, Is.EqualTo(0));
            Assert.That(game.Player.Y, Is.EqualTo(0));
        }

        [Test]
        public void Game_StartCellIsDetected() // стартовая клетка сразу открыта
        {
            Assert.That(game.DetectedPlace[0, 0], Is.True);
        }

        [Test]
        public void Game_StartCellIsEmpty() // стартовая клетка всегда пустая
        {
            Assert.That(game.Map[0, 0], Is.EqualTo(CellType.EmptyPoint));
        }

        [Test]
        public void Game_TryMove_ReturnsFalse_OutOfBounds() // нельзя выйти за границы поля
        {
            bool moved = game.TryMove(-1, 0); // вверх из 0,0
            Assert.That(moved, Is.False);
        }

        [Test]
        public void Game_TryMove_ReturnsFalse_WhenPickaxeBroken() // нельзя двигаться со сломанной киркой
        {
            game.Player.PickaxeStrength = 0;
            bool moved = game.TryMove(0, 1);
            Assert.That(moved, Is.False);
        }

        [Test]
        public void Game_TryMove_ReturnsTrue_ValidMove() // можно двигаться в допустимую клетку
        {
            // ставим пустую клетку рядом чтобы не попасть на мину/пропасть
            game.Map[0, 1] = CellType.EmptyPoint;
            bool moved = game.TryMove(0, 1);
            Assert.That(moved, Is.True);
        }

        [Test]
        public void Game_TryMove_OpensCell() // при движении клетка открывается
        {
            game.Map[0, 1] = CellType.EmptyPoint;
            game.TryMove(0, 1);
            Assert.That(game.DetectedPlace[0, 1], Is.True);
        }

        [Test]
        public void Game_CollectedAllArtifacts_ReturnsFalse_WhenArtifactsExist() // не все собраны пока есть артефакты
        {
            // на поле точно есть артефакты после генерации
            Assert.That(game.CollectedAllArtifacts(), Is.False);
        }

        [Test]
        public void Game_DefuseMine_MakesCellEmpty() // обезвреживание убирает мину
        {
            game.Map[0, 0] = CellType.Mine;
            game.DefuseMine();
            Assert.That(game.Map[0, 0], Is.EqualTo(CellType.EmptyPoint));
        }

        [Test]
        public void Game_CountMinesAround_ReturnsCorrectCount() // правильно считает мины вокруг клетки
        {
            // чистим поле и ставим мины вручную
            for (int i = 0; i < 6; i++)
                for (int j = 0; j < 6; j++)
                    game.Map[i, j] = CellType.EmptyPoint;

            game.Map[0, 1] = CellType.Mine; // одна мина рядом с 0,0
            game.Map[1, 0] = CellType.Mine; // ещё одна

            Assert.That(game.CountMinesAround(0, 0), Is.EqualTo(2));
        }

        [Test]
        public void Game_IsPlayerNearPit_ReturnsTrue_WhenPitAdjacent() // обнаруживает пропасть рядом
        {
            game.Map[0, 1] = CellType.Pit; // пропасть справа от старта
            Assert.That(game.IsPlayerNearPit(), Is.True);
        }

        [Test]
        public void Game_IsPlayerNearPit_ReturnsFalse_WhenNoPitAdjacent() // нет пропасти рядом
        {
            // чистим всё поле
            for (int i = 0; i < 6; i++)
                for (int j = 0; j < 6; j++)
                    game.Map[i, j] = CellType.EmptyPoint;

            Assert.That(game.IsPlayerNearPit(), Is.False);
        }

        [Test]
        public void Game_NextLevel_IncrementsLevel() // следующий уровень увеличивает счётчик
        {
            game.NextLevel();
            Assert.That(game.CurrentLevel, Is.EqualTo(2));
        }

        [Test]
        public void Game_NextLevel_ResetsPlayerPosition() // следующий уровень возвращает игрока на старт
        {
            game.Player.X = 3;
            game.Player.Y = 3;
            game.NextLevel();
            Assert.That(game.Player.X, Is.EqualTo(0));
            Assert.That(game.Player.Y, Is.EqualTo(0));
        }

        [Test]
        public void Game_NextLevel_RestoresPickaxe() // следующий уровень восстанавливает кирку
        {
            game.Player.PickaxeStrength = 30;
            game.NextLevel();
            Assert.That(game.Player.PickaxeStrength, Is.EqualTo(100));
        }

        [Test]
        public void Game_Pit_ReturnsPlayerToOldPosition() // пропасть откатывает игрока назад
        {
            // чистим поле
            for (int i = 0; i < 6; i++)
                for (int j = 0; j < 6; j++)
                    game.Map[i, j] = CellType.EmptyPoint;

            game.Map[0, 1] = CellType.Pit; // пропасть справа
            game.TryMove(0, 1); // идём в пропасть

            Assert.That(game.Player.X, Is.EqualTo(0));
            Assert.That(game.Player.Y, Is.EqualTo(0)); // вернулись на старт
        }

        [Test]
        public void Game_Pit_ReducesHealth() // пропасть уменьшает здоровье вдвое
        {
            for (int i = 0; i < 6; i++)
                for (int j = 0; j < 6; j++)
                    game.Map[i, j] = CellType.EmptyPoint;

            game.Map[0, 1] = CellType.Pit;
            game.TryMove(0, 1);

            Assert.That(game.Player.Health, Is.EqualTo(50)); // 100 / 2
        }
    }
}