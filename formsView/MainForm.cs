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
        private Panel statsPanel; //панель для меток

        private Label lblHealth; //метка здоровья
        private Label lblMaxHealth; //метка максимального здоровья
        private Label lblPickaxe; //метка прочности
        private Label lblCoins; //метка монет
        private Label lblLevel; //метка уровня
        private Label lblStatus; //метка события
        private Label lblFlashlights; // метка фонариков

        private System.Windows.Forms.Timer pitWarningTimer; // таймер мигания при пропасти
        private bool pitFlash = false; // флаг состояния мигания — true/false чередуется

        private const int CellSize = 55; //размер одной клетки
        private const int SideIndent = 10; //отступ от края
        private const int StatsTop = 10; //отступ панели сверху
        private const int StatsHeight = 125; //высота панели статистики
        private const int Gap = 10; //расстояние между блоками
        private const int GridTop = StatsTop + StatsHeight + Gap; //отступ поля сверху
        private const int StatusIndent = 10; //отступ статуса от поля
        private const int StatusHeight = 50; //высота строки статуса
        private const int BottomIndent = 20; //нижний отступ окна
        private const int MinWindowWidth = 620; //минимальная ширина окна, чтобы текст не обрезался
        private const int DarkLevelRadius = 1; //радиус видимости на тёмном уровне

        public MainForm() //основной конструктор игры
        {
            InitializeComponent();
            InitializeGame();
            this.KeyDown += MainForm_Go;
        }

        private int GetFieldSize() //считаю размер поля по размеру карты
        {
            return game.SizeMap * CellSize;
        }

        private int GetPanelWidth() //считаю ширину панели по размеру карты
        {
            return Math.Max(GetFieldSize() + 3, MinWindowWidth - SideIndent * 2);
        }

        private void ResizeWindowForMap() //обновляю размеры окна под текущий уровень
        {
            int fieldSize = GetFieldSize();
            int panelWidth = GetPanelWidth();
            int gridLeft = SideIndent + (panelWidth - fieldSize) / 2; //центрирую поле под верхней панелью

            this.ClientSize = new Size(panelWidth + SideIndent * 2, GridTop + fieldSize + StatusIndent + StatusHeight + BottomIndent); //размер окна зависит от карты

            if (statsPanel != null)
            {
                statsPanel.Location = new Point(SideIndent, StatsTop); //верхняя панель всегда сверху
                statsPanel.Size = new Size(panelWidth, StatsHeight); //растягиваю верхнюю панель
                LayoutStatsLabels(); //выравниваю метки внутри панели
            }

            if (grid != null)
            {
                grid.Location = new Point(gridLeft, GridTop); //поле всегда под панелью
                grid.Size = new Size(fieldSize + 3, fieldSize + 3); //растягиваю поле без обрезания границ
            }

            if (lblStatus != null)
            {
                lblStatus.Location = new Point(SideIndent, GridTop + fieldSize + StatusIndent); //ставлю статус под полем
                lblStatus.Size = new Size(panelWidth, StatusHeight); //растягиваю статус под ширину поля
            }
        }

        private void LayoutStatsLabels() //выравниваю статистику внутри панели
        {
            int panelWidth = GetPanelWidth();
            int columnWidth = (panelWidth - 30) / 2; //две равные колонки без наезда текста
            int rightX = 20 + columnWidth;

            lblHealth.Location = new Point(10, 10);
            lblHealth.Size = new Size(columnWidth, 25);

            lblMaxHealth.Location = new Point(rightX, 10);
            lblMaxHealth.Size = new Size(columnWidth, 25);

            lblPickaxe.Location = new Point(10, 45);
            lblPickaxe.Size = new Size(columnWidth, 25);

            lblCoins.Location = new Point(rightX, 45);
            lblCoins.Size = new Size(columnWidth, 25);

            lblLevel.Location = new Point(10, 80);
            lblLevel.Size = new Size(columnWidth, 25);

            lblFlashlights.Location = new Point(rightX, 80);
            lblFlashlights.Size = new Size(columnWidth, 25);
        }

        private void InitializeGame()
        {
            game = new Game(); //инициализируем размер поля
            controller = new GameController(game, this); // создаём контроллер

            this.Text = "Шахтёр: Тайны глубин"; //название окна
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

            statsPanel = new Panel(); //панель для меток
            statsPanel.Location = new Point(SideIndent, StatsTop);
            statsPanel.Size = new Size(GetPanelWidth(), StatsHeight);
            statsPanel.BackColor = Color.White;
            statsPanel.BorderStyle = BorderStyle.FixedSingle;

            //Метка Здоровье
            lblHealth = new Label();
            lblHealth.Location = new Point(10, 10);
            lblHealth.Size = new Size(150, 25);
            lblHealth.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblHealth.Text = "❤️ Здоровье: 100";

            //Метка максимального здоровья
            lblMaxHealth = new Label();
            lblMaxHealth.Location = new Point(180, 10);
            lblMaxHealth.Size = new Size(150, 25);
            lblMaxHealth.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblMaxHealth.Text = "💖 Максимальное здоровье: 100";

            //Метка Прочности
            lblPickaxe = new Label();
            lblPickaxe.Location = new Point(10, 45);
            lblPickaxe.Size = new Size(150, 25);
            lblPickaxe.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblPickaxe.Text = "⛏️ Кирка: 150";

            //Метка Монет
            lblCoins = new Label();
            lblCoins.Location = new Point(180, 45);
            lblCoins.Size = new Size(150, 25);
            lblCoins.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblCoins.Text = "💰 Монеты: 0";

            //Метка Уровень
            lblLevel = new Label();
            lblLevel.Location = new Point(10, 80);
            lblLevel.Size = new Size(150, 25);
            lblLevel.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblLevel.Text = "📊 Уровень: 1";

            // Метка фонариков
            lblFlashlights = new Label();
            lblFlashlights.Location = new Point(180, 80);
            lblFlashlights.Size = new Size(150, 25);
            lblFlashlights.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblFlashlights.Text = "🔦 Фонарики: 0";

            //Метка события
            lblStatus = new Label();
            lblStatus.Location = new Point(SideIndent, GridTop + GetFieldSize() + StatusIndent);
            lblStatus.Size = new Size(GetPanelWidth(), StatusHeight);
            lblStatus.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblStatus.BorderStyle = BorderStyle.FixedSingle;
            lblStatus.BackColor = Color.White;
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;
            this.Controls.Add(lblStatus);

            //добавляю метки на панель
            statsPanel.Controls.Add(lblHealth);
            statsPanel.Controls.Add(lblMaxHealth);
            statsPanel.Controls.Add(lblPickaxe);
            statsPanel.Controls.Add(lblCoins);
            statsPanel.Controls.Add(lblLevel);
            statsPanel.Controls.Add(lblFlashlights);
            this.Controls.Add(statsPanel); //добавляю панель на форму

            grid = new DataGridView(); //создаю разметку
            grid.Location = new Point(SideIndent, GridTop);
            grid.Size = new Size(GetFieldSize() + 3, GetFieldSize() + 3);

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
                grid.Columns[i].Width = CellSize;
                grid.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable; // без сортировки
            }

            // Создаём строки
            for (int i = 0; i < game.SizeMap; i++)
            {
                grid.Rows[i].Height = CellSize;
            }

            this.Controls.Add(grid); //добавляю разметку на форму

            ResizeWindowForMap(); //выравниваю окно и элементы после создания поля

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

        private bool IsCellVisibleOnDarkLevel(int x, int y) //проверяю видимость клетки на тёмном уровне
        {
            if (!game.IsDarkLevel)
                return true;

            int dx = Math.Abs(game.Player.X - x);
            int dy = Math.Abs(game.Player.Y - y);

            return dx <= DarkLevelRadius && dy <= DarkLevelRadius;
        }

        private void TryUseFlashlight() // пробуем автоматически использовать фонарик рядом с пропастью
        {
            if (game.Player.Flashlights <= 0) // фонариков нет — ничего не делаем
                return;

            int x = game.Player.X;
            int y = game.Player.Y;

            int[] dx = { -1, 1, 0, 0 }; // 4 направления без диагоналей
            int[] dy = { 0, 0, -1, 1 };

            for (int i = 0; i < 4; i++)
            {
                int nx = x + dx[i];
                int ny = y + dy[i];

                if (nx < 0 || ny < 0 || nx >= game.SizeMap || ny >= game.SizeMap) // за границей
                    continue;

                if (game.Map[nx, ny] == CellType.Pit && !game.DetectedPlace[nx, ny]) // пропасть рядом и не открыта
                {
                    game.DetectedPlace[nx, ny] = true; // открываем пропасть
                    game.Player.Flashlights--;          // тратим фонарик
                    game.ActionMessage = "🔦 Фонарик подсветил пропасть рядом!";
                    return; // открываем только первую найденную
                }
            }
        }

        public void UpdateGrid() // public — контроллер вызывает это
        {
            TryUseFlashlight();
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

                    if (!IsCellVisibleOnDarkLevel(i, j)) //если тёмный уровень и клетка далеко
                    {
                        cell.Value = "";
                        cell.Style.BackColor = Color.Black;
                        cell.Style.ForeColor = Color.White;
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
            ResizeWindowForMap(); //обновляю размер окна под новую карту

            grid.Columns.Clear();
            grid.Rows.Clear();

            for (int i = 0; i < game.SizeMap; i++)
            {
                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                column.Width = CellSize;
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
                grid.Columns.Add(column);
            }

            for (int i = 0; i < game.SizeMap; i++)
            {
                grid.Rows.Add();
                grid.Rows[i].Height = CellSize;
            }

            ResizeWindowForMap(); //после пересоздания поля снова выравниваю элементы
        }

        public void UpdateStats() //обновляю статистику
        {
            lblHealth.Text = "❤️ Здоровье: " + game.Player.Health;
            lblMaxHealth.Text = "💖 Максимальное здоровье: " + game.Player.MaxHealth;
            lblPickaxe.Text = "⛏️ Кирка: " + game.Player.PickaxeStrength;
            lblCoins.Text = "💰 Монеты: " + game.Player.Coins;
            lblLevel.Text = "📊 Уровень: " + game.CurrentLevel;
            lblFlashlights.Text = "🔦 Фонарики: " + game.Player.Flashlights;
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
