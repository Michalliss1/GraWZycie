using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GraWZycie
{
    public class BoardRenderer
    {
        private Canvas canvas;
        private GameEngine engine;
        private int cellSize;

        public BoardRenderer(Canvas canvas, GameEngine engine, int cellSize)
        {
            this.canvas = canvas;
            this.engine = engine;
            this.cellSize = cellSize;
        }

        public void SetCellSize(int size)
        {
            cellSize = size;
        }

        public void Render()
        {
            canvas.Children.Clear();

            for (int r = 0; r < engine.Rows; r++)
            {
                for (int c = 0; c < engine.Cols; c++)
                {
                    if (!engine.Board[r, c]) continue;

                    var rect = new Rectangle
                    {
                        Width = cellSize - 1,
                        Height = cellSize - 1,
                        Fill = Brushes.Lime
                    };

                    Canvas.SetLeft(rect, c * cellSize);
                    Canvas.SetTop(rect, r * cellSize);

                    canvas.Children.Add(rect);
                }
            }
        }
    }
}