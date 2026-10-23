using Alexandria.ItemAPI;
using Scytodes.passive_items;
using Scytodes.guns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

/*
 * CODE TAKEN FROM THE FOLLOWING GITHUB LINK
 * https://github.com/Ricky2148/ETG-Mod-League-of-Legends-Items/blob/main/Synergies.cs
 * MASSIVE THANKS TO RICKY2148 IN THE "Mod The Gungeon" DISCORD SERVER 
 * ALSO MASSIVE THANKS TO REALMSXMYTHS & CAPTAIN PRETZEL IN THE SAME DISCORD SERVER
 * https://discord.gg/Ne9yh5Krsh
*/

namespace Scytodes
{
    public static class ScytodesSynergies
    {
        private static int _NUM_SYNERGIES = Enum.GetNames(typeof(Synergy)).Length;                                              // number of synergies our mod adds
        public static List<CustomSynergyType> _Synergies = Enumerable.Repeat<CustomSynergyType>(0, _NUM_SYNERGIES).ToList();    // list of the actual new synergies
        public static List<string> _SynergyNames = Enumerable.Repeat<string>(null, _NUM_SYNERGIES).ToList();                    // list of friendly names of synergies
        public static List<string> _SynergyEnums = new(Enum.GetNames(typeof(Synergy)));                                         // list of enum names of synergies
        public static List<int> _SynergyIds = Enumerable.Repeat<int>(0, _NUM_SYNERGIES).ToList();                               // list of ids of synergies in the AdvancedSynergyEntry database

        //NOTE: needs to be done early so guns and items can reference synergy ids properly
        public static void InitEnums()
        {
            // Extend the base game's CustomSynergyType enum to make room for our new synergy
            for (int i = 0; i < _SynergyNames.Count; ++i)
                _Synergies[i] = _SynergyEnums[i].ExtendEnum<CustomSynergyType>();
        }
        public static T ExtendEnum<T>(this string s) where T : System.Enum
        {
            return ETGModCompatibility.ExtendEnum<T>("Scytodes".ToUpper(), s);
        }

        public static void Init()
        {
            #region Synergies
            /*
             * When writing a new synergy, make sure to add the name of the synergy in the Synergy enum at the bottom!
             * For example, the normal name "Scyto's Bias" would be written as "SCYTOS_BIAS" in the enum. (idk how important this is but do it anyway)
             * For the itemtips file, refer to the nameKey section under the RegsiterSynergy() method.
            */

            //PASSIVE ITEMS ======================================================================================================================================================================================================================================


            //Rod of Ages [EXAMPLE]
            //NewSynergy(LOLItems.Synergy.AGE_OLD_WISDOM, "Age old wisdom", new[] { IName(RodOfAges.ItemName) }, new[] { "old_knights_shield", "old_knights_helm", "old_knights_flask" });

            //Scyto's Bias
            NewSynergy(Scytodes.Synergy.SCYTOS_BIAS, "Scyto's Bias", new[] { IName(TheItemInQuestion.ItemName), "blasphemy" }, null, true);

            //Name Filler
            //NewSynergy();

            //ACTIVE ITEMS =======================================================================================================================================================================================================================================


            //Refillable Potion [EXAMPLE]
            //NewSynergy(LOLItems.Synergy.COCKTAIL_POTION, "Cocktail Potion", new[] { IName(RefillablePotion.ItemName) }, new[] { "potion_of_lead_skin", "potion_of_gun_friendship" });

            //Name Filler
            //NewSynergy();

            //WEAPONS ============================================================================================================================================================================================================================================


            //Virtue [EXAMPLE]
            //NewSynergy(LOLItems.Synergy.EXP_SHARE_FORM_1, "EXP. Share", new[] { VirtueForm1.internalName }, new[] { "macho_brace", "scouter", "life_orb" });

            //Name Filler
            //NewSynergy();

            #endregion
        }

        //THIS IS IMPORTANT TO REFER TO WHEN MAKING NEW SYNERGIES!!!
        private static AdvancedSynergyEntry NewSynergy(Synergy synergy, string name, string[] mandatory, string[] optional = null, bool ignoreLichEyeBullets = false)
        {
            // Get the enum index of our synergy
            int index = (int)synergy;
            // Register the AdvancedSynergyEntry so that the game knows about it
            AdvancedSynergyEntry ase = RegisterSynergy(_Synergies[index], name, mandatory.ToList(), (optional != null) ? optional.ToList() : null, ignoreLichEyeBullets);
            // Index the friendly name of our synergy
            _SynergyNames[index] = name;
            // Get the actual ID of our synergy entry in the AdvancedSynergyDatabase, which doesn't necessarily match the CustomSynergyType enum
            _SynergyIds[index] = GameManager.Instance.SynergyManager.synergies.Length - 1;
            // Return the AdvancedSynergyEntry
            return ase;
        }

        public static AdvancedSynergyEntry RegisterSynergy(CustomSynergyType synergy, string name, List<string> mandatoryConsoleIDs, List<string> optionalConsoleIDs = null, bool ignoreLichEyeBullets = false)
        {
            List<int> itemIDs = new();
            List<int> gunIDs = new();
            List<int> optItemIDs = new();
            List<int> optGunIDs = new();
            foreach (var id in mandatoryConsoleIDs)
            {
                PickupObject pickup = Gungeon.Game.Items[id];
                if (pickup && pickup.GetComponent<Gun>())
                    gunIDs.Add(pickup.PickupObjectId);
                else if (pickup && (pickup.GetComponent<PlayerItem>() || pickup.GetComponent<PassiveItem>()))
                    itemIDs.Add(pickup.PickupObjectId);
            }

            if (optionalConsoleIDs != null)
            {
                foreach (var id in optionalConsoleIDs)
                {
                    PickupObject pickup = Gungeon.Game.Items[id];
                    if (pickup && pickup.GetComponent<Gun>())
                        optGunIDs.Add(pickup.PickupObjectId);
                    else if (pickup && (pickup.GetComponent<PlayerItem>() || pickup.GetComponent<PassiveItem>()))
                        optItemIDs.Add(pickup.PickupObjectId);
                }
            }

            /* 
             * Add our synergy's name to the string manager so it displays properly when activated.
             * Also the itemtips file takes from the nameKey!!! NOT the Synergy enum!!!
             * This means for the name "Scyto's Bias", it'd be turned into "#SCYTO'S_BIAS" WITH the apostrophe!!!
             * Reference other itemtips files if you don't know how to set it up. And have the item tips mod installed ofc.
            */ 
            string nameKey = $"#{name.ToID().ToUpperInvariant()}";
            ETGMod.Databases.Strings.Synergy.Set(nameKey, name);

            AdvancedSynergyEntry entry = new AdvancedSynergyEntry()
            {
                NameKey = nameKey,                            //Remember, your nameKey is what the item tips mod uses for synergies
                MandatoryItemIDs = itemIDs,
                MandatoryGunIDs = gunIDs,
                OptionalItemIDs = optItemIDs,
                OptionalGunIDs = optGunIDs,
                bonusSynergies = new() { synergy },           //Idk what this does but everything breaks without it
                statModifiers = new List<StatModifier>(),     //This too, dont get rid of it
                IgnoreLichEyeBullets = ignoreLichEyeBullets,  //Set this to true to ignore the gunslinger's passive!
            };

            int oldLength = GameManager.Instance.SynergyManager.synergies.Length;
            Array.Resize(ref GameManager.Instance.SynergyManager.synergies, oldLength + 1);
            GameManager.Instance.SynergyManager.synergies[oldLength] = entry;
            return entry;
        }

        private static string IName(string itemName) => "Scytodes:" + itemName.ToID();
        public static CustomSynergyType Synergy(this Synergy synergy) => _Synergies[(int)synergy];
        public static string SynergyName(this Synergy synergy) => _SynergyNames[(int)synergy];
        public static bool HasSynergy(this PlayerController player, Synergy synergy) => player.ActiveExtraSynergies.Contains((int)_SynergyIds[(int)synergy]);
    }

    //Put your new synergies into this enum without special characters, all capitalized, and turn spaces into underscores!
    //This is NOT used in your itemtips file, your nameKey is used!
    public enum Synergy
    {
        // Synergies
        SCYTOS_BIAS,
        //FILLER_SYNERGY_NAME,
    };
}