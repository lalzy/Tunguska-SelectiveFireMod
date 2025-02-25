using System;
using System.IO;
using System.Collections.Generic;
using UnityModManagerNet;
using System.Xml.Serialization;

namespace FireSelection
{

    [Serializable]
    public class WeaponStateDictionary
    {
        [XmlElement("WeaponState")]
        public List<WeaponStateEntry> Entries { get; set; } = new List<WeaponStateEntry>();
    }

    [Serializable]
    public class WeaponStateEntry
    {
        [XmlAttribute("WeaponID")]
        public string WeaponID { get; set; }

        [XmlAttribute("GunFireMode")]
        public GunFireModes FireMode { get; set; }
    }
    internal class Save
    {
        public static void SaveWeaponStates(string filePath, Dictionary<string, GunFireModes> weaponStatesOriginal)
        {
            WeaponStateDictionary weaponStateDictionary = new WeaponStateDictionary();

            foreach (var entry in weaponStatesOriginal)
            {
                weaponStateDictionary.Entries.Add(new WeaponStateEntry { WeaponID = entry.Key, FireMode = entry.Value });
            }

            XmlSerializer serializer = new XmlSerializer(typeof(WeaponStateDictionary));

            using (FileStream stream = new FileStream(filePath, FileMode.Create))
            {
                serializer.Serialize(stream, weaponStateDictionary);
            }
        }
        public static Dictionary<string, GunFireModes> LoadWeaponStates(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(WeaponStateDictionary));

            using (FileStream stream = new FileStream(filePath, FileMode.Open))
            {
                WeaponStateDictionary weaponStateDictionary = (WeaponStateDictionary)serializer.Deserialize(stream);
                Dictionary<string, GunFireModes> weaponStates = new Dictionary<string, GunFireModes>();

                foreach (var entry in weaponStateDictionary.Entries)
                {
                    weaponStates[entry.WeaponID] = entry.FireMode;
                }

                return weaponStates;
            }
        }
    }
}
