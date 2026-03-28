using System;
using System.Collections.Generic;
using System.Text;

namespace WinFormsApp1 {
    internal class MorsecodeLookup {
        // morsecode lookup table (char -> morse)
        private static readonly Dictionary<char, string> _encodeMap = new()
        {
            { 'A', ".-" },    { 'B', "-..." },  { 'C', "-.-." },  { 'D', "-.." },
            { 'E', "." },     { 'F', "..-." },  { 'G', "--." },   { 'H', "...." },
            { 'I', ".." },    { 'J', ".---" },  { 'K', "-.-" },   { 'L', ".-.." },
            { 'M', "--" },    { 'N', "-." },    { 'O', "---" },   { 'P', ".--." },
            { 'Q', "--.-" },  { 'R', ".-." },   { 'S', "..." },   { 'T', "-" },
            { 'U', "..-" },   { 'V', "...-" },  { 'W', ".--" },   { 'X', "-..-" },
            { 'Y', "-.--" },  { 'Z', "--.." },

            { '0', "-----" }, { '1', ".----" }, { '2', "..---" },
            { '3', "...--" }, { '4', "....-" }, { '5', "....." },
            { '6', "-...." }, { '7', "--..." }, { '8', "---.." },
            { '9', "----." },

            { ' ', "_" }
        };


        // reverse dictionary ( morse -> char )
        private static readonly Dictionary<string, char> _decodeMap =
            _encodeMap.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

        // encode text → morse
        public static string Encode(string text) {
            var result = new List<string>();

            foreach (char c in text.ToUpper()) {
                if (_encodeMap.TryGetValue(c, out var morse)) {
                    result.Add(morse);
                } else {
                    result.Add("#");
                }
            }

            return string.Join(" ", result);
        }

        // decode morse → text
        public static string Decode(string morse) {
            var result = new List<char>();
            var parts = morse.Split(' ');

            foreach (var part in parts) {
                if (_decodeMap.TryGetValue(part, out var character)) {
                    result.Add(character);
                } else {
                    result.Add('#');
                }
            }

            return new string(result.ToArray());
        }
    }
}
