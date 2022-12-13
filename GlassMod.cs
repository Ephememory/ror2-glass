using UnityEngine;
using System.IO;
using System.Linq;
using BepInEx;
using R2API;
using On.RoR2;
using RoR2;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace GlassArtifactMod
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.jays.GlassMod", "GlassMod", "1.0.0")]
    public class GlassMod : BaseUnityPlugin
    {
        private const string configPath = "glassconfig.xml";

        private static float healthMultiplier = 0.10f;
        private static float damageMultiplier = 8f;
        private static float turretHealthMultiplier = 0.5f;
        private static float turretDamageMultiplier = 8f;
        private static float droneHealthMultiplier = 0.5f;
        private static float droneDamageMultiplier = 8f;
        private static bool modEnabled = false;
        public GlassMod()
        {
            var file = File.OpenText(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + configPath);
            if(file != null)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(file);
                var xmlNode = xmlDoc.SelectSingleNode("config");
                
                foreach (XmlNode node in xmlNode.ChildNodes)
                {
                    switch (node.Name)
                    {
                        case "ModEnabled":
                            modEnabled = bool.Parse(node.InnerText);
                            break;
                        case "HealthMultiplier":
                            healthMultiplier = float.Parse(node.InnerText);
                            break;
                        case "DamageMultiplier":
                            damageMultiplier = float.Parse(node.InnerText);
                            break;
                        case "TurretHealthMult":
                            turretHealthMultiplier = float.Parse(node.InnerText);
                            break;
                        case "TurretDamageMult":
                            turretDamageMultiplier = float.Parse(node.InnerText);
                            break;
                        case "DroneHealthMult":
                            droneHealthMultiplier = float.Parse(node.InnerText);
                            break;
                        case "DroneDamageMult":
                            droneDamageMultiplier = float.Parse(node.InnerText);
                            break;
                    }

                }
                
                
            }

            InitHooks();
        }
        private void InitHooks()
        {
            if (modEnabled)
            {
                On.RoR2.SurvivorCatalog.RegisterSurvivor += SurvivorCatalog_RegisterSurvivor;
                On.RoR2.BodyCatalog.SetBodyPrefabs += BodyCatalog_SetBodyPrefabs;
            }

        }

        private void BodyCatalog_SetBodyPrefabs(On.RoR2.BodyCatalog.orig_SetBodyPrefabs orig, UnityEngine.GameObject[] newBodyPrefabs)
        {
            
            foreach(var body in newBodyPrefabs)
            {
                var cb = body.GetComponent<RoR2.CharacterBody>();
                var cbName = cb.name.ToLower();
                if(cb == null)
                {
                    return;
                }

                if(cbName.Contains("turret"))
                {                    
                    cb.baseMaxHealth *= turretHealthMultiplier;
                    cb.levelMaxHealth *= turretHealthMultiplier;
                    cb.baseDamage *= turretDamageMultiplier;
                    cb.levelDamage *= turretDamageMultiplier;
                }else if(cbName.Contains("drone"))
                {
                    cb.baseMaxHealth *= droneHealthMultiplier;
                    cb.levelMaxHealth *= droneHealthMultiplier;
                    cb.baseDamage *= droneDamageMultiplier;
                    cb.levelDamage *= droneDamageMultiplier;
                }
            }
            orig(newBodyPrefabs);
        }

        private void SurvivorCatalog_RegisterSurvivor(On.RoR2.SurvivorCatalog.orig_RegisterSurvivor orig, SurvivorIndex survivorIndex, RoR2.SurvivorDef survivorDef)
        {
            var c = survivorDef.bodyPrefab.GetComponent<RoR2.CharacterBody>();
            c.baseMaxHealth *= healthMultiplier;
            c.levelMaxHealth *= healthMultiplier;
            c.baseDamage *= damageMultiplier;
            c.levelDamage *= damageMultiplier;
           
            orig(survivorIndex, survivorDef);          
        }

        public void Awake()
        {
            var enabledordisabled = modEnabled ? "<color=#00ff00>Enabled</color>" : "<color=#FF0000>Disabled</color>";
            RoR2.Chat.AddMessage("<style=cEvent><sprite name=\"Skull\" tint=1> Glass Mod is " + enabledordisabled + "<sprite name=\"Skull\" tint=1></color>");
        }


    }
}
