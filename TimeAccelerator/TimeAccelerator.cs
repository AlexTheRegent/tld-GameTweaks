using System;
using System.IO;
using System.Reflection;
using Harmony;
using UnityEngine;
using ModSettings;

static class TimeAccelerator
{
    static KeyCode accelerationKey = KeyCode.V;
    static float accelerationValue = 3.0f;
    static bool holdToAccelerate = false;
    static bool accelerated = false;

    internal class TimeAcceleratorSettings : ModSettingsBase
    {
        [Name("Acceleration key")]
        [Choice("A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z")]
        public int accelerationKey = 2;

        [Name("Acceleration behaviour")]
        [Choice("Hold to accelerate", "Toggle on press")]
        public int accelerationBehaviour = 0;

        [Name("Acceleration speed")]
        [Description("1 = 100%, 2 = 200%")]
        [Slider(0f, 10f, 101)]
        public float accelerationValue = 3f;

        protected override void OnConfirm()
        {
            TimeAccelerator.accelerationKey = KeyCode.A + accelerationKey;
            TimeAccelerator.holdToAccelerate = accelerationBehaviour==0;
            TimeAccelerator.accelerationValue = accelerationValue;

            string settings = FastJson.Serialize(this);
            File.WriteAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "TimeAccelerator.json"), settings);
        }
    }

    static public void OnLoad()
    {
        Debug.LogFormat("TimeAccelerator: init");

        TimeAcceleratorSettings settings = new TimeAcceleratorSettings();
        string opts = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "TimeAccelerator.json"));
        settings = FastJson.Deserialize<TimeAcceleratorSettings>(opts);
        settings.AddToModSettings("Time Accelerator");
        
        accelerationKey = KeyCode.A + settings.accelerationKey;
        holdToAccelerate = settings.accelerationBehaviour == 0;
        accelerationValue = settings.accelerationValue;
    }

    [HarmonyPatch(typeof(InterfaceManager), "Update")]
    public class TimeAcceleratorUpdate
    {
        public static void Prefix(HUDManager __instance)
        {
            if (holdToAccelerate)
            {
                if (Input.GetKey(accelerationKey))
                {
                    if (Time.timeScale < accelerationValue)
                    {
                        Time.timeScale = accelerationValue;
                    }
                }
                else if (Input.GetKeyUp(accelerationKey))
                {
                    Time.timeScale = 1.0f;
                }
            }
            else
            {
                if (Input.GetKeyDown(accelerationKey))
                {
                    accelerated = !accelerated;
                }

                if (accelerated)
                {
                    if (Time.timeScale < accelerationValue)
                    {
                        Time.timeScale = accelerationValue;
                    }
                }
                else
                {
                    Time.timeScale = 1.0f;
                }
            }
        }
    }
}
