using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityModManagerNet;

namespace FireSelection
{
    [DrawFields(DrawFieldMask.Public)]
    public class WeaponsSettings
    {
        public bool M16 = true;
        public bool ump45 = true;
        public bool skorpion = true;
        public bool AK47 = true;
        public bool AK74 = true;
        public bool AK74u = true;
        public bool asval = true;
        public bool vss = true;
        public bool ppsh41 = true;
        public bool pp19bizon = true;
        public bool sr3viktor = true;
        public bool thompson = true;
        public bool jackhammer = true;
    }


    public class Settings : UnityModManager.ModSettings, IDrawable
    {
        //[Draw("text")] public bool variableToSet = true;
        [Draw("FireMode Select key")] public KeyCode key = KeyCode.F;
        [Draw("Weapons allowed to be fire-selected", Collapsible = true)] public WeaponsSettings WeaponsSettings = new WeaponsSettings();

        public override void Save(UnityModManager.ModEntry modEntry)
        {
            Save(this, modEntry);
        }

        public void OnChange()
        {
            // Vars.var = variableToSet
        }
    }
}
