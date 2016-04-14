
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
	public static class DPartDocEvents_EventObservables {


		public class RegenNotifyEventArgs {
					public RegenNotifyEventArgs(){
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_RegenNotifyEventHandler.html
		public static IObservable<RegenNotifyEventArgs> RegenNotifyObservable(this PartDoc partDoc)
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

					partDoc.RegenNotify += callback;
					return Disposable.Create(()=> partDoc.RegenNotify-= callback);
					
				}
			);
		}


		public class DestroyNotifyEventArgs {
					public DestroyNotifyEventArgs(){
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_DestroyNotifyEventHandler.html
		public static IObservable<DestroyNotifyEventArgs> DestroyNotifyObservable(this PartDoc partDoc)
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

					partDoc.DestroyNotify += callback;
					return Disposable.Create(()=> partDoc.DestroyNotify-= callback);
					
				}
			);
		}


		public class RegenPostNotifyEventArgs {
					public RegenPostNotifyEventArgs(){
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_RegenPostNotifyEventHandler.html
		public static IObservable<RegenPostNotifyEventArgs> RegenPostNotifyObservable(this PartDoc partDoc)
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

					partDoc.RegenPostNotify += callback;
					return Disposable.Create(()=> partDoc.RegenPostNotify-= callback);
					
				}
			);
		}


		public class ViewNewNotifyEventArgs {
					public ViewNewNotifyEventArgs(){
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_ViewNewNotifyEventHandler.html
		public static IObservable<ViewNewNotifyEventArgs> ViewNewNotifyObservable(this PartDoc partDoc)
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

					partDoc.ViewNewNotify += callback;
					return Disposable.Create(()=> partDoc.ViewNewNotify-= callback);
					
				}
			);
		}


		public class NewSelectionNotifyEventArgs {
					public NewSelectionNotifyEventArgs(){
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_NewSelectionNotifyEventHandler.html
		public static IObservable<NewSelectionNotifyEventArgs> NewSelectionNotifyObservable(this PartDoc partDoc)
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

					partDoc.NewSelectionNotify += callback;
					return Disposable.Create(()=> partDoc.NewSelectionNotify-= callback);
					
				}
			);
		}


		public class FileSaveNotifyEventArgs {
					public String FileName {get;}
					public FileSaveNotifyEventArgs(System.String FileName){
					this.FileName=FileName;
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_FileSaveNotifyEventHandler.html
		public static IObservable<FileSaveNotifyEventArgs> FileSaveNotifyObservable(this PartDoc partDoc)
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

					partDoc.FileSaveNotify += callback;
					return Disposable.Create(()=> partDoc.FileSaveNotify-= callback);
					
				}
			);
		}


		public class FileSaveAsNotifyEventArgs {
					public String FileName {get;}
					public FileSaveAsNotifyEventArgs(System.String FileName){
					this.FileName=FileName;
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_FileSaveAsNotifyEventHandler.html
		public static IObservable<FileSaveAsNotifyEventArgs> FileSaveAsNotifyObservable(this PartDoc partDoc)
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

					partDoc.FileSaveAsNotify += callback;
					return Disposable.Create(()=> partDoc.FileSaveAsNotify-= callback);
					
				}
			);
		}


		public class LoadFromStorageNotifyEventArgs {
					public LoadFromStorageNotifyEventArgs(){
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_LoadFromStorageNotifyEventHandler.html
		public static IObservable<LoadFromStorageNotifyEventArgs> LoadFromStorageNotifyObservable(this PartDoc partDoc)
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

					partDoc.LoadFromStorageNotify += callback;
					return Disposable.Create(()=> partDoc.LoadFromStorageNotify-= callback);
					
				}
			);
		}


		public class SaveToStorageNotifyEventArgs {
					public SaveToStorageNotifyEventArgs(){
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_SaveToStorageNotifyEventHandler.html
		public static IObservable<SaveToStorageNotifyEventArgs> SaveToStorageNotifyObservable(this PartDoc partDoc)
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

					partDoc.SaveToStorageNotify += callback;
					return Disposable.Create(()=> partDoc.SaveToStorageNotify-= callback);
					
				}
			);
		}


		public class ActiveConfigChangeNotifyEventArgs {
					public ActiveConfigChangeNotifyEventArgs(){
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_ActiveConfigChangeNotifyEventHandler.html
		public static IObservable<ActiveConfigChangeNotifyEventArgs> ActiveConfigChangeNotifyObservable(this PartDoc partDoc)
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

					partDoc.ActiveConfigChangeNotify += callback;
					return Disposable.Create(()=> partDoc.ActiveConfigChangeNotify-= callback);
					
				}
			);
		}


		public class ActiveConfigChangePostNotifyEventArgs {
					public ActiveConfigChangePostNotifyEventArgs(){
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_ActiveConfigChangePostNotifyEventHandler.html
		public static IObservable<ActiveConfigChangePostNotifyEventArgs> ActiveConfigChangePostNotifyObservable(this PartDoc partDoc)
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

					partDoc.ActiveConfigChangePostNotify += callback;
					return Disposable.Create(()=> partDoc.ActiveConfigChangePostNotify-= callback);
					
				}
			);
		}


		public class ViewNewNotify2EventArgs {
					public Object viewBeingAdded {get;}
					public ViewNewNotify2EventArgs(System.Object viewBeingAdded){
					this.viewBeingAdded=viewBeingAdded;
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_ViewNewNotify2EventHandler.html
		public static IObservable<ViewNewNotify2EventArgs> ViewNewNotify2Observable(this PartDoc partDoc)
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

					partDoc.ViewNewNotify2 += callback;
					return Disposable.Create(()=> partDoc.ViewNewNotify2-= callback);
					
				}
			);
		}


		public class LightingDialogCreateNotifyEventArgs {
					public Object dialog {get;}
					public LightingDialogCreateNotifyEventArgs(System.Object dialog){
					this.dialog=dialog;
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_LightingDialogCreateNotifyEventHandler.html
		public static IObservable<LightingDialogCreateNotifyEventArgs> LightingDialogCreateNotifyObservable(this PartDoc partDoc)
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

					partDoc.LightingDialogCreateNotify += callback;
					return Disposable.Create(()=> partDoc.LightingDialogCreateNotify-= callback);
					
				}
			);
		}


		public class AddItemNotifyEventArgs {
					public Int32 EntityType {get;}
					public String itemName {get;}
					public AddItemNotifyEventArgs(System.Int32 EntityType, System.String itemName){
					this.EntityType=EntityType;
					this.itemName=itemName;
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_AddItemNotifyEventHandler.html
		public static IObservable<AddItemNotifyEventArgs> AddItemNotifyObservable(this PartDoc partDoc)
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

					partDoc.AddItemNotify += callback;
					return Disposable.Create(()=> partDoc.AddItemNotify-= callback);
					
				}
			);
		}


		public class RenameItemNotifyEventArgs {
					public Int32 EntityType {get;}
					public String oldName {get;}
					public String NewName {get;}
					public RenameItemNotifyEventArgs(System.Int32 EntityType, System.String oldName, System.String NewName){
					this.EntityType=EntityType;
					this.oldName=oldName;
					this.NewName=NewName;
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_RenameItemNotifyEventHandler.html
		public static IObservable<RenameItemNotifyEventArgs> RenameItemNotifyObservable(this PartDoc partDoc)
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

					partDoc.RenameItemNotify += callback;
					return Disposable.Create(()=> partDoc.RenameItemNotify-= callback);
					
				}
			);
		}


		public class DeleteItemNotifyEventArgs {
					public Int32 EntityType {get;}
					public String itemName {get;}
					public DeleteItemNotifyEventArgs(System.Int32 EntityType, System.String itemName){
					this.EntityType=EntityType;
					this.itemName=itemName;
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_DeleteItemNotifyEventHandler.html
		public static IObservable<DeleteItemNotifyEventArgs> DeleteItemNotifyObservable(this PartDoc partDoc)
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

					partDoc.DeleteItemNotify += callback;
					return Disposable.Create(()=> partDoc.DeleteItemNotify-= callback);
					
				}
			);
		}


		public class ModifyNotifyEventArgs {
					public ModifyNotifyEventArgs(){
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_ModifyNotifyEventHandler.html
		public static IObservable<ModifyNotifyEventArgs> ModifyNotifyObservable(this PartDoc partDoc)
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

					partDoc.ModifyNotify += callback;
					return Disposable.Create(()=> partDoc.ModifyNotify-= callback);
					
				}
			);
		}


		public class FileReloadNotifyEventArgs {
					public FileReloadNotifyEventArgs(){
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_FileReloadNotifyEventHandler.html
		public static IObservable<FileReloadNotifyEventArgs> FileReloadNotifyObservable(this PartDoc partDoc)
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

					partDoc.FileReloadNotify += callback;
					return Disposable.Create(()=> partDoc.FileReloadNotify-= callback);
					
				}
			);
		}


		public class AddCustomPropertyNotifyEventArgs {
					public String propName {get;}
					public String Configuration {get;}
					public String Value {get;}
					public Int32 valueType {get;}
					public AddCustomPropertyNotifyEventArgs(System.String propName, System.String Configuration, System.String Value, System.Int32 valueType){
					this.propName=propName;
					this.Configuration=Configuration;
					this.Value=Value;
					this.valueType=valueType;
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_AddCustomPropertyNotifyEventHandler.html
		public static IObservable<AddCustomPropertyNotifyEventArgs> AddCustomPropertyNotifyObservable(this PartDoc partDoc)
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

					partDoc.AddCustomPropertyNotify += callback;
					return Disposable.Create(()=> partDoc.AddCustomPropertyNotify-= callback);
					
				}
			);
		}


		public class ChangeCustomPropertyNotifyEventArgs {
					public String propName {get;}
					public String Configuration {get;}
					public String oldValue {get;}
					public String NewValue {get;}
					public Int32 valueType {get;}
					public ChangeCustomPropertyNotifyEventArgs(System.String propName, System.String Configuration, System.String oldValue, System.String NewValue, System.Int32 valueType){
					this.propName=propName;
					this.Configuration=Configuration;
					this.oldValue=oldValue;
					this.NewValue=NewValue;
					this.valueType=valueType;
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_ChangeCustomPropertyNotifyEventHandler.html
		public static IObservable<ChangeCustomPropertyNotifyEventArgs> ChangeCustomPropertyNotifyObservable(this PartDoc partDoc)
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

					partDoc.ChangeCustomPropertyNotify += callback;
					return Disposable.Create(()=> partDoc.ChangeCustomPropertyNotify-= callback);
					
				}
			);
		}


		public class DeleteCustomPropertyNotifyEventArgs {
					public String propName {get;}
					public String Configuration {get;}
					public String Value {get;}
					public Int32 valueType {get;}
					public DeleteCustomPropertyNotifyEventArgs(System.String propName, System.String Configuration, System.String Value, System.Int32 valueType){
					this.propName=propName;
					this.Configuration=Configuration;
					this.Value=Value;
					this.valueType=valueType;
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_DeleteCustomPropertyNotifyEventHandler.html
		public static IObservable<DeleteCustomPropertyNotifyEventArgs> DeleteCustomPropertyNotifyObservable(this PartDoc partDoc)
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

					partDoc.DeleteCustomPropertyNotify += callback;
					return Disposable.Create(()=> partDoc.DeleteCustomPropertyNotify-= callback);
					
				}
			);
		}


		public class FeatureEditPreNotifyEventArgs {
					public Object EditFeature {get;}
					public FeatureEditPreNotifyEventArgs(System.Object EditFeature){
					this.EditFeature=EditFeature;
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_FeatureEditPreNotifyEventHandler.html
		public static IObservable<FeatureEditPreNotifyEventArgs> FeatureEditPreNotifyObservable(this PartDoc partDoc)
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

					partDoc.FeatureEditPreNotify += callback;
					return Disposable.Create(()=> partDoc.FeatureEditPreNotify-= callback);
					
				}
			);
		}


		public class FeatureSketchEditPreNotifyEventArgs {
					public Object EditFeature {get;}
					public Object featureSketch {get;}
					public FeatureSketchEditPreNotifyEventArgs(System.Object EditFeature, System.Object featureSketch){
					this.EditFeature=EditFeature;
					this.featureSketch=featureSketch;
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_FeatureSketchEditPreNotifyEventHandler.html
		public static IObservable<FeatureSketchEditPreNotifyEventArgs> FeatureSketchEditPreNotifyObservable(this PartDoc partDoc)
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

					partDoc.FeatureSketchEditPreNotify += callback;
					return Disposable.Create(()=> partDoc.FeatureSketchEditPreNotify-= callback);
					
				}
			);
		}


		public class FileSaveAsNotify2EventArgs {
					public String FileName {get;}
					public FileSaveAsNotify2EventArgs(System.String FileName){
					this.FileName=FileName;
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_FileSaveAsNotify2EventHandler.html
		public static IObservable<FileSaveAsNotify2EventArgs> FileSaveAsNotify2Observable(this PartDoc partDoc)
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

					partDoc.FileSaveAsNotify2 += callback;
					return Disposable.Create(()=> partDoc.FileSaveAsNotify2-= callback);
					
				}
			);
		}


		public class DeleteSelectionPreNotifyEventArgs {
					public DeleteSelectionPreNotifyEventArgs(){
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_DeleteSelectionPreNotifyEventHandler.html
		public static IObservable<DeleteSelectionPreNotifyEventArgs> DeleteSelectionPreNotifyObservable(this PartDoc partDoc)
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

					partDoc.DeleteSelectionPreNotify += callback;
					return Disposable.Create(()=> partDoc.DeleteSelectionPreNotify-= callback);
					
				}
			);
		}


		public class FileReloadPreNotifyEventArgs {
					public FileReloadPreNotifyEventArgs(){
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_FileReloadPreNotifyEventHandler.html
		public static IObservable<FileReloadPreNotifyEventArgs> FileReloadPreNotifyObservable(this PartDoc partDoc)
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

					partDoc.FileReloadPreNotify += callback;
					return Disposable.Create(()=> partDoc.FileReloadPreNotify-= callback);
					
				}
			);
		}


		public class BodyVisibleChangeNotifyEventArgs {
					public BodyVisibleChangeNotifyEventArgs(){
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_BodyVisibleChangeNotifyEventHandler.html
		public static IObservable<BodyVisibleChangeNotifyEventArgs> BodyVisibleChangeNotifyObservable(this PartDoc partDoc)
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

					partDoc.BodyVisibleChangeNotify += callback;
					return Disposable.Create(()=> partDoc.BodyVisibleChangeNotify-= callback);
					
				}
			);
		}


		public class RegenPostNotify2EventArgs {
					public Object stopFeature {get;}
					public RegenPostNotify2EventArgs(System.Object stopFeature){
					this.stopFeature=stopFeature;
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_RegenPostNotify2EventHandler.html
		public static IObservable<RegenPostNotify2EventArgs> RegenPostNotify2Observable(this PartDoc partDoc)
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

					partDoc.RegenPostNotify2 += callback;
					return Disposable.Create(()=> partDoc.RegenPostNotify2-= callback);
					
				}
			);
		}


		public class FileSavePostNotifyEventArgs {
					public Int32 saveType {get;}
					public String FileName {get;}
					public FileSavePostNotifyEventArgs(System.Int32 saveType, System.String FileName){
					this.saveType=saveType;
					this.FileName=FileName;
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_FileSavePostNotifyEventHandler.html
		public static IObservable<FileSavePostNotifyEventArgs> FileSavePostNotifyObservable(this PartDoc partDoc)
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

					partDoc.FileSavePostNotify += callback;
					return Disposable.Create(()=> partDoc.FileSavePostNotify-= callback);
					
				}
			);
		}


		public class LoadFromStorageStoreNotifyEventArgs {
					public LoadFromStorageStoreNotifyEventArgs(){
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_LoadFromStorageStoreNotifyEventHandler.html
		public static IObservable<LoadFromStorageStoreNotifyEventArgs> LoadFromStorageStoreNotifyObservable(this PartDoc partDoc)
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

					partDoc.LoadFromStorageStoreNotify += callback;
					return Disposable.Create(()=> partDoc.LoadFromStorageStoreNotify-= callback);
					
				}
			);
		}


		public class SaveToStorageStoreNotifyEventArgs {
					public SaveToStorageStoreNotifyEventArgs(){
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_SaveToStorageStoreNotifyEventHandler.html
		public static IObservable<SaveToStorageStoreNotifyEventArgs> SaveToStorageStoreNotifyObservable(this PartDoc partDoc)
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

					partDoc.SaveToStorageStoreNotify += callback;
					return Disposable.Create(()=> partDoc.SaveToStorageStoreNotify-= callback);
					
				}
			);
		}


		public class FeatureManagerTreeRebuildNotifyEventArgs {
					public FeatureManagerTreeRebuildNotifyEventArgs(){
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_FeatureManagerTreeRebuildNotifyEventHandler.html
		public static IObservable<FeatureManagerTreeRebuildNotifyEventArgs> FeatureManagerTreeRebuildNotifyObservable(this PartDoc partDoc)
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

					partDoc.FeatureManagerTreeRebuildNotify += callback;
					return Disposable.Create(()=> partDoc.FeatureManagerTreeRebuildNotify-= callback);
					
				}
			);
		}


		public class FileDropPostNotifyEventArgs {
					public String FileName {get;}
					public FileDropPostNotifyEventArgs(System.String FileName){
					this.FileName=FileName;
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_FileDropPostNotifyEventHandler.html
		public static IObservable<FileDropPostNotifyEventArgs> FileDropPostNotifyObservable(this PartDoc partDoc)
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

					partDoc.FileDropPostNotify += callback;
					return Disposable.Create(()=> partDoc.FileDropPostNotify-= callback);
					
				}
			);
		}


		public class DynamicHighlightNotifyEventArgs {
					public Boolean bHighlightState {get;}
					public DynamicHighlightNotifyEventArgs(System.Boolean bHighlightState){
					this.bHighlightState=bHighlightState;
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_DynamicHighlightNotifyEventHandler.html
		public static IObservable<DynamicHighlightNotifyEventArgs> DynamicHighlightNotifyObservable(this PartDoc partDoc)
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

					partDoc.DynamicHighlightNotify += callback;
					return Disposable.Create(()=> partDoc.DynamicHighlightNotify-= callback);
					
				}
			);
		}


		public class DimensionChangeNotifyEventArgs {
					public Object displayDim {get;}
					public DimensionChangeNotifyEventArgs(System.Object displayDim){
					this.displayDim=displayDim;
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_DimensionChangeNotifyEventHandler.html
		public static IObservable<DimensionChangeNotifyEventArgs> DimensionChangeNotifyObservable(this PartDoc partDoc)
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

					partDoc.DimensionChangeNotify += callback;
					return Disposable.Create(()=> partDoc.DimensionChangeNotify-= callback);
					
				}
			);
		}


		public class FileReloadCancelNotifyEventArgs {
					public Int32 ErrorCode {get;}
					public FileReloadCancelNotifyEventArgs(System.Int32 ErrorCode){
					this.ErrorCode=ErrorCode;
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_FileReloadCancelNotifyEventHandler.html
		public static IObservable<FileReloadCancelNotifyEventArgs> FileReloadCancelNotifyObservable(this PartDoc partDoc)
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

					partDoc.FileReloadCancelNotify += callback;
					return Disposable.Create(()=> partDoc.FileReloadCancelNotify-= callback);
					
				}
			);
		}


		public class FileSavePostCancelNotifyEventArgs {
					public FileSavePostCancelNotifyEventArgs(){
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_FileSavePostCancelNotifyEventHandler.html
		public static IObservable<FileSavePostCancelNotifyEventArgs> FileSavePostCancelNotifyObservable(this PartDoc partDoc)
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

					partDoc.FileSavePostCancelNotify += callback;
					return Disposable.Create(()=> partDoc.FileSavePostCancelNotify-= callback);
					
				}
			);
		}


		public class SketchSolveNotifyEventArgs {
					public String featName {get;}
					public SketchSolveNotifyEventArgs(System.String featName){
					this.featName=featName;
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_SketchSolveNotifyEventHandler.html
		public static IObservable<SketchSolveNotifyEventArgs> SketchSolveNotifyObservable(this PartDoc partDoc)
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

					partDoc.SketchSolveNotify += callback;
					return Disposable.Create(()=> partDoc.SketchSolveNotify-= callback);
					
				}
			);
		}


		public class DeleteItemPreNotifyEventArgs {
					public Int32 EntityType {get;}
					public String itemName {get;}
					public DeleteItemPreNotifyEventArgs(System.Int32 EntityType, System.String itemName){
					this.EntityType=EntityType;
					this.itemName=itemName;
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_DeleteItemPreNotifyEventHandler.html
		public static IObservable<DeleteItemPreNotifyEventArgs> DeleteItemPreNotifyObservable(this PartDoc partDoc)
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

					partDoc.DeleteItemPreNotify += callback;
					return Disposable.Create(()=> partDoc.DeleteItemPreNotify-= callback);
					
				}
			);
		}


		public class ClearSelectionsNotifyEventArgs {
					public ClearSelectionsNotifyEventArgs(){
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_ClearSelectionsNotifyEventHandler.html
		public static IObservable<ClearSelectionsNotifyEventArgs> ClearSelectionsNotifyObservable(this PartDoc partDoc)
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

					partDoc.ClearSelectionsNotify += callback;
					return Disposable.Create(()=> partDoc.ClearSelectionsNotify-= callback);
					
				}
			);
		}


		public class EquationEditorPreNotifyEventArgs {
					public EquationEditorPreNotifyEventArgs(){
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_EquationEditorPreNotifyEventHandler.html
		public static IObservable<EquationEditorPreNotifyEventArgs> EquationEditorPreNotifyObservable(this PartDoc partDoc)
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

					partDoc.EquationEditorPreNotify += callback;
					return Disposable.Create(()=> partDoc.EquationEditorPreNotify-= callback);
					
				}
			);
		}


		public class EquationEditorPostNotifyEventArgs {
					public Boolean Changed {get;}
					public EquationEditorPostNotifyEventArgs(System.Boolean Changed){
					this.Changed=Changed;
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_EquationEditorPostNotifyEventHandler.html
		public static IObservable<EquationEditorPostNotifyEventArgs> EquationEditorPostNotifyObservable(this PartDoc partDoc)
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

					partDoc.EquationEditorPostNotify += callback;
					return Disposable.Create(()=> partDoc.EquationEditorPostNotify-= callback);
					
				}
			);
		}


		public class OpenDesignTableNotifyEventArgs {
					public Object DesignTable {get;}
					public OpenDesignTableNotifyEventArgs(System.Object DesignTable){
					this.DesignTable=DesignTable;
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_OpenDesignTableNotifyEventHandler.html
		public static IObservable<OpenDesignTableNotifyEventArgs> OpenDesignTableNotifyObservable(this PartDoc partDoc)
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

					partDoc.OpenDesignTableNotify += callback;
					return Disposable.Create(()=> partDoc.OpenDesignTableNotify-= callback);
					
				}
			);
		}


		public class CloseDesignTableNotifyEventArgs {
					public Object DesignTable {get;}
					public CloseDesignTableNotifyEventArgs(System.Object DesignTable){
					this.DesignTable=DesignTable;
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_CloseDesignTableNotifyEventHandler.html
		public static IObservable<CloseDesignTableNotifyEventArgs> CloseDesignTableNotifyObservable(this PartDoc partDoc)
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

					partDoc.CloseDesignTableNotify += callback;
					return Disposable.Create(()=> partDoc.CloseDesignTableNotify-= callback);
					
				}
			);
		}


		public class UnitsChangeNotifyEventArgs {
					public UnitsChangeNotifyEventArgs(){
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_UnitsChangeNotifyEventHandler.html
		public static IObservable<UnitsChangeNotifyEventArgs> UnitsChangeNotifyObservable(this PartDoc partDoc)
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

					partDoc.UnitsChangeNotify += callback;
					return Disposable.Create(()=> partDoc.UnitsChangeNotify-= callback);
					
				}
			);
		}


		public class DestroyNotify2EventArgs {
					public Int32 DestroyType {get;}
					public DestroyNotify2EventArgs(System.Int32 DestroyType){
					this.DestroyType=DestroyType;
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_DestroyNotify2EventHandler.html
		public static IObservable<DestroyNotify2EventArgs> DestroyNotify2Observable(this PartDoc partDoc)
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

					partDoc.DestroyNotify2 += callback;
					return Disposable.Create(()=> partDoc.DestroyNotify2-= callback);
					
				}
			);
		}


		public class ConfigurationChangeNotifyEventArgs {
					public String ConfigurationName {get;}
					public Object Object {get;}
					public Int32 ObjectType {get;}
					public Int32 changeType {get;}
					public ConfigurationChangeNotifyEventArgs(System.String ConfigurationName, System.Object Object, System.Int32 ObjectType, System.Int32 changeType){
					this.ConfigurationName=ConfigurationName;
					this.Object=Object;
					this.ObjectType=ObjectType;
					this.changeType=changeType;
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_ConfigurationChangeNotifyEventHandler.html
		public static IObservable<ConfigurationChangeNotifyEventArgs> ConfigurationChangeNotifyObservable(this PartDoc partDoc)
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

					partDoc.ConfigurationChangeNotify += callback;
					return Disposable.Create(()=> partDoc.ConfigurationChangeNotify-= callback);
					
				}
			);
		}


		public class ActiveViewChangeNotifyEventArgs {
					public ActiveViewChangeNotifyEventArgs(){
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_ActiveViewChangeNotifyEventHandler.html
		public static IObservable<ActiveViewChangeNotifyEventArgs> ActiveViewChangeNotifyObservable(this PartDoc partDoc)
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

					partDoc.ActiveViewChangeNotify += callback;
					return Disposable.Create(()=> partDoc.ActiveViewChangeNotify-= callback);
					
				}
			);
		}


		public class FeatureManagerFilterStringChangeNotifyEventArgs {
					public String FilterString {get;}
					public FeatureManagerFilterStringChangeNotifyEventArgs(System.String FilterString){
					this.FilterString=FilterString;
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_FeatureManagerFilterStringChangeNotifyEventHandler.html
		public static IObservable<FeatureManagerFilterStringChangeNotifyEventArgs> FeatureManagerFilterStringChangeNotifyObservable(this PartDoc partDoc)
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

					partDoc.FeatureManagerFilterStringChangeNotify += callback;
					return Disposable.Create(()=> partDoc.FeatureManagerFilterStringChangeNotify-= callback);
					
				}
			);
		}


		public class FlipLoopNotifyEventArgs {
					public Object TheLoop {get;}
					public Object TheEdge {get;}
					public FlipLoopNotifyEventArgs(System.Object TheLoop, System.Object TheEdge){
					this.TheLoop=TheLoop;
					this.TheEdge=TheEdge;
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_FlipLoopNotifyEventHandler.html
		public static IObservable<FlipLoopNotifyEventArgs> FlipLoopNotifyObservable(this PartDoc partDoc)
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

					partDoc.FlipLoopNotify += callback;
					return Disposable.Create(()=> partDoc.FlipLoopNotify-= callback);
					
				}
			);
		}


		public class AutoSaveNotifyEventArgs {
					public String FileName {get;}
					public AutoSaveNotifyEventArgs(System.String FileName){
					this.FileName=FileName;
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_AutoSaveNotifyEventHandler.html
		public static IObservable<AutoSaveNotifyEventArgs> AutoSaveNotifyObservable(this PartDoc partDoc)
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

					partDoc.AutoSaveNotify += callback;
					return Disposable.Create(()=> partDoc.AutoSaveNotify-= callback);
					
				}
			);
		}


		public class AutoSaveToStorageNotifyEventArgs {
					public AutoSaveToStorageNotifyEventArgs(){
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_AutoSaveToStorageNotifyEventHandler.html
		public static IObservable<AutoSaveToStorageNotifyEventArgs> AutoSaveToStorageNotifyObservable(this PartDoc partDoc)
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

					partDoc.AutoSaveToStorageNotify += callback;
					return Disposable.Create(()=> partDoc.AutoSaveToStorageNotify-= callback);
					
				}
			);
		}


		public class FileDropPreNotifyEventArgs {
					public String FileName {get;}
					public FileDropPreNotifyEventArgs(System.String FileName){
					this.FileName=FileName;
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_FileDropPreNotifyEventHandler.html
		public static IObservable<FileDropPreNotifyEventArgs> FileDropPreNotifyObservable(this PartDoc partDoc)
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

					partDoc.FileDropPreNotify += callback;
					return Disposable.Create(()=> partDoc.FileDropPreNotify-= callback);
					
				}
			);
		}


		public class SensorAlertPreNotifyEventArgs {
					public Object SensorIn {get;}
					public Int32 SensorAlertType {get;}
					public SensorAlertPreNotifyEventArgs(System.Object SensorIn, System.Int32 SensorAlertType){
					this.SensorIn=SensorIn;
					this.SensorAlertType=SensorAlertType;
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_SensorAlertPreNotifyEventHandler.html
		public static IObservable<SensorAlertPreNotifyEventArgs> SensorAlertPreNotifyObservable(this PartDoc partDoc)
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

					partDoc.SensorAlertPreNotify += callback;
					return Disposable.Create(()=> partDoc.SensorAlertPreNotify-= callback);
					
				}
			);
		}


		public class UndoPostNotifyEventArgs {
					public UndoPostNotifyEventArgs(){
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_UndoPostNotifyEventHandler.html
		public static IObservable<UndoPostNotifyEventArgs> UndoPostNotifyObservable(this PartDoc partDoc)
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

					partDoc.UndoPostNotify += callback;
					return Disposable.Create(()=> partDoc.UndoPostNotify-= callback);
					
				}
			);
		}


		public class UserSelectionPreNotifyEventArgs {
					public Int32 SelType {get;}
					public UserSelectionPreNotifyEventArgs(System.Int32 SelType){
					this.SelType=SelType;
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_UserSelectionPreNotifyEventHandler.html
		public static IObservable<UserSelectionPreNotifyEventArgs> UserSelectionPreNotifyObservable(this PartDoc partDoc)
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

					partDoc.UserSelectionPreNotify += callback;
					return Disposable.Create(()=> partDoc.UserSelectionPreNotify-= callback);
					
				}
			);
		}


		public class ActiveDisplayStateChangePreNotifyEventArgs {
					public ActiveDisplayStateChangePreNotifyEventArgs(){
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_ActiveDisplayStateChangePreNotifyEventHandler.html
		public static IObservable<ActiveDisplayStateChangePreNotifyEventArgs> ActiveDisplayStateChangePreNotifyObservable(this PartDoc partDoc)
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

					partDoc.ActiveDisplayStateChangePreNotify += callback;
					return Disposable.Create(()=> partDoc.ActiveDisplayStateChangePreNotify-= callback);
					
				}
			);
		}


		public class ActiveDisplayStateChangePostNotifyEventArgs {
					public String DisplayStateName {get;}
					public ActiveDisplayStateChangePostNotifyEventArgs(System.String DisplayStateName){
					this.DisplayStateName=DisplayStateName;
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_ActiveDisplayStateChangePostNotifyEventHandler.html
		public static IObservable<ActiveDisplayStateChangePostNotifyEventArgs> ActiveDisplayStateChangePostNotifyObservable(this PartDoc partDoc)
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

					partDoc.ActiveDisplayStateChangePostNotify += callback;
					return Disposable.Create(()=> partDoc.ActiveDisplayStateChangePostNotify-= callback);
					
				}
			);
		}


		public class RedoPostNotifyEventArgs {
					public RedoPostNotifyEventArgs(){
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_RedoPostNotifyEventHandler.html
		public static IObservable<RedoPostNotifyEventArgs> RedoPostNotifyObservable(this PartDoc partDoc)
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

					partDoc.RedoPostNotify += callback;
					return Disposable.Create(()=> partDoc.RedoPostNotify-= callback);
					
				}
			);
		}


		public class RedoPreNotifyEventArgs {
					public RedoPreNotifyEventArgs(){
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_RedoPreNotifyEventHandler.html
		public static IObservable<RedoPreNotifyEventArgs> RedoPreNotifyObservable(this PartDoc partDoc)
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

					partDoc.RedoPreNotify += callback;
					return Disposable.Create(()=> partDoc.RedoPreNotify-= callback);
					
				}
			);
		}


		public class UndoPreNotifyEventArgs {
					public UndoPreNotifyEventArgs(){
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_UndoPreNotifyEventHandler.html
		public static IObservable<UndoPreNotifyEventArgs> UndoPreNotifyObservable(this PartDoc partDoc)
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

					partDoc.UndoPreNotify += callback;
					return Disposable.Create(()=> partDoc.UndoPreNotify-= callback);
					
				}
			);
		}


		public class WeldmentCutListUpdatePostNotifyEventArgs {
					public WeldmentCutListUpdatePostNotifyEventArgs(){
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_WeldmentCutListUpdatePostNotifyEventHandler.html
		public static IObservable<WeldmentCutListUpdatePostNotifyEventArgs> WeldmentCutListUpdatePostNotifyObservable(this PartDoc partDoc)
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

					partDoc.WeldmentCutListUpdatePostNotify += callback;
					return Disposable.Create(()=> partDoc.WeldmentCutListUpdatePostNotify-= callback);
					
				}
			);
		}


		public class AutoSaveToStorageStoreNotifyEventArgs {
					public AutoSaveToStorageStoreNotifyEventArgs(){
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_AutoSaveToStorageStoreNotifyEventHandler.html
		public static IObservable<AutoSaveToStorageStoreNotifyEventArgs> AutoSaveToStorageStoreNotifyObservable(this PartDoc partDoc)
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

					partDoc.AutoSaveToStorageStoreNotify += callback;
					return Disposable.Create(()=> partDoc.AutoSaveToStorageStoreNotify-= callback);
					
				}
			);
		}


		public class DragStateChangeNotifyEventArgs {
					public Boolean State {get;}
					public DragStateChangeNotifyEventArgs(System.Boolean State){
					this.State=State;
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_DragStateChangeNotifyEventHandler.html
		public static IObservable<DragStateChangeNotifyEventArgs> DragStateChangeNotifyObservable(this PartDoc partDoc)
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

					partDoc.DragStateChangeNotify += callback;
					return Disposable.Create(()=> partDoc.DragStateChangeNotify-= callback);
					
				}
			);
		}


		public class InsertTableNotifyEventArgs {
					public TableAnnotation TableAnnotation {get;}
					public Int32 TableType {get;}
					public String TemplatePath {get;}
					public InsertTableNotifyEventArgs(SolidWorks.Interop.sldworks.TableAnnotation TableAnnotation, System.Int32 TableType, System.String TemplatePath){
					this.TableAnnotation=TableAnnotation;
					this.TableType=TableType;
					this.TemplatePath=TemplatePath;
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_InsertTableNotifyEventHandler.html
		public static IObservable<InsertTableNotifyEventArgs> InsertTableNotifyObservable(this PartDoc partDoc)
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

					partDoc.InsertTableNotify += callback;
					return Disposable.Create(()=> partDoc.InsertTableNotify-= callback);
					
				}
			);
		}


		public class ModifyTableNotifyEventArgs {
					public TableAnnotation TableAnnotation {get;}
					public Int32 TableType {get;}
					public Int32 reason {get;}
					public Int32 RowInfo {get;}
					public Int32 ColumnInfo {get;}
					public String DataInfo {get;}
					public ModifyTableNotifyEventArgs(SolidWorks.Interop.sldworks.TableAnnotation TableAnnotation, System.Int32 TableType, System.Int32 reason, System.Int32 RowInfo, System.Int32 ColumnInfo, System.String DataInfo){
					this.TableAnnotation=TableAnnotation;
					this.TableType=TableType;
					this.reason=reason;
					this.RowInfo=RowInfo;
					this.ColumnInfo=ColumnInfo;
					this.DataInfo=DataInfo;
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_ModifyTableNotifyEventHandler.html
		public static IObservable<ModifyTableNotifyEventArgs> ModifyTableNotifyObservable(this PartDoc partDoc)
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

					partDoc.ModifyTableNotify += callback;
					return Disposable.Create(()=> partDoc.ModifyTableNotify-= callback);
					
				}
			);
		}


		public class UserSelectionPostNotifyEventArgs {
					public UserSelectionPostNotifyEventArgs(){
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_UserSelectionPostNotifyEventHandler.html
		public static IObservable<UserSelectionPostNotifyEventArgs> UserSelectionPostNotifyObservable(this PartDoc partDoc)
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

					partDoc.UserSelectionPostNotify += callback;
					return Disposable.Create(()=> partDoc.UserSelectionPostNotify-= callback);
					
				}
			);
		}


		public class CommandManagerTabActivatedPreNotifyEventArgs {
					public Int32 CommandTabIndex {get;}
					public String CommandTabName {get;}
					public CommandManagerTabActivatedPreNotifyEventArgs(System.Int32 CommandTabIndex, System.String CommandTabName){
					this.CommandTabIndex=CommandTabIndex;
					this.CommandTabName=CommandTabName;
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_CommandManagerTabActivatedPreNotifyEventHandler.html
		public static IObservable<CommandManagerTabActivatedPreNotifyEventArgs> CommandManagerTabActivatedPreNotifyObservable(this PartDoc partDoc)
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

					partDoc.CommandManagerTabActivatedPreNotify += callback;
					return Disposable.Create(()=> partDoc.CommandManagerTabActivatedPreNotify-= callback);
					
				}
			);
		}


		public class PreRenameItemNotifyEventArgs {
					public Int32 EntityType {get;}
					public String oldName {get;}
					public String NewName {get;}
					public PreRenameItemNotifyEventArgs(System.Int32 EntityType, System.String oldName, System.String NewName){
					this.EntityType=EntityType;
					this.oldName=oldName;
					this.NewName=NewName;
					}
		}

		/// See http://chocolatecubed.com/2016/English/api/sldworksapi/SolidWorks.Interop.sldworks~SolidWorks.Interop.sldworks.DPartDocEvents_PreRenameItemNotifyEventHandler.html
		public static IObservable<PreRenameItemNotifyEventArgs> PreRenameItemNotifyObservable(this PartDoc partDoc)
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

					partDoc.PreRenameItemNotify += callback;
					return Disposable.Create(()=> partDoc.PreRenameItemNotify-= callback);
					
				}
			);
		}

	}
}



