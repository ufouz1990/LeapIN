using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;

using LeapIN.Extras;


namespace LeapIN.Interface
{
    class KeyboardModule : PropertyChange
    {
        List<KeySet> currentkeys; // The currently displayed set
        List<KeySet> keys; // Regular set of keygroups
        List<KeySet> capskeys; // Upper case
        List<KeySet> speckeys; // Number Keys etc

        ICommand inputCommand; // On a char
        ICommand actionCommand; // Space/Enter/Backspace
        ICommand shiftCommand;

        // struct to hold the definition of a key:
        // its string representation and the virtual keyboard code used to call the event
        public struct Key
        {
            public string name;
            public ushort code;

            public Key(char kn, int c)
            {
                name = kn.ToString();
                code = (ushort)c;
            }

            public string Name
            {
                get { return name; }
            }
        }

        // struct specifically for small sets of keys like A, B, C
        public class KeySet
        {
            public List<Key> keygroup;
            public int size;

            public KeySet()
            {
                keygroup = new List<Key>();
                size = 0;
            }

            public KeySet(KeySet t)
            {
                keygroup = t.keygroup;
                size = t.size;
            }

            public List<Key> KeyGroup
            {
                get { return keygroup; }
            }
        }

        public KeyboardModule()
        {
            keys = new List<KeySet>();
            capskeys = new List<KeySet>();
            speckeys = new List<KeySet>();

            InitialiseKeyboard();

            currentkeys = keys;
        }

        public List<KeySet> SKeys
        {
            get { return currentkeys; }
            set
            {
                if (currentkeys != value)
                {
                    currentkeys = value;
                    OnPropertyChanged("SKeys");
                }
            }
        }

        public ICommand InputCommand
        {
            get
            {
                if (inputCommand == null)
                {
                    inputCommand = new RelayCommand(
                        param => InputKey(param)
                    );
                }
                return inputCommand;
            }
        }

        public ICommand ActionCommand
        {
            get
            {
                if (actionCommand == null)
                {
                    actionCommand = new RelayCommand(
                        param => ActionKey(param)
                    );
                }
                return actionCommand;
            }
        }

        /// <summary>
        /// Creates the three sets of keys for displayed the virtual keyboard
        /// </summary>
        void InitialiseKeyboard()
        {
            CreateAlphabet(97, ref keys);
            CreateAlphabet(65, ref capskeys);
            CreateSpecial();
        }

        /// <summary>
        /// Creates the A-Z keys, start value changes the displayed characters
        /// </summary>
        void CreateAlphabet(int start, ref List<KeySet> list)
        {
            int count = 0;
            int codeVal = 65;

            if (start == 65)
                codeVal += 256;

            for (int i = start; i < start+26; i = i + 3)
            {
                KeySet temp = new KeySet();

                for (int j = 0; j < 3; j++)
                {
                    if (count == 26)
                        break;

                    temp.keygroup.Add(new Key((char)(i + j), codeVal + count));
                    temp.size += 1;
                    count++;
                }

                list.Add(new KeySet(temp));
            }

            CreateKeyGroup(new char[] { '.', ',', '\'' }, new int[] { 190, 188, 192 }, 3, ref list);
            CreateKeyGroup(new char[] { '!', '?', '@' }, new int[] { 49 + 256, 191 + 256, 192 + 256 }, 3, ref list);
        }

        /// <summary>
        /// Adds all non alphabet characters to a separate set of key groups
        /// </summary>
        void CreateSpecial()
        {
            CreateKeyGroup(new char[] { '1', ';', ':' }, new int[] { 49, 186, (186+256) }, 3, ref speckeys); //  1 ; :
            CreateKeyGroup(new char[] { '2', '"', '#' }, new int[] { 50, (50+256), 222 }, 3, ref speckeys);// 2 " #
            CreateKeyGroup(new char[] { '3', '£', '&' }, new int[] { 51, (51+256), (55+256) }, 3, ref speckeys);// 3 £ &
            CreateKeyGroup(new char[] { '4', '/', '\\'}, new int[] { 52, 191, 220 }, 3, ref speckeys);// 4 / \
            CreateKeyGroup(new char[] { '5', '%', '_' }, new int[] { 53, (53+256), (189+256) }, 3, ref speckeys);// 5 % _
            CreateKeyGroup(new char[] { '6', '^', '-' }, new int[] { 54, (54+256), 189 }, 3, ref speckeys);// 6 ^ -
            CreateKeyGroup(new char[] { '7', '+', '=' }, new int[] { 55, (187+256), 187 }, 3, ref speckeys);// 7 + =
            CreateKeyGroup(new char[] { '8', '*', '(' }, new int[] { 56, (56+256), (57+256) }, 3, ref speckeys);// 8 * (
            CreateKeyGroup(new char[] { '9', ')', '[' }, new int[] { 57, (48+256), 219 }, 3, ref speckeys);// 9 ) [
            CreateKeyGroup(new char[] { '0', ']', '<' }, new int[] { 48, 221, (188+256) }, 3, ref speckeys);// 0 ] <
            CreateKeyGroup(new char[] { '{', '}', '>' }, new int[] { (219+256), (221+256), (190+256) }, 3, ref speckeys);// { } >
        }

        /// <summary>
        /// Create a set of keys and add it to an existing list of keysets / module
        /// </summary>
        void CreateKeyGroup(char[] items, int[] codes, int size, ref List<KeySet> set)
        {
            KeySet temp = new KeySet();
            temp.size = size;

            for (int i = 0; i < size; i++)
            {
                temp.keygroup.Add(new Key(items[i], codes[i]));
            }

            set.Add(new KeySet(temp));
        }

        /// <summary>
        /// Takes a key object and passes it along to be pressed
        /// </summary>
        void InputKey(object o)
        {
            Key pressed = (Key)o;
            SendKey(pressed.code);
        }

        /// <summary>
        /// Method to activate important keys such as space or enter
        /// </summary>
        void ActionKey(object n)
        {
            string type = (string)n;

            switch (type)
            {
                case "Space":
                    SendKey((ushort)0x20);
                    break;
                case "Enter":
                    SendKey((ushort)0x0D);
                    break;
                case "Backspace":
                    SendKey((ushort)0x08);
                    break;
                case "Shift":
                    ShiftKeys();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Shifts the keyboard into upper case or special
        /// </summary>
        void ShiftKeys()
        {
            if (SKeys == keys)
                SKeys = capskeys;
            else if (SKeys == capskeys)
                SKeys = speckeys;
            else
                SKeys = keys;
        }

        /// <summary>
        /// Emulates the key press event using the virtual key codes
        /// </summary>
        void SendKey(ushort code)
        {
            Win32Services.INPUT structInput;

            structInput = new Win32Services.INPUT();
            structInput.type = (uint)1;
            structInput.ki.wScan = 0;
            structInput.ki.time = 0;
            structInput.ki.dwFlags = 0;
            structInput.ki.dwExtraInfo = 0;

            if (code < 256)
            {
                // set to the virtual key code
                structInput.ki.wVk = code;
                Input(ref structInput);

                structInput.ki.dwFlags = Win32Services.KEYEVENTF_KEYUP;
                Input(ref structInput);
            }
            else // shift
            {
                code -= 256;
                structInput.ki.wVk = 16; // SHIFT CODE
                Input(ref structInput);

                structInput.ki.wVk = code;
                Input(ref structInput);
                structInput.ki.dwFlags = Win32Services.KEYEVENTF_KEYUP;

                Input(ref structInput);
                structInput.ki.wVk = 16;

                Input(ref structInput);
            }
        }

        void Input(ref Win32Services.INPUT sI)
        {
            Win32Services.SendInput(1, ref sI, Marshal.SizeOf(sI));
        }
    }
}
