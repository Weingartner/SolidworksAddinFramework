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

        public static IDisposable SetColor(Color value)
        {
            const MaterialFace materialFace = MaterialFace.FrontAndBack;

            // store current settings
            var currentAmbient = GetMaterial(materialFace, MaterialParameter.Ambient);
            var currentDiffuse = GetMaterial(materialFace, MaterialParameter.Diffuse);
            var currentSpecular = GetMaterial(materialFace, MaterialParameter.Specular);
            var currentEmmission = GetMaterial(materialFace, MaterialParameter.Emission);
            var currentShininess = GetMaterial(materialFace, MaterialParameter.Shininess);

            // set default values (see https://www.opengl.org/sdk/docs/man2/xhtml/glMaterial.xml)
            GL.Material(materialFace, MaterialParameter.Ambient, new[] { 0.2f, 0.2f, 0.2f, 1f });
            GL.Material(materialFace, MaterialParameter.Diffuse, new[] { 0.8f, 0.8f, 0.8f, 1f });
            GL.Material(materialFace, MaterialParameter.Specular, new[] { 0f, 0.0f, 0.0f, 1f });
            GL.Material(materialFace, MaterialParameter.Emission, new[] { 0f, 0f, 0f, 1f });
            GL.Material(materialFace, MaterialParameter.Shininess, new[] { 0f });

            var color = new[] { value.R / 255f, value.G / 255f, value.B / 255f, value.A / 255f };

            //TODO: the standard process to set transparency is to use SrcAlpha and OneMinusSrcAlpha,
            //but this doesn't work- no idea why??
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.DstColor, BlendingFactorDest.Zero);
            //GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);

            GL.Material(materialFace, MaterialParameter.AmbientAndDiffuse, color);
            GL.Material(materialFace, MaterialParameter.Shininess, new [] { 100f });
            GL.Material(materialFace, MaterialParameter.Specular, new[] { 1f, 1f, 1f, 1f });
            GL.Material(materialFace, MaterialParameter.Diffuse, new[] { 1f, 0f, 0, 1f });

            return Disposable.Create(() =>
            {
                GL.Material(materialFace, MaterialParameter.Ambient, currentAmbient);
                GL.Material(materialFace, MaterialParameter.Diffuse, currentDiffuse);
                GL.Material(materialFace, MaterialParameter.Specular, currentSpecular);
                GL.Material(materialFace, MaterialParameter.Emission, currentEmmission);
                GL.Material(materialFace, MaterialParameter.Shininess, currentShininess);
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