using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KSKR
{
    public partial class Size : Form
    {
        public Size()
        {
            InitializeComponent();
        }

        //обработчик события при нажатии копки "Назад"
        private void BackBtn_Click(object sender, EventArgs e)
        {
            Choose choose = new Choose();
            this.Hide();
            choose.Show();
        }

        //обработчик события при нажатии копки "Принять"
        private void AcceptBtn_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("Для того, чтобы продожить, введите размерность!", "Уведомление");
                return;
            }
            if (Convert.ToInt32(textBox1.Text) > 1 && Convert.ToInt32(textBox1.Text) < 11)
            {
                KeyBoard keyBoard = new KeyBoard();
                //размерность передается в метод Sizable() класса KeyBoard
                keyBoard.Sizable(Convert.ToInt32(textBox1.Text));
                //передает значение true, чтобы класс KeyBoard мог определить каким способом ему считать
                keyBoard.Choose(true);
                //вызывается метод для ввода с клавиатуры класса KeyBoard
                keyBoard.keyEnter();
                keyBoard.Show();
                this.Hide();
            } else
            {
                MessageBox.Show("Вы ввели недопустимую размерность! Число слишком мало, " +
                    "либо слишком велико!", "Уведомление");
            }
        }

        //запрещает пользователю ввод символов
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            char symbol = e.KeyChar;
            if((symbol <= 47 || symbol >= 58) && symbol != 8)
            {
                e.Handled = true;
            }
        }

        //обработчик события закрытия формы
        private void Size_FormClosed(object sender, FormClosedEventArgs e)
        {
            Dispose();
            Close();
            Application.Exit();
        }

        //обработчик события при нажатии копки "Выйти"
        private void ExitBtn_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите выйти?", "Уведомление",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.Dispose();
                this.Close();
                Application.Exit();
            }
        }
    }
}
