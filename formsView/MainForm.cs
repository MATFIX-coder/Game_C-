using System;
using System.Drawing;
using System.Windows.Forms;
using ShahtyorGame.classes;
using ShahtyorGame.controllers;
using ShahtyorGame.enums;

namespace ShahtyorGame.forms
{
    public partial class MainForm : Form
    {
        private Game game; //Игра
        private GameController controller; // контроллер — обрабатывает логику
        private DataGridView grid; //таблица разметка

        private Label lblHealth; //метка здоровья
        private Label lblPickaxe; //метка прочности
        private Label lblCoins; //метка монет
        private Label lblLevel; //метка уровня
        private Label lblStatus; //метка события

        private System.Windows.Forms.Timer pitWarningTimer; // таймер мигания при пропасти
        private bool pitFlash = false; // флаг состояния мигания — true/false чередуется

        public MainForm() //основной конструктор игры
        {
            InitializeComponent();
            InitializeGame();
            this.KeyDown += MainForm_Go;
        }

        private void InitializeGame()
        {
            game = new Game(6); //инициализируем размер поля
            controller = new GameController(game, this); // создаём контроллер

            this.Text = "Шахтёр: Тайны глубин"; //название окна
            this.ClientSize = new Size(360, 550); //размер окна изначально
            this.StartPosition = FormStartPosition.CenterScreen; //окно запускается по центру
            this.KeyPreview = true; // форма перехватывает нажатия клавиш раньше дочерних элементов
            this.BackColor = Color.LightGray; //задний фон серый

            pitWarningTimer = new System.Windows.Forms.Timer();
            pitWarningTimer.Interval = 300;
            pitWarningTimer.Tick += (s, e) =>
            {
                pitFlash = !pitFlash;
                UpdateGrid();
            };
            pitWarningTimer.Start();

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
            grid.Size = new Size(330, 380);

            grid.RowHeadersVisible = false; // скрыть заголовки строк и столбцов
            grid.ColumnHeadersVisible = false;

            grid.AllowUserToAddRows = false; //нельзя добовлять
            grid.AllowUserToDeleteRows = false;

            grid.AllowUserToResizeRows = false; //нельзя управлять размером
            grid.AllowUserToResizeColumns = false;

            grid.ScrollBars = ScrollBars.None; //убрать скролл бар
            grid.ReadOnly = true; //только для просмотра
            grid.TabStop = false; //чтобы grid не забирал фокус
            grid.MultiSelect = false;

            grid.SelectionMode = DataGridViewSelectionMode.CellSelect; //можно выделять ячейки
            grid.DefaultCellStyle.SelectionBackColor = Color.White; //цвета фона при выделении
            grid.DefaultCellStyle.SelectionForeColor = Color.Black; //цвет текста при выделении
            grid.DefaultCellStyle.Font = new Font("Segoe UI Emoji", 18, FontStyle.Regular); //шрифт ячейки
            grid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter; //выравнивание по центру

            grid.ColumnCount = game.SizeMap; // кол-во строк и столбцов
            grid.RowCount = game.SizeMap;

            // Настраиваю столбцы
            for (int i = 0; i < game.SizeMap; i++)
            {
                grid.Columns[i].Width = 55;
                grid.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable; // без сортировки
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

        private Color GetHintColor(int x, int y) //цвет цифр как подсказок к минам
        {
            int count = game.CountMinesAround(x, y);
            switch (count)
            {
                case 1: return Color.Blue;
                case 2: return Color.Green;
                case 3: return Color.Red;
                case 4: return Color.DarkBlue;
                case 5: return Color.DarkRed;
                default: return Color.Black;
            }
        }

        public void UpdateGrid() // public — контроллер вызывает это
        {
            bool nearPit = game.IsPlayerNearPit(); // проверяем рядом ли пропасть

            for (int i = 0; i < game.SizeMap; i++)
            {
                for (int j = 0; j < game.SizeMap; j++) //пробегаю по каждой ячейке
                {
                    DataGridViewCell cell = grid.Rows[i].Cells[j]; //считываю ячейку

                    if (game.Player.X == i && game.Player.Y == j) //если там игрок
                    {
                        cell.Value = "⛏";
                        // мигаем красным если рядом пропасть
                        if (nearPit && pitFlash)
                            cell.Style.BackColor = Color.Red;
                        else
                            cell.Style.BackColor = Color.LightBlue; //ячейка шахтера
                        cell.Style.ForeColor = Color.Black; //цвет символа
                        continue;
                    }

                    if (!game.DetectedPlace[i, j]) //если ячейка неизвестна нам
                    {
                        cell.Value = "?";
                        cell.Style.BackColor = Color.DarkGray;
                        cell.Style.ForeColor = Color.Black;
                        continue;
                    }

                    //клетка открыта
                    cell.Value = GetCellDisplay(i, j); //отрисовываем на основе назначения клетки
                    cell.Style.BackColor = Color.White;
                    cell.Style.ForeColor = GetHintColor(i, j);
                }
            }
            grid.ClearSelection(); //убираю выделение серое с ячеек
        }

        private string GetCellDisplay(int x, int y) //отрисовываем на основе назначения клетки
        {
            switch (game.Map[x, y])
            {
                case CellType.Artifact: // если в клетке артефакт
                    return "💎";

                case CellType.Pit: // если в клетке пропасть
                    return "🕳";

                case CellType.Mine: // если в клетке мина
                    return "💣";

                case CellType.EmptyPoint:
                    if (game.CollectedArtifacts[x, y]) // тут был артефакт — показываем иконку
                        return "💎";
                    int minesAround = game.CountMinesAround(x, y);
                    if (minesAround > 0)
                        return minesAround.ToString();
                    return ".";
                    
                default:
                    return ".";
            }
        }

        public void ResetGrid() //обновление разметки, все пересоздаю
        {
            grid.Columns.Clear();
            grid.Rows.Clear();

            for (int i = 0; i < game.SizeMap; i++)
            {
                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                column.Width = 55;
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
                grid.Columns.Add(column);
            }

            for (int i = 0; i < game.SizeMap; i++)
            {
                grid.Rows.Add();
                grid.Rows[i].Height = 55;
            }
        }

        public void UpdateStats() //обновляю статистику
        {
            lblHealth.Text = "❤️ Здоровье: " + game.Player.Health;
            lblPickaxe.Text = "⛏️ Кирка: " + game.Player.PickaxeStrength;
            lblCoins.Text = "💰 Монеты: " + game.Player.Coins;
            lblLevel.Text = "📊 Уровень: " + game.CurrentLevel;
        }

        public void UpdateStatus() //обрабытываем события
        {
            lblStatus.Text = "Статус: " + game.ActionMessage;
            lblStatus.ForeColor = Color.Black;

            //устанавливаю цвет фона
            if (game.ActionMessage.Contains("добыт"))
                lblStatus.BackColor = Color.LightGreen;
            else if (game.ActionMessage.Contains("Мина обезврежена"))
                lblStatus.BackColor = Color.LightGreen;
            else if (game.ActionMessage.Contains("взорвалась"))
                lblStatus.BackColor = Color.Red;
            else if (game.ActionMessage.Contains("Пустая"))
                lblStatus.BackColor = Color.LightGray;
            else
                lblStatus.BackColor = Color.White;
        }

        private void MainForm_Go(object sender, KeyEventArgs e) //движение по полю
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    controller.HandleKey(e.KeyCode); // передаём нажатие контроллеру
                    break;
            }
        }

        private void InitializeComponent()
        {
            this.AutoScaleMode = AutoScaleMode.Font;
        }
    }
}