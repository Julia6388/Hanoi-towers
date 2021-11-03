using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HTowers
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            groupBox1.AllowDrop = groupBox2.AllowDrop = groupBox3.AllowDrop = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Random r = new Random();
            int n = (int)numericUpDown1.Value,
                w = groupBox1.Width,
                h = groupBox1.Height;
            for (int i=0;i<n;i++)
            {
                Label lb = new Label();                
                lb.BorderStyle = BorderStyle.FixedSingle;
                lb.Size = new Size((w - 10) * (n - i) / n, (h - 15) / n);
                lb.Location = new Point((w - lb.Width) / 2, h - 2 - (i + 1) * lb.Height);
                lb.BackColor = Color.FromArgb(r.Next(256), r.Next(256), r.Next(256));
                groupBox1.Controls.Add(lb);
                lb.MouseDown += label_MouseDown;
            }
            count = 0;
            minCount = (int)Math.Round(Math.Pow(2, n)) - 1;
            Info();
            label2.Visible = false;
        }


        private void label_MouseDown(object sender, MouseEventArgs e)
        {
            if (!button1.Enabled)
                return;
            if (e.Button == MouseButtons.Left)
                DoDragDrop(sender, DragDropEffects.Move);
        }
        //Перемещение блока
        private void label_Move(Label lb, GroupBox gb)
        {
            lb.Parent = gb;
            lb.Top = gb.Height - 2 - gb.Controls.Count * lb.Height; ;
            count++;
            Info();
            if (groupBox2.Controls.Count == numericUpDown1.Value || groupBox3.Controls.Count == numericUpDown1.Value)
                label2.Visible = true;
        }
        // Удаление башни для перерисовки
        private void label_Dispose (GroupBox gb)
        {
            for (int i = gb.Controls.Count - 1; i >= 0; i--)
                gb.Controls[i].Dispose();
        }
        //запуск формы заново при изменении количества блоков
        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            label_Dispose (groupBox1);
            label_Dispose(groupBox2);
            label_Dispose(groupBox3);
            Form1_Load(this, null);
        }
        //Перетаскивание на другой бокс + нельзя перетаскивать нижние блоки
        // + нельзя ставить маленький на большой
        private void groupBox1_DragEnter (object sender,DragEventArgs e)
        {
            Label lb = e.Data.GetData(typeof(Label)) as Label;
            int k = int.MaxValue;
            GroupBox gb = sender as GroupBox;
            if(gb.Controls.Count>0)
                k = gb.Controls[gb.Controls.Count - 1].Width;
            if (lb.Parent.Controls[lb.Parent.Controls.Count-1] != lb || lb.Width > k)
                e.Effect = DragDropEffects.None;
            else  e.Effect = DragDropEffects.Move;
            if (label2.Visible)
            {
                e.Effect = DragDropEffects.None;
                return;
            }
        }
        //пеирерисовка в другой блок
        private void groupBox1_DragDrop(object sender, DragEventArgs e)
        {
            Label lb = e.Data.GetData (typeof(Label)) as Label;
            GroupBox gb = sender as GroupBox;
            if (gb == lb.Parent)
                return;
            label_Move(lb, gb);
        }
        private int count;
        private int minCount;
        // Подсчет количества шагов
        private void Info()
        {
            label1.Text = string.Format("Число ходов: {0} ({1})", count, minCount);
        }
        private void Step (int n, GroupBox src,GroupBox dst, GroupBox tmp)
        {
            if (n == 0)
                return;
            Step(n - 1, src, tmp, dst);
            if (button1.Enabled)
                return;
            label_Move(src.Controls[src.Controls.Count - 1] as Label, dst);
            Application.DoEvents(); ;
            System.Threading.Thread.Sleep(1500 / ((int)numericUpDown1.Value) - 1);
            Step(n - 1, tmp, dst, src);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            numericUpDown1.Enabled = button1.Enabled = !button1.Enabled;
            if(!button1.Enabled)
            {
                if (groupBox1.Controls.Count != numericUpDown1.Value)
                    NumericUpDown1_ValueChanged(null, null);
                Step((int)numericUpDown1.Value, groupBox1, groupBox3, groupBox2);
                numericUpDown1.Enabled = button1.Enabled = true;
            }
        }
        private void Form1_Form1Cloused (object senger, FormClosedEventArgs e)
        {
            button1.Enabled = true;
        }
    }
}
