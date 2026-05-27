using System;
using System.IO;
using System.Text;

namespace XyBeeDOS.Drivers
{
    public class SingleByteEncoding : Encoding
    {
        public byte[] FontArray8;
        public byte[] FontArray16;

        public char[] CodePageTable { get; set; }

        private const byte ReplacementChar = (byte)'?';

        public override bool IsSingleByte => true;

        public override int GetByteCount(char[] chars, int index, int count)
        {
            // Validate input parameters
            if (chars == null)
            {
                throw new ArgumentNullException(nameof(chars));
            }

            if (index < 0 || count < 0)
            {
                throw new ArgumentOutOfRangeException(index < 0 ? nameof(index) : nameof(count), "negative number");
            }

            if (chars.Length - index < count)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "The 'count' parameter is greater than the length of the 'chars' array.");
            }

            // If no input, return 0, avoid fixed empty array problem
            if (count == 0)
            {
                return 0;
            }

            // If no input just return 0, fixed doesn't like 0 length arrays
            if (count == 0)
            {
                return 0;
            }

            //return chars.Length - index - count;
            return count - index;
        }

        private int GetCodePageIdxFromChr(char ch)
        {
            int idx;

            /* IL2CPU bug again with interfaces :-( let's do it manually... */
            //idx = Array.IndexOf<char>(CodePageTable, ch);

            for (idx = 0; idx < CodePageTable.Length; idx++)
            {
                if (CodePageTable[idx] == ch)
                {
                    break;
                }
            }

            // All CodePageTable searched, nothing found!
            return idx == CodePageTable.Length ? -1 : idx + 128;
        }

        public byte GetByte(char ch)
        {
            /* ch is in reality an ASCII character? */
            if (ch < 127)
            {
                return (byte)ch;
            }

            int idx = GetCodePageIdxFromChr(ch);
            if (idx == -1)
            {
                return ReplacementChar;
            }

            return (byte)idx;
        }

        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            // Validate input parameters
            if (chars == null)
            {
                throw new ArgumentNullException(nameof(chars));
            }

            if (charIndex < 0 || charCount < 0)
            {
                throw new ArgumentOutOfRangeException(charIndex < 0 ? nameof(charIndex) : nameof(charCount), "A negative number was given.");
            }

            if (chars.Length - charIndex < charCount)
            {
                throw new ArgumentOutOfRangeException(nameof(chars), "'count' is greater than the length of the array.");
            }

            for (int i = charIndex; i < charCount; i++)
            {
                bytes[byteIndex + i] = GetByte(chars[i]);
            }

            return bytes.Length;
        }

        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            // Validate Parameters
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            if (index < 0 || count < 0)
            {
                throw new ArgumentOutOfRangeException(index < 0 ? nameof(index) : nameof(count), "The given number was negative.");
            }

            if (bytes.Length - index < count)
            {
                throw new ArgumentOutOfRangeException(nameof(bytes), "count more that what is in array");
            }

            // If no input just return 0, fixed doesn't like 0 length arrays
            return count == 0 ? 0 : count - index;
        }

        public char GetChar(byte b)
        {
            /* Ascii? Simply cast it then... */
            if (b < 127)
            {
                return (char)b;
            }

            return CodePageTable[b - 128];
        }

        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            // Validate Parameters
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            if (byteIndex < 0 || byteCount < 0)
            {
                throw new ArgumentOutOfRangeException(byteIndex < 0 ? nameof(byteIndex) : nameof(byteCount), "The given number was negative.");
            }

            if (bytes.Length - byteIndex < byteCount)
            {
                throw new ArgumentOutOfRangeException(nameof(bytes), "'count' is greater than the length of the array.");
            }

            // If no input just return 0, fixed doesn't like 0 length arrays
            if (byteCount == 0)
            {
                return 0;
            }

            for (int i = byteIndex; i < byteCount; i++)
            {
                chars[charIndex + i] = GetChar(bytes[i]);
            }

            return chars.Length;
        }

        public override int GetMaxByteCount(int charCount)
        {
            if (charCount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(charCount), "The given number was negative.");
            }

            // Characters would be # of characters + 1 in case high surrogate is ? * max fallback
            return charCount + 1;
        }

        public override int GetMaxCharCount(int byteCount)
        {
            // Just return length, SBCS stay the same length because they don't map to surrogate
            return byteCount;
        }
    }

    class CodePageTables
    {
        static readonly char[] CP437 = new char[] {
                'Ç' , 'ü' , 'é' , 'â' , 'ä' , 'à' , 'å' , 'ç' , 'ê' , 'ë' , 'è' , 'ï' , 'î' , 'ì' , 'Ä' , 'Å' ,
                'É' , 'æ' , 'Æ' , 'ô' , 'ö' , 'ò' , 'û' , 'ù' , 'ÿ' , 'Ö' , 'Ü' , '¢' , '£' , '¥' , '₧' , 'ƒ' ,
                'á' , 'í' , 'ó' , 'ú' , 'ñ' , 'Ñ' , 'ª' , 'º' , '¿' , '⌐' , '¬' , '½' , '¼' , '¡' , '«' , '»' ,
                '░' , '▒' , '▓' , '│' , '┤' , '╡' , '╢' , '╖' , '╕' , '╣' , '║' , '╗' , '╝' , '╜' , '╛' , '┐' ,
                '└' , '┴' , '┬' , '├' , '─' , '┼' , '╞' , '╟' , '╚' , '╔' , '╩' , '╦' , '╠' , '═' , '╬' , '╧' ,
                '╨' , '╤' , '╥' , '╙' , '╘' , '╒' , '╓' , '╫' , '╪' , '┘' , '┌' , '█' , '▄' , '▌' , '▐' , '▀' ,
                'α' , 'ß' , 'Γ' , 'π' , 'Σ' , 'σ' , 'µ' , 'τ' , 'Φ' , 'Θ' , 'Ω' , 'δ' , '∞' , 'φ' , 'ε' , '∩' ,
                '≡' , '±' , '≥' , '≤' , '⌠' , '⌡' , '÷' , '≈' , '°' , '∙' , '·' , '√' , 'ⁿ' , '²' , '■' , '\x00A0'
        };
        static readonly char[] CP866 = new char[]
        {
            'А', 'Б', 'В', 'Г', 'Д', 'Е', 'Ж', 'З', 'И', 'Й', 'К', 'Л', 'М', 'Н', 'О', 'П', 'Р', 'С', 'Т', 'У', 'Ф', 'Х', 'Ц', 'Ч', 'Ш', 'Щ', 'Ъ', 'Ы', 'Ь', 'Э', 'Ю', 'Я', 'а', 'б', 'в', 'г', 'д', 'е', 'ж', 'з', 'и', 'й', 'к', 'л', 'м', 'н', 'о', 'п', '░', '▒', '▓', '│', '┤', '╡', '╢', '╖', '╕', '╣', '║', '╗', '╝', '╜', '╛', '┐', '└', '┴', '┬', '├', '─', '┼', '╞', '╟', '╚', '╔', '╩', '╦', '╠', '═', '╬', '╧', '╨', '╤', '╥', '╙', '╘', '╒', '╓', '╫', '╪', '┘', '┌', '█', '▄', '▌', '▐', '▀', 'р', 'с', 'т', 'у', 'ф', 'х', 'ц', 'ч', 'ш', 'щ', 'ъ', 'ы', 'ь', 'э', 'ю', 'я', 'Ё', 'ё', 'Є', 'є', 'Ї', 'ї', 'Ў', 'ў', '°', '∙', '·', '√', '№', '¤', '■', '\xa0'
        };

        public static char[] GetCodePageTable(int codepage)
        {
            switch (codepage)
            {
                case 437:
                    return CP437;
                case 866:
                    return CP866;
            }

            return null;
        }
    }

    class CP437Encoding : SingleByteEncoding
    {
        internal CP437Encoding() { CodePageTable = CodePageTables.GetCodePageTable(437); }
        public override string BodyName => "IBM437";
        public override int CodePage => 437;
    }

    class CP866Encoding : SingleByteEncoding
    {

        internal CP866Encoding() { CodePageTable = CodePageTables.GetCodePageTable(866); }
        public override string BodyName => "IBM00866";
        public override int CodePage => 866;
    }

    public class XyBeeEncodingProvider : EncodingProvider
    {
        static readonly EncodingProvider singleton = new XyBeeEncodingProvider();
        internal XyBeeEncodingProvider() { }
        public static EncodingProvider Instance => singleton;

        public override Encoding GetEncoding(int codepage)
        {
            if (codepage is < 0 or > 65535)
            {
                return null;
            }

            switch (codepage)
            {
                case 437:
                    return new CP437Encoding();
                case 866:
                    return new CP866Encoding();
            }

            return null;
        }

        public override Encoding GetEncoding(string name)
        {
            return GetEncoding(437);
        }
    }
}