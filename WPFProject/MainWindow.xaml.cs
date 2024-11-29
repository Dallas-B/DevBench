using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
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
        private DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();
            InitializeSelectionRectangle();
            InitializeClock();
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
            // Hook up events for selection
            SelectionCanvas.MouseLeftButtonDown += Window_MouseLeftButtonDown;
            SelectionCanvas.MouseMove += Window_MouseMove;
            SelectionCanvas.MouseLeftButtonUp += Window_MouseLeftButtonUp;

            selectionRectangle = new RectangleGeometry();
            selectionRectangleRect.Fill = new SolidColorBrush(Color.FromArgb(50, 0, 0, 255));
            selectionRectangleRect.DataContext = selectionRectangle;
        }

        private void InjectInput_Click(object sender, RoutedEventArgs e)
        {
            int[] MinMaxValues = ParseCoordinates();

            // Get the number of times to generate random values
            if (!int.TryParse(NumberOfInputsTextBox.Text, out int inputCount) || inputCount <= 0)
            {
                MessageBox.Show("Please enter a valid positive integer for the number of inputs.");
                return;
            }

            for (int i = 0; i < inputCount; i++)
            {
                // Generate random values within the bounds
                Random random = new Random();
                int randomX = random.Next(MinMaxValues[0], MinMaxValues[1]);
                int randomY = random.Next(MinMaxValues[2], MinMaxValues[3]);

                Thread.Sleep(ParseSpeedInput());
                        
                IInputInjector injector = new InputInjector();
                injector.InjectMouseClick(randomX, randomY);
            }
        }

        private void InjectInput_Touch(object sender, RoutedEventArgs e)
        {
            int[] MinMaxValues = ParseCoordinates();

            IntPtr hwnd = new WindowInteropHelper(this).Handle;

            // Get the number of times to generate random values
            if (!int.TryParse(NumberOfInputsTextBox.Text, out int inputCount) || inputCount <= 0)
            {
                MessageBox.Show("Please enter a valid positive integer for the number of inputs.");
                return;
            }

            for (int i = 0; i < inputCount; i++)
            {
                // Generate random values within the bounds
                Random random = new Random();
                int randomX = random.Next(MinMaxValues[0], MinMaxValues[1]);
                int randomY = random.Next(MinMaxValues[2], MinMaxValues[3]);

                Thread.Sleep(ParseSpeedInput());

                IInputInjector injector = new InputInjector();
                injector.InjectTouchInput(hwnd, randomX, randomY, true);
            }
        }

        private void InitializeClock()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            // Event handler for the timer
            timer.Tick += Timer_Tick; 
            timer.Start();
        }

        private bool IsTextNumeric(string text)
        {
            Regex regex = new Regex("[^0-9]+");
            return !regex.IsMatch(text);
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

        private void NumberOfInputsTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextNumeric(e.Text);
        }

        private int[] ParseCoordinates()
        {
            int[] MinMaxValues = new int[4];
            if (string.IsNullOrWhiteSpace(SpeedOfInput.Text) || string.IsNullOrEmpty(CoordinatesTextBox.Text))
            {
                MessageBox.Show("The input speed and input count is required. Please enter a value.", "Input Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return MinMaxValues;
            }
            // Parse the coordinates from the CoordinatesTextBox
            string[] lines = CoordinatesTextBox.Text.Split('\n');

            int inputSpeed = int.Parse(SpeedOfInput.Text);

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
                    MinMaxValues[0] = Math.Min(x1, x2);
                    MinMaxValues[1] = Math.Max(x1, x2);
                    MinMaxValues[2] = Math.Min(y1, y2);
                    MinMaxValues[3] = Math.Max(y1, y2);

                    return MinMaxValues;
                }
                catch (FormatException)
                {
                    MessageBox.Show("Error: Invalid format in coordinates.");
                    return MinMaxValues;
                }
            }
            else
            {
                MessageBox.Show("Error: Coordinates are not properly formatted.");
                return MinMaxValues;
            }
        }

        private int ParseSpeedInput()
        {
            return int.Parse(SpeedOfInput.Text);
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

                SelectionCanvas.Opacity = 1;
                isTransparent = false;
            }
        }

        private void SpeedOfInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextNumeric(e.Text);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Update the TextBlock with the current time
            // 24-hour format | "hh:mm:ss tt" for 12-hour format with AM/PM
            ClockTextBlock.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        private void TitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (isSelectingArea)
            {
                // Start point of the selection
                startPoint = e.GetPosition(SelectionCanvas);

                // Reset rectangle properties
                Canvas.SetLeft(selectionRectangleRect, startPoint.X);
                Canvas.SetTop(selectionRectangleRect, startPoint.Y);
                selectionRectangleRect.Width = 0;
                selectionRectangleRect.Height = 0;

                // Show the rectangle
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
                Point endPoint = e.GetPosition(SelectionCanvas);

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
            if (isSelectingArea && e.LeftButton == MouseButtonState.Pressed)
            {
                Point currentPoint = e.GetPosition(SelectionCanvas);

                // Calculate the new rectangle coordinates
                double x = Math.Min(currentPoint.X, startPoint.X);
                double y = Math.Min(currentPoint.Y, startPoint.Y);
                double width = Math.Abs(currentPoint.X - startPoint.X);
                double height = Math.Abs(currentPoint.Y - startPoint.Y);

                // Update the rectangle's position and size
                Canvas.SetLeft(selectionRectangleRect, x);
                Canvas.SetTop(selectionRectangleRect, y);
                selectionRectangleRect.Width = width;
                selectionRectangleRect.Height = height;

                // Update the selection rectangle geometry
                selectionRectangle.Rect = new Rect(x, y, width, height);
            }
        }

    }
}
