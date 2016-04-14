
// ReSharper disable All

using System;
using System.Reactive;
using System.Reactive.Linq;
using SolidWorks.Interop.swconst;
using SolidWorks.Interop.swpublished;
using SolidWorks.Interop.sldworks;
using System.Collections.Generic;
using SolidworksAddinFramework;
using System.Reactive.Disposables;


namespace SolidworksAddinFramework.Events {
    public static class DPartDocEvents_Event {

        public class RegenNotifyEventArgs
        {
            public RegenNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_RegenNotifyEventHandler.html
        public static IObservable<RegenNotifyEventArgs> RegenNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<RegenNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_RegenNotifyEventHandler callback = 
                        ()=>{
                            var ea = new RegenNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.RegenNotify += callback;
                    return Disposable.Create(()=> eventSource.RegenNotify-= callback);
                    
                }
            );
        }
        public class DestroyNotifyEventArgs
        {
            public DestroyNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_DestroyNotifyEventHandler.html
        public static IObservable<DestroyNotifyEventArgs> DestroyNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<DestroyNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_DestroyNotifyEventHandler callback = 
                        ()=>{
                            var ea = new DestroyNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.DestroyNotify += callback;
                    return Disposable.Create(()=> eventSource.DestroyNotify-= callback);
                    
                }
            );
        }
        public class RegenPostNotifyEventArgs
        {
            public RegenPostNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_RegenPostNotifyEventHandler.html
        public static IObservable<RegenPostNotifyEventArgs> RegenPostNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<RegenPostNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_RegenPostNotifyEventHandler callback = 
                        ()=>{
                            var ea = new RegenPostNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.RegenPostNotify += callback;
                    return Disposable.Create(()=> eventSource.RegenPostNotify-= callback);
                    
                }
            );
        }
        public class ViewNewNotifyEventArgs
        {
            public ViewNewNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_ViewNewNotifyEventHandler.html
        public static IObservable<ViewNewNotifyEventArgs> ViewNewNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<ViewNewNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_ViewNewNotifyEventHandler callback = 
                        ()=>{
                            var ea = new ViewNewNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.ViewNewNotify += callback;
                    return Disposable.Create(()=> eventSource.ViewNewNotify-= callback);
                    
                }
            );
        }
        public class NewSelectionNotifyEventArgs
        {
            public NewSelectionNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_NewSelectionNotifyEventHandler.html
        public static IObservable<NewSelectionNotifyEventArgs> NewSelectionNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<NewSelectionNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_NewSelectionNotifyEventHandler callback = 
                        ()=>{
                            var ea = new NewSelectionNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.NewSelectionNotify += callback;
                    return Disposable.Create(()=> eventSource.NewSelectionNotify-= callback);
                    
                }
            );
        }
        public class FileSaveNotifyEventArgs
        {
            public FileSaveNotifyEventArgs (System.String FileName)
            {
                this.FileName = FileName;
            }
            public System.String FileName { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_FileSaveNotifyEventHandler.html
        public static IObservable<FileSaveNotifyEventArgs> FileSaveNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<FileSaveNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_FileSaveNotifyEventHandler callback = 
                        (System.String FileName)=>{
                            var ea = new FileSaveNotifyEventArgs(FileName);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.FileSaveNotify += callback;
                    return Disposable.Create(()=> eventSource.FileSaveNotify-= callback);
                    
                }
            );
        }
        public class FileSaveAsNotifyEventArgs
        {
            public FileSaveAsNotifyEventArgs (System.String FileName)
            {
                this.FileName = FileName;
            }
            public System.String FileName { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_FileSaveAsNotifyEventHandler.html
        public static IObservable<FileSaveAsNotifyEventArgs> FileSaveAsNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<FileSaveAsNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_FileSaveAsNotifyEventHandler callback = 
                        (System.String FileName)=>{
                            var ea = new FileSaveAsNotifyEventArgs(FileName);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.FileSaveAsNotify += callback;
                    return Disposable.Create(()=> eventSource.FileSaveAsNotify-= callback);
                    
                }
            );
        }
        public class LoadFromStorageNotifyEventArgs
        {
            public LoadFromStorageNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_LoadFromStorageNotifyEventHandler.html
        public static IObservable<LoadFromStorageNotifyEventArgs> LoadFromStorageNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<LoadFromStorageNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_LoadFromStorageNotifyEventHandler callback = 
                        ()=>{
                            var ea = new LoadFromStorageNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.LoadFromStorageNotify += callback;
                    return Disposable.Create(()=> eventSource.LoadFromStorageNotify-= callback);
                    
                }
            );
        }
        public class SaveToStorageNotifyEventArgs
        {
            public SaveToStorageNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_SaveToStorageNotifyEventHandler.html
        public static IObservable<SaveToStorageNotifyEventArgs> SaveToStorageNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<SaveToStorageNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_SaveToStorageNotifyEventHandler callback = 
                        ()=>{
                            var ea = new SaveToStorageNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.SaveToStorageNotify += callback;
                    return Disposable.Create(()=> eventSource.SaveToStorageNotify-= callback);
                    
                }
            );
        }
        public class ActiveConfigChangeNotifyEventArgs
        {
            public ActiveConfigChangeNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_ActiveConfigChangeNotifyEventHandler.html
        public static IObservable<ActiveConfigChangeNotifyEventArgs> ActiveConfigChangeNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<ActiveConfigChangeNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_ActiveConfigChangeNotifyEventHandler callback = 
                        ()=>{
                            var ea = new ActiveConfigChangeNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.ActiveConfigChangeNotify += callback;
                    return Disposable.Create(()=> eventSource.ActiveConfigChangeNotify-= callback);
                    
                }
            );
        }
        public class ActiveConfigChangePostNotifyEventArgs
        {
            public ActiveConfigChangePostNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_ActiveConfigChangePostNotifyEventHandler.html
        public static IObservable<ActiveConfigChangePostNotifyEventArgs> ActiveConfigChangePostNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<ActiveConfigChangePostNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_ActiveConfigChangePostNotifyEventHandler callback = 
                        ()=>{
                            var ea = new ActiveConfigChangePostNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.ActiveConfigChangePostNotify += callback;
                    return Disposable.Create(()=> eventSource.ActiveConfigChangePostNotify-= callback);
                    
                }
            );
        }
        public class ViewNewNotify2EventArgs
        {
            public ViewNewNotify2EventArgs (System.Object viewBeingAdded)
            {
                this.viewBeingAdded = viewBeingAdded;
            }
            public System.Object viewBeingAdded { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_ViewNewNotify2EventHandler.html
        public static IObservable<ViewNewNotify2EventArgs> ViewNewNotify2Observable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<ViewNewNotify2EventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_ViewNewNotify2EventHandler callback = 
                        (System.Object viewBeingAdded)=>{
                            var ea = new ViewNewNotify2EventArgs(viewBeingAdded);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.ViewNewNotify2 += callback;
                    return Disposable.Create(()=> eventSource.ViewNewNotify2-= callback);
                    
                }
            );
        }
        public class LightingDialogCreateNotifyEventArgs
        {
            public LightingDialogCreateNotifyEventArgs (System.Object dialog)
            {
                this.dialog = dialog;
            }
            public System.Object dialog { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_LightingDialogCreateNotifyEventHandler.html
        public static IObservable<LightingDialogCreateNotifyEventArgs> LightingDialogCreateNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<LightingDialogCreateNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_LightingDialogCreateNotifyEventHandler callback = 
                        (System.Object dialog)=>{
                            var ea = new LightingDialogCreateNotifyEventArgs(dialog);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.LightingDialogCreateNotify += callback;
                    return Disposable.Create(()=> eventSource.LightingDialogCreateNotify-= callback);
                    
                }
            );
        }
        public class AddItemNotifyEventArgs
        {
            public AddItemNotifyEventArgs (System.Int32 EntityType, System.String itemName)
            {
                this.EntityType = EntityType;
                this.itemName = itemName;
            }
            public System.Int32 EntityType { get; }
            public System.String itemName { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_AddItemNotifyEventHandler.html
        public static IObservable<AddItemNotifyEventArgs> AddItemNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<AddItemNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_AddItemNotifyEventHandler callback = 
                        (System.Int32 EntityType, System.String itemName)=>{
                            var ea = new AddItemNotifyEventArgs(EntityType, itemName);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.AddItemNotify += callback;
                    return Disposable.Create(()=> eventSource.AddItemNotify-= callback);
                    
                }
            );
        }
        public class RenameItemNotifyEventArgs
        {
            public RenameItemNotifyEventArgs (System.Int32 EntityType, System.String oldName, System.String NewName)
            {
                this.EntityType = EntityType;
                this.oldName = oldName;
                this.NewName = NewName;
            }
            public System.Int32 EntityType { get; }
            public System.String oldName { get; }
            public System.String NewName { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_RenameItemNotifyEventHandler.html
        public static IObservable<RenameItemNotifyEventArgs> RenameItemNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<RenameItemNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_RenameItemNotifyEventHandler callback = 
                        (System.Int32 EntityType, System.String oldName, System.String NewName)=>{
                            var ea = new RenameItemNotifyEventArgs(EntityType, oldName, NewName);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.RenameItemNotify += callback;
                    return Disposable.Create(()=> eventSource.RenameItemNotify-= callback);
                    
                }
            );
        }
        public class DeleteItemNotifyEventArgs
        {
            public DeleteItemNotifyEventArgs (System.Int32 EntityType, System.String itemName)
            {
                this.EntityType = EntityType;
                this.itemName = itemName;
            }
            public System.Int32 EntityType { get; }
            public System.String itemName { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_DeleteItemNotifyEventHandler.html
        public static IObservable<DeleteItemNotifyEventArgs> DeleteItemNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<DeleteItemNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_DeleteItemNotifyEventHandler callback = 
                        (System.Int32 EntityType, System.String itemName)=>{
                            var ea = new DeleteItemNotifyEventArgs(EntityType, itemName);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.DeleteItemNotify += callback;
                    return Disposable.Create(()=> eventSource.DeleteItemNotify-= callback);
                    
                }
            );
        }
        public class ModifyNotifyEventArgs
        {
            public ModifyNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_ModifyNotifyEventHandler.html
        public static IObservable<ModifyNotifyEventArgs> ModifyNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<ModifyNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_ModifyNotifyEventHandler callback = 
                        ()=>{
                            var ea = new ModifyNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.ModifyNotify += callback;
                    return Disposable.Create(()=> eventSource.ModifyNotify-= callback);
                    
                }
            );
        }
        public class FileReloadNotifyEventArgs
        {
            public FileReloadNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_FileReloadNotifyEventHandler.html
        public static IObservable<FileReloadNotifyEventArgs> FileReloadNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<FileReloadNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_FileReloadNotifyEventHandler callback = 
                        ()=>{
                            var ea = new FileReloadNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.FileReloadNotify += callback;
                    return Disposable.Create(()=> eventSource.FileReloadNotify-= callback);
                    
                }
            );
        }
        public class AddCustomPropertyNotifyEventArgs
        {
            public AddCustomPropertyNotifyEventArgs (System.String propName, System.String Configuration, System.String Value, System.Int32 valueType)
            {
                this.propName = propName;
                this.Configuration = Configuration;
                this.Value = Value;
                this.valueType = valueType;
            }
            public System.String propName { get; }
            public System.String Configuration { get; }
            public System.String Value { get; }
            public System.Int32 valueType { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_AddCustomPropertyNotifyEventHandler.html
        public static IObservable<AddCustomPropertyNotifyEventArgs> AddCustomPropertyNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<AddCustomPropertyNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_AddCustomPropertyNotifyEventHandler callback = 
                        (System.String propName, System.String Configuration, System.String Value, System.Int32 valueType)=>{
                            var ea = new AddCustomPropertyNotifyEventArgs(propName, Configuration, Value, valueType);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.AddCustomPropertyNotify += callback;
                    return Disposable.Create(()=> eventSource.AddCustomPropertyNotify-= callback);
                    
                }
            );
        }
        public class ChangeCustomPropertyNotifyEventArgs
        {
            public ChangeCustomPropertyNotifyEventArgs (System.String propName, System.String Configuration, System.String oldValue, System.String NewValue, System.Int32 valueType)
            {
                this.propName = propName;
                this.Configuration = Configuration;
                this.oldValue = oldValue;
                this.NewValue = NewValue;
                this.valueType = valueType;
            }
            public System.String propName { get; }
            public System.String Configuration { get; }
            public System.String oldValue { get; }
            public System.String NewValue { get; }
            public System.Int32 valueType { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_ChangeCustomPropertyNotifyEventHandler.html
        public static IObservable<ChangeCustomPropertyNotifyEventArgs> ChangeCustomPropertyNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<ChangeCustomPropertyNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_ChangeCustomPropertyNotifyEventHandler callback = 
                        (System.String propName, System.String Configuration, System.String oldValue, System.String NewValue, System.Int32 valueType)=>{
                            var ea = new ChangeCustomPropertyNotifyEventArgs(propName, Configuration, oldValue, NewValue, valueType);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.ChangeCustomPropertyNotify += callback;
                    return Disposable.Create(()=> eventSource.ChangeCustomPropertyNotify-= callback);
                    
                }
            );
        }
        public class DeleteCustomPropertyNotifyEventArgs
        {
            public DeleteCustomPropertyNotifyEventArgs (System.String propName, System.String Configuration, System.String Value, System.Int32 valueType)
            {
                this.propName = propName;
                this.Configuration = Configuration;
                this.Value = Value;
                this.valueType = valueType;
            }
            public System.String propName { get; }
            public System.String Configuration { get; }
            public System.String Value { get; }
            public System.Int32 valueType { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_DeleteCustomPropertyNotifyEventHandler.html
        public static IObservable<DeleteCustomPropertyNotifyEventArgs> DeleteCustomPropertyNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<DeleteCustomPropertyNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_DeleteCustomPropertyNotifyEventHandler callback = 
                        (System.String propName, System.String Configuration, System.String Value, System.Int32 valueType)=>{
                            var ea = new DeleteCustomPropertyNotifyEventArgs(propName, Configuration, Value, valueType);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.DeleteCustomPropertyNotify += callback;
                    return Disposable.Create(()=> eventSource.DeleteCustomPropertyNotify-= callback);
                    
                }
            );
        }
        public class FeatureEditPreNotifyEventArgs
        {
            public FeatureEditPreNotifyEventArgs (System.Object EditFeature)
            {
                this.EditFeature = EditFeature;
            }
            public System.Object EditFeature { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_FeatureEditPreNotifyEventHandler.html
        public static IObservable<FeatureEditPreNotifyEventArgs> FeatureEditPreNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<FeatureEditPreNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_FeatureEditPreNotifyEventHandler callback = 
                        (System.Object EditFeature)=>{
                            var ea = new FeatureEditPreNotifyEventArgs(EditFeature);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.FeatureEditPreNotify += callback;
                    return Disposable.Create(()=> eventSource.FeatureEditPreNotify-= callback);
                    
                }
            );
        }
        public class FeatureSketchEditPreNotifyEventArgs
        {
            public FeatureSketchEditPreNotifyEventArgs (System.Object EditFeature, System.Object featureSketch)
            {
                this.EditFeature = EditFeature;
                this.featureSketch = featureSketch;
            }
            public System.Object EditFeature { get; }
            public System.Object featureSketch { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_FeatureSketchEditPreNotifyEventHandler.html
        public static IObservable<FeatureSketchEditPreNotifyEventArgs> FeatureSketchEditPreNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<FeatureSketchEditPreNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_FeatureSketchEditPreNotifyEventHandler callback = 
                        (System.Object EditFeature, System.Object featureSketch)=>{
                            var ea = new FeatureSketchEditPreNotifyEventArgs(EditFeature, featureSketch);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.FeatureSketchEditPreNotify += callback;
                    return Disposable.Create(()=> eventSource.FeatureSketchEditPreNotify-= callback);
                    
                }
            );
        }
        public class FileSaveAsNotify2EventArgs
        {
            public FileSaveAsNotify2EventArgs (System.String FileName)
            {
                this.FileName = FileName;
            }
            public System.String FileName { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_FileSaveAsNotify2EventHandler.html
        public static IObservable<FileSaveAsNotify2EventArgs> FileSaveAsNotify2Observable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<FileSaveAsNotify2EventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_FileSaveAsNotify2EventHandler callback = 
                        (System.String FileName)=>{
                            var ea = new FileSaveAsNotify2EventArgs(FileName);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.FileSaveAsNotify2 += callback;
                    return Disposable.Create(()=> eventSource.FileSaveAsNotify2-= callback);
                    
                }
            );
        }
        public class DeleteSelectionPreNotifyEventArgs
        {
            public DeleteSelectionPreNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_DeleteSelectionPreNotifyEventHandler.html
        public static IObservable<DeleteSelectionPreNotifyEventArgs> DeleteSelectionPreNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<DeleteSelectionPreNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_DeleteSelectionPreNotifyEventHandler callback = 
                        ()=>{
                            var ea = new DeleteSelectionPreNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.DeleteSelectionPreNotify += callback;
                    return Disposable.Create(()=> eventSource.DeleteSelectionPreNotify-= callback);
                    
                }
            );
        }
        public class FileReloadPreNotifyEventArgs
        {
            public FileReloadPreNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_FileReloadPreNotifyEventHandler.html
        public static IObservable<FileReloadPreNotifyEventArgs> FileReloadPreNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<FileReloadPreNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_FileReloadPreNotifyEventHandler callback = 
                        ()=>{
                            var ea = new FileReloadPreNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.FileReloadPreNotify += callback;
                    return Disposable.Create(()=> eventSource.FileReloadPreNotify-= callback);
                    
                }
            );
        }
        public class BodyVisibleChangeNotifyEventArgs
        {
            public BodyVisibleChangeNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_BodyVisibleChangeNotifyEventHandler.html
        public static IObservable<BodyVisibleChangeNotifyEventArgs> BodyVisibleChangeNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<BodyVisibleChangeNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_BodyVisibleChangeNotifyEventHandler callback = 
                        ()=>{
                            var ea = new BodyVisibleChangeNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.BodyVisibleChangeNotify += callback;
                    return Disposable.Create(()=> eventSource.BodyVisibleChangeNotify-= callback);
                    
                }
            );
        }
        public class RegenPostNotify2EventArgs
        {
            public RegenPostNotify2EventArgs (System.Object stopFeature)
            {
                this.stopFeature = stopFeature;
            }
            public System.Object stopFeature { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_RegenPostNotify2EventHandler.html
        public static IObservable<RegenPostNotify2EventArgs> RegenPostNotify2Observable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<RegenPostNotify2EventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_RegenPostNotify2EventHandler callback = 
                        (System.Object stopFeature)=>{
                            var ea = new RegenPostNotify2EventArgs(stopFeature);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.RegenPostNotify2 += callback;
                    return Disposable.Create(()=> eventSource.RegenPostNotify2-= callback);
                    
                }
            );
        }
        public class FileSavePostNotifyEventArgs
        {
            public FileSavePostNotifyEventArgs (System.Int32 saveType, System.String FileName)
            {
                this.saveType = saveType;
                this.FileName = FileName;
            }
            public System.Int32 saveType { get; }
            public System.String FileName { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_FileSavePostNotifyEventHandler.html
        public static IObservable<FileSavePostNotifyEventArgs> FileSavePostNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<FileSavePostNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_FileSavePostNotifyEventHandler callback = 
                        (System.Int32 saveType, System.String FileName)=>{
                            var ea = new FileSavePostNotifyEventArgs(saveType, FileName);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.FileSavePostNotify += callback;
                    return Disposable.Create(()=> eventSource.FileSavePostNotify-= callback);
                    
                }
            );
        }
        public class LoadFromStorageStoreNotifyEventArgs
        {
            public LoadFromStorageStoreNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_LoadFromStorageStoreNotifyEventHandler.html
        public static IObservable<LoadFromStorageStoreNotifyEventArgs> LoadFromStorageStoreNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<LoadFromStorageStoreNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_LoadFromStorageStoreNotifyEventHandler callback = 
                        ()=>{
                            var ea = new LoadFromStorageStoreNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.LoadFromStorageStoreNotify += callback;
                    return Disposable.Create(()=> eventSource.LoadFromStorageStoreNotify-= callback);
                    
                }
            );
        }
        public class SaveToStorageStoreNotifyEventArgs
        {
            public SaveToStorageStoreNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_SaveToStorageStoreNotifyEventHandler.html
        public static IObservable<SaveToStorageStoreNotifyEventArgs> SaveToStorageStoreNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<SaveToStorageStoreNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_SaveToStorageStoreNotifyEventHandler callback = 
                        ()=>{
                            var ea = new SaveToStorageStoreNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.SaveToStorageStoreNotify += callback;
                    return Disposable.Create(()=> eventSource.SaveToStorageStoreNotify-= callback);
                    
                }
            );
        }
        public class FeatureManagerTreeRebuildNotifyEventArgs
        {
            public FeatureManagerTreeRebuildNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_FeatureManagerTreeRebuildNotifyEventHandler.html
        public static IObservable<FeatureManagerTreeRebuildNotifyEventArgs> FeatureManagerTreeRebuildNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<FeatureManagerTreeRebuildNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_FeatureManagerTreeRebuildNotifyEventHandler callback = 
                        ()=>{
                            var ea = new FeatureManagerTreeRebuildNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.FeatureManagerTreeRebuildNotify += callback;
                    return Disposable.Create(()=> eventSource.FeatureManagerTreeRebuildNotify-= callback);
                    
                }
            );
        }
        public class FileDropPostNotifyEventArgs
        {
            public FileDropPostNotifyEventArgs (System.String FileName)
            {
                this.FileName = FileName;
            }
            public System.String FileName { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_FileDropPostNotifyEventHandler.html
        public static IObservable<FileDropPostNotifyEventArgs> FileDropPostNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<FileDropPostNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_FileDropPostNotifyEventHandler callback = 
                        (System.String FileName)=>{
                            var ea = new FileDropPostNotifyEventArgs(FileName);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.FileDropPostNotify += callback;
                    return Disposable.Create(()=> eventSource.FileDropPostNotify-= callback);
                    
                }
            );
        }
        public class DynamicHighlightNotifyEventArgs
        {
            public DynamicHighlightNotifyEventArgs (System.Boolean bHighlightState)
            {
                this.bHighlightState = bHighlightState;
            }
            public System.Boolean bHighlightState { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_DynamicHighlightNotifyEventHandler.html
        public static IObservable<DynamicHighlightNotifyEventArgs> DynamicHighlightNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<DynamicHighlightNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_DynamicHighlightNotifyEventHandler callback = 
                        (System.Boolean bHighlightState)=>{
                            var ea = new DynamicHighlightNotifyEventArgs(bHighlightState);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.DynamicHighlightNotify += callback;
                    return Disposable.Create(()=> eventSource.DynamicHighlightNotify-= callback);
                    
                }
            );
        }
        public class DimensionChangeNotifyEventArgs
        {
            public DimensionChangeNotifyEventArgs (System.Object displayDim)
            {
                this.displayDim = displayDim;
            }
            public System.Object displayDim { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_DimensionChangeNotifyEventHandler.html
        public static IObservable<DimensionChangeNotifyEventArgs> DimensionChangeNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<DimensionChangeNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_DimensionChangeNotifyEventHandler callback = 
                        (System.Object displayDim)=>{
                            var ea = new DimensionChangeNotifyEventArgs(displayDim);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.DimensionChangeNotify += callback;
                    return Disposable.Create(()=> eventSource.DimensionChangeNotify-= callback);
                    
                }
            );
        }
        public class FileReloadCancelNotifyEventArgs
        {
            public FileReloadCancelNotifyEventArgs (System.Int32 ErrorCode)
            {
                this.ErrorCode = ErrorCode;
            }
            public System.Int32 ErrorCode { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_FileReloadCancelNotifyEventHandler.html
        public static IObservable<FileReloadCancelNotifyEventArgs> FileReloadCancelNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<FileReloadCancelNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_FileReloadCancelNotifyEventHandler callback = 
                        (System.Int32 ErrorCode)=>{
                            var ea = new FileReloadCancelNotifyEventArgs(ErrorCode);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.FileReloadCancelNotify += callback;
                    return Disposable.Create(()=> eventSource.FileReloadCancelNotify-= callback);
                    
                }
            );
        }
        public class FileSavePostCancelNotifyEventArgs
        {
            public FileSavePostCancelNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_FileSavePostCancelNotifyEventHandler.html
        public static IObservable<FileSavePostCancelNotifyEventArgs> FileSavePostCancelNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<FileSavePostCancelNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_FileSavePostCancelNotifyEventHandler callback = 
                        ()=>{
                            var ea = new FileSavePostCancelNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.FileSavePostCancelNotify += callback;
                    return Disposable.Create(()=> eventSource.FileSavePostCancelNotify-= callback);
                    
                }
            );
        }
        public class SketchSolveNotifyEventArgs
        {
            public SketchSolveNotifyEventArgs (System.String featName)
            {
                this.featName = featName;
            }
            public System.String featName { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_SketchSolveNotifyEventHandler.html
        public static IObservable<SketchSolveNotifyEventArgs> SketchSolveNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<SketchSolveNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_SketchSolveNotifyEventHandler callback = 
                        (System.String featName)=>{
                            var ea = new SketchSolveNotifyEventArgs(featName);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.SketchSolveNotify += callback;
                    return Disposable.Create(()=> eventSource.SketchSolveNotify-= callback);
                    
                }
            );
        }
        public class DeleteItemPreNotifyEventArgs
        {
            public DeleteItemPreNotifyEventArgs (System.Int32 EntityType, System.String itemName)
            {
                this.EntityType = EntityType;
                this.itemName = itemName;
            }
            public System.Int32 EntityType { get; }
            public System.String itemName { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_DeleteItemPreNotifyEventHandler.html
        public static IObservable<DeleteItemPreNotifyEventArgs> DeleteItemPreNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<DeleteItemPreNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_DeleteItemPreNotifyEventHandler callback = 
                        (System.Int32 EntityType, System.String itemName)=>{
                            var ea = new DeleteItemPreNotifyEventArgs(EntityType, itemName);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.DeleteItemPreNotify += callback;
                    return Disposable.Create(()=> eventSource.DeleteItemPreNotify-= callback);
                    
                }
            );
        }
        public class ClearSelectionsNotifyEventArgs
        {
            public ClearSelectionsNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_ClearSelectionsNotifyEventHandler.html
        public static IObservable<ClearSelectionsNotifyEventArgs> ClearSelectionsNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<ClearSelectionsNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_ClearSelectionsNotifyEventHandler callback = 
                        ()=>{
                            var ea = new ClearSelectionsNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.ClearSelectionsNotify += callback;
                    return Disposable.Create(()=> eventSource.ClearSelectionsNotify-= callback);
                    
                }
            );
        }
        public class EquationEditorPreNotifyEventArgs
        {
            public EquationEditorPreNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_EquationEditorPreNotifyEventHandler.html
        public static IObservable<EquationEditorPreNotifyEventArgs> EquationEditorPreNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<EquationEditorPreNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_EquationEditorPreNotifyEventHandler callback = 
                        ()=>{
                            var ea = new EquationEditorPreNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.EquationEditorPreNotify += callback;
                    return Disposable.Create(()=> eventSource.EquationEditorPreNotify-= callback);
                    
                }
            );
        }
        public class EquationEditorPostNotifyEventArgs
        {
            public EquationEditorPostNotifyEventArgs (System.Boolean Changed)
            {
                this.Changed = Changed;
            }
            public System.Boolean Changed { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_EquationEditorPostNotifyEventHandler.html
        public static IObservable<EquationEditorPostNotifyEventArgs> EquationEditorPostNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<EquationEditorPostNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_EquationEditorPostNotifyEventHandler callback = 
                        (System.Boolean Changed)=>{
                            var ea = new EquationEditorPostNotifyEventArgs(Changed);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.EquationEditorPostNotify += callback;
                    return Disposable.Create(()=> eventSource.EquationEditorPostNotify-= callback);
                    
                }
            );
        }
        public class OpenDesignTableNotifyEventArgs
        {
            public OpenDesignTableNotifyEventArgs (System.Object DesignTable)
            {
                this.DesignTable = DesignTable;
            }
            public System.Object DesignTable { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_OpenDesignTableNotifyEventHandler.html
        public static IObservable<OpenDesignTableNotifyEventArgs> OpenDesignTableNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<OpenDesignTableNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_OpenDesignTableNotifyEventHandler callback = 
                        (System.Object DesignTable)=>{
                            var ea = new OpenDesignTableNotifyEventArgs(DesignTable);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.OpenDesignTableNotify += callback;
                    return Disposable.Create(()=> eventSource.OpenDesignTableNotify-= callback);
                    
                }
            );
        }
        public class CloseDesignTableNotifyEventArgs
        {
            public CloseDesignTableNotifyEventArgs (System.Object DesignTable)
            {
                this.DesignTable = DesignTable;
            }
            public System.Object DesignTable { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_CloseDesignTableNotifyEventHandler.html
        public static IObservable<CloseDesignTableNotifyEventArgs> CloseDesignTableNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<CloseDesignTableNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_CloseDesignTableNotifyEventHandler callback = 
                        (System.Object DesignTable)=>{
                            var ea = new CloseDesignTableNotifyEventArgs(DesignTable);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.CloseDesignTableNotify += callback;
                    return Disposable.Create(()=> eventSource.CloseDesignTableNotify-= callback);
                    
                }
            );
        }
        public class UnitsChangeNotifyEventArgs
        {
            public UnitsChangeNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_UnitsChangeNotifyEventHandler.html
        public static IObservable<UnitsChangeNotifyEventArgs> UnitsChangeNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<UnitsChangeNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_UnitsChangeNotifyEventHandler callback = 
                        ()=>{
                            var ea = new UnitsChangeNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.UnitsChangeNotify += callback;
                    return Disposable.Create(()=> eventSource.UnitsChangeNotify-= callback);
                    
                }
            );
        }
        public class DestroyNotify2EventArgs
        {
            public DestroyNotify2EventArgs (System.Int32 DestroyType)
            {
                this.DestroyType = DestroyType;
            }
            public System.Int32 DestroyType { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_DestroyNotify2EventHandler.html
        public static IObservable<DestroyNotify2EventArgs> DestroyNotify2Observable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<DestroyNotify2EventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_DestroyNotify2EventHandler callback = 
                        (System.Int32 DestroyType)=>{
                            var ea = new DestroyNotify2EventArgs(DestroyType);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.DestroyNotify2 += callback;
                    return Disposable.Create(()=> eventSource.DestroyNotify2-= callback);
                    
                }
            );
        }
        public class ConfigurationChangeNotifyEventArgs
        {
            public ConfigurationChangeNotifyEventArgs (System.String ConfigurationName, System.Object Object, System.Int32 ObjectType, System.Int32 changeType)
            {
                this.ConfigurationName = ConfigurationName;
                this.Object = Object;
                this.ObjectType = ObjectType;
                this.changeType = changeType;
            }
            public System.String ConfigurationName { get; }
            public System.Object Object { get; }
            public System.Int32 ObjectType { get; }
            public System.Int32 changeType { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_ConfigurationChangeNotifyEventHandler.html
        public static IObservable<ConfigurationChangeNotifyEventArgs> ConfigurationChangeNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<ConfigurationChangeNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_ConfigurationChangeNotifyEventHandler callback = 
                        (System.String ConfigurationName, System.Object Object, System.Int32 ObjectType, System.Int32 changeType)=>{
                            var ea = new ConfigurationChangeNotifyEventArgs(ConfigurationName, Object, ObjectType, changeType);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.ConfigurationChangeNotify += callback;
                    return Disposable.Create(()=> eventSource.ConfigurationChangeNotify-= callback);
                    
                }
            );
        }
        public class ActiveViewChangeNotifyEventArgs
        {
            public ActiveViewChangeNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_ActiveViewChangeNotifyEventHandler.html
        public static IObservable<ActiveViewChangeNotifyEventArgs> ActiveViewChangeNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<ActiveViewChangeNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_ActiveViewChangeNotifyEventHandler callback = 
                        ()=>{
                            var ea = new ActiveViewChangeNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.ActiveViewChangeNotify += callback;
                    return Disposable.Create(()=> eventSource.ActiveViewChangeNotify-= callback);
                    
                }
            );
        }
        public class FeatureManagerFilterStringChangeNotifyEventArgs
        {
            public FeatureManagerFilterStringChangeNotifyEventArgs (System.String FilterString)
            {
                this.FilterString = FilterString;
            }
            public System.String FilterString { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_FeatureManagerFilterStringChangeNotifyEventHandler.html
        public static IObservable<FeatureManagerFilterStringChangeNotifyEventArgs> FeatureManagerFilterStringChangeNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<FeatureManagerFilterStringChangeNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_FeatureManagerFilterStringChangeNotifyEventHandler callback = 
                        (System.String FilterString)=>{
                            var ea = new FeatureManagerFilterStringChangeNotifyEventArgs(FilterString);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.FeatureManagerFilterStringChangeNotify += callback;
                    return Disposable.Create(()=> eventSource.FeatureManagerFilterStringChangeNotify-= callback);
                    
                }
            );
        }
        public class FlipLoopNotifyEventArgs
        {
            public FlipLoopNotifyEventArgs (System.Object TheLoop, System.Object TheEdge)
            {
                this.TheLoop = TheLoop;
                this.TheEdge = TheEdge;
            }
            public System.Object TheLoop { get; }
            public System.Object TheEdge { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_FlipLoopNotifyEventHandler.html
        public static IObservable<FlipLoopNotifyEventArgs> FlipLoopNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<FlipLoopNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_FlipLoopNotifyEventHandler callback = 
                        (System.Object TheLoop, System.Object TheEdge)=>{
                            var ea = new FlipLoopNotifyEventArgs(TheLoop, TheEdge);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.FlipLoopNotify += callback;
                    return Disposable.Create(()=> eventSource.FlipLoopNotify-= callback);
                    
                }
            );
        }
        public class AutoSaveNotifyEventArgs
        {
            public AutoSaveNotifyEventArgs (System.String FileName)
            {
                this.FileName = FileName;
            }
            public System.String FileName { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_AutoSaveNotifyEventHandler.html
        public static IObservable<AutoSaveNotifyEventArgs> AutoSaveNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<AutoSaveNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_AutoSaveNotifyEventHandler callback = 
                        (System.String FileName)=>{
                            var ea = new AutoSaveNotifyEventArgs(FileName);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.AutoSaveNotify += callback;
                    return Disposable.Create(()=> eventSource.AutoSaveNotify-= callback);
                    
                }
            );
        }
        public class AutoSaveToStorageNotifyEventArgs
        {
            public AutoSaveToStorageNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_AutoSaveToStorageNotifyEventHandler.html
        public static IObservable<AutoSaveToStorageNotifyEventArgs> AutoSaveToStorageNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<AutoSaveToStorageNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_AutoSaveToStorageNotifyEventHandler callback = 
                        ()=>{
                            var ea = new AutoSaveToStorageNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.AutoSaveToStorageNotify += callback;
                    return Disposable.Create(()=> eventSource.AutoSaveToStorageNotify-= callback);
                    
                }
            );
        }
        public class FileDropPreNotifyEventArgs
        {
            public FileDropPreNotifyEventArgs (System.String FileName)
            {
                this.FileName = FileName;
            }
            public System.String FileName { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_FileDropPreNotifyEventHandler.html
        public static IObservable<FileDropPreNotifyEventArgs> FileDropPreNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<FileDropPreNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_FileDropPreNotifyEventHandler callback = 
                        (System.String FileName)=>{
                            var ea = new FileDropPreNotifyEventArgs(FileName);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.FileDropPreNotify += callback;
                    return Disposable.Create(()=> eventSource.FileDropPreNotify-= callback);
                    
                }
            );
        }
        public class SensorAlertPreNotifyEventArgs
        {
            public SensorAlertPreNotifyEventArgs (System.Object SensorIn, System.Int32 SensorAlertType)
            {
                this.SensorIn = SensorIn;
                this.SensorAlertType = SensorAlertType;
            }
            public System.Object SensorIn { get; }
            public System.Int32 SensorAlertType { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_SensorAlertPreNotifyEventHandler.html
        public static IObservable<SensorAlertPreNotifyEventArgs> SensorAlertPreNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<SensorAlertPreNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_SensorAlertPreNotifyEventHandler callback = 
                        (System.Object SensorIn, System.Int32 SensorAlertType)=>{
                            var ea = new SensorAlertPreNotifyEventArgs(SensorIn, SensorAlertType);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.SensorAlertPreNotify += callback;
                    return Disposable.Create(()=> eventSource.SensorAlertPreNotify-= callback);
                    
                }
            );
        }
        public class UndoPostNotifyEventArgs
        {
            public UndoPostNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_UndoPostNotifyEventHandler.html
        public static IObservable<UndoPostNotifyEventArgs> UndoPostNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<UndoPostNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_UndoPostNotifyEventHandler callback = 
                        ()=>{
                            var ea = new UndoPostNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.UndoPostNotify += callback;
                    return Disposable.Create(()=> eventSource.UndoPostNotify-= callback);
                    
                }
            );
        }
        public class UserSelectionPreNotifyEventArgs
        {
            public UserSelectionPreNotifyEventArgs (System.Int32 SelType)
            {
                this.SelType = SelType;
            }
            public System.Int32 SelType { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_UserSelectionPreNotifyEventHandler.html
        public static IObservable<UserSelectionPreNotifyEventArgs> UserSelectionPreNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<UserSelectionPreNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_UserSelectionPreNotifyEventHandler callback = 
                        (System.Int32 SelType)=>{
                            var ea = new UserSelectionPreNotifyEventArgs(SelType);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.UserSelectionPreNotify += callback;
                    return Disposable.Create(()=> eventSource.UserSelectionPreNotify-= callback);
                    
                }
            );
        }
        public class ActiveDisplayStateChangePreNotifyEventArgs
        {
            public ActiveDisplayStateChangePreNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_ActiveDisplayStateChangePreNotifyEventHandler.html
        public static IObservable<ActiveDisplayStateChangePreNotifyEventArgs> ActiveDisplayStateChangePreNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<ActiveDisplayStateChangePreNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_ActiveDisplayStateChangePreNotifyEventHandler callback = 
                        ()=>{
                            var ea = new ActiveDisplayStateChangePreNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.ActiveDisplayStateChangePreNotify += callback;
                    return Disposable.Create(()=> eventSource.ActiveDisplayStateChangePreNotify-= callback);
                    
                }
            );
        }
        public class ActiveDisplayStateChangePostNotifyEventArgs
        {
            public ActiveDisplayStateChangePostNotifyEventArgs (System.String DisplayStateName)
            {
                this.DisplayStateName = DisplayStateName;
            }
            public System.String DisplayStateName { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_ActiveDisplayStateChangePostNotifyEventHandler.html
        public static IObservable<ActiveDisplayStateChangePostNotifyEventArgs> ActiveDisplayStateChangePostNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<ActiveDisplayStateChangePostNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_ActiveDisplayStateChangePostNotifyEventHandler callback = 
                        (System.String DisplayStateName)=>{
                            var ea = new ActiveDisplayStateChangePostNotifyEventArgs(DisplayStateName);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.ActiveDisplayStateChangePostNotify += callback;
                    return Disposable.Create(()=> eventSource.ActiveDisplayStateChangePostNotify-= callback);
                    
                }
            );
        }
        public class RedoPostNotifyEventArgs
        {
            public RedoPostNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_RedoPostNotifyEventHandler.html
        public static IObservable<RedoPostNotifyEventArgs> RedoPostNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<RedoPostNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_RedoPostNotifyEventHandler callback = 
                        ()=>{
                            var ea = new RedoPostNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.RedoPostNotify += callback;
                    return Disposable.Create(()=> eventSource.RedoPostNotify-= callback);
                    
                }
            );
        }
        public class RedoPreNotifyEventArgs
        {
            public RedoPreNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_RedoPreNotifyEventHandler.html
        public static IObservable<RedoPreNotifyEventArgs> RedoPreNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<RedoPreNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_RedoPreNotifyEventHandler callback = 
                        ()=>{
                            var ea = new RedoPreNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.RedoPreNotify += callback;
                    return Disposable.Create(()=> eventSource.RedoPreNotify-= callback);
                    
                }
            );
        }
        public class UndoPreNotifyEventArgs
        {
            public UndoPreNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_UndoPreNotifyEventHandler.html
        public static IObservable<UndoPreNotifyEventArgs> UndoPreNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<UndoPreNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_UndoPreNotifyEventHandler callback = 
                        ()=>{
                            var ea = new UndoPreNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.UndoPreNotify += callback;
                    return Disposable.Create(()=> eventSource.UndoPreNotify-= callback);
                    
                }
            );
        }
        public class WeldmentCutListUpdatePostNotifyEventArgs
        {
            public WeldmentCutListUpdatePostNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_WeldmentCutListUpdatePostNotifyEventHandler.html
        public static IObservable<WeldmentCutListUpdatePostNotifyEventArgs> WeldmentCutListUpdatePostNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<WeldmentCutListUpdatePostNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_WeldmentCutListUpdatePostNotifyEventHandler callback = 
                        ()=>{
                            var ea = new WeldmentCutListUpdatePostNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.WeldmentCutListUpdatePostNotify += callback;
                    return Disposable.Create(()=> eventSource.WeldmentCutListUpdatePostNotify-= callback);
                    
                }
            );
        }
        public class AutoSaveToStorageStoreNotifyEventArgs
        {
            public AutoSaveToStorageStoreNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_AutoSaveToStorageStoreNotifyEventHandler.html
        public static IObservable<AutoSaveToStorageStoreNotifyEventArgs> AutoSaveToStorageStoreNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<AutoSaveToStorageStoreNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_AutoSaveToStorageStoreNotifyEventHandler callback = 
                        ()=>{
                            var ea = new AutoSaveToStorageStoreNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.AutoSaveToStorageStoreNotify += callback;
                    return Disposable.Create(()=> eventSource.AutoSaveToStorageStoreNotify-= callback);
                    
                }
            );
        }
        public class DragStateChangeNotifyEventArgs
        {
            public DragStateChangeNotifyEventArgs (System.Boolean State)
            {
                this.State = State;
            }
            public System.Boolean State { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_DragStateChangeNotifyEventHandler.html
        public static IObservable<DragStateChangeNotifyEventArgs> DragStateChangeNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<DragStateChangeNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_DragStateChangeNotifyEventHandler callback = 
                        (System.Boolean State)=>{
                            var ea = new DragStateChangeNotifyEventArgs(State);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.DragStateChangeNotify += callback;
                    return Disposable.Create(()=> eventSource.DragStateChangeNotify-= callback);
                    
                }
            );
        }
        public class InsertTableNotifyEventArgs
        {
            public InsertTableNotifyEventArgs (SolidWorks.Interop.sldworks.TableAnnotation TableAnnotation, System.Int32 TableType, System.String TemplatePath)
            {
                this.TableAnnotation = TableAnnotation;
                this.TableType = TableType;
                this.TemplatePath = TemplatePath;
            }
            public SolidWorks.Interop.sldworks.TableAnnotation TableAnnotation { get; }
            public System.Int32 TableType { get; }
            public System.String TemplatePath { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_InsertTableNotifyEventHandler.html
        public static IObservable<InsertTableNotifyEventArgs> InsertTableNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<InsertTableNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_InsertTableNotifyEventHandler callback = 
                        (SolidWorks.Interop.sldworks.TableAnnotation TableAnnotation, System.Int32 TableType, System.String TemplatePath)=>{
                            var ea = new InsertTableNotifyEventArgs(TableAnnotation, TableType, TemplatePath);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.InsertTableNotify += callback;
                    return Disposable.Create(()=> eventSource.InsertTableNotify-= callback);
                    
                }
            );
        }
        public class ModifyTableNotifyEventArgs
        {
            public ModifyTableNotifyEventArgs (SolidWorks.Interop.sldworks.TableAnnotation TableAnnotation, System.Int32 TableType, System.Int32 reason, System.Int32 RowInfo, System.Int32 ColumnInfo, System.String DataInfo)
            {
                this.TableAnnotation = TableAnnotation;
                this.TableType = TableType;
                this.reason = reason;
                this.RowInfo = RowInfo;
                this.ColumnInfo = ColumnInfo;
                this.DataInfo = DataInfo;
            }
            public SolidWorks.Interop.sldworks.TableAnnotation TableAnnotation { get; }
            public System.Int32 TableType { get; }
            public System.Int32 reason { get; }
            public System.Int32 RowInfo { get; }
            public System.Int32 ColumnInfo { get; }
            public System.String DataInfo { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_ModifyTableNotifyEventHandler.html
        public static IObservable<ModifyTableNotifyEventArgs> ModifyTableNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<ModifyTableNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_ModifyTableNotifyEventHandler callback = 
                        (SolidWorks.Interop.sldworks.TableAnnotation TableAnnotation, System.Int32 TableType, System.Int32 reason, System.Int32 RowInfo, System.Int32 ColumnInfo, System.String DataInfo)=>{
                            var ea = new ModifyTableNotifyEventArgs(TableAnnotation, TableType, reason, RowInfo, ColumnInfo, DataInfo);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.ModifyTableNotify += callback;
                    return Disposable.Create(()=> eventSource.ModifyTableNotify-= callback);
                    
                }
            );
        }
        public class UserSelectionPostNotifyEventArgs
        {
            public UserSelectionPostNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_UserSelectionPostNotifyEventHandler.html
        public static IObservable<UserSelectionPostNotifyEventArgs> UserSelectionPostNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<UserSelectionPostNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_UserSelectionPostNotifyEventHandler callback = 
                        ()=>{
                            var ea = new UserSelectionPostNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.UserSelectionPostNotify += callback;
                    return Disposable.Create(()=> eventSource.UserSelectionPostNotify-= callback);
                    
                }
            );
        }
        public class CommandManagerTabActivatedPreNotifyEventArgs
        {
            public CommandManagerTabActivatedPreNotifyEventArgs (System.Int32 CommandTabIndex, System.String CommandTabName)
            {
                this.CommandTabIndex = CommandTabIndex;
                this.CommandTabName = CommandTabName;
            }
            public System.Int32 CommandTabIndex { get; }
            public System.String CommandTabName { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_CommandManagerTabActivatedPreNotifyEventHandler.html
        public static IObservable<CommandManagerTabActivatedPreNotifyEventArgs> CommandManagerTabActivatedPreNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<CommandManagerTabActivatedPreNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_CommandManagerTabActivatedPreNotifyEventHandler callback = 
                        (System.Int32 CommandTabIndex, System.String CommandTabName)=>{
                            var ea = new CommandManagerTabActivatedPreNotifyEventArgs(CommandTabIndex, CommandTabName);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.CommandManagerTabActivatedPreNotify += callback;
                    return Disposable.Create(()=> eventSource.CommandManagerTabActivatedPreNotify-= callback);
                    
                }
            );
        }
        public class PreRenameItemNotifyEventArgs
        {
            public PreRenameItemNotifyEventArgs (System.Int32 EntityType, System.String oldName, System.String NewName)
            {
                this.EntityType = EntityType;
                this.oldName = oldName;
                this.NewName = NewName;
            }
            public System.Int32 EntityType { get; }
            public System.String oldName { get; }
            public System.String NewName { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_PreRenameItemNotifyEventHandler.html
        public static IObservable<PreRenameItemNotifyEventArgs> PreRenameItemNotifyObservable(this SolidWorks.Interop.sldworks.DPartDocEvents_Event eventSource)
        {
            return Observable.Create<PreRenameItemNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DPartDocEvents_PreRenameItemNotifyEventHandler callback = 
                        (System.Int32 EntityType, System.String oldName, System.String NewName)=>{
                            var ea = new PreRenameItemNotifyEventArgs(EntityType, oldName, NewName);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.PreRenameItemNotify += callback;
                    return Disposable.Create(()=> eventSource.PreRenameItemNotify-= callback);
                    
                }
            );
        }
    }
}



namespace SolidworksAddinFramework.Events {
    public static class DAssemblyDocEvents_Event {

        public class RegenNotifyEventArgs
        {
            public RegenNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_RegenNotifyEventHandler.html
        public static IObservable<RegenNotifyEventArgs> RegenNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<RegenNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_RegenNotifyEventHandler callback = 
                        ()=>{
                            var ea = new RegenNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.RegenNotify += callback;
                    return Disposable.Create(()=> eventSource.RegenNotify-= callback);
                    
                }
            );
        }
        public class DestroyNotifyEventArgs
        {
            public DestroyNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_DestroyNotifyEventHandler.html
        public static IObservable<DestroyNotifyEventArgs> DestroyNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<DestroyNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_DestroyNotifyEventHandler callback = 
                        ()=>{
                            var ea = new DestroyNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.DestroyNotify += callback;
                    return Disposable.Create(()=> eventSource.DestroyNotify-= callback);
                    
                }
            );
        }
        public class RegenPostNotifyEventArgs
        {
            public RegenPostNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_RegenPostNotifyEventHandler.html
        public static IObservable<RegenPostNotifyEventArgs> RegenPostNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<RegenPostNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_RegenPostNotifyEventHandler callback = 
                        ()=>{
                            var ea = new RegenPostNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.RegenPostNotify += callback;
                    return Disposable.Create(()=> eventSource.RegenPostNotify-= callback);
                    
                }
            );
        }
        public class ViewNewNotifyEventArgs
        {
            public ViewNewNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_ViewNewNotifyEventHandler.html
        public static IObservable<ViewNewNotifyEventArgs> ViewNewNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<ViewNewNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_ViewNewNotifyEventHandler callback = 
                        ()=>{
                            var ea = new ViewNewNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.ViewNewNotify += callback;
                    return Disposable.Create(()=> eventSource.ViewNewNotify-= callback);
                    
                }
            );
        }
        public class NewSelectionNotifyEventArgs
        {
            public NewSelectionNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_NewSelectionNotifyEventHandler.html
        public static IObservable<NewSelectionNotifyEventArgs> NewSelectionNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<NewSelectionNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_NewSelectionNotifyEventHandler callback = 
                        ()=>{
                            var ea = new NewSelectionNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.NewSelectionNotify += callback;
                    return Disposable.Create(()=> eventSource.NewSelectionNotify-= callback);
                    
                }
            );
        }
        public class FileSaveNotifyEventArgs
        {
            public FileSaveNotifyEventArgs (System.String FileName)
            {
                this.FileName = FileName;
            }
            public System.String FileName { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_FileSaveNotifyEventHandler.html
        public static IObservable<FileSaveNotifyEventArgs> FileSaveNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<FileSaveNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_FileSaveNotifyEventHandler callback = 
                        (System.String FileName)=>{
                            var ea = new FileSaveNotifyEventArgs(FileName);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.FileSaveNotify += callback;
                    return Disposable.Create(()=> eventSource.FileSaveNotify-= callback);
                    
                }
            );
        }
        public class FileSaveAsNotifyEventArgs
        {
            public FileSaveAsNotifyEventArgs (System.String FileName)
            {
                this.FileName = FileName;
            }
            public System.String FileName { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_FileSaveAsNotifyEventHandler.html
        public static IObservable<FileSaveAsNotifyEventArgs> FileSaveAsNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<FileSaveAsNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_FileSaveAsNotifyEventHandler callback = 
                        (System.String FileName)=>{
                            var ea = new FileSaveAsNotifyEventArgs(FileName);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.FileSaveAsNotify += callback;
                    return Disposable.Create(()=> eventSource.FileSaveAsNotify-= callback);
                    
                }
            );
        }
        public class LoadFromStorageNotifyEventArgs
        {
            public LoadFromStorageNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_LoadFromStorageNotifyEventHandler.html
        public static IObservable<LoadFromStorageNotifyEventArgs> LoadFromStorageNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<LoadFromStorageNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_LoadFromStorageNotifyEventHandler callback = 
                        ()=>{
                            var ea = new LoadFromStorageNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.LoadFromStorageNotify += callback;
                    return Disposable.Create(()=> eventSource.LoadFromStorageNotify-= callback);
                    
                }
            );
        }
        public class SaveToStorageNotifyEventArgs
        {
            public SaveToStorageNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_SaveToStorageNotifyEventHandler.html
        public static IObservable<SaveToStorageNotifyEventArgs> SaveToStorageNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<SaveToStorageNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_SaveToStorageNotifyEventHandler callback = 
                        ()=>{
                            var ea = new SaveToStorageNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.SaveToStorageNotify += callback;
                    return Disposable.Create(()=> eventSource.SaveToStorageNotify-= callback);
                    
                }
            );
        }
        public class ActiveConfigChangeNotifyEventArgs
        {
            public ActiveConfigChangeNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_ActiveConfigChangeNotifyEventHandler.html
        public static IObservable<ActiveConfigChangeNotifyEventArgs> ActiveConfigChangeNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<ActiveConfigChangeNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_ActiveConfigChangeNotifyEventHandler callback = 
                        ()=>{
                            var ea = new ActiveConfigChangeNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.ActiveConfigChangeNotify += callback;
                    return Disposable.Create(()=> eventSource.ActiveConfigChangeNotify-= callback);
                    
                }
            );
        }
        public class ActiveConfigChangePostNotifyEventArgs
        {
            public ActiveConfigChangePostNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_ActiveConfigChangePostNotifyEventHandler.html
        public static IObservable<ActiveConfigChangePostNotifyEventArgs> ActiveConfigChangePostNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<ActiveConfigChangePostNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_ActiveConfigChangePostNotifyEventHandler callback = 
                        ()=>{
                            var ea = new ActiveConfigChangePostNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.ActiveConfigChangePostNotify += callback;
                    return Disposable.Create(()=> eventSource.ActiveConfigChangePostNotify-= callback);
                    
                }
            );
        }
        public class BeginInContextEditNotifyEventArgs
        {
            public BeginInContextEditNotifyEventArgs (System.Object docBeingEdited, System.Int32 DocType)
            {
                this.docBeingEdited = docBeingEdited;
                this.DocType = DocType;
            }
            public System.Object docBeingEdited { get; }
            public System.Int32 DocType { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_BeginInContextEditNotifyEventHandler.html
        public static IObservable<BeginInContextEditNotifyEventArgs> BeginInContextEditNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<BeginInContextEditNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_BeginInContextEditNotifyEventHandler callback = 
                        (System.Object docBeingEdited, System.Int32 DocType)=>{
                            var ea = new BeginInContextEditNotifyEventArgs(docBeingEdited, DocType);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.BeginInContextEditNotify += callback;
                    return Disposable.Create(()=> eventSource.BeginInContextEditNotify-= callback);
                    
                }
            );
        }
        public class EndInContextEditNotifyEventArgs
        {
            public EndInContextEditNotifyEventArgs (System.Object docBeingEdited, System.Int32 DocType)
            {
                this.docBeingEdited = docBeingEdited;
                this.DocType = DocType;
            }
            public System.Object docBeingEdited { get; }
            public System.Int32 DocType { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_EndInContextEditNotifyEventHandler.html
        public static IObservable<EndInContextEditNotifyEventArgs> EndInContextEditNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<EndInContextEditNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_EndInContextEditNotifyEventHandler callback = 
                        (System.Object docBeingEdited, System.Int32 DocType)=>{
                            var ea = new EndInContextEditNotifyEventArgs(docBeingEdited, DocType);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.EndInContextEditNotify += callback;
                    return Disposable.Create(()=> eventSource.EndInContextEditNotify-= callback);
                    
                }
            );
        }
        public class ViewNewNotify2EventArgs
        {
            public ViewNewNotify2EventArgs (System.Object viewBeingAdded)
            {
                this.viewBeingAdded = viewBeingAdded;
            }
            public System.Object viewBeingAdded { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_ViewNewNotify2EventHandler.html
        public static IObservable<ViewNewNotify2EventArgs> ViewNewNotify2Observable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<ViewNewNotify2EventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_ViewNewNotify2EventHandler callback = 
                        (System.Object viewBeingAdded)=>{
                            var ea = new ViewNewNotify2EventArgs(viewBeingAdded);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.ViewNewNotify2 += callback;
                    return Disposable.Create(()=> eventSource.ViewNewNotify2-= callback);
                    
                }
            );
        }
        public class LightingDialogCreateNotifyEventArgs
        {
            public LightingDialogCreateNotifyEventArgs (System.Object dialog)
            {
                this.dialog = dialog;
            }
            public System.Object dialog { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_LightingDialogCreateNotifyEventHandler.html
        public static IObservable<LightingDialogCreateNotifyEventArgs> LightingDialogCreateNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<LightingDialogCreateNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_LightingDialogCreateNotifyEventHandler callback = 
                        (System.Object dialog)=>{
                            var ea = new LightingDialogCreateNotifyEventArgs(dialog);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.LightingDialogCreateNotify += callback;
                    return Disposable.Create(()=> eventSource.LightingDialogCreateNotify-= callback);
                    
                }
            );
        }
        public class AddItemNotifyEventArgs
        {
            public AddItemNotifyEventArgs (System.Int32 EntityType, System.String itemName)
            {
                this.EntityType = EntityType;
                this.itemName = itemName;
            }
            public System.Int32 EntityType { get; }
            public System.String itemName { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_AddItemNotifyEventHandler.html
        public static IObservable<AddItemNotifyEventArgs> AddItemNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<AddItemNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_AddItemNotifyEventHandler callback = 
                        (System.Int32 EntityType, System.String itemName)=>{
                            var ea = new AddItemNotifyEventArgs(EntityType, itemName);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.AddItemNotify += callback;
                    return Disposable.Create(()=> eventSource.AddItemNotify-= callback);
                    
                }
            );
        }
        public class RenameItemNotifyEventArgs
        {
            public RenameItemNotifyEventArgs (System.Int32 EntityType, System.String oldName, System.String NewName)
            {
                this.EntityType = EntityType;
                this.oldName = oldName;
                this.NewName = NewName;
            }
            public System.Int32 EntityType { get; }
            public System.String oldName { get; }
            public System.String NewName { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_RenameItemNotifyEventHandler.html
        public static IObservable<RenameItemNotifyEventArgs> RenameItemNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<RenameItemNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_RenameItemNotifyEventHandler callback = 
                        (System.Int32 EntityType, System.String oldName, System.String NewName)=>{
                            var ea = new RenameItemNotifyEventArgs(EntityType, oldName, NewName);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.RenameItemNotify += callback;
                    return Disposable.Create(()=> eventSource.RenameItemNotify-= callback);
                    
                }
            );
        }
        public class DeleteItemNotifyEventArgs
        {
            public DeleteItemNotifyEventArgs (System.Int32 EntityType, System.String itemName)
            {
                this.EntityType = EntityType;
                this.itemName = itemName;
            }
            public System.Int32 EntityType { get; }
            public System.String itemName { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_DeleteItemNotifyEventHandler.html
        public static IObservable<DeleteItemNotifyEventArgs> DeleteItemNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<DeleteItemNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_DeleteItemNotifyEventHandler callback = 
                        (System.Int32 EntityType, System.String itemName)=>{
                            var ea = new DeleteItemNotifyEventArgs(EntityType, itemName);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.DeleteItemNotify += callback;
                    return Disposable.Create(()=> eventSource.DeleteItemNotify-= callback);
                    
                }
            );
        }
        public class ModifyNotifyEventArgs
        {
            public ModifyNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_ModifyNotifyEventHandler.html
        public static IObservable<ModifyNotifyEventArgs> ModifyNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<ModifyNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_ModifyNotifyEventHandler callback = 
                        ()=>{
                            var ea = new ModifyNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.ModifyNotify += callback;
                    return Disposable.Create(()=> eventSource.ModifyNotify-= callback);
                    
                }
            );
        }
        public class ComponentStateChangeNotifyEventArgs
        {
            public ComponentStateChangeNotifyEventArgs (System.Object componentModel, System.Int16 oldCompState, System.Int16 newCompState)
            {
                this.componentModel = componentModel;
                this.oldCompState = oldCompState;
                this.newCompState = newCompState;
            }
            public System.Object componentModel { get; }
            public System.Int16 oldCompState { get; }
            public System.Int16 newCompState { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_ComponentStateChangeNotifyEventHandler.html
        public static IObservable<ComponentStateChangeNotifyEventArgs> ComponentStateChangeNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<ComponentStateChangeNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_ComponentStateChangeNotifyEventHandler callback = 
                        (System.Object componentModel, System.Int16 oldCompState, System.Int16 newCompState)=>{
                            var ea = new ComponentStateChangeNotifyEventArgs(componentModel, oldCompState, newCompState);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.ComponentStateChangeNotify += callback;
                    return Disposable.Create(()=> eventSource.ComponentStateChangeNotify-= callback);
                    
                }
            );
        }
        public class FileDropNotifyEventArgs
        {
            public FileDropNotifyEventArgs (System.String FileName)
            {
                this.FileName = FileName;
            }
            public System.String FileName { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_FileDropNotifyEventHandler.html
        public static IObservable<FileDropNotifyEventArgs> FileDropNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<FileDropNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_FileDropNotifyEventHandler callback = 
                        (System.String FileName)=>{
                            var ea = new FileDropNotifyEventArgs(FileName);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.FileDropNotify += callback;
                    return Disposable.Create(()=> eventSource.FileDropNotify-= callback);
                    
                }
            );
        }
        public class FileReloadNotifyEventArgs
        {
            public FileReloadNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_FileReloadNotifyEventHandler.html
        public static IObservable<FileReloadNotifyEventArgs> FileReloadNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<FileReloadNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_FileReloadNotifyEventHandler callback = 
                        ()=>{
                            var ea = new FileReloadNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.FileReloadNotify += callback;
                    return Disposable.Create(()=> eventSource.FileReloadNotify-= callback);
                    
                }
            );
        }
        public class ComponentStateChangeNotify2EventArgs
        {
            public ComponentStateChangeNotify2EventArgs (System.Object componentModel, System.String CompName, System.Int16 oldCompState, System.Int16 newCompState)
            {
                this.componentModel = componentModel;
                this.CompName = CompName;
                this.oldCompState = oldCompState;
                this.newCompState = newCompState;
            }
            public System.Object componentModel { get; }
            public System.String CompName { get; }
            public System.Int16 oldCompState { get; }
            public System.Int16 newCompState { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_ComponentStateChangeNotify2EventHandler.html
        public static IObservable<ComponentStateChangeNotify2EventArgs> ComponentStateChangeNotify2Observable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<ComponentStateChangeNotify2EventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_ComponentStateChangeNotify2EventHandler callback = 
                        (System.Object componentModel, System.String CompName, System.Int16 oldCompState, System.Int16 newCompState)=>{
                            var ea = new ComponentStateChangeNotify2EventArgs(componentModel, CompName, oldCompState, newCompState);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.ComponentStateChangeNotify2 += callback;
                    return Disposable.Create(()=> eventSource.ComponentStateChangeNotify2-= callback);
                    
                }
            );
        }
        public class AddCustomPropertyNotifyEventArgs
        {
            public AddCustomPropertyNotifyEventArgs (System.String propName, System.String Configuration, System.String Value, System.Int32 valueType)
            {
                this.propName = propName;
                this.Configuration = Configuration;
                this.Value = Value;
                this.valueType = valueType;
            }
            public System.String propName { get; }
            public System.String Configuration { get; }
            public System.String Value { get; }
            public System.Int32 valueType { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_AddCustomPropertyNotifyEventHandler.html
        public static IObservable<AddCustomPropertyNotifyEventArgs> AddCustomPropertyNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<AddCustomPropertyNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_AddCustomPropertyNotifyEventHandler callback = 
                        (System.String propName, System.String Configuration, System.String Value, System.Int32 valueType)=>{
                            var ea = new AddCustomPropertyNotifyEventArgs(propName, Configuration, Value, valueType);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.AddCustomPropertyNotify += callback;
                    return Disposable.Create(()=> eventSource.AddCustomPropertyNotify-= callback);
                    
                }
            );
        }
        public class ChangeCustomPropertyNotifyEventArgs
        {
            public ChangeCustomPropertyNotifyEventArgs (System.String propName, System.String Configuration, System.String oldValue, System.String NewValue, System.Int32 valueType)
            {
                this.propName = propName;
                this.Configuration = Configuration;
                this.oldValue = oldValue;
                this.NewValue = NewValue;
                this.valueType = valueType;
            }
            public System.String propName { get; }
            public System.String Configuration { get; }
            public System.String oldValue { get; }
            public System.String NewValue { get; }
            public System.Int32 valueType { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_ChangeCustomPropertyNotifyEventHandler.html
        public static IObservable<ChangeCustomPropertyNotifyEventArgs> ChangeCustomPropertyNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<ChangeCustomPropertyNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_ChangeCustomPropertyNotifyEventHandler callback = 
                        (System.String propName, System.String Configuration, System.String oldValue, System.String NewValue, System.Int32 valueType)=>{
                            var ea = new ChangeCustomPropertyNotifyEventArgs(propName, Configuration, oldValue, NewValue, valueType);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.ChangeCustomPropertyNotify += callback;
                    return Disposable.Create(()=> eventSource.ChangeCustomPropertyNotify-= callback);
                    
                }
            );
        }
        public class DeleteCustomPropertyNotifyEventArgs
        {
            public DeleteCustomPropertyNotifyEventArgs (System.String propName, System.String Configuration, System.String Value, System.Int32 valueType)
            {
                this.propName = propName;
                this.Configuration = Configuration;
                this.Value = Value;
                this.valueType = valueType;
            }
            public System.String propName { get; }
            public System.String Configuration { get; }
            public System.String Value { get; }
            public System.Int32 valueType { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_DeleteCustomPropertyNotifyEventHandler.html
        public static IObservable<DeleteCustomPropertyNotifyEventArgs> DeleteCustomPropertyNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<DeleteCustomPropertyNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_DeleteCustomPropertyNotifyEventHandler callback = 
                        (System.String propName, System.String Configuration, System.String Value, System.Int32 valueType)=>{
                            var ea = new DeleteCustomPropertyNotifyEventArgs(propName, Configuration, Value, valueType);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.DeleteCustomPropertyNotify += callback;
                    return Disposable.Create(()=> eventSource.DeleteCustomPropertyNotify-= callback);
                    
                }
            );
        }
        public class FeatureEditPreNotifyEventArgs
        {
            public FeatureEditPreNotifyEventArgs (System.Object EditFeature)
            {
                this.EditFeature = EditFeature;
            }
            public System.Object EditFeature { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_FeatureEditPreNotifyEventHandler.html
        public static IObservable<FeatureEditPreNotifyEventArgs> FeatureEditPreNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<FeatureEditPreNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_FeatureEditPreNotifyEventHandler callback = 
                        (System.Object EditFeature)=>{
                            var ea = new FeatureEditPreNotifyEventArgs(EditFeature);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.FeatureEditPreNotify += callback;
                    return Disposable.Create(()=> eventSource.FeatureEditPreNotify-= callback);
                    
                }
            );
        }
        public class FeatureSketchEditPreNotifyEventArgs
        {
            public FeatureSketchEditPreNotifyEventArgs (System.Object EditFeature, System.Object featureSketch)
            {
                this.EditFeature = EditFeature;
                this.featureSketch = featureSketch;
            }
            public System.Object EditFeature { get; }
            public System.Object featureSketch { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_FeatureSketchEditPreNotifyEventHandler.html
        public static IObservable<FeatureSketchEditPreNotifyEventArgs> FeatureSketchEditPreNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<FeatureSketchEditPreNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_FeatureSketchEditPreNotifyEventHandler callback = 
                        (System.Object EditFeature, System.Object featureSketch)=>{
                            var ea = new FeatureSketchEditPreNotifyEventArgs(EditFeature, featureSketch);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.FeatureSketchEditPreNotify += callback;
                    return Disposable.Create(()=> eventSource.FeatureSketchEditPreNotify-= callback);
                    
                }
            );
        }
        public class FileSaveAsNotify2EventArgs
        {
            public FileSaveAsNotify2EventArgs (System.String FileName)
            {
                this.FileName = FileName;
            }
            public System.String FileName { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_FileSaveAsNotify2EventHandler.html
        public static IObservable<FileSaveAsNotify2EventArgs> FileSaveAsNotify2Observable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<FileSaveAsNotify2EventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_FileSaveAsNotify2EventHandler callback = 
                        (System.String FileName)=>{
                            var ea = new FileSaveAsNotify2EventArgs(FileName);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.FileSaveAsNotify2 += callback;
                    return Disposable.Create(()=> eventSource.FileSaveAsNotify2-= callback);
                    
                }
            );
        }
        public class DeleteSelectionPreNotifyEventArgs
        {
            public DeleteSelectionPreNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_DeleteSelectionPreNotifyEventHandler.html
        public static IObservable<DeleteSelectionPreNotifyEventArgs> DeleteSelectionPreNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<DeleteSelectionPreNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_DeleteSelectionPreNotifyEventHandler callback = 
                        ()=>{
                            var ea = new DeleteSelectionPreNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.DeleteSelectionPreNotify += callback;
                    return Disposable.Create(()=> eventSource.DeleteSelectionPreNotify-= callback);
                    
                }
            );
        }
        public class FileReloadPreNotifyEventArgs
        {
            public FileReloadPreNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_FileReloadPreNotifyEventHandler.html
        public static IObservable<FileReloadPreNotifyEventArgs> FileReloadPreNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<FileReloadPreNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_FileReloadPreNotifyEventHandler callback = 
                        ()=>{
                            var ea = new FileReloadPreNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.FileReloadPreNotify += callback;
                    return Disposable.Create(()=> eventSource.FileReloadPreNotify-= callback);
                    
                }
            );
        }
        public class ComponentMoveNotifyEventArgs
        {
            public ComponentMoveNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_ComponentMoveNotifyEventHandler.html
        public static IObservable<ComponentMoveNotifyEventArgs> ComponentMoveNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<ComponentMoveNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_ComponentMoveNotifyEventHandler callback = 
                        ()=>{
                            var ea = new ComponentMoveNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.ComponentMoveNotify += callback;
                    return Disposable.Create(()=> eventSource.ComponentMoveNotify-= callback);
                    
                }
            );
        }
        public class ComponentVisibleChangeNotifyEventArgs
        {
            public ComponentVisibleChangeNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_ComponentVisibleChangeNotifyEventHandler.html
        public static IObservable<ComponentVisibleChangeNotifyEventArgs> ComponentVisibleChangeNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<ComponentVisibleChangeNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_ComponentVisibleChangeNotifyEventHandler callback = 
                        ()=>{
                            var ea = new ComponentVisibleChangeNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.ComponentVisibleChangeNotify += callback;
                    return Disposable.Create(()=> eventSource.ComponentVisibleChangeNotify-= callback);
                    
                }
            );
        }
        public class BodyVisibleChangeNotifyEventArgs
        {
            public BodyVisibleChangeNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_BodyVisibleChangeNotifyEventHandler.html
        public static IObservable<BodyVisibleChangeNotifyEventArgs> BodyVisibleChangeNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<BodyVisibleChangeNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_BodyVisibleChangeNotifyEventHandler callback = 
                        ()=>{
                            var ea = new BodyVisibleChangeNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.BodyVisibleChangeNotify += callback;
                    return Disposable.Create(()=> eventSource.BodyVisibleChangeNotify-= callback);
                    
                }
            );
        }
        public class FileDropPreNotifyEventArgs
        {
            public FileDropPreNotifyEventArgs (System.String FileName)
            {
                this.FileName = FileName;
            }
            public System.String FileName { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_FileDropPreNotifyEventHandler.html
        public static IObservable<FileDropPreNotifyEventArgs> FileDropPreNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<FileDropPreNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_FileDropPreNotifyEventHandler callback = 
                        (System.String FileName)=>{
                            var ea = new FileDropPreNotifyEventArgs(FileName);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.FileDropPreNotify += callback;
                    return Disposable.Create(()=> eventSource.FileDropPreNotify-= callback);
                    
                }
            );
        }
        public class FileSavePostNotifyEventArgs
        {
            public FileSavePostNotifyEventArgs (System.Int32 saveType, System.String FileName)
            {
                this.saveType = saveType;
                this.FileName = FileName;
            }
            public System.Int32 saveType { get; }
            public System.String FileName { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_FileSavePostNotifyEventHandler.html
        public static IObservable<FileSavePostNotifyEventArgs> FileSavePostNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<FileSavePostNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_FileSavePostNotifyEventHandler callback = 
                        (System.Int32 saveType, System.String FileName)=>{
                            var ea = new FileSavePostNotifyEventArgs(saveType, FileName);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.FileSavePostNotify += callback;
                    return Disposable.Create(()=> eventSource.FileSavePostNotify-= callback);
                    
                }
            );
        }
        public class LoadFromStorageStoreNotifyEventArgs
        {
            public LoadFromStorageStoreNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_LoadFromStorageStoreNotifyEventHandler.html
        public static IObservable<LoadFromStorageStoreNotifyEventArgs> LoadFromStorageStoreNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<LoadFromStorageStoreNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_LoadFromStorageStoreNotifyEventHandler callback = 
                        ()=>{
                            var ea = new LoadFromStorageStoreNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.LoadFromStorageStoreNotify += callback;
                    return Disposable.Create(()=> eventSource.LoadFromStorageStoreNotify-= callback);
                    
                }
            );
        }
        public class SaveToStorageStoreNotifyEventArgs
        {
            public SaveToStorageStoreNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_SaveToStorageStoreNotifyEventHandler.html
        public static IObservable<SaveToStorageStoreNotifyEventArgs> SaveToStorageStoreNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<SaveToStorageStoreNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_SaveToStorageStoreNotifyEventHandler callback = 
                        ()=>{
                            var ea = new SaveToStorageStoreNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.SaveToStorageStoreNotify += callback;
                    return Disposable.Create(()=> eventSource.SaveToStorageStoreNotify-= callback);
                    
                }
            );
        }
        public class FeatureManagerTreeRebuildNotifyEventArgs
        {
            public FeatureManagerTreeRebuildNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_FeatureManagerTreeRebuildNotifyEventHandler.html
        public static IObservable<FeatureManagerTreeRebuildNotifyEventArgs> FeatureManagerTreeRebuildNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<FeatureManagerTreeRebuildNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_FeatureManagerTreeRebuildNotifyEventHandler callback = 
                        ()=>{
                            var ea = new FeatureManagerTreeRebuildNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.FeatureManagerTreeRebuildNotify += callback;
                    return Disposable.Create(()=> eventSource.FeatureManagerTreeRebuildNotify-= callback);
                    
                }
            );
        }
        public class AssemblyElectricalDataUpdateNotifyEventArgs
        {
            public AssemblyElectricalDataUpdateNotifyEventArgs (System.Int32 saveType)
            {
                this.saveType = saveType;
            }
            public System.Int32 saveType { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_AssemblyElectricalDataUpdateNotifyEventHandler.html
        public static IObservable<AssemblyElectricalDataUpdateNotifyEventArgs> AssemblyElectricalDataUpdateNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<AssemblyElectricalDataUpdateNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_AssemblyElectricalDataUpdateNotifyEventHandler callback = 
                        (System.Int32 saveType)=>{
                            var ea = new AssemblyElectricalDataUpdateNotifyEventArgs(saveType);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.AssemblyElectricalDataUpdateNotify += callback;
                    return Disposable.Create(()=> eventSource.AssemblyElectricalDataUpdateNotify-= callback);
                    
                }
            );
        }
        public class DynamicHighlightNotifyEventArgs
        {
            public DynamicHighlightNotifyEventArgs (System.Boolean bHighlightState)
            {
                this.bHighlightState = bHighlightState;
            }
            public System.Boolean bHighlightState { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_DynamicHighlightNotifyEventHandler.html
        public static IObservable<DynamicHighlightNotifyEventArgs> DynamicHighlightNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<DynamicHighlightNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_DynamicHighlightNotifyEventHandler callback = 
                        (System.Boolean bHighlightState)=>{
                            var ea = new DynamicHighlightNotifyEventArgs(bHighlightState);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.DynamicHighlightNotify += callback;
                    return Disposable.Create(()=> eventSource.DynamicHighlightNotify-= callback);
                    
                }
            );
        }
        public class ComponentVisualPropertiesChangeNotifyEventArgs
        {
            public ComponentVisualPropertiesChangeNotifyEventArgs (System.Object Component)
            {
                this.Component = Component;
            }
            public System.Object Component { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_ComponentVisualPropertiesChangeNotifyEventHandler.html
        public static IObservable<ComponentVisualPropertiesChangeNotifyEventArgs> ComponentVisualPropertiesChangeNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<ComponentVisualPropertiesChangeNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_ComponentVisualPropertiesChangeNotifyEventHandler callback = 
                        (System.Object Component)=>{
                            var ea = new ComponentVisualPropertiesChangeNotifyEventArgs(Component);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.ComponentVisualPropertiesChangeNotify += callback;
                    return Disposable.Create(()=> eventSource.ComponentVisualPropertiesChangeNotify-= callback);
                    
                }
            );
        }
        public class ComponentDisplayStateChangeNotifyEventArgs
        {
            public ComponentDisplayStateChangeNotifyEventArgs (System.Object Component)
            {
                this.Component = Component;
            }
            public System.Object Component { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_ComponentDisplayStateChangeNotifyEventHandler.html
        public static IObservable<ComponentDisplayStateChangeNotifyEventArgs> ComponentDisplayStateChangeNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<ComponentDisplayStateChangeNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_ComponentDisplayStateChangeNotifyEventHandler callback = 
                        (System.Object Component)=>{
                            var ea = new ComponentDisplayStateChangeNotifyEventArgs(Component);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.ComponentDisplayStateChangeNotify += callback;
                    return Disposable.Create(()=> eventSource.ComponentDisplayStateChangeNotify-= callback);
                    
                }
            );
        }
        public class DimensionChangeNotifyEventArgs
        {
            public DimensionChangeNotifyEventArgs (System.Object displayDim)
            {
                this.displayDim = displayDim;
            }
            public System.Object displayDim { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_DimensionChangeNotifyEventHandler.html
        public static IObservable<DimensionChangeNotifyEventArgs> DimensionChangeNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<DimensionChangeNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_DimensionChangeNotifyEventHandler callback = 
                        (System.Object displayDim)=>{
                            var ea = new DimensionChangeNotifyEventArgs(displayDim);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.DimensionChangeNotify += callback;
                    return Disposable.Create(()=> eventSource.DimensionChangeNotify-= callback);
                    
                }
            );
        }
        public class FileReloadCancelNotifyEventArgs
        {
            public FileReloadCancelNotifyEventArgs (System.Int32 ErrorCode)
            {
                this.ErrorCode = ErrorCode;
            }
            public System.Int32 ErrorCode { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_FileReloadCancelNotifyEventHandler.html
        public static IObservable<FileReloadCancelNotifyEventArgs> FileReloadCancelNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<FileReloadCancelNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_FileReloadCancelNotifyEventHandler callback = 
                        (System.Int32 ErrorCode)=>{
                            var ea = new FileReloadCancelNotifyEventArgs(ErrorCode);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.FileReloadCancelNotify += callback;
                    return Disposable.Create(()=> eventSource.FileReloadCancelNotify-= callback);
                    
                }
            );
        }
        public class FileSavePostCancelNotifyEventArgs
        {
            public FileSavePostCancelNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_FileSavePostCancelNotifyEventHandler.html
        public static IObservable<FileSavePostCancelNotifyEventArgs> FileSavePostCancelNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<FileSavePostCancelNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_FileSavePostCancelNotifyEventHandler callback = 
                        ()=>{
                            var ea = new FileSavePostCancelNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.FileSavePostCancelNotify += callback;
                    return Disposable.Create(()=> eventSource.FileSavePostCancelNotify-= callback);
                    
                }
            );
        }
        public class SketchSolveNotifyEventArgs
        {
            public SketchSolveNotifyEventArgs (System.String featName)
            {
                this.featName = featName;
            }
            public System.String featName { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_SketchSolveNotifyEventHandler.html
        public static IObservable<SketchSolveNotifyEventArgs> SketchSolveNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<SketchSolveNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_SketchSolveNotifyEventHandler callback = 
                        (System.String featName)=>{
                            var ea = new SketchSolveNotifyEventArgs(featName);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.SketchSolveNotify += callback;
                    return Disposable.Create(()=> eventSource.SketchSolveNotify-= callback);
                    
                }
            );
        }
        public class DeleteItemPreNotifyEventArgs
        {
            public DeleteItemPreNotifyEventArgs (System.Int32 EntityType, System.String itemName)
            {
                this.EntityType = EntityType;
                this.itemName = itemName;
            }
            public System.Int32 EntityType { get; }
            public System.String itemName { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_DeleteItemPreNotifyEventHandler.html
        public static IObservable<DeleteItemPreNotifyEventArgs> DeleteItemPreNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<DeleteItemPreNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_DeleteItemPreNotifyEventHandler callback = 
                        (System.Int32 EntityType, System.String itemName)=>{
                            var ea = new DeleteItemPreNotifyEventArgs(EntityType, itemName);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.DeleteItemPreNotify += callback;
                    return Disposable.Create(()=> eventSource.DeleteItemPreNotify-= callback);
                    
                }
            );
        }
        public class ClearSelectionsNotifyEventArgs
        {
            public ClearSelectionsNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_ClearSelectionsNotifyEventHandler.html
        public static IObservable<ClearSelectionsNotifyEventArgs> ClearSelectionsNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<ClearSelectionsNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_ClearSelectionsNotifyEventHandler callback = 
                        ()=>{
                            var ea = new ClearSelectionsNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.ClearSelectionsNotify += callback;
                    return Disposable.Create(()=> eventSource.ClearSelectionsNotify-= callback);
                    
                }
            );
        }
        public class FileDropPostNotifyEventArgs
        {
            public FileDropPostNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_FileDropPostNotifyEventHandler.html
        public static IObservable<FileDropPostNotifyEventArgs> FileDropPostNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<FileDropPostNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_FileDropPostNotifyEventHandler callback = 
                        ()=>{
                            var ea = new FileDropPostNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.FileDropPostNotify += callback;
                    return Disposable.Create(()=> eventSource.FileDropPostNotify-= callback);
                    
                }
            );
        }
        public class EquationEditorPreNotifyEventArgs
        {
            public EquationEditorPreNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_EquationEditorPreNotifyEventHandler.html
        public static IObservable<EquationEditorPreNotifyEventArgs> EquationEditorPreNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<EquationEditorPreNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_EquationEditorPreNotifyEventHandler callback = 
                        ()=>{
                            var ea = new EquationEditorPreNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.EquationEditorPreNotify += callback;
                    return Disposable.Create(()=> eventSource.EquationEditorPreNotify-= callback);
                    
                }
            );
        }
        public class EquationEditorPostNotifyEventArgs
        {
            public EquationEditorPostNotifyEventArgs (System.Boolean Changed)
            {
                this.Changed = Changed;
            }
            public System.Boolean Changed { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_EquationEditorPostNotifyEventHandler.html
        public static IObservable<EquationEditorPostNotifyEventArgs> EquationEditorPostNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<EquationEditorPostNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_EquationEditorPostNotifyEventHandler callback = 
                        (System.Boolean Changed)=>{
                            var ea = new EquationEditorPostNotifyEventArgs(Changed);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.EquationEditorPostNotify += callback;
                    return Disposable.Create(()=> eventSource.EquationEditorPostNotify-= callback);
                    
                }
            );
        }
        public class OpenDesignTableNotifyEventArgs
        {
            public OpenDesignTableNotifyEventArgs (System.Object DesignTable)
            {
                this.DesignTable = DesignTable;
            }
            public System.Object DesignTable { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_OpenDesignTableNotifyEventHandler.html
        public static IObservable<OpenDesignTableNotifyEventArgs> OpenDesignTableNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<OpenDesignTableNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_OpenDesignTableNotifyEventHandler callback = 
                        (System.Object DesignTable)=>{
                            var ea = new OpenDesignTableNotifyEventArgs(DesignTable);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.OpenDesignTableNotify += callback;
                    return Disposable.Create(()=> eventSource.OpenDesignTableNotify-= callback);
                    
                }
            );
        }
        public class CloseDesignTableNotifyEventArgs
        {
            public CloseDesignTableNotifyEventArgs (System.Object DesignTable)
            {
                this.DesignTable = DesignTable;
            }
            public System.Object DesignTable { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_CloseDesignTableNotifyEventHandler.html
        public static IObservable<CloseDesignTableNotifyEventArgs> CloseDesignTableNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<CloseDesignTableNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_CloseDesignTableNotifyEventHandler callback = 
                        (System.Object DesignTable)=>{
                            var ea = new CloseDesignTableNotifyEventArgs(DesignTable);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.CloseDesignTableNotify += callback;
                    return Disposable.Create(()=> eventSource.CloseDesignTableNotify-= callback);
                    
                }
            );
        }
        public class UnitsChangeNotifyEventArgs
        {
            public UnitsChangeNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_UnitsChangeNotifyEventHandler.html
        public static IObservable<UnitsChangeNotifyEventArgs> UnitsChangeNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<UnitsChangeNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_UnitsChangeNotifyEventHandler callback = 
                        ()=>{
                            var ea = new UnitsChangeNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.UnitsChangeNotify += callback;
                    return Disposable.Create(()=> eventSource.UnitsChangeNotify-= callback);
                    
                }
            );
        }
        public class DestroyNotify2EventArgs
        {
            public DestroyNotify2EventArgs (System.Int32 DestroyType)
            {
                this.DestroyType = DestroyType;
            }
            public System.Int32 DestroyType { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_DestroyNotify2EventHandler.html
        public static IObservable<DestroyNotify2EventArgs> DestroyNotify2Observable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<DestroyNotify2EventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_DestroyNotify2EventHandler callback = 
                        (System.Int32 DestroyType)=>{
                            var ea = new DestroyNotify2EventArgs(DestroyType);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.DestroyNotify2 += callback;
                    return Disposable.Create(()=> eventSource.DestroyNotify2-= callback);
                    
                }
            );
        }
        public class ConfigurationChangeNotifyEventArgs
        {
            public ConfigurationChangeNotifyEventArgs (System.String ConfigurationName, System.Object Object, System.Int32 ObjectType, System.Int32 changeType)
            {
                this.ConfigurationName = ConfigurationName;
                this.Object = Object;
                this.ObjectType = ObjectType;
                this.changeType = changeType;
            }
            public System.String ConfigurationName { get; }
            public System.Object Object { get; }
            public System.Int32 ObjectType { get; }
            public System.Int32 changeType { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_ConfigurationChangeNotifyEventHandler.html
        public static IObservable<ConfigurationChangeNotifyEventArgs> ConfigurationChangeNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<ConfigurationChangeNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_ConfigurationChangeNotifyEventHandler callback = 
                        (System.String ConfigurationName, System.Object Object, System.Int32 ObjectType, System.Int32 changeType)=>{
                            var ea = new ConfigurationChangeNotifyEventArgs(ConfigurationName, Object, ObjectType, changeType);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.ConfigurationChangeNotify += callback;
                    return Disposable.Create(()=> eventSource.ConfigurationChangeNotify-= callback);
                    
                }
            );
        }
        public class ComponentReorganizeNotifyEventArgs
        {
            public ComponentReorganizeNotifyEventArgs (System.String sourceName, System.String targetName)
            {
                this.sourceName = sourceName;
                this.targetName = targetName;
            }
            public System.String sourceName { get; }
            public System.String targetName { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_ComponentReorganizeNotifyEventHandler.html
        public static IObservable<ComponentReorganizeNotifyEventArgs> ComponentReorganizeNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<ComponentReorganizeNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_ComponentReorganizeNotifyEventHandler callback = 
                        (System.String sourceName, System.String targetName)=>{
                            var ea = new ComponentReorganizeNotifyEventArgs(sourceName, targetName);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.ComponentReorganizeNotify += callback;
                    return Disposable.Create(()=> eventSource.ComponentReorganizeNotify-= callback);
                    
                }
            );
        }
        public class ActiveViewChangeNotifyEventArgs
        {
            public ActiveViewChangeNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_ActiveViewChangeNotifyEventHandler.html
        public static IObservable<ActiveViewChangeNotifyEventArgs> ActiveViewChangeNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<ActiveViewChangeNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_ActiveViewChangeNotifyEventHandler callback = 
                        ()=>{
                            var ea = new ActiveViewChangeNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.ActiveViewChangeNotify += callback;
                    return Disposable.Create(()=> eventSource.ActiveViewChangeNotify-= callback);
                    
                }
            );
        }
        public class FeatureManagerFilterStringChangeNotifyEventArgs
        {
            public FeatureManagerFilterStringChangeNotifyEventArgs (System.String FilterString)
            {
                this.FilterString = FilterString;
            }
            public System.String FilterString { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_FeatureManagerFilterStringChangeNotifyEventHandler.html
        public static IObservable<FeatureManagerFilterStringChangeNotifyEventArgs> FeatureManagerFilterStringChangeNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<FeatureManagerFilterStringChangeNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_FeatureManagerFilterStringChangeNotifyEventHandler callback = 
                        (System.String FilterString)=>{
                            var ea = new FeatureManagerFilterStringChangeNotifyEventArgs(FilterString);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.FeatureManagerFilterStringChangeNotify += callback;
                    return Disposable.Create(()=> eventSource.FeatureManagerFilterStringChangeNotify-= callback);
                    
                }
            );
        }
        public class FlipLoopNotifyEventArgs
        {
            public FlipLoopNotifyEventArgs (System.Object TheLoop, System.Object TheEdge)
            {
                this.TheLoop = TheLoop;
                this.TheEdge = TheEdge;
            }
            public System.Object TheLoop { get; }
            public System.Object TheEdge { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_FlipLoopNotifyEventHandler.html
        public static IObservable<FlipLoopNotifyEventArgs> FlipLoopNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<FlipLoopNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_FlipLoopNotifyEventHandler callback = 
                        (System.Object TheLoop, System.Object TheEdge)=>{
                            var ea = new FlipLoopNotifyEventArgs(TheLoop, TheEdge);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.FlipLoopNotify += callback;
                    return Disposable.Create(()=> eventSource.FlipLoopNotify-= callback);
                    
                }
            );
        }
        public class AutoSaveNotifyEventArgs
        {
            public AutoSaveNotifyEventArgs (System.String FileName)
            {
                this.FileName = FileName;
            }
            public System.String FileName { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_AutoSaveNotifyEventHandler.html
        public static IObservable<AutoSaveNotifyEventArgs> AutoSaveNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<AutoSaveNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_AutoSaveNotifyEventHandler callback = 
                        (System.String FileName)=>{
                            var ea = new AutoSaveNotifyEventArgs(FileName);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.AutoSaveNotify += callback;
                    return Disposable.Create(()=> eventSource.AutoSaveNotify-= callback);
                    
                }
            );
        }
        public class AutoSaveToStorageNotifyEventArgs
        {
            public AutoSaveToStorageNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_AutoSaveToStorageNotifyEventHandler.html
        public static IObservable<AutoSaveToStorageNotifyEventArgs> AutoSaveToStorageNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<AutoSaveToStorageNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_AutoSaveToStorageNotifyEventHandler callback = 
                        ()=>{
                            var ea = new AutoSaveToStorageNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.AutoSaveToStorageNotify += callback;
                    return Disposable.Create(()=> eventSource.AutoSaveToStorageNotify-= callback);
                    
                }
            );
        }
        public class SensorAlertPreNotifyEventArgs
        {
            public SensorAlertPreNotifyEventArgs (System.Object SensorIn, System.Int32 SensorAlertType)
            {
                this.SensorIn = SensorIn;
                this.SensorAlertType = SensorAlertType;
            }
            public System.Object SensorIn { get; }
            public System.Int32 SensorAlertType { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_SensorAlertPreNotifyEventHandler.html
        public static IObservable<SensorAlertPreNotifyEventArgs> SensorAlertPreNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<SensorAlertPreNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_SensorAlertPreNotifyEventHandler callback = 
                        (System.Object SensorIn, System.Int32 SensorAlertType)=>{
                            var ea = new SensorAlertPreNotifyEventArgs(SensorIn, SensorAlertType);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.SensorAlertPreNotify += callback;
                    return Disposable.Create(()=> eventSource.SensorAlertPreNotify-= callback);
                    
                }
            );
        }
        public class ActiveDisplayStateChangePreNotifyEventArgs
        {
            public ActiveDisplayStateChangePreNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_ActiveDisplayStateChangePreNotifyEventHandler.html
        public static IObservable<ActiveDisplayStateChangePreNotifyEventArgs> ActiveDisplayStateChangePreNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<ActiveDisplayStateChangePreNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_ActiveDisplayStateChangePreNotifyEventHandler callback = 
                        ()=>{
                            var ea = new ActiveDisplayStateChangePreNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.ActiveDisplayStateChangePreNotify += callback;
                    return Disposable.Create(()=> eventSource.ActiveDisplayStateChangePreNotify-= callback);
                    
                }
            );
        }
        public class ActiveDisplayStateChangePostNotifyEventArgs
        {
            public ActiveDisplayStateChangePostNotifyEventArgs (System.String DisplayStateName)
            {
                this.DisplayStateName = DisplayStateName;
            }
            public System.String DisplayStateName { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_ActiveDisplayStateChangePostNotifyEventHandler.html
        public static IObservable<ActiveDisplayStateChangePostNotifyEventArgs> ActiveDisplayStateChangePostNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<ActiveDisplayStateChangePostNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_ActiveDisplayStateChangePostNotifyEventHandler callback = 
                        (System.String DisplayStateName)=>{
                            var ea = new ActiveDisplayStateChangePostNotifyEventArgs(DisplayStateName);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.ActiveDisplayStateChangePostNotify += callback;
                    return Disposable.Create(()=> eventSource.ActiveDisplayStateChangePostNotify-= callback);
                    
                }
            );
        }
        public class AddMatePostNotifyEventArgs
        {
            public AddMatePostNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_AddMatePostNotifyEventHandler.html
        public static IObservable<AddMatePostNotifyEventArgs> AddMatePostNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<AddMatePostNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_AddMatePostNotifyEventHandler callback = 
                        ()=>{
                            var ea = new AddMatePostNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.AddMatePostNotify += callback;
                    return Disposable.Create(()=> eventSource.AddMatePostNotify-= callback);
                    
                }
            );
        }
        public class ComponentConfigurationChangeNotifyEventArgs
        {
            public ComponentConfigurationChangeNotifyEventArgs (System.String componentName, System.String oldConfigurationName, System.String newConfigurationName)
            {
                this.componentName = componentName;
                this.oldConfigurationName = oldConfigurationName;
                this.newConfigurationName = newConfigurationName;
            }
            public System.String componentName { get; }
            public System.String oldConfigurationName { get; }
            public System.String newConfigurationName { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_ComponentConfigurationChangeNotifyEventHandler.html
        public static IObservable<ComponentConfigurationChangeNotifyEventArgs> ComponentConfigurationChangeNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<ComponentConfigurationChangeNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_ComponentConfigurationChangeNotifyEventHandler callback = 
                        (System.String componentName, System.String oldConfigurationName, System.String newConfigurationName)=>{
                            var ea = new ComponentConfigurationChangeNotifyEventArgs(componentName, oldConfigurationName, newConfigurationName);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.ComponentConfigurationChangeNotify += callback;
                    return Disposable.Create(()=> eventSource.ComponentConfigurationChangeNotify-= callback);
                    
                }
            );
        }
        public class UndoPostNotifyEventArgs
        {
            public UndoPostNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_UndoPostNotifyEventHandler.html
        public static IObservable<UndoPostNotifyEventArgs> UndoPostNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<UndoPostNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_UndoPostNotifyEventHandler callback = 
                        ()=>{
                            var ea = new UndoPostNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.UndoPostNotify += callback;
                    return Disposable.Create(()=> eventSource.UndoPostNotify-= callback);
                    
                }
            );
        }
        public class UserSelectionPreNotifyEventArgs
        {
            public UserSelectionPreNotifyEventArgs (System.Int32 SelType)
            {
                this.SelType = SelType;
            }
            public System.Int32 SelType { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_UserSelectionPreNotifyEventHandler.html
        public static IObservable<UserSelectionPreNotifyEventArgs> UserSelectionPreNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<UserSelectionPreNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_UserSelectionPreNotifyEventHandler callback = 
                        (System.Int32 SelType)=>{
                            var ea = new UserSelectionPreNotifyEventArgs(SelType);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.UserSelectionPreNotify += callback;
                    return Disposable.Create(()=> eventSource.UserSelectionPreNotify-= callback);
                    
                }
            );
        }
        public class RedoPostNotifyEventArgs
        {
            public RedoPostNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_RedoPostNotifyEventHandler.html
        public static IObservable<RedoPostNotifyEventArgs> RedoPostNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<RedoPostNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_RedoPostNotifyEventHandler callback = 
                        ()=>{
                            var ea = new RedoPostNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.RedoPostNotify += callback;
                    return Disposable.Create(()=> eventSource.RedoPostNotify-= callback);
                    
                }
            );
        }
        public class RedoPreNotifyEventArgs
        {
            public RedoPreNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_RedoPreNotifyEventHandler.html
        public static IObservable<RedoPreNotifyEventArgs> RedoPreNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<RedoPreNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_RedoPreNotifyEventHandler callback = 
                        ()=>{
                            var ea = new RedoPreNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.RedoPreNotify += callback;
                    return Disposable.Create(()=> eventSource.RedoPreNotify-= callback);
                    
                }
            );
        }
        public class UndoPreNotifyEventArgs
        {
            public UndoPreNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_UndoPreNotifyEventHandler.html
        public static IObservable<UndoPreNotifyEventArgs> UndoPreNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<UndoPreNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_UndoPreNotifyEventHandler callback = 
                        ()=>{
                            var ea = new UndoPreNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.UndoPreNotify += callback;
                    return Disposable.Create(()=> eventSource.UndoPreNotify-= callback);
                    
                }
            );
        }
        public class ComponentReferredDisplayStateChangeNotifyEventArgs
        {
            public ComponentReferredDisplayStateChangeNotifyEventArgs (System.Object componentModel, System.String CompName, System.Int32 oldDSId, System.String oldDSName, System.Int32 newDSId, System.String newDSName)
            {
                this.componentModel = componentModel;
                this.CompName = CompName;
                this.oldDSId = oldDSId;
                this.oldDSName = oldDSName;
                this.newDSId = newDSId;
                this.newDSName = newDSName;
            }
            public System.Object componentModel { get; }
            public System.String CompName { get; }
            public System.Int32 oldDSId { get; }
            public System.String oldDSName { get; }
            public System.Int32 newDSId { get; }
            public System.String newDSName { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_ComponentReferredDisplayStateChangeNotifyEventHandler.html
        public static IObservable<ComponentReferredDisplayStateChangeNotifyEventArgs> ComponentReferredDisplayStateChangeNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<ComponentReferredDisplayStateChangeNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_ComponentReferredDisplayStateChangeNotifyEventHandler callback = 
                        (System.Object componentModel, System.String CompName, System.Int32 oldDSId, System.String oldDSName, System.Int32 newDSId, System.String newDSName)=>{
                            var ea = new ComponentReferredDisplayStateChangeNotifyEventArgs(componentModel, CompName, oldDSId, oldDSName, newDSId, newDSName);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.ComponentReferredDisplayStateChangeNotify += callback;
                    return Disposable.Create(()=> eventSource.ComponentReferredDisplayStateChangeNotify-= callback);
                    
                }
            );
        }
        public class RegenPostNotify2EventArgs
        {
            public RegenPostNotify2EventArgs (System.Object stopFeature)
            {
                this.stopFeature = stopFeature;
            }
            public System.Object stopFeature { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_RegenPostNotify2EventHandler.html
        public static IObservable<RegenPostNotify2EventArgs> RegenPostNotify2Observable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<RegenPostNotify2EventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_RegenPostNotify2EventHandler callback = 
                        (System.Object stopFeature)=>{
                            var ea = new RegenPostNotify2EventArgs(stopFeature);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.RegenPostNotify2 += callback;
                    return Disposable.Create(()=> eventSource.RegenPostNotify2-= callback);
                    
                }
            );
        }
        public class AutoSaveToStorageStoreNotifyEventArgs
        {
            public AutoSaveToStorageStoreNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_AutoSaveToStorageStoreNotifyEventHandler.html
        public static IObservable<AutoSaveToStorageStoreNotifyEventArgs> AutoSaveToStorageStoreNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<AutoSaveToStorageStoreNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_AutoSaveToStorageStoreNotifyEventHandler callback = 
                        ()=>{
                            var ea = new AutoSaveToStorageStoreNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.AutoSaveToStorageStoreNotify += callback;
                    return Disposable.Create(()=> eventSource.AutoSaveToStorageStoreNotify-= callback);
                    
                }
            );
        }
        public class DragStateChangeNotifyEventArgs
        {
            public DragStateChangeNotifyEventArgs (System.Boolean State)
            {
                this.State = State;
            }
            public System.Boolean State { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_DragStateChangeNotifyEventHandler.html
        public static IObservable<DragStateChangeNotifyEventArgs> DragStateChangeNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<DragStateChangeNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_DragStateChangeNotifyEventHandler callback = 
                        (System.Boolean State)=>{
                            var ea = new DragStateChangeNotifyEventArgs(State);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.DragStateChangeNotify += callback;
                    return Disposable.Create(()=> eventSource.DragStateChangeNotify-= callback);
                    
                }
            );
        }
        public class InsertTableNotifyEventArgs
        {
            public InsertTableNotifyEventArgs (SolidWorks.Interop.sldworks.TableAnnotation TableAnnotation, System.String TableType, System.String TemplatePath)
            {
                this.TableAnnotation = TableAnnotation;
                this.TableType = TableType;
                this.TemplatePath = TemplatePath;
            }
            public SolidWorks.Interop.sldworks.TableAnnotation TableAnnotation { get; }
            public System.String TableType { get; }
            public System.String TemplatePath { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_InsertTableNotifyEventHandler.html
        public static IObservable<InsertTableNotifyEventArgs> InsertTableNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<InsertTableNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_InsertTableNotifyEventHandler callback = 
                        (SolidWorks.Interop.sldworks.TableAnnotation TableAnnotation, System.String TableType, System.String TemplatePath)=>{
                            var ea = new InsertTableNotifyEventArgs(TableAnnotation, TableType, TemplatePath);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.InsertTableNotify += callback;
                    return Disposable.Create(()=> eventSource.InsertTableNotify-= callback);
                    
                }
            );
        }
        public class ModifyTableNotifyEventArgs
        {
            public ModifyTableNotifyEventArgs (SolidWorks.Interop.sldworks.TableAnnotation TableAnnotation, System.Int32 TableType, System.Int32 reason, System.Int32 RowInfo, System.Int32 ColumnInfo, System.String DataInfo)
            {
                this.TableAnnotation = TableAnnotation;
                this.TableType = TableType;
                this.reason = reason;
                this.RowInfo = RowInfo;
                this.ColumnInfo = ColumnInfo;
                this.DataInfo = DataInfo;
            }
            public SolidWorks.Interop.sldworks.TableAnnotation TableAnnotation { get; }
            public System.Int32 TableType { get; }
            public System.Int32 reason { get; }
            public System.Int32 RowInfo { get; }
            public System.Int32 ColumnInfo { get; }
            public System.String DataInfo { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_ModifyTableNotifyEventHandler.html
        public static IObservable<ModifyTableNotifyEventArgs> ModifyTableNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<ModifyTableNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_ModifyTableNotifyEventHandler callback = 
                        (SolidWorks.Interop.sldworks.TableAnnotation TableAnnotation, System.Int32 TableType, System.Int32 reason, System.Int32 RowInfo, System.Int32 ColumnInfo, System.String DataInfo)=>{
                            var ea = new ModifyTableNotifyEventArgs(TableAnnotation, TableType, reason, RowInfo, ColumnInfo, DataInfo);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.ModifyTableNotify += callback;
                    return Disposable.Create(()=> eventSource.ModifyTableNotify-= callback);
                    
                }
            );
        }
        public class UserSelectionPostNotifyEventArgs
        {
            public UserSelectionPostNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_UserSelectionPostNotifyEventHandler.html
        public static IObservable<UserSelectionPostNotifyEventArgs> UserSelectionPostNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<UserSelectionPostNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_UserSelectionPostNotifyEventHandler callback = 
                        ()=>{
                            var ea = new UserSelectionPostNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.UserSelectionPostNotify += callback;
                    return Disposable.Create(()=> eventSource.UserSelectionPostNotify-= callback);
                    
                }
            );
        }
        public class ComponentDisplayModeChangePreNotifyEventArgs
        {
            public ComponentDisplayModeChangePreNotifyEventArgs (System.Object Component)
            {
                this.Component = Component;
            }
            public System.Object Component { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_ComponentDisplayModeChangePreNotifyEventHandler.html
        public static IObservable<ComponentDisplayModeChangePreNotifyEventArgs> ComponentDisplayModeChangePreNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<ComponentDisplayModeChangePreNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_ComponentDisplayModeChangePreNotifyEventHandler callback = 
                        (System.Object Component)=>{
                            var ea = new ComponentDisplayModeChangePreNotifyEventArgs(Component);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.ComponentDisplayModeChangePreNotify += callback;
                    return Disposable.Create(()=> eventSource.ComponentDisplayModeChangePreNotify-= callback);
                    
                }
            );
        }
        public class ComponentDisplayModeChangePostNotifyEventArgs
        {
            public ComponentDisplayModeChangePostNotifyEventArgs (System.Object Component)
            {
                this.Component = Component;
            }
            public System.Object Component { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_ComponentDisplayModeChangePostNotifyEventHandler.html
        public static IObservable<ComponentDisplayModeChangePostNotifyEventArgs> ComponentDisplayModeChangePostNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<ComponentDisplayModeChangePostNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_ComponentDisplayModeChangePostNotifyEventHandler callback = 
                        (System.Object Component)=>{
                            var ea = new ComponentDisplayModeChangePostNotifyEventArgs(Component);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.ComponentDisplayModeChangePostNotify += callback;
                    return Disposable.Create(()=> eventSource.ComponentDisplayModeChangePostNotify-= callback);
                    
                }
            );
        }
        public class CommandManagerTabActivatedPreNotifyEventArgs
        {
            public CommandManagerTabActivatedPreNotifyEventArgs (System.Int32 CommandTabIndex, System.String CommandTabName)
            {
                this.CommandTabIndex = CommandTabIndex;
                this.CommandTabName = CommandTabName;
            }
            public System.Int32 CommandTabIndex { get; }
            public System.String CommandTabName { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_CommandManagerTabActivatedPreNotifyEventHandler.html
        public static IObservable<CommandManagerTabActivatedPreNotifyEventArgs> CommandManagerTabActivatedPreNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<CommandManagerTabActivatedPreNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_CommandManagerTabActivatedPreNotifyEventHandler callback = 
                        (System.Int32 CommandTabIndex, System.String CommandTabName)=>{
                            var ea = new CommandManagerTabActivatedPreNotifyEventArgs(CommandTabIndex, CommandTabName);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.CommandManagerTabActivatedPreNotify += callback;
                    return Disposable.Create(()=> eventSource.CommandManagerTabActivatedPreNotify-= callback);
                    
                }
            );
        }
        public class PreRenameItemNotifyEventArgs
        {
            public PreRenameItemNotifyEventArgs (System.Int32 EntityType, System.String oldName, System.String NewName)
            {
                this.EntityType = EntityType;
                this.oldName = oldName;
                this.NewName = NewName;
            }
            public System.Int32 EntityType { get; }
            public System.String oldName { get; }
            public System.String NewName { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DAssemblyDocEvents_PreRenameItemNotifyEventHandler.html
        public static IObservable<PreRenameItemNotifyEventArgs> PreRenameItemNotifyObservable(this SolidWorks.Interop.sldworks.DAssemblyDocEvents_Event eventSource)
        {
            return Observable.Create<PreRenameItemNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DAssemblyDocEvents_PreRenameItemNotifyEventHandler callback = 
                        (System.Int32 EntityType, System.String oldName, System.String NewName)=>{
                            var ea = new PreRenameItemNotifyEventArgs(EntityType, oldName, NewName);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.PreRenameItemNotify += callback;
                    return Disposable.Create(()=> eventSource.PreRenameItemNotify-= callback);
                    
                }
            );
        }
    }
}



namespace SolidworksAddinFramework.Events {
    public static class DDrawingDocEvents_Event {

        public class RegenNotifyEventArgs
        {
            public RegenNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_RegenNotifyEventHandler.html
        public static IObservable<RegenNotifyEventArgs> RegenNotifyObservable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<RegenNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_RegenNotifyEventHandler callback = 
                        ()=>{
                            var ea = new RegenNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.RegenNotify += callback;
                    return Disposable.Create(()=> eventSource.RegenNotify-= callback);
                    
                }
            );
        }
        public class DestroyNotifyEventArgs
        {
            public DestroyNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_DestroyNotifyEventHandler.html
        public static IObservable<DestroyNotifyEventArgs> DestroyNotifyObservable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<DestroyNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_DestroyNotifyEventHandler callback = 
                        ()=>{
                            var ea = new DestroyNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.DestroyNotify += callback;
                    return Disposable.Create(()=> eventSource.DestroyNotify-= callback);
                    
                }
            );
        }
        public class RegenPostNotifyEventArgs
        {
            public RegenPostNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_RegenPostNotifyEventHandler.html
        public static IObservable<RegenPostNotifyEventArgs> RegenPostNotifyObservable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<RegenPostNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_RegenPostNotifyEventHandler callback = 
                        ()=>{
                            var ea = new RegenPostNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.RegenPostNotify += callback;
                    return Disposable.Create(()=> eventSource.RegenPostNotify-= callback);
                    
                }
            );
        }
        public class ViewNewNotifyEventArgs
        {
            public ViewNewNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_ViewNewNotifyEventHandler.html
        public static IObservable<ViewNewNotifyEventArgs> ViewNewNotifyObservable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<ViewNewNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_ViewNewNotifyEventHandler callback = 
                        ()=>{
                            var ea = new ViewNewNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.ViewNewNotify += callback;
                    return Disposable.Create(()=> eventSource.ViewNewNotify-= callback);
                    
                }
            );
        }
        public class NewSelectionNotifyEventArgs
        {
            public NewSelectionNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_NewSelectionNotifyEventHandler.html
        public static IObservable<NewSelectionNotifyEventArgs> NewSelectionNotifyObservable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<NewSelectionNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_NewSelectionNotifyEventHandler callback = 
                        ()=>{
                            var ea = new NewSelectionNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.NewSelectionNotify += callback;
                    return Disposable.Create(()=> eventSource.NewSelectionNotify-= callback);
                    
                }
            );
        }
        public class FileSaveNotifyEventArgs
        {
            public FileSaveNotifyEventArgs (System.String FileName)
            {
                this.FileName = FileName;
            }
            public System.String FileName { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_FileSaveNotifyEventHandler.html
        public static IObservable<FileSaveNotifyEventArgs> FileSaveNotifyObservable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<FileSaveNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_FileSaveNotifyEventHandler callback = 
                        (System.String FileName)=>{
                            var ea = new FileSaveNotifyEventArgs(FileName);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.FileSaveNotify += callback;
                    return Disposable.Create(()=> eventSource.FileSaveNotify-= callback);
                    
                }
            );
        }
        public class FileSaveAsNotifyEventArgs
        {
            public FileSaveAsNotifyEventArgs (System.String FileName)
            {
                this.FileName = FileName;
            }
            public System.String FileName { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_FileSaveAsNotifyEventHandler.html
        public static IObservable<FileSaveAsNotifyEventArgs> FileSaveAsNotifyObservable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<FileSaveAsNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_FileSaveAsNotifyEventHandler callback = 
                        (System.String FileName)=>{
                            var ea = new FileSaveAsNotifyEventArgs(FileName);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.FileSaveAsNotify += callback;
                    return Disposable.Create(()=> eventSource.FileSaveAsNotify-= callback);
                    
                }
            );
        }
        public class LoadFromStorageNotifyEventArgs
        {
            public LoadFromStorageNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_LoadFromStorageNotifyEventHandler.html
        public static IObservable<LoadFromStorageNotifyEventArgs> LoadFromStorageNotifyObservable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<LoadFromStorageNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_LoadFromStorageNotifyEventHandler callback = 
                        ()=>{
                            var ea = new LoadFromStorageNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.LoadFromStorageNotify += callback;
                    return Disposable.Create(()=> eventSource.LoadFromStorageNotify-= callback);
                    
                }
            );
        }
        public class SaveToStorageNotifyEventArgs
        {
            public SaveToStorageNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_SaveToStorageNotifyEventHandler.html
        public static IObservable<SaveToStorageNotifyEventArgs> SaveToStorageNotifyObservable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<SaveToStorageNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_SaveToStorageNotifyEventHandler callback = 
                        ()=>{
                            var ea = new SaveToStorageNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.SaveToStorageNotify += callback;
                    return Disposable.Create(()=> eventSource.SaveToStorageNotify-= callback);
                    
                }
            );
        }
        public class ActiveConfigChangeNotifyEventArgs
        {
            public ActiveConfigChangeNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_ActiveConfigChangeNotifyEventHandler.html
        public static IObservable<ActiveConfigChangeNotifyEventArgs> ActiveConfigChangeNotifyObservable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<ActiveConfigChangeNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_ActiveConfigChangeNotifyEventHandler callback = 
                        ()=>{
                            var ea = new ActiveConfigChangeNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.ActiveConfigChangeNotify += callback;
                    return Disposable.Create(()=> eventSource.ActiveConfigChangeNotify-= callback);
                    
                }
            );
        }
        public class ActiveConfigChangePostNotifyEventArgs
        {
            public ActiveConfigChangePostNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_ActiveConfigChangePostNotifyEventHandler.html
        public static IObservable<ActiveConfigChangePostNotifyEventArgs> ActiveConfigChangePostNotifyObservable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<ActiveConfigChangePostNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_ActiveConfigChangePostNotifyEventHandler callback = 
                        ()=>{
                            var ea = new ActiveConfigChangePostNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.ActiveConfigChangePostNotify += callback;
                    return Disposable.Create(()=> eventSource.ActiveConfigChangePostNotify-= callback);
                    
                }
            );
        }
        public class ViewNewNotify2EventArgs
        {
            public ViewNewNotify2EventArgs (System.Object viewBeingAdded)
            {
                this.viewBeingAdded = viewBeingAdded;
            }
            public System.Object viewBeingAdded { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_ViewNewNotify2EventHandler.html
        public static IObservable<ViewNewNotify2EventArgs> ViewNewNotify2Observable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<ViewNewNotify2EventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_ViewNewNotify2EventHandler callback = 
                        (System.Object viewBeingAdded)=>{
                            var ea = new ViewNewNotify2EventArgs(viewBeingAdded);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.ViewNewNotify2 += callback;
                    return Disposable.Create(()=> eventSource.ViewNewNotify2-= callback);
                    
                }
            );
        }
        public class AddItemNotifyEventArgs
        {
            public AddItemNotifyEventArgs (System.Int32 EntityType, System.String itemName)
            {
                this.EntityType = EntityType;
                this.itemName = itemName;
            }
            public System.Int32 EntityType { get; }
            public System.String itemName { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_AddItemNotifyEventHandler.html
        public static IObservable<AddItemNotifyEventArgs> AddItemNotifyObservable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<AddItemNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_AddItemNotifyEventHandler callback = 
                        (System.Int32 EntityType, System.String itemName)=>{
                            var ea = new AddItemNotifyEventArgs(EntityType, itemName);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.AddItemNotify += callback;
                    return Disposable.Create(()=> eventSource.AddItemNotify-= callback);
                    
                }
            );
        }
        public class RenameItemNotifyEventArgs
        {
            public RenameItemNotifyEventArgs (System.Int32 EntityType, System.String oldName, System.String NewName)
            {
                this.EntityType = EntityType;
                this.oldName = oldName;
                this.NewName = NewName;
            }
            public System.Int32 EntityType { get; }
            public System.String oldName { get; }
            public System.String NewName { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_RenameItemNotifyEventHandler.html
        public static IObservable<RenameItemNotifyEventArgs> RenameItemNotifyObservable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<RenameItemNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_RenameItemNotifyEventHandler callback = 
                        (System.Int32 EntityType, System.String oldName, System.String NewName)=>{
                            var ea = new RenameItemNotifyEventArgs(EntityType, oldName, NewName);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.RenameItemNotify += callback;
                    return Disposable.Create(()=> eventSource.RenameItemNotify-= callback);
                    
                }
            );
        }
        public class DeleteItemNotifyEventArgs
        {
            public DeleteItemNotifyEventArgs (System.Int32 EntityType, System.String itemName)
            {
                this.EntityType = EntityType;
                this.itemName = itemName;
            }
            public System.Int32 EntityType { get; }
            public System.String itemName { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_DeleteItemNotifyEventHandler.html
        public static IObservable<DeleteItemNotifyEventArgs> DeleteItemNotifyObservable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<DeleteItemNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_DeleteItemNotifyEventHandler callback = 
                        (System.Int32 EntityType, System.String itemName)=>{
                            var ea = new DeleteItemNotifyEventArgs(EntityType, itemName);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.DeleteItemNotify += callback;
                    return Disposable.Create(()=> eventSource.DeleteItemNotify-= callback);
                    
                }
            );
        }
        public class ModifyNotifyEventArgs
        {
            public ModifyNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_ModifyNotifyEventHandler.html
        public static IObservable<ModifyNotifyEventArgs> ModifyNotifyObservable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<ModifyNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_ModifyNotifyEventHandler callback = 
                        ()=>{
                            var ea = new ModifyNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.ModifyNotify += callback;
                    return Disposable.Create(()=> eventSource.ModifyNotify-= callback);
                    
                }
            );
        }
        public class FileReloadNotifyEventArgs
        {
            public FileReloadNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_FileReloadNotifyEventHandler.html
        public static IObservable<FileReloadNotifyEventArgs> FileReloadNotifyObservable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<FileReloadNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_FileReloadNotifyEventHandler callback = 
                        ()=>{
                            var ea = new FileReloadNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.FileReloadNotify += callback;
                    return Disposable.Create(()=> eventSource.FileReloadNotify-= callback);
                    
                }
            );
        }
        public class AddCustomPropertyNotifyEventArgs
        {
            public AddCustomPropertyNotifyEventArgs (System.String propName, System.String Configuration, System.String Value, System.Int32 valueType)
            {
                this.propName = propName;
                this.Configuration = Configuration;
                this.Value = Value;
                this.valueType = valueType;
            }
            public System.String propName { get; }
            public System.String Configuration { get; }
            public System.String Value { get; }
            public System.Int32 valueType { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_AddCustomPropertyNotifyEventHandler.html
        public static IObservable<AddCustomPropertyNotifyEventArgs> AddCustomPropertyNotifyObservable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<AddCustomPropertyNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_AddCustomPropertyNotifyEventHandler callback = 
                        (System.String propName, System.String Configuration, System.String Value, System.Int32 valueType)=>{
                            var ea = new AddCustomPropertyNotifyEventArgs(propName, Configuration, Value, valueType);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.AddCustomPropertyNotify += callback;
                    return Disposable.Create(()=> eventSource.AddCustomPropertyNotify-= callback);
                    
                }
            );
        }
        public class ChangeCustomPropertyNotifyEventArgs
        {
            public ChangeCustomPropertyNotifyEventArgs (System.String propName, System.String Configuration, System.String oldValue, System.String NewValue, System.Int32 valueType)
            {
                this.propName = propName;
                this.Configuration = Configuration;
                this.oldValue = oldValue;
                this.NewValue = NewValue;
                this.valueType = valueType;
            }
            public System.String propName { get; }
            public System.String Configuration { get; }
            public System.String oldValue { get; }
            public System.String NewValue { get; }
            public System.Int32 valueType { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_ChangeCustomPropertyNotifyEventHandler.html
        public static IObservable<ChangeCustomPropertyNotifyEventArgs> ChangeCustomPropertyNotifyObservable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<ChangeCustomPropertyNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_ChangeCustomPropertyNotifyEventHandler callback = 
                        (System.String propName, System.String Configuration, System.String oldValue, System.String NewValue, System.Int32 valueType)=>{
                            var ea = new ChangeCustomPropertyNotifyEventArgs(propName, Configuration, oldValue, NewValue, valueType);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.ChangeCustomPropertyNotify += callback;
                    return Disposable.Create(()=> eventSource.ChangeCustomPropertyNotify-= callback);
                    
                }
            );
        }
        public class DeleteCustomPropertyNotifyEventArgs
        {
            public DeleteCustomPropertyNotifyEventArgs (System.String propName, System.String Configuration, System.String Value, System.Int32 valueType)
            {
                this.propName = propName;
                this.Configuration = Configuration;
                this.Value = Value;
                this.valueType = valueType;
            }
            public System.String propName { get; }
            public System.String Configuration { get; }
            public System.String Value { get; }
            public System.Int32 valueType { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_DeleteCustomPropertyNotifyEventHandler.html
        public static IObservable<DeleteCustomPropertyNotifyEventArgs> DeleteCustomPropertyNotifyObservable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<DeleteCustomPropertyNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_DeleteCustomPropertyNotifyEventHandler callback = 
                        (System.String propName, System.String Configuration, System.String Value, System.Int32 valueType)=>{
                            var ea = new DeleteCustomPropertyNotifyEventArgs(propName, Configuration, Value, valueType);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.DeleteCustomPropertyNotify += callback;
                    return Disposable.Create(()=> eventSource.DeleteCustomPropertyNotify-= callback);
                    
                }
            );
        }
        public class FileSaveAsNotify2EventArgs
        {
            public FileSaveAsNotify2EventArgs (System.String FileName)
            {
                this.FileName = FileName;
            }
            public System.String FileName { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_FileSaveAsNotify2EventHandler.html
        public static IObservable<FileSaveAsNotify2EventArgs> FileSaveAsNotify2Observable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<FileSaveAsNotify2EventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_FileSaveAsNotify2EventHandler callback = 
                        (System.String FileName)=>{
                            var ea = new FileSaveAsNotify2EventArgs(FileName);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.FileSaveAsNotify2 += callback;
                    return Disposable.Create(()=> eventSource.FileSaveAsNotify2-= callback);
                    
                }
            );
        }
        public class DeleteSelectionPreNotifyEventArgs
        {
            public DeleteSelectionPreNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_DeleteSelectionPreNotifyEventHandler.html
        public static IObservable<DeleteSelectionPreNotifyEventArgs> DeleteSelectionPreNotifyObservable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<DeleteSelectionPreNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_DeleteSelectionPreNotifyEventHandler callback = 
                        ()=>{
                            var ea = new DeleteSelectionPreNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.DeleteSelectionPreNotify += callback;
                    return Disposable.Create(()=> eventSource.DeleteSelectionPreNotify-= callback);
                    
                }
            );
        }
        public class FileReloadPreNotifyEventArgs
        {
            public FileReloadPreNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_FileReloadPreNotifyEventHandler.html
        public static IObservable<FileReloadPreNotifyEventArgs> FileReloadPreNotifyObservable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<FileReloadPreNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_FileReloadPreNotifyEventHandler callback = 
                        ()=>{
                            var ea = new FileReloadPreNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.FileReloadPreNotify += callback;
                    return Disposable.Create(()=> eventSource.FileReloadPreNotify-= callback);
                    
                }
            );
        }
        public class FileSavePostNotifyEventArgs
        {
            public FileSavePostNotifyEventArgs (System.Int32 saveType, System.String FileName)
            {
                this.saveType = saveType;
                this.FileName = FileName;
            }
            public System.Int32 saveType { get; }
            public System.String FileName { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_FileSavePostNotifyEventHandler.html
        public static IObservable<FileSavePostNotifyEventArgs> FileSavePostNotifyObservable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<FileSavePostNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_FileSavePostNotifyEventHandler callback = 
                        (System.Int32 saveType, System.String FileName)=>{
                            var ea = new FileSavePostNotifyEventArgs(saveType, FileName);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.FileSavePostNotify += callback;
                    return Disposable.Create(()=> eventSource.FileSavePostNotify-= callback);
                    
                }
            );
        }
        public class LoadFromStorageStoreNotifyEventArgs
        {
            public LoadFromStorageStoreNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_LoadFromStorageStoreNotifyEventHandler.html
        public static IObservable<LoadFromStorageStoreNotifyEventArgs> LoadFromStorageStoreNotifyObservable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<LoadFromStorageStoreNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_LoadFromStorageStoreNotifyEventHandler callback = 
                        ()=>{
                            var ea = new LoadFromStorageStoreNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.LoadFromStorageStoreNotify += callback;
                    return Disposable.Create(()=> eventSource.LoadFromStorageStoreNotify-= callback);
                    
                }
            );
        }
        public class SaveToStorageStoreNotifyEventArgs
        {
            public SaveToStorageStoreNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_SaveToStorageStoreNotifyEventHandler.html
        public static IObservable<SaveToStorageStoreNotifyEventArgs> SaveToStorageStoreNotifyObservable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<SaveToStorageStoreNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_SaveToStorageStoreNotifyEventHandler callback = 
                        ()=>{
                            var ea = new SaveToStorageStoreNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.SaveToStorageStoreNotify += callback;
                    return Disposable.Create(()=> eventSource.SaveToStorageStoreNotify-= callback);
                    
                }
            );
        }
        public class FeatureManagerTreeRebuildNotifyEventArgs
        {
            public FeatureManagerTreeRebuildNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_FeatureManagerTreeRebuildNotifyEventHandler.html
        public static IObservable<FeatureManagerTreeRebuildNotifyEventArgs> FeatureManagerTreeRebuildNotifyObservable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<FeatureManagerTreeRebuildNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_FeatureManagerTreeRebuildNotifyEventHandler callback = 
                        ()=>{
                            var ea = new FeatureManagerTreeRebuildNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.FeatureManagerTreeRebuildNotify += callback;
                    return Disposable.Create(()=> eventSource.FeatureManagerTreeRebuildNotify-= callback);
                    
                }
            );
        }
        public class ViewCreatePreNotifyEventArgs
        {
            public ViewCreatePreNotifyEventArgs (System.Object modelDocBeingAdded)
            {
                this.modelDocBeingAdded = modelDocBeingAdded;
            }
            public System.Object modelDocBeingAdded { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_ViewCreatePreNotifyEventHandler.html
        public static IObservable<ViewCreatePreNotifyEventArgs> ViewCreatePreNotifyObservable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<ViewCreatePreNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_ViewCreatePreNotifyEventHandler callback = 
                        (System.Object modelDocBeingAdded)=>{
                            var ea = new ViewCreatePreNotifyEventArgs(modelDocBeingAdded);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.ViewCreatePreNotify += callback;
                    return Disposable.Create(()=> eventSource.ViewCreatePreNotify-= callback);
                    
                }
            );
        }
        public class DynamicHighlightNotifyEventArgs
        {
            public DynamicHighlightNotifyEventArgs (System.Boolean bHighlightState)
            {
                this.bHighlightState = bHighlightState;
            }
            public System.Boolean bHighlightState { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_DynamicHighlightNotifyEventHandler.html
        public static IObservable<DynamicHighlightNotifyEventArgs> DynamicHighlightNotifyObservable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<DynamicHighlightNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_DynamicHighlightNotifyEventHandler callback = 
                        (System.Boolean bHighlightState)=>{
                            var ea = new DynamicHighlightNotifyEventArgs(bHighlightState);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.DynamicHighlightNotify += callback;
                    return Disposable.Create(()=> eventSource.DynamicHighlightNotify-= callback);
                    
                }
            );
        }
        public class DimensionChangeNotifyEventArgs
        {
            public DimensionChangeNotifyEventArgs (System.Object displayDim)
            {
                this.displayDim = displayDim;
            }
            public System.Object displayDim { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_DimensionChangeNotifyEventHandler.html
        public static IObservable<DimensionChangeNotifyEventArgs> DimensionChangeNotifyObservable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<DimensionChangeNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_DimensionChangeNotifyEventHandler callback = 
                        (System.Object displayDim)=>{
                            var ea = new DimensionChangeNotifyEventArgs(displayDim);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.DimensionChangeNotify += callback;
                    return Disposable.Create(()=> eventSource.DimensionChangeNotify-= callback);
                    
                }
            );
        }
        public class FileReloadCancelNotifyEventArgs
        {
            public FileReloadCancelNotifyEventArgs (System.Int32 ErrorCode)
            {
                this.ErrorCode = ErrorCode;
            }
            public System.Int32 ErrorCode { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_FileReloadCancelNotifyEventHandler.html
        public static IObservable<FileReloadCancelNotifyEventArgs> FileReloadCancelNotifyObservable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<FileReloadCancelNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_FileReloadCancelNotifyEventHandler callback = 
                        (System.Int32 ErrorCode)=>{
                            var ea = new FileReloadCancelNotifyEventArgs(ErrorCode);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.FileReloadCancelNotify += callback;
                    return Disposable.Create(()=> eventSource.FileReloadCancelNotify-= callback);
                    
                }
            );
        }
        public class FileSavePostCancelNotifyEventArgs
        {
            public FileSavePostCancelNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_FileSavePostCancelNotifyEventHandler.html
        public static IObservable<FileSavePostCancelNotifyEventArgs> FileSavePostCancelNotifyObservable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<FileSavePostCancelNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_FileSavePostCancelNotifyEventHandler callback = 
                        ()=>{
                            var ea = new FileSavePostCancelNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.FileSavePostCancelNotify += callback;
                    return Disposable.Create(()=> eventSource.FileSavePostCancelNotify-= callback);
                    
                }
            );
        }
        public class SketchSolveNotifyEventArgs
        {
            public SketchSolveNotifyEventArgs (System.String featName)
            {
                this.featName = featName;
            }
            public System.String featName { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_SketchSolveNotifyEventHandler.html
        public static IObservable<SketchSolveNotifyEventArgs> SketchSolveNotifyObservable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<SketchSolveNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_SketchSolveNotifyEventHandler callback = 
                        (System.String featName)=>{
                            var ea = new SketchSolveNotifyEventArgs(featName);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.SketchSolveNotify += callback;
                    return Disposable.Create(()=> eventSource.SketchSolveNotify-= callback);
                    
                }
            );
        }
        public class DeleteItemPreNotifyEventArgs
        {
            public DeleteItemPreNotifyEventArgs (System.Int32 EntityType, System.String itemName)
            {
                this.EntityType = EntityType;
                this.itemName = itemName;
            }
            public System.Int32 EntityType { get; }
            public System.String itemName { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_DeleteItemPreNotifyEventHandler.html
        public static IObservable<DeleteItemPreNotifyEventArgs> DeleteItemPreNotifyObservable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<DeleteItemPreNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_DeleteItemPreNotifyEventHandler callback = 
                        (System.Int32 EntityType, System.String itemName)=>{
                            var ea = new DeleteItemPreNotifyEventArgs(EntityType, itemName);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.DeleteItemPreNotify += callback;
                    return Disposable.Create(()=> eventSource.DeleteItemPreNotify-= callback);
                    
                }
            );
        }
        public class ClearSelectionsNotifyEventArgs
        {
            public ClearSelectionsNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_ClearSelectionsNotifyEventHandler.html
        public static IObservable<ClearSelectionsNotifyEventArgs> ClearSelectionsNotifyObservable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<ClearSelectionsNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_ClearSelectionsNotifyEventHandler callback = 
                        ()=>{
                            var ea = new ClearSelectionsNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.ClearSelectionsNotify += callback;
                    return Disposable.Create(()=> eventSource.ClearSelectionsNotify-= callback);
                    
                }
            );
        }
        public class EquationEditorPreNotifyEventArgs
        {
            public EquationEditorPreNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_EquationEditorPreNotifyEventHandler.html
        public static IObservable<EquationEditorPreNotifyEventArgs> EquationEditorPreNotifyObservable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<EquationEditorPreNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_EquationEditorPreNotifyEventHandler callback = 
                        ()=>{
                            var ea = new EquationEditorPreNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.EquationEditorPreNotify += callback;
                    return Disposable.Create(()=> eventSource.EquationEditorPreNotify-= callback);
                    
                }
            );
        }
        public class EquationEditorPostNotifyEventArgs
        {
            public EquationEditorPostNotifyEventArgs (System.Boolean Changed)
            {
                this.Changed = Changed;
            }
            public System.Boolean Changed { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_EquationEditorPostNotifyEventHandler.html
        public static IObservable<EquationEditorPostNotifyEventArgs> EquationEditorPostNotifyObservable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<EquationEditorPostNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_EquationEditorPostNotifyEventHandler callback = 
                        (System.Boolean Changed)=>{
                            var ea = new EquationEditorPostNotifyEventArgs(Changed);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.EquationEditorPostNotify += callback;
                    return Disposable.Create(()=> eventSource.EquationEditorPostNotify-= callback);
                    
                }
            );
        }
        public class UnitsChangeNotifyEventArgs
        {
            public UnitsChangeNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_UnitsChangeNotifyEventHandler.html
        public static IObservable<UnitsChangeNotifyEventArgs> UnitsChangeNotifyObservable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<UnitsChangeNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_UnitsChangeNotifyEventHandler callback = 
                        ()=>{
                            var ea = new UnitsChangeNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.UnitsChangeNotify += callback;
                    return Disposable.Create(()=> eventSource.UnitsChangeNotify-= callback);
                    
                }
            );
        }
        public class DestroyNotify2EventArgs
        {
            public DestroyNotify2EventArgs (System.Int32 DestroyType)
            {
                this.DestroyType = DestroyType;
            }
            public System.Int32 DestroyType { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_DestroyNotify2EventHandler.html
        public static IObservable<DestroyNotify2EventArgs> DestroyNotify2Observable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<DestroyNotify2EventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_DestroyNotify2EventHandler callback = 
                        (System.Int32 DestroyType)=>{
                            var ea = new DestroyNotify2EventArgs(DestroyType);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.DestroyNotify2 += callback;
                    return Disposable.Create(()=> eventSource.DestroyNotify2-= callback);
                    
                }
            );
        }
        public class AutoSaveNotifyEventArgs
        {
            public AutoSaveNotifyEventArgs (System.String FileName)
            {
                this.FileName = FileName;
            }
            public System.String FileName { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_AutoSaveNotifyEventHandler.html
        public static IObservable<AutoSaveNotifyEventArgs> AutoSaveNotifyObservable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<AutoSaveNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_AutoSaveNotifyEventHandler callback = 
                        (System.String FileName)=>{
                            var ea = new AutoSaveNotifyEventArgs(FileName);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.AutoSaveNotify += callback;
                    return Disposable.Create(()=> eventSource.AutoSaveNotify-= callback);
                    
                }
            );
        }
        public class AutoSaveToStorageNotifyEventArgs
        {
            public AutoSaveToStorageNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_AutoSaveToStorageNotifyEventHandler.html
        public static IObservable<AutoSaveToStorageNotifyEventArgs> AutoSaveToStorageNotifyObservable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<AutoSaveToStorageNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_AutoSaveToStorageNotifyEventHandler callback = 
                        ()=>{
                            var ea = new AutoSaveToStorageNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.AutoSaveToStorageNotify += callback;
                    return Disposable.Create(()=> eventSource.AutoSaveToStorageNotify-= callback);
                    
                }
            );
        }
        public class UndoPostNotifyEventArgs
        {
            public UndoPostNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_UndoPostNotifyEventHandler.html
        public static IObservable<UndoPostNotifyEventArgs> UndoPostNotifyObservable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<UndoPostNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_UndoPostNotifyEventHandler callback = 
                        ()=>{
                            var ea = new UndoPostNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.UndoPostNotify += callback;
                    return Disposable.Create(()=> eventSource.UndoPostNotify-= callback);
                    
                }
            );
        }
        public class UserSelectionPreNotifyEventArgs
        {
            public UserSelectionPreNotifyEventArgs (System.Int32 SelType)
            {
                this.SelType = SelType;
            }
            public System.Int32 SelType { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_UserSelectionPreNotifyEventHandler.html
        public static IObservable<UserSelectionPreNotifyEventArgs> UserSelectionPreNotifyObservable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<UserSelectionPreNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_UserSelectionPreNotifyEventHandler callback = 
                        (System.Int32 SelType)=>{
                            var ea = new UserSelectionPreNotifyEventArgs(SelType);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.UserSelectionPreNotify += callback;
                    return Disposable.Create(()=> eventSource.UserSelectionPreNotify-= callback);
                    
                }
            );
        }
        public class RedoPostNotifyEventArgs
        {
            public RedoPostNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_RedoPostNotifyEventHandler.html
        public static IObservable<RedoPostNotifyEventArgs> RedoPostNotifyObservable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<RedoPostNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_RedoPostNotifyEventHandler callback = 
                        ()=>{
                            var ea = new RedoPostNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.RedoPostNotify += callback;
                    return Disposable.Create(()=> eventSource.RedoPostNotify-= callback);
                    
                }
            );
        }
        public class RedoPreNotifyEventArgs
        {
            public RedoPreNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_RedoPreNotifyEventHandler.html
        public static IObservable<RedoPreNotifyEventArgs> RedoPreNotifyObservable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<RedoPreNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_RedoPreNotifyEventHandler callback = 
                        ()=>{
                            var ea = new RedoPreNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.RedoPreNotify += callback;
                    return Disposable.Create(()=> eventSource.RedoPreNotify-= callback);
                    
                }
            );
        }
        public class UndoPreNotifyEventArgs
        {
            public UndoPreNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_UndoPreNotifyEventHandler.html
        public static IObservable<UndoPreNotifyEventArgs> UndoPreNotifyObservable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<UndoPreNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_UndoPreNotifyEventHandler callback = 
                        ()=>{
                            var ea = new UndoPreNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.UndoPreNotify += callback;
                    return Disposable.Create(()=> eventSource.UndoPreNotify-= callback);
                    
                }
            );
        }
        public class AutoSaveToStorageStoreNotifyEventArgs
        {
            public AutoSaveToStorageStoreNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_AutoSaveToStorageStoreNotifyEventHandler.html
        public static IObservable<AutoSaveToStorageStoreNotifyEventArgs> AutoSaveToStorageStoreNotifyObservable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<AutoSaveToStorageStoreNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_AutoSaveToStorageStoreNotifyEventHandler callback = 
                        ()=>{
                            var ea = new AutoSaveToStorageStoreNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.AutoSaveToStorageStoreNotify += callback;
                    return Disposable.Create(()=> eventSource.AutoSaveToStorageStoreNotify-= callback);
                    
                }
            );
        }
        public class InsertTableNotifyEventArgs
        {
            public InsertTableNotifyEventArgs (SolidWorks.Interop.sldworks.TableAnnotation TableAnnotation, System.String TableType, System.String TemplatePath)
            {
                this.TableAnnotation = TableAnnotation;
                this.TableType = TableType;
                this.TemplatePath = TemplatePath;
            }
            public SolidWorks.Interop.sldworks.TableAnnotation TableAnnotation { get; }
            public System.String TableType { get; }
            public System.String TemplatePath { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_InsertTableNotifyEventHandler.html
        public static IObservable<InsertTableNotifyEventArgs> InsertTableNotifyObservable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<InsertTableNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_InsertTableNotifyEventHandler callback = 
                        (SolidWorks.Interop.sldworks.TableAnnotation TableAnnotation, System.String TableType, System.String TemplatePath)=>{
                            var ea = new InsertTableNotifyEventArgs(TableAnnotation, TableType, TemplatePath);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.InsertTableNotify += callback;
                    return Disposable.Create(()=> eventSource.InsertTableNotify-= callback);
                    
                }
            );
        }
        public class ModifyTableNotifyEventArgs
        {
            public ModifyTableNotifyEventArgs (SolidWorks.Interop.sldworks.TableAnnotation TableAnnotation, System.Int32 TableType, System.Int32 reason, System.Int32 RowInfo, System.Int32 ColumnInfo, System.String DataInfo)
            {
                this.TableAnnotation = TableAnnotation;
                this.TableType = TableType;
                this.reason = reason;
                this.RowInfo = RowInfo;
                this.ColumnInfo = ColumnInfo;
                this.DataInfo = DataInfo;
            }
            public SolidWorks.Interop.sldworks.TableAnnotation TableAnnotation { get; }
            public System.Int32 TableType { get; }
            public System.Int32 reason { get; }
            public System.Int32 RowInfo { get; }
            public System.Int32 ColumnInfo { get; }
            public System.String DataInfo { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_ModifyTableNotifyEventHandler.html
        public static IObservable<ModifyTableNotifyEventArgs> ModifyTableNotifyObservable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<ModifyTableNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_ModifyTableNotifyEventHandler callback = 
                        (SolidWorks.Interop.sldworks.TableAnnotation TableAnnotation, System.Int32 TableType, System.Int32 reason, System.Int32 RowInfo, System.Int32 ColumnInfo, System.String DataInfo)=>{
                            var ea = new ModifyTableNotifyEventArgs(TableAnnotation, TableType, reason, RowInfo, ColumnInfo, DataInfo);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.ModifyTableNotify += callback;
                    return Disposable.Create(()=> eventSource.ModifyTableNotify-= callback);
                    
                }
            );
        }
        public class UserSelectionPostNotifyEventArgs
        {
            public UserSelectionPostNotifyEventArgs ()
            {
            }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_UserSelectionPostNotifyEventHandler.html
        public static IObservable<UserSelectionPostNotifyEventArgs> UserSelectionPostNotifyObservable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<UserSelectionPostNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_UserSelectionPostNotifyEventHandler callback = 
                        ()=>{
                            var ea = new UserSelectionPostNotifyEventArgs();
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.UserSelectionPostNotify += callback;
                    return Disposable.Create(()=> eventSource.UserSelectionPostNotify-= callback);
                    
                }
            );
        }
        public class ActivateSheetPreNotifyEventArgs
        {
            public ActivateSheetPreNotifyEventArgs (System.String SheetName)
            {
                this.SheetName = SheetName;
            }
            public System.String SheetName { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_ActivateSheetPreNotifyEventHandler.html
        public static IObservable<ActivateSheetPreNotifyEventArgs> ActivateSheetPreNotifyObservable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<ActivateSheetPreNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_ActivateSheetPreNotifyEventHandler callback = 
                        (System.String SheetName)=>{
                            var ea = new ActivateSheetPreNotifyEventArgs(SheetName);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.ActivateSheetPreNotify += callback;
                    return Disposable.Create(()=> eventSource.ActivateSheetPreNotify-= callback);
                    
                }
            );
        }
        public class ActivateSheetPostNotifyEventArgs
        {
            public ActivateSheetPostNotifyEventArgs (System.String SheetName)
            {
                this.SheetName = SheetName;
            }
            public System.String SheetName { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_ActivateSheetPostNotifyEventHandler.html
        public static IObservable<ActivateSheetPostNotifyEventArgs> ActivateSheetPostNotifyObservable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<ActivateSheetPostNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_ActivateSheetPostNotifyEventHandler callback = 
                        (System.String SheetName)=>{
                            var ea = new ActivateSheetPostNotifyEventArgs(SheetName);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.ActivateSheetPostNotify += callback;
                    return Disposable.Create(()=> eventSource.ActivateSheetPostNotify-= callback);
                    
                }
            );
        }
        public class CommandManagerTabActivatedPreNotifyEventArgs
        {
            public CommandManagerTabActivatedPreNotifyEventArgs (System.Int32 CommandTabIndex, System.String CommandTabName)
            {
                this.CommandTabIndex = CommandTabIndex;
                this.CommandTabName = CommandTabName;
            }
            public System.Int32 CommandTabIndex { get; }
            public System.String CommandTabName { get; }
        }
        /// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DDrawingDocEvents_CommandManagerTabActivatedPreNotifyEventHandler.html
        public static IObservable<CommandManagerTabActivatedPreNotifyEventArgs> CommandManagerTabActivatedPreNotifyObservable(this SolidWorks.Interop.sldworks.DDrawingDocEvents_Event eventSource)
        {
            return Observable.Create<CommandManagerTabActivatedPreNotifyEventArgs>
            ( observer => 
                {
                    SolidWorks.Interop.sldworks.DDrawingDocEvents_CommandManagerTabActivatedPreNotifyEventHandler callback = 
                        (System.Int32 CommandTabIndex, System.String CommandTabName)=>{
                            var ea = new CommandManagerTabActivatedPreNotifyEventArgs(CommandTabIndex, CommandTabName);
                            observer.OnNext(ea);
                            return default(System.Int32);
                        }; 

                    eventSource.CommandManagerTabActivatedPreNotify += callback;
                    return Disposable.Create(()=> eventSource.CommandManagerTabActivatedPreNotify-= callback);
                    
                }
            );
        }
    }
}


namespace SolidworksAddinFramework.Events {
public static class ModelDoc2Events
{
        public class RegenNotifyEventArgs
        {
            public RegenNotifyEventArgs ()
            {
            }
        }
        public static IObservable<RegenNotifyEventArgs> RegenNotifyObservable(this IModelDoc2 modelDoc)
        {
            return
            (modelDoc as PartDoc)?.RegenNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as DrawingDoc)?.RegenNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as AssemblyDoc)?.RegenNotifyObservable().Select(v=>Convert(v));

        }

        static RegenNotifyEventArgs Convert(SolidworksAddinFramework.Events.DPartDocEvents_Event.RegenNotifyEventArgs source ){
            return new RegenNotifyEventArgs();;
        }
        static RegenNotifyEventArgs Convert(SolidworksAddinFramework.Events.DAssemblyDocEvents_Event.RegenNotifyEventArgs source ){
            return new RegenNotifyEventArgs();;
        }
        static RegenNotifyEventArgs Convert(SolidworksAddinFramework.Events.DDrawingDocEvents_Event.RegenNotifyEventArgs source ){
            return new RegenNotifyEventArgs();;
        }
        public class DestroyNotifyEventArgs
        {
            public DestroyNotifyEventArgs ()
            {
            }
        }
        public static IObservable<DestroyNotifyEventArgs> DestroyNotifyObservable(this IModelDoc2 modelDoc)
        {
            return
            (modelDoc as PartDoc)?.DestroyNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as DrawingDoc)?.DestroyNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as AssemblyDoc)?.DestroyNotifyObservable().Select(v=>Convert(v));

        }

        static DestroyNotifyEventArgs Convert(SolidworksAddinFramework.Events.DPartDocEvents_Event.DestroyNotifyEventArgs source ){
            return new DestroyNotifyEventArgs();;
        }
        static DestroyNotifyEventArgs Convert(SolidworksAddinFramework.Events.DAssemblyDocEvents_Event.DestroyNotifyEventArgs source ){
            return new DestroyNotifyEventArgs();;
        }
        static DestroyNotifyEventArgs Convert(SolidworksAddinFramework.Events.DDrawingDocEvents_Event.DestroyNotifyEventArgs source ){
            return new DestroyNotifyEventArgs();;
        }
        public class RegenPostNotifyEventArgs
        {
            public RegenPostNotifyEventArgs ()
            {
            }
        }
        public static IObservable<RegenPostNotifyEventArgs> RegenPostNotifyObservable(this IModelDoc2 modelDoc)
        {
            return
            (modelDoc as PartDoc)?.RegenPostNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as DrawingDoc)?.RegenPostNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as AssemblyDoc)?.RegenPostNotifyObservable().Select(v=>Convert(v));

        }

        static RegenPostNotifyEventArgs Convert(SolidworksAddinFramework.Events.DPartDocEvents_Event.RegenPostNotifyEventArgs source ){
            return new RegenPostNotifyEventArgs();;
        }
        static RegenPostNotifyEventArgs Convert(SolidworksAddinFramework.Events.DAssemblyDocEvents_Event.RegenPostNotifyEventArgs source ){
            return new RegenPostNotifyEventArgs();;
        }
        static RegenPostNotifyEventArgs Convert(SolidworksAddinFramework.Events.DDrawingDocEvents_Event.RegenPostNotifyEventArgs source ){
            return new RegenPostNotifyEventArgs();;
        }
        public class ViewNewNotifyEventArgs
        {
            public ViewNewNotifyEventArgs ()
            {
            }
        }
        public static IObservable<ViewNewNotifyEventArgs> ViewNewNotifyObservable(this IModelDoc2 modelDoc)
        {
            return
            (modelDoc as PartDoc)?.ViewNewNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as DrawingDoc)?.ViewNewNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as AssemblyDoc)?.ViewNewNotifyObservable().Select(v=>Convert(v));

        }

        static ViewNewNotifyEventArgs Convert(SolidworksAddinFramework.Events.DPartDocEvents_Event.ViewNewNotifyEventArgs source ){
            return new ViewNewNotifyEventArgs();;
        }
        static ViewNewNotifyEventArgs Convert(SolidworksAddinFramework.Events.DAssemblyDocEvents_Event.ViewNewNotifyEventArgs source ){
            return new ViewNewNotifyEventArgs();;
        }
        static ViewNewNotifyEventArgs Convert(SolidworksAddinFramework.Events.DDrawingDocEvents_Event.ViewNewNotifyEventArgs source ){
            return new ViewNewNotifyEventArgs();;
        }
        public class NewSelectionNotifyEventArgs
        {
            public NewSelectionNotifyEventArgs ()
            {
            }
        }
        public static IObservable<NewSelectionNotifyEventArgs> NewSelectionNotifyObservable(this IModelDoc2 modelDoc)
        {
            return
            (modelDoc as PartDoc)?.NewSelectionNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as DrawingDoc)?.NewSelectionNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as AssemblyDoc)?.NewSelectionNotifyObservable().Select(v=>Convert(v));

        }

        static NewSelectionNotifyEventArgs Convert(SolidworksAddinFramework.Events.DPartDocEvents_Event.NewSelectionNotifyEventArgs source ){
            return new NewSelectionNotifyEventArgs();;
        }
        static NewSelectionNotifyEventArgs Convert(SolidworksAddinFramework.Events.DAssemblyDocEvents_Event.NewSelectionNotifyEventArgs source ){
            return new NewSelectionNotifyEventArgs();;
        }
        static NewSelectionNotifyEventArgs Convert(SolidworksAddinFramework.Events.DDrawingDocEvents_Event.NewSelectionNotifyEventArgs source ){
            return new NewSelectionNotifyEventArgs();;
        }
        public class FileSaveNotifyEventArgs
        {
            public FileSaveNotifyEventArgs (System.String FileName)
            {
                this.FileName = FileName;
            }
            public System.String FileName { get; }
        }
        public static IObservable<FileSaveNotifyEventArgs> FileSaveNotifyObservable(this IModelDoc2 modelDoc)
        {
            return
            (modelDoc as PartDoc)?.FileSaveNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as DrawingDoc)?.FileSaveNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as AssemblyDoc)?.FileSaveNotifyObservable().Select(v=>Convert(v));

        }

        static FileSaveNotifyEventArgs Convert(SolidworksAddinFramework.Events.DPartDocEvents_Event.FileSaveNotifyEventArgs source ){
            return new FileSaveNotifyEventArgs(source.FileName);;
        }
        static FileSaveNotifyEventArgs Convert(SolidworksAddinFramework.Events.DAssemblyDocEvents_Event.FileSaveNotifyEventArgs source ){
            return new FileSaveNotifyEventArgs(source.FileName);;
        }
        static FileSaveNotifyEventArgs Convert(SolidworksAddinFramework.Events.DDrawingDocEvents_Event.FileSaveNotifyEventArgs source ){
            return new FileSaveNotifyEventArgs(source.FileName);;
        }
        public class FileSaveAsNotifyEventArgs
        {
            public FileSaveAsNotifyEventArgs (System.String FileName)
            {
                this.FileName = FileName;
            }
            public System.String FileName { get; }
        }
        public static IObservable<FileSaveAsNotifyEventArgs> FileSaveAsNotifyObservable(this IModelDoc2 modelDoc)
        {
            return
            (modelDoc as PartDoc)?.FileSaveAsNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as DrawingDoc)?.FileSaveAsNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as AssemblyDoc)?.FileSaveAsNotifyObservable().Select(v=>Convert(v));

        }

        static FileSaveAsNotifyEventArgs Convert(SolidworksAddinFramework.Events.DPartDocEvents_Event.FileSaveAsNotifyEventArgs source ){
            return new FileSaveAsNotifyEventArgs(source.FileName);;
        }
        static FileSaveAsNotifyEventArgs Convert(SolidworksAddinFramework.Events.DAssemblyDocEvents_Event.FileSaveAsNotifyEventArgs source ){
            return new FileSaveAsNotifyEventArgs(source.FileName);;
        }
        static FileSaveAsNotifyEventArgs Convert(SolidworksAddinFramework.Events.DDrawingDocEvents_Event.FileSaveAsNotifyEventArgs source ){
            return new FileSaveAsNotifyEventArgs(source.FileName);;
        }
        public class LoadFromStorageNotifyEventArgs
        {
            public LoadFromStorageNotifyEventArgs ()
            {
            }
        }
        public static IObservable<LoadFromStorageNotifyEventArgs> LoadFromStorageNotifyObservable(this IModelDoc2 modelDoc)
        {
            return
            (modelDoc as PartDoc)?.LoadFromStorageNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as DrawingDoc)?.LoadFromStorageNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as AssemblyDoc)?.LoadFromStorageNotifyObservable().Select(v=>Convert(v));

        }

        static LoadFromStorageNotifyEventArgs Convert(SolidworksAddinFramework.Events.DPartDocEvents_Event.LoadFromStorageNotifyEventArgs source ){
            return new LoadFromStorageNotifyEventArgs();;
        }
        static LoadFromStorageNotifyEventArgs Convert(SolidworksAddinFramework.Events.DAssemblyDocEvents_Event.LoadFromStorageNotifyEventArgs source ){
            return new LoadFromStorageNotifyEventArgs();;
        }
        static LoadFromStorageNotifyEventArgs Convert(SolidworksAddinFramework.Events.DDrawingDocEvents_Event.LoadFromStorageNotifyEventArgs source ){
            return new LoadFromStorageNotifyEventArgs();;
        }
        public class SaveToStorageNotifyEventArgs
        {
            public SaveToStorageNotifyEventArgs ()
            {
            }
        }
        public static IObservable<SaveToStorageNotifyEventArgs> SaveToStorageNotifyObservable(this IModelDoc2 modelDoc)
        {
            return
            (modelDoc as PartDoc)?.SaveToStorageNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as DrawingDoc)?.SaveToStorageNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as AssemblyDoc)?.SaveToStorageNotifyObservable().Select(v=>Convert(v));

        }

        static SaveToStorageNotifyEventArgs Convert(SolidworksAddinFramework.Events.DPartDocEvents_Event.SaveToStorageNotifyEventArgs source ){
            return new SaveToStorageNotifyEventArgs();;
        }
        static SaveToStorageNotifyEventArgs Convert(SolidworksAddinFramework.Events.DAssemblyDocEvents_Event.SaveToStorageNotifyEventArgs source ){
            return new SaveToStorageNotifyEventArgs();;
        }
        static SaveToStorageNotifyEventArgs Convert(SolidworksAddinFramework.Events.DDrawingDocEvents_Event.SaveToStorageNotifyEventArgs source ){
            return new SaveToStorageNotifyEventArgs();;
        }
        public class ActiveConfigChangeNotifyEventArgs
        {
            public ActiveConfigChangeNotifyEventArgs ()
            {
            }
        }
        public static IObservable<ActiveConfigChangeNotifyEventArgs> ActiveConfigChangeNotifyObservable(this IModelDoc2 modelDoc)
        {
            return
            (modelDoc as PartDoc)?.ActiveConfigChangeNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as DrawingDoc)?.ActiveConfigChangeNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as AssemblyDoc)?.ActiveConfigChangeNotifyObservable().Select(v=>Convert(v));

        }

        static ActiveConfigChangeNotifyEventArgs Convert(SolidworksAddinFramework.Events.DPartDocEvents_Event.ActiveConfigChangeNotifyEventArgs source ){
            return new ActiveConfigChangeNotifyEventArgs();;
        }
        static ActiveConfigChangeNotifyEventArgs Convert(SolidworksAddinFramework.Events.DAssemblyDocEvents_Event.ActiveConfigChangeNotifyEventArgs source ){
            return new ActiveConfigChangeNotifyEventArgs();;
        }
        static ActiveConfigChangeNotifyEventArgs Convert(SolidworksAddinFramework.Events.DDrawingDocEvents_Event.ActiveConfigChangeNotifyEventArgs source ){
            return new ActiveConfigChangeNotifyEventArgs();;
        }
        public class ActiveConfigChangePostNotifyEventArgs
        {
            public ActiveConfigChangePostNotifyEventArgs ()
            {
            }
        }
        public static IObservable<ActiveConfigChangePostNotifyEventArgs> ActiveConfigChangePostNotifyObservable(this IModelDoc2 modelDoc)
        {
            return
            (modelDoc as PartDoc)?.ActiveConfigChangePostNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as DrawingDoc)?.ActiveConfigChangePostNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as AssemblyDoc)?.ActiveConfigChangePostNotifyObservable().Select(v=>Convert(v));

        }

        static ActiveConfigChangePostNotifyEventArgs Convert(SolidworksAddinFramework.Events.DPartDocEvents_Event.ActiveConfigChangePostNotifyEventArgs source ){
            return new ActiveConfigChangePostNotifyEventArgs();;
        }
        static ActiveConfigChangePostNotifyEventArgs Convert(SolidworksAddinFramework.Events.DAssemblyDocEvents_Event.ActiveConfigChangePostNotifyEventArgs source ){
            return new ActiveConfigChangePostNotifyEventArgs();;
        }
        static ActiveConfigChangePostNotifyEventArgs Convert(SolidworksAddinFramework.Events.DDrawingDocEvents_Event.ActiveConfigChangePostNotifyEventArgs source ){
            return new ActiveConfigChangePostNotifyEventArgs();;
        }
        public class ViewNewNotify2EventArgs
        {
            public ViewNewNotify2EventArgs (System.Object viewBeingAdded)
            {
                this.viewBeingAdded = viewBeingAdded;
            }
            public System.Object viewBeingAdded { get; }
        }
        public static IObservable<ViewNewNotify2EventArgs> ViewNewNotify2Observable(this IModelDoc2 modelDoc)
        {
            return
            (modelDoc as PartDoc)?.ViewNewNotify2Observable().Select(v=>Convert(v))
                ??
            (modelDoc as DrawingDoc)?.ViewNewNotify2Observable().Select(v=>Convert(v))
                ??
            (modelDoc as AssemblyDoc)?.ViewNewNotify2Observable().Select(v=>Convert(v));

        }

        static ViewNewNotify2EventArgs Convert(SolidworksAddinFramework.Events.DPartDocEvents_Event.ViewNewNotify2EventArgs source ){
            return new ViewNewNotify2EventArgs(source.viewBeingAdded);;
        }
        static ViewNewNotify2EventArgs Convert(SolidworksAddinFramework.Events.DAssemblyDocEvents_Event.ViewNewNotify2EventArgs source ){
            return new ViewNewNotify2EventArgs(source.viewBeingAdded);;
        }
        static ViewNewNotify2EventArgs Convert(SolidworksAddinFramework.Events.DDrawingDocEvents_Event.ViewNewNotify2EventArgs source ){
            return new ViewNewNotify2EventArgs(source.viewBeingAdded);;
        }
        public class AddItemNotifyEventArgs
        {
            public AddItemNotifyEventArgs (System.Int32 EntityType, System.String itemName)
            {
                this.EntityType = EntityType;
                this.itemName = itemName;
            }
            public System.Int32 EntityType { get; }
            public System.String itemName { get; }
        }
        public static IObservable<AddItemNotifyEventArgs> AddItemNotifyObservable(this IModelDoc2 modelDoc)
        {
            return
            (modelDoc as PartDoc)?.AddItemNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as DrawingDoc)?.AddItemNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as AssemblyDoc)?.AddItemNotifyObservable().Select(v=>Convert(v));

        }

        static AddItemNotifyEventArgs Convert(SolidworksAddinFramework.Events.DPartDocEvents_Event.AddItemNotifyEventArgs source ){
            return new AddItemNotifyEventArgs(source.EntityType,source.itemName);;
        }
        static AddItemNotifyEventArgs Convert(SolidworksAddinFramework.Events.DAssemblyDocEvents_Event.AddItemNotifyEventArgs source ){
            return new AddItemNotifyEventArgs(source.EntityType,source.itemName);;
        }
        static AddItemNotifyEventArgs Convert(SolidworksAddinFramework.Events.DDrawingDocEvents_Event.AddItemNotifyEventArgs source ){
            return new AddItemNotifyEventArgs(source.EntityType,source.itemName);;
        }
        public class RenameItemNotifyEventArgs
        {
            public RenameItemNotifyEventArgs (System.Int32 EntityType, System.String oldName, System.String NewName)
            {
                this.EntityType = EntityType;
                this.oldName = oldName;
                this.NewName = NewName;
            }
            public System.Int32 EntityType { get; }
            public System.String oldName { get; }
            public System.String NewName { get; }
        }
        public static IObservable<RenameItemNotifyEventArgs> RenameItemNotifyObservable(this IModelDoc2 modelDoc)
        {
            return
            (modelDoc as PartDoc)?.RenameItemNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as DrawingDoc)?.RenameItemNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as AssemblyDoc)?.RenameItemNotifyObservable().Select(v=>Convert(v));

        }

        static RenameItemNotifyEventArgs Convert(SolidworksAddinFramework.Events.DPartDocEvents_Event.RenameItemNotifyEventArgs source ){
            return new RenameItemNotifyEventArgs(source.EntityType,source.oldName,source.NewName);;
        }
        static RenameItemNotifyEventArgs Convert(SolidworksAddinFramework.Events.DAssemblyDocEvents_Event.RenameItemNotifyEventArgs source ){
            return new RenameItemNotifyEventArgs(source.EntityType,source.oldName,source.NewName);;
        }
        static RenameItemNotifyEventArgs Convert(SolidworksAddinFramework.Events.DDrawingDocEvents_Event.RenameItemNotifyEventArgs source ){
            return new RenameItemNotifyEventArgs(source.EntityType,source.oldName,source.NewName);;
        }
        public class DeleteItemNotifyEventArgs
        {
            public DeleteItemNotifyEventArgs (System.Int32 EntityType, System.String itemName)
            {
                this.EntityType = EntityType;
                this.itemName = itemName;
            }
            public System.Int32 EntityType { get; }
            public System.String itemName { get; }
        }
        public static IObservable<DeleteItemNotifyEventArgs> DeleteItemNotifyObservable(this IModelDoc2 modelDoc)
        {
            return
            (modelDoc as PartDoc)?.DeleteItemNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as DrawingDoc)?.DeleteItemNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as AssemblyDoc)?.DeleteItemNotifyObservable().Select(v=>Convert(v));

        }

        static DeleteItemNotifyEventArgs Convert(SolidworksAddinFramework.Events.DPartDocEvents_Event.DeleteItemNotifyEventArgs source ){
            return new DeleteItemNotifyEventArgs(source.EntityType,source.itemName);;
        }
        static DeleteItemNotifyEventArgs Convert(SolidworksAddinFramework.Events.DAssemblyDocEvents_Event.DeleteItemNotifyEventArgs source ){
            return new DeleteItemNotifyEventArgs(source.EntityType,source.itemName);;
        }
        static DeleteItemNotifyEventArgs Convert(SolidworksAddinFramework.Events.DDrawingDocEvents_Event.DeleteItemNotifyEventArgs source ){
            return new DeleteItemNotifyEventArgs(source.EntityType,source.itemName);;
        }
        public class ModifyNotifyEventArgs
        {
            public ModifyNotifyEventArgs ()
            {
            }
        }
        public static IObservable<ModifyNotifyEventArgs> ModifyNotifyObservable(this IModelDoc2 modelDoc)
        {
            return
            (modelDoc as PartDoc)?.ModifyNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as DrawingDoc)?.ModifyNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as AssemblyDoc)?.ModifyNotifyObservable().Select(v=>Convert(v));

        }

        static ModifyNotifyEventArgs Convert(SolidworksAddinFramework.Events.DPartDocEvents_Event.ModifyNotifyEventArgs source ){
            return new ModifyNotifyEventArgs();;
        }
        static ModifyNotifyEventArgs Convert(SolidworksAddinFramework.Events.DAssemblyDocEvents_Event.ModifyNotifyEventArgs source ){
            return new ModifyNotifyEventArgs();;
        }
        static ModifyNotifyEventArgs Convert(SolidworksAddinFramework.Events.DDrawingDocEvents_Event.ModifyNotifyEventArgs source ){
            return new ModifyNotifyEventArgs();;
        }
        public class FileReloadNotifyEventArgs
        {
            public FileReloadNotifyEventArgs ()
            {
            }
        }
        public static IObservable<FileReloadNotifyEventArgs> FileReloadNotifyObservable(this IModelDoc2 modelDoc)
        {
            return
            (modelDoc as PartDoc)?.FileReloadNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as DrawingDoc)?.FileReloadNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as AssemblyDoc)?.FileReloadNotifyObservable().Select(v=>Convert(v));

        }

        static FileReloadNotifyEventArgs Convert(SolidworksAddinFramework.Events.DPartDocEvents_Event.FileReloadNotifyEventArgs source ){
            return new FileReloadNotifyEventArgs();;
        }
        static FileReloadNotifyEventArgs Convert(SolidworksAddinFramework.Events.DAssemblyDocEvents_Event.FileReloadNotifyEventArgs source ){
            return new FileReloadNotifyEventArgs();;
        }
        static FileReloadNotifyEventArgs Convert(SolidworksAddinFramework.Events.DDrawingDocEvents_Event.FileReloadNotifyEventArgs source ){
            return new FileReloadNotifyEventArgs();;
        }
        public class AddCustomPropertyNotifyEventArgs
        {
            public AddCustomPropertyNotifyEventArgs (System.String propName, System.String Configuration, System.String Value, System.Int32 valueType)
            {
                this.propName = propName;
                this.Configuration = Configuration;
                this.Value = Value;
                this.valueType = valueType;
            }
            public System.String propName { get; }
            public System.String Configuration { get; }
            public System.String Value { get; }
            public System.Int32 valueType { get; }
        }
        public static IObservable<AddCustomPropertyNotifyEventArgs> AddCustomPropertyNotifyObservable(this IModelDoc2 modelDoc)
        {
            return
            (modelDoc as PartDoc)?.AddCustomPropertyNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as DrawingDoc)?.AddCustomPropertyNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as AssemblyDoc)?.AddCustomPropertyNotifyObservable().Select(v=>Convert(v));

        }

        static AddCustomPropertyNotifyEventArgs Convert(SolidworksAddinFramework.Events.DPartDocEvents_Event.AddCustomPropertyNotifyEventArgs source ){
            return new AddCustomPropertyNotifyEventArgs(source.propName,source.Configuration,source.Value,source.valueType);;
        }
        static AddCustomPropertyNotifyEventArgs Convert(SolidworksAddinFramework.Events.DAssemblyDocEvents_Event.AddCustomPropertyNotifyEventArgs source ){
            return new AddCustomPropertyNotifyEventArgs(source.propName,source.Configuration,source.Value,source.valueType);;
        }
        static AddCustomPropertyNotifyEventArgs Convert(SolidworksAddinFramework.Events.DDrawingDocEvents_Event.AddCustomPropertyNotifyEventArgs source ){
            return new AddCustomPropertyNotifyEventArgs(source.propName,source.Configuration,source.Value,source.valueType);;
        }
        public class ChangeCustomPropertyNotifyEventArgs
        {
            public ChangeCustomPropertyNotifyEventArgs (System.String propName, System.String Configuration, System.String oldValue, System.String NewValue, System.Int32 valueType)
            {
                this.propName = propName;
                this.Configuration = Configuration;
                this.oldValue = oldValue;
                this.NewValue = NewValue;
                this.valueType = valueType;
            }
            public System.String propName { get; }
            public System.String Configuration { get; }
            public System.String oldValue { get; }
            public System.String NewValue { get; }
            public System.Int32 valueType { get; }
        }
        public static IObservable<ChangeCustomPropertyNotifyEventArgs> ChangeCustomPropertyNotifyObservable(this IModelDoc2 modelDoc)
        {
            return
            (modelDoc as PartDoc)?.ChangeCustomPropertyNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as DrawingDoc)?.ChangeCustomPropertyNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as AssemblyDoc)?.ChangeCustomPropertyNotifyObservable().Select(v=>Convert(v));

        }

        static ChangeCustomPropertyNotifyEventArgs Convert(SolidworksAddinFramework.Events.DPartDocEvents_Event.ChangeCustomPropertyNotifyEventArgs source ){
            return new ChangeCustomPropertyNotifyEventArgs(source.propName,source.Configuration,source.oldValue,source.NewValue,source.valueType);;
        }
        static ChangeCustomPropertyNotifyEventArgs Convert(SolidworksAddinFramework.Events.DAssemblyDocEvents_Event.ChangeCustomPropertyNotifyEventArgs source ){
            return new ChangeCustomPropertyNotifyEventArgs(source.propName,source.Configuration,source.oldValue,source.NewValue,source.valueType);;
        }
        static ChangeCustomPropertyNotifyEventArgs Convert(SolidworksAddinFramework.Events.DDrawingDocEvents_Event.ChangeCustomPropertyNotifyEventArgs source ){
            return new ChangeCustomPropertyNotifyEventArgs(source.propName,source.Configuration,source.oldValue,source.NewValue,source.valueType);;
        }
        public class DeleteCustomPropertyNotifyEventArgs
        {
            public DeleteCustomPropertyNotifyEventArgs (System.String propName, System.String Configuration, System.String Value, System.Int32 valueType)
            {
                this.propName = propName;
                this.Configuration = Configuration;
                this.Value = Value;
                this.valueType = valueType;
            }
            public System.String propName { get; }
            public System.String Configuration { get; }
            public System.String Value { get; }
            public System.Int32 valueType { get; }
        }
        public static IObservable<DeleteCustomPropertyNotifyEventArgs> DeleteCustomPropertyNotifyObservable(this IModelDoc2 modelDoc)
        {
            return
            (modelDoc as PartDoc)?.DeleteCustomPropertyNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as DrawingDoc)?.DeleteCustomPropertyNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as AssemblyDoc)?.DeleteCustomPropertyNotifyObservable().Select(v=>Convert(v));

        }

        static DeleteCustomPropertyNotifyEventArgs Convert(SolidworksAddinFramework.Events.DPartDocEvents_Event.DeleteCustomPropertyNotifyEventArgs source ){
            return new DeleteCustomPropertyNotifyEventArgs(source.propName,source.Configuration,source.Value,source.valueType);;
        }
        static DeleteCustomPropertyNotifyEventArgs Convert(SolidworksAddinFramework.Events.DAssemblyDocEvents_Event.DeleteCustomPropertyNotifyEventArgs source ){
            return new DeleteCustomPropertyNotifyEventArgs(source.propName,source.Configuration,source.Value,source.valueType);;
        }
        static DeleteCustomPropertyNotifyEventArgs Convert(SolidworksAddinFramework.Events.DDrawingDocEvents_Event.DeleteCustomPropertyNotifyEventArgs source ){
            return new DeleteCustomPropertyNotifyEventArgs(source.propName,source.Configuration,source.Value,source.valueType);;
        }
        public class FileSaveAsNotify2EventArgs
        {
            public FileSaveAsNotify2EventArgs (System.String FileName)
            {
                this.FileName = FileName;
            }
            public System.String FileName { get; }
        }
        public static IObservable<FileSaveAsNotify2EventArgs> FileSaveAsNotify2Observable(this IModelDoc2 modelDoc)
        {
            return
            (modelDoc as PartDoc)?.FileSaveAsNotify2Observable().Select(v=>Convert(v))
                ??
            (modelDoc as DrawingDoc)?.FileSaveAsNotify2Observable().Select(v=>Convert(v))
                ??
            (modelDoc as AssemblyDoc)?.FileSaveAsNotify2Observable().Select(v=>Convert(v));

        }

        static FileSaveAsNotify2EventArgs Convert(SolidworksAddinFramework.Events.DPartDocEvents_Event.FileSaveAsNotify2EventArgs source ){
            return new FileSaveAsNotify2EventArgs(source.FileName);;
        }
        static FileSaveAsNotify2EventArgs Convert(SolidworksAddinFramework.Events.DAssemblyDocEvents_Event.FileSaveAsNotify2EventArgs source ){
            return new FileSaveAsNotify2EventArgs(source.FileName);;
        }
        static FileSaveAsNotify2EventArgs Convert(SolidworksAddinFramework.Events.DDrawingDocEvents_Event.FileSaveAsNotify2EventArgs source ){
            return new FileSaveAsNotify2EventArgs(source.FileName);;
        }
        public class DeleteSelectionPreNotifyEventArgs
        {
            public DeleteSelectionPreNotifyEventArgs ()
            {
            }
        }
        public static IObservable<DeleteSelectionPreNotifyEventArgs> DeleteSelectionPreNotifyObservable(this IModelDoc2 modelDoc)
        {
            return
            (modelDoc as PartDoc)?.DeleteSelectionPreNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as DrawingDoc)?.DeleteSelectionPreNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as AssemblyDoc)?.DeleteSelectionPreNotifyObservable().Select(v=>Convert(v));

        }

        static DeleteSelectionPreNotifyEventArgs Convert(SolidworksAddinFramework.Events.DPartDocEvents_Event.DeleteSelectionPreNotifyEventArgs source ){
            return new DeleteSelectionPreNotifyEventArgs();;
        }
        static DeleteSelectionPreNotifyEventArgs Convert(SolidworksAddinFramework.Events.DAssemblyDocEvents_Event.DeleteSelectionPreNotifyEventArgs source ){
            return new DeleteSelectionPreNotifyEventArgs();;
        }
        static DeleteSelectionPreNotifyEventArgs Convert(SolidworksAddinFramework.Events.DDrawingDocEvents_Event.DeleteSelectionPreNotifyEventArgs source ){
            return new DeleteSelectionPreNotifyEventArgs();;
        }
        public class FileReloadPreNotifyEventArgs
        {
            public FileReloadPreNotifyEventArgs ()
            {
            }
        }
        public static IObservable<FileReloadPreNotifyEventArgs> FileReloadPreNotifyObservable(this IModelDoc2 modelDoc)
        {
            return
            (modelDoc as PartDoc)?.FileReloadPreNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as DrawingDoc)?.FileReloadPreNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as AssemblyDoc)?.FileReloadPreNotifyObservable().Select(v=>Convert(v));

        }

        static FileReloadPreNotifyEventArgs Convert(SolidworksAddinFramework.Events.DPartDocEvents_Event.FileReloadPreNotifyEventArgs source ){
            return new FileReloadPreNotifyEventArgs();;
        }
        static FileReloadPreNotifyEventArgs Convert(SolidworksAddinFramework.Events.DAssemblyDocEvents_Event.FileReloadPreNotifyEventArgs source ){
            return new FileReloadPreNotifyEventArgs();;
        }
        static FileReloadPreNotifyEventArgs Convert(SolidworksAddinFramework.Events.DDrawingDocEvents_Event.FileReloadPreNotifyEventArgs source ){
            return new FileReloadPreNotifyEventArgs();;
        }
        public class FileSavePostNotifyEventArgs
        {
            public FileSavePostNotifyEventArgs (System.Int32 saveType, System.String FileName)
            {
                this.saveType = saveType;
                this.FileName = FileName;
            }
            public System.Int32 saveType { get; }
            public System.String FileName { get; }
        }
        public static IObservable<FileSavePostNotifyEventArgs> FileSavePostNotifyObservable(this IModelDoc2 modelDoc)
        {
            return
            (modelDoc as PartDoc)?.FileSavePostNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as DrawingDoc)?.FileSavePostNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as AssemblyDoc)?.FileSavePostNotifyObservable().Select(v=>Convert(v));

        }

        static FileSavePostNotifyEventArgs Convert(SolidworksAddinFramework.Events.DPartDocEvents_Event.FileSavePostNotifyEventArgs source ){
            return new FileSavePostNotifyEventArgs(source.saveType,source.FileName);;
        }
        static FileSavePostNotifyEventArgs Convert(SolidworksAddinFramework.Events.DAssemblyDocEvents_Event.FileSavePostNotifyEventArgs source ){
            return new FileSavePostNotifyEventArgs(source.saveType,source.FileName);;
        }
        static FileSavePostNotifyEventArgs Convert(SolidworksAddinFramework.Events.DDrawingDocEvents_Event.FileSavePostNotifyEventArgs source ){
            return new FileSavePostNotifyEventArgs(source.saveType,source.FileName);;
        }
        public class LoadFromStorageStoreNotifyEventArgs
        {
            public LoadFromStorageStoreNotifyEventArgs ()
            {
            }
        }
        public static IObservable<LoadFromStorageStoreNotifyEventArgs> LoadFromStorageStoreNotifyObservable(this IModelDoc2 modelDoc)
        {
            return
            (modelDoc as PartDoc)?.LoadFromStorageStoreNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as DrawingDoc)?.LoadFromStorageStoreNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as AssemblyDoc)?.LoadFromStorageStoreNotifyObservable().Select(v=>Convert(v));

        }

        static LoadFromStorageStoreNotifyEventArgs Convert(SolidworksAddinFramework.Events.DPartDocEvents_Event.LoadFromStorageStoreNotifyEventArgs source ){
            return new LoadFromStorageStoreNotifyEventArgs();;
        }
        static LoadFromStorageStoreNotifyEventArgs Convert(SolidworksAddinFramework.Events.DAssemblyDocEvents_Event.LoadFromStorageStoreNotifyEventArgs source ){
            return new LoadFromStorageStoreNotifyEventArgs();;
        }
        static LoadFromStorageStoreNotifyEventArgs Convert(SolidworksAddinFramework.Events.DDrawingDocEvents_Event.LoadFromStorageStoreNotifyEventArgs source ){
            return new LoadFromStorageStoreNotifyEventArgs();;
        }
        public class SaveToStorageStoreNotifyEventArgs
        {
            public SaveToStorageStoreNotifyEventArgs ()
            {
            }
        }
        public static IObservable<SaveToStorageStoreNotifyEventArgs> SaveToStorageStoreNotifyObservable(this IModelDoc2 modelDoc)
        {
            return
            (modelDoc as PartDoc)?.SaveToStorageStoreNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as DrawingDoc)?.SaveToStorageStoreNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as AssemblyDoc)?.SaveToStorageStoreNotifyObservable().Select(v=>Convert(v));

        }

        static SaveToStorageStoreNotifyEventArgs Convert(SolidworksAddinFramework.Events.DPartDocEvents_Event.SaveToStorageStoreNotifyEventArgs source ){
            return new SaveToStorageStoreNotifyEventArgs();;
        }
        static SaveToStorageStoreNotifyEventArgs Convert(SolidworksAddinFramework.Events.DAssemblyDocEvents_Event.SaveToStorageStoreNotifyEventArgs source ){
            return new SaveToStorageStoreNotifyEventArgs();;
        }
        static SaveToStorageStoreNotifyEventArgs Convert(SolidworksAddinFramework.Events.DDrawingDocEvents_Event.SaveToStorageStoreNotifyEventArgs source ){
            return new SaveToStorageStoreNotifyEventArgs();;
        }
        public class FeatureManagerTreeRebuildNotifyEventArgs
        {
            public FeatureManagerTreeRebuildNotifyEventArgs ()
            {
            }
        }
        public static IObservable<FeatureManagerTreeRebuildNotifyEventArgs> FeatureManagerTreeRebuildNotifyObservable(this IModelDoc2 modelDoc)
        {
            return
            (modelDoc as PartDoc)?.FeatureManagerTreeRebuildNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as DrawingDoc)?.FeatureManagerTreeRebuildNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as AssemblyDoc)?.FeatureManagerTreeRebuildNotifyObservable().Select(v=>Convert(v));

        }

        static FeatureManagerTreeRebuildNotifyEventArgs Convert(SolidworksAddinFramework.Events.DPartDocEvents_Event.FeatureManagerTreeRebuildNotifyEventArgs source ){
            return new FeatureManagerTreeRebuildNotifyEventArgs();;
        }
        static FeatureManagerTreeRebuildNotifyEventArgs Convert(SolidworksAddinFramework.Events.DAssemblyDocEvents_Event.FeatureManagerTreeRebuildNotifyEventArgs source ){
            return new FeatureManagerTreeRebuildNotifyEventArgs();;
        }
        static FeatureManagerTreeRebuildNotifyEventArgs Convert(SolidworksAddinFramework.Events.DDrawingDocEvents_Event.FeatureManagerTreeRebuildNotifyEventArgs source ){
            return new FeatureManagerTreeRebuildNotifyEventArgs();;
        }
        public class DynamicHighlightNotifyEventArgs
        {
            public DynamicHighlightNotifyEventArgs (System.Boolean bHighlightState)
            {
                this.bHighlightState = bHighlightState;
            }
            public System.Boolean bHighlightState { get; }
        }
        public static IObservable<DynamicHighlightNotifyEventArgs> DynamicHighlightNotifyObservable(this IModelDoc2 modelDoc)
        {
            return
            (modelDoc as PartDoc)?.DynamicHighlightNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as DrawingDoc)?.DynamicHighlightNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as AssemblyDoc)?.DynamicHighlightNotifyObservable().Select(v=>Convert(v));

        }

        static DynamicHighlightNotifyEventArgs Convert(SolidworksAddinFramework.Events.DPartDocEvents_Event.DynamicHighlightNotifyEventArgs source ){
            return new DynamicHighlightNotifyEventArgs(source.bHighlightState);;
        }
        static DynamicHighlightNotifyEventArgs Convert(SolidworksAddinFramework.Events.DAssemblyDocEvents_Event.DynamicHighlightNotifyEventArgs source ){
            return new DynamicHighlightNotifyEventArgs(source.bHighlightState);;
        }
        static DynamicHighlightNotifyEventArgs Convert(SolidworksAddinFramework.Events.DDrawingDocEvents_Event.DynamicHighlightNotifyEventArgs source ){
            return new DynamicHighlightNotifyEventArgs(source.bHighlightState);;
        }
        public class DimensionChangeNotifyEventArgs
        {
            public DimensionChangeNotifyEventArgs (System.Object displayDim)
            {
                this.displayDim = displayDim;
            }
            public System.Object displayDim { get; }
        }
        public static IObservable<DimensionChangeNotifyEventArgs> DimensionChangeNotifyObservable(this IModelDoc2 modelDoc)
        {
            return
            (modelDoc as PartDoc)?.DimensionChangeNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as DrawingDoc)?.DimensionChangeNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as AssemblyDoc)?.DimensionChangeNotifyObservable().Select(v=>Convert(v));

        }

        static DimensionChangeNotifyEventArgs Convert(SolidworksAddinFramework.Events.DPartDocEvents_Event.DimensionChangeNotifyEventArgs source ){
            return new DimensionChangeNotifyEventArgs(source.displayDim);;
        }
        static DimensionChangeNotifyEventArgs Convert(SolidworksAddinFramework.Events.DAssemblyDocEvents_Event.DimensionChangeNotifyEventArgs source ){
            return new DimensionChangeNotifyEventArgs(source.displayDim);;
        }
        static DimensionChangeNotifyEventArgs Convert(SolidworksAddinFramework.Events.DDrawingDocEvents_Event.DimensionChangeNotifyEventArgs source ){
            return new DimensionChangeNotifyEventArgs(source.displayDim);;
        }
        public class FileReloadCancelNotifyEventArgs
        {
            public FileReloadCancelNotifyEventArgs (System.Int32 ErrorCode)
            {
                this.ErrorCode = ErrorCode;
            }
            public System.Int32 ErrorCode { get; }
        }
        public static IObservable<FileReloadCancelNotifyEventArgs> FileReloadCancelNotifyObservable(this IModelDoc2 modelDoc)
        {
            return
            (modelDoc as PartDoc)?.FileReloadCancelNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as DrawingDoc)?.FileReloadCancelNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as AssemblyDoc)?.FileReloadCancelNotifyObservable().Select(v=>Convert(v));

        }

        static FileReloadCancelNotifyEventArgs Convert(SolidworksAddinFramework.Events.DPartDocEvents_Event.FileReloadCancelNotifyEventArgs source ){
            return new FileReloadCancelNotifyEventArgs(source.ErrorCode);;
        }
        static FileReloadCancelNotifyEventArgs Convert(SolidworksAddinFramework.Events.DAssemblyDocEvents_Event.FileReloadCancelNotifyEventArgs source ){
            return new FileReloadCancelNotifyEventArgs(source.ErrorCode);;
        }
        static FileReloadCancelNotifyEventArgs Convert(SolidworksAddinFramework.Events.DDrawingDocEvents_Event.FileReloadCancelNotifyEventArgs source ){
            return new FileReloadCancelNotifyEventArgs(source.ErrorCode);;
        }
        public class FileSavePostCancelNotifyEventArgs
        {
            public FileSavePostCancelNotifyEventArgs ()
            {
            }
        }
        public static IObservable<FileSavePostCancelNotifyEventArgs> FileSavePostCancelNotifyObservable(this IModelDoc2 modelDoc)
        {
            return
            (modelDoc as PartDoc)?.FileSavePostCancelNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as DrawingDoc)?.FileSavePostCancelNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as AssemblyDoc)?.FileSavePostCancelNotifyObservable().Select(v=>Convert(v));

        }

        static FileSavePostCancelNotifyEventArgs Convert(SolidworksAddinFramework.Events.DPartDocEvents_Event.FileSavePostCancelNotifyEventArgs source ){
            return new FileSavePostCancelNotifyEventArgs();;
        }
        static FileSavePostCancelNotifyEventArgs Convert(SolidworksAddinFramework.Events.DAssemblyDocEvents_Event.FileSavePostCancelNotifyEventArgs source ){
            return new FileSavePostCancelNotifyEventArgs();;
        }
        static FileSavePostCancelNotifyEventArgs Convert(SolidworksAddinFramework.Events.DDrawingDocEvents_Event.FileSavePostCancelNotifyEventArgs source ){
            return new FileSavePostCancelNotifyEventArgs();;
        }
        public class SketchSolveNotifyEventArgs
        {
            public SketchSolveNotifyEventArgs (System.String featName)
            {
                this.featName = featName;
            }
            public System.String featName { get; }
        }
        public static IObservable<SketchSolveNotifyEventArgs> SketchSolveNotifyObservable(this IModelDoc2 modelDoc)
        {
            return
            (modelDoc as PartDoc)?.SketchSolveNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as DrawingDoc)?.SketchSolveNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as AssemblyDoc)?.SketchSolveNotifyObservable().Select(v=>Convert(v));

        }

        static SketchSolveNotifyEventArgs Convert(SolidworksAddinFramework.Events.DPartDocEvents_Event.SketchSolveNotifyEventArgs source ){
            return new SketchSolveNotifyEventArgs(source.featName);;
        }
        static SketchSolveNotifyEventArgs Convert(SolidworksAddinFramework.Events.DAssemblyDocEvents_Event.SketchSolveNotifyEventArgs source ){
            return new SketchSolveNotifyEventArgs(source.featName);;
        }
        static SketchSolveNotifyEventArgs Convert(SolidworksAddinFramework.Events.DDrawingDocEvents_Event.SketchSolveNotifyEventArgs source ){
            return new SketchSolveNotifyEventArgs(source.featName);;
        }
        public class DeleteItemPreNotifyEventArgs
        {
            public DeleteItemPreNotifyEventArgs (System.Int32 EntityType, System.String itemName)
            {
                this.EntityType = EntityType;
                this.itemName = itemName;
            }
            public System.Int32 EntityType { get; }
            public System.String itemName { get; }
        }
        public static IObservable<DeleteItemPreNotifyEventArgs> DeleteItemPreNotifyObservable(this IModelDoc2 modelDoc)
        {
            return
            (modelDoc as PartDoc)?.DeleteItemPreNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as DrawingDoc)?.DeleteItemPreNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as AssemblyDoc)?.DeleteItemPreNotifyObservable().Select(v=>Convert(v));

        }

        static DeleteItemPreNotifyEventArgs Convert(SolidworksAddinFramework.Events.DPartDocEvents_Event.DeleteItemPreNotifyEventArgs source ){
            return new DeleteItemPreNotifyEventArgs(source.EntityType,source.itemName);;
        }
        static DeleteItemPreNotifyEventArgs Convert(SolidworksAddinFramework.Events.DAssemblyDocEvents_Event.DeleteItemPreNotifyEventArgs source ){
            return new DeleteItemPreNotifyEventArgs(source.EntityType,source.itemName);;
        }
        static DeleteItemPreNotifyEventArgs Convert(SolidworksAddinFramework.Events.DDrawingDocEvents_Event.DeleteItemPreNotifyEventArgs source ){
            return new DeleteItemPreNotifyEventArgs(source.EntityType,source.itemName);;
        }
        public class ClearSelectionsNotifyEventArgs
        {
            public ClearSelectionsNotifyEventArgs ()
            {
            }
        }
        public static IObservable<ClearSelectionsNotifyEventArgs> ClearSelectionsNotifyObservable(this IModelDoc2 modelDoc)
        {
            return
            (modelDoc as PartDoc)?.ClearSelectionsNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as DrawingDoc)?.ClearSelectionsNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as AssemblyDoc)?.ClearSelectionsNotifyObservable().Select(v=>Convert(v));

        }

        static ClearSelectionsNotifyEventArgs Convert(SolidworksAddinFramework.Events.DPartDocEvents_Event.ClearSelectionsNotifyEventArgs source ){
            return new ClearSelectionsNotifyEventArgs();;
        }
        static ClearSelectionsNotifyEventArgs Convert(SolidworksAddinFramework.Events.DAssemblyDocEvents_Event.ClearSelectionsNotifyEventArgs source ){
            return new ClearSelectionsNotifyEventArgs();;
        }
        static ClearSelectionsNotifyEventArgs Convert(SolidworksAddinFramework.Events.DDrawingDocEvents_Event.ClearSelectionsNotifyEventArgs source ){
            return new ClearSelectionsNotifyEventArgs();;
        }
        public class EquationEditorPreNotifyEventArgs
        {
            public EquationEditorPreNotifyEventArgs ()
            {
            }
        }
        public static IObservable<EquationEditorPreNotifyEventArgs> EquationEditorPreNotifyObservable(this IModelDoc2 modelDoc)
        {
            return
            (modelDoc as PartDoc)?.EquationEditorPreNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as DrawingDoc)?.EquationEditorPreNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as AssemblyDoc)?.EquationEditorPreNotifyObservable().Select(v=>Convert(v));

        }

        static EquationEditorPreNotifyEventArgs Convert(SolidworksAddinFramework.Events.DPartDocEvents_Event.EquationEditorPreNotifyEventArgs source ){
            return new EquationEditorPreNotifyEventArgs();;
        }
        static EquationEditorPreNotifyEventArgs Convert(SolidworksAddinFramework.Events.DAssemblyDocEvents_Event.EquationEditorPreNotifyEventArgs source ){
            return new EquationEditorPreNotifyEventArgs();;
        }
        static EquationEditorPreNotifyEventArgs Convert(SolidworksAddinFramework.Events.DDrawingDocEvents_Event.EquationEditorPreNotifyEventArgs source ){
            return new EquationEditorPreNotifyEventArgs();;
        }
        public class EquationEditorPostNotifyEventArgs
        {
            public EquationEditorPostNotifyEventArgs (System.Boolean Changed)
            {
                this.Changed = Changed;
            }
            public System.Boolean Changed { get; }
        }
        public static IObservable<EquationEditorPostNotifyEventArgs> EquationEditorPostNotifyObservable(this IModelDoc2 modelDoc)
        {
            return
            (modelDoc as PartDoc)?.EquationEditorPostNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as DrawingDoc)?.EquationEditorPostNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as AssemblyDoc)?.EquationEditorPostNotifyObservable().Select(v=>Convert(v));

        }

        static EquationEditorPostNotifyEventArgs Convert(SolidworksAddinFramework.Events.DPartDocEvents_Event.EquationEditorPostNotifyEventArgs source ){
            return new EquationEditorPostNotifyEventArgs(source.Changed);;
        }
        static EquationEditorPostNotifyEventArgs Convert(SolidworksAddinFramework.Events.DAssemblyDocEvents_Event.EquationEditorPostNotifyEventArgs source ){
            return new EquationEditorPostNotifyEventArgs(source.Changed);;
        }
        static EquationEditorPostNotifyEventArgs Convert(SolidworksAddinFramework.Events.DDrawingDocEvents_Event.EquationEditorPostNotifyEventArgs source ){
            return new EquationEditorPostNotifyEventArgs(source.Changed);;
        }
        public class UnitsChangeNotifyEventArgs
        {
            public UnitsChangeNotifyEventArgs ()
            {
            }
        }
        public static IObservable<UnitsChangeNotifyEventArgs> UnitsChangeNotifyObservable(this IModelDoc2 modelDoc)
        {
            return
            (modelDoc as PartDoc)?.UnitsChangeNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as DrawingDoc)?.UnitsChangeNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as AssemblyDoc)?.UnitsChangeNotifyObservable().Select(v=>Convert(v));

        }

        static UnitsChangeNotifyEventArgs Convert(SolidworksAddinFramework.Events.DPartDocEvents_Event.UnitsChangeNotifyEventArgs source ){
            return new UnitsChangeNotifyEventArgs();;
        }
        static UnitsChangeNotifyEventArgs Convert(SolidworksAddinFramework.Events.DAssemblyDocEvents_Event.UnitsChangeNotifyEventArgs source ){
            return new UnitsChangeNotifyEventArgs();;
        }
        static UnitsChangeNotifyEventArgs Convert(SolidworksAddinFramework.Events.DDrawingDocEvents_Event.UnitsChangeNotifyEventArgs source ){
            return new UnitsChangeNotifyEventArgs();;
        }
        public class DestroyNotify2EventArgs
        {
            public DestroyNotify2EventArgs (System.Int32 DestroyType)
            {
                this.DestroyType = DestroyType;
            }
            public System.Int32 DestroyType { get; }
        }
        public static IObservable<DestroyNotify2EventArgs> DestroyNotify2Observable(this IModelDoc2 modelDoc)
        {
            return
            (modelDoc as PartDoc)?.DestroyNotify2Observable().Select(v=>Convert(v))
                ??
            (modelDoc as DrawingDoc)?.DestroyNotify2Observable().Select(v=>Convert(v))
                ??
            (modelDoc as AssemblyDoc)?.DestroyNotify2Observable().Select(v=>Convert(v));

        }

        static DestroyNotify2EventArgs Convert(SolidworksAddinFramework.Events.DPartDocEvents_Event.DestroyNotify2EventArgs source ){
            return new DestroyNotify2EventArgs(source.DestroyType);;
        }
        static DestroyNotify2EventArgs Convert(SolidworksAddinFramework.Events.DAssemblyDocEvents_Event.DestroyNotify2EventArgs source ){
            return new DestroyNotify2EventArgs(source.DestroyType);;
        }
        static DestroyNotify2EventArgs Convert(SolidworksAddinFramework.Events.DDrawingDocEvents_Event.DestroyNotify2EventArgs source ){
            return new DestroyNotify2EventArgs(source.DestroyType);;
        }
        public class AutoSaveNotifyEventArgs
        {
            public AutoSaveNotifyEventArgs (System.String FileName)
            {
                this.FileName = FileName;
            }
            public System.String FileName { get; }
        }
        public static IObservable<AutoSaveNotifyEventArgs> AutoSaveNotifyObservable(this IModelDoc2 modelDoc)
        {
            return
            (modelDoc as PartDoc)?.AutoSaveNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as DrawingDoc)?.AutoSaveNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as AssemblyDoc)?.AutoSaveNotifyObservable().Select(v=>Convert(v));

        }

        static AutoSaveNotifyEventArgs Convert(SolidworksAddinFramework.Events.DPartDocEvents_Event.AutoSaveNotifyEventArgs source ){
            return new AutoSaveNotifyEventArgs(source.FileName);;
        }
        static AutoSaveNotifyEventArgs Convert(SolidworksAddinFramework.Events.DAssemblyDocEvents_Event.AutoSaveNotifyEventArgs source ){
            return new AutoSaveNotifyEventArgs(source.FileName);;
        }
        static AutoSaveNotifyEventArgs Convert(SolidworksAddinFramework.Events.DDrawingDocEvents_Event.AutoSaveNotifyEventArgs source ){
            return new AutoSaveNotifyEventArgs(source.FileName);;
        }
        public class AutoSaveToStorageNotifyEventArgs
        {
            public AutoSaveToStorageNotifyEventArgs ()
            {
            }
        }
        public static IObservable<AutoSaveToStorageNotifyEventArgs> AutoSaveToStorageNotifyObservable(this IModelDoc2 modelDoc)
        {
            return
            (modelDoc as PartDoc)?.AutoSaveToStorageNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as DrawingDoc)?.AutoSaveToStorageNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as AssemblyDoc)?.AutoSaveToStorageNotifyObservable().Select(v=>Convert(v));

        }

        static AutoSaveToStorageNotifyEventArgs Convert(SolidworksAddinFramework.Events.DPartDocEvents_Event.AutoSaveToStorageNotifyEventArgs source ){
            return new AutoSaveToStorageNotifyEventArgs();;
        }
        static AutoSaveToStorageNotifyEventArgs Convert(SolidworksAddinFramework.Events.DAssemblyDocEvents_Event.AutoSaveToStorageNotifyEventArgs source ){
            return new AutoSaveToStorageNotifyEventArgs();;
        }
        static AutoSaveToStorageNotifyEventArgs Convert(SolidworksAddinFramework.Events.DDrawingDocEvents_Event.AutoSaveToStorageNotifyEventArgs source ){
            return new AutoSaveToStorageNotifyEventArgs();;
        }
        public class UndoPostNotifyEventArgs
        {
            public UndoPostNotifyEventArgs ()
            {
            }
        }
        public static IObservable<UndoPostNotifyEventArgs> UndoPostNotifyObservable(this IModelDoc2 modelDoc)
        {
            return
            (modelDoc as PartDoc)?.UndoPostNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as DrawingDoc)?.UndoPostNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as AssemblyDoc)?.UndoPostNotifyObservable().Select(v=>Convert(v));

        }

        static UndoPostNotifyEventArgs Convert(SolidworksAddinFramework.Events.DPartDocEvents_Event.UndoPostNotifyEventArgs source ){
            return new UndoPostNotifyEventArgs();;
        }
        static UndoPostNotifyEventArgs Convert(SolidworksAddinFramework.Events.DAssemblyDocEvents_Event.UndoPostNotifyEventArgs source ){
            return new UndoPostNotifyEventArgs();;
        }
        static UndoPostNotifyEventArgs Convert(SolidworksAddinFramework.Events.DDrawingDocEvents_Event.UndoPostNotifyEventArgs source ){
            return new UndoPostNotifyEventArgs();;
        }
        public class UserSelectionPreNotifyEventArgs
        {
            public UserSelectionPreNotifyEventArgs (System.Int32 SelType)
            {
                this.SelType = SelType;
            }
            public System.Int32 SelType { get; }
        }
        public static IObservable<UserSelectionPreNotifyEventArgs> UserSelectionPreNotifyObservable(this IModelDoc2 modelDoc)
        {
            return
            (modelDoc as PartDoc)?.UserSelectionPreNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as DrawingDoc)?.UserSelectionPreNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as AssemblyDoc)?.UserSelectionPreNotifyObservable().Select(v=>Convert(v));

        }

        static UserSelectionPreNotifyEventArgs Convert(SolidworksAddinFramework.Events.DPartDocEvents_Event.UserSelectionPreNotifyEventArgs source ){
            return new UserSelectionPreNotifyEventArgs(source.SelType);;
        }
        static UserSelectionPreNotifyEventArgs Convert(SolidworksAddinFramework.Events.DAssemblyDocEvents_Event.UserSelectionPreNotifyEventArgs source ){
            return new UserSelectionPreNotifyEventArgs(source.SelType);;
        }
        static UserSelectionPreNotifyEventArgs Convert(SolidworksAddinFramework.Events.DDrawingDocEvents_Event.UserSelectionPreNotifyEventArgs source ){
            return new UserSelectionPreNotifyEventArgs(source.SelType);;
        }
        public class RedoPostNotifyEventArgs
        {
            public RedoPostNotifyEventArgs ()
            {
            }
        }
        public static IObservable<RedoPostNotifyEventArgs> RedoPostNotifyObservable(this IModelDoc2 modelDoc)
        {
            return
            (modelDoc as PartDoc)?.RedoPostNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as DrawingDoc)?.RedoPostNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as AssemblyDoc)?.RedoPostNotifyObservable().Select(v=>Convert(v));

        }

        static RedoPostNotifyEventArgs Convert(SolidworksAddinFramework.Events.DPartDocEvents_Event.RedoPostNotifyEventArgs source ){
            return new RedoPostNotifyEventArgs();;
        }
        static RedoPostNotifyEventArgs Convert(SolidworksAddinFramework.Events.DAssemblyDocEvents_Event.RedoPostNotifyEventArgs source ){
            return new RedoPostNotifyEventArgs();;
        }
        static RedoPostNotifyEventArgs Convert(SolidworksAddinFramework.Events.DDrawingDocEvents_Event.RedoPostNotifyEventArgs source ){
            return new RedoPostNotifyEventArgs();;
        }
        public class RedoPreNotifyEventArgs
        {
            public RedoPreNotifyEventArgs ()
            {
            }
        }
        public static IObservable<RedoPreNotifyEventArgs> RedoPreNotifyObservable(this IModelDoc2 modelDoc)
        {
            return
            (modelDoc as PartDoc)?.RedoPreNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as DrawingDoc)?.RedoPreNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as AssemblyDoc)?.RedoPreNotifyObservable().Select(v=>Convert(v));

        }

        static RedoPreNotifyEventArgs Convert(SolidworksAddinFramework.Events.DPartDocEvents_Event.RedoPreNotifyEventArgs source ){
            return new RedoPreNotifyEventArgs();;
        }
        static RedoPreNotifyEventArgs Convert(SolidworksAddinFramework.Events.DAssemblyDocEvents_Event.RedoPreNotifyEventArgs source ){
            return new RedoPreNotifyEventArgs();;
        }
        static RedoPreNotifyEventArgs Convert(SolidworksAddinFramework.Events.DDrawingDocEvents_Event.RedoPreNotifyEventArgs source ){
            return new RedoPreNotifyEventArgs();;
        }
        public class UndoPreNotifyEventArgs
        {
            public UndoPreNotifyEventArgs ()
            {
            }
        }
        public static IObservable<UndoPreNotifyEventArgs> UndoPreNotifyObservable(this IModelDoc2 modelDoc)
        {
            return
            (modelDoc as PartDoc)?.UndoPreNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as DrawingDoc)?.UndoPreNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as AssemblyDoc)?.UndoPreNotifyObservable().Select(v=>Convert(v));

        }

        static UndoPreNotifyEventArgs Convert(SolidworksAddinFramework.Events.DPartDocEvents_Event.UndoPreNotifyEventArgs source ){
            return new UndoPreNotifyEventArgs();;
        }
        static UndoPreNotifyEventArgs Convert(SolidworksAddinFramework.Events.DAssemblyDocEvents_Event.UndoPreNotifyEventArgs source ){
            return new UndoPreNotifyEventArgs();;
        }
        static UndoPreNotifyEventArgs Convert(SolidworksAddinFramework.Events.DDrawingDocEvents_Event.UndoPreNotifyEventArgs source ){
            return new UndoPreNotifyEventArgs();;
        }
        public class AutoSaveToStorageStoreNotifyEventArgs
        {
            public AutoSaveToStorageStoreNotifyEventArgs ()
            {
            }
        }
        public static IObservable<AutoSaveToStorageStoreNotifyEventArgs> AutoSaveToStorageStoreNotifyObservable(this IModelDoc2 modelDoc)
        {
            return
            (modelDoc as PartDoc)?.AutoSaveToStorageStoreNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as DrawingDoc)?.AutoSaveToStorageStoreNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as AssemblyDoc)?.AutoSaveToStorageStoreNotifyObservable().Select(v=>Convert(v));

        }

        static AutoSaveToStorageStoreNotifyEventArgs Convert(SolidworksAddinFramework.Events.DPartDocEvents_Event.AutoSaveToStorageStoreNotifyEventArgs source ){
            return new AutoSaveToStorageStoreNotifyEventArgs();;
        }
        static AutoSaveToStorageStoreNotifyEventArgs Convert(SolidworksAddinFramework.Events.DAssemblyDocEvents_Event.AutoSaveToStorageStoreNotifyEventArgs source ){
            return new AutoSaveToStorageStoreNotifyEventArgs();;
        }
        static AutoSaveToStorageStoreNotifyEventArgs Convert(SolidworksAddinFramework.Events.DDrawingDocEvents_Event.AutoSaveToStorageStoreNotifyEventArgs source ){
            return new AutoSaveToStorageStoreNotifyEventArgs();;
        }
        public class ModifyTableNotifyEventArgs
        {
            public ModifyTableNotifyEventArgs (SolidWorks.Interop.sldworks.TableAnnotation TableAnnotation, System.Int32 TableType, System.Int32 reason, System.Int32 RowInfo, System.Int32 ColumnInfo, System.String DataInfo)
            {
                this.TableAnnotation = TableAnnotation;
                this.TableType = TableType;
                this.reason = reason;
                this.RowInfo = RowInfo;
                this.ColumnInfo = ColumnInfo;
                this.DataInfo = DataInfo;
            }
            public SolidWorks.Interop.sldworks.TableAnnotation TableAnnotation { get; }
            public System.Int32 TableType { get; }
            public System.Int32 reason { get; }
            public System.Int32 RowInfo { get; }
            public System.Int32 ColumnInfo { get; }
            public System.String DataInfo { get; }
        }
        public static IObservable<ModifyTableNotifyEventArgs> ModifyTableNotifyObservable(this IModelDoc2 modelDoc)
        {
            return
            (modelDoc as PartDoc)?.ModifyTableNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as DrawingDoc)?.ModifyTableNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as AssemblyDoc)?.ModifyTableNotifyObservable().Select(v=>Convert(v));

        }

        static ModifyTableNotifyEventArgs Convert(SolidworksAddinFramework.Events.DPartDocEvents_Event.ModifyTableNotifyEventArgs source ){
            return new ModifyTableNotifyEventArgs(source.TableAnnotation,source.TableType,source.reason,source.RowInfo,source.ColumnInfo,source.DataInfo);;
        }
        static ModifyTableNotifyEventArgs Convert(SolidworksAddinFramework.Events.DAssemblyDocEvents_Event.ModifyTableNotifyEventArgs source ){
            return new ModifyTableNotifyEventArgs(source.TableAnnotation,source.TableType,source.reason,source.RowInfo,source.ColumnInfo,source.DataInfo);;
        }
        static ModifyTableNotifyEventArgs Convert(SolidworksAddinFramework.Events.DDrawingDocEvents_Event.ModifyTableNotifyEventArgs source ){
            return new ModifyTableNotifyEventArgs(source.TableAnnotation,source.TableType,source.reason,source.RowInfo,source.ColumnInfo,source.DataInfo);;
        }
        public class UserSelectionPostNotifyEventArgs
        {
            public UserSelectionPostNotifyEventArgs ()
            {
            }
        }
        public static IObservable<UserSelectionPostNotifyEventArgs> UserSelectionPostNotifyObservable(this IModelDoc2 modelDoc)
        {
            return
            (modelDoc as PartDoc)?.UserSelectionPostNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as DrawingDoc)?.UserSelectionPostNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as AssemblyDoc)?.UserSelectionPostNotifyObservable().Select(v=>Convert(v));

        }

        static UserSelectionPostNotifyEventArgs Convert(SolidworksAddinFramework.Events.DPartDocEvents_Event.UserSelectionPostNotifyEventArgs source ){
            return new UserSelectionPostNotifyEventArgs();;
        }
        static UserSelectionPostNotifyEventArgs Convert(SolidworksAddinFramework.Events.DAssemblyDocEvents_Event.UserSelectionPostNotifyEventArgs source ){
            return new UserSelectionPostNotifyEventArgs();;
        }
        static UserSelectionPostNotifyEventArgs Convert(SolidworksAddinFramework.Events.DDrawingDocEvents_Event.UserSelectionPostNotifyEventArgs source ){
            return new UserSelectionPostNotifyEventArgs();;
        }
        public class CommandManagerTabActivatedPreNotifyEventArgs
        {
            public CommandManagerTabActivatedPreNotifyEventArgs (System.Int32 CommandTabIndex, System.String CommandTabName)
            {
                this.CommandTabIndex = CommandTabIndex;
                this.CommandTabName = CommandTabName;
            }
            public System.Int32 CommandTabIndex { get; }
            public System.String CommandTabName { get; }
        }
        public static IObservable<CommandManagerTabActivatedPreNotifyEventArgs> CommandManagerTabActivatedPreNotifyObservable(this IModelDoc2 modelDoc)
        {
            return
            (modelDoc as PartDoc)?.CommandManagerTabActivatedPreNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as DrawingDoc)?.CommandManagerTabActivatedPreNotifyObservable().Select(v=>Convert(v))
                ??
            (modelDoc as AssemblyDoc)?.CommandManagerTabActivatedPreNotifyObservable().Select(v=>Convert(v));

        }

        static CommandManagerTabActivatedPreNotifyEventArgs Convert(SolidworksAddinFramework.Events.DPartDocEvents_Event.CommandManagerTabActivatedPreNotifyEventArgs source ){
            return new CommandManagerTabActivatedPreNotifyEventArgs(source.CommandTabIndex,source.CommandTabName);;
        }
        static CommandManagerTabActivatedPreNotifyEventArgs Convert(SolidworksAddinFramework.Events.DAssemblyDocEvents_Event.CommandManagerTabActivatedPreNotifyEventArgs source ){
            return new CommandManagerTabActivatedPreNotifyEventArgs(source.CommandTabIndex,source.CommandTabName);;
        }
        static CommandManagerTabActivatedPreNotifyEventArgs Convert(SolidworksAddinFramework.Events.DDrawingDocEvents_Event.CommandManagerTabActivatedPreNotifyEventArgs source ){
            return new CommandManagerTabActivatedPreNotifyEventArgs(source.CommandTabIndex,source.CommandTabName);;
        }
}
}

