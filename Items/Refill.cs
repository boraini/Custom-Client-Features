using System;
using System.Collections.Generic;
using System.Linq;

namespace Items {
	public class Refill : Consumable {
		public Refill(Item item) : base(item){}

        private static List<string> allItemNames = null;

        public static Item BestItem() {
            if (allItemNames == null) {
                allItemNames = (
                    from item in Config.main.AllItems()
                    where item.action == Item.Action.Refill
                    orderby item.power descending
                    select item.name
                ).ToList();
            }

            Player player = ReplaceableSingleton<Player>.main;

            foreach (string id in allItemNames) {
                Item item = Item.Get(id);
                if (player.MaxSteam() - player.steam >= item.power && player.inventory.Quantity(item) > 0) {
                    return item;
                }
            }

            if (player.steam > 0f) {
                return Item.Get(allItemNames[allItemNames.Count - 1]);
            }

            return null;
        }

        public static bool UseTheBest() {
            Item bestItem = BestItem();

            if (bestItem != null) {
                Consumable.Get(bestItem).Use(null);
            }

            return false;
        }

		protected override bool UseInternal(object useData) {
			if (base.player.steam < base.player.MaxSteam()) {
				base.player.RestoreSteam(this.item.power);
				base.player.CreateQuip("+" + (int)this.item.power + " steam", null);
				return true;
			}
			return false;
		}

		protected override bool ShouldRemoveInventory() {
			return true;
		}
	}
}
