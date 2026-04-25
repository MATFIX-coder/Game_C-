using System;
using System.Drawing;
using System.Windows.Forms;

namespace ShahtyorGame.forms
{
    public class AnagramForm : Form
    {
        private Label lblAnagram;    // показывает перемешанное слово
        private Label lblHint;       // подсказка
        private TextBox txtAnswer;   // поле для ввода ответа
        private Button btnCheck;     // кнопка проверки
        private Label lblResult;     // результат (верно/неверно)

        public bool Solved { get; private set; } // угадал ли игрок

        private int attemptsLeft = 3; //попытки дать ответ
         
        private string correctWord; // правильное слово

        public AnagramForm(string anagram, string correct, string hint)
        {
            correctWord = correct.ToUpper(); // приводим к верхнему регистру чтобы сравнение было
            Solved = false;

            this.Text = "⚠️ Мина! Реши анаграмму!";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.LightGray;
            this.FormBorderStyle = FormBorderStyle.FixedDialog; // нельзя растягивать
            this.MaximizeBox = false; // убираем кнопку разворота
            this.MinimizeBox = false; // убираем кнопку свернуть
            this.FormClosing += AnagramForm_Closing; //не закрываем пока не решит или  попытки не уйдут

            // Перемешанное слово
            lblAnagram = new Label();
            lblAnagram.Text = anagram;
            lblAnagram.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            lblAnagram.Location = new Point(50, 20);
            lblAnagram.Size = new Size(300, 50);
            lblAnagram.TextAlign = ContentAlignment.MiddleCenter;
            lblAnagram.ForeColor = Color.DarkRed;

            // Подсказка
            lblHint = new Label();
            lblHint.Text = "Подсказка: " + hint;
            lblHint.Font = new Font("Segoe UI", 11, FontStyle.Italic);
            lblHint.Location = new Point(50, 75);
            lblHint.Size = new Size(300, 25);
            lblHint.TextAlign = ContentAlignment.MiddleCenter;
            lblHint.ForeColor = Color.Gray;

            // Поле ввода
            txtAnswer = new TextBox();
            txtAnswer.Location = new Point(100, 115);
            txtAnswer.Size = new Size(200, 30);
            txtAnswer.Font = new Font("Segoe UI", 14);
            txtAnswer.TextAlign = HorizontalAlignment.Center;

            // Кнопка проверки
            btnCheck = new Button();
            btnCheck.Text = "Проверить";
            btnCheck.Location = new Point(150, 160);
            btnCheck.Size = new Size(100, 35);
            btnCheck.Font = new Font("Segoe UI", 11);
            btnCheck.Click += BtnCheck_Click;

            // Результат
            lblResult = new Label();
            lblResult.Location = new Point(50, 205);
            lblResult.Size = new Size(300, 30);
            lblResult.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblResult.TextAlign = ContentAlignment.MiddleCenter;

            this.Controls.Add(lblAnagram);
            this.Controls.Add(lblHint);
            this.Controls.Add(txtAnswer);
            this.Controls.Add(btnCheck);
            this.Controls.Add(lblResult);
        }

        private void AnagramForm_Closing(object? sender, FormClosingEventArgs e)
        {
            if (!Solved && attemptsLeft > 0)
            {
                // запрещаем закрытие крестиком, нужно либо решить либо исчерпать попытки
                e.Cancel = true;
            }
        }

        private void BtnCheck_Click(object? sender, EventArgs e) //обработчик кнопки проверить
        {
            string answer = txtAnswer.Text.Trim().ToUpper(); //достаем ввод

            if (answer == correctWord)
            {
                Solved = true;
                lblResult.Text = "✅ Верно! Мина обезврежена!";
                lblResult.ForeColor = Color.Green;
                btnCheck.Enabled = false; // блокируем кнопку

                // закрываем форму через 1.5 секунды
                System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
                timer.Interval = 1500;
                timer.Tick += (s, args) =>
                {
                    timer.Stop();
                    this.Close();
                };
                timer.Start();
            }
            else
            {
                attemptsLeft--;
                txtAnswer.Clear();

                if (attemptsLeft <= 0)
                {
                    lblResult.Text = "💥 Попытки закончились! Мина взрывается!";
                    lblResult.ForeColor = Color.Red;
                    btnCheck.Enabled = false; // блокируем кнопку

                    System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
                    timer.Interval = 1500;
                    timer.Tick += (s, args) => { timer.Stop(); this.Close(); };
                    timer.Start();
                }
                else
                {
                    lblResult.Text = $"❌ Неверно! Осталось попыток: {attemptsLeft}";
                    lblResult.ForeColor = Color.Red;
                }
            }
        }
    }
}