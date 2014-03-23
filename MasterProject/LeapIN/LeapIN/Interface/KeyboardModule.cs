﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

using LeapIN.Extras;
using System.Runtime.InteropServices;

namespace LeapIN.Interface
{
    class KeyboardModule : PropertyChange
    {
        byte[] keycodes;
        List<KeySet> currentkeys;
        List<KeySet> keys; // Regular set of keygroups
        List<KeySet> capskeys;
        List<KeySet> speckeys; // Number Keys

        string currentWord;
        List<Key> inputKeys;

        ICommand inputCommand; // On a char
        ICommand postCommand; // Space or enter
        ICommand deleteCommand; // Backspace
        ICommand nextKeyCommand;
        ICommand switchModeCommand;

        // struct to hold the definition of a key:
        // its string representation and the virtual keyboard code used to call the event
        public struct Key
        {
            public string k;
            public ushort code;

            public Key(char kn, int c)
            {
                k = kn.ToString();
                code = (ushort)c;
            }

            public string Name
            {
                get { return k; }
            }
        }

        // struct specifically for small sets of keys like A, B, C
        public class KeySet : PropertyChange
        {
            public List<Key> keygroup;
            public int size;
            public Key selected;

            public KeySet()
            {
                keygroup = new List<Key>();
                size = 0;
            }

            public KeySet(KeySet t)
            {
                keygroup = t.keygroup;
                size = t.size;
                //selected = t.keygroup[0];
            }

            public Key Selected
            {
                get { return selected; }
                set { selected = value; OnPropertyChanged("Selected"); }
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
            inputKeys = new List<Key>();

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

        public string CurrentWord
        {
            get { return currentWord; }
            set
            {
                if (currentWord != value)
                {
                    currentWord = value;
                    OnPropertyChanged("CurrentWord");
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

        public ICommand PostCommand
        {
            get
            {
                if (postCommand == null)
                {
                    postCommand = new RelayCommand(
                        param => SendWord(param)
                    );
                }
                return postCommand;
            }
        }

        public ICommand DeleteCommand
        {
            get
            {
                if (deleteCommand == null)
                {
                    deleteCommand = new RelayCommand(
                        param => BackSpace()
                    );
                }
                return deleteCommand;
            }
        }

        public ICommand NextKeyCommand
        {
            get
            {
                if (nextKeyCommand == null)
                {
                    nextKeyCommand = new RelayCommand(
                        param => SwitchSelected(param)
                    );
                }
                return nextKeyCommand;
            }
        }

        public ICommand SwitchModeCommand
        {
            get
            {
                if (switchModeCommand == null)
                {
                    switchModeCommand = new RelayCommand(
                        param => SwitchMode()
                    );
                }
                return switchModeCommand;
            }
        }

        void InitialiseKeyboard()
        {
            // Create the key code array
            keycodes = new byte[255];

            // A - Z
            for (int i = 0; i < 254; i++)
            {
                keycodes[i] = (byte)(i + 1);
            }

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

            for (int i = start; i < start+26; i = i + 3)
            {
                KeySet temp = new KeySet();

                for (int j = 0; j < 3; j++)
                {
                    if (count == 26)
                        break;

                    temp.keygroup.Add(new Key((char)(i + j), 65 + count));
                    temp.size += 1;
                    count++;
                }

                list.Add(new KeySet(temp));
            }

            CreateKeyGroup(new char[] { '.', ',', '\'' }, new int[] { 190, 188, 222 }, 3, ref list);
            CreateKeyGroup(new char[] { '!', '?' }, new int[] { 49, 191, 7 }, 2, ref list);
        }

        // Adds all non alphabet characters to a separate set of key groups
        void CreateSpecial()
        {
            CreateKeyGroup(new char[] { '1', '@', ':', ';' }, new int[] { 49, 222, 186, 186 }, 4, ref speckeys); //  1 @ : ;
            CreateKeyGroup(new char[] { '2', '"', '#' }, new int[] { 50, 50, 39 }, 3, ref speckeys);// 2 " #
            CreateKeyGroup(new char[] { '3', '£', '&' }, new int[] { 51, 39, 55 }, 3, ref speckeys);// 3 £ &
            CreateKeyGroup(new char[] { '4', '/', '\\' }, new int[] { 52, 191, 220 }, 3, ref speckeys);// 4 / \
            CreateKeyGroup(new char[] { '5', '%', '_' }, new int[] { 53, 53, 189 }, 3, ref speckeys);// 5 % _
            CreateKeyGroup(new char[] { '6', '^' }, new int[] { 54, 54 }, 2, ref speckeys);// 6 ^ 
            CreateKeyGroup(new char[] { '7', '-', '+' }, new int[] { 55, 189, 187 }, 3, ref speckeys);// 7 - +
            CreateKeyGroup(new char[] { '8', '*', '=' }, new int[] { 56, 56, 187 }, 3, ref speckeys);// 8 * =
            CreateKeyGroup(new char[] { '9', '(', ')', '{', '}', }, new int[] { 57, 57, 39, 219, 221, }, 5, ref speckeys);// 9 ( ) { }
            CreateKeyGroup(new char[] { '0', '[', ']', '<', '>' }, new int[] { 39, 219, 221, 188, 190 }, 5, ref speckeys);// 0 [ ] < >
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

        void SwitchSelected(object o)
        {
            KeySet t = (KeySet)o;

            // Find the current index of the selected
            int i = t.keygroup.IndexOf(t.selected);

            if (i == (t.size - 1))
            {
                i = 0;
            }
            else
            {
                i += 1;
            }

            t.Selected = t.keygroup[i];
        }

        void SwitchMode()
        {
            if (SKeys == keys)
                SKeys = capskeys;
            else if (SKeys == capskeys)
                SKeys = speckeys;
            else
                SKeys = keys;
        }

        void InputKey(object o)
        {
            Key pressed = (Key)o;
            CurrentWord += pressed.k;
            inputKeys.Add(pressed);
        }

        void SendWord(object n)
        {
            string type = (string)n;

            foreach (Key k in inputKeys)
            {
                SendKey(k.code);
            }

            switch (type)
            {
                case "Space":
                    SendKey((ushort)0x20);
                    break;
                case "Enter":
                    SendKey((ushort)0x0D);
                    break;
                default:
                    break;
            }

            inputKeys.Clear();
            CurrentWord = "";
        }

        void SendKey(ushort code)
        {
            Win32Services.INPUT structInput;

            structInput = new Win32Services.INPUT();
            structInput.type = (uint)1;
            structInput.ki.wScan = 0;
            structInput.ki.time = 0;
            structInput.ki.dwFlags = 0;
            structInput.ki.dwExtraInfo = 0;

            // set to the virtual key code
            structInput.ki.wVk = code;
            Win32Services.SendInput(1, ref structInput, Marshal.SizeOf(structInput));

            structInput.ki.dwFlags = Win32Services.KEYEVENTF_KEYUP;
            Win32Services.SendInput(1, ref structInput, Marshal.SizeOf(structInput));
        }

        void BackSpace()
        {
            if (CurrentWord != "" && CurrentWord != null)
            {
                inputKeys.RemoveAt(inputKeys.Count - 1);
                CurrentWord = CurrentWord.Remove(CurrentWord.Length - 1);
            }
            else
            {
                SendKey((ushort)0x08);
            }
        }
    }
}
