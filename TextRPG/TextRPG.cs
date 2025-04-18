using Newtonsoft.Json;
using static TextRPG.TextRPG;

namespace TextRPG
{
    internal class TextRPG
    {
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
        }

        public class PlayerInfo
        {
            public string Name { get; set; }
            public string Job { get; set; }
            public float[] PlayerState { get; set; } = { 1, 10, 5, 100, 1500 };
            public int[] EquiptState { get; set; } = { 0, 0, 0 };
            public List<Item> Inventory { get; set; } = new List<Item>
            {
                new Item("초심자의 검", ItemType.Weapon, 3, "철로 만들어진 보급형 검.", 500),
                new Item("초심자의 갑옷", ItemType.Armor, 3, "철로 만들어진 보급형 값옷.", 500),
                new Item("추억의 목걸이", ItemType.Accessory, 10, "사실 길가에서 주웠습니다.", 2500)
            };
        }

        public class StoreInfo
        {
            public List<Item> storeItems { get; set; } = new List<Item>
            {
                new Item("수련자 갑옷", ItemType.Armor, 5, "수련에 도움을 주는 갑옷입니다.", 1000),
                new Item("무쇠갑옷", ItemType.Armor, 9, "무쇠로 만들어져 튼튼한 갑옷입니다.", 2000),
                new Item("스파르타의 갑옷", ItemType.Armor, 15, "스파르타의 전사들이 사용했다는 전설의 갑옷입니다.", 3500),
                new Item("낡은 검", ItemType.Weapon, 2, "쉽게 볼 수 있는 낡은 검 입니다.", 600),
                new Item("청동 도끼", ItemType.Weapon, 5, "어디선가 사용됐던거 같은 도끼입니다.", 1500),
                new Item("스파르타의 창", ItemType.Weapon, 7, "스파르타의 전사들이 사용했다는 전설의 창입니다.", 3000)
            };
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

        PlayerInfo player = new PlayerInfo();
        StoreInfo store = new StoreInfo();

        static void Main()
        {
            new TextRPG().Start();
        }

        void Start()
        {
            Console.WriteLine("여기는 텍스트 던전의 세계.");

            while (true)
            {
                Console.Write("이곳에 들어온 당신의 이름을 알려주세요.\n\n>>");
                string input = Console.ReadLine();

                if (Confirm($"{input}... 당신의 이름이 맞나요?"))
                {
                    player.Name = input;
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

                int selection = Select(jobOptions, false);

                if (selection != -1 && Confirm($"{jobs[selection - 1]}... 당신의 직업이 맞나요?"))
                {
                    player.Job = jobs[selection - 1];
                    break;
                }
            }

            ShowMessage($"환영합니다, {player.Job} {player.Name}.");
            Town();
        }

        void Town()
        {
            while (true)
            {
                Console.WriteLine("마을에 오신걸 환영합니다.\n이곳에서 던전에 들어가기 전 행동을 할 수 있습니다.\n");

                int selection = Select(new string[] { "1.상태보기", "2.인벤토리", "3.상점", "4.휴식하기", "5.던전입장" }, false);

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
            while (true)
            {
                Console.WriteLine($"상태 보기\n캐릭터의 정보가 표시됩니다.\n");

                string[] stats = { "Lv. ", "공격력 \t: ", "방어력 \t: ", "체력 \t: ", "Gold \t: " };
                Console.WriteLine($"{stats[0]}{player.PlayerState[0]}\n{player.Name} ({player.Job})");

                for (int i = 1; i < 5; i++)
                {
                    Console.WriteLine($"{stats[i]}{player.PlayerState[i] + (i < 4 ? player.EquiptState[i - 1] : 0)} "
                        + $"{(i < 4 ? (player.EquiptState[i - 1] != 0 ?
                        $"(+{player.EquiptState[i - 1]})" : "") : "")}"
                        + (i == 4 ? " G" : ""));
                }
                Console.WriteLine();

                int input = Select(new string[] { "1.저장하기", "2.불러오기" }, true);

                switch (input)
                {
                    case 1 :
                        if(Confirm("현재 상태를 저장합니다. 확실합니까?(기존의 저장은 삭제됩니다.)"))
                        {
                            SaveManager.SaveGame(player, store);
                        }
                        break;
                    case 2 :
                        if (Confirm("저장된 파일을 불러옵니다. 확실합니까?(지금까지의 진행은 저장되지 않습니다.)"))
                        {
                            (PlayerInfo loadedPlayer, StoreInfo loadedStore) = SaveManager.LoadGame();

                            if (loadedPlayer != null && loadedStore != null)
                            {
                                player.Inventory.Clear();
                                Array.Clear(player.PlayerState, 0, player.PlayerState.Length);
                                Array.Clear(player.EquiptState, 0, player.EquiptState.Length);
                                store.storeItems.Clear();

                                player = loadedPlayer;
                                store = loadedStore;

                                ShowMessage($"{loadedPlayer.Name}의 정보를 불러왔습니다.");
                            }
                        }
                        break;
                    case 0:
                        Console.Clear();
                        return;
                    default :
                        ShowMessage("잘못된 입력입니다.");
                        break;
                }

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
            while (true)
            {
                Console.WriteLine(mode == Mode.View ?
                    "인벤토리\n보유 중인 아이템을 관리할 수 있습니다.\n\n[아이템 목록]" :
                    "장착 관리\n보유 중인 아이템을 관리할 수 있습니다.\n\n[아이템 목록]");

                string[]? items = DisplayInventory(mode, "Inventory");

                if (mode == Mode.View)
                {
                    Console.WriteLine();
                    int select = Select(new string[] { "1.장착관리" }, true);

                    if (items == null && select == 1)
                    {
                        ShowMessage("아이템이 없어 관리할 수 없습니다..");
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
                    int choice = Select(items, true);

                    if (choice == 0)
                    {
                        return;
                    }

                    int index = choice - 1;

                    if (index >= 0 && index < player.Inventory.Count)
                    {
                        Item selectedItem = player.Inventory[index];
                        int slot = (int)selectedItem.Type;

                        if (slot == (int)ItemType.Etc)
                        {
                            ShowMessage("\n장착할 수 없는 아이템입니다.");
                        }
                        else if (selectedItem.IsEquipped)
                        {
                            if(selectedItem.Type == ItemType.Accessory && player.PlayerState[3] < 0)
                            {
                                ShowMessage($"해제할 수 없다.\n{selectedItem.Name}을(를) 해제하면 나는 죽고만다.");
                                continue;
                            }
                            selectedItem.IsEquipped = false;
                            player.EquiptState[slot] -= selectedItem.Value;
                            ShowMessage($"{selectedItem.Name}을(를) 해제했습니다.");
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
                            player.EquiptState[slot] += selectedItem.Value;
                            ShowMessage($"{selectedItem.Name}을(를) 장착했습니다.");
                        }
                    }
                }
                else
                {
                    ShowMessage("잘못된 입력입니다.");
                }
            }
        }

        void Store(Mode mode)
        {
            List<Item> storeItems = store.storeItems;
            List<Item> playeritems = player.Inventory;

            while (true)
            {
                Console.Write((mode == Mode.View ? "상점\n필요한 아이템을 얻을 수 있는 상점입니다." : (mode == Mode.Interaction ? "구매\n필요한 아이템을 골라주세요." : "")) +
                    (mode != Mode.Sell ? $"\n\n[보유 골드] {player.PlayerState[4]} G\n\n[아이템 목록]\n" : ""));
                string[]? items = DisplayInventory(mode, "Store");


                if (mode == Mode.View)
                {
                    Console.WriteLine();
                    int select = Select(new string[] { "1.구매하기", "2.판매하기" }, true);

                    if (select == 1)
                    {
                        Store(Mode.Interaction);
                        return;
                    }
                    else if (select == 2)
                    {
                        Store(Mode.Sell);
                        return;
                    }
                    else if (select == 0)
                    {
                        return;
                    }
                }
                else if (mode == Mode.Interaction)
                {
                    int choice = Select(items, true);

                    if (choice == 0)
                    {
                        Store(Mode.View);
                        return;
                    }
                    else if (choice >= 1 && choice <= storeItems.Count)
                    {
                        Item selectedItem = storeItems[choice - 1];

                        if (selectedItem.AlreadyBought)
                        {
                            ShowMessage("이미 구매한 아이템 입니다.");
                        }
                        else if (player.PlayerState[4] >= selectedItem.Price)
                        {
                            if (Confirm($"{selectedItem.Name}을(를) {selectedItem.Price} G에 구매하시겠습니까?"))
                            {
                                player.Inventory.Add(new Item(selectedItem.Name, selectedItem.Type, selectedItem.Value, selectedItem.Description, selectedItem.Price));
                                selectedItem.AlreadyBought = true;
                                player.PlayerState[4] -= selectedItem.Price;
                                ShowMessage($"{selectedItem.Name}을(를) 구매했습니다.");
                            }
                        }
                        else ShowMessage("골드가 부족합니다.");
                    }
                }
                else if (mode == Mode.Sell)
                {
                    while (true)
                    {
                        Console.WriteLine($"판매\n판매할 아이템을 골라주세요.\n\n[보유 골드] {player.PlayerState[4]} G\n\n[아이템 목록]");
                        string[]? sellOption = DisplayInventory(Mode.Sell, "Inventory");

                        if (sellOption == null)
                        {
                            int input = DigitInput("\n0.나가기\n해당하는 번호를 입력해주세요.\n>>", 0, 0);

                            if (input == 0)
                            {
                                Console.Clear();
                                Store(Mode.View);
                                return;
                            }
                            else
                            {
                                ShowMessage("잘못된 입력입니다.");
                                continue;
                            }
                        }

                        int choice = Select(sellOption, true);

                        if (choice >= 1 && choice <= playeritems.Count)
                        {
                            Item selectedItem = playeritems[choice - 1];
                            int sellprice = (int)(selectedItem.Price * 0.85);

                            if (Confirm($"{selectedItem.Name}을(를) {sellprice} G에 판매하시겠습니까?"))
                            {
                                if ((int)selectedItem.Type < 3 && selectedItem.IsEquipped)
                                {
                                    if (selectedItem.Type == ItemType.Accessory && player.PlayerState[3] < 0)
                                    {
                                        ShowMessage($"판매할 수 없다.\n{selectedItem.Name}을(를) 판매하면 나는 죽고만다.");
                                        continue;
                                    }
                                    selectedItem.IsEquipped = false;
                                    player.EquiptState[((int)selectedItem.Type)] -= selectedItem.Value;

                                }

                                player.Inventory.Remove(selectedItem);
                                foreach (Item item in store.storeItems.Where(item => item.Name == selectedItem.Name))
                                {
                                    item.AlreadyBought = false;
                                }
                                player.PlayerState[4] += sellprice;
                                ShowMessage($"{selectedItem.Name}을(를) 판매했습니다.");
                            }
                        }
                        else if(choice == 0)
                        {
                            Store(Mode.View);
                            return;
                        }
                    }
                }
            }
        }

        void Rest()
        {
            while (true)
            {
                Console.WriteLine($"휴식하기\n500 G 를 내면 체력을 회복할 수 있습니다.[보유 골드] {player.PlayerState[4]} G");
                int selection = Select(new string[] { "1.휴식하기" }, true);

                if (selection == 1 && player.PlayerState[4] < 500)
                {
                    Console.WriteLine("골드가 부족합니다.");
                    continue;
                }
                else if (selection == 1 && player.PlayerState[3] == 100)
                {
                    ShowMessage("휴식 할 필요가 없습니다.");
                }
                else if (selection == 1 && Confirm("500 G 를 내고 휴식하겠습니까?"))
                {
                    int totalHealth = (int)player.PlayerState[3] + player.EquiptState[2];
                    ShowMessage($"휴식을 완료했습니다. (체력 : {totalHealth} -> {((player.PlayerState[3] + 50 <= 100) ? totalHealth + 50 : 100 + player.EquiptState[2])}");
                    player.PlayerState[3] = ((player.PlayerState[3] + 50 <= 100) ? player.PlayerState[3] + 50 : 100);
                    player.PlayerState[4] -= 500;
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

                int selection = Select(stages, true);

                if (selection == 0) return;
                else if (selection == -1) continue;

                int totalAttack = (int)(player.PlayerState[1] + player.EquiptState[0]);
                int totalDefence = (int)(player.PlayerState[2] + player.EquiptState[1]);
                int totalHealth = (int)(player.PlayerState[3] + player.EquiptState[2]);

                if ((totalHealth - 35) < 0 && !Confirm("체력이 부족하다. 죽을 수도 있다. 그래도 도전할까?"))
                {
                    ShowMessage("휴식을 취하고 다시오자.");
                    continue;
                }

                if (totalDefence < stageList[selection - 1].Item2)
                {
                    int chance = random.Next(100);
                    if(totalHealth == 1)
                    {
                        player.PlayerState[3]--;
                        GameOver(1);
                    }
                    else if (chance < 40)
                    {
                        ShowMessage(".");
                        ShowMessage("..");
                        ShowMessage("...");
                        ShowMessage($"던전 공략 실패!\n\n보상없음.\n체력을 {(int)(player.PlayerState[3] + player.EquiptState[2]) / 2} 만큼 잃었습니다!");
                        player.PlayerState[3] -= (int)((player.PlayerState[3] + player.EquiptState[2]) / 2);
                        continue;
                    }
                }

                int originalGold = (int)player.PlayerState[4];
                int healthLost = (random.Next(20, 36)) - (totalDefence - stageList[selection - 1].Item2);
                float bonus = (random.Next(totalAttack, (totalAttack * 2) + 1)) / 100;

                player.PlayerState[3] -= healthLost;
                player.PlayerState[4] += (stageList[selection - 1].Item3 + (int)(stageList[selection - 1].Item3 * bonus));
                for (int i = 0; i < 3; i++)
                {
                    if (i == 1)
                    {
                        player.PlayerState[i] += 0.5f;
                    }
                    player.PlayerState[i]++;
                }

                if (totalHealth - healthLost < 0) GameOver(healthLost);

                while (true)
                {
                    ShowMessage(".");
                    ShowMessage("..");
                    ShowMessage("...");
                    Console.WriteLine($"던전 클리어\n축하합니다!!\n" +
                    $"{stageList[selection - 1].Item1}을 클리어 하였습니다.\n\n[탐험결과]\n" +
                    $"체력 {totalHealth} -> {totalHealth - healthLost}\n" +
                    $"Gold {originalGold} -> {player.PlayerState[4]}");

                    int input = DigitInput("\n0.나가기\n해당하는 번호를 입력해주세요.\n>>", 0, 0);

                    if (input == 0)
                    {
                        Console.Clear();
                        break;
                    }
                    else ShowMessage("잘못된 입력입니다.");
                }
            }
        }

        void GameOver(int healthLost)
        {
            ShowMessage(".");
            ShowMessage("..");
            ShowMessage("...");
            ShowMessage("앗...!");
            Console.WriteLine($"당신은 죽었습니다.\n체력을 {healthLost}만큼 잃어 {player.PlayerState[3] + player.EquiptState[2]}이(가) 되었습니다.\n[당신의 최종 상태]\n");
            player.PlayerState[3] = 100;
            string[] stats = { "Lv. ", "공격력 \t: ", "방어력 \t: ", "체력 \t: ", "Gold \t: " };
            Console.WriteLine($"{stats[0]}{player.PlayerState[0]}\n{player.Name} ({player.Job})");

            for (int i = 1; i < 5; i++)
            {
                Console.WriteLine($"{stats[i]}{player.PlayerState[i] + (i < 4 ? player.EquiptState[i - 1] : 0)} "
                    + $"{(i < 4 ? (player.EquiptState[i - 1] != 0 ?
                    $"(+{player.EquiptState[i - 1]})" : "") : "")}"
                    + (i == 4 ? " G" : ""));
            }
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


        int DigitInput(string question, int min, int max)
        {
            Console.Write(question);
            if (int.TryParse(Console.ReadLine(), out int input) && input >= min && input <= max)
                return input;
            return -1;
        }

        void ShowMessage(string message)
        {
            Console.WriteLine(message);
            Thread.Sleep(1000);
            Console.Clear();
        }

        bool Confirm(string question)
        {
            while (true)
            {
                Console.Clear();
                int input = DigitInput($"{question}\n1.맞다\t2.아니다\n\n>>", 1, 2);

                if (input == 1)
                {
                    Console.Clear();
                    return true;
                }
                else if (input == 2)
                {
                    Console.Clear();
                    return false;
                }

                ShowMessage("잘못된 입력입니다.");
            }

        }

        int Select(string[] options, bool hasExit)
        {
            for (int i = 0; i < options.Length; i++)
            {
                Console.WriteLine($"{options[i]}\t");
            }
            int input = DigitInput($"{(hasExit ? "\n0.나가기" : "")}\n해당하는 번호를 입력해주세요.\n>>", 0, options.Length);

            if ((input >= 1 && input <= options.Length) || (hasExit && input == 0))
            {
                Console.Clear();
                return input;
            }
            else
            {
                ShowMessage("잘못된 입력입니다.");
                return -1;
            }
        }
    }
}