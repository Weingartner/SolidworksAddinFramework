using System.Drawing;
using SolidWorks.Interop.sldworks;
using Weingartner.WeinCad.Interfaces;

namespace SolidworksAddinFramework
{
    public class BodyMaterials
    {
        private readonly IBody2 _Body;
        public double[] Materials
        {
            get
            {
                var castArray = _Body.MaterialPropertyValues2.CastArray<double>();
                if(castArray == null || castArray.Length == 0)
                    return InitMat();
                else
                {
                    return castArray;
                }
            }
            set
            {
                var materialPropertyValues2 = value ?? InitMat();
                _Body.MaterialPropertyValues2 = materialPropertyValues2;
                //Debug.Assert(_Body.MaterialPropertyValues2!=null);
            }
        }

        public BodyMaterials(IBody2 body)
        {
            _Body = body;
            if(body.MaterialPropertyValues2 == null || body.MaterialPropertyValues2.CastArray<double>().Length == 0)
                body.MaterialPropertyValues2 = InitMat();
            Color = Color.Black;
        }
        public BodyMaterials()
        {
            Color = Color.Black;
        }

        private static double[] InitMat(double[] mat= null)
        {
            mat = mat ?? new double[9];
            mat[0] = 0;
            mat[1] = 0;
            mat[2] = 0;
            mat[3] = 1;
            mat[4] = 1;
            mat[5] = 1;
            mat[6] = 1;
            mat[7] = 0;
            mat[8] = 0;
            return mat;
        }

        public Color Color
        {
            get
            {
                return Color.FromArgb((int)Materials[7]*255,(int)Materials[0]*255,(int)Materials[1]*255,(int)Materials[3]*255);
            }
            set
            {
                var mat = Materials;
                var c = value;
                mat[0] = ((double) c.R)/255;
                mat[1] = ((double) c.G)/255;
                mat[2] = ((double) c.B)/255;
                Materials = mat;


            }
        }
    }
}