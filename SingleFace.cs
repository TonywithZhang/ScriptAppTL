using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace ScriptAppTL
{
    public class SingleFace
    {
        private Vector3D _normal;
        private Point3D _point1;
        private Point3D _point2;
        private Point3D _point3;

        public Vector3D Normal
        {
            get
            {
                return _normal;
            }
            set
            {
                _normal = value;
            }
        }

        public Point3D Point1
        {
            get
            {
                return _point1;
            }
            set
            {
                _point1 = value;
            }
        }

        public Point3D Point2
        {
            get
            {
                return _point2;
            }
            set
            {
                _point2 = value;
            }
        }

        public Point3D Point3
        {
            get
            {
                return _point3;
            }
            set
            {
                _point3 = value;
            }
        }

        public SingleFace()
        {

        }

        public SingleFace(Vector3D vector3)
        {
            _normal = vector3;
        }

        public SingleFace(Vector3D vector3,Point3D point1,Point3D point2,Point3D point3)
        {
            _normal = vector3;
            _point3 = point3;
            _point1 = point1;
            _point2 = point2;
        }

        public SingleFace(float vectorX,float vectorY,float vectorZ,float point1X,float point1Y,float point1Z,float point2X,float point2Y,float point2Z,float point3X,float point3Y,float point3Z)
        {
            _normal = new Vector3D(vectorX, vectorY, vectorZ);
            _point1 = new Point3D(point1X, point1Y, point1Z);
            _point2 = new Point3D(point2X, point2Y, point2Z);
            _point3 = new Point3D(point3X, point3Y, point3Z);
        }
    }
}
