using System.Drawing;
using System.Linq;
using System.Reactive.Disposables;
using SolidWorks.Interop.sldworks;
using Weingartner.WeinCad.Interfaces;
using XUnit.Solidworks.Addin;

namespace SolidworksAddinFramework.Spec.opengl
{
    public class MeshRenderSpec : SolidWorksSpec
    {
        [SolidworksFact]
        public void RenderFaceShouldWork()
        {
            CreatePartDoc(modelDoc =>
            {
                var modeller = SwAddinBase.Active.Modeler;
                var body = modeller.CreateBox(1, 1, 1);

                //var face = (ISurface)Modeler.CreateToroidalSurface(new[] {0, 0, 0.0}, new[] {0, 1, 0.0}, new[] {0, 0, 1.0}, 1, 0.5);
                var face = body.GetFaces().CastArray<IFace2>().First();
                //var clonedFace = (IFace2) face.
                //var clonedSurface = (ISurface) clonedFace.Copy();
                var faceBody = (IBody2)face.GetBody();
                var faceMesh = faceBody.CreateMesh(Color.Green, isSolid:false);
                var d1 = faceMesh.DisplayUndoable(modelDoc);

                //return new CompositeDisposable(d, d1);
                return new CompositeDisposable(d1);
            });
        }
    }
}
