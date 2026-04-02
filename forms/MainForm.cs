using System;
using System.Drawing;
using System.Windows.Forms;
using ShahtyorGame.classes;
using ShahtyorGame.enums;

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
        private Label lblStatus; //метка события

        public MainForm() //основной конструктор игры
        {
            InitializeComponent();

            InitializeGame();

            this.KeyDown += MainForm_KeyDown;
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

            //Метка события
            lblStatus = new Label();
            lblStatus.Location = new Point(10, 440);
            lblStatus.Size = new Size(330, 50);
            lblStatus.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblStatus.BorderStyle = BorderStyle.FixedSingle;
            lblStatus.BackColor = Color.White;
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            this.Controls.Add(lblStatus);


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
            UpdateStatus(); //обсновляю метку события
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
                        cell.Style.BackColor = Color.LightBlue; //ячейка шахтера
                        continue;
                    }

                    if (!game.DetectedPlace[i, j]) //если ячейка неизвестна нам
                    {
                        cell.Value = "?";
                        cell.Style.BackColor = Color.DarkGray;
                        continue;
                    }

                    cell.Value = GetCellDisplay(i, j); //отрисовываем на основе назначения клетки

                    if (game.DetectedPlace[i, j]) //после того как посетили ячейку, то делаю белой
                    {
                        cell.Style.BackColor = Color.White;
                        continue;
                    }
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

        private void UpdateStatus()
        {
            lblStatus.Text = "Статус: " + game.ActionMessage;

            //цвет по умолчанию
            lblStatus.BackColor = Color.White;
            lblStatus.ForeColor = Color.Black;

            //подсветка в зависимости от события
            if (game.ActionMessage.Contains("добыт"))
            {
                lblStatus.BackColor = Color.LightGreen; //получилось сломать
            }
            else if (game.ActionMessage.Contains("слабая"))
            {
                lblStatus.BackColor = Color.Orange; //нехватает прочности
            }
            else if (game.ActionMessage.Contains("Пустая"))
            {
                lblStatus.BackColor = Color.Gray; //если клетка пустая
            }
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            bool moved = false;

            switch (e.KeyCode)
            {
                case Keys.Up:
                    moved = game.TryMove(-1, 0);
                    break;

                case Keys.Down:
                    moved = game.TryMove(1, 0);
                    break;
                
                case Keys.Left:
                    moved = game.TryMove(0, -1);
                    break;
                
                case Keys.Right:
                    moved = game.TryMove(0, 1);
                    break;
            }

            if (moved)
            {
                UpdateGrid(); // обновляю ячейки
                UpdateStats(); // обновляю метки
                UpdateStatus(); //обновляю метку события
            }

            if (game.CollectedAllArtifacts())
            {
                MessageBox.Show("Ура! Все артефакты собраны!");
            }
        }

        private void InitializeComponent()
        {
            this.AutoScaleMode = AutoScaleMode.Font;
        }
    }
}
