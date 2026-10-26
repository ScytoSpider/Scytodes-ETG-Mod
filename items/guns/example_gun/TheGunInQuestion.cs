using Alexandria.BreakableAPI;
using Alexandria.ItemAPI;
using Alexandria.Misc;
using Alexandria.SoundAPI;
using BepInEx;
using Gungeon;
using MonoMod;
using Scytodes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* MASSIVE THANKS TO RICKY2148 IN THE "Mod The Gungeon" DISCORD SERVER 
 * ALSO MASSIVE THANKS TO REALMSXMYTHS & CAPTAIN PRETZEL IN THE SAME DISCORD SERVER
 * https://discord.gg/Ne9yh5Krsh */

namespace Scytodes.guns
{
    internal class TheGunInQuestion : AdvancedGunBehavior
    {
        /* The ID stored by the game for this gun. Can be used by other functions to call this gun. */
        public static int ID;

        /* The ID of whatever gun you want to use as a base for this gun.
         * This must be an int. Setting BaseID to 86 for example would use the Marine Sidearm as a base.
         * To find IDs for guns, use the following link.
         * https://mtgmodders.gitbook.io/etg-modding-guide/various-lists-of-ids-sounds-etc./list-of-item-and-gun-ids */
        public static int BaseID = 89;
        /* The ID of whatever gun's projectile you want to use as a base for this gun's projectiles.
         * This must be an int. Setting BaseProjectileID to 86 for example would use the Marine Sidearm's projectiles as a base.
         * The ID of a projectile is referenced by the ID of the gun it's attached to. They do not have a unique ID separate from their original gun. */
        public static int BaseProjectileID = 89;
        /* The ID of whatever gun's muzzle flash you want to use for this gun's muzzle flash.
         * This must be an int. Setting MuzzleFlashID to 86 for example would use the Marine Sidearm's muzzle flash.
         * The ID of a muzzle flash is referenced by the ID of the gun it's attached to. They do not have a unique ID separate from their original gun. */
        public static int MuzzleFlashID = 89;
        /* The ID of whatever gun's firing and reload sfx you want to use for this gun.
         * This must be an int. Setting SoundEffectsID to 86 for example would use the Marine Sidearm's firing and reload sounds.
         * The ID of a gun's sfx is referenced by the ID of the gun it's attached to. They do not have a unique ID separate from their original gun (unless you are setting them by name of the sfx file). */
        public static int SoundEffectsID = 89;

        /* Set the name of this gun here. */
        public static string FullName = "The Gun In Question";
        /* Set the name that prefixes your sprite files.
         * All sprites for this gun must begin with the EXACT same string as this prefix.
         * Example: "theguninquestion_idle_001", "theguninquestion_fire_001", "theguninquestion_reload_001"
         * Note: All sprites for your guns must be placed within specifically named folders in your COMPILED mod folder, NOT your decompiled project folder!
         * For help regarding setting up these folders, use the following link.
         * https://mtgmodders.gitbook.io/etg-modding-guide/making-a-gun/making-the-gun/setting-up-gun-sprite-folders (your gun sprites should go into the "WeaponCollection" folder)
         * Alternatively, you can head to the following github page and analyze the file structure there.
         * https://github.com/ScytoSpider/Scytodes-ETG-Mod/tree/master/Resources/sprites (this is my github page)
         * If you still need help, you can join the "Mod The Gungeon" discord server using the following link.
         * https://discord.gg/Ne9yh5Krsh
         * Also, refer to the following link for help setting up jsons/jtk2ds to accompany your sprites (at least one json/jtk2d is required for each animation).
         * https://mtgmodders.gitbook.io/etg-modding-guide/making-a-gun/making-the-gun/creating-gun-jsons */
        public static string SpritePrefix = "theguninquestion";

        /* Put the exact names of your gun firing sfx into this List<>. 
         * Use this instead of SoundEffectsID if you want to use custom sfx, separate shooting/reloading sfx, or want to play a random sfx each shot.
         * This still accepts vanilla sfx, just has less limitations (and imo looks more pleasant).
         * To find the exact names of vanilla sfx, use the following link.
         * https://mtgmodders.gitbook.io/etg-modding-guide/sounds/sound-list
         * Note: You don't need to put mutliple sfx into this list. Having one sfx will just play that sfx like normal. */
        private static List<string> normalFiringSFXList = new List<string>
        {
            "Play_WPN_plasmarifle_shot_01",
            //theguninquestion_shoot_sfx_01,
            //theguninquestion_shoot_sfx_02,
        };

        /* Put the exact names of your gun reloading sfx into this List<>. 
         * Use this instead of SoundEffectsID if you want to use custom sfx, separate shooting/reloading sfx, or want to play a random sfx each shot.
         * This still accepts vanilla sfx, just has less limitations (and imo looks more pleasant).
         * To find the exact names of vanilla sfx, use the following link.
         * https://mtgmodders.gitbook.io/etg-modding-guide/sounds/sound-list
         * Note: You don't need to put mutliple sfx into this list. Having one sfx will just play that sfx like normal. */
        private static List<string> normalReloadingSFXList = new List<string>
        {
            "Play_WPN_dl45heavylaser_reload",
            //theguninquestion_shoot_sfx_01,
            //theguninquestion_shoot_sfx_02,
        };

        /* Used for synergy logic in the Update() method.
         * You can safely DELETE or comment this line if you aren't going to have a synergy or want to set up synergies differently. */
        public bool WHYWOULDYOUDOTHISActivated = false;

        /* Used for the synergy fire rate modifier for this gun. */
        private static float WHYWOULDYOUDOTHISDissapointedFireRate = 0.7f;
        /* Used for the synergy damage modifier for this gun. */
        private static float WHYWOULDYOUDOTHISDissapointedDamage = 0.7f;

        /* Call this method from the Start() method of your ETGModule extension (usually called Plugin.cs or Module.cs).
         * If you have the GMStart() method inside your Start() method, call this method from there instead. */
        public static void Add()
        {
            /* Sets the name of this gun from the static string FullName as defined above. */
            string fullName = FullName;
            /* Sets the sprite prefixes of this gun from the static string SpriteName as defined above. */
            string spritePrefix = SpritePrefix;
            /* Sets the internal name of this gun by converting fullName with the ToID() method (makes the string all lowercase, removes certain characters, and replaces spaces with underscores). */ 
            string internalName = $"{fullName.ToID()}";
            /* Sets the ID for the base of this gun from the static int BaseID as defined above. */
            int baseID = BaseID;
            /* Sets the ID for the base projectile of this gun from the static int BaseProjectileID as defined above. */
            int baseProjectileID = BaseProjectileID;
            /* Sets the ID for the muzzle flash of this gun from the static int MuzzleFlashID as defined above. */
            int muzzleFlashID = MuzzleFlashID;
            /* Sets the ID for the firing/reload sfx of this gun from the static int SoundEffectsID as defined above. */
            int soundEffectsID = SoundEffectsID;

            /* Sets the base of this gun using baseID. */
            Gun baseGun = PickupObjectDatabase.GetById(baseID) as Gun;
            /* Adds this gun to the game with its name (fullName), its base (baseGun), and the prefix for its sprites (spritePrefix). */
            Gun gun = ETGMod.Databases.Items.NewGun($"{fullName}", baseGun, $"{spritePrefix}");

            /* Sets up this gun to be spawned in-game using the f2 console.
             * The following is the command used to spawn an item in-game: give <prefix>:<all lowercase item name but spaces are underscores>
             * <prefix>:<all lowercase item name but spaces are underscores> is also how you define an item for the ItemTips mod. Use the following link for more info on the ItemTips mod.
             * https://thunderstore.io/c/enter-the-gungeon/p/Glorfindel/ItemTips/
             * Note: Your prefix should be the same all across your project! Everywhere you need to input a prefix, use the same exact one.
             * This class should be set up where you do not need to touch this line. However, if you did not define your prefix in your "Plugin"/"Module" class, go do that (static string)! Otherwise you will have to modify this line. */
            Game.Items.Rename($"outdated_gun_mods:{internalName}", $"{Plugin.ModPrefix}:{internalName}");

            /* Adds a AdvancedGunBehavior component to the object. */
            gun.gameObject.AddComponent<TheGunInQuestion>();

            /* Sets the unique identifier for this gun as your sprite prefix. */
            gun.encounterTrackable.EncounterGuid = $"{spritePrefix}";

            /* Ammonomicon entry variables.
             * shortDesc should be one sentence max. Usually only one or a few words.
             * longDesc is your entire item description. Describe exactly what your item does and include flavor text at the top or bottom if you want. */
            gun.SetShortDescription("This is a test gun.");
            gun.SetLongDescription("Doesn't this look kinda like Pilot's pistol? Well, hopefully this recreation is better than the original...\n" + "I wouldn't show this to the Pilot.\n\n" + "If you have this item, something terrible has happened.");

            /* Sets up this gun's default sprite for the ammonomicon and the "gun get" popup.
             * Make sure to add a COPY of your default sprite to your "Ammonomicon Encounter Icon Collection" folder!
             * Example: "sprites/Ammonomicon Encounter Icon Collection/theguninquestion_idle_001.png" in your COMPILED mod folder, NOT your decompiled project folder (though it's fine to be in both)!
             * Note: You do not need to attach a json/jtk2d in your "Ammonomicon Encounter Icon Collection" folder! Only your "WeaponCollection" folder needs them.
             * This class should be set up where you do not need to touch this line. However, if you did not define your SpritePrefix, go do that (static string)! Otherwise you will have to modify this line. */
            gun.SetupSprite(null, $"{spritePrefix}_idle_001", 8);
            /* Sets this gun's fps for specific animations such as shooting and reloading
             * The first parameter in this/these line(s) determines the target animation to set the fps for this gun.
             * The last parameter in this/these line(s) determines the animation fps for the target animation.
             * Note: this gun's animation will be cut off early if its stats determine it can shoot again before its animation finishes, or reloads faster than its animation has time to play out. Make sure the animation fps doesn't collide with this gun's stats! */
            gun.SetAnimationFPS(gun.shootAnimation, 15);
            gun.SetAnimationFPS(gun.reloadAnimation, 5);

            /* Sets up this gun's intro animation that plays when picking up this gun.
             * This is OPTIONAL and NOT required unless you want an intro animation for this gun. You may safely "comment out" this section.
             * Uses sprites with "_intro" after the sprite prefix. */
            //tk2dSpriteAnimationClip clip = gun.spriteAnimator.GetClipByName($"{spritePrefix}_intro");
            /* The last parameter in this line determines the animation fps for this gun's intro animation. */
            //gun.SetAnimationFPS(gun.introAnimation, 15);

            /* Sets up this gun to use a looping shooting animation.
             * May be used for something like a minigun which spins up, then loops its full spin animation.
             * This is OPTIONAL and NOT required unless you want a looping shooting animation for this gun. You may safely "comment out" this section.
             * Note: Only use this if this gun is full-auto, NOT semi-auto. */
            //gun.usesContinuousFireAnimation = true;
            //gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).wrapMode = tk2dSpriteAnimationClip.WrapMode.LoopSection;
            /* Sets the frame to start looping from for this gun's shooting animation. Starts at the first frame by default.
            //gun.GetComponent<tk2dSpriteAnimator>().GetClipByName(gun.shootAnimation).loopStart = 0;

            /* Sets the base of this gun's projectiles using baseProjectileID. */
            gun.AddProjectileModuleFrom(PickupObjectDatabase.GetById(baseProjectileID) as Gun, true, false);

            /* Sets this gun's muzzle flash effect using muzzleFlashID.
             * This is OPTIONAL and NOT required unless you want a specific muzzle flash for this gun. You may safely "comment out" this line. */
            gun.muzzleFlashEffects = (PickupObjectDatabase.GetById(muzzleFlashID) as Gun).muzzleFlashEffects;

            /* Sets how this gun is held through gunHandedness.
             * OneHanded and TwoHanded control the player's hands hold the gun.
             * AutoDetect will select between one and two handed based on the json/jtk2d attached to this gun's sprites.
             * HiddenOneHanded holds the gun with an invisible hand (think armcannons like the Megahand (Megabuster)).
             * NoHanded means the gun should not swing or aim at all, like the Crown of Guns which sits on the player's head. */
            gun.gunHandedness = GunHandedness.AutoDetect;

            /* Offset when holding this gun vertically. */
            gun.carryPixelOffset += new IntVector2(0, 0);
            /* Offset when aiming down. */
            gun.carryPixelDownOffset += new IntVector2(0, 0);
            /* Offset when aiming up. */
            gun.carryPixelUpOffset += new IntVector2(0, 0);

            /* Sets the the length and width away on the sprite where the barrel should end.
             * This is where the muzzle flash and projectile will appear. */
            gun.barrelOffset.localPosition = new Vector3(16 / 16f, 9 / 16f, 0f);

            /* Sets the firing and reload sounds of this gun to the firing and reloading sounds of a different one. Otherwise, use a custom gunSwitchGroup name then assign your sound effects manually. */
            //gun.gunSwitchGroup = (PickupObjectDatabase.GetById(soundEffectsID) as Gun).gunSwitchGroup;
            /* !!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!
             * OR YOU CAN SET UP YOUR FIRING/RELOAD SFX USING THE LINES BELOW! DO NOT USE BOTH THE ABOVE LINE AND BELOW LINES FOR SFX! 
             * !!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!! */
            /* Sets the unique name for this gun's sound group. In this line, it uses your console name but with an underscore (_) instead of a colon (:).
             * To find the exact names of default sound files, use the following link.
             * https://mtgmodders.gitbook.io/etg-modding-guide/various-lists-of-ids-sounds-etc./sound-list
             *This class should be set up where you do not need to touch this line. However, if you did not define your prefix in your "Plugin"/"Module" class, go do that (static string)! Otherwise you will have to modify this line. */
            gun.gunSwitchGroup = $"{Plugin.ModPrefix}_{fullName.ToID()}";
            /* "Play_WPN_Gun_Shot_01" is this gun's base firing sound.
             * The last parameter in this line determines the new firing sound. Set this to null to use a custom sound (USING CUSTOM SOUNDS REQUIRES MORE SETUP). */
            SoundManager.AddCustomSwitchData("WPN_Guns", gun.gunSwitchGroup, "Play_WPN_Gun_Shot_01", null);
            /* "Play_WPN_Gun_Reload_01" is this gun's base reload sound.
             * The last parameter in this line determines the new reload sound. Set this to null to use a custom sound (USING CUSTOM SOUNDS REQUIRES MORE SETUP). */
            SoundManager.AddCustomSwitchData("WPN_Guns", gun.gunSwitchGroup, "Play_WPN_Gun_Reload_01", null);

            /* Sets the quality (rarity) of this gun.
             * You cannot have a standard quality AND have special, excluded, or common quality at the same time!
             * However, remember that you MUST have a quality set so make sure to pick either a standard quality or special/excluded/common!
             * Standard qualities include D, C, B, A, and S.
             * COMMON quality allows items to spawn in D tier chests and as a room clear reward. This is also the same quality for hearts, armor, keys, and blanks (but not heart containers).
             * SPECIAL quality is used by Master Rounds, Junk, Lies, and other "specific method for obtaining" items. This quality stops items from spawning in-game normally (except MODDED shops which explicitly allow this item).
             * EXCLUDED quality stops items from spawning in-game and hides it from the ammonomicon. Only use this quality for WIP items or other items you dont want players to have at any time in a run (note: this doesnt stop the item from being spawned in using the f2 console). */
            gun.quality = PickupObject.ItemQuality.EXCLUDED;

            /* Lets this gun spawn from C, B, and A tiers, not just its own quality.
             * This is ignored if this gun's quality is not C, B, or A. */
            gun.ItemSpansBaseQualityTiers = true;

            /* Hides this gun from the ammonomicon.
             * Set this line to true if you don't want this gun to appear in the ammonomicon. 
             * This is ignored if this gun's quality is EXCLUDED. */
            gun.encounterTrackable.journalData.SuppressInAmmonomicon = false;

            /* Hides this gun from the player's inventory ammonomicon.
             * Set this line to true if you don't want this gun to appear in the player's inventory ammonomicon (I wouldn't recommend doing this for 99% of items).*/
            gun.encounterTrackable.SuppressInInventory = false;

            /* Sets whether this gun increases the player's hidden magnificence stat when picking up this gun FROM an A tier chest.
             * Higher magnificence reduces the chance of A and S quality items appearing for the rest of a run. 
             * Set this line to true to stop the magnificence stat from increasing, and vice versa by setting it to false.
             * This is mostly used for heart containers and makes this gun count as a heart container for boss pedestals. */
            gun.ItemRespectsHeartMagnificence = false;

            /* Stops this gun from being picked up by the Resourceful Rat if you don't pick it up and leave the room (Does NOT include DROPPING an item, that's covered in the line below).
             * Set this line to true make him ignore this gun. */
            gun.IgnoredByRat = false;

            /* Stops this gun from being picked up by the Resourceful Rat if you DROP it and leave the room.
             * This is ignored if IgnoredByRat is false. 
             * Set this line to true to make him ignore this gun after DROPPING it. */
            gun.ClearIgnoredByRatFlagOnPickup = false;

            /* Excludes this gun from Bello's shop (normal shopkeeper).
             * Set this line to true if you don't want this gun to be sold in Bello's shop.
             * This is ignored if this gun has SPECIAL or EXCLUDED quality */
            gun.ShouldBeExcludedFromShops = false;

            /* Adds this gun to specific special shops. These accept any quality EXCEPT special & excluded (idk if this works for COMMON items)!
             * When added to a special shop, this gun will still appear even with ShouldBeExcludedFromShops set to true!
             * ShouldBeExcludedFromShops being set to true only matters for Bello's shop (normal shopkeeper)!
             * Uncomment the related lines for the special shops you want to include this gun in. */
            /* Goopton is the one that primarily sells goop\liquid related items. */
            //gun.AddToSubShop(ItemBuilder.ShopType.Goopton);
            /* Flynt is the one that primarily sells key, chest, and mimic related items. */
            //gun.AddToSubShop(ItemBuilder.ShopType.Flynt);
            /* Old Red is the one that primarily sells blank and bullet related items. */
            //gun.AddToSubShop(ItemBuilder.ShopType.OldRed);
            /* Trorc is the one that primarily sells miliary styled/related items. */
            //gun.AddToSubShop(ItemBuilder.ShopType.Trorc);
            /* Cursula is the one that sells cursed items. 
             * Any item bought from her gives an additional 2.5 curse on top of the default curse of the item, even if it doesn't give curse normally.
             * If you steal from Cursula, however, it only adds 1 curse on top since stealing (from any shop) always gives 1 curse, and it bypasses the "purchasing" of the item. */
            //gun.AddToSubShop(ItemBuilder.ShopType.Cursula);

            /* Lets this gun have a custom buy price. */
            gun.UsesCustomCost = false;
            /* This is ignored if UsesCustomCost is false. */
            gun.CustomCost = 22;

            /* Sets whether this gun can be dropped or not */
            gun.CanBeDropped = true;

            gun.DefaultModule.ammoCost = 1;
            gun.DefaultModule.angleVariance = 20f;
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            gun.reloadTime = 1.2f;
            gun.DefaultModule.cooldownTime = 0.3f;
            gun.DefaultModule.numberOfShotsInClip = 8;
            gun.SetBaseMaxAmmo(250);
            gun.InfiniteAmmo = true; //Gives a gun infinite ammo. By default infinite ammo guns can't crack walls leading to secret rooms.
            gun.CanGainAmmo = false; //Prevents a gun from being able to pick up ammo boxes.
            //gun.preventRotation = true; //Prevents the gun from rotating with aim direction -> will always face directly right or left.
            //gun.CanReloadNoMatterAmmo = true; //Allows a gun to trigger reload events if it has a full clip or zero ammo.
            //gun.GainsRateOfFireAsContinueAttack = true; //Makes gun shoot faster the longer fire is held (e.g., Vulcan Cannon).
            //gun.RateOfFireMultiplierAdditionPerSecond = rampUpFactor; //Used in conjunction with GainsRateOfFireAsContinueAttack.
            //gun.Volley.ModulesAreTiers = true; //Treats each individual volley of a gun as a tier that can be selected using gun.CurrentStrengthTier
            //gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            //gun.DefaultModule.customAmmoType = "mega";

            /* This block of code helps clone our projectile. Basically it makes it so things like Shadow Clone and Hip Holster keep the stats/sprite of your custom gun's projectiles. */
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);

            /* For these impact sound effects, the file names have to start with "Play_WPN" + "yourSoundFileName" + "_impact_01". */
            //projectile.objectImpactEventName = "objectImpact1";
            //projectile.enemyImpactEventName = "enemyImpact1";

            /* This section makes sure cloned copies of your projectile have the same properties. */
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;

            //You can modify a good number of stats but for now, let's just modify the damage and speed.
            projectile.baseData.damage = 4f;
            projectile.baseData.speed = 18f;
            projectile.baseData.range = 10f;
            projectile.baseData.force = 4f;
            projectile.transform.parent = gun.barrelOffset;
            //This determines what sprite you want your projectile to use. Note this isn't necessary if you don't want to have a custom projectile sprite.
            //The x and y values determine the size of your custom projectile
            //projectile.SetProjectileSpriteRight("build_projectile", x, y, null, null);
            ETGMod.Databases.Items.Add(gun, null, "ANY");

            ID = gun.PickupObjectId;
        }

        /* This code runs when the player fires this gun.
         * Plays a random sfx from your normalFiringSFXList (List<>). */
        public override void OnPostFired(PlayerController player, Gun gun)
        {
            HelpfulMethods.PlayRandomSFX(gun.gameObject, normalFiringSFXList);
            base.OnPostFired(player, gun);
        }
        /* This code runs when the player reloads this gun.
         * Plays a random sfx from your normalReloadingSFXList (List<>). */
        public override void OnReload(PlayerController player, Gun gun)
        {
            HelpfulMethods.PlayRandomSFX(gun.gameObject, normalReloadingSFXList);
            base.OnReload(player, gun);
        }

        /* Handles synergy logic.
         * For help regarding setting up synergies, use the following link.
         * https://mtgmodders.gitbook.io/etg-modding-guide/making-mods-setup/synergies
         * Alternatively, you can head to either of the following 2 github pages and analyze the code there.
         * https://github.com/ScytoSpider/Scytodes-ETG-Mod/blob/master/ScytodesSynergies.cs (this is my synergy class, adapted code)
         * https://github.com/Ricky2148/ETG-Mod-League-of-Legends-Items/blob/main/Synergies.cs (this is Ricky2148's synergy class)
         * If you still need help, you can join the "Mod The Gungeon" discord server using the following link.
         * https://discord.gg/Ne9yh5Krsh */
        protected override void Update()
        {
            if (Player != null)
            {
                if (Player.HasSynergy(Synergy.WHY_WOULD_YOU_DO_THIS) && !WHYWOULDYOUDOTHISActivated)
                {
                    ItemBuilder.AddCurrentGunStatModifier(gun, PlayerStats.StatType.RateOfFire, WHYWOULDYOUDOTHISDissapointedFireRate, StatModifier.ModifyMethod.MULTIPLICATIVE);
                    ItemBuilder.AddCurrentGunStatModifier(gun, PlayerStats.StatType.Damage, WHYWOULDYOUDOTHISDissapointedDamage, StatModifier.ModifyMethod.MULTIPLICATIVE);
                    Player.stats.RecalculateStatsWithoutRebuildingGunVolleys(Player);

                    WHYWOULDYOUDOTHISActivated = true;
                }
                else if (!Player.HasSynergy(Synergy.WHY_WOULD_YOU_DO_THIS) && WHYWOULDYOUDOTHISActivated)
                {
                    ItemBuilder.RemoveCurrentGunStatModifier(gun, PlayerStats.StatType.RateOfFire);
                    ItemBuilder.RemoveCurrentGunStatModifier(gun, PlayerStats.StatType.Damage);
                    Player.stats.RecalculateStatsWithoutRebuildingGunVolleys(Player);

                    WHYWOULDYOUDOTHISActivated = false;
                }
            }

            base.Update();
        }
    }
}