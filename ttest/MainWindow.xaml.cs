using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ttest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PointManager pointManager;

        public MainWindow()
        {
            InitializeComponent();
            InitializeTextBoxes();
            pointManager = new PointManager();
            LoadPointsFromExistingFile("points.txt");
            Closing += MainWindow_Closing;
        }

        private void LoadPointsFromFile(string filePath)
        {
            //pointManager.LoadPointsFromFile(filePath);
            UpdatePointsList();
        }


        private void LoadPointsFromExistingFile(string filePath)
        {
            try
            {
                pointManager.LoadPointsFromFile(filePath);
                UpdatePointsList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке файла: {ex.Message}", "Ошибка");
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)//закрытие сохранение
        {
            try
            {
                string filePath = "points.txt";

                List<string> lines = new List<string>();
                foreach (Point point in pointManager.GetPoints())
                {
                    lines.Add($"({point.X}, {point.Y})");
                }

                if (int.TryParse(xTextBox.Text, out int x) && int.TryParse(yTextBox.Text, out int y))
                {
                    if (IsValidPoint(x, y))
                    {
                        pointManager.AddPoint(new Point(x, y));
                    }
                    else
                    {
                        MessageBox.Show("Координаты должны быть в диапазоне от -10 до 10", "Ошибка");
                    }
                }
                else
                {
                    MessageBox.Show("Некорректный ввод координат", "Ошибка");
                }


                File.WriteAllLines(filePath, lines);

                MessageBox.Show("Точки успешно сохранены", "Успех");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении файла: {ex.Message}", "Ошибка");
            }
        }


        private void AddPointsFromFile(string filePath)
        {
            try
            {
                string[] lines = File.ReadAllLines(filePath);

                foreach (string line in lines)
                {
                    string[] parts = line.Trim('(', ')').Split(',');
                    if (parts.Length == 2 && int.TryParse(parts[0].Trim(), out int x) && int.TryParse(parts[1].Trim(), out int y))
                    {
                        pointManager.AddPoint(new Point(x, y));
                    }
                }

                UpdatePointsList();
                UpdatePointsCanvas();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке файла: {ex.Message}", "Ошибка");
            }
        }
        private void SavePointsToFile(string filePath)
        {
            try
            {
                List<string> lines = new List<string>();
                foreach (Point point in pointManager.GetPoints())
                {
                    lines.Add($"({point.X}, {point.Y})");
                }

                File.WriteAllLines(filePath, lines);

                MessageBox.Show("Точки успешно сохранены", "Успех");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении файлай: {ex.Message}", "Ошибка");
            }
        }

        private void InitializeTextBoxes()
        {
            xTextBox.Text = "Введите координату X";
            yTextBox.Text = "Введите координату Y";

            xTextBox.GotFocus += TextBox_GotFocus;
            yTextBox.GotFocus += TextBox_GotFocus;

            xTextBox.LostFocus += TextBox_LostFocus;
            yTextBox.LostFocus += TextBox_LostFocus;
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text.StartsWith("Введите"))
            {
                textBox.Text = "";
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                if (textBox == xTextBox)
                {
                    textBox.Text = "Введите координату X";
                }
                else if (textBox == yTextBox)
                {
                    textBox.Text = "Введите координату Y";
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) //рассчитать
        {
            if (int.TryParse(xTextBox.Text, out int x) && int.TryParse(yTextBox.Text, out int y))
            {
                if (IsValidPoint(x, y))
                {
                    pointManager.AddPoint(new Point(x, y));
                    UpdatePointsList();
                }
                else
                {
                    MessageBox.Show("Координаты должны быть в диапазоне от -10 до 10", "Ошибка");
                }
            }
            else
            {
                MessageBox.Show("Некорректный ввод координат", "Ошибка");
            }
        }

        private bool IsValidPoint(int x, int y)
        {
            return x >= -10 && x <= 10 && y >= -10 && y <= 10;
        }

        private void UpdatePointsList()
        {
            pointsListBox.Items.Clear();
            foreach (Point point in pointManager.GetPoints())
            {
                pointsListBox.Items.Add($"({point.X}, {point.Y})");
            }
        }

        private void Button_Save(object sender, RoutedEventArgs e) //сохр.новый
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

            bool? result = saveFileDialog.ShowDialog();
            if (result == true)
            {
                try
                {
                    string filePath = saveFileDialog.FileName;
                    List<string> lines = new List<string>();
                    foreach (Point point in pointManager.GetPoints())
                    {
                        lines.Add($"({point.X}, {point.Y})");
                    }

                    File.WriteAllLines(filePath, lines);

                    MessageBox.Show("Файл успешно сохранен", "Успех");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении файлай: {ex.Message}", "Ошибка");
                }
            }
        }

        private void Button_Load_Click(object sender, RoutedEventArgs e) //Загрузка
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
                LoadPoints(openFileDialog.FileName);
            }
        }

        private void LoadPoints(string filePath)
        {
            {
                try
                {
                    pointManager.ClearPoints();

                    string[] lines = File.ReadAllLines(filePath);

                    foreach (string line in lines)
                    {
                        string[] parts = line.Trim('(', ')').Split(',');
                        if (parts.Length == 2 && int.TryParse(parts[0].Trim(), out int x) && int.TryParse(parts[1].Trim(), out int y))
                        {
                            pointManager.AddPoint(new Point(x, y));
                        }
                    }


                    UpdatePointsList();
                    UpdatePointsCanvas();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке файла: {ex.Message}", "Ошибка");
                }
            }
        }

        private void UpdatePointsCanvas()
        {

            pointCanvas.Children.Clear();


            foreach (Point point in pointManager.GetPoints())
            {
                Ellipse ellipse = new Ellipse();
                ellipse.Width = 6;
                ellipse.Height = 6;
                ellipse.Fill = Brushes.Red;
                Canvas.SetLeft(ellipse, point.X * 20 + 200);
                Canvas.SetTop(ellipse, -point.Y * 20 + 200);
                pointCanvas.Children.Add(ellipse);
            }
        }

        private void AddButton_click(object sender, RoutedEventArgs e) //отобразить
        {
            {
                UpdatePointsList();
                UpdatePointsCanvas();
            }
        }



        private void Button_Click_2(object sender, RoutedEventArgs e) //сохр.пакетно

        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

            bool? result = saveFileDialog.ShowDialog();
            if (result == true)
            {
                try
                {
                    string filePath = saveFileDialog.FileName;
                    List<string> lines = new List<string>();
                    foreach (Point point in pointManager.GetPoints())
                    {
                        lines.Add($"({point.X}, {point.Y})");
                    }

                    File.AppendAllLines(filePath, lines);

                    MessageBox.Show("Точки успешно добавлены", "Успех");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении точек: {ex.Message}", "Ошибка");
                }
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e) //Добавить новый
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true; //выбрарать несколько файлов
            openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
                foreach (string filename in openFileDialog.FileNames)
                {
                    AddPointsFromFile(filename);
                }
            }
        }

        private void pointCanvas_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }
    }
}



public class PointManager
{
    private List<Point> points;

    public PointManager()
    {
        points = new List<Point>();
    }

    public void AddPoint(Point point)
    {
        points.Add(point);
    }

    public List<Point> GetPoints()
    {
        return points;
    }

    public void ClearPoints()
    {
        points.Clear();
    }

    public void SavePointsToFile(string filePath)
    {
        try
        {
            List<string> lines = new List<string>();
            foreach (Point point in points)
            {
                lines.Add($"({point.X}, {point.Y})");
            }

            File.WriteAllLines(filePath, lines);
        }
        catch (Exception ex)
        {
            throw new Exception($"Ошибка при сохранении точек в файл: {ex.Message}");
        }
    }

    public void LoadPointsFromFile(string filePath)
    {
        try
        {
            points.Clear();
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                string[] parts = line.Trim('(', ')').Split(',');
                if (parts.Length == 2 && int.TryParse(parts[0], out int x) && int.TryParse(parts[1], out int y))
                {
                    points.Add(new Point(x, y));
                }
            }
        }
        catch (Exception ex)
        {
            // Обработка ошибки загрузки
            Console.WriteLine($"Ошибка при загрузке файла: {ex.Message}");
        }
    }
}