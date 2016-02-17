using System.Runtime.InteropServices;
using System.Reflection;

using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using SolidWorksTools;
using SolidWorksTools.File;
using SwCSharpAddinMF.SWAddin;


namespace SwCSharpAddinMF
{
    /// <summary>
    /// Summary description for SwCSharpAddinMF.
    /// </summary>
    [Guid("7612e834-6277-4122-9e8f-675258162910"), ComVisible(true)]
    [SwAddin(
        Description = "SwCSharpAddinMF description",
        Title = "SwCSharpAddinMF",
        LoadAtStartup = true
        )]
    public class SwAddin : SwAddinBase
    {
        #region Local Variables

        public const int MainCmdGroupId = 5;
        public const int MainItemId1 = 0;
        public const int MainItemId2 = 1;
        public const int MainItemId3 = 2;
        public const int FlyoutGroupId = 91;

        #endregion


        #region UI Methods
        public void AddCommandMgr()
        {
            if (Bmp == null)
                Bmp = new BitmapHandler();
            var cmdIndex1 = 0;
            const string title = "C# Addin";
            const string toolTip = "C# Addin";


            int[] docTypes = {(int)swDocumentTypes_e.swDocASSEMBLY,
                                       (int)swDocumentTypes_e.swDocDRAWING,
                                       (int)swDocumentTypes_e.swDocPART};

            var thisAssembly = Assembly.GetAssembly(GetType());


            var cmdGroupErr = 0;
            var ignorePrevious = false;

            object registryIDs;
            //get the ID information stored in the registry
            var getDataResult = ICmdMgr.GetGroupDataFromRegistry(MainCmdGroupId, out registryIDs);

            int[] knownIDs = { MainItemId1, MainItemId2 };

            if (getDataResult)
            {
                if (!CompareIDs((int[])registryIDs, knownIDs)) //if the IDs don't match, reset the commandGroup
                {
                    ignorePrevious = true;
                }
            }

            ICommandGroup cmdGroup = ICmdMgr.CreateCommandGroup2(MainCmdGroupId, title, toolTip, "", -1, ignorePrevious, ref cmdGroupErr);
            cmdGroup.LargeIconList = Bmp.CreateFileFromResourceBitmap("SwCSharpAddinMF.ToolbarLarge.bmp", thisAssembly);
            cmdGroup.SmallIconList = Bmp.CreateFileFromResourceBitmap("SwCSharpAddinMF.ToolbarSmall.bmp", thisAssembly);
            cmdGroup.LargeMainIcon = Bmp.CreateFileFromResourceBitmap("SwCSharpAddinMF.MainIconLarge.bmp", thisAssembly);
            cmdGroup.SmallMainIcon = Bmp.CreateFileFromResourceBitmap("SwCSharpAddinMF.MainIconSmall.bmp", thisAssembly);

            var menuToolbarOption = (int)swCommandItemType_e.swToolbarItem | (int)swCommandItemType_e.swMenuItem;
            var cmdIndex0 = cmdGroup.AddCommandItem2("CreateCube", -1, "Create a cube", "Create cube", 0, nameof(CreateCube), "", MainItemId1, menuToolbarOption);

            cmdGroup.HasToolbar = true;
            cmdGroup.HasMenu = true;
            cmdGroup.Activate();


            var flyGroup = ICmdMgr.CreateFlyoutGroup(FlyoutGroupId, "Dynamic Flyout", "Flyout Tooltip", "Flyout Hint",
              cmdGroup.SmallMainIcon, cmdGroup.LargeMainIcon, cmdGroup.SmallIconList, cmdGroup.LargeIconList, nameof(FlyoutCallback), nameof(FlyoutEnable));


            flyGroup.AddCommandItem("FlyoutCommand 1", "test", 0, "FlyoutCommandItem1", "FlyoutEnableCommandItem1");

            flyGroup.FlyoutType = (int)swCommandFlyoutStyle_e.swCommandFlyoutStyle_Simple;


            foreach (var type in docTypes)
            {
                var cmdTab = ICmdMgr.GetCommandTab(type, title);

                if (cmdTab != null & !getDataResult | ignorePrevious)//if tab exists, but we have ignored the registry info (or changed command group ID), re-create the tab.  Otherwise the ids won't matchup and the tab will be blank
                {
                    ICmdMgr.RemoveCommandTab(cmdTab);
                    cmdTab = null;
                }

                //if cmdTab is null, must be first load (possibly after reset), add the commands to the tabs
                if (cmdTab == null)
                {
                    cmdTab = ICmdMgr.AddCommandTab(type, title);

                    var cmdBox = cmdTab.AddCommandTabBox();

                    var cmdIDs = new int[3];
                    var textType = new int[3];

                    cmdIDs[0] = cmdGroup.CommandID[cmdIndex0];

                    textType[0] = (int)swCommandTabButtonTextDisplay_e.swCommandTabButton_TextHorizontal;

                    cmdIDs[1] = cmdGroup.CommandID[cmdIndex1];

                    textType[1] = (int)swCommandTabButtonTextDisplay_e.swCommandTabButton_TextHorizontal;

                    cmdIDs[2] = cmdGroup.ToolbarId;

                    textType[2] = (int)swCommandTabButtonTextDisplay_e.swCommandTabButton_TextHorizontal | (int)swCommandTabButtonFlyoutStyle_e.swCommandTabButton_ActionFlyout;

                    cmdBox.AddCommands(cmdIDs, textType);



                    var cmdBox1 = cmdTab.AddCommandTabBox();
                    cmdIDs = new int[1];
                    textType = new int[1];

                    cmdIDs[0] = flyGroup.CmdID;
                    textType[0] = (int)swCommandTabButtonTextDisplay_e.swCommandTabButton_TextBelow | (int)swCommandTabButtonFlyoutStyle_e.swCommandTabButton_ActionFlyout;

                    cmdBox1.AddCommands(cmdIDs, textType);

                    cmdTab.AddSeparator(cmdBox1, cmdIDs[0]);

                }

            }
        }

        public void RemoveCommandMgr()
        {
            Bmp.Dispose();

            ICmdMgr.RemoveCommandGroup(MainCmdGroupId);
            ICmdMgr.RemoveFlyoutGroup(FlyoutGroupId);
        }

        #endregion

        #region UI Callbacks
        public void CreateCube()
        {
            SampleMacroFeature.AddMacroFeature(SwApp);
        }


        public void FlyoutCallback()
        {
            var flyGroup = ICmdMgr.GetFlyoutGroup(FlyoutGroupId);
            flyGroup.RemoveAllCommandItems();

            flyGroup.AddCommandItem(System.DateTime.Now.ToLongTimeString(), "test", 0, "FlyoutCommandItem1", "FlyoutEnableCommandItem1");

        }
        public int FlyoutEnable()
        {
            return 1;
        }

        public void FlyoutCommandItem1()
        {
            ISwApp.SendMsgToUser("Flyout command 1");
        }

        public int FlyoutEnableCommandItem1()
        {
            return 1;
        }
        #endregion


        public override void Connect()
        {
            ICmdMgr = ISwApp.GetCommandManager(AddinId);
            AddCommandMgr();
        }

        public override void Disconnect()
        {
           RemoveCommandMgr(); 
        }
    }

}
