using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager;
/*
 
                GameManager.Inst.ItemManager.LoadItem("ak47").Name = "AK 47";
In update, new mod to change to real names??
 
 */
namespace FireSelection
{
    
    public static class Main
    {
        public static Settings settings;
        public static UnityModManager.ModEntry ModEntry;
        public static Dictionary<string, GunFireModes> currentWeaponState = new Dictionary<string, GunFireModes>();
        private static string originalText;
        public static string weaponID;
        private const string FILENAME = "weaponStates.xml";

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            settings = Settings.Load<Settings>(modEntry);
            // Create a Harmony instance
            var harmony = new Harmony("com.example.harmonypatch");
            ModEntry = modEntry;
            // Apply the patch
            harmony.PatchAll();
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
            modEntry.OnUpdate = onUpdate;
            try { 
                currentWeaponState = Save.LoadWeaponStates(modEntry.Path + FILENAME);
            }
            catch { } // Ignores if no file is found.
            return true;
        }

        static private string getText(GunFireModes fireMode)
        {
            switch (fireMode)
            {
                case GunFireModes.Semi: return "\nSingle";
                case GunFireModes.Burst: return "\nBurst";
                case GunFireModes.Full: return "\nAuto";
                default: return "";
            }
        }


        public static Dictionary<string, List<GunFireModes>> weapons = new Dictionary<string, List<GunFireModes>>()
        {
            { "m16", new List<GunFireModes> { GunFireModes.Semi, GunFireModes.Burst } },
            { "ump45", new List<GunFireModes> { GunFireModes.Semi, GunFireModes.Full} }, // add burst?
            { "ak47", new List<GunFireModes> { GunFireModes.Semi, GunFireModes.Full } },
            { "skorpion", new List<GunFireModes> { GunFireModes.Semi, GunFireModes.Full } },
            { "ak74", new List<GunFireModes> { GunFireModes.Semi, GunFireModes.Full } },
            { "asval", new List<GunFireModes> { GunFireModes.Semi, GunFireModes.Full } },
            { "vss", new List<GunFireModes> { GunFireModes.Semi, GunFireModes.Full } },
            { "ppsh41", new List<GunFireModes> { GunFireModes.Semi, GunFireModes.Full } },
            { "pp19bizon", new List<GunFireModes> { GunFireModes.Semi, GunFireModes.Full } },
            { "thompson", new List<GunFireModes> { GunFireModes.Semi, GunFireModes.Full } },
            { "jackhammer", new List<GunFireModes> { GunFireModes.Semi, GunFireModes.Full } },
            { "sr3viktor", new List<GunFireModes> { GunFireModes.Semi, GunFireModes.Full } },
            { "ak74u", new List<GunFireModes> { GunFireModes.Semi, GunFireModes.Full } }
        };

        public static bool isAllowed(string id)
        {
            switch (id)
            {
                case "m16": return settings.WeaponsSettings.M16;
                case "ump45": return settings.WeaponsSettings.ump45;
                case "skorpion": return settings.WeaponsSettings.skorpion;
                case "ak47": return settings.WeaponsSettings.AK47;
                case "ak74": return settings.WeaponsSettings.AK74;
                case "ak74u": return settings.WeaponsSettings.AK74u;
                case "asval": return settings.WeaponsSettings.asval;
                case "vss": return settings.WeaponsSettings.vss;
                case "ppsh41": return settings.WeaponsSettings.ppsh41;
                case "pp19bizon": return settings.WeaponsSettings.pp19bizon;
                case "sr3viktor": return settings.WeaponsSettings.sr3viktor;
                case "thompson": return settings.WeaponsSettings.thompson;
                case "jackhammer": return settings.WeaponsSettings.jackhammer;
                default: return false;
            }
        }

        private static void setFireMode(string id, GunFireModes fireMode)
        {
            List<GunFireModes> modes = weapons[id];
            int currentIndex = modes.IndexOf(fireMode);
            if (currentIndex >= 0)
            {
                int nextIndex = (currentIndex + 1) % modes.Count;
                harmonyFireMode.FireMode = modes[nextIndex];
            }
        }
        static void onUpdate(UnityModManager.ModEntry modEntry, float dt)
        {
            try
            {
                Item weapon = GameManager.Inst.PlayerControl.SelectedPC.MyAI.BlackBoard.EquippedWeapon;
                
                if(weapon != null)
                {
                    if (weaponID != weapon.ID)
                    {
                        harmonyFireMode.FireMode = GameManager.Inst.PlayerControl.SelectedPC.MyReference.CurrentWeaponG.CurrentFireMode;
                        weaponID = null;
                    }

                    if ((bool)weapon.GetAttributeByName("_IsRanged").Value && weapons.ContainsKey(weapon.ID.ToLower()) && isAllowed(weapon.ID))
                    {
                        
                        if(weaponID == null && currentWeaponState.ContainsKey(weapon.ID))
                        {
                            harmonyFireMode.FireMode = currentWeaponState[weapon.ID];
                            weaponID = weapon.ID;
                        }
                        else if (!currentWeaponState.ContainsKey(weapon.ID))
                        {
                            currentWeaponState.Add(weapon.ID, harmonyFireMode.FireMode);
                        }

                        if (Input.GetKeyDown(settings.key))
                        {
                            if (weaponID == null)
                            {
                                weaponID = weapon.ID;
                            }
                            setFireMode(weaponID, harmonyFireMode.FireMode);
                            currentWeaponState[weaponID] = harmonyFireMode.FireMode;

                            Save.SaveWeaponStates(modEntry.Path + FILENAME, currentWeaponState);
                        }
                        
                        if (originalText != null) { 
                            harmonyAmmoPanel.ammoPanel.text = originalText;
                        }
                        else 
                        { 
                            originalText = harmonyAmmoPanel.ammoPanel.text;
                        }
                        harmonyAmmoPanel.ammoPanel.text += getText(harmonyFireMode.FireMode);

                    }
                }
            }
            catch
            {
                //Ignore
            }
        }
        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Draw(modEntry);
            settings.OnChange();
        }

        static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Save(modEntry);
        }
    }
}
