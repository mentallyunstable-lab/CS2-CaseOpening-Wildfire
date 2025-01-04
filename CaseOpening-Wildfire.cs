using System;
using System.Collections.Generic;
using CounterStrikeSharp;

namespace CounterStrikeSharp.Plugins
{
    public class CaseOpening : Plugin
    {
        private Random random = new Random();
        private int playerCredits = 0; // Track player's credits
        private List<string> inventory = new List<string>(); // Player's inventory
        private Dictionary<string, List<string>> cases = new Dictionary<string, List<string>>(); // Player's cases
        private Dictionary<string, string> caseKeys = new Dictionary<string, string>(); // Keys for each case

        // Define cases and their corresponding items with probabilities and conditions
        private Dictionary<string, List<(string item, int probability)>> casesDict = new Dictionary<string, List<(string item, int probability)>>
        {
            { "Fracture Case", new List<(string, int)>
                { 
                    ("Negev Ultralight", 70), 
                    ("P250 Cassette", 70), 
                    ("SSG 08 Mainframe 001", 70), 
                    ("SG 553 Ol' Rusty", 70), 
                    ("P2000 Gnarled", 70), 
                    ("P90 Freight", 70), 
                    ("PP-Bizon Runic", 70), 
                    ("MAG-7 Monster Call", 20), 
                    ("Tec-9 Brother", 20), 
                    ("MAC-10 Allure", 20), 
                    ("Galil AR Connexion", 20), 
                    ("Glock-18 Vogue", 20), 
                    ("XM1014 Entombed", 20), 
                    ("Desert Eagle Printerstream", 9), 
                    ("AK-47 Legion of Anubis", 9),
                    ("★ Paracord Knife", 1)
                }
            }   
            { "Gallery Case", new List<(string, int)> 
                { 
                    ("MP9 | Featherweight", 70), 
                    ("P250 | Digital Architect", 70), 
                    ("Nova | Rust Coat", 70), 
                    ("UMP-45 | Oscillator", 20), 
                    ("Glock-18 | Synth Leaf", 20), 
                    ("M4A1-S | Printstream", 9), 
                    ("AK-47 | Neon Revolution", 9), 
                    ("AWP | Chromatic Aberration", 1), 
                    ("Desert Eagle | Code Red", 1), 
                    ("★ Karambit", 1) 
                } 
            },
            { "Kilowatt Case", new List<(string, int)> 
                { 
                    ("MP7 | Power Surge", 70), 
                    ("P90 | Electric Hive", 70), 
                    ("FAMAS | Pulse", 20), 
                    ("Tec-9 | Red Quartz", 20), 
                    ("M4A4 | Asiimov", 9), 
                    ("AWP | Lightning Strike", 9), 
                    ("AK-47 | Vulcan", 1), 
                    ("Desert Eagle | Blaze", 1), 
                    ("★ Butterfly Knife", 1) 
                } 
            },
            { "Revolution Case", new List<(string, int)>
                { 
                    ("MAC-10 | Monkeyflage", 70),
                    ("SCAR-20 | Fragments", 70),
                    ("MP5-SD | Liquidation", 70), 
                    ("USP-S | Ticket to Hell", 70), 
                    ("M4A1-S | Emphorosaur-S", 20), 
                    ("AK-47 | Head Shot", 20), 
                    ("P2000 | Wicked Sick", 9), 
                    ("AWP | Duality", 9), 
                    ("★ M9 Bayonet", 1)
                 }, 
            },
            { "Recoil Case", new List<(string, int)> 
                { 
                    ("R8 Revolver | Crazy 8", 70), 
                    ("P250 | Visions", 70), 
                    ("SG 553 | Dragon Tech", 70), 
                    ("Glock-18 | Winterized", 20), 
                    ("M4A4 | Poly Mag", 20), 
                    ("AWP | Chromatic Aberration", 9), 
                    ("AK-47 | Ice Coaled", 9), 
                    ("USP-S | Printstream", 1), 
                    ("★ Talon Knife", 1) 
                } 
            },
            { "Dreams & Nightmares Case", new List<(string, int)>
            { 
                ("MP9 | Starlight Protector", 70), 
                ("PP-Bizon | Space Cat", 70), 
                ("FAMAS | Rapid Eye Movement", 20), 
                ("Dual Berettas | Melondrama", 20), 
                ("M4A1-S | Night Terror", 9), 
                ("AK-47 | Nightwish", 9), 
                ("USP-S | Ticket to Hell", 1), 
                ("AWP | Fade", 1), 
                ("★ Skeleton Knife", 1) 
            } 
            },
            { "Snakebite Case", new List<(string, int)> 
            { 
                ("MP9 | Food Chain", 70), 
                ("P250 | Cyber Shell", 70), 
                ("XM1014 | Watchdog", 20), 
                ("MAC-10 | Button Masher", 20), 
                ("AK-47 | Slate", 9), 
                ("Desert Eagle | Trigger Discipline", 9), 
                ("USP-S | The Traitor", 1), 
                ("AWP | Chromatic Aberration", 1), 
                ("★ Survival Knife", 1) 
            } 
            },
            { "Broken Fang Case", new List<(string, int)> 
            { 
                ("Dual Berettas | Dezastre", 70), 
                ("P90 | Cocoa Rampage", 70), 
                ("MP5-SD | Condition Zero", 20), 
                ("AUG | Arctic Wolf", 20), 
                ("AWP | Exoskeleton", 9), 
                ("M4A1-S | Printstream", 9), 
                ("AK-47 | Panthera Onca", 1), 
                ("USP-S | Monster Mashup", 1), 
                ("★ Skeleton Knife", 1) 
            } 
            },
            { "Prisma 2 Case", new List<(string, int)> 
            { 
                ("MP5-SD | Desert Strike", 70), 
                ("SG 553 | Darkwing", 70), 
                ("Tec-9 | Decimator", 20), 
                ("AUG | Momentum", 20), 
                ("M4A1-S | Player Two", 9), 
                ("AK-47 | Phantom Disruptor", 9), 
                ("AWP | Capillary", 1), 
                ("Desert Eagle | Blue Ply", 1), 
                ("★ Talon Knife", 1) 
            } 
            },
            { "Prisma Case", new List<(string, int)> 
            { 
                ("P250 | Verdigris", 70), 
                ("MP7 | Mischief", 70), 
                ("MAC-10 | Whitefish", 20), 
                ("Galil AR | Akoben", 20), 
                ("AUG | Momentum", 9), 
                ("M4A4 | The Emperor", 9), 
                ("Desert Eagle | Light Rail", 1), 
                ("AK-47 | Neo-Noir", 1), 
                ("★ Ursus Knife", 1) 
            } 
            },
            { "Clutch Case", new List<(string, int)> 
            { 
                ("MP9 | Black Sand", 70), 
                ("SG 553 | Aloha", 70), 
                ("Nova | Wild Six", 20), 
                ("Five-SeveN | Flame Test", 20), 
                ("AWP | Mortis", 9), 
                ("M4A4 | Buzz Kill", 9), 
                ("USP-S | Cortex", 1), 
                ("AK-47 | Redline", 1), 
                ("★ Specialist Gloves", 1) 
            } 
            },
            { "Spectrum 2 Case", new List<(string, int)> 
            { 
                ("R8 Revolver | Survivalist", 70), 
                ("MP9 | Goo", 70), 
                ("XM1014 | Ziggy", 20), 
                ("Glock-18 | Off World", 20), 
                ("M4A1-S | Decimator", 9), 
                ("AK-47 | Bloodsport", 9), 
                ("AWP | Fever Dream", 1), 
                ("Desert Eagle | Code Red", 1), 
                ("★ Butterfly Knife", 1) 
            } 
            },
            { "Hydra Case", new List<(string, int)>
            { 
                ("MAG-7 | Hard Water", 70), 
                ("MAC-10 | Aloha", 70), 
                ("Tec-9 | Cut Out", 20), 
                ("UMP-45 | Metal Flowers", 20), 
                ("M4A1-S | Briefing", 9), 
                ("P250 | Ripple", 9), 
                ("AWP | Oni Taiji", 1), 
                ("Desert Eagle | Emerald Jörmungandr", 1), 
                ("★ Karambit", 1) 
            } 
            },
            { "Spectrum Case", new List<(string, int)> 
            { 
                ("PP-Bizon | Jungle Slipstream", 70), 
                ("Galil AR | Crimson Tsunami", 70), 
                ("MP7 | Akoben", 20), 
                ("Tec-9 | Ice Cap", 20), 
                ("M4A1-S | Decimator", 9), 
                ("AK-47 | Bloodsport", 9), 
                ("AWP | Fever Dream", 1), 
                ("USP-S | Neo-Noir", 1), 
                ("★ Butterfly Knifer", 1) 
            } 
            },
            { "Glove Case", new List<(string, int)> 
            { 
                ("CZ75-Auto | Polymer", 70), 
                ("MP9 | Sand Scale", 70), 
                ("USP-S | Cyrex", 20), 
                ("G3SG1 | Stinger", 20), 
                ("M4A1-S | Flashback", 9), 
                ("P90 | Shallow Grave", 9), 
                ("AWP | Phobos", 1), 
                ("AK-47 | Point Disarray", 1), 
                ("★ Specialist Gloves", 1) 
            } 
            },
            { "Gamma 2 Case", new List<(string, int)> 
            { 
                ("P250 | Iron Clad", 70), 
                ("SG 553 | Triarch", 70), 
                ("MP9 | Airlock", 20), 
                ("Glock-18 | Weasel", 20), 
                ("AWP | Phobos", 9), 
                ("M4A1-S | Mecha Industries", 9), 
                ("AK-47 | Neon Revolution", 1), 
                ("Desert Eagle | Directive", 1), 
                ("★ Falchion Knife", 1) 
            } 
            },
            { "Gamma Case", new List<(string, int)> 
            { 
                ("P250 | Iron Clad", 70), 
                ("Nova | Exo", 70), 
                ("UMP-45 | Briefing", 20), 
                ("Tec-9 | Fuel Injector", 20), 
                ("M4A1-S | Mecha Industries", 9), 
                ("AWP | Phobos", 9), 
                ("AK-47 | Neon Revolution", 1), 
                ("Glock-18 | Wasteland Rebel", 1), 
                ("★ Huntsman Knife", 1) 
            } 
            },
            { "Chroma 3 Case", new List<(string, int)> 
            { 
                ("P250 | Asiimov", 70), 
                ("PP-Bizon | Judgement of Anubis", 70), 
                ("MP7 | Nemesis", 20), 
                ("Tec-9 | Re-Entry", 20), 
                ("AK-47 | Point Disarray", 9), 
                ("M4A1-S | Chantico’s Fire", 9), 
                ("Desert Eagle | Crimson Web", 1), 
                ("★ Shadow Daggers", 1) 
            } 
            },
            { "Operation Wildfire Case", new List<(string, int)> 
            { 
                ("Nova | Hyper Beast", 70), 
                ("UMP-45 | Blaze", 70), 
                ("Glock-18 | Royal Legion", 20), 
                ("FAMAS | Valence", 20), 
                ("AWP | Elite Build", 9), 
                ("M4A1-S | Flashback", 9), 
                ("AK-47 | Fuel Injector", 1), 
                ("Desert Eagle | Kumicho Dragon", 1), 
                ("★ Bowie Knife", 1) 
            } 
            },
            { "Revolver Case", new List<(string, int)> 
            { 
                ("R8 Revolver | Amber Fade", 70), 
                ("PP-Bizon | Fuel Rod", 70), 
                ("Glock-18 | Grinder", 20), 
                ("Tec-9 | Avalanche", 20), 
                ("AWP | Crimson Web", 9), 
                ("M4A4 | Royal Paladin", 9), 
                ("AK-47 | Point Disarray", 1), 
                ("USP-S | Kill Confirmed", 1), 
                ("★ Karambit", 1) 
            } 
            },
            { "Shadow Case", new List<(string, int)> 
            { 
                ("P90 | Elite Build", 70), 
                ("PP-Bizon | Fuel Rod", 70), 
                ("Five-SeveN | Triumvirate", 20), 
                ("Nova | Ranger", 20), 
                ("M4A1-S | Golden Coil", 9), 
                ("AWP | Man-o'-War", 9), 
                ("AK-47 | Aquamarine Revenge", 1), 
                ("Desert Eagle | Conspiracy", 1), 
                ("★ Shadow Daggers", 1) 
            } 
            },
        };

        private List<string> conditions = new List<string> { "Factory New", "Minimal Wear", "Field-Tested", "Well-Worn", "Battle-Scarred" };

        // Knife variations with skins
        private Dictionary<string, List<string>> knifeVariations = new Dictionary<string, List<string>>
        {
            { "Paracord Knife", new List<string>
                {
                    "Forest DDPAT", "Safari Mesh", "Boreal Forest", "Urban Masked", "Scorched", 
                    "Case Hardened", "Crimson Web", "Fade", "Blue Steel", "Stained", 
                    "Night Stripe", "Slaughter", "Tiger Tooth", "Marble Fade", "Damascus Steel", 
                    "Rust Coat", "Doppler (Ruby, Sapphire, Black Pearl, Phase 1–4)", 
                    "Gamma Doppler (Emerald, Phase 1–4)", "Autotronic"
                }
            },
            { "Karambit", new List<string>
                {
                    "Forest DDPAT", "Safari Mesh", "Boreal Forest", "Urban Masked", "Scorched", 
                    "Case Hardened", "Crimson Web", "Fade", "Blue Steel", "Stained", 
                    "Night", "Slaughter", "Tiger Tooth", "Marble Fade", "Damascus Steel", 
                    "Rust Coat", "Doppler (Ruby, Sapphire, Black Pearl, Phase 1–4)", 
                    "Gamma Doppler (Emerald, Phase 1–4)", "Autotronic", "Lore", 
                    "Freehand", "Black Laminate", "Bright Water"
                }
            },
            { "Butterfly Knife", new List<string>
                {
                    "Forest DDPAT", "Safari Mesh", "Boreal Forest", "Urban Masked", "Scorched", 
                    "Case Hardened", "Crimson Web", "Fade", "Blue Steel", "Stained", 
                    "Night", "Slaughter", "Tiger Tooth", "Marble Fade", "Damascus Steel", 
                    "Rust Coat", "Doppler (Ruby, Sapphire, Black Pearl, Phase 1–4)", 
                    "Gamma Doppler (Emerald, Phase 1–4)", "Autotronic", "Lore", 
                    "Freehand", "Black Laminate", "Bright Water"
                }
            },
            { "M9 Bayonet", new List<string>
                {
                    "Forest DDPAT", "Safari Mesh", "Boreal Forest", "Urban Masked", "Scorched", 
                    "Case Hardened", "Crimson Web", "Fade", "Blue Steel", "Stained", 
                    "Night", "Slaughter", "Tiger Tooth", "Marble Fade", "Damascus Steel", 
                    "Rust Coat", "Doppler (Ruby, Sapphire, Black Pearl, Phase 1–4)", 
                    "Gamma Doppler (Emerald, Phase 1–4)", "Autotronic", "Lore", 
                    "Freehand", "Black Laminate", "Bright Water"
                }
            },
            { "Talon Knife", new List<string>
                {
                    "Forest DDPAT", "Safari Mesh", "Boreal Forest", "Urban Masked", "Scorched", 
                    "Case Hardened", "Crimson Web", "Fade", "Blue Steel", "Stained", 
                    "Night Stripe", "Slaughter", "Tiger Tooth", "Marble Fade", "Damascus Steel", 
                    "Rust Coat", "Doppler (Ruby, Sapphire, Black Pearl, Phase 1–4)", 
                    "Gamma Doppler (Emerald, Phase 1–4)", "Autotronic", "Lore"
                }
            },
            { "Skeleton Knife", new List<string>
                {
                    "Forest DDPAT", "Safari Mesh", "Boreal Forest", "Urban Masked", "Scorched", 
                    "Case Hardened", "Crimson Web", "Fade", "Blue Steel", "Stained", 
                    "Night Stripe", "Slaughter", "Tiger Tooth", "Marble Fade", "Damascus Steel", 
                    "Rust Coat", "Doppler (Ruby, Sapphire, Black Pearl, Phase 1–4)", 
                    "Gamma Doppler (Emerald, Phase 1–4)", "Autotronic"
                }
            },
            { "Survival Knife", new List<string>
                {
                    "Forest DDPAT", "Safari Mesh", "Boreal Forest", "Urban Masked", "Scorched", 
                    "Case Hardened", "Crimson Web", "Fade", "Blue Steel", "Stained", 
                    "Night Stripe", "Slaughter", "Tiger Tooth", "Marble Fade", "Damascus Steel", 
                    "Rust Coat", "Doppler (Ruby, Sapphire, Black Pearl, Phase 1–4)", 
                    "Gamma Doppler (Emerald, Phase 1–4)", "Autotronic"
                }
            },
            { "Ursus Knife", new List<string>
                {
                    "Forest DDPAT", "Safari Mesh", "Boreal Forest", "Urban Masked", "Scorched", 
                    "Case Hardened", "Crimson Web", "Fade", "Blue Steel", "Stained", 
                    "Night Stripe", "Slaughter", "Tiger Tooth", "Marble Fade", "Damascus Steel", 
                    "Rust Coat", "Doppler (Ruby, Sapphire, Black Pearl, Phase 1–4)", 
                    "Gamma Doppler (Emerald, Phase 1–4)", "Autotronic"
                }
            },
            { "Falchion Knife", new List<string>
                {
                    "Forest DDPAT", "Safari Mesh", "Boreal Forest", "Urban Masked", "Scorched", 
                    "Case Hardened", "Crimson Web", "Fade", "Blue Steel", "Stained", 
                    "Night", "Slaughter", "Tiger Tooth", "Marble Fade", "Damascus Steel", 
                    "Rust Coat", "Doppler (Ruby, Sapphire, Black Pearl, Phase 1–4)"
                }
            },
            { "Huntsman Knife", new List<string>
                {
                    "Forest DDPAT", "Safari Mesh", "Boreal Forest", "Urban Masked", "Scorched", 
                    "Case Hardened", "Crimson Web", "Fade", "Blue Steel", "Stained", 
                    "Night", "Slaughter", "Tiger Tooth", "Marble Fade", "Damascus Steel", 
                    "Rust Coat", "Doppler (Ruby, Sapphire, Black Pearl, Phase 1–4)",
                    "Autotronic"
                }
            },
            { "Shadow Daggers", new List<string>
                {
                    "Forest DDPAT", "Safari Mesh", "Boreal Forest", "Urban Masked", "Scorched", 
                    "Case Hardened", "Crimson Web", "Fade", "Blue Steel", "Stained", 
                    "Night", "Slaughter", "Tiger Tooth", "Marble Fade", "Damascus Steel", 
                    "Rust Coat", "Doppler (Ruby, Sapphire, Black Pearl, Phase 1–4)",
                    "Gamma Doppler (Emerald, Phase 1–4)"
                }
            },
            { "Bowie Knife", new List<string>
                {
                    "Forest DDPAT", "Safari Mesh", "Boreal Forest", "Urban Masked", "Scorched", 
                    "Case Hardened", "Crimson Web", "Fade", "Blue Steel", "Stained", 
                    "Night", "Slaughter", "Tiger Tooth", "Marble Fade", "Damascus Steel", 
                    "Rust Coat", "Doppler (Ruby, Sapphire, Black Pearl, Phase 1–4)"
                }
            },
        };

        public CaseOpening()
        {
            // Initialize case keys
            caseKeys.Add("Fracture Case", "Fracture Key");
            caseKeys.Add("Gallery Case", "Gallery Key");
            caseKeys.Add("Kilowatt Case", "Kilowatt Key");
            caseKeys.Add("Revolution Case", "Revolution Key");
            caseKeys.Add("Recoil Case", "Recoil Key");
            caseKeys.Add("Dreams & Nightmares Case", "Dreams Key");
            caseKeys.Add("Snakebite Case", "Snakebite Key");
            caseKeys.Add("Broken Fang Case", "Broken Fang Key");
            caseKeys.Add("Prisma 2 Case", "Prisma 2 Key");
            caseKeys.Add("Prisma Case", "Prisma Key");
            caseKeys.Add("Clutch Case", "Clutch Key");
            caseKeys.Add("Spectrum 2 Case", "Spectrum 2 Key");
            caseKeys.Add("Hydra Case", "Hydra Key");
            caseKeys.Add("Spectrum Case", "Spectrum Key");
            caseKeys.Add("Glove Case", "Glove Key");
            caseKeys.Add("Gamma 2 Case", "Gamma 2 Key");
            caseKeys.Add("Gamma Case", "Gamma Key");
            caseKeys.Add("Chroma 3 Case", "Chroma 3 Key");
            caseKeys.Add("Operation Wildfire Case", "Operation Wildfire Key");
            caseKeys.Add("Revolver Case", "Revolver Key");
            caseKeys.Add("Shadow Case", "Shadow Key");
        }

        public string OpenCase(string player, string caseName)
        {
            const int caseCost = 20; // Cost to open a case

            // Check if the player has enough credits to open the case
            if (playerCredits < caseCost)
            {
                return "You don't have enough credits to open this case.";
            }

            // Deduct the cost from the player's credits
            playerCredits -= caseCost;

            // Determine the item received from the case
            if (casesDict.ContainsKey(caseName))
            {
                List<(string item, int probability)> items = casesDict[caseName];
                var (itemReceived, _) = GetRandomItem(items);
                
                // Check if the item received is a knife and select a random variation if it is
                if (itemReceived.StartsWith("★"))
                {
                    string knifeType = itemReceived.Split('|')[0].Trim();
                    if (knifeVariations.ContainsKey(knifeType))
                    {
                        List<string> variations = knifeVariations[knifeType];
                        if (variations.Count == 1)
                        {
                            itemReceived = $"{knifeType} | {variations[0]}"; // Directly assign the only variation
                        }
                        else
                        {
                            itemReceived = $"{knifeType} | {variations[random.Next(variations.Count)]}"; // Randomly select from variations
                        }
                    }
                }

                string condition = GetRandomCondition(); // Randomize condition for each item
                AddToInventory(itemReceived); // Add item to inventory
                return $"Congratulations, {player}! You opened {caseName} and received: {itemReceived} ({condition})";
            }
            else
            {
                return "Invalid case name.";
            }
        }

        public void EarnCredits(string player, int dailyLogin = 0, int kills = 0, int deaths = 0, bool headshot = false, bool penetration = false, bool noScope = false, bool knifeKill = false)
        {
            playerCredits += dailyLogin * 5; // Daily login reward
            playerCredits += kills * 2; // 2 credits per kill
            playerCredits -= deaths; // 1 credit per death (deducted)
            if (headshot) playerCredits += 1; // +1 credit for headshot
            if (penetration) playerCredits += 1; // +1 credit for penetration
            if (noScope) playerCredits += 1; // +1 credit for no-scope kill
            if (knifeKill) playerCredits += 10; // +10 for knife kill
        }

        public string TradeItems(string playerFrom, string playerTo, List<string> itemsToTrade, List<string> itemsToReceive)
        {
            // Check if playerFrom has all items to trade
            foreach (var item in itemsToTrade)
            {
                if (!inventory.Contains(item))
                {
                    return $"{playerFrom} does not have {item} to trade.";
                }
            }

            // Check if playerTo has all items to receive
            foreach (var item in itemsToReceive)
            {
                if (!cases.ContainsKey(playerTo) || !cases[playerTo].Contains(item))
                {
                    return $"{playerTo} does not have {item} to trade.";
                }
            }

            // Remove traded items from playerFrom's inventory
            foreach (var item in itemsToTrade)
            {
                inventory.Remove(item);
            }

            // Add received items to playerFrom's inventory
            foreach (var item in itemsToReceive)
            {
                inventory.Add(item);
            }

            return $"{playerFrom} successfully traded {string.Join(", ", itemsToTrade)} for {string.Join(", ", itemsToReceive)} with {playerTo}.";
        }

        private void AddToInventory(string item)
        {
            inventory.Add(item); // Add the received item to the inventory
        }

        public List<string> GetInventory()
        {
            return inventory; // Return the current inventory
        }

        public void RewardRandomCaseOrKey(string player)
        {
            // Randomly reward a case or key at the end of every match
            if (random.Next(2) == 0) // 50% chance to give a case
            {
                string randomCase = GetRandomCase();
                cases.Add(randomCase);
            }
            else // 50% chance to give a key
            {
                string caseName = GetRandomCase();
                if (caseKeys.ContainsKey(caseName))
                {
                    keys.Add(caseKeys[caseName]); // Add the specific key for the case
                }
            }
        }

        private string GetRandomCase()
        {
            var caseNames = new List<string>(casesDict.Keys);
            return caseNames[random.Next(caseNames.Count)];
        }

        public List<string> GetCases()
        {
            return cases; // Return the player's cases
        }

        public List<string> GetKeys()
        {
            return keys; // Return the player's keys
        }

        private (string item, int probability) GetRandomItem(List<(string item, int probability)> items)
        {
            int roll = random.Next(1, 101); // Roll a number between 1 and 100
            int cumulativeProbability = 0;

            foreach (var (item, probability) in items)
            {
                cumulativeProbability += probability;
                if (roll <= cumulativeProbability)
                {
                    return (item, probability);
                }
            }

            return ("No item", 0); // Fallback (should not happen)
        }

        private string GetRandomCondition()
        {
            return conditions[random.Next(conditions.Count)];
        }

        private int GetPlayerBalance(string player)
        {
            // Return current credits
            return playerCredits; 
        }

        private void UpdatePlayerBalance(string player, int amount)
        {
            // Update credits
            playerCredits += amount; 
        }
    }
}
