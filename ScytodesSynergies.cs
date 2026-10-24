using Alexandria.ItemAPI;
using Scytodes.passive_items;
using Scytodes.guns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

/* CODE TAKEN FROM THE FOLLOWING GITHUB LINK
 * https://github.com/Ricky2148/ETG-Mod-League-of-Legends-Items/blob/main/Synergies.cs
 * MASSIVE THANKS TO RICKY2148 IN THE "Mod The Gungeon" DISCORD SERVER 
 * ALSO MASSIVE THANKS TO REALMSXMYTHS & CAPTAIN PRETZEL IN THE SAME DISCORD SERVER
 * https://discord.gg/Ne9yh5Krsh */

namespace Scytodes
{
    public static class ScytodesSynergies
    {
        /* Number of synergies our mod adds. */
        private static int _NUM_SYNERGIES = Enum.GetNames(typeof(Synergy)).Length;
        /* List of the actual synergies. */
        public static List<CustomSynergyType> _Synergies = Enumerable.Repeat<CustomSynergyType>(0, _NUM_SYNERGIES).ToList();
        /* List of friendly names of synergies. */
        public static List<string> _SynergyNames = Enumerable.Repeat<string>(null, _NUM_SYNERGIES).ToList();
        /* List of enum names of synergies. */
        public static List<string> _SynergyEnums = new(Enum.GetNames(typeof(Synergy)));
        /* List of ids of synergies in the AdvancedSynergyEntry database. */
        public static List<int> _SynergyIds = Enumerable.Repeat<int>(0, _NUM_SYNERGIES).ToList();

        /* Note: this method needs to be done early so guns and items can reference synergy ids properly. */
        public static void InitEnums()
        {
            /* Extend the base game's CustomSynergyType enum to make room for our new synergy. */
            for (int i = 0; i < _SynergyNames.Count; ++i)
                _Synergies[i] = _SynergyEnums[i].ExtendEnum<CustomSynergyType>();
        }
        public static T ExtendEnum<T>(this string s) where T : System.Enum
        {
            /* The first parameter in this line should be the same as your "prefix" when spawning your item(s) in-game through the f2 console.
             * Note: Your prefix should be the same all across your project! Everywhere you need to input a prefix, use the same exact one.
             * This class should be set up where you do not need to touch this line. However, if you did not define your prefix in your "Plugin" class, go do that! Otherwise you will have to modify this line. */
            return ETGModCompatibility.ExtendEnum<T>($"{Plugin.ModPrefix.ToUpper()}", s);
        }

        /* Call this method from the Start() method of your ETGModule extension (usually called Plugin.cs).
         * If you have the GMStart() method inside your Start() method, call this method from there instead.
         * Note: you should call this method right AFTER your item and gun Init() methods! */
        public static void Init()
        {
            #region Synergies
            /* When writing a new synergy, make sure to add the name of the synergy in the Synergy enum at the very bottom of this class!
             * For example, the synergy name "Scyto's Bias" would be written as "SCYTOS_BIAS" in the Synergy enum (idk how important it is to write it like this but do it anyway).
             * Refer to the NewSynergy() method below if you are unsure how to make a new synergy.
             * For the itemtips file, refer to the nameKey section under the RegsiterSynergy() method. */

            /* ==============================================================================================================
             * PASSIVE ITEM SYNERGIES
             * ============================================================================================================== */

            /* Rod of Ages [EXAMPLE] */
            //NewSynergy(LOLItems.Synergy.AGE_OLD_WISDOM, "Age old wisdom", new[] { IName(RodOfAges.ItemName) }, new[] { "old_knights_shield", "old_knights_helm", "old_knights_flask" });

            /* Scyto's Bias */
            NewSynergy(Scytodes.Synergy.SCYTOS_BIAS, "Scyto's Bias", new[] { IName(TheItemInQuestion.ItemName), "blasphemy" }, null, true);

            /* Name Filler */
            //NewSynergy();

            /* ==============================================================================================================
             * ACTIVE ITEM SYNERGIES
             * ============================================================================================================== */

            /* Refillable Potion [EXAMPLE] */
            //NewSynergy(LOLItems.Synergy.COCKTAIL_POTION, "Cocktail Potion", new[] { IName(RefillablePotion.ItemName) }, new[] { "potion_of_lead_skin", "potion_of_gun_friendship" });

            //Name Filler
            //NewSynergy();

            /* ==============================================================================================================
             * GUN SYNERGIES
             * ============================================================================================================== */

            //Virtue [EXAMPLE]
            //NewSynergy(LOLItems.Synergy.EXP_SHARE_FORM_1, "EXP. Share", new[] { VirtueForm1.internalName }, new[] { "macho_brace", "scouter", "life_orb" });

            //Name Filler
            //NewSynergy();

            #endregion
        }

        /* THIS IS IMPORTANT TO REFER TO WHEN MAKING NEW SYNERGIES!!! */
        private static AdvancedSynergyEntry NewSynergy(Synergy synergy, string name, string[] mandatory, string[] optional = null, bool ignoreLichEyeBullets = false)
        {
            /* Gets the enum index of our synergy. */
            int index = (int)synergy;
            /* Register the AdvancedSynergyEntry so that the game knows about it. */
            AdvancedSynergyEntry ase = RegisterSynergy(_Synergies[index], name, mandatory.ToList(), (optional != null) ? optional.ToList() : null, ignoreLichEyeBullets);
            /* Index the friendly name of our synergy. */
            _SynergyNames[index] = name;
            /* Gets the actual ID of our synergy entry in the AdvancedSynergyDatabase, which doesn't necessarily match the CustomSynergyType enum. */
            _SynergyIds[index] = GameManager.Instance.SynergyManager.synergies.Length - 1;
            /* Returns the AdvancedSynergyEntry. */
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

            /* Adds our synergy's name to the string manager so it displays properly when activated.
             * Also the itemtips file takes from the nameKey, NOT the Synergy enum!
             * If you don't know about the ItemTips mod or need help setting up an itemtips file, use the following link for more info.
             * https://thunderstore.io/c/enter-the-gungeon/p/Glorfindel/ItemTips/
             * This means for the synergy name "Scyto's Bias", it'd be turned into "#SCYTO'S_BIAS" WITH the apostrophe.
             * Reference other itemtips files if you still need help with that. And have the item tips mod installed ofc. */ 
            string nameKey = $"#{name.ToID().ToUpperInvariant()}";
            ETGMod.Databases.Strings.Synergy.Set(nameKey, name);

            AdvancedSynergyEntry entry = new AdvancedSynergyEntry()
            {
                /* Remember, your nameKey is what the item tips mod uses for synergies. */
                NameKey = nameKey,
                MandatoryItemIDs = itemIDs,
                MandatoryGunIDs = gunIDs,
                OptionalItemIDs = optItemIDs,
                OptionalGunIDs = optGunIDs,
                /* Idk what this does but everything breaks without it... */
                bonusSynergies = new() { synergy },
                /* This too, dont get rid of it. */
                statModifiers = new List<StatModifier>(),
                /* When IgnoreLichEyeBullets is set to true, your gun's synergy is ignored by the gunslinger's passive item! 
                 * For more info on the gunslinger's passive item, use the following link.
                 * https://enterthegungeon.fandom.com/wiki/Lich%27s_Eye_Bullets */
                IgnoreLichEyeBullets = ignoreLichEyeBullets,
            };

            int oldLength = GameManager.Instance.SynergyManager.synergies.Length;
            Array.Resize(ref GameManager.Instance.SynergyManager.synergies, oldLength + 1);
            GameManager.Instance.SynergyManager.synergies[oldLength] = entry;
            return entry;
        }

        /* The string after the "IName() =>" in this line defines your "prefix" when spawning this item in-game through the f2 console.
         * Note: Your prefix should be the same all across your project! Everywhere you need to input a prefix, use the same exact one.
         * This class should be set up where you do not need to touch this line. However, if you did not define your prefix in your "Plugin" class, go do that! Otherwise you will have to modify this line. */
        private static string IName(string itemName) => $"{Plugin.ModPrefix}:" + itemName.ToID();
        public static CustomSynergyType Synergy(this Synergy synergy) => _Synergies[(int)synergy];
        public static string SynergyName(this Synergy synergy) => _SynergyNames[(int)synergy];
        public static bool HasSynergy(this PlayerController player, Synergy synergy) => player.ActiveExtraSynergies.Contains((int)_SynergyIds[(int)synergy]);
    }

    /* Put your new synergies into this enum without special characters, all capitalized, and turn spaces into underscores!
     * This is NOT used in your itemtips file, your nameKey is used! */
    public enum Synergy
    {
        /* Add your synergy names to this list as shown below. */
        SCYTOS_BIAS,
        //FILLER_SYNERGY_NAME,
    };
}