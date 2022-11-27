using Jobs;
using ModLoaderInterfaces;
using NetworkUI;
using NetworkUI.AreaJobs;
using NetworkUI.Items;

namespace TunnelDigger
{
    [ModLoader.ModManager]
    public class CommandTool : IAfterWorldLoad, IOnConstructCommandTool
    {
        public void AfterWorldLoad()
        {
            CommandToolManager.MenuTooltips.Add("Khanx.TunnelDigger", ("popup.tooljob.TunnelDiggerA", "popup.tooljob.TunnelDiggerB"));
            CommandToolManager.AreaDescriptions.Add("Khanx.TunnelDigger",
                new BlockToolDescriptionSettings("Tunnel Digger", "Khanx.TunnelDiggerJob", "pipliz.constructor", EBlockToolHoverType.GreenIfNPCCanStand));
        }

        public void OnConstructCommandTool(Players.Player player, NetworkMenu networkMenu, string menuName)
        {
            if (!menuName.Equals("popup.tooljob.construction"))
                return;

            networkMenu.Items.Add(new EmptySpace(20));
            CommandToolManager.GenerateTwoColumnCenteredRow(networkMenu, CommandToolManager.GetButtonTool(player, "Khanx.TunnelDigger", "popup.tooljob.TunnelDigger", 200), new EmptySpace());
        }
    }
}
