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
using System.Threading;
using AnyCAD.Platform;
using AnyCAD.Presentation;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace ScriptAppTL
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private string fileName;
        private string stpFileName;
        private bool isSTPSelected, isIGSSelected;
        private int secondsPassed = 0;
        List<SingleFace> faceData = new List<SingleFace>();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {

        }

        private void OnOpen(object sender, ExecutedRoutedEventArgs e)
        {
            var dlg = new OpenFileDialog()
            {
                Title = "Simple Editor - Open File",
                CheckFileExists = true,
                CheckPathExists = true,
                Filter = "Text files (*.txt)|*.txt|All files|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (dlg.ShowDialog() == true)
            {
                var lines = File.ReadLines(dlg.FileName);
                int i = 1;
                foreach(var line in lines)
                {
                    Console.WriteLine($"{i++},{line}");
                }
            }

        }

        private void OnSave(object sender, ExecutedRoutedEventArgs e)
        {
            var dlg = new SaveFileDialog()
            {
                Title = "Simple Editor - Save as",
                DefaultExt = "txt",
                Filter = "Text files (*.txt)|*.txt|All files|*.*"
            };
            if(dlg.ShowDialog() == true)
            {
                File.WriteAllText(dlg.FileName, property.Text);
                MessageBox.Show("保存成功！");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dialog = new OpenFileDialog()
                {
                    Title = "请选择一个stl文件",
                    CheckFileExists = true,
                    CheckPathExists = true,
                    Filter = "STL (*.stl)|*.stl",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                };

                if (dialog.ShowDialog() == true)
                {
                    convert.IsEnabled = false;
                    convert.Background = Brushes.Gray;
                    convert.Content = "正在转换";
                    fileName = dialog.FileName;
                    stpFileName = STPName.Text;
                    isSTPSelected = convertToSTP.IsChecked.GetValueOrDefault();
                    isIGSSelected = convertToIGES.IsChecked.GetValueOrDefault();
                    //string allText = File.ReadAllText(fileName);
                    FileStream fs = new FileStream(fileName, FileMode.Open);
                    BinaryReader br = new BinaryReader(fs);
                    br.ReadBytes(80);
                    Int32 numberOfFace = br.ReadInt32();
                    var info = new FileInfo(fileName);
                    var lengthOfFile = 50 * numberOfFace + 84;
                    
                    if (info.Length != lengthOfFile)
                    {
                        br.Close();
                        fs.Close();
                        readACSIIFile(fileName);
                    }
                    else
                    {
                        property.Text = $"stl文件中的片体数量为：{numberOfFace.ToString()}";
                        Console.WriteLine("正在读取二进制文件");
                        readBinaryFile(br, numberOfFace);
                        fs.Close();
                    }

                    Task.Run(() =>
                    {
                        TopoShapeGroup group = new TopoShapeGroup();
                        foreach (var face in faceData)
                        {
                            var line1 = GlobalInstance.BrepTools.MakeLine(new Vector3(face.Point1.X, face.Point1.Y, face.Point1.Z), new Vector3(face.Point2.X, face.Point2.Y, face.Point2.Z));
                            var line2 = GlobalInstance.BrepTools.MakeLine(new Vector3(face.Point1.X, face.Point1.Y, face.Point1.Z), new Vector3(face.Point3.X, face.Point3.Y, face.Point3.Z));
                            var line3 = GlobalInstance.BrepTools.MakeLine(new Vector3(face.Point2.X, face.Point2.Y, face.Point2.Z), new Vector3(face.Point3.X, face.Point3.Y, face.Point3.Z));
                            TopoShapeGroup wire = new TopoShapeGroup();
                            wire.Add(line1);
                            wire.Add(line2);
                            wire.Add(line3);
                            TopoShape wireLine = GlobalInstance.BrepTools.MakeWire(wire);
                            var boundedFace = GlobalInstance.BrepTools.MakeFace(wireLine);

                            group.Add(boundedFace);
                        }
                        var shell = GlobalInstance.BrepTools.MakeShell(group);
                        if(isSTPSelected)
                        {
                            GlobalInstance.BrepTools.SaveFile(shell, new AnyCAD.Platform.Path($"D:\\{stpFileName}.stp"));
                        }else if(isIGSSelected)
                        {
                            GlobalInstance.BrepTools.SaveFile(shell, new AnyCAD.Platform.Path($"D:\\{stpFileName}.igs"));
                        }
                        faceData.Clear();
                        this.Dispatcher.BeginInvoke(new Action(() => {
                            convert.Content = "转换完毕";
                            convert.Background = Brushes.Green;
                            convert.IsEnabled = true;
                        }));

                        var timer = new DispatcherTimer(DispatcherPriority.Normal);
                        timer.Interval = new TimeSpan(0, 0, 0, 1);
                        timer.Tick += new EventHandler(restoreText);
                        timer.Start();
                    });


                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            

        }

        void readACSIIFile(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            Vector3D normal = new Vector3D();
            Point3D point1 = new Point3D();
            Point3D point2 = new Point3D();
            Point3D point3 = new Point3D();
            int pointIndex = 1;
            foreach (var line in lines)
            {
                line.Trim();
                if (line.StartsWith("facet"))
                {
                    var words = line.Split(' ');
                    int length = words.Length;
                    normal.X = Convert.ToSingle(words[length - 3]);
                    normal.Y = Convert.ToSingle(words[length - 2]);
                    normal.Z = Convert.ToSingle(words[length - 1]);
                }else if (line.StartsWith("vertex"))
                {
                    switch (pointIndex)
                    {
                        case 1:
                            point1 = createPoint(line);
                            pointIndex++;
                            break;

                        case 2:
                            point2 = createPoint(line);
                            pointIndex++;
                            break;

                        case 3:
                            point3 = createPoint(line);
                            pointIndex = 1;
                            faceData.Add(new SingleFace(normal, point1, point2, point3));
                            break;

                        default:
                            break;
                    }
                }

            }
        }

        private Point3D createPoint(string line)
        {
            Point3D point = new Point3D();
            var words = line.Split(' ');
            point.X = Convert.ToSingle(words[1]);
            point.Y = Convert.ToSingle(words[2]);
            point.Z = Convert.ToSingle(words[3]);
            return point;
        }

        private void readBinaryFile(BinaryReader br,int numberOfFace)
        {
            
            for (int i = 0; i < numberOfFace; i++)
            {
                faceData.Add(new SingleFace(br.ReadSingle(),
                    br.ReadSingle(),
                    br.ReadSingle(),
                    br.ReadSingle(),
                    br.ReadSingle(),
                    br.ReadSingle(),
                    br.ReadSingle(),
                    br.ReadSingle(),
                    br.ReadSingle(),
                    br.ReadSingle(),
                    br.ReadSingle(),
                    br.ReadSingle()));
                br.ReadInt16();
            }
            br.Close();
        }

        private void restoreText(object sender,EventArgs e)
        {
            if(secondsPassed == 3)
            {
                Dispatcher.BeginInvoke(new Action(() => {
                    convert.Content = "选择stl文件并开始转换";
                }));            }
            else
            {
                secondsPassed++;
            }
        }
    }
}