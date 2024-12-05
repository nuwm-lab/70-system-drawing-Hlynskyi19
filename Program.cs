using System;
using System.Drawing;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace GraphDrawer
{
    public class GraphForm : Form
    {
        private float startX = 2.5f;  // Початкове значення x
        private float endX = 9.0f;   // Кінцеве значення x
        private float step = 0.8f;   // Крок x
        private Color graphColor = Color.Blue; // Колір графіка
        private Button colorButton; // Кнопка для зміни кольору
        private Button drawButton;  // Кнопка для малювання графіка
        private bool drawGraph = false; // Прапорець для перевірки, чи малювати графік

        public GraphForm()
        {
            this.Text = "Графік функції";
            this.Size = new Size(800, 600);
            this.Resize += (sender, e) => this.Invalidate(); // Перемалювати при зміні розміру
            this.DoubleBuffered = true; // Уникнення мерехтіння

            // Кнопка для зміни кольору графіка
            colorButton = new Button
            {
                Text = "Змінити колір графіка",
                Location = new Point(140, 5),
                AutoSize = true
            };
            colorButton.Click += ChangeColor;
            this.Controls.Add(colorButton);

            // Кнопка для малювання графіка
            drawButton = new Button
            {
                Text = "Намалювати графік",
                Location = new Point(10, 5),
                AutoSize = true
            };
            drawButton.Click += (sender, e) =>
            {
                drawGraph = true; // Увімкнути малювання графіка
                this.Invalidate(); // Перемалювати форму
            };
            this.Controls.Add(drawButton);
        }

        // Метод для зміни кольору графіка
        private void ChangeColor(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    graphColor = colorDialog.Color;
                    if (drawGraph)
                    {
                        this.Invalidate(); // Перемалювати графік з новим кольором
                    }
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (!drawGraph) return;

            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            int width = this.ClientSize.Width;
            int height = this.ClientSize.Height;

            // Визначення масштабів
            float margin = 50; // Відступ від країв
            float graphWidth = width - 2 * margin;
            float graphHeight = height - 2 * margin;

            // Оси координат
            Pen axisPen = new Pen(Color.Black, 2);
            g.DrawLine(axisPen, margin, height / 2, width - margin, height / 2); // X-axis
            g.DrawLine(axisPen, margin, margin, margin, height - margin);        // Y-axis

            // Підготовка до обчислення графіка
            float scaleX = graphWidth / (endX - startX);
            float scaleY = graphHeight / 10.0f; // Умовний масштаб для Y

            // Визначення точок графіка
            PointF? prevPoint = null;
            for (float x = startX; x <= endX; x += step)
            {
                // Обчислення значення y
                float y = (1.5f * x - (float)Math.Log(2 * x)) / (3 * x + 1);

                // Перетворення координат в координати екрану
                float screenX = margin + (x - startX) * scaleX;
                float screenY = height / 2 - y * scaleY;

                PointF currentPoint = new PointF(screenX, screenY);

                // Малювання лінії
                if (prevPoint != null)
                {
                    using (Pen graphPen = new Pen(graphColor, 2))
                    {
                        g.DrawLine(graphPen, prevPoint.Value, currentPoint);
                    }
                }

                prevPoint = currentPoint;
            }

            Font font = new Font("Arial", 10);
            Brush brush = Brushes.Black;
            g.DrawString("X", font, brush, width - margin, height / 2 + 10);
            g.DrawString("Y", font, brush, margin - 20, margin);
        }

        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new GraphForm());
        }
    }
}