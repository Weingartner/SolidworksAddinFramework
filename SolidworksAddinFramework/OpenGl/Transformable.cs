using System.DoubleNumerics;

namespace SolidworksAddinFramework.OpenGl
{
    /// <summary>
    /// A simple class for accumulating transforms
    /// </summary>
    public class Transformable
    {
        private Matrix4x4 _AdditionalTransform = Matrix4x4.Identity;
        private Matrix4x4 _BaseTransform = Matrix4x4.Identity;

        /// <summary>
        /// Apply the transform to the base transform and optionally replace 
        /// the base transform.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="accumulate"></param>
        public void ApplyTransform(Matrix4x4 transform, bool accumulate = false)
        {

            if (accumulate == false)
            {
                _AdditionalTransform = transform;
            }
            else
            {
                _BaseTransform = _BaseTransform*transform;
                _AdditionalTransform = Matrix4x4.Identity;
            }
        }

        public Matrix4x4 Transform => this._BaseTransform*_AdditionalTransform;
    }
}