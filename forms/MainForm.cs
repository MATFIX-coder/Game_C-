using System;
using System.Drawing;
using System.Windows.Forms;
using ShahtyorGame.classes;

namespace ShahtyorGame.forms
{

    public partial class MainForm : Form
    {
        private Game game; //Игра
        private DataGridView grid; //таблица разметка

        private Label lblHealth; //метка здоровья
        private Label lblPickaxe; //метка прочности
        private Label lblCoins; //метка монет
        private Label lblLevel; //метка уровня

        public MainForm() //основной конструктор игры
        {
            InitializeComponent();

            InitializeGame();
        }

        private void InitializeGame()
        {
            game = new Game(6); //инициализируем размер поля

            this.Text = "Шахтёр: Тайны глубин"; //название окна

            this.ClientSize = new Size(350, 550); //размер окна изначально

            this.StartPosition = FormStartPosition.CenterScreen; //окно запускается по центру

            this.KeyPreview = true; //!узнать

            this.BackColor = Color.LightGray; //задний фон серый

            Panel statsPanel = new Panel(); //панель для меток
            statsPanel.Location = new Point(10, 10);
            statsPanel.Size = new Size(330, 80);
            statsPanel.BackColor = Color.White;
            statsPanel.BorderStyle = BorderStyle.FixedSingle;

            //Метка Здоровье
            lblHealth = new Label();
            lblHealth.Location = new Point(10, 10);
            lblHealth.Size = new Size(150, 25);
            lblHealth.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblHealth.Text = "❤️ Здоровье: 100";

            //Метка Прочности
            lblPickaxe = new Label();
            lblPickaxe.Location = new Point(10, 40);
            lblPickaxe.Size = new Size(150, 25);
            lblPickaxe.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblPickaxe.Text = "⛏️ Кирка: 100";

            //Метка Монет
            lblCoins = new Label();
            lblCoins.Location = new Point(170, 10);
            lblCoins.Size = new Size(150, 25);
            lblCoins.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblCoins.Text = "💰 Монеты: 0";

            //Метка Уровень
            lblLevel = new Label();
            lblLevel.Location = new Point(170, 40);
            lblLevel.Size = new Size(150, 25);
            lblLevel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblLevel.Text = "📊 Уровень: 1";

            //добавляю метки на панель
            statsPanel.Controls.Add(lblHealth);
            statsPanel.Controls.Add(lblPickaxe);
            statsPanel.Controls.Add(lblCoins);
            statsPanel.Controls.Add(lblLevel);

            this.Controls.Add(statsPanel); //добавляю панель на форму

            grid = new DataGridView(); //создаю разметку
            grid.Location = new Point(10, 100); 
            grid.Size = new Size(330, 330);

            grid.RowHeadersVisible = false; // скрыть заголовки строк и столбцов
            grid.ColumnHeadersVisible = false;

            grid.AllowUserToAddRows = false; //нельзя добовлять
            grid.AllowUserToDeleteRows = false;

            grid.AllowUserToResizeRows = false; //нельзя управлять размером
            grid.AllowUserToResizeColumns = false;

            grid.ScrollBars = ScrollBars.None; //убрать скролл бар
            grid.ReadOnly = true; //только для просмотра

            grid.SelectionMode = DataGridViewSelectionMode.CellSelect; //можно выделять ячейки
            grid.DefaultCellStyle.Font = new Font("Segoe UI Emoji", 18, FontStyle.Regular); //шрифт ячейки
            grid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; //выравнивание по центру

            grid.Columns.Clear(); //очищаем столбцы
            grid.ColumnCount = game.SizeMap;
            grid.RowCount = game.SizeMap;

            // Настраиваю столбцы
            for (int i = 0; i < game.SizeMap; i++)
            {
                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                column.Width = 55;
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }

            // Создаём строки
            for (int i = 0; i < game.SizeMap; i++)
            {
                grid.Rows[i].Height = 55;
            }

            this.Controls.Add(grid); //добавляю разметку на форму

            UpdateGrid(); //обновляю содержимые клеток
            UpdateStats(); //обновляю содержимое меток
        }

        private void UpdateGrid()
        {
            for(int i = 0; i < game.SizeMap; i++)
            {
                for(int j = 0; j < game.SizeMap; j++) //пробегаю по каждой ячейке
                {
                    DataGridViewCell cell = grid.Rows[i].Cells[j]; //считываю ячейку

                    if (game.Player.X == i && game.Player.Y == j) //если там игрок
                    {
                        cell.Value = "⛏";
                        continue;
                    }

                    if (!game.DetectedPlace[i, j]) //если ячейка неизвестна нам
                    {
                        cell.Value = "?";
                        continue;
                    }

                    cell.Value = GetCellDisplay(i, j); //отрисовываем на основе назначения клетки
                }
            }
        }

        private string GetCellDisplay(int x, int y)
        {
            switch (game.Map[x, y])
            {
                case ShahtyorGame.enums.CellType.Artifact: // если в клетке артефакт
                    return "💎";

                case ShahtyorGame.enums.CellType.EmptyPoint: // если клетка открыта и пуста
                    return ".";

                default:
                    return ".";
            }
        }

        private void UpdateStats() //обновляю статистику
        {
            lblHealth.Text = "❤️ Здоровье: " + game.Player.Health;
            lblPickaxe.Text = "⛏️ Кирка: " + game.Player.PickaxeStrength;
            lblCoins.Text = "💰 Монеты: " + game.Player.Coins;
            lblLevel.Text = "📊 Уровень: " + game.CurrentLevel;
        }

        private void InitializeComponent()
        {
            this.AutoScaleMode = AutoScaleMode.Font;
        }
    }
}
