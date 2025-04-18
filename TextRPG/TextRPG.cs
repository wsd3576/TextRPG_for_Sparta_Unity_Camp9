using Newtonsoft.Json;
using static TextRPG.GameSystem;
using static TextRPG.GameData;
using static TextRPG.TextRPG;
using System.Reflection.Emit;
using System.Xml.Linq;

namespace TextRPG
{
    internal class TextRPG
    {
        GameSystem gameSystem = new GameSystem();
        PlayerInfo player = new PlayerInfo();
        StoreInfo store = new StoreInfo();
        public static class SaveManager
        {

            private static string saveFilePath = "save.json";

            public static void SaveGame(PlayerInfo player, StoreInfo store)
            {
                GameData gameData = new GameData(player, store);
                string json = JsonConvert.SerializeObject(gameData, Formatting.Indented);
                File.WriteAllText(saveFilePath, json);
            }

            public static (PlayerInfo, StoreInfo) LoadGame()
            {
                if (!File.Exists(saveFilePath))
                {
                    Console.WriteLine("저장 파일이 존재하지 않습니다.");
                    return (null, null);
                }

                string json = File.ReadAllText(saveFilePath);
                GameData gameData = JsonConvert.DeserializeObject<GameData>(json);

                return (gameData.Player, gameData.Store);
            }
        }

        static void Main()
        {
            new TextRPG().Start();
        }

        void Start()
        {
            player.Inventory.AddRange(new List<Item>
                {
                new Item("초심자의 검", ItemType.Weapon, 3, "철로 만들어진 보급형 검.", 500),
                new Item("초심자의 갑옷", ItemType.Armor, 3, "철로 만들어진 보급형 값옷.", 500),
                new Item("추억의 목걸이", ItemType.Accessory, 10, "사실 길가에서 주웠습니다.", 2500)
                });
            store.storeItems.AddRange(new List<Item>
                {
                new Item("수련자 갑옷", ItemType.Armor, 5, "수련에 도움을 주는 갑옷입니다.", 1000),
                new Item("무쇠갑옷", ItemType.Armor, 9, "무쇠로 만들어져 튼튼한 갑옷입니다.", 2000),
                new Item("스파르타의 갑옷", ItemType.Armor, 15, "스파르타의 전사들이 사용했다는 전설의 갑옷입니다.", 3500),
                new Item("낡은 검", ItemType.Weapon, 2, "쉽게 볼 수 있는 낡은 검 입니다.", 600),
                new Item("청동 도끼", ItemType.Weapon, 5, "어디선가 사용됐던거 같은 도끼입니다.", 1500),
                new Item("스파르타의 창", ItemType.Weapon, 7, "스파르타의 전사들이 사용했다는 전설의 창입니다.", 3000)
                });
            Console.WriteLine("여기는 텍스트 던전의 세계.");

            while (true)
            {
                Console.Write("이곳에 들어온 당신의 이름을 알려주세요.\n\n>>");
                string selection = Console.ReadLine();

                if (gameSystem.Confirm($"{selection}... 당신의 이름이 맞나요?"))
                {
                    player.Name = selection;
                    break;
                }
            }

            while (true)
            {
                string[] jobs = { "전사", "도적", "궁수" };
                string[] jobOptions = new string[jobs.Length];

                Console.WriteLine("당신의 직업은 무엇인가요?");

                for (int i = 0; i < jobs.Length; i++)
                {
                    jobOptions[i] = $"{i + 1}.{jobs[i]}";
                }

                int selection = gameSystem.Select(jobOptions, false);

                if (selection != -1 && gameSystem.Confirm($"{jobs[selection - 1]}... 당신의 직업이 맞나요?"))
                {
                    player.Job = jobs[selection - 1];
                    break;
                }
            }

            gameSystem.ShowMessage($"환영합니다, {player.Job} {player.Name}.");
            Town();
        }

        void Town()
        {
            while (true)
            {
                Console.WriteLine("마을에 오신걸 환영합니다.\n이곳에서 던전에 들어가기 전 행동을 할 수 있습니다.\n");

                int selection = gameSystem.Select(new string[] { "1.상태보기", "2.인벤토리", "3.상점", "4.휴식하기", "5.던전입장" }, false);

                switch (selection)
                {
                    case 1:
                        Status();
                        break;
                    case 2:
                        Inventory(Mode.View);
                        break;
                    case 3:
                        Store(Mode.View);
                        break;
                    case 4:
                        Rest();
                        break;
                    case 5:
                        Dungeon();
                        break;
                }
            }
        }

        void Status()
        {
            Console.WriteLine($"상태 보기");
            Console.WriteLine("캐릭터의 정보가 표시됩니다.");

            player.DisplayPlayerStats(player);

            int selection = gameSystem.Select(new string[] { "1.저장하기", "2.불러오기" }, true);

            switch (selection)
            {
                case 1:
                    if (gameSystem.Confirm("현재 상태를 저장합니다. 확실합니까?(기존의 저장은 삭제됩니다.)"))
                    {
                        SaveManager.SaveGame(player, store);
                    }
                    break;
                case 2:
                    if (gameSystem.Confirm("저장된 파일을 불러옵니다. 확실합니까?(지금까지의 진행은 저장되지 않습니다.)"))
                    {
                        (PlayerInfo loadedPlayer, StoreInfo loadedStore) = SaveManager.LoadGame();

                        if (loadedPlayer != null && loadedStore != null)
                        {
                            player.Inventory.Clear();
                            store.storeItems.Clear();

                            player = loadedPlayer;
                            store = loadedStore;

                            gameSystem.ShowMessage($"{loadedPlayer.Name}의 정보를 불러왔습니다.");
                        }
                    }
                    break;
                case 0:
                    Console.Clear();
                    return;
                default:
                    gameSystem.ShowMessage("잘못된 입력입니다.");
                    break;
            }
        }

        enum Mode
        {
            View,
            Interaction,
            Sell
        }

        void Inventory(Mode mode)
        {
            Console.WriteLine(mode == Mode.View ?
                "인벤토리\n" +
                "보유 중인 아이템을 관리할 수 있습니다.\n\n" +
                "[아이템 목록]" :
                "장착 관리\n" +
                "보유 중인 아이템을 관리할 수 있습니다.\n\n" +
                "[아이템 목록]");

            string[]? items = DisplayInventory(mode, "Inventory");

            if (mode == Mode.View)
            {
                Console.WriteLine();
                int select = gameSystem.Select(new string[] { "1.장착관리" }, true);

                if (items == null && select == 1)
                {
                    gameSystem.ShowMessage("아이템이 없어 관리할 수 없습니다..");
                }
                else if (select == 1)
                {
                    Inventory(Mode.Interaction);
                }
                else if (select == 0)
                {
                    return;
                }
            }
            else if (mode == Mode.Interaction)
            {
                int selection = gameSystem.Select(items, true);

                if (selection == 0)
                {
                    Inventory(Mode.View);
                }

                if (selection - 1 >= 0 && selection - 1 < player.Inventory.Count)
                {
                    Item selectedItem = player.Inventory[selection - 1];

                    if (selectedItem.Type == ItemType.Etc)
                    {
                        gameSystem.ShowMessage("\n장착할 수 없는 아이템입니다.");
                    }
                    else if (selectedItem.IsEquipped)
                    {
                        if (selectedItem.Type == ItemType.Accessory && player.Health < 0)
                        {
                            gameSystem.ShowMessage($"해제할 수 없다.\n" +
                                $"{selectedItem.Name}을(를) 해제하면 나는 죽고만다.");
                            Inventory(Mode.Interaction);
                        }

                        selectedItem.IsEquipped = false;
                        player.EquipItem(player, selectedItem.Type, selectedItem.Value, false);

                        gameSystem.ShowMessage($"{selectedItem.Name}을(를) 해제했습니다.");
                    }
                    else
                    {
                        foreach (Item item in player.Inventory)
                        {
                            if (item.Type == selectedItem.Type && item.IsEquipped)
                            {
                                item.IsEquipped = false;
                            }
                        }

                        selectedItem.IsEquipped = true;
                        player.EquipItem(player, selectedItem.Type, selectedItem.Value, true);

                        gameSystem.ShowMessage($"{selectedItem.Name}을(를) 장착했습니다.");
                    }
                }
            }
            else
            {
                gameSystem.ShowMessage("잘못된 입력입니다.");
            }
        }

        void Store(Mode mode)
        {
            List<Item> storeItems = store.storeItems;
            List<Item> playeritems = player.Inventory;

            Console.Write((mode == Mode.View ? "상점\n필요한 아이템을 얻을 수 있는 상점입니다." : (mode == Mode.Interaction ? "구매\n필요한 아이템을 골라주세요." : "")) +
                (mode != Mode.Sell ? $"\n\n[보유 골드] {player.Gold} G\n\n[아이템 목록]\n" : ""));
            string[]? items = DisplayInventory(mode, "Store");


            if (mode == Mode.View)
            {
                Console.WriteLine();
                int select = gameSystem.Select(new string[] { "1.구매하기", "2.판매하기" }, true);

                if (select == 1)
                {
                    Store(Mode.Interaction);
                }
                else if (select == 2)
                {
                    Store(Mode.Sell);
                }
                else if (select == 0)
                {
                    return;
                }
            }
            else if (mode == Mode.Interaction)
            {
                int selection = gameSystem.Select(items, true);

                if (selection == 0)
                {
                    Store(Mode.View);
                }
                else if (selection >= 1 && selection <= storeItems.Count)
                {
                    Item selectedItem = storeItems[selection - 1];

                    if (selectedItem.AlreadyBought)
                    {
                        gameSystem.ShowMessage("이미 구매한 아이템 입니다.");
                    }
                    else if (player.Gold >= selectedItem.Price)
                    {
                        if (gameSystem.Confirm($"{selectedItem.Name}을(를) {selectedItem.Price} G에 구매하시겠습니까?"))
                        {
                            player.Inventory.Add(new Item(selectedItem.Name, selectedItem.Type, selectedItem.Value, selectedItem.Description, selectedItem.Price));
                            selectedItem.AlreadyBought = true;
                            player.Gold -= selectedItem.Price;
                            gameSystem.ShowMessage($"{selectedItem.Name}을(를) 구매했습니다.");
                        }
                    }
                    else gameSystem.ShowMessage("골드가 부족합니다.");
                }
            }
            else if (mode == Mode.Sell)
            {
                Console.WriteLine($"판매\n판매할 아이템을 골라주세요.\n\n[보유 골드] {player.Gold} G\n\n[아이템 목록]");
                string[]? sellOption = DisplayInventory(Mode.Sell, "Inventory");

                if (sellOption == null)
                {
                    int input = gameSystem.DigitInput("\n0.나가기\n해당하는 번호를 입력해주세요.\n>>", 0, 0);

                    if (input == 0)
                    {
                        Console.Clear();
                        Store(Mode.View);
                        return;
                    }
                    else
                    {
                        gameSystem.ShowMessage("잘못된 입력입니다.");
                        Store(Mode.Sell);
                    }
                }

                int selection = gameSystem.Select(sellOption, true);

                if (selection >= 1 && selection <= playeritems.Count)
                {
                    Item selectedItem = playeritems[selection - 1];
                    int sellprice = (int)(selectedItem.Price * 0.85);

                    if (gameSystem.Confirm($"{selectedItem.Name}을(를) {sellprice} G에 판매하시겠습니까?"))
                    {
                        if ((int)selectedItem.Type < 3 && selectedItem.IsEquipped)
                        {
                            if (selectedItem.Type == ItemType.Accessory && player.Health < 0)
                            {
                                gameSystem.ShowMessage($"판매할 수 없다.\n{selectedItem.Name}을(를) 판매하면 나는 죽고만다.");
                                Store(Mode.Sell);
                            }
                            selectedItem.IsEquipped = false;
                            player.EquipItem(player, selectedItem.Type, selectedItem.Value, false);
                        }

                        player.Inventory.Remove(selectedItem);
                        foreach (Item item in store.storeItems.Where(item => item.Name == selectedItem.Name))
                        {
                            item.AlreadyBought = false;
                        }
                        player.Gold += sellprice;
                        gameSystem.ShowMessage($"{selectedItem.Name}을(를) 판매했습니다.");
                    }
                }
                else if (selection == 0)
                {
                    Store(Mode.View);
                    return;
                }
            }
        }

        void Rest()
        {
            while (true)
            {
                Console.WriteLine($"휴식하기\n500 G 를 내면 체력을 회복할 수 있습니다.[보유 골드] {player.Gold} G");
                int selection = gameSystem.Select(new string[] { "1.휴식하기" }, true);

                if (selection == 1 && player.Gold < 500)
                {
                    Console.WriteLine("골드가 부족합니다.");
                    continue;
                }
                else if (selection == 1 && player.Health == 100)
                {
                    gameSystem.ShowMessage("휴식 할 필요가 없습니다.");
                }
                else if (selection == 1 && gameSystem.Confirm("500 G 를 내고 휴식하겠습니까?"))
                {
                    int totalHealth = (int)player.Health + player.EquipHealth;
                    gameSystem.ShowMessage($"휴식을 완료했습니다. (체력 : {totalHealth} -> {((player.Health + 50 <= 100) ? totalHealth + 50 : 100 + player.EquipHealth)}");
                    player.Health = ((player.Health + 50 <= 100) ? player.Health + 50 : 100);
                    player.Gold -= 500;
                    continue;
                }
                else if (selection == 0)
                {
                    return;
                }
            }
        }

        void Dungeon()
        {
            Random random = new Random();
            while (true)
            {
                Console.WriteLine("던전입장\n이곳에서 던전으로 들어가기전 활동을 할 수 있습니다.\n");

                List<(string, int, int)> stageList = new List<(string, int, int)>
                {
                    ("쉬운던전", 5, 1000),
                    ("일반던전", 10, 1700),
                    ("어려운던전", 17, 2500)
                };

                string[] stages = new string[stageList.Count];

                for (int i = 0; i < stageList.Count; i++)
                {
                    stages[i] = $"{i + 1}.{stageList[i].Item1}\t| 방어력 {stageList[i].Item2} 이상 권장";
                }

                int selection = gameSystem.Select(stages, true);

                if (selection == 0) return;
                else if (selection == -1) continue;

                int totalAttack = (int)(player.Attack + player.EquipAttack);
                int totalDefence = (int)(player.Defence + player.EquipDefence);
                int totalHealth = (int)(player.Health + player.EquipHealth);

                if ((totalHealth - 35) < 0 && !gameSystem.Confirm("체력이 부족하다. 죽을 수도 있다. 그래도 도전할까?"))
                {
                    gameSystem.ShowMessage("휴식을 취하고 다시오자.");
                    continue;
                }

                if (totalDefence < stageList[selection - 1].Item2)
                {
                    int chance = random.Next(100);
                    if (totalHealth == 1)
                    {
                        player.Health--;
                        GameOver(1);
                    }
                    else if (chance < 40)
                    {
                        gameSystem.ShowMessage(".");
                        gameSystem.ShowMessage("..");
                        gameSystem.ShowMessage("...");
                        gameSystem.ShowMessage($"던전 공략 실패!\n\n보상없음.\n체력을 {(int)(player.Health + player.EquipHealth) / 2} 만큼 잃었습니다!");
                        player.Health -= (int)((player.Health + player.EquipHealth) / 2);
                        continue;
                    }
                }

                int originalGold = (int)player.Gold;
                int healthLost = (random.Next(20, 36)) - (totalDefence - stageList[selection - 1].Item2);
                float bonus = (random.Next(totalAttack, (totalAttack * 2) + 1)) / 100;

                player.Health -= healthLost;
                player.Gold += (stageList[selection - 1].Item3 + (int)(stageList[selection - 1].Item3 * bonus));

                player.Level++;
                player.Attack += 0.5f;
                player.Defence++;

                if (totalHealth - healthLost < 0) GameOver(healthLost);

                while (true)
                {
                    gameSystem.ShowMessage(".");
                    gameSystem.ShowMessage("..");
                    gameSystem.ShowMessage("...");
                    Console.WriteLine($"던전 클리어\n축하합니다!!\n" +
                    $"{stageList[selection - 1].Item1}을 클리어 하였습니다.\n\n[탐험결과]\n" +
                    $"체력 {totalHealth} -> {totalHealth - healthLost}\n" +
                    $"Gold {originalGold} -> {player.Gold}");

                    int input = gameSystem.DigitInput("\n0.나가기\n해당하는 번호를 입력해주세요.\n>>", 0, 0);

                    if (input == 0)
                    {
                        Console.Clear();
                        break;
                    }
                    else gameSystem.ShowMessage("잘못된 입력입니다.");
                }
            }
        }

        void GameOver(int healthLost)
        {
            gameSystem.ShowMessage(".");
            gameSystem.ShowMessage("..");
            gameSystem.ShowMessage("...");
            gameSystem.ShowMessage("앗...!");
            Console.WriteLine($"당신은 죽었습니다.\n체력을 {healthLost}만큼 잃어 {player.Health + player.EquipHealth}이(가) 되었습니다.\n[당신의 최종 상태]\n");

            player.Health = 100;

            player.DisplayPlayerStats(player);

            Console.WriteLine("아무 키나 누르면 종료됩니다...");
            Console.ReadKey();
            Environment.Exit(0);
        }

        string[]? DisplayInventory(Mode mode, string where)
        {
            List<Item> itemsSource;

            if (where == "Inventory")
            {
                itemsSource = player.Inventory;
                if (itemsSource.Count == 0)
                {
                    Console.WriteLine("인벤토리가 비어 있습니다.");
                    return null;
                }
            }
            else if (where == "Store")
            {
                itemsSource = store.storeItems;
            }
            else
            {
                Console.WriteLine("잘못된 위치입니다.");
                return null;
            }

            string[] items = new string[itemsSource.Count];

            for (int i = 0; i < itemsSource.Count; i++)
            {
                Item item = itemsSource[i];
                string prefix = mode == Mode.View ? "- " : $"{i + 1}. ";
                string equippedMark = (where == "Inventory" && item.IsEquipped) ? "[E]" : "";
                string stat = item.Type switch
                {
                    ItemType.Weapon => $"공격력 +{item.Value}",
                    ItemType.Armor => $"방어력 +{item.Value}",
                    ItemType.Accessory => $"추가체력 +{item.Value}",
                    ItemType.Etc => "기타\t",
                    _ => $"\t{(item.Value > 0 ? item.Value.ToString() : "")}"
                };
                string priceLabel = ((where == "Inventory" && mode == Mode.Sell) ? $"{(int)(item.Price * 0.85)} G" : (where == "Store" && item.AlreadyBought) ? "구매완료" : $"{item.Price} G");

                items[i] = $"{prefix}{equippedMark}{item.Name}\t| {stat}\t| {item.Description}\t| {priceLabel}";

                if (mode == Mode.View)
                {
                    Console.WriteLine(items[i]);
                }
            }

            return items;
        }
    }
}