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
    public partial class KeyBoard : Form
    {
        bool work = true;
        bool choice = false;
        int count = 0;
        StreamReader streamReader;
        int i, j, n, m;
        int x, y;        
        int s;
        double[,] a;
        double[] b;
        double[] X;
        double[,] S;
        string fileName;
        string savePath;
        ComboBox comboBox = new ComboBox();

        public KeyBoard()
        {
            InitializeComponent();            
        }

        //определяет детерминант матрицы
        private bool matrixDeterminant(double[,] matrix)
        {
            double det = 1;
            //определяем переменную EPS
            const double EPS = 1E-9;
            //определяем массив размером nxn
            double[][] a = new double[s][];
            double[][] b = new double[1][];
            b[0] = new double[s];
            //заполняем его;
            for (int i = 1; i < s; i++)
            {
                a[i] = new double[s];
                for (int j = 1; j < s; j++)
                    a[i][j] = matrix[i,j];
            }
            //проходим по строкам
            for (int i = 1; i < s; ++i)
            {
                //присваиваем k номер строки
                int k = i;
                //идем по строке от i+1 до конца
                for (int j = i + 1; j < s; ++j)
                    if (Math.Abs(a[j][i]) > Math.Abs(a[k][i]))
                        //k присваиваем j
                        k = j;
                if (Math.Abs(a[k][i]) < EPS)
                {
                    //если равенство выполняется то определитель приравниваем к 0 и выходим из программы
                    det = 0;
                    break;
                }
                //меняем местами a[i] и a[k]
                b[0] = a[i];
                a[i] = a[k];
                a[k] = b[0];
                if (i != k)
                    //меняем знак определителя
                    det = -det;
                det *= a[i][i];
                //идем по строке от i+1 до конца
                for (int j = i + 1; j < s; ++j)
                    //каждый элемент делим на a[i][i]
                    a[i][j] /= a[i][i];
                //идем по столбцам
                for (int j = 1; j < s; ++j)
                    if ((j != i) && (Math.Abs(a[j][i]) > EPS))
                        //идем по k от i+1
                        for (k = i + 1; k < s; ++k)
                            a[j][k] -= a[i][k] * a[j][i];
            }
            //возвращаем результат
            return det == 0 ? false : true;            
        }
        
        //определяет главную диагональ
        private bool mainDiagonal(double[,] matrix)
        {            
            for (int i = 1; i < s; i++)
            {
                //присвоим максимальному первое значение
                double max = matrix[i, i];
                for (int j = 1; j < s; j++)
                {
                    if (max < matrix[i, j])
                        //присвоим максимальному новое значение
                        max = a[i, j];
                }
                //если максимальный элемент не стоит по диагонали
                if (max != matrix[i, i])
                    return false;
            }
            return true;
        }

        Choose choose = new Choose();

        TextBox[,] textBoxes;
        Label[,] labels;

        //позволяет отделить метод чтения из файла от метода ввода с клавиатуры
        public bool Choose(bool ch)
        {
            choice = ch;
            return choice;
        }

        //забирает размерность для метода ввода с клавиатуры
        public int Sizable(int p)
        {
            s = p;
            return s+=1;
        }

        //метод создания кнопок и выпадающего меню
        public void CreateButtons()
        {            
            this.Controls.Add(comboBox);
            comboBox.Location = new Point(x+=30, y);
            comboBox.Width = 30;
            comboBox.Height = 23;
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;            
            comboBox.Items.AddRange(new string[] { "0", "1", "2", "3", "4", "5" });
            comboBox.Text = "3";

            Button SolveBtn = new Button();
            this.Controls.Add(SolveBtn);
            SolveBtn.Location = new Point(x += 40, y -= 1);
            SolveBtn.Width = 75;
            SolveBtn.Height = 23;
            SolveBtn.Text = "Решить";
            SolveBtn.Click += new EventHandler(SolveBtn_Click);

            Button LoadBtn = new Button();
            this.Controls.Add(LoadBtn);
            LoadBtn.Location = new Point(x -= 40, y += 40);
            LoadBtn.Width = 118;
            LoadBtn.Height = 23;
            LoadBtn.Text = "Записать в файл";
            LoadBtn.Click += new EventHandler(LoadBtn_Click);

            Button BackBtn = new Button();
            this.Controls.Add(BackBtn);
            BackBtn.Location = new Point(x += 128, y);
            BackBtn.Width = 80;
            BackBtn.Height = 23;
            BackBtn.Text = "Назад";
            BackBtn.Click += new EventHandler(BackBtn_Click);

            Button ExitBtn = new Button();
            this.Controls.Add(ExitBtn);
            ExitBtn.Location = new Point(x -= 128, y+=40);
            ExitBtn.Width = 80;
            ExitBtn.Height = 23;
            ExitBtn.Text = "Выход";
            ExitBtn.Click += new EventHandler(ExitBtn_Click);
        }

        //метод создания текстовых полей
        public void CreateTextBoxes(int ii, int j)
        {
            textBoxes[ii, j] = new TextBox();
            this.Controls.Add(textBoxes[ii, j]);
            textBoxes[ii, j].Location = new Point(x += 60, y);
            textBoxes[ii, j].Width = 25;
            textBoxes[ii, j].Height = 20;
            textBoxes[ii, j].KeyPress += new KeyPressEventHandler(textBoxes_KeyPress);
        }

        //метод создания меток
        public void CreateLabels()
        {
            labels[n, m] = new Label();
            this.Controls.Add(labels[n, m]);
            labels[n, m].Location = new Point(x += 60, y);
            labels[n, m].Width = 40;
            labels[n, m].Height = 40;
        }        

        //метод для построения окна для ввода системы с клавиатуры
        public void keyEnter()
        {            
            x = 0;
            y = 70;
            //создаем массив текстовых полей
            textBoxes = new TextBox[s, s];
            //создаем массив матрицы А
            a = new double[s+1, s+1];
            //создаем массив столбца свободных членов
            b = new double[s];
            for (i = 0; i < s - 1; i++)
            {
                for (j = 0; j < s; j++)
                {
                    textBoxes[i, j] = new TextBox();
                    //добавляем новое текстовое поле в форму
                    this.Controls.Add(textBoxes[i, j]);
                    textBoxes[i, j].Location = new Point(x += 60, y);
                    textBoxes[i, j].Width = 25;
                    textBoxes[i, j].Height = 20;
                    //привязываем к нему обработ события
                    textBoxes[i, j].KeyPress += new KeyPressEventHandler(textBoxes_KeyPress);
                }
                y += 40;
                x = 0;
            }
            y = 72;
            x = 29;

            //создаем массив меток
            labels = new Label[s, s];
            //в цикле будут добавляться метки для создания подобающего вида системе уравнений
            for (n = 0; n < s - 1; n++)
            {
                int k = s - 1;
                for (m = 0; m < s; m++)
                {
                    //создается новая метка
                    CreateLabels();
                    int z = m + 1;
                    if (m == s - 2)                        
                        labels[n, m].Text = "x" + z + " =";
                    else if (m == s - 1)
                        continue;
                    else
                        labels[n, m].Text = "x" + z + " +";
                }
                y += 40;
                x = 29;
            }
            //создаются кнопки
            CreateButtons();
            AutoSize = true;
            ClientSize = new System.Drawing.Size(Width, Height);
            AutoSize = false;

            //создается массив для хранения корней уравнений
            X = new double[s];
            //создается массив для хранения треугольной матрицы
            S = new double[s+1, s+1]; 
        }

        //забирает путь для метода чтения из файла
        public string FileName(string filePath)
        {
            fileName = filePath;
            return fileName;
        }

        //метод для построения окна для чтения системы из файла
        public void readFile()
        {
            try
            {
                x = 0;
                y = 0;
                //создаем поток для чтения из файла
                streamReader = new StreamReader(@""+fileName);
                //преобразовываем прочтенную строку в Integer
                s = Convert.ToInt32(streamReader.ReadLine());
                s += 1;
                //создаем массив текстовых полей
                textBoxes = new TextBox[s+1, s+1];
                //создаем массив матрицы А
                a = new double[s + 1, s + 1];
                //создаем массив столбца свободных членов
                b = new double[s];

                //читаем в переменную line все строки из файла
                foreach (string line in File.ReadAllLines(@"..\..\..\KSKR\files\input.txt"))
                {
                    i++;
                    //чтение массива из файла без значения размерности
                    if (i != 1)
                    {
                        string[] arr = line.Trim().Split(' ');
                        //проверка на совпадение длины массива из файла с размерностью
                        if (arr.Length > s)
                        {
                            MessageBox.Show("Проверьте данные в файле! Возможно в " +
                                "какой-то строке больше значений, чем нужно!", 
                                "Уведомление");
                            Application.Exit();
                        }
                        //проверка на совпадение размерности с длиной массива из файла
                        if(s > arr.Length)
                        {
                            MessageBox.Show("Проверьте данные в файле!", "Уведомдение");
                            Application.Exit();
                        }
                        //проход по массиву из файла за вычетом столбца свободных членов
                        for (j = 0; j < arr.Length - 1; j++)
                        {
                            //заполнение матрицы А
                            a[i - 1, j + 1] = Convert.ToDouble(arr[j]);
                            //если размерность матрицы < 12, создаются текстовые поля
                            if (s < 12)
                            {
                                CreateTextBoxes(i, j);
                                textBoxes[i, j].Text = Convert.ToString(a[i - 1, j + 1]);
                            }
                        }
                        //заполнение столбца свободных членов B
                        b[i - 1] = Convert.ToDouble(arr[arr.Length - 1]);
                        //если размерность матрицы < 12, создаются текстовые поля
                        if (s < 12)
                        {
                            CreateTextBoxes(i, j);
                            textBoxes[i, j].Text = Convert.ToString(b[i - 1]);
                        }
                    }
                    y += 40;
                    x = 18;
                }
                y = 42;
                x = 47;

                //если размерность матрицы < 12 создаются метки для текстовых полей
                if (s < 12)
                {
                    labels = new Label[s, s];
                    for (n = 0; n < s - 1; n++)
                    {
                        for (m = 0; m < s; m++)
                        {
                            CreateLabels();
                            int z = m + 1;
                            if (m == s - 2)
                                labels[n, m].Text = "x" + z + " =";
                            else if (m == s - 1)
                                continue;
                            else
                                labels[n, m].Text = "x" + z + " +";
                        }
                        y += 40;
                        x = 47;
                    }
                }

                //создаются кнопки
                CreateButtons();
                AutoSize = true;
                ClientSize = new System.Drawing.Size(Width, Height);
                AutoSize = false;

                //создается массив для хранения корней уравнений
                X = new double[s + 1];
                //создается массив для хранения треугольной матрицы
                S = new double[s + 1, s + 1];                             
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("Файл не найден", e.Message);
            }            
        }

        //метод решения системы линейных уравнений
        public void Solve()
        {
            //если был выбран ввод с клавиатуры
            if (choice)
            {
                //создаем массив для хранения слау
                double[,] q = new double[s - 1, s];
                for (int i = 0; i < s - 1; i++)
                    for (int j = 0; j < s; j++)
                        //записываем массив слау
                        q[i, j] = Convert.ToDouble(textBoxes[i, j].Text);
                for (int i = 1; i < s; i++)
                    for (int j = 1; j < s; j++)
                        //заполняем массив матрицы А
                        a[i, j] = q[i - 1, j - 1];                

                //элемент S11 треугольной матрицы
                S[1, 1] = Math.Sqrt(Math.Abs(a[1, 1]));
                for (int i = 1, j = s - 1; i < s; i++)
                    //заполняем столбец свободных членов B
                    b[i] = q[i - 1, j];
            }

            for (int i = 1; i <= s - 1; i++)
            {
                for (int j = 1; j <= s - 1; j++)
                {
                    //если матрица А не является невырожденной
                    if (a[i, j] != a[j, i])
                    {
                        Conditions conditions = new Conditions();
                        conditions.Show();
                        work = false;
                        return;
                    }
                    //если определитель матрицы А равен нулю
                    else if (!matrixDeterminant(a))
                    {
                        //если был выбран ввод с клавиатуры
                        if (choice)
                        {
                            MessageBox.Show("Матрица не явлется невырожденной, определитель равен нулю!", "Уведомдение");
                            work = false;
                            return;
                        }
                        else
                        {
                            MessageBox.Show("Матрица не явлется невырожденной, определитель равен нулю! " +
                                "Проверьте данные в файле!", "Уведомление");
                            this.Dispose();
                            Application.Exit();
                        }                        
                    }
                    //если диагональ не является преобладающей
                    if (!mainDiagonal(a))
                    {
                        //если был выбран ввод с клавиатуры
                        if (choice)
                        {
                            MessageBox.Show("Главная диагональ не преобладающая!", "Уведомление");
                            work = false;
                            return;
                        }
                        else
                        {
                            MessageBox.Show("Главная диагональ не преобладающая! " +
                                "Проверьте данные в файле!", "Уведомление");
                            this.Dispose();
                            Application.Exit();
                        }
                    }

                    work = true;
                    //если матрица А вырожденная находим треугольную матрицу
                    if (a[i, j] == a[j, i] && work == true)
                    {
                        //для первой строки
                        if (i == 1)
                        {
                            //если был выбран ввод с клавиатуры
                            if (choice)
                                //элементы S1j треугольной матрицы
                                S[i, j] = a[i, j] / S[i, i];
                            else
                            {
                                //элемент S11 треугольной матрицы
                                S[1, 1] = Math.Sqrt(Math.Abs(a[1, 1]));
                                //элементы s1j треугольной матрицы
                                S[i, j] = a[i, j] / S[i, i];
                            }
                        }
                        //для строки равной столбцу
                        else if (i == j)
                        {
                            double u = 0;
                            for (int k = 1; k <= i - 1; k++)
                            {
                                //сумма элементов Ski в квадрате треугольной матрицы
                                u = u + S[k, i] * S[k, i];
                                //если выражение из которого будет извлекаться корень меньше нуля
                                if ((a[i, j] - u) < 0)
                                {
                                    //если был выбран ввод с клавиатуры
                                    if (choice)
                                    {
                                        MessageBox.Show("Под корнем получилось отрицательное значение. " +
                                        "Решение данного СЛАУ методом квадратного корня невозможно!", "Уведомление");
                                        work = false;
                                        return;
                                    }
                                    else
                                    {
                                        MessageBox.Show("Под корнем получилось отрицательное значение. " +
                                        "Решение данного СЛАУ методом квадратного корня невозможно! Проверьте данные введенные в файл", "Уведомление");
                                        this.Close();
                                        this.Dispose();
                                        Application.Exit();
                                    }
                                }
                                else
                                    //элементы sii треугольной матрицы
                                    S[i, j] = Math.Sqrt(a[i, j] - u);
                            }
                        }
                        //если строка меньше столбца
                        else if (i < j)
                        {
                            double u = 0;
                            for (int k = 1; k <= i - 1; k++)
                            {
                                //сумма элементов Ski умноженное на Skj треугольной матрицы
                                u = u + S[k, i] * S[k, j];
                                //элементы sij треугольной матрицы
                                S[i, j] = (a[i, j] - u) / S[i, i];
                            }
                        }
                        //если строка больше столбца
                        else if (i > j)
                            //элементы sij треугольной матрицы буду равны нулю
                            S[i, j] = 0;
                    }
                }
            }

            //нахождение взаимно транспонированной треугольной матрицы
            for (int i = 0; i < s; i++)
                for (int j = 0; j < s; j++)
                {
                    double temp = S[i, j];
                    S[i, j] = S[j, i];
                    S[j, i] = temp;
                }

            //решение системы s^t * y = b
            double[] Y = new double[s];
            Y[1] = b[1] / S[1, 1];
            for (i = 2; i <= s - 1; i++)
            {
                double sum = 0;
                for (j = 1; j <= i - 1; j++)
                    sum += S[j, i] * Y[j];
                Y[i] = (b[i] - sum) / S[i, i];
            }

            //решение системы s * x = y
            X[s - 1] = (Y[s - 1] / S[s - 1, s - 1]);
            for (i = s - 2; i >= 1; i--)
            {
                double sum = 0;
                for (int k = i + 1; k <= s - 1; k++)
                    sum += (S[i, k] * X[k]);
                X[i] = (Y[i] - sum) / S[i, i];
            }            
        }
        
        //метод записи корней в файл
        public void WriteFile(int s, double[] X)
        {
            StreamWriter sw = new StreamWriter(@""+savePath);
            for (int i = 1; i <= s - 1; i++)
                for (int j = 0; j < 1; j++)
                    sw.WriteLine($"{X[i]}\n");
            sw.Flush();
            sw.Close();
        }

        //обработчик события нажатия на кнопку "Решить"
        public void SolveBtn_Click(object sender, EventArgs e)
        {
            if (s >= 7)
            {
                AutoScroll = false;
                HorizontalScroll.Enabled = false;
                HorizontalScroll.Visible = false;
                HorizontalScroll.Maximum = 0;
                AutoScroll = true;
            }
            try
            {
                //вызов метода для решения
                Solve();
                if (work == false)
                    return;
                else
                {               
                    y += 50;
                    //для чтения из файла создаем метки для текстовых полей с корнями уравнений
                    if (!choice && count < 1 && s >= 12)
                        labels = new Label[s, s];
                    for (int i = 1; i <= s - 1; i++)
                    {
                        for (int j = 0; j < 1; j++)
                        {
                            //отрисовка меток с корнями один раз
                            if (count < 1)
                            {                                
                                x = 50;
                                labels[i, j] = new Label();
                                this.Controls.Add(labels[i, j]);
                                labels[i, j].Location = new Point(x += 30, y);
                                labels[i, j].Width = Width;
                                labels[i, j].Height = 40;                                
                                ClientSize = new System.Drawing.Size(Width, Height);
                            }
                            //записываем в переменную значение точности
                            int p = Convert.ToInt32(comboBox.Text);
                            int degree = Convert.ToInt32(X[i] * Math.Pow(10, p));
                            //присваиваем корню значение с заданной точностью
                            X[i] = degree / Math.Pow(10, p);
                            //создаем метки с корнями уравнений
                            labels[i, j].Text = $"x{i} = {X[i]}\n";

                        }
                        y += 40;
                        x = 47;
                    }                    
                    count++;
                }
            }
            catch (Exception ex)
            {                
                MessageBox.Show("Проверьте правильность введенных данных", "Уведомление");
            }
        }

        //обработчик события нажатия на кнопку "Загрузить в файл"
        public void LoadBtn_Click(object sender, EventArgs e)
        {
            saveFileDialog1.InitialDirectory = @"..\..\..\KSKR\files";
            saveFileDialog1.Filter = "(*.txt)|*.txt";
            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel) 
                return; 
            savePath = saveFileDialog1.FileName;
            //вызов метода для записи в файл
            WriteFile(s, X);
            MessageBox.Show("Файл сохранен!", "Уведомление");            
        }

        //обработчик события нажатия на кнопку "Назад"
        public void BackBtn_Click(object sender, EventArgs e)
        {           
            choose.Show();
            this.Hide();
        }

        //обработчик события нажатия на кнопку "Выход"
        public void ExitBtn_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите выйти?", "Уведомление",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.Dispose();
                this.Close();
                Application.Exit();
            }
        }

        //запрещает пользователю ввод символов
        public void textBoxes_KeyPress(object sender, KeyPressEventArgs e)
        {
            char symbol = e.KeyChar;
            if ((symbol <= 47 || symbol >= 58) && symbol != 8 && symbol != 44 && symbol != 45)
            {
                e.Handled = true;
            }
        }

        //обработчик события закрытия формы
        private void KeyBoard_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Dispose();            
            this.Close();
            Application.Exit();
        }
    }    
}