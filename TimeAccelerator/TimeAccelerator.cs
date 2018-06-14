using System;
using System.Xml;
using System.IO;
using System.Reflection;
using Harmony;
using UnityEngine;

static class TimeAccelerator
{
    static string configFileName = "TimeAccelerator.xml";
    static string accelerationKey = "V";
    static float accelerationValue = 3.0f;
    static bool holdToAccelerate = false;
    static bool accelerated = false;
    static KeyCode accelerationKeyCode = KeyCode.V;

    static public void OnLoad()
    {
        Debug.LogFormat("TimeAccelerator: init");

        string modsDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string configPath = Path.Combine(modsDir, configFileName);

        XmlDocument xml = new XmlDocument();
        xml.Load(configPath);

        if (!GetNodeFloat(xml.SelectSingleNode("/config/acceleration_value"), out accelerationValue))
        {
            Debug.LogFormat("TimeAccelerator: missing/invalid 'acceleration_value' entry");
            accelerationValue = 5.0f;
        }

        if (!GetNodeString(xml.SelectSingleNode("/config/acceleration_key"), out accelerationKey))
        {
            Debug.LogFormat("TimeAccelerator: missing/invalid 'acceleration_key' entry");
            accelerationKey = "E";
        }

        accelerationKeyCode = (KeyCode)Enum.Parse(typeof(KeyCode), accelerationKey);
        if (!Enum.IsDefined(typeof(KeyCode), accelerationKeyCode) && !accelerationKeyCode.ToString().Contains(","))
        {
            Debug.LogFormat("TimeAccelerator: invalid key for 'acceleration_key' entry");
            accelerationKeyCode = KeyCode.V;
        }

        if (!GetNodeBool(xml.SelectSingleNode("/config/hold_to_accelerate"), out holdToAccelerate))
        {
            Debug.LogFormat("TimeAccelerator: missing/invalid 'hold_to_accelerate' entry");
            holdToAccelerate = true;
        }
    }

    static private bool GetNodeString(XmlNode node, out string value)
    {
        if (node == null || node.Attributes["value"] == null)
        {
            value = "V";
            return false;
        }

        value = node.Attributes["value"].Value;
        return true;
    }

    static private bool GetNodeFloat(XmlNode node, out float value)
    {
        if (node == null || node.Attributes["value"] == null || !float.TryParse(node.Attributes["value"].Value, out value))
        {
            value = -1f;
            return false;
        }

        return true;
    }
    static private bool GetNodeBool(XmlNode node, out bool value)
    {
        if (node == null || node.Attributes["value"] == null || !bool.TryParse(node.Attributes["value"].Value, out value))
        {
            value = false;
            return false;
        }

        return true;
    }

    [HarmonyPatch(typeof(InterfaceManager), "Update")]
    public class TimeAcceleratorUpdate
    {
        public static void Prefix(HUDManager __instance)
        {
            if (holdToAccelerate)
            {
                if (Input.GetKey(accelerationKeyCode))
                {
                    if (Time.timeScale < accelerationValue)
                    {
                        Time.timeScale = accelerationValue;
                    }
                }
                else if (Input.GetKeyUp(accelerationKeyCode))
                {
                    Time.timeScale = 1.0f;
                }
            }
            else
            {
                if (Input.GetKeyDown(accelerationKeyCode))
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
