using System;
using System.Collections.Generic;
using System.Windows;

namespace LeapIN.Interface
{
    class KeyboardModule
    {
        byte[] keycodes;

        // struct to hold the definition of a key:
        // its string representation and the virtual keyboard code used to call the event
        public struct Key
        {
            public string k;
            public int code;

            public Key(char kn, int c)
            {
                k = kn.ToString();
                code = c;
            }

            public string Name
            {
                get { return k; }
            }
        }

        // struct specifically for small sets of keys like A, B, C
        public struct KeySet
        {
            public List<Key> keygroup;
            public Key selected;

            public KeySet(KeySet t)
            {
                keygroup = t.keygroup;
                selected = t.keygroup[0];
            }

            public Key Selected
            {
                get { return selected; }
            }
        }

        List<KeySet> keys;

        public KeyboardModule()
        {
            keys = new List<KeySet>();
            //create lower case
            CreateKeyboard();
        }

        public List<KeySet> SKeys
        {
            get { return keys; }
        }

        // Alter this to a double for loop which separates out the keys and adds them 3/4 at a time to new keysets
        void CreateKeyboard()
        {
            // Create the key code array
            keycodes = new byte[256];

            for (int i = 0; i < 256; i++)
            {
                keycodes[i] = (byte)(i + 1);
            }

            for (int i = 97; i < 123; i=i+3)
            {
                KeySet temp = new KeySet();
                temp.keygroup = new List<Key>();
                for (int j = 0; j < 3; j++)
                {
                    temp.keygroup.Add(new Key((char)(i+j), i + j - 56));
                }

                if (i >= 121)
                {
                    temp.keygroup[2] = new Key((char)46, 190);
                }

                keys.Add(new KeySet(temp));
            }
        }


        // Command binding leading to a function that adds to a property/variable displaying the current word, detect space, apply the current word to actual keypresses using win32
        // detect things like backspace too
        // 1 button switches through the letters, which updates the second button which adds the letters??
    }
}
