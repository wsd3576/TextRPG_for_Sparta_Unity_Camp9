using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static TextRPG.TextRPG;

namespace TextRPG
{
    public class GameData
    {
        public PlayerInfo Player { get; set; }
        public StoreInfo Store { get; set; }

        public GameData() { }

        public GameData(PlayerInfo player, StoreInfo store)
        {
            Player = player;
            Store = store;
        }

        public class PlayerInfo
        {
            public string Name { get; set; }
            public string Job { get; set; }
            public int Level { get; set; } = 1;
            public float Attack { get; set; } = 10;
            public int Defence { get; set; } = 5;
            public int Health { get; set; } = 100;
            public int Gold { get; set; } = 1500;
            public List<Item> Inventory { get; set; } = new List<Item>();

            public int EquipAttack { get; set; } = 0;
            public int EquipDefence { get; set; } = 0;
            public int EquipHealth { get; set; } = 0;

            public float TotalAttack => Attack + Inventory.Where(item => item.IsEquipped && item.Type == ItemType.Weapon).Sum(i => i.Value);

            public int TotalDefence => Defence + Inventory.Where(item => item.IsEquipped && item.Type == ItemType.Armor).Sum(i => i.Value);

            public int TotalHealth => Health + Inventory.Where(item => item.IsEquipped && item.Type == ItemType.Accessory).Sum(i => i.Value);

            public void DisplayPlayerStats(PlayerInfo player)
            {
                Console.WriteLine($"Lv. 0{player.Level}");
                Console.WriteLine($"{player.Name} ( {player.Job} )");
                Console.WriteLine($"공격력 : {player.TotalAttack}" + (player.EquipAttack > 0 ? $" (+{player.EquipAttack})" : ""));
                Console.WriteLine($"방어력 : {player.TotalDefence}" + (player.EquipDefence > 0 ? $" (+{player.EquipDefence})" : ""));
                Console.WriteLine($"공격력 : {player.TotalHealth}" + (player.EquipHealth > 0 ? $" (+{player.EquipHealth})" : ""));
                Console.WriteLine($"Gold   : {player.Gold} G");
            }

            public void EquipItem(PlayerInfo player, ItemType type, int value, bool equip)
            {
                int modifier = equip ? value : -value;

                switch (type)
                {
                    case ItemType.Weapon:
                        player.EquipAttack += modifier;
                        break;
                    case ItemType.Armor:
                        player.EquipDefence += modifier;
                        break;
                    case ItemType.Accessory:
                        player.EquipHealth += modifier;
                        break;
                }
            }
        }

        

        public class StoreInfo
        {
            public List<Item> storeItems { get; set; } = new List<Item>();
        }

        public enum ItemType
        {
            Weapon = 0,
            Armor,
            Accessory,
            Etc
        }

        public class Item
        {
            public string Name { get; set; }
            public ItemType Type { get; set; }
            public int Value { get; set; }
            public string Description { get; set; }
            public int Price { get; set; }
            public bool IsEquipped { get; set; }
            public bool AlreadyBought { get; set; }

            public Item() { }

            public Item(string name, ItemType type, int value, string description, int price, bool isEquipped = false, bool alreadyBought = false)
            {
                Name = name;
                Type = type;
                Value = value;
                Description = description;
                Price = price;
                IsEquipped = isEquipped;
                AlreadyBought = alreadyBought;
            }
        }

    }
}
