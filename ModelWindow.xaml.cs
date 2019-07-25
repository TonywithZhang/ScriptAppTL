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
                this.Show();
                showData();
            }
        }

        private RenderWindow3d window;
        private TopoShapeConvert convertor;
        private View3d theView;
        private AnyCAD.Platform.Application app = new AnyCAD.Platform.Application();

        public ModelWindow()
        {
            InitializeComponent();
            window = new RenderWindow3d();
            convertor = new TopoShapeConvert();
        }

        private void showData()
        {
            int id = 1;
            foreach(var face in _faces)
            {

            }
        }
        public override void BeginInit()
        {
            base.BeginInit();
            app.Initialize();
            Size size = this.RenderSize;
            theView = app.CreateView(new WindowInteropHelper(this).Handle.ToInt32(),Convert.ToInt32(size.Width) ,Convert.ToInt32(size.Height));
            theView.RequestDraw(10);
            
        }
    }
}
