using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.DoubleNumerics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using LanguageExt;
using LanguageExt.Parsec;
using static LanguageExt.Prelude;
using SolidworksAddinFramework.Events;
using SolidworksAddinFramework.Geometry;
using SolidworksAddinFramework.OpenGl;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using static LanguageExt.Parsec.Prim;
using static LanguageExt.Parsec.Char;

namespace SolidworksAddinFramework
{
    public static class ModelDocExtensions
    {
        public static IBody2[] GetBodiesTs(this IModelDoc2 doc, swBodyType_e type = swBodyType_e.swSolidBody,
            bool visibleOnly = false)
        {
            var part = (IPartDoc) doc;
            var objects = (object[]) part.GetBodies2((int) type, visibleOnly);
            return objects?.Cast<IBody2>().ToArray() ?? new IBody2[0];
        }

        public static IDisposable CloseDisposable(this IModelDoc2 @this)
        {
            return Disposable.Create(@this.Close);
        }

        public static void Using(this IModelDoc2 doc, ISldWorks sldWorks, Action<IModelDoc2> run)
        {
            doc.Using(m => sldWorks.CloseDoc(doc.GetTitle()), run);
        }
        public static Task Using(this IModelDoc2 doc, ISldWorks sldWorks, Func<IModelDoc2, Task> run)
        {
            return doc.Using(m => sldWorks.CloseDoc(doc.GetTitle()), run);
        }


        /// <summary>
        /// Get all reference planes from the model
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static IEnumerable<IRefPlane> GetPlanes(this IModelDoc2 doc)
        {
            return doc.FeatureManager
                .GetFeatures(false)
                .CastArray<IFeature>()
                .Select(f => f.GetSpecificFeature2() as IRefPlane);
        }

        public static IObservable<IReadOnlyList<object>> SelectionObservable(this IModelDoc2 modelDoc, 
            Func<swSelectType_e, int, bool> filter = null)
        {
            var sm = modelDoc
                .SelectionManager
                .DirectCast<ISelectionMgr>();

            filter = filter ?? ((type,mark)=> true);
            return modelDoc
                .UserSelectionPostNotifyObservable()
                .Select(e => sm.GetSelectedObjects(filter));
        }

        public static void AddSelections(this IModelDoc2 doc, int mark, IReadOnlyCollection<object> objects)
        {
            var selectionMgr = (ISelectionMgr) doc.SelectionManager;
            var selectData = selectionMgr.CreateSelectData();
            selectData.Mark = mark;

            var before = selectionMgr.GetAllSelectedObjects().Count;
            var after = doc.Extension.MultiSelect2(ComWangling.ObjectArrayToDispatchWrapper(objects), true, selectData);
            //var after = selectionMgr.AddSelectionListObjects(ComWangling.ObjectArrayToDispatchWrapper(o.Objects), selectData);
            var selectionCount = after - before;
            Debug.Assert(selectionCount == objects.Count, $"Tried to select {objects.Count} objects, selected only {selectionCount}");
        }

        /// <summary>
        /// Add 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="selections"></param>
        public static void AddSelections(this IModelDoc2 doc, IEnumerable<SelectionData> selections)
        {
            var selectionMgr = (ISelectionMgr) doc.SelectionManager;
            var selectedObjects = selectionMgr.GetAllSelectedObjects();
            selections
                .GroupBy(p => p.Mark)
                .Select(p =>
                {
                    var objects = p
                        .SelectMany(selectionData => selectionData
                            .GetObjects(doc)
                            .Except(selectedObjects))
                        .ToArray();
                    return new {Mark = p.Key, Objects = objects};
                })
                .Where(p => p.Objects.Length > 0)
                .ForEach(o => doc.AddSelections(o.Mark, o.Objects));
        }

        public static void AddSelectionsFromModel(this IModelDoc2 doc, object model)
        {
            var selections = SelectionDataExtensions.GetSelectionsFromModel(model).ToList();
            doc.AddSelections(selections);
        }

        public static void AddSelection(this IModelDoc2 doc, SelectionData selection)
        {
            doc.AddSelections(new[] { selection });
        }

        public static void ClearSelections(this IModelDoc2 doc, IEnumerable<SelectionData> selections)
        {
            var selectionMgr = (ISelectionMgr)doc.SelectionManager;
            selections
                .SelectMany(s => s.GetObjects(doc))
                .ForEach(obj =>
                {
                    var selection = selectionMgr
                        .GetObjectSelections()
                        .SingleOrDefault(o => o.Object == obj);
                    if (selection == null) return;

                    var isDeselected = selectionMgr.DeSelect2(selection.Index, SelectionManagerExtensions.AnyMark) == 1;
                    Debug.Assert(isDeselected, "Couldn't deselect object");
                });
        }

        public static void ClearSelection(this IModelDoc2 doc, SelectionData selection)
        {
            doc.ClearSelections(new[] { selection });
        }

        public static IEnumerable<object> GetSelectedObjectsFromModel(this IModelDoc2 doc, object model)
        {
            return SelectionDataExtensions.GetSelectionsFromModel(model)
                .SelectMany(data => data.GetObjects(doc));
        }

        public static Tuple<object[], int[], IView[]> GetMacroFeatureDataSelectionInfo(this IModelDoc2 doc, object model)
        {
            var view = (IView) (doc as IDrawingDoc)?.GetFirstView();

            var selections = SelectionDataExtensions.GetSelectionsFromModel(model).ToList();
            var selectedObjects = selections.SelectMany(s => s.GetObjects(doc)).ToArray();
            var marks = selections.SelectMany(s => Enumerable.Repeat(s.Mark, s.ObjectIds.Count)).ToArray();
            var views = selections.SelectMany(s => Enumerable.Repeat(view, s.ObjectIds.Count)).ToArray();
            return Tuple(selectedObjects, marks, views);
        }

        /// <summary>
        /// Get an entity from its persist id.
        /// 
        /// Because we should use the same version of `GetPersistReference` and `GetObjectFromPersistReference` resp.
        /// and the persist reference is saved with the models we should never ever use another version of the two methods.
        /// For more details see http://help.solidworks.com/2016/english/api/sldworksapi/SolidWorks.interop.sldworks~SolidWorks.interop.sldworks.IModelDocExtension~GetPersistReference3.html#remarksSection
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="persistId"></param>
        /// <returns></returns>
        public static object GetObjectFromPersistReference(this IModelDoc2 doc, byte[] persistId)
        {
            int errorCode;
            var @object = doc.Extension.GetObjectByPersistReference3(persistId, out errorCode);
            var result = (swPersistReferencedObjectStates_e) errorCode;
            if (result != swPersistReferencedObjectStates_e.swPersistReferencedObject_Ok)
            {
                throw new SelectionException($"GetObjectByPersistReference3 returned {result}");
            }
            return @object;
        }

        /// <summary>
        /// Get a persist id from an entity.
        /// 
        /// Because we should use the same version of `GetPersistReference` and `GetObjectFromPersistReference` resp.
        /// and the persist reference is saved with the models we should never ever use another version of the two methods.
        /// For more details see http://help.solidworks.com/2016/english/api/sldworksapi/SolidWorks.interop.sldworks~SolidWorks.interop.sldworks.IModelDocExtension~GetPersistReference3.html#remarksSection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="modelDoc"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] GetPersistReference<T>(this IModelDoc2 modelDoc, T obj)
        {
            return modelDoc.Extension.GetPersistReference3(obj).CastArray<byte>();
        }

        /// <summary>
        /// Generates a lambda which resolves an entity from its persist reference every time you invoke it.
        /// </summary>
        /// <typeparam name="T">Entity type, e.g. IBody2</typeparam>
        /// <param name="modelDoc"></param>
        /// <param name="obj">Entity with a valid persist reference</param>
        /// <returns></returns>
        public static Func<T> GetPersistentEntityReference<T>(this IModelDoc2 modelDoc, T obj)
        {
            var persistReference = modelDoc.GetPersistReference(obj);
            return fun(() => (T)GetObjectFromPersistReference(modelDoc, persistReference));
        }

        /// <summary>
        /// Doesn't work when intersecting with wire bodies. 
        /// </summary>
        /// <param name="modelDoc"></param>
        /// <param name="ray"></param>
        /// <param name="bodies"></param>
        /// <param name="hitRadius"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static List<RayIntersection> GetRayIntersections(this IModelDoc2 modelDoc, PointDirection3 ray, IBody2[] bodies, double hitRadius, double offset)
        {
            var icount = modelDoc.RayIntersections
                (BodiesIn: bodies
                , BasePointsIn: ray.Point.ToDoubles()
                , VectorsIn: ray.Direction.ToDoubles()
                , Options: (int) (swRayPtsOpts_e.swRayPtsOptsENTRY_EXIT | swRayPtsOpts_e.swRayPtsOptsNORMALS |
                                  swRayPtsOpts_e.swRayPtsOptsTOPOLS | swRayPtsOpts_e.swRayPtsOptsUNBLOCK)
                , HitRadius: hitRadius
                , Offset: offset);
            var result = modelDoc.GetRayIntersectionsPoints().CastArray<double>();

            const int fields = 9;
            return Enumerable.Range(0, icount)
                .Select(i =>
                {
                    var baseOffset = i * fields;

                    var bodyIndex = result[baseOffset + 0];
                    var rayIndex = result[baseOffset + 1];
                    var intersectionType = result[baseOffset + 2];
                    var x = result[baseOffset + 3];
                    var y = result[baseOffset + 4];
                    var z = result[baseOffset + 5];
                    var nx = result[baseOffset + 6];
                    var ny = result[baseOffset + 7];
                    var nz = result[baseOffset + 8];

                    return new RayIntersection(
                        bodies[(int)bodyIndex],
                        (int)rayIndex,
                        (swRayPtsResults_e)intersectionType,
                        new [] { x, y, z }.ToVector3(),
                        new[] { nx, ny, nz }.ToVector3()
                        );
                }).ToList();
        }

        public class RayIntersection
        {
            public RayIntersection(IBody2 body, int rayIndex, swRayPtsResults_e intersectionType, Vector3 hitPoint, Vector3 normals)
            {
                Body = body;
                RayIndex = rayIndex;
                IntersectionType = intersectionType;
                HitPoint = hitPoint;
                Normals = normals;
            }

            public IBody2 Body { get; }
            public int RayIndex { get; }
            public swRayPtsResults_e IntersectionType { get; }
            public Vector3 HitPoint { get; }
            public Vector3 Normals { get; }

        }

        /// <summary>
        /// From a given X,Y screen coordinate return the model
        /// coordinates and the direction of looking.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static PointDirection3 ScreenToView(this IModelDoc2 doc, int x, int y)
        {
            var math = SwAddinBase.Active.Math;
            var view = doc.ActiveView.DirectCast<IModelView>();
            var t = view.Transform.Inverse().DirectCast<MathTransform>();

            var eye = math.Point(new[] {x, y, 0.0});

            var look = math.ZAxis().DirectCast<MathVector>();

            eye = eye.MultiplyTransformTs(t);
            look = look.MultiplyTransformTs(t);

            return new PointDirection3(Vector3Extensions.ToVector3(eye), look.ToVector3().Unit());
        }

        public static Vector2 ViewToScreen(this IModelDoc2 doc, Vector3 point)
        {
            var math = SwAddinBase.Active.Math;
            var view = doc.ActiveView.DirectCast<IModelView>();
            var t = view.Transform.DirectCast<MathTransform>();
            var mathPoint = point.ToSwMathPoint(math);
            mathPoint = mathPoint.MultiplyTransformTs(t);
            var v3 = mathPoint.ToVector3();
            return new Vector2(v3.X, v3.Y);
        }


        public enum EquationsDimensionType
        {
            Length,
            Angle
        }

        /// <summary>
        /// Set a global variable as you would find in the equation manager. The units
        /// of this setting will always be meters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="doc"></param>
        /// <param name="name"></param>
        /// <param name="meters"></param>
        /// <returns></returns>
        public static bool SetGlobalMeters(this IModelDoc2 doc, string name, double meters)
        {
            return SetGlobal(doc, new SwEq(name, meters, "m"));
        }
        public static bool SetGlobalRadians(this IModelDoc2 doc, string name, double radians)
        {
            return SetGlobal(doc, new SwEq(name, radians, "rad"));
        }
        public static bool SetGlobalDegrees(this IModelDoc2 doc, string name, double degrees)
        {
            return SetGlobal(doc, new SwEq(name, degrees, "deg"));
        }

        /// <summary>
        /// Get all the globals found in the equation manager into a dictionary.
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static Dictionary<int, SwEq> GetGlobals(this IModelDoc2 doc )
        {
            var swEqnMgr = doc.GetEquationMgr();
            Debug.Assert(swEqnMgr != null, "Couldn't get equation manager");
            var p = ModelDocExtensions.SwEwParser;
            return Enumerable.Range(0, swEqnMgr.GetCount())
                .Select
                (i =>
                {
                    var str = swEqnMgr.Equation[i];
                    var r = p.Parse(str);
                    if(r.IsFaulted)
                    {
                        Debug.WriteLine($"Can't parse variable {str}");
                        return None;

                    }
                    return Some(new {i, r.Reply.Result});
                })
                .WhereIsSome()
                .ToDictionary(v => v.i, v => v.Result);
        }

        /// <summary>
        /// Look-up a configuration based on it's ID
        /// </summary>
        /// <param name="modelDoc"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        public static Option<Configuration> GetConfigurationFromID(this IModelDoc2 modelDoc,int ID)
        {
            return modelDoc
                .GetConfigurations()
                .FirstOrDefault(config => config.GetID() == ID)
                .ToOption();

        }

        /// <summary>
        /// Get all configurations
        /// </summary>
        /// <param name="modelDoc"></param>
        /// <returns></returns>
        public static IEnumerable<Configuration> GetConfigurations(this IModelDoc2 modelDoc) =>
            modelDoc
            .GetConfigurationNames()
            .CastArray<string>()
            .Select(name => (Configuration)modelDoc.GetConfigurationByName(name));

        /// <summary>
        /// Set a number of globals at specific positions in the equation table
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="globals"></param>
        public static void SetGlobals(this IModelDoc2 doc, Dictionary<int, SwEq> globals)
        {
            foreach (var kv in globals)
            {
                SetGlobal(doc,kv.Key, kv.Value);
            }
        }

        /// <summary>
        /// Set a global variable at position i in the globals table
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="i"></param>
        /// <param name="eq"></param>
        private static void SetGlobal(this IModelDoc2 doc, int i, SwEq eq)
        {
            var swEqnMgr = doc.GetEquationMgr();
            swEqnMgr.Equation[i] = eq.ToString();
        }

        /// <summary>
        /// Set a global variable if the name exists allready. If it doesn't
        /// then nothing happens and the function returns false;
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="eq"></param>
        /// <returns></returns>
        public static bool SetGlobal(this IModelDoc2 doc, SwEq eq)
        {
            var existing = doc.GetGlobal(eq.Id);
            return existing.Match
                (kv =>
                {
                    SetGlobal(doc, kv.Key, eq);
                    return true;
                },()=>false);
        }

        public static Option<KeyValuePair<int, SwEq>> GetGlobal(this IModelDoc2 doc, string name)
        {
            return doc.GetGlobals().FirstOrDefault(g => g.Value.Id == name);
        }

        /// <summary>
        /// Try to set the equation with the form " $name = $value ". If the 
        /// name does not equal $name then the tryset will fail.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="eq"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static Option<string> TrySet<T>(string eq, string name, T value)
        {
            var vsplit = eq.Split('=').Select(sub=>sub.Trim()).ToList();
            Option<string> r = None;
            if (vsplit[0] == $@"""{name}""")
            {
                r = Some(eq.Replace(vsplit[1], value.ToString()));
            }
            return r;
        }

        static Parser<T> DeOpt<T>(Parser<Option<T>> p)
            => from i in p from r in i.Match(result,()=> failure<T>($"Could not parse {typeof(T).Name}")) select r;

        static Parser<T> DeOpt<T>(Option<T> p) => p.Match(result, () => failure<T>($"Could not parse {typeof(T).Name}"));


        public static ParserResult<double> TryParseDouble(string value)
        {

            var intLiteral = DeOpt(asInteger(many1(digit)));

            var floatLiteral = from i0 in intLiteral
                from p in ch('.')
                from i1 in intLiteral
                from r in DeOpt(parseDouble($"{i0}.{i1}"))
                select r;

            var expFloatLiteral = from fl in floatLiteral
                from e in choice(ch('e'), ch('E'))
                from d in intLiteral
                from r in DeOpt(parseDouble($"{fl}E{d}"))
                select r;

            var f = choice
                    ( attempt(expFloatLiteral) 
                    , attempt(floatLiteral)
                    , from i in intLiteral select (double) i
                    );

            return f.Parse(value);

        }

        /// <summary>
        /// Solidworks floating point parser for doubles as used in the equation manager.
        /// </summary>
        public static Parser<double> SwDoubleParser
        {
            get
            {
                var optSign = optionOrElse("", from x in oneOf("+-") select x.ToString());

                return from si in optSign
                    from nu in asString(many(digit))
                    from frac in optionOrElse("",
                        from pt in ch('.') 
                        from fr in asString(many(digit))
                        from ex in optionOrElse("",
                            from e in oneOf("eE")
                            from s in optSign
                            from n in asString(many1(digit))
                            select $"{e}{s}{n}"
                            )
                        select $"{pt}{fr}{ex}")
                    let all = $"{si}{nu}{frac}"
                    let opt = parseDouble(all)
                    from res in opt.Match(
                        result,
                        () => failure<double>("Invalid floating point value")
                        )
                    select res;
            }
        }

        public static Parser<SwEq> SwEwParser
        {
            get
            {
                var nameParser = from a in letter
                                 from b in asString(many(alphaNum))
                                 select a + b;

                var idParser = nameParser.doubleQuoted().label("Id Parser").skipWhite();
                var valueParser = SwDoubleParser.label("Value parser").skipWhite();
                var eq = ch('=').skipWhite();
                var unitsParser = asString(many1(letter)).label("Unit parser").skipWhite();

                return from id in idParser.skip(eq)
                       from val in valueParser
                       from units in unitsParser
                       select new SwEq(id, val, units);

            }
        }


        public static void WriteStorage(this IModelDoc2 doc, string selectionStorageId, string data)
        {
            using (var stream = doc.OpenStreamForWriting(selectionStorageId))
            {
                stream.Resource?.WriteAllText(data);
            }
        }

        public static void ReadStorage(this IModelDoc2 doc, string storageId, Action<string> action)
        {
            using (var stream = doc.OpenStreamForReading(storageId))
            {
                var storage = stream.Resource;
                var data = storage?.ReadAllText();
                action(data);
            }
        }

        public static ResourceDisposable<IStream> OpenStreamForWriting(this IModelDoc2 doc, string storageId)
        {
            return OpenStream(doc, storageId, isStoring: true);
        }

        public static ResourceDisposable<IStream> OpenStreamForReading(this IModelDoc2 doc, string storageId)
        {
            return OpenStream(doc, storageId, isStoring: false);
        }

        private static ResourceDisposable<IStream> OpenStream(IModelDoc2 doc, string storageId, bool isStoring)
        {
            var obj = doc.IGet3rdPartyStorage(storageId, IsStoring: isStoring);
            var closeStream = Disposable.Create(() => doc.IRelease3rdPartyStorage(storageId));
            return ResourceDisposable.Create((IStream) obj, closeStream);
        }
    }

    public static class ParserExt
    {
    }

    public enum UnitsEnum
    {
        Length,
        Angle
    };
}
