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
            grid.ColumnCount = game.SizeMap; //кол-во стобцов и строк
            grid.RowCount = game.SizeMap;

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

            grid.Columns.Clear(); //очищаем столбца
            for (int i = 0; i < game.SizeMap; i++)
            {
                //добавляю столбыцы
                DataGridViewColumn column = new DataGridViewTextBoxColumn();
                column.Width = 55;
                column.HeaderText = "";
                column.SortMode = DataGridViewColumnSortMode.NotSortable; //без сортировки
                grid.Columns.Add(column);
            }

            grid.Rows.Clear(); //очищаем строки
            for (int i = 0; i < game.SizeMap; i++)
            {
                //добавляю строки
                grid.Rows.Add();
                grid.Rows[i].Height = 55;
            }

            this.Controls.Add(grid); //добавляю разметку на форму

            Label lblTemp = new Label();
            lblTemp.Text = "Костя привет!";
            lblTemp.Location = new Point(80, 440);
            lblTemp.Size = new Size(200, 30);
            this.Controls.Add(lblTemp);
        }

        private void InitializeComponent()
        {
            this.AutoScaleMode = AutoScaleMode.Font;
        }
    }
}
