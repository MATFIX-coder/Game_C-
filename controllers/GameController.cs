using ShahtyorGame.classes;
using ShahtyorGame.forms;

namespace ShahtyorGame.controllers
{
    public class GameController
    {
        private Game game; // модель игры
        private MainForm view; // вид — главная форма

        public GameController(Game game, MainForm view) // конструктор — получаем модель и вид
        {
            this.game = game;
            this.view = view;
        }

        public void HandleKey(Keys key) // обрабатываем нажатие клавиши
        {
            bool moved = false; //флаг

            switch (key)
            {
                case Keys.Up:    moved = game.TryMove(-1, 0); break;
                case Keys.Down:  moved = game.TryMove(1, 0);  break;
                case Keys.Left:  moved = game.TryMove(0, -1); break;
                case Keys.Right: moved = game.TryMove(0, 1);  break;
            }

            if (moved) //если получилось двигаться
            {
                if (game.SteppedOnMine) // наступили на мину — открываем анаграмму
                {
                    game.SteppedOnMine = false; // сбрасываем флаг

                    var anagram = game.GetRandomAnagram(); //берем анаграмму
                    AnagramForm anagramForm = new AnagramForm(anagram.anagram, anagram.correct, anagram.hint);
                    anagramForm.ShowDialog(); // открываем как диалог (блокирует игру)

                    if (anagramForm.Solved) //решил
                    {
                        game.DefuseMine(); // обезвреживаем мину
                        game.ActionMessage = "✅ Мина обезврежена!";
                    }
                    else
                    {
                        game.Player.Health -= 15;
                        game.ActionMessage = "💥 Мина взорвалась! -15 здоровья";
                    }
                }

                view.UpdateGrid();
                view.UpdateStats();
                view.UpdateStatus();

                // проверка поражения
                if (!game.Player.IsAlive())
                {
                    MessageBox.Show("💀 Вы погибли! Игра окончена.");
                    Application.Exit();
                }

                // проверка победы на уровне
                if (game.CollectedAllArtifacts())
                {
                    int finishedLevel = game.CurrentLevel;
 
                    // Открываем магазин после каждого уровня
                    ShopForm shop = new ShopForm(game.Player, finishedLevel);
                    shop.ShowDialog();

                    // если прошли последний уровень — завершаем игру
                    if (finishedLevel >= 5)
                    {
                        MessageBox.Show("🏆 Победа! Все уровни шахты пройдены.");
                        Application.Exit();
                        return;
                    }
 
                    // Переходим на следующий уровень
                    game.NextLevel();
 
                    // Если купили детектор — открываем все мины на новом уровне
                    if (game.Player.HasDetector)
                    {
                        game.RevealAllMines();
                        game.Player.HasDetector = false; // расходуем
                        game.ActionMessage = "🔍 Детектор активирован — мины видны!";
                    }
                    else
                    {
                        game.ActionMessage = "Начался новый уровень";
                    }
 
                    view.ResetGrid();
                    view.UpdateGrid();
                    view.UpdateStats();
                    view.UpdateStatus();
                }
            }
        }
    }
}
