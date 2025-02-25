using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FireSelection
{
    [HarmonyPatch(typeof(HUDPanel))]
    [HarmonyPatch("OnUpdateMagAmmo")]
    public class harmonyAmmoPanel
    {
        static public UILabel ammoPanel;
        static private bool initialized;
        static void Postfix(HUDPanel __instance)
        {
            if (!initialized)
                ammoPanel = __instance.AmmoType;
            else
                initialized = true;
        }
    }

    [HarmonyPatch(typeof(PlayerControl))]
    [HarmonyPatch("OnWeaponPullTrigger")]
    public class harmonyFireMode
    {
        public static GunFireModes FireMode;
        public static GunFireModes baseMode;

        static void Prefix(PlayerControl __instance)
        {
            Gun weapon = GameManager.Inst.PlayerControl.SelectedPC.MyReference.CurrentWeaponG;
            baseMode = weapon.CurrentFireMode;

            // sets the burst firerate if weapon is burst.
            weapon.CurrentFireMode = FireMode;
        }

        // For some reason just doing the prefix bugs out, so we need to revert it. Also helps prevent permanent change to the weapon.
        static void Postfix(PlayerControl __instance)
        {
            //Reset firemode here?
            GameManager.Inst.PlayerControl.SelectedPC.MyReference.CurrentWeaponG.CurrentFireMode = baseMode;
        }
    }

}
