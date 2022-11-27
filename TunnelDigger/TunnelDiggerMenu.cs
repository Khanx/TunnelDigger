using Pipliz;
using ModLoaderInterfaces;
using NetworkUI;
using Newtonsoft.Json.Linq;
using NetworkUI.AreaJobs;
using Jobs;
using Shared;
using NetworkUI.Items;
using System.Collections.Generic;

namespace TunnelDigger
{
    [ModLoader.ModManager]
    public class TunnelDiggerMenu : IOnPlayerPushedNetworkUIButton, IOnPlayerClicked
    {
        public void OnPlayerClicked(Players.Player player, PlayerClickedData click)
        {
            if (click.ClickType != PlayerClickedData.EClickType.Right)
                return;

            if (click.HitType != PlayerClickedData.EHitType.Block)
                return;

            if (!ItemTypes.GetType(click.GetVoxelHit().TypeHit).HasParentType(ItemTypes.GetType("Khanx.TunnelDiggerJob")))
                return;

            SendMenu(click.GetVoxelHit().BlockHit, ItemTypes.GetType(click.GetVoxelHit().TypeHit).Name, player);
        }

        public static void SendMenu(Vector3Int blockPosition, string typeName, Players.Player player)
        {
            NetworkMenu tunnelMenu = new NetworkMenu();

            tunnelMenu.Identifier = "TunnelDigger";
            tunnelMenu.LocalStorage.SetAs("header", "Tunnel Digger");
            tunnelMenu.Width = 325;
            tunnelMenu.Height = 250;

            int width = 75;

            tunnelMenu.Items.Add(new HorizontalRow(new List<(IItem, int)>
            {
                (new EmptySpace(), 40),
                (new EmptySpace() , width/2),
                (new Label(new LabelData("Includes box row.", UnityEngine.Color.white)), width*2),
                (new EmptySpace(), width/2)
            }));

            InputField inputUp = new InputField("Khanx.TunnelDigger.Up." + player.Name, width, 30);

            tunnelMenu.LocalStorage.SetAs("Khanx.TunnelDigger.Up." + player.Name, "Up");
            //tunnelMenu.Items.Add(inputUp);

            tunnelMenu.Items.Add(new HorizontalRow(new List<(IItem, int)>
            {
                (new EmptySpace(), 20),
                (new EmptySpace() , width),
                (inputUp, width),
                (new EmptySpace(), width)
            }));

            InputField inputLeft = new InputField("Khanx.TunnelDigger.Left." + player.Name, width, 30);

            tunnelMenu.LocalStorage.SetAs("Khanx.TunnelDigger.Left." + player.Name, "Left");
            //tunnelMenu.Items.Add(inputLeft);

            InputField inputForward = new InputField("Khanx.TunnelDigger.Forward." + player.Name, width, 30);

            tunnelMenu.LocalStorage.SetAs("Khanx.TunnelDigger.Forward." + player.Name, "Forward");
            //tunnelMenu.Items.Add(inputDepth);

            InputField inputRight = new InputField("Khanx.TunnelDigger.Right." + player.Name, width, 30);

            tunnelMenu.LocalStorage.SetAs("Khanx.TunnelDigger.Right." + player.Name, "Right");
            //tunnelMenu.Items.Add(inputRight);

            tunnelMenu.Items.Add(new HorizontalRow(new List<(IItem, int)>
            {
                (new EmptySpace(), 20),
                (inputLeft , width),
                (inputForward, width),
                (inputRight, width)
            }));

            InputField inputDown = new InputField("Khanx.TunnelDigger.Down." + player.Name, width, 30);

            tunnelMenu.LocalStorage.SetAs("Khanx.TunnelDigger.Down." + player.Name, "Down");
            //tunnelMenu.Items.Add(inputDown);

            tunnelMenu.Items.Add(new HorizontalRow(new List<(IItem, int)>
            {
                (new EmptySpace(), 20),
                (new EmptySpace() , width),
                (inputDown, width),
                (new EmptySpace(), width)
            }));

            ButtonCallback digButton = new ButtonCallback("Khanx.TunnelDigger.Dig",
                                                                      new LabelData("Dig", UnityEngine.Color.white),
                                                                      width * 2,
                                                                      30,
                                                                      ButtonCallback.EOnClickActions.ClosePopup);

            //tunnelMenu.Items.Add(digButton);

            tunnelMenu.Items.Add(new EmptySpace(25));
            tunnelMenu.Items.Add(new HorizontalRow(new List<(IItem, int)>
            {
                (new EmptySpace(), 20),
                (new EmptySpace() , width/2),
                (digButton, width*2),
                (new EmptySpace(), width/2)
            }));

            tunnelMenu.LocalStorage.SetAs("Khanx.TunnelDigger.Position." + player.Name, blockPosition.ToString());

            string rotation = typeName;
            rotation = rotation.Substring(rotation.Length - 2);

            tunnelMenu.LocalStorage.SetAs("Khanx.TunnelDigger.Rotation." + player.Name, rotation);

            NetworkMenuManager.SendServerPopup(player, tunnelMenu);
        }

        public void OnPlayerPushedNetworkUIButton(ButtonPressCallbackData data)
        {
            if (data.ButtonIdentifier.Equals("Khanx.TunnelDigger.Dig"))
            {
                int up = data.Storage.GetAsOrDefaultOrError<int>("Khanx.TunnelDigger.Up." + data.Player.Name, 0);
                int left = data.Storage.GetAsOrDefaultOrError<int>("Khanx.TunnelDigger.Left." + data.Player.Name, 0);
                int forward = data.Storage.GetAsOrDefaultOrError<int>("Khanx.TunnelDigger.Forward." + data.Player.Name, 0);
                int right = data.Storage.GetAsOrDefaultOrError<int>("Khanx.TunnelDigger.Right." + data.Player.Name, 0);
                int down = data.Storage.GetAsOrDefaultOrError<int>("Khanx.TunnelDigger.Down." + data.Player.Name, 0);

                if (up < 0 || left < 0 || forward < 0 || right < 0 || down < 0)
                {
                    Chatting.Chat.Send(data.Player, "Excavation not set up correctly.");
                    return;
                }

                string sPosition = data.Storage.GetAsOrDefaultOrError<string>("Khanx.TunnelDigger.Position." + data.Player.Name, "");
                Vector3Int position = Vector3Int.Parse(sPosition);

                Vector3Int pos1 = Vector3Int.zero, pos2 = Vector3Int.zero;
                string rotation = data.Storage.GetAsOrDefaultOrError<string>("Khanx.TunnelDigger.Rotation." + data.Player.Name, "");

                /*
                if (up > 0)
                    up -= 1;

                if (left > 0)
                    left -= 1;
                */
                if (forward > 0)
                    forward -= 1;
                /*
                if (right > 0)
                    right -= 1;

                if (down > 0)
                    down -= 1;
                */

                switch (rotation)
                {
                    case "x+":
                    {
                        position += new Vector3Int(1, 0, 0);

                        pos1 = position + new Vector3Int(0, up, left);
                        pos2 = position + new Vector3Int(forward, -down, -right);
                    }
                    break;

                    case "x-":
                    {
                        position += new Vector3Int(-1, 0, 0);

                        pos1 = position + new Vector3Int(0, up, -left);
                        pos2 = position + new Vector3Int(-forward, -down, +right);
                    }
                    break;

                    case "z+":
                    {
                        position += new Vector3Int(0, 0, 1);

                        pos1 = position + new Vector3Int(-left, up, 0);
                        pos2 = position + new Vector3Int(right, -down, forward);
                    }
                    break;

                    case "z-":
                    {
                        position += new Vector3Int(0, 0, -1);

                        pos1 = position + new Vector3Int(left, up, 0);
                        pos2 = position + new Vector3Int(-right, -down, -forward);
                    }
                    break;
                }

                //Chatting.Chat.Send(data.Player, "Rotation: " + rotation);

                Vector3Int corner1 = Vector3Int.Min(pos1, pos2);
                Vector3Int corner2 = Vector3Int.Max(pos1, pos2);

                JObject args = new JObject
                {
                    { "constructionType", "pipliz.digger" }
                };

                if (AreaJobTracker.CreateNewAreaJob("pipliz.constructionarea", args, data.Player.ActiveColony, corner1, corner2) == null)
                {
                    //Area no created because overlaps another area.
                    return;
                }

                AreaJobTracker.SendData(data.Player);

                Chatting.Chat.Send(data.Player, "Digger area created.");

                CommandToolManager.AreaDescriptions.TryGetValue("constructionjob", out var description);

                BlockToolDescriptionSettings blockToolDescriptionSettings = description as BlockToolDescriptionSettings;

                CommandToolManager.StartCommandToolSelection(data.Player, blockToolDescriptionSettings);

                ServerManager.TryChangeBlock(Vector3Int.Parse(sPosition), BlockTypes.BuiltinBlocks.Types.air, data.Player);
            }
        }
    }
}
