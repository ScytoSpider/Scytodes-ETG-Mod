using Alexandria.ItemAPI;
using Alexandria.Misc;
using Dungeonator;
using Scytodes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/* MASSIVE THANKS TO RICKY2148 IN THE "Mod The Gungeon" DISCORD SERVER
 * ALSO MASSIVE THANKS TO REALMSXMYTHS & CAPTAIN PRETZEL IN THE SAME DISCORD SERVER
 * https://discord.gg/Ne9yh5Krsh */

/* For help regarding setting up synergies, use the following link.
 * https://mtgmodders.gitbook.io/etg-modding-guide/making-mods-setup/synergies
 * Alternatively, you can head to either of the following 2 github pages and analyze the code there.
 * https://github.com/ScytoSpider/Scytodes-ETG-Mod/blob/master/ScytodesSynergies.cs (this is my synergy class, adapted code)
 * https://github.com/Ricky2148/ETG-Mod-League-of-Legends-Items/blob/main/Synergies.cs (this is Ricky2148's synergy class)
 * If you still need help, you can join the "Mod The Gungeon" discord server using the following link.
 * https://discord.gg/Ne9yh5Krsh */

namespace Scytodes.active_items
{
    public class APieceOfHome : PlayerItem
    {
        /* Set the name of this item here. */
        public static string ItemName = "A Piece Of Home";

        /* Put the exact names of your active's on-use sfx into this List<>. 
         * To find the exact names of vanilla sfx, use the following link.
         * https://mtgmodders.gitbook.io/etg-modding-guide/sounds/sound-list
         * Note: You don't need to put mutliple sfx into this list. Having one sfx will just play that sfx like normal. */
        private static List<string> normalUseSFXList = new List<string>
        {
            "siren_allure_SFX_01",
            "siren_allure_SFX_02",
            "siren_allure_SFX_03",
            "siren_allure_SFX_04",
        };

        /* Call this method from the Start() method of your ETGModule extension (usually called Plugin.cs or Module.cs).
         * If you have the GMStart() method inside your Start() method, call this method from there instead. */
        public static void Init()
        {
            /* Sets the name of this item from the static string ItemName as defined above. */
            string itemName = ItemName;

            /* This is your item's sprite. If you need help making a sprite, use the following link.
             * https://mtgmodders.gitbook.io/etg-modding-guide/all-things-spriting (I personally use libesprite over aseprite because it's free).
             * Refers to an embedded png in the project. Make sure to embed your resources!
             * DO NOT include the ".png" or any other file extension in resourceName! Just the name of the file.
             * To embed your sprite: In Solution Explorer, Show all files > Find your png > Right click, include in project > Right click, Properties > Set Build Action to Embedded Resource. */
            string resourceName = "Scytodes/Resources/active_item_sprites/a_piece_of_home";

            /* Create new GameObject. */
            GameObject obj = new GameObject(itemName);

            /* Adds a PassiveItem component to the object. */
            var item = obj.AddComponent<APieceOfHome>();

            /* Adds a sprite component to the object and adds your texture to the item sprite collection. */
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            /* Ammonomicon entry variables.
             * shortDesc should be one sentence max. Usually only one or a few words.
             * longDesc is your entire item description. Describe exactly what your item does and include flavor text at the top or bottom if you want. */
            string shortDesc = "Anywhere you go";
            string longDesc = "The pearlescent scale of an aquatic beast. Grants an extra active item slot.\n" + "Enables Sairyn to call upon her ancestry to allure enemies toward her for a short time. Those affected may experience reduced spatial awareness... This strains her voice though, so she has to take breaks in-between uses.\n\n" + "It's a little piece of home.";

            /* Adds this item to the gungeon item list, ammonomicon, loot table, etc.
             * This line should always come AFTER ItemBuilder.AddSpriteToObject()!
             * The last parameter in this line defines your "prefix" when spawning this item in-game through the f2 console.
             * The following is the command used to spawn an item in-game: give <prefix>:<all lowercase item name but spaces are underscores>
             * <prefix>:<all lowercase item name but spaces are underscores> is also how you define an item for the ItemTips mod. Use the following link for more info on the ItemTips mod.
             * https://thunderstore.io/c/enter-the-gungeon/p/Glorfindel/ItemTips/
             * Note: Your prefix should be the same all across your project! Everywhere you need to input a prefix, use the same exact one.
             * This class should be set up where you do not need to touch this line. However, if you did not define your prefix in your "Plugin"/"Module" class, go do that (static string)! Otherwise you will have to modify this line. */
            ItemBuilder.SetupItem(item, shortDesc, longDesc, $"{Plugin.ModPrefix}");

            /* Adds a passive effect to this item. Though this is an active item so you probably don't want this.
             * Depending on what you want your item to do, you may or may not want to delete this part.
             * You can safely DELETE or "comment out" this section if you do not want this item to have passive effects. */
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.AdditionalItemCapacity, 1);

            /* Sets the cooldown type of this item.
             * Cooldown types include Damage, Timed, PerRoom, and None. 
             * Damage recharges active items based on damage dealt by the player since last use.
             * Timed recharges active items based on how much time has passed in seconds since last use.
             * PerRoom recharges active items based on how many rooms the player has cleared since last use.
             * None is used for active items which can be reused instantly, have multiple charges before disappearing, or have special conditions for recharging. */
            ItemBuilder.SetCooldownType(item, ItemBuilder.CooldownType.Timed, 33f);

            /* Set whether this item is one-time consumable or not.
             * Instead of recharging, this item is destroyed upon use.
             * If this line is set to true and the player picks up another copy of this item, they will stack within the same active item slot (ex: Chaff Grenade). */
            item.consumable = false;

            /* Set whether this item is usable during a dodgeroll. */
            item.usableDuringDodgeRoll = true;

            /* Sets the quality (rarity) of this item.
             * You cannot have a standard quality AND have special, excluded, or common quality at the same time!
             * However, remember that you MUST have a quality set so make sure to pick either a standard quality or special/excluded/common!
             * Standard qualities include D, C, B, A, and S.
             * COMMON quality allows items to spawn in D tier chests and as a room clear reward. This is also the same quality for hearts, armor, keys, and blanks (but not heart containers).
             * SPECIAL quality is used by Master Rounds, Junk, Lies, and other "specific method for obtaining" items. This quality stops items from spawning in-game normally (except MODDED shops which explicitly allow this item).
             * EXCLUDED quality stops items from spawning in-game and hides it from the ammonomicon. Only use this quality for WIP items or other items you dont want players to have at any time in a run (note: this doesnt stop the item from being spawned in using the f2 console). */
            item.quality = PickupObject.ItemQuality.SPECIAL;

            /* Lets this item spawn from C, B, and A tiers, not just its own quality.
             * This is ignored if this item's quality is not C, B, or A. */
            item.ItemSpansBaseQualityTiers = false;

            /* Hides this item from the ammonomicon.
             * Set this line to true if you don't want this item to appear in the ammonomicon. 
             * This is ignored if this item's quality is EXCLUDED. */
            item.encounterTrackable.journalData.SuppressInAmmonomicon = false;

            /* Hides this item from the player's inventory ammonomicon.
             * Set this line to true if you don't want this item to appear in the player's inventory ammonomicon. (I wouldn't recommend doing this for 99% of items).*/
            item.encounterTrackable.SuppressInInventory = false;

            /* Sets whether this item increases the player's hidden magnificence stat when picking up this item FROM an A tier chest.
             * Higher magnificence reduces the chance of A and S quality items appearing for the rest of a run. 
             * Set this line to true to stop the magnificence stat from increasing, and vice versa by setting it to false.
             * This is mostly used for heart containers and makes this item count as a heart container for boss pedestals. */
            item.ItemRespectsHeartMagnificence = false;

            /* Stops this item from being picked up by the Resourceful Rat if you don't pick it up and leave the room (Does NOT include DROPPING an item, that's covered in the line below).
             * Set this line to true make him ignore this item. */
            item.IgnoredByRat = true;

            /* Stops this item from being picked up by the Resourceful Rat if you DROP it and leave the room.
             * This is ignored if IgnoredByRat is false. 
             * Set this line to true to make him ignore this item after DROPPING it. */
            item.ClearIgnoredByRatFlagOnPickup = true;

            /* Excludes this item from Bello's shop (normal shopkeeper).
             * Set this line to true if you don't want this item to be sold in Bello's shop.
             * This is ignored if this item has SPECIAL or EXCLUDED quality */
            item.ShouldBeExcludedFromShops = true;

            /* Adds this item to specific special shops. These accept any quality EXCEPT special & excluded (idk if this works for COMMON items)!
             * When added to a special shop, this item will still appear even with ShouldBeExcludedFromShops set to true!
             * ShouldBeExcludedFromShops being set to true only matters for Bello's shop (normal shopkeeper)!
             * Uncomment the related lines for the special shops you want to include this item in. */
            /* Goopton is the one that primarily sells goop\liquid related items. */
            //item.AddToSubShop(ItemBuilder.ShopType.Goopton);
            /* Flynt is the one that primarily sells key, chest, and mimic related items. */
            //item.AddToSubShop(ItemBuilder.ShopType.Flynt);
            /* Old Red is the one that primarily sells blank and bullet related items. */
            //item.AddToSubShop(ItemBuilder.ShopType.OldRed);
            /* Trorc is the one that primarily sells miliary styled/related items. */
            //item.AddToSubShop(ItemBuilder.ShopType.Trorc);
            /* Cursula is the one that sells cursed items. 
             * Any item bought from her gives an additional 2.5 curse on top of the default curse of the item, even if it doesn't give curse normally.
             * If you steal from Cursula, however, it only adds 1 curse on top since stealing (from any shop) always gives 1 curse, and it bypasses the "purchasing" of the item. */
            //item.AddToSubShop(ItemBuilder.ShopType.Cursula);

            /* Lets this item have a custom buy price. */
            item.UsesCustomCost = false;
            /* This is ignored if UsesCustomCost is false. 
             * This price is still affected by floor price modifiers. */
            item.CustomCost = 0;

            /* Adds curse to the player when buying/picking up this item.
             * The last parameter in this line defines the amount of curse the player receives.
             * Set the last parameter to 0f if you do not want this item to give you curse.
             * Remember that the 'f' after the number marks it as a "float" value. Make sure to include the 'f' as AddPassiveStatModifier takes float values. */
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Curse, 0f);

            /* Sets whether this item can be dropped or not.
             * Since this is an active item, it can be swapped out for another active item REGARDLESS if this line is set to false or not.
             * When swapped out with CanBeDropped set to false, this item will simply drop to the floor like a normal item.
             * For an active, it is generally not recommended to set this to false. 
             * You can safely DELETE or "comment out" this line unless you're going to mess with this which again, IS NOT RECOMMENDED! */
            //item.CanBeDropped = true;
        }

        /* This code runs when this item is picked up.
         * Feel free to keep this method as it is if you are not going to use it. */
        public override void Pickup(PlayerController player)
        {
            base.Pickup(player);
            //Plugin.Log($"Player picked up {DisplayName}.");
        }

        /* This method determines whether this active item can be used. If this method doesn't return true, the player is NOT able to use it in-game until it returns true. */
        public override bool CanBeUsed(PlayerController user)
        {
            bool isBossRoom = false;
            bool hasWhitelistEnemies = false;

            RoomHandler room = user.CurrentRoom;
            if (room == null) { return false; }

            List<AIActor> enemies = room.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
            if (enemies == null) { return false; }

            foreach (AIActor enemy in enemies)
            {
                if (enemy.healthHaver.IsBoss == true || enemy.healthHaver.IsSubboss == true)
                {
                    isBossRoom = true;
                    return false;
                }
                else
                {
                    if (!ForceMoveTowardsTargetBehavior.BannedEnemies.Contains(enemy.EnemyGuid) && !isBossRoom)
                    {
                        hasWhitelistEnemies = true;
                        return true;
                    }
                    else if (!hasWhitelistEnemies)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /* This method determines what this active item actually does when the player uses it.
         * MASSIVE THANK YOU TO CAPTAIN PRETZEL IN THE "Mod The Gungeon" DISCORD SERVER!
         * HE WROTE THIS CODE! I DON'T UNDERSTAND IT ONE BIT BUT IT WORKS EXACTLY HOW I WANTED IT TO!
         * https://discord.gg/Ne9yh5Krsh */
        public override void DoEffect(PlayerController user)
        {
            if (!CanBeUsed(user)) { return; }

            RoomHandler room = user.CurrentRoom;
            if (room == null) { return; }

            List<AIActor> enemies = room.GetActiveEnemies(RoomHandler.ActiveEnemyType.All);
            if (enemies == null) { return; }

            foreach (AIActor enemy in enemies)
            {
                /* Plays a random sfx from your normalUseSFXList(List<>). */
                HelpfulMethods.PlayRandomSFX(this.gameObject, normalUseSFXList);
                enemy.StartCoroutine(ForceMoveTowardsTargetBehavior.ForceFollowPlayer(enemy, user, 4.25f));
            }
        }
    }
}