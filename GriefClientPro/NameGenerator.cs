using System;
using System.Collections.Generic;
using System.Linq;

namespace GriefClientPro
{
    public static class NameGenerator
    {
        private static readonly string[] FantasyMale =
        {
            "Abardon", "Acaman", "Achard", "Ackmard", "Agon", "Agnar", "Abdun", "Aidan", "Airis", "Aldaren", "Alderman", "Alkirk", "Amerdan", "Anfarc", "Aslan", "Actar", "Atgur", "Atlin", "Aldan",
            "Badek", "Baduk", "Bedic", "Beeron", "Bein", "Bithon", "Bohl", "Boldel", "Bolrock", "Bredin", "Bredock", "Breen", "tristan", "Bydern", "Cainon", "Calden", "Camon", "Cardon", "Casdon",
            "Celthric", "Cevelt", "Chamon", "Chidak", "Cibrock", "Cipyar", "Colthan", "Connell", "Cordale", "Cos", "Cyton", "Daburn", "Dawood", "Dak", "Dakamon", "Darkboon", "Dark", "Darg", "Darmor",
            "Darpick", "Dask", "Deathmar", "Derik", "Dismer", "Dokohan", "Doran", "Dorn", "Dosman", "Draghone", "Drit", "Driz", "Drophar", "Durmark", "Dusaro", "Eckard", "Efar", "Egmardern", "Elvar",
            "Elmut", "Eli", "Elik", "Elson", "Elthin", "Elbane", "Eldor", "Elidin", "Eloon", "Enro", "Erik", "Erim", "Eritai", "Escariet", "Espardo", "Etar", "Eldar", "Elthen", "Etran", "Eythil",
            "Fearlock", "Fenrirr", "Fildon", "Firdorn", "Florian", "Folmer", "Fronar", "Fydar", "Gai", "Galin", "Galiron", "Gametris", "Gauthus", "Gehardt", "Gemedes", "Gefirr", "Gibolt", "Geth",
            "Gom", "Gosform", "Gothar", "Gothor", "Greste", "Grim", "Gryni", "Gundir", "Gustov", "Halmar", "Haston", "Hectar", "Hecton", "Helmon", "Hermedes", "Hezaq", "Hildar", "Idon", "Ieli",
            "Ipdorn", "Ibfist", "Iroldak", "Ixen", "Ixil", "Izic", "Jamik", "Jethol", "Jihb", "Jibar", "Jhin", "Julthor", "Justahl", "Kafar", "Kaldar", "Kelar", "Keran", "Kib", "Kilden", "Kilbas",
            "Kildar", "Kimdar", "Kilder", "Koldof", "Kylrad", "Lackus", "Lacspor", "Lahorn", "Laracal", "Ledal", "Leith", "Lalfar", "Lerin", "Letor", "Lidorn", "Lich", "Loban", "Lox", "Ludok", "Ladok",
            "Lupin", "Lurd", "Mardin", "Markard", "Merklin", "Mathar", "Meldin", "Merdon", "Meridan", "Mezo", "Migorn", "Milen", "Mitar", "Modric", "Modum", "Madon", "Mafur", "Mujardin", "Mylo",
            "Mythik", "Nalfar", "Nadorn", "Naphazw", "Neowald", "Nildale", "Nizel", "Nilex", "Niktohal", "Niro", "Nothar", "Nathon", "Nadale", "Nythil", "Ozhar", "Oceloth", "Odeir", "Ohmar", "Orin",
            "Oxpar", "Othelen", "Padan", "Palid", "Palpur", "Peitar", "Pendus", "Penduhl", "Pildoor", "Puthor", "Phar", "Phalloz", "Qidan", "Quid", "Qupar", "Randar", "Raydan", "Reaper", "Relboron",
            "Riandur", "Rikar", "Rismak", "Riss", "Ritic", "Ryodan", "Rysdan", "Rythen", "Rythorn", "Sabalz", "Sadaron", "Safize", "Samon", "Samot", "Secor", "Sedar", "Senic", "Santhil", "Sermak",
            "Seryth", "Seth", "Shane", "Shard", "Shardo", "Shillen", "Silco", "Sildo", "Silpal", "Sithik", "Soderman", "Sothale", "Staph", "Suktar", "zuth", "Sutlin", "Syr", "Syth", "Sythril",
            "Talberon", "Telpur", "Temil", "Tamilfist", "Tempist", "Teslanar", "Tespan", "Tesio", "Thiltran", "Tholan", "Tibers", "Tibolt", "Thol", "Tildor", "Tilthan", "Tobaz", "Todal", "Tothale",
            "Touck", "Tok", "Tuscan", "Tusdar", "Tyden", "Uerthe", "Uhmar", "Uhrd", "Updar", "Uther", "Vacon", "Valker", "Valyn", "Vectomon", "Veldar", "Velpar", "Vethelot", "Vildher", "Vigoth",
            "Vilan", "Vildar", "Vi", "Vinkol", "Virdo", "Voltain", "Wanar", "Wekmar", "Weshin", "Witfar", "Wrathran", "Waytel", "Wathmon", "Wider", "Wyeth", "Xandar", "Xavor", "Xenil", "Xelx",
            "Xithyl", "Yerpal", "Yesirn", "Ylzik", "Zak", "Zek", "Zerin", "Zestor", "Zidar", "Zigmal", "Zilex", "Zilz", "Zio", "Zotar", "Zutar", "Zytan", "alarm", "albatross", "anaconda", "antique",
            "artificial", "autopsy", "autumn", "avenue", "backpack", "balcony", "barbershop", "boomerang", "bulldozer", "butter", "canal", "cloud", "clown", "coffin", "comic", "compass", "cosmic",
            "crayon", "creek", "crossbow", "dagger", "dinosaur", "dog", "donut", "door", "doorstop", "electrical", "electron", "eyelid", "firecracker", "fish", "flag", "flannel", "flea", "frostbite",
            "gravel", "haystack", "helium", "kangaroo", "lantern", "leather", "limousine", "lobster", "locomotive", "logbook", "longitude", "metaphor", "microphone", "monkey", "moose", "morning",
            "mountain", "mustard", "neutron", "nitrogen", "notorious", "obscure", "ostrich", "oyster", "parachute", "peasant", "pineapple", "plastic", "postal", "pottery", "proton", "puppet",
            "railroad", "rhinestone", "roadrunner", "rubber", "scarecrow", "scoreboard", "scorpion", "shower", "skunk", "sound", "street", "subdivision", "summer", "sunshine", "tea", "temple", "test",
            "tire", "tombstone", "toothbrush", "torpedo", "toupee", "trendy", "trombone", "tuba", "tuna", "tungsten", "vegetable", "venom", "vulture", "waffle", "warehouse", "waterbird", "weather",
            "weeknight", "windshield", "winter", "wrench", "xylophone", "alpha", "arm", "beam", "beta", "bird", "breeze", "burst", "cat", "cobra", "crystal", "drill", "eagle", "emerald", "epsilon",
            "finger", "fist", "foot", "fox", "galaxy", "gamma", "hammer", "heart", "hook", "hurricane", "iron", "jazz", "jupiter", "knife", "lama", "laser", "lion", "mars", "mercury", "moon", "moose",
            "neptune", "omega", "panther", "planet", "pluto", "plutonium", "poseidon", "python", "ray", "sapphire", "scissors", "screwdriver", "serpent", "sledgehammer", "smoke", "snake", "space",
            "spider", "star", "steel", "storm", "sun", "swallow", "tiger", "uranium", "venus", "viper", "wrench", "yard", "zeus"
        };

        private const int Order = 1;
        private const int MinLength = 5;

        private static readonly Random Random = new Random(DateTime.Now.Millisecond);

        private static readonly Dictionary<string, List<char>> Chains = new Dictionary<string, List<char>>();
        private static readonly List<string> Samples = new List<string>();
        private static readonly List<string> Used = new List<string>();

        static NameGenerator()
        {
            foreach (var upper in FantasyMale.Select(name => name.Trim().ToUpper()).Where(upper => upper.Length >= Order + 1))
            {
                Samples.Add(upper);
            }

            foreach (var word in Samples)
            {
                for (var letter = 0; letter < word.Length - Order; letter++)
                {
                    var token = word.Substring(letter, Order);
                    List<char> entry;
                    if (Chains.ContainsKey(token))
                    {
                        entry = Chains[token];
                    }
                    else
                    {
                        entry = new List<char>();
                        Chains[token] = entry;
                    }
                    entry.Add(word[letter + Order]);
                }
            }
        }

        public static string NextName
        {
            get
            {
                string s;
                do
                {
                    var n = Random.Next(Samples.Count);
                    var nameLength = Samples[n].Length;
                    s = Samples[n].Substring(Random.Next(0, Samples[n].Length - Order), Order);
                    while (s.Length < nameLength)
                    {
                        var token = s.Substring(s.Length - Order, Order);
                        var c = GetLetter(token);
                        if (c != '?')
                        {
                            s += GetLetter(token);
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (s.Contains(" "))
                    {
                        var tokens = s.Split(' ');
                        s = "";
                        for (var t = 0; t < tokens.Length; t++)
                        {
                            if (tokens[t] == "")
                            {
                                continue;
                            }
                            if (tokens[t].Length == 1)
                            {
                                tokens[t] = tokens[t].ToUpper();
                            }
                            else
                            {
                                tokens[t] = tokens[t].Substring(0, 1) + tokens[t].Substring(1).ToLower();
                            }
                            if (s != "")
                            {
                                s += " ";
                            }
                            s += tokens[t];
                        }
                    }
                    else
                    {
                        s = s.Substring(0, 1) + s.Substring(1).ToLower();
                    }
                } while (Used.Contains(s) || s.Length < MinLength);
                Used.Add(s);
                return s;
            }
        }

        public static void Reset()
        {
            Used.Clear();
        }

        private static char GetLetter(string token)
        {
            if (!Chains.ContainsKey(token))
            {
                return '?';
            }
            var letters = Chains[token];
            var n = Random.Next(letters.Count);
            return letters[n];
        }
    }
}
