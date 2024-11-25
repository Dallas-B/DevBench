using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WPFProject.InputController;

namespace WPFProject
{
    public partial class MainWindow : Window
    {
        private Point startPoint;
        private RectangleGeometry selectionRectangle;
        private bool isSelectingArea = false;
        private bool isTransparent = false;
        private bool isDragging = false;

        public MainWindow()
        {
            InitializeComponent();
            InitializeSelectionRectangle();
        }

        private void ClearAndReset()
        {
            // Reset the selection state
            isSelectingArea = false;
            selectionRectangleRect.Visibility = Visibility.Collapsed;
            selectionRectangle.Rect = new Rect();

            // Restore the main window
            WindowState = WindowState.Normal;
            WindowStyle = WindowStyle.None;
            this.Opacity = 1;
            isTransparent = false;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void InitializeSelectionRectangle()
        {
            selectionRectangle = new RectangleGeometry();
            selectionRectangleRect.Fill = new SolidColorBrush(Color.FromArgb(50, 0, 0, 255));
            selectionRectangleRect.DataContext = selectionRectangle;
        }

        private void InjectInput_Click(object sender, RoutedEventArgs e)
        {
            // Parse the coordinates from the CoordinatesTextBox
            string[] lines = CoordinatesTextBox.Text.Split('\n');
            if (lines.Length == 2)
            {
                try
                {
                    // Extract the values for Top Left and Bottom Right
                    string[] topLeftValues = lines[0].Replace("Top Left: (", "").Replace(")", "").Split(',');
                    string[] bottomRightValues = lines[1].Replace("Bottom Right: (", "").Replace(")", "").Split(',');

                    int x1 = int.Parse(topLeftValues[0]);
                    int y1 = int.Parse(topLeftValues[1]);
                    int x2 = int.Parse(bottomRightValues[0]);
                    int y2 = int.Parse(bottomRightValues[1]);

                    // Determine the bounds for random number generation
                    int minX = Math.Min(x1, x2);
                    int maxX = Math.Max(x1, x2);
                    int minY = Math.Min(y1, y2);
                    int maxY = Math.Max(y1, y2);

                    // Generate random values within the bounds
                    Random random = new Random();
                    int randomX = random.Next(minX, maxX);
                    int randomY = random.Next(minY, maxY);

                    IInputInjector injector = new InputInjector();
                    injector.InjectMouseClick(randomX, randomY);
                }
                catch (FormatException)
                {
                    MessageBox.Show("Error: Invalid format in coordinates.");
                }
            }
            else
            {
                MessageBox.Show("Error: Coordinates are not properly formatted.");
            }
        }

        private void MaximizeRestoreButton_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            else
                WindowState = WindowState.Maximized;
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void SelectArea_Click(object sender, RoutedEventArgs e)
        {
            isSelectingArea = !isSelectingArea;

            if (isSelectingArea)
            {
                selectionRectangleRect.Visibility = Visibility.Visible;

                WindowState = WindowState.Maximized;
                WindowStyle = WindowStyle.None;

                this.Opacity = 0.5;
                isTransparent = true;
            }
            else
            {
                selectionRectangleRect.Visibility = Visibility.Collapsed;
                WindowState = WindowState.Normal;
                WindowStyle = WindowStyle.None;

                this.Opacity = 1;
                isTransparent = false;
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.GetPosition(this).Y <= 25)
            {
                isDragging = true;
                startPoint = e.GetPosition(this);
            }
            if (isSelectingArea)
            {
                startPoint = e.GetPosition(this);
                Canvas.SetLeft(selectionRectangleRect, startPoint.X);
                Canvas.SetTop(selectionRectangleRect, startPoint.Y);
                selectionRectangleRect.Width = 0;
                selectionRectangleRect.Height = 0;
                selectionRectangle.Rect = new Rect(startPoint, startPoint);
                selectionRectangleRect.Visibility = Visibility.Visible;
            }
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                isDragging = false;
            }
            if (isSelectingArea)
            {
                Point endPoint = e.GetPosition(this);

                double x1 = Math.Min(startPoint.X, endPoint.X);
                double y1 = Math.Min(startPoint.Y, endPoint.Y);
                double x2 = Math.Max(startPoint.X, endPoint.X);
                double y2 = Math.Max(startPoint.Y, endPoint.Y);

                CoordinatesTextBox.Text = $"Top Left: ({x1}, {y1})\nBottom Right: ({x2}, {y2})";

                ClearAndReset();
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                Point currentPosition = e.GetPosition(this);

                double differenceX = currentPosition.X - startPoint.X;
                double differenceY = currentPosition.Y - startPoint.Y;

                Left += differenceX;
                Top += differenceY;
            }
            if (isSelectingArea && e.LeftButton == MouseButtonState.Pressed)
            {
                Point currentPoint = e.GetPosition(this);

                double x = Math.Min(currentPoint.X, startPoint.X);
                double y = Math.Min(currentPoint.Y, startPoint.Y);
                double width = Math.Max(currentPoint.X, startPoint.X) - x;
                double height = Math.Max(currentPoint.Y, startPoint.Y) - y;

                selectionRectangleRect.Width = width;
                selectionRectangleRect.Height = height;

                selectionRectangle.Rect = new Rect(x, y, width, height);
            }
        }

    }
}
