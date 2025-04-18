using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TextRPG.GameData;
using static TextRPG.TextRPG;

namespace TextRPG
{
    class GameSystem
    {

        public void ShowMessage(string message)
        {
            Console.WriteLine(message);
            Thread.Sleep(1000);
            Console.Clear();
        }

        public int DigitInput(string question, int min, int max)
        {
            Console.Write(question);
            if (int.TryParse(Console.ReadLine(), out int input) && input >= min && input <= max)
                return input;
            return -1;
        }

        public bool Confirm(string question)
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

        public int Select(string[] options, bool hasExit)
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
