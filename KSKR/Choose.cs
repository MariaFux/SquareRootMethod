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
    public partial class Choose : Form
    {        
        public Choose()
        {            
            InitializeComponent();
        }

        //обработчик нажатия на кнопку принять
        private void AcceptBtn_Click(object sender, EventArgs e)
        {            
            if (checkedListBox1.GetItemChecked(0))
            {
                Size size = new Size();
                size.Show();
                this.Hide();
            }
            if (checkedListBox1.GetItemChecked(1))
            {
                FileChoose fileChoose = new FileChoose();
                fileChoose.Show();
                this.Hide();
            }
            else if (!checkedListBox1.GetItemChecked(0) && !checkedListBox1.GetItemChecked(1))
            {
                MessageBox.Show("Выберите способ работы!", "Уведомление");
            }
        }

        //делает активным только один из checkedListBox
        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(checkedListBox1.CheckedItems.Count > 0)
            {
                for(int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    checkedListBox1.SetItemChecked(i, false);
                    checkedListBox1.SetItemChecked(checkedListBox1.SelectedIndex, true);
                }
            }
        }

        //срабатывает при закрытии формы
        private void Choose_FormClosed(object sender, FormClosedEventArgs e)
        {
            Dispose();
            Close();
            Application.Exit();
        }

        //обработчик кнопки выхода
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
