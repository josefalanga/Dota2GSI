using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dota2GSI.Nodes.ItemsProvider
{
    /// <summary>
    /// Class representing item details.
    /// </summary>
    public class ItemDetails : Node
    {
        /// <summary>
        /// List of the inventory items.
        /// </summary>
        public readonly NodeList<Item> Inventory = new NodeList<Item>();

        /// <summary>
        /// List of the stash items.
        /// </summary>
        public readonly NodeList<Item> Stash = new NodeList<Item>();

        /// <summary>
        /// Number of items in the inventory.
        /// </summary>
        public int CountInventory { get { return Inventory.Where(s => s.Name != "empty").Count(); } }

        /// <summary>
        /// Number of items in the stash.
        /// </summary>
        public int CountStash { get { return Stash.Where(s => s.Name != "empty").Count(); } }

        /// <summary>
        /// Gets the teleport item.
        /// </summary>
        public Item Teleport = new Item();

        /// <summary>
        /// Gets the neutral item in the primary neutral slot (neutral0).
        /// </summary>
        public Item Neutral = new Item();

        /// <summary>
        /// Gets the neutral item in the secondary neutral slot (neutral1).
        /// </summary>
        public Item Neutral1 = new Item();

        /// <summary>
        /// List of the preserved neutral items (neutral item slots 6..10).
        /// </summary>
        public readonly NodeList<Item> PreservedNeutral = new NodeList<Item>();

        private Regex _slot_regex = new Regex(@"slot(\d+)");
        private Regex _stash_regex = new Regex(@"stash(\d+)");
        private Regex _teleport_regex = new Regex(@"teleport(\d+)");
        private Regex _neutral_regex = new Regex(@"^neutral0$");
        private Regex _neutral1_regex = new Regex(@"^neutral1$");
        private Regex _preserved_neutral_regex = new Regex(@"preserved_neutral(\d+)");

        internal ItemDetails(JObject parsed_data = null) : base(parsed_data)
        {
            // GSI emits slot0..slot8 in order, but a player with gaps (e.g. an
            // item in the backpack while a main slot is empty) can be sent
            // with varying key presence across gs payloads. Order the parsed
            // items by their slot number so the 6 inventory slots and the 3
            // backpack slots stay stable for consumers that slice [0..6)/[6..9).
            var slots = new List<System.Collections.Generic.KeyValuePair<int, Item>>(9);
            GetMatchingObjects(parsed_data, _slot_regex, (Match match, JObject obj) =>
            {
slots.Add(new System.Collections.Generic.KeyValuePair<int, Item>(
                int.Parse(match.Groups[1].Value), new Item(obj, int.Parse(match.Groups[1].Value))));
            });
            foreach (var kv in slots.OrderBy(kv => kv.Key))
                Inventory.Add(kv.Value);

            GetMatchingObjects(parsed_data, _stash_regex, (Match match, JObject obj) =>
            {
                Item item = new Item(obj);
                Stash.Add(item);
            });

            GetMatchingObjects(parsed_data, _teleport_regex, (Match match, JObject obj) =>
            {
                Item item = new Item(obj);
                Teleport = item;
            });

            GetMatchingObjects(parsed_data, _neutral_regex, (Match match, JObject obj) =>
            {
                Item item = new Item(obj);
                Neutral = item;
            });

            GetMatchingObjects(parsed_data, _neutral1_regex, (Match match, JObject obj) =>
            {
                Item item = new Item(obj);
                Neutral1 = item;
            });

            GetMatchingObjects(parsed_data, _preserved_neutral_regex, (Match match, JObject obj) =>
            {
                Item item = new Item(obj);
                PreservedNeutral.Add(item);
            });
        }

        /// <summary>
        /// Gets the inventory item at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The inventory item.</returns>
        public Item GetInventoryAt(int index)
        {
            if (index < 0 || index > Inventory.Count - 1)
            {
                return new Item();
            }

            return Inventory[index];
        }

        /// <summary>
        /// Gets the inventory item by item name.
        /// </summary>
        /// <param name="item_name">The item name to look for.</param>
        /// <returns>The inventory item.</returns>
        public Item GetInventoryItem(string item_name)
        {
            foreach (var item in Inventory)
            {
                if (item.Name.Equals(item_name))
                {
                    return item;
                }
            }

            return new Item();
        }

        /// <summary>
        /// Checks if item exists in the inventory.
        /// </summary>
        /// <param name="item_name">The item name.</param>
        /// <returns>True if item is in the inventory, false otherwise.</returns>
        public bool InventoryContains(string item_name)
        {
            var found_index = InventoryIndexOf(item_name);
            return found_index > -1;
        }

        /// <summary>
        /// Gets index of the first occurence of the item in the inventory.
        /// </summary>
        /// <param name="item_name">The item name.</param>
        /// <returns>The first index at which item is found, -1 if not found.</returns>
        public int InventoryIndexOf(string item_name)
        {
            for (int x = 0; x < Inventory.Count; x++)
            {
                if (Inventory[x].Name == item_name)
                {
                    return x;
                }
            }

            return -1;
        }

        /// <summary>
        /// Gets the stash item at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The stash item.</returns>
        public Item GetStashAt(int index)
        {
            if (index < 0 || index > Stash.Count - 1)
            {
                return new Item();
            }

            return Stash[index];
        }

        /// <summary>
        /// Gets the stash item by item name.
        /// </summary>
        /// <param name="item_name">The item name to look for.</param>
        /// <returns>The inventory item.</returns>
        public Item GetStashItem(string item_name)
        {
            foreach (var item in Stash)
            {
                if (item.Name.Equals(item_name))
                {
                    return item;
                }
            }

            return new Item();
        }

        /// <summary>
        /// Checks if item exists in the stash.
        /// </summary>
        /// <param name="item_name">The item name.</param>
        /// <returns>True if item is in the stash, false otherwise.</returns>
        public bool StashContains(string item_name)
        {
            var found_index = StashIndexOf(item_name);
            return found_index > -1;
        }

        /// <summary>
        /// Gets index of the first occurence of the item in the stash.
        /// </summary>
        /// <param name="item_name">The item name.</param>
        /// <returns>The first index at which item is found, -1 if not found.</returns>
        public int StashIndexOf(string item_name)
        {
            for (int x = 0; x < Stash.Count; x++)
            {
                if (Stash[x].Name == item_name)
                {
                    return x;
                }
            }

            return -1;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"[" +
                $"Inventory: {Inventory}, " +
                $"Stash: {Stash}, " +
                $"Teleport: {Teleport}, " +
                $"Neutral: {Neutral}, " +
                $"Neutral1: {Neutral1}, " +
                $"PreservedNeutral: {PreservedNeutral}, " +
                $"]";
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (null == obj)
            {
                return false;
            }

            return obj is ItemDetails other &&
                Inventory.Equals(other.Inventory) &&
                Stash.Equals(other.Stash) &&
                Teleport.Equals(other.Teleport) &&
                Neutral.Equals(other.Neutral) &&
                Neutral1.Equals(other.Neutral1) &&
                PreservedNeutral.Equals(other.PreservedNeutral);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            int hashCode = 74222497;
            hashCode = hashCode * -709592358 + Inventory.GetHashCode();
            hashCode = hashCode * -709592358 + Stash.GetHashCode();
            hashCode = hashCode * -709592358 + Teleport.GetHashCode();
            hashCode = hashCode * -709592358 + Neutral.GetHashCode();
            hashCode = hashCode * -709592358 + Neutral1.GetHashCode();
            hashCode = hashCode * -709592358 + PreservedNeutral.GetHashCode();
            return hashCode;
        }
    }
}
