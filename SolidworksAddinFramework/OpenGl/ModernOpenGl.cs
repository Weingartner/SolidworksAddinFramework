using System;
using System.Drawing;
using System.Reactive.Disposables;
using OpenTK.Graphics.OpenGL;

namespace SolidworksAddinFramework.OpenGl
{
    public static class ModernOpenGl
    {
        private static float[] GetMaterial(MaterialFace materialFace, MaterialParameter materialParameter)
        {
            var @params = new float[4];
            GL.GetMaterial(materialFace, materialParameter, @params);
            return @params;
        }

        public static IDisposable SetColor(Color value, ShadingModel shadingModel)
        {

            GL.ShadeModel(shadingModel);
            if (shadingModel == ShadingModel.Smooth)
            {
                GL.Enable(EnableCap.ColorMaterial);
                GL.ColorMaterial(MaterialFace.FrontAndBack, ColorMaterialParameter.AmbientAndDiffuse);

                GL.Enable(EnableCap.ColorMaterial);
                GL.ColorMaterial(MaterialFace.FrontAndBack, ColorMaterialParameter.AmbientAndDiffuse);
                /*
                GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, SpecularColor);
                GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Emission, EmissionColor);
                GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Shininess, Shinyness);
                */
                GL.Enable(EnableCap.Lighting);

            }
            else
            {
                GL.Disable(EnableCap.ColorMaterial);
                GL.Disable(EnableCap.Lighting);
            }

            if (value.A<255)
            {
                GL.DepthMask(false);
                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);


            }

            var color = new[] { value.R / 255f, value.G / 255f, value.B / 255f, value.A / 255f };
            GL.Color4(color);

            return Disposable.Create(() =>
            {
                if(value.A<255)
                {
                    GL.Disable(EnableCap.Blend);
                    GL.DepthMask(true);
                }
            });
        }

        public static IDisposable SetLineWidth(float value)
        {
            var currentLineWidth = GL.GetFloat(GetPName.LineWidth);
            GL.LineWidth(value);
            return Disposable.Create(() => GL.LineWidth(currentLineWidth));
        }

        public static IDisposable Begin(PrimitiveType mode)
        {
            GL.Begin(mode);
            return Disposable.Create(GL.End);
        }
    }
}