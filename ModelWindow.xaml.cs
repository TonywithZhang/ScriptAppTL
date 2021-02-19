using AnyCAD.Platform;
using AnyCAD.Presentation;
using System;
using System.Collections.Generic;
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
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Windows.Interop;

namespace ScriptAppTL
{
    /// <summary>
    /// ModelWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ModelWindow : Window
    {
        private List<SingleFace> _faces;

        public List<SingleFace> Faces
        {
            set
            {
                _faces = value;
            }
        }


        public ModelWindow()
        {
            InitializeComponent();
            MeshGeometry3D  model= new MeshGeometry3D();
            Point3DCollection points = new Point3DCollection();
            
        }


    }
}
