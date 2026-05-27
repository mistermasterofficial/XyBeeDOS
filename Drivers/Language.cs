using Cosmos.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sys = Cosmos.System;

namespace XyBeeDOS.Drivers
{
    class Language
    {
        static int current_lang_num = 0;
        static ScanMapBase[] scanMaps = new ScanMapBase[] { new Sys.ScanMaps.US_Standard(), new RU_Standart() };

        public static int LanguageCount() { return scanMaps.Length; }

        public static void SetLanguage(int lang_num)
        {
            Sys.KeyboardManager.SetKeyLayout(scanMaps[lang_num]);
            current_lang_num = lang_num;
        }

        public static void SwitchLanguage()
        {
            SetLanguage((current_lang_num+1)%2);
        }

        public static string GetName(int lang_num)
        {
            switch(lang_num)
            {
                case 0:
                    return "english";
                case 1:
                    return "русский";
                default:
                    throw new ArgumentException();
            }
        }

        public static int GetCurrentLanguageNum()
        {
            return current_lang_num;
        }
    }

    internal class RU_Standart : Sys.ScanMapBase
    {
        protected override void InitKeys()
        {
            _keys = new List<KeyMapping>(100);
            _keys.Add(new KeyMapping(0, ConsoleKeyEx.NoName));
            _keys.Add(new KeyMapping(1, ConsoleKeyEx.Escape));
            _keys.Add(new KeyMapping(2, '1', '!', '1', '1', '!', '1', ConsoleKeyEx.D1));
            _keys.Add(new KeyMapping(3, '2', '"', '2', '2', '"', '2', ConsoleKeyEx.D2));
            _keys.Add(new KeyMapping(4, '3', '№', '3', '3', '№', '3', ConsoleKeyEx.D3));
            _keys.Add(new KeyMapping(5, '4', ';', '4', '4', ';', '4', ConsoleKeyEx.D4));
            _keys.Add(new KeyMapping(6, '5', '%', '5', '5', '%', '5', ConsoleKeyEx.D5));
            _keys.Add(new KeyMapping(7, '6', ':', '6', '6', ':', '6', ConsoleKeyEx.D6));
            _keys.Add(new KeyMapping(8, '7', '?', '7', '7', '?', '7', ConsoleKeyEx.D7));
            _keys.Add(new KeyMapping(9, '8', '*', '8', '8', '*', '8', ConsoleKeyEx.D8));
            _keys.Add(new KeyMapping(10, '9', '(', '9', '9', '(', '9', ConsoleKeyEx.D9));
            _keys.Add(new KeyMapping(11, '0', ')', '0', '0', ')', '0', ConsoleKeyEx.D0));
            _keys.Add(new KeyMapping(12, '-', '_', '-', '-', '_', '-', ConsoleKeyEx.Minus));
            _keys.Add(new KeyMapping(13, '=', '+', '=', '=', '+', '=', ConsoleKeyEx.Equal));
            _keys.Add(new KeyMapping(14, ConsoleKeyEx.Backspace));
            _keys.Add(new KeyMapping(15, ConsoleKeyEx.Tab));
            _keys.Add(new KeyMapping(16, 'й', 'Й', 'й', 'Й', 'й', 'Й', ConsoleKeyEx.Q));
            _keys.Add(new KeyMapping(17, 'ц', 'Ц', 'ц', 'Ц', 'ц', 'Ц', ConsoleKeyEx.W));
            _keys.Add(new KeyMapping(18, 'у', 'У', 'у', 'У', 'у', 'У', ConsoleKeyEx.E));
            _keys.Add(new KeyMapping(19, 'к', 'К', 'к', 'К', 'к', 'К', ConsoleKeyEx.R));
            _keys.Add(new KeyMapping(20, 'е', 'Е', 'е', 'Е', 'е', 'Е', ConsoleKeyEx.T));
            _keys.Add(new KeyMapping(21, 'н', 'Н', 'н', 'Н', 'н', 'Н', ConsoleKeyEx.Y));
            _keys.Add(new KeyMapping(22, 'г', 'Г', 'г', 'Г', 'г', 'Г', ConsoleKeyEx.U));
            _keys.Add(new KeyMapping(23, 'ш', 'Ш', 'ш', 'Ш', 'ш', 'Ш', ConsoleKeyEx.I));
            _keys.Add(new KeyMapping(24, 'щ', 'Щ', 'щ', 'Щ', 'щ', 'Щ', ConsoleKeyEx.O));
            _keys.Add(new KeyMapping(25, 'з', 'З', 'з', 'З', 'з', 'З', ConsoleKeyEx.P));
            _keys.Add(new KeyMapping(26, 'х', 'Х', 'х', 'Х', 'х', 'Х', ConsoleKeyEx.LBracket));
            _keys.Add(new KeyMapping(27, 'ъ', 'Ъ', 'ъ', 'Ъ', 'ъ', 'Ъ', ConsoleKeyEx.RBracket));
            _keys.Add(new KeyMapping(28, ConsoleKeyEx.Enter));
            _keys.Add(new KeyMapping(29, ConsoleKeyEx.LCtrl));
            _keys.Add(new KeyMapping(30, 'ф', 'Ф', 'ф', 'Ф', 'ф', 'Ф', ConsoleKeyEx.A));
            _keys.Add(new KeyMapping(31, 'ы', 'Ы', 'ы', 'Ы', 'ы', 'Ы', ConsoleKeyEx.S));
            _keys.Add(new KeyMapping(32, 'в', 'В', 'в', 'В', 'в', 'В', ConsoleKeyEx.D));
            _keys.Add(new KeyMapping(33, 'а', 'А', 'а', 'А', 'а', 'А', ConsoleKeyEx.F));
            _keys.Add(new KeyMapping(34, 'п', 'П', 'п', 'П', 'п', 'П', ConsoleKeyEx.G));
            _keys.Add(new KeyMapping(35, 'р', 'Р', 'р', 'Р', 'р', 'Р', ConsoleKeyEx.H));
            _keys.Add(new KeyMapping(36, 'о', 'О', 'о', 'О', 'о', 'О', ConsoleKeyEx.J));
            _keys.Add(new KeyMapping(37, 'л', 'Л', 'л', 'Л', 'л', 'Л', ConsoleKeyEx.K));
            _keys.Add(new KeyMapping(38, 'д', 'Д', 'д', 'Д', 'д', 'Д', ConsoleKeyEx.L));
            _keys.Add(new KeyMapping(39, 'ж', 'Ж', 'ж', 'Ж', 'ж', 'Ж', ConsoleKeyEx.Semicolon));
            _keys.Add(new KeyMapping(40, 'э', 'Э', 'э', 'Э', 'э', 'Э', ConsoleKeyEx.Apostrophe));
            _keys.Add(new KeyMapping(41, 'ё', 'Ё', 'ё', 'Ё', 'ё', 'Ё', ConsoleKeyEx.Backquote));
            _keys.Add(new KeyMapping(42, ConsoleKeyEx.LShift));
            _keys.Add(new KeyMapping(43, '\\', '|', '\\', '\\', '|', '|', ConsoleKeyEx.Backslash));
            _keys.Add(new KeyMapping(44, 'я', 'Я', 'я', 'Я', 'я', 'Я', ConsoleKeyEx.Z));
            _keys.Add(new KeyMapping(45, 'ч', 'Ч', 'ч', 'Ч', 'ч', 'Ч', ConsoleKeyEx.X));
            _keys.Add(new KeyMapping(46, 'с', 'С', 'с', 'С', 'с', 'С', ConsoleKeyEx.C));
            _keys.Add(new KeyMapping(47, 'м', 'М', 'м', 'М', 'м', 'М', ConsoleKeyEx.V));
            _keys.Add(new KeyMapping(48, 'и', 'И', 'и', 'И', 'и', 'И', ConsoleKeyEx.B));
            _keys.Add(new KeyMapping(49, 'т', 'Т', 'т', 'Т', 'т', 'Т', ConsoleKeyEx.N));
            _keys.Add(new KeyMapping(50, 'ь', 'Ь', 'ь', 'Ь', 'ь', 'Ь', ConsoleKeyEx.M));
            _keys.Add(new KeyMapping(51, 'б', 'Б', 'б', 'Б', 'б', 'Б', ConsoleKeyEx.Comma));
            _keys.Add(new KeyMapping(52, 'ю', 'Ю', 'ю', 'Ю', 'ю', 'Ю', ConsoleKeyEx.Period));
            _keys.Add(new KeyMapping(53, '.', ',', '.', ',', '.', ',', ConsoleKeyEx.Slash));
            _keys.Add(new KeyMapping(54, ConsoleKeyEx.RShift));
            _keys.Add(new KeyMapping(55, '*', '*', '*', '*', '*', '*', ConsoleKeyEx.NumMultiply));
            _keys.Add(new KeyMapping(56, ConsoleKeyEx.LAlt));
            _keys.Add(new KeyMapping(57, ' ', ConsoleKeyEx.Spacebar));
            _keys.Add(new KeyMapping(58, ConsoleKeyEx.CapsLock));
            _keys.Add(new KeyMapping(59, ConsoleKeyEx.F1));
            _keys.Add(new KeyMapping(60, ConsoleKeyEx.F2));
            _keys.Add(new KeyMapping(61, ConsoleKeyEx.F3));
            _keys.Add(new KeyMapping(62, ConsoleKeyEx.F4));
            _keys.Add(new KeyMapping(63, ConsoleKeyEx.F5));
            _keys.Add(new KeyMapping(64, ConsoleKeyEx.F6));
            _keys.Add(new KeyMapping(65, ConsoleKeyEx.F7));
            _keys.Add(new KeyMapping(66, ConsoleKeyEx.F8));
            _keys.Add(new KeyMapping(67, ConsoleKeyEx.F9));
            _keys.Add(new KeyMapping(68, ConsoleKeyEx.F10));
            _keys.Add(new KeyMapping(87, ConsoleKeyEx.F11));
            _keys.Add(new KeyMapping(88, ConsoleKeyEx.F12));
            _keys.Add(new KeyMapping(69, ConsoleKeyEx.NumLock));
            _keys.Add(new KeyMapping(70, ConsoleKeyEx.ScrollLock));
            _keys.Add(new KeyMapping(71, '\0', '\0', '7', '\0', '\0', '\0', ConsoleKeyEx.Home, ConsoleKeyEx.Num7));
            _keys.Add(new KeyMapping(72, '\0', '\0', '8', '\0', '\0', '\0', ConsoleKeyEx.UpArrow, ConsoleKeyEx.Num8));
            _keys.Add(new KeyMapping(73, '\0', '\0', '9', '\0', '\0', '\0', ConsoleKeyEx.PageUp, ConsoleKeyEx.Num9));
            _keys.Add(new KeyMapping(74, '-', '-', '-', '-', '-', '-', ConsoleKeyEx.NumMinus));
            _keys.Add(new KeyMapping(75, '\0', '\0', '4', '\0', '\0', '\0', ConsoleKeyEx.LeftArrow, ConsoleKeyEx.Num4));
            _keys.Add(new KeyMapping(76, '\0', '\0', '5', '\0', '\0', '\0', ConsoleKeyEx.Num5));
            _keys.Add(new KeyMapping(77, '\0', '\0', '6', '\0', '\0', '\0', ConsoleKeyEx.RightArrow, ConsoleKeyEx.Num6));
            _keys.Add(new KeyMapping(78, '+', '+', '+', '+', '+', '+', ConsoleKeyEx.NumPlus));
            _keys.Add(new KeyMapping(79, '\0', '\0', '1', '\0', '\0', '\0', ConsoleKeyEx.End, ConsoleKeyEx.Num1));
            _keys.Add(new KeyMapping(80, '\0', '\0', '2', '\0', '\0', '\0', ConsoleKeyEx.DownArrow, ConsoleKeyEx.Num2));
            _keys.Add(new KeyMapping(81, '\0', '\0', '3', '\0', '\0', '\0', ConsoleKeyEx.PageDown, ConsoleKeyEx.Num3));
            _keys.Add(new KeyMapping(82, '\0', '\0', '0', '\0', '\0', '\0', ConsoleKeyEx.Insert, ConsoleKeyEx.Num0));
            _keys.Add(new KeyMapping(83, '\0', '\0', '.', '\0', '\0', '\0', ConsoleKeyEx.Delete, ConsoleKeyEx.NumPeriod));
            _keys.Add(new KeyMapping(91, ConsoleKeyEx.LWin));
            _keys.Add(new KeyMapping(92, ConsoleKeyEx.RWin));
        }
    }
}
