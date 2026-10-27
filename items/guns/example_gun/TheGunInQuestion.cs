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

/* !!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!
 * THIS GUN EXAMPLE DOES NOT INCLUDE THE CODE TO MAKE A CHARGED OR BEAM GUN! YOU WILL HAVE TO ADD THE RESPECTIVE CODE YOURSELF!
 * ALSO, THIS GUN EXAMPLE DOES NOT INCLUDE THE CODE FOR MULTIPLE PROJECTILES WHICH INCLUDES MAKING A SEPARATE PROJECTILE FOR THE END OF YOUR MAG/CLIP!
 * !!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!! */

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
         * The ID of a gun's sfx is referenced by the ID of the gun it's attached to. They do not have a unique ID separate from their original gun (unless you are setting them by name of the sfx file).
         * If you want to use custom sfx, separate shooting/reloading sfx, or want to play a random sfx each shot, use normalFiringSFXList (List<>) and normalReloadingSFXList (List<>) instead.
         * If you want to use custom sfx, make sure that you insert the following code into the Start() method of your ETGModule extension (usually called Plugin.cs or Module.cs).
         * SoundManager.LoadSoundbanksFromAssembly();
         * If you have the GMStart() method inside your Start() method, call the above code from there instead. */
        public static int SoundEffectsID = 89;

        /* Set the name of this gun here. */
        public static string FullName = "The Gun In Question";
        /* Set the name that prefixes your sprite files.
         * All sprites for this gun must begin with the EXACT same string as this prefix.
         * Example: "theguninquestion_idle_001", "theguninquestion_fire_001", "theguninquestion_reload_001"
         * Note: All sprites for your guns must be placed within specifically named folders in your COMPILED mod folder, NOT your decompiled project folder!
         * If you want to ONLY place your sprites in your DECOMPILED project folder and compile them with your mods ".dll" file, insert the following code into the Start() method of your ETGModule extension (usually called Plugin.cs or Module.cs).
         * ETGMod.Assets.SetupSpritesFromAssembly(Assembly.GetExecutingAssembly(), "<your mod's project folder name>/Resources/sprites");
         * If you have the GMStart() method inside your Start() method, call the above code from there instead.
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
         * If you want to use this, make sure where you call AddCustomSwitchData() for your firing sound has null as its last parameter.
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
         * If you want to use this, make sure where you cal AddCustomSwitchData() for your reloading sound has null as its last parameter.
         * Note: You don't need to put mutliple sfx into this list. Having one sfx will just play that sfx like normal. */
        private static List<string> normalReloadingSFXList = new List<string>
        {
            "Play_WPN_dl45heavylaser_reload",
            //theguninquestion_shoot_sfx_01 [EXAMPLE],
            //theguninquestion_shoot_sfx_02 [EXAMPLE],
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

            /* Offset when holding this gun vertically. 
             * You can usually leave these as is (I think). */
            gun.carryPixelOffset += new IntVector2(0, 0);
            /* Offset when aiming down. */
            gun.carryPixelDownOffset += new IntVector2(0, 0);
            /* Offset when aiming up. */
            gun.carryPixelUpOffset += new IntVector2(0, 0);

            /* Sets the the length and width away on the sprite where the barrel should end.
             * This is where the muzzle flash and projectile will appear. 
             * I forget why it's important but divding by 16f helps when figuring out your offset (I think). */
            gun.barrelOffset.localPosition = new Vector3(16 / 16f, 9 / 16f, 0f);

            /* Sets the firing and reload sounds of this gun to the firing and reloading sounds of a different one. */
            //gun.gunSwitchGroup = (PickupObjectDatabase.GetById(soundEffectsID) as Gun).gunSwitchGroup;
            /* !!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!
             * OR YOU CAN SET UP YOUR FIRING/RELOAD SFX USING THE LINES BELOW! DO NOT USE BOTH THE ABOVE LINE AND BELOW LINES FOR SFX!
             * MORE INFO ON WHICH TO USE CAN BE FOUND AT THE TOP OF THIS CLASS IN THE COMMENTS FOR soundEffectsID, normalFiringSFXList, AND normalReloadingSFXList!
             * !!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!!--!! */
            /* Sets the unique name for this gun's sound group. In this line, it uses your console name but with an underscore (_) instead of a colon (:).
             * To find the exact names of default sound files, use the following link.
             * https://mtgmodders.gitbook.io/etg-modding-guide/various-lists-of-ids-sounds-etc./sound-list
             *This class should be set up where you do not need to touch this line. However, if you did not define your prefix in your "Plugin"/"Module" class, go do that (static string)! Otherwise you will have to modify this line. */
            gun.gunSwitchGroup = $"{Plugin.ModPrefix}_{fullName.ToID()}";
            /* "Play_WPN_Gun_Shot_01" is this gun's base firing sound.
             * The last parameter in this line determines the new firing sound. Set this to null to use a custom sound (USING CUSTOM SOUNDS REQUIRES MORE SETUP).
             * If you are using the normalFiringSFXList (List<>), you do not need to touch this line (as long as the last parameter is null). */
            SoundManager.AddCustomSwitchData("WPN_Guns", gun.gunSwitchGroup, "Play_WPN_Gun_Shot_01", null);
            /* "Play_WPN_Gun_Reload_01" is this gun's base reload sound.
             * The last parameter in this line determines the new reload sound. Set this to null to use a custom sound (USING CUSTOM SOUNDS REQUIRES MORE SETUP). 
             * If you are using the normalRelodingSFXList (List<>), you do not need to touch this line (as long as the last parameter is null). */
            SoundManager.AddCustomSwitchData("WPN_Guns", gun.gunSwitchGroup, "Play_WPN_Gun_Reload_01", null);

            /* Sets the class of this gun.
             * When the player picks up a gun of a specific class, it reduces the drop rate of finding other guns belonging to the same class by 50%.
             * The SHITTY class is an exception to this as it reduces the chance of finding other SHITTY class guns by 90% instead of 50%.
             * Gun clases include BEAM, CHARGE, CHARM, EXPLOSIVE, FIRE, FULLAUTO, ICE, NONE, PISTOL, POISON, RIFLE, SHITTY, SHOTGUN, and SILLY.
             * Because a gun in the NONE class has no assigned category, it does not benefit from class-specific stat boosts or item synergies that require a certain type of weapon to activate.
             * Guns in the NONE class include the gunslinger's "Slinger", Kruller Glaive, and Chamber Gun. */
            gun.gunClass = GunClass.SHITTY;

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

            /* Adds curse to the player when buying/picking up this item.
             * The last parameter in this line defines the amount of curse the player receives.
             * Set the last parameter to 0f if you do not want this item to give you curse.
             * Remember that the 'f' after the number marks it as a "float" value. Make sure to include the 'f' as AddPassiveStatModifier takes float values. */
            ItemBuilder.AddPassiveStatModifier(gun, PlayerStats.StatType.Curse, 0f);

            /* If you have multiple projectiles, this line determines in what order they are fired.
             * Setting this to Random makes them fire in a random order regardless of the order they were added to your projectile module. 
             * Setting this to Ordered makes them fire in the order they were added to your projectile module. 
             * Idk what setting it to OrderedGroups does...
             * Usually, you can just set this to Random. */
            gun.DefaultModule.sequenceStyle = ProjectileModule.ProjectileSequenceStyle.Random;
            /* Set the way your gun fires. 
             * Firing groups include Automatic, SemiAutomatic, Charged, Beam, and Burst. */
            gun.DefaultModule.shootStyle = ProjectileModule.ShootStyle.SemiAutomatic;
            /* Set how much ammo is consumed for one projectile. Usually this is set to 1. */
            gun.DefaultModule.ammoCost = 1;
            /* Set how much ammo your gun has in one mag/clip. */
            gun.DefaultModule.numberOfShotsInClip = 8;
            /* Set the number of shots per burst if this gun shoots in bursts.
             * You can safely DELETE or "comment out" this line if you do not want this gun to shoot in bursts. */
            //gun.DefaultModule.burstShotCount = 3;
            /* Set the time between shots during a burst.
             * This is NOT the same as cooldownTime. 
             * You can safely DELETE or "comment out" this line if you do not want this gun to shoot in bursts. */
            //gun.DefaultModule.burstCooldownTime = 0.1f; //Time in between shots during a burst.
            /* Set the time between shots in seconds before the player can fire this gun again. */
            gun.DefaultModule.cooldownTime = 0.3f;
            /* Gives a gun infinite ammo. By default, inifinite ammo guns can't crack walls leading to secret rooms. */
            gun.InfiniteAmmo = true;
            /* Set the max ammo you can carry for this gun (NOT your mag size). 
             * Ignored if InfiniteAmmo is set to true. */
            gun.SetBaseMaxAmmo(250);
            /* Set the "accuracy" of your gun.
             * 0f is perfectly accurate, while higher values increases the spread of this gun's projectiles. */
            gun.DefaultModule.angleVariance = 20f;
            /* Set the time it takes to reload this gun in seconds. */
            gun.reloadTime = 1.2f;
            /* Set whether this gun can use ammo boxes. */
            gun.CanGainAmmo = false;
            /* Set whether this gun will rotate with your aim direction. It will always face directly left or right. 
             * Usually, you can set this to false. */
            gun.preventRotation = false;
            /* Set whether this gun can trigger reload events if it no matter how much ammo it has.
             * You can safely DELETE or "comment out" this line if you want to use the default provided by this gun's base. */
            //gun.CanReloadNoMatterAmmo = false;
            /* Set whether this gun shoots faster the longer fire is held (like Vulcan Cannon). */
            gun.GainsRateOfFireAsContinueAttack = false;
            /* Set the factor which this gun should multiply its rate of fire. 
             * This line is ignored if GainsRateOfFireAsContinueAttack is set to false. */
            gun.RateOfFireMultiplierAdditionPerSecond = 2.2f;

            /* Set how much the gun shakes the screen when fired. 
             * Set this line to 0f if you do not want the screen to shake at all when firing this gun.
             * Alternatively, you can safely DELETE or "comment out" this line if you want to use the default provided by this gun's base. */
            //gun.gunScreenShake.magnitude = 0.8f;

            /* Sets the visual for this gun's ammo clip.
             * If you need help with how to change this, use the following link.
             * https://mtgmodders.gitbook.io/etg-modding-guide/making-a-gun/gun-ammo-types
             * You can safely DELETE or "comment out" this line if you want to use the default provided by this gun's base. */
            //gun.DefaultModule.ammoType = GameUIAmmoType.AmmoType.CUSTOM;
            /* Sets the customAmmoType for this gun's ammo clip visual.
             * This is ignored if this gun's ammoType is not CUSTOM.
             * You can safely DELETE or "comment out" this line if you want to use the default provided by this gun's base. */
            //gun.DefaultModule.customAmmoType = "mega";

            /* This line of code helps clone our projectile. Basically, it makes it so things like Shadow Clone and Hip Holster keep the stats/sprite of this gun's projectiles. */
            Projectile projectile = UnityEngine.Object.Instantiate<Projectile>(gun.DefaultModule.projectiles[0]);

            /* For these impact sound effects, the file names have to start with "Play_WPN" + "yourSoundFileName" + "_impact_01". */
            //projectile.objectImpactEventName = "objectImpact1";
            //projectile.enemyImpactEventName = "enemyImpact1";

            /* This section makes sure cloned copies of your projectile have the same properties. */
            projectile.gameObject.SetActive(false);
            FakePrefab.MarkAsFakePrefab(projectile.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(projectile);
            gun.DefaultModule.projectiles[0] = projectile;

            /* Set the damage of this gun's projectiles. */
            projectile.baseData.damage = 4f;
            /* Set the speed of this gun's projectiles. */
            projectile.baseData.speed = 18f;
            /* Set the range of this gun's projectiles. */
            projectile.baseData.range = 10f;
            /* Set this knockback force of this gun's projectiles. */
            projectile.baseData.force = 4f;
            /* Set whether this gun's projectiles should rotate depending on the direction the player is aiming. */
            projectile.shouldRotate = true;
            /* Set the scale of this gun's projectiles.
             * Usually, you can just set this to 1f. */
            projectile.AdditionalScaleMultiplier = 1f;
            /* This line makes sure your bullet spawns on this gun's barrel. */
            projectile.transform.parent = gun.barrelOffset;

            /* Set a custom Projectile Sprite if you don't want to use this gun's default.
             * The first parameter in this line is the sprite name in sprites\ProjectileCollection.
             * tk2dBaseSprite.Anchor.MiddleCenter controls where the sprite is anchored. MiddleCenter will work in most cases.
             * The first set of numbers is the visual dimensions of the sprite while the last set of numbers is the hitbox.  Generally the hitbox should be a little smaller than the visuals.
             * Note: your sprite will stretch to match the visual dimensions.
             * You can safely DELETE or "comment out" this line if you want to use the default provided by this gun's base. */
            //projectile.SetProjectileSpriteRight("theguninquestion_projectile_001", 16, 7, true, tk2dBaseSprite.Anchor.MiddleCenter, 14, 5);

            /* Adds this gun to the list of items in-game. */
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