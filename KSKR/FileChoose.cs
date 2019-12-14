using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace KSKR
{
    public partial class FileChoose : Form
    {
        public FileChoose()
        {
            InitializeComponent();
        }

        KeyBoard keyBoard = new KeyBoard();

        //обработчик события при нажатии кнопки "Обзор"
        private void OverviewBtn_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory =  @"..\..\..\KSKR\files";
            openFileDialog1.Filter = "(*.txt)|*.txt";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog1.FileName;                
            }
        }

        //обработчик события при нажатии копки "Принять"
        private void AcceptBtn_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                try
                {
                    KeyBoard keyBoard = new KeyBoard();
                    //передает в класс KeyBoard путь к файлу
                    keyBoard.FileName(textBox1.Text);
                    //вызывается метод для чтения из файла класса KeyBoard
                    keyBoard.readFile();
                    keyBoard.Show();
                    this.Hide();
                }
                catch (Exception)
                {
                    MessageBox.Show("Что-то пошло не так!", "Уведомление");
                }
            }
            else MessageBox.Show("Файл не выбран!", "Уведомление");
        }

        //обработчик события при нажатии копки "Назад"
        private void BackBtn_Click(object sender, EventArgs e)
        {
            Choose choose = new Choose();
            choose.Show();
            this.Hide();
        }

        //обработчик события закрытия формы
        private void FileChoose_FormClosed(object sender, FormClosedEventArgs e)
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