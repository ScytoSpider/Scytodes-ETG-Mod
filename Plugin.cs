using BepInEx;
using Alexandria;
using Alexandria.ItemAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Scytodes.passive_items;
using Scytodes.guns;

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

        public void Start()
        {
            ETGModMainBehaviour.WaitForGameManagerStart(GMStart);
        }

        public void GMStart(GameManager g)
        {
            //Initialize all your passive items here
            #region PassiveItems
            TheItemInQuestion.Init();
            //FillerName.Init();
            #endregion

            //Initialize all your active items here
            #region ActiveItems
            //FillerName.Init();
            #endregion

            //Initialize all your guns here
            #region Guns
            TheGunInQuestion.Add();
            //FillerName.Add();
            #endregion

            //Synergies Init (You won't need more than just this line)
            ScytodesSynergies.Init();

            Log($"{NAME} v{VERSION} started successfully. If any bugs occur with my mod, contact me at lethal_inc. on discord!", TEXT_COLOR);
        }

        public static void Log(string text, string color="#FFFFFF")
        {
            ETGModConsole.Log($"<color={color}>{text}</color>");
        }
    }
}
