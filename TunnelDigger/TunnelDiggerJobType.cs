using Pipliz;
using NetworkUI;
using NetworkUI.Items;
using System.Collections.Generic;
using BlockEntities;

namespace TunnelDigger
{
    [BlockEntityAutoLoader]
    public class TunnelDiggerJobType : IChangedWithType, IMultiBlockEntityMapping
    {
        public ItemTypes.ItemType TypeToRegister { get { return ItemTypes.GetType("Khanx.TunnelDiggerJob"); } }

        public IEnumerable<ItemTypes.ItemType> TypesToRegister { get { return types; } }

        readonly ItemTypes.ItemType[] types = new ItemTypes.ItemType[]
            {
                 ItemTypes.GetType("Khanx.TunnelDiggerJobx-"),
                 ItemTypes.GetType("Khanx.TunnelDiggerJobx+"),
                 ItemTypes.GetType("Khanx.TunnelDiggerJobz-"),
                 ItemTypes.GetType("Khanx.TunnelDiggerJobz+")
            };

        public void OnChangedWithType(Chunk chunk, BlockChangeRequestOrigin requestOrigin, Vector3Int blockPosition, ItemTypes.ItemType typeOld, ItemTypes.ItemType typeNew)
        {
            //OnAdd
            if (typeOld != BlockTypes.BuiltinBlocks.Types.air)
                return;

            if (requestOrigin.Type != BlockChangeRequestOrigin.EType.Player)
                return;

            TunnelDiggerMenu.SendMenu(blockPosition, typeNew.Name, requestOrigin.AsPlayer);
        }

    }
}
