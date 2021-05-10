using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace K_means
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            DoubleBuffered = true;
            timer1.Enabled = true;
            textInfoDebug.Text = "1. Чтобы расположить точки на поле, используйте ЛКМ.\r\n" +
                "2. Чтобы удалить последнюю точку, используйте ПКМ.\r\n" +
                "3. Укажите количество требуемых операций и предпологаемое количество кластеров\r\n" +
                "4. Нажмите Пуск";
            ReDraw();
        }

        List<Dot> list = new();
        List<Dot> means = new();
        bool flag = true;
        Bitmap bitmap;
        ClusterShell drawingClusters = new();
        ClusterShell bestCLusterShell = new();
        int aligmentItets = 300;
        int itersRemaining = 0;
        List<Color> colors = new List<Color> { Color.Red, Color.Green, Color.Blue, Color.Orange, Color.Magenta, Color.Black, Color.Aquamarine, Color.Gray, Color.Beige};
        bool isCalculating = false;
        


        private void ReDraw()
        {
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                lock (drawingClusters.Clusters)
                {
                    g.Clear(pictureBox1.BackColor);
                    foreach (var dot in list)
                    {
                        g.DrawRectangle(new Pen(dot.Color, 3), dot.X, dot.Y, 3, 3);
                    }
                    foreach (var cluster in drawingClusters.Clusters)
                    {
                        var dot = cluster.Mean;
                        g.DrawEllipse(new Pen(dot.Color, 5), dot.X, dot.Y, 5, 5);
                    }

                    foreach (var cluster in drawingClusters.Clusters)
                    {
                        foreach (var dot in cluster.Dots)
                        {
                            g.DrawRectangle(new Pen(dot.Color, 3), dot.X, dot.Y, 3, 3);
                        }
                    }
                }
            }
            pointCount.Text = list.Count.ToString();
            pictureBox1.Image = bitmap;
            pictureBox1.Refresh();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        { 
            if (e.Button == MouseButtons.Left)
            {
                if (flag)
                {
                    list.Add(new Dot(e.X, e.Y, Color.Gray));
                }

            }
            else if(e.Button == MouseButtons.Right && list.Count != 0)
            {
                list.RemoveAt(list.Count - 1);
            }

            ReDraw();
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            bestCLusterShell = new();
            drawingClusters = new();
            list.Clear();
            ReDraw();
            textInfoDebug.Text = "1. Чтобы расположить точки на поле, используйте ЛКМ.\r\n" +
                        "2. Чтобы удалить последнюю точку, используйте ПКМ.\r\n" +
                        "3. Укажите количество требуемых операций и предпологаемое количество кластеров\r\n" +
                        "4. Нажмите Пуск";
        }
        private void textIters_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(char.IsDigit(e.KeyChar) || e.KeyChar == (char)Keys.Back);
        }
        private void textClusterCount_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(char.IsDigit(e.KeyChar) || e.KeyChar == (char)Keys.Back);
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            if (textIterations.Text == "")
            {
                textInfoDebug.Text = "Ошибка. Поле с количеством итераций не содержит данных";
                return;
            }
            if (textClusterCount.Text == "")
            {
                textInfoDebug.Text = "Ошибка. Поле с количеством кластеров не содержит данных";
                return;
            }
            
            bestCLusterShell = new();
            drawingClusters = new();
            buttonStart.Enabled = false;
            isCalculating = true;
            itersRemaining = int.Parse(textIterations.Text);
            progressBar1.Maximum = itersRemaining;
            progressBar1.Minimum = 0;
            progressBar1.Value = 0;
            progressBar1.Step = 1;
        }

        async private void timer1_Tick(object sender, EventArgs e)
        {       
            if (isCalculating && itersRemaining > 0)
            {
                timer1.Enabled = false;
                SetProgressNoAnimation(progressBar1, progressBar1.Maximum - itersRemaining);
                Application.DoEvents();
                var result = await Task.Run(Calculate);
                textInfoDebug.Text = "Алгоритм выполняется...\r\n" + result;
                itersRemaining--;
                ReDraw();
                timer1.Enabled = true;
            }
            else if(isCalculating)
            {
                SetProgressNoAnimation(progressBar1, progressBar1.Maximum - itersRemaining);
                isCalculating = false;
                buttonStart.Enabled = true;
                textInfoDebug.Text = $"Алгоритм выполнен!\r\n" +
                    $"На поле показана расстановка с минимальным V\r\n" +
                    $"V = {Math.Round(bestCLusterShell.V)} px" +
                    $"\r\n\r\nМожете закрывать программу, или начать заново, нажав Очистить";
                ReDraw();
            }
        }

        private string Calculate()
        {
            var rnd = new Random();

            var bufshell = new ClusterShell();
            for (var k = 0; k < int.Parse(textClusterCount.Text); k++)
            {
                var cluster = new Cluster(new Dot(rnd.Next(0, pictureBox1.Width), rnd.Next(0, pictureBox1.Height)), colors[k]);
                bufshell.Add(cluster);
            }
            lock (drawingClusters.Clusters)
            {
                for (var j = 0; j < aligmentItets; j++)
                {
                    // добавление точки к ближайшему кластеру
                    bufshell.ClearClusters();
                    foreach (var dot in list)
                    {
                        var nearestCluster = bufshell.Clusters.First();

                        foreach (var cluster in bufshell.Clusters)
                        {
                            if (dot.DistanceTo(cluster.Mean) < dot.DistanceTo(nearestCluster.Mean))
                            {
                                nearestCluster = cluster;
                            }
                        }
                        nearestCluster.Add(dot);
                    }

                    foreach (var cluster in bufshell.Clusters)
                    {
                        cluster.RecalculateMean();
                    }

                    drawingClusters.Clusters.Clear();
                    foreach (var cluster in bufshell.Clusters) 
                    {
                        drawingClusters.Clusters.Add(cluster);
                    }
                }
            }

            var outStrInfo = "";
            outStrInfo += $"Текущее V: {Math.Round(bufshell.V)} px\r\n" +
                $"Минимальное V: {Math.Round(bestCLusterShell.V)} px\r\n";
            if (bestCLusterShell.V == 0 || bestCLusterShell.V > bufshell.V)
            {
                bestCLusterShell = bufshell;
            }

            
            return outStrInfo;
        }

        private static void SetProgressNoAnimation(ProgressBar pb, int value)
        {
            if (value >= pb.Maximum)
            {
                // Special case as value can't be set greater than Maximum.
                pb.Maximum = value + 1;     // Temporarily Increase Maximum
                pb.Value = value + 1;       // Move past
                pb.Maximum = value;         // Reset maximum
            }
            else
            {
                pb.Value = value + 1;       // Move past
            }
            pb.Value = value;               // Move to correct value
        }
    }
}
