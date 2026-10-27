using Alexandria;
using Alexandria.ItemAPI;
using Alexandria.SoundAPI;
using BepInEx;
using Scytodes.guns;
using Scytodes.passive_items;
using Scytodes.active_items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Scytodes
{
    [BepInDependency(Alexandria.Alexandria.GUID)] // this mod depends on the Alexandria API: https://enter-the-gungeon.thunderstore.io/package/Alexandria/Alexandria/
    [BepInDependency(ETGModMainBehaviour.GUID)]
    [BepInPlugin(GUID, NAME, VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public const string GUID = "scytodesthoracica.etg.scytodes";
        public const string NAME = "Scytodes";
        public const string VERSION = "1.0.0";
        public const string TEXT_COLOR = "#671090";

        /* This line defines your "prefix" when spawning this item in-game through the f2 console.
         * The following is the command used to spawn an item in-game: give <prefix>:<all lowercase item name but spaces are underscores>
         * <prefix>:<all lowercase item name but spaces are underscores> is also how you define an item for the ItemTips mod. Use the following link for more info on the ItemTips mod.
         * https://thunderstore.io/c/enter-the-gungeon/p/Glorfindel/ItemTips/
         * Note: Your prefix should be the same all across your project! Everywhere you need to input a prefix, use the same exact one.
         * However, this should be the only line in your project that you are defining your prefix, to reduce the chance of errors. */
        public static string ModPrefix = "Scytodes";

        public void Start()
        {
            ETGModMainBehaviour.WaitForGameManagerStart(GMStart);
        }

        public void GMStart(GameManager g)
        {
            /* Lets you compile your gun sprites into your mod's ".dll" file, and not have to include them with your mod's COMPILED code. */
            ETGMod.Assets.SetupSpritesFromAssembly(Assembly.GetExecutingAssembly(), "Scytodes/Resources/sprites");
            /* Lets you use custom sfx files for your guns. */
            SoundManager.LoadSoundbanksFromAssembly();

            /* Initialize all your passive items here. */
            #region PassiveItems
            TheItemInQuestion.Init();
            //FillerName.Init();
            #endregion

            /* Initialize all your active items here. */
            #region ActiveItems
            TheActiveInQuestion.Init();
            //FillerName.Init();
            #endregion

            /* Initialize all your guns here. */
            #region Guns
            TheGunInQuestion.Add();
            //FillerName.Add();
            #endregion

            /* Synergies Init (You won't need more than just this line). */
            ScytodesSynergies.Init();

            Log($"{NAME} v{VERSION} started successfully. If any bugs occur with my mod, contact me at lethal_inc. on discord!", TEXT_COLOR);
        }

        public static void Log(string text, string color="#FFFFFF")
        {
            ETGModConsole.Log($"<color={color}>{text}</color>");
        }
    }
}
