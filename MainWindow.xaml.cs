using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Canvas = System.Windows.Controls.Canvas;

namespace GraWZycie
{
    public partial class MainWindow : Window
    {
        private GameEngine engine;
        private DispatcherTimer timer;
        private int rows = 100;
        private int cols = 100;

        private bool zoomedIn = true;
        private int cellSize = 10;

        public MainWindow()
        {
            InitializeComponent();
            engine = new GameEngine(rows, cols);
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(150);
            timer.Tick += Tick;
            Draw();
        }

        private void Tick(object sender, EventArgs e)
        {
            engine.NextGeneration();
            Draw();
        }

        private void Draw()
        {
            BoardCanvas.Children.Clear();
            bool circle = StyleBox.SelectedIndex == 1;

            for (int r = 0; r < engine.Rows; r++)
            {
                for (int c = 0; c < engine.Cols; c++)
                {
                    if (!engine.Board[r, c]) continue;

                    Shape shape;
                    if (circle)
                        shape = new Ellipse { Width = cellSize - 1, Height = cellSize - 1, Style = (Style)FindResource("CellEllipseStyle") };
                    else
                        shape = new Rectangle { Width = cellSize - 1, Height = cellSize - 1, Style = (Style)FindResource("CellStyle") };

                    Canvas.SetLeft(shape, c * cellSize);
                    Canvas.SetTop(shape, r * cellSize);

                    // Fade-in animacja
                    shape.Opacity = 0;
                    DoubleAnimation fade = new DoubleAnimation { From = 0, To = 1, Duration = TimeSpan.FromMilliseconds(300) };
                    shape.BeginAnimation(UIElement.OpacityProperty, fade);

                    BoardCanvas.Children.Add(shape);
                }
            }

            GenText.Text = $"Gen: {engine.Generation}";
            AliveText.Text = $"Alive: {engine.AliveCount}";
            BornText.Text = $"Born: {engine.BornCount} (Σ {engine.TotalBorn})";
            DeadText.Text = $"Dead: {engine.DeadCount} (Σ {engine.TotalDead})";
        }

        private void ZoomButton_Click(object sender, RoutedEventArgs e)
        {
            zoomedIn = !zoomedIn;
            cellSize = zoomedIn ? 10 : 4;
            Title = zoomedIn ? "Gra w Życie - Zoom BIG" : "Gra w Życie - Zoom SMALL";
            Draw();
        }

        private void SpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (timer != null) timer.Interval = TimeSpan.FromMilliseconds(SpeedSlider.Value);
        }

        private void StartButton_Click(object sender, RoutedEventArgs e) => timer.Start();
        private void StopButton_Click(object sender, RoutedEventArgs e) => timer.Stop();
        private void StepButton_Click(object sender, RoutedEventArgs e) { engine.NextGeneration(); Draw(); }
        private void ClearButton_Click(object sender, RoutedEventArgs e) { engine.Clear(); Draw(); }
        private void RandomButton_Click(object sender, RoutedEventArgs e) { engine.Randomize(); Draw(); }

        private void ResizeBoard_Click(object sender, RoutedEventArgs e)
        {
            rows = int.Parse(RowsBox.Text);
            cols = int.Parse(ColsBox.Text);
            engine = new GameEngine(rows, cols);
            Draw();
        }

        private void BoardCanvas_MouseDown(object sender, MouseButtonEventArgs e) => Toggle(e);
        private void BoardCanvas_MouseMove(object sender, MouseEventArgs e) { if (e.LeftButton == MouseButtonState.Pressed) Toggle(e); }
        private void Toggle(MouseEventArgs e)
        {
            var p = e.GetPosition(BoardCanvas);
            int r = (int)(p.Y / cellSize);
            int c = (int)(p.X / cellSize);
            if (r >= 0 && r < engine.Rows && c >= 0 && c < engine.Cols)
            {
                engine.ToggleCell(r, c);
                Draw();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog { Filter = "Plik (*.txt)|*.txt" };
            if (dialog.ShowDialog() == true)
            {
                using var writer = new StreamWriter(dialog.FileName);
                writer.WriteLine($"{engine.Rows} {engine.Cols}");
                for (int r = 0; r < engine.Rows; r++)
                {
                    for (int c = 0; c < engine.Cols; c++) writer.Write(engine.Board[r, c] ? "1 " : "0 ");
                    writer.WriteLine();
                }
            }
        }

        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog { Filter = "Plik (*.txt)|*.txt" };
            if (dialog.ShowDialog() == true)
            {
                var lines = File.ReadAllLines(dialog.FileName);
                var parts = lines[0].Split(' ');
                rows = int.Parse(parts[0]);
                cols = int.Parse(parts[1]);
                engine = new GameEngine(rows, cols);
                for (int r = 1; r <= rows; r++)
                {
                    var row = lines[r].Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    for (int c = 0; c < cols; c++) engine.Board[r - 1, c] = row[c] == "1";
                }
                Draw();
            }
        }
    }
}