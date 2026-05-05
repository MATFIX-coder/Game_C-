using System;
using System.Drawing;
using System.Windows.Forms;
using ShahtyorGame.classes;

namespace ShahtyorGame.forms
{
    public class ShopForm : Form
    {
        private Player player;
        private Label lblTitle;
        private Label lblCoins;
        private Panel itemsPanel;

        // Каждый товар: название, описание, цена, действие
        private (string icon, string name, string desc, int price, Action<Player> effect)[] shopItems =
        {
            (
                "❤️",
                "Аптечка",
                "+15 здоровья",
                15,
                p =>
                {
                    p.Health = Math.Min(p.Health + 15, p.MaxHealth);
                }
            ),
            (
                "💪",
                "Укрепление тела",
                "+20 к макс. здоровью\n(и лечит до нового макс.)",
                25,
                p =>
                {
                    p.MaxHealth += 20;
                    p.Health = Math.Min(p.Health + 10, p.MaxHealth); // небольшой бонус
                }
            ),
            (
                "⛏️",
                "Заточка кирки",
                "Прочность на максимум",
                15,
                p =>
                {
                    p.PickaxeStrength += 20;
                }
            ),
            (
                "🔍",
                "Детектор мин",
                "Открывает ВСЕ мины на\nследующем уровне",
                30,
                p =>
                {
                    p.HasDetector = true;
                }
            ),
            (
                "🔦",
                "Фонарик",
                "Автоматически открывает\nближайшую пропасть рядом",
                20,
                p =>
                {
                    p.Flashlights += 1; // +1 фонарик
                }
            ),
        };

        public ShopForm(Player player, int level)
        {
            this.player = player;

            this.Text = "🛒 Магазин шахтёра";
            this.Size = new Size(480, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(30, 28, 24);       // тёмный фон
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // --- Заголовок ---
            lblTitle = new Label();
            lblTitle.Text = $"⛏ Уровень {level} пройден!";
            lblTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(255, 210, 80);   // золото
            lblTitle.Location = new Point(20, 18);
            lblTitle.Size = new Size(420, 32);
            lblTitle.TextAlign = ContentAlignment.MiddleLeft;
            this.Controls.Add(lblTitle);

            // --- Монеты ---
            lblCoins = new Label();
            lblCoins.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            lblCoins.ForeColor = Color.FromArgb(200, 200, 200);
            lblCoins.Location = new Point(20, 52);
            lblCoins.Size = new Size(420, 26);
            lblCoins.TextAlign = ContentAlignment.MiddleLeft;
            RefreshCoinsLabel();
            this.Controls.Add(lblCoins);

            // --- Разделитель ---
            Panel separator = new Panel();
            separator.Location = new Point(20, 86);
            separator.Size = new Size(420, 1);
            separator.BackColor = Color.FromArgb(70, 68, 60);
            this.Controls.Add(separator);

            // --- Панель товаров ---
            itemsPanel = new Panel();
            itemsPanel.Location = new Point(20, 100);
            itemsPanel.Size = new Size(420, 455);
            itemsPanel.BackColor = Color.Transparent;
            this.Controls.Add(itemsPanel);

            BuildShopItems();

            // --- Кнопка «Продолжить» ---
            Button btnContinue = new Button();
            btnContinue.Text = "▶  Продолжить";
            btnContinue.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            btnContinue.ForeColor = Color.FromArgb(30, 28, 24);
            btnContinue.BackColor = Color.FromArgb(255, 210, 80);
            btnContinue.FlatStyle = FlatStyle.Flat;
            btnContinue.FlatAppearance.BorderSize = 0;
            btnContinue.Location = new Point(20, 570);
            btnContinue.Size = new Size(420, 42);
            btnContinue.Cursor = Cursors.Hand;
            btnContinue.Click += (s, e) => this.Close();
            this.Controls.Add(btnContinue);
        }

        private void BuildShopItems()
        {
            itemsPanel.Controls.Clear();
            int y = 0;

            for (int i = 0; i < shopItems.Length; i++)
            {
                var item = shopItems[i];
                int capturedIndex = i;

                // Карточка товара
                Panel card = new Panel();
                card.Location = new Point(0, y);
                card.Size = new Size(420, 76);
                card.BackColor = Color.FromArgb(48, 44, 36);
                card.Cursor = Cursors.Hand;

                // Рамка карточки
                card.Paint += (s, e) =>
                {
                    e.Graphics.DrawRectangle(
                        new Pen(Color.FromArgb(80, 75, 60), 1),
                        0, 0, card.Width - 1, card.Height - 1
                    );
                };

                // Иконка
                Label lblIcon = new Label();
                lblIcon.Text = item.icon;
                lblIcon.Font = new Font("Segoe UI Emoji", 22);
                lblIcon.Location = new Point(12, 12);
                lblIcon.Size = new Size(46, 46);
                lblIcon.TextAlign = ContentAlignment.MiddleCenter;
                card.Controls.Add(lblIcon);

                // Название товара
                Label lblName = new Label();
                lblName.Text = item.name;
                lblName.Font = new Font("Segoe UI", 12, FontStyle.Bold);
                lblName.ForeColor = Color.FromArgb(240, 235, 210);
                lblName.Location = new Point(68, 10);
                lblName.Size = new Size(220, 24);
                card.Controls.Add(lblName);

                // Описание
                Label lblDesc = new Label();
                lblDesc.Text = item.desc;
                lblDesc.Font = new Font("Segoe UI", 10);
                lblDesc.ForeColor = Color.FromArgb(150, 145, 130);
                lblDesc.Location = new Point(68, 34);
                lblDesc.Size = new Size(220, 36);
                card.Controls.Add(lblDesc);

                // Кнопка «Купить»
                Button btnBuy = new Button();
                btnBuy.Text = $"💰 {item.price}";
                btnBuy.Font = new Font("Segoe UI", 11, FontStyle.Bold);
                btnBuy.Size = new Size(90, 36);
                btnBuy.Location = new Point(316, 20);
                btnBuy.FlatStyle = FlatStyle.Flat;
                btnBuy.Cursor = Cursors.Hand;
                UpdateBuyButton(btnBuy, item.price);

                btnBuy.Click += (s, e) =>
                {
                    var si = shopItems[capturedIndex];
                    if (player.Coins >= si.price)
                    {
                        player.Coins -= si.price;
                        si.effect(player);
                        RefreshCoinsLabel();
                        // Обновляем все кнопки (монет могло стать меньше)
                        RefreshAllBuyButtons();
                    }
                };

                card.Controls.Add(btnBuy);
                itemsPanel.Controls.Add(card);

                y += 82; // отступ между карточками
            }
        }

        private void UpdateBuyButton(Button btn, int price)
        {
            bool canAfford = player.Coins >= price;
            btn.BackColor = canAfford
                ? Color.FromArgb(255, 210, 80)
                : Color.FromArgb(60, 58, 50);
            btn.ForeColor = canAfford
                ? Color.FromArgb(30, 28, 24)
                : Color.FromArgb(100, 98, 90);
            btn.FlatAppearance.BorderColor = canAfford
                ? Color.FromArgb(200, 160, 40)
                : Color.FromArgb(80, 78, 70);
            btn.Enabled = canAfford;
        }

        private void RefreshAllBuyButtons()
        {
            // Перебираем все карточки и обновляем кнопки
            int i = 0;
            foreach (Control card in itemsPanel.Controls)
            {
                if (card is Panel p && i < shopItems.Length)
                {
                    foreach (Control c in p.Controls)
                    {
                        if (c is Button btn)
                            UpdateBuyButton(btn, shopItems[i].price);
                    }
                    i++;
                }
            }
        }

        private void RefreshCoinsLabel()
        {
            lblCoins.Text = $"💰 Монеты: {player.Coins}";
        }
    }
}
