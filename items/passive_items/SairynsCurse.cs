using Alexandria.ItemAPI;
using Alexandria.Misc;
using Scytodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static tk2dTextGeomGen;

/* MASSIVE THANKS TO RICKY2148 IN THE "Mod The Gungeon" DISCORD SERVER
 * ALSO MASSIVE THANKS TO REALMSXMYTHS & CAPTAIN PRETZEL IN THE SAME DISCORD SERVER
 * https://discord.gg/Ne9yh5Krsh */

namespace Scytodes.passive_items
{
    public class SairynsCurse : PassiveItem
    {
        /* Set the name of this item here. */
        public static string ItemName = "Sairyn's Curse";

        /* Set the max distance the player's projectiles will have normal damage/scale. */
        private static float NormalDamageMaxDistance = 6f;
        /* Set the distance after NormalDamageMaxDistance that determines where the final damage/scale reductions should be made by. */
        private static float FinalReductionDistance = 9f;
        /* Set the max reduction in damage/scale the player's projectiles will have after travelling past NormalDamageMaxDistance. */
        private static float FarDamageReduction = 0.2f;

        /* Used for synergy logic in the Update() method.
         * You can safely DELETE or comment this line if you aren't going to have a synergy or want to set up synergies differently. */
        //public bool SCYTOSBIASActivated = false;

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
            string resourceName = "Scytodes/Resources/passive_item_sprites/sairyns_curse";

            /* Create new GameObject. */
            GameObject obj = new GameObject(itemName);

            /* Adds a PassiveItem component to the object. */
            var item = obj.AddComponent<SairynsCurse>();

            /* Adds a sprite component to the object and adds your texture to the item sprite collection. */
            ItemBuilder.AddSpriteToObject(itemName, resourceName, obj);

            /* Ammonomicon entry variables.
             * shortDesc should be one sentence max. Usually only one or a few words.
             * longDesc is your entire item description. Describe exactly what your item does and include flavor text at the top or bottom if you want. */
            string shortDesc = "Immune to papercuts";
            string longDesc = "A WATERed-down curse. Provides immunity to contact damage. Adds one heart container.\n" + "Being the descendant of a siren has left Sairyn with a highly weakened version of the siren's curse. She is sturdier, but has trouble harming anyone who isn't close to her.\n\n" + "It's sort of useful, I guess...";

            /* Adds this item to the gungeon item list, ammonomicon, loot table, etc.
             * This line should always come AFTER ItemBuilder.AddSpriteToObject()!
             * The last parameter in this line defines your "prefix" when spawning this item in-game through the f2 console.
             * The following is the command used to spawn an item in-game: give <prefix>:<all lowercase item name but spaces are underscores>
             * <prefix>:<all lowercase item name but spaces are underscores> is also how you define an item for the ItemTips mod. Use the following link for more info on the ItemTips mod.
             * https://thunderstore.io/c/enter-the-gungeon/p/Glorfindel/ItemTips/
             * Note: Your prefix should be the same all across your project! Everywhere you need to input a prefix, use the same exact one.
             * This class should be set up where you do not need to touch this line. However, if you did not define your prefix in your "Plugin"/"Module" class, go do that (static string)! Otherwise you will have to modify this line. */
            ItemBuilder.SetupItem(item, shortDesc, longDesc, $"{Plugin.ModPrefix}");

            /* Adds the actual passive effect to this item.
             * Depending on what you want your item to do, you may or may not want to delete this part. */
            ItemBuilder.AddPassiveStatModifier(item, PlayerStats.StatType.Health, 1, StatModifier.ModifyMethod.ADDITIVE);

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
            item.ItemSpansBaseQualityTiers = true;

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

            /* Sets whether this item can be dropped or not. */
            item.CanBeDropped = false;
        }

        /* This code runs when this item is picked up.
         * If this item has some sort of effect (except stat modifiers) that activates on Pickup(), you need to disable that effect in Drop() or DisableEffect().
         * Feel free to keep this method as it is if you are not going to use it. */
        public override void Pickup(PlayerController player)
        {
            player.ReceivesTouchDamage = false;
            base.Pickup(player);
            //Plugin.Log($"Player picked up {this.EncounterNameOrDisplayName}");
            player.PostProcessProjectile += OnPostProcessProjectile;
            player.PostProcessBeam += OnPostProcessBeam;
        }

        /* This code runs when this item is removed from you OR dropped.
         * Drop() runs ONLY when this item is dropped, not when something else takes it away. You should probably use this instead if you need to disable an effect (except stat modifiers) when the player no longer has this item.
         * Feel free to keep this method as it is if you are not going to use it. */
        public override void DisableEffect(PlayerController player)
        {
            player.ReceivesTouchDamage = true;
            base.DisableEffect(player);
            //Plugin.Log($"Player dropped or got rid of {this.EncounterNameOrDisplayName}");

            if (player != null)
            {
                player.PostProcessProjectile -= OnPostProcessProjectile;
                player.PostProcessBeam -= OnPostProcessBeam;
            }
        }

        /* Handles the PostProcessProjectile event that runs once for each projectile fired by the player (bullet). */
        private void OnPostProcessProjectile(Projectile proj, float effectChanceScalar)
        {
            if (!(bool)proj) { return; }

            if (Owner.HasSynergy(Synergy.MIGHT_AS_WELL))
            {
                proj.baseData.force *= 0.5f;
            }

            // Prevent attaching the behavior more than once.
            if (proj.GetComponent<ProjectilDamageFalloff>() == null)
            {
                ProjectilDamageFalloff behavior = proj.gameObject.AddComponent<ProjectilDamageFalloff>();

                behavior.Initialize(
                    NormalDamageMaxDistance,
                    FinalReductionDistance,
                    FarDamageReduction
                );
            }
        }

        /* Handles the PostProcessProjectile event that runs once for each projectile fired by the player (beam). */
        private void OnPostProcessBeam(BeamController beam)
        {
            if (!(bool)beam) { return; }

            if (Owner.HasSynergy(Synergy.MIGHT_AS_WELL))
            {
                beam.projectile.baseData.force *= 0.5f;
            }

            if (beam && beam.Owner is PlayerController player)
            {
                // Prevent attaching the behavior more than once.
                if (beam.GetComponent<ProjectilDamageFalloff>() == null)
                {
                    ProjectilDamageFalloff behavior = beam.gameObject.AddComponent<ProjectilDamageFalloff>();

                    behavior.Initialize(
                        NormalDamageMaxDistance,
                        FinalReductionDistance,
                        FarDamageReduction
                    );
                }
            }
        }
    }
}