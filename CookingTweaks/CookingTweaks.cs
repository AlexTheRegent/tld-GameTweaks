using System;
using System.Xml;
using System.IO;
using System.Reflection;
using Harmony;
using UnityEngine;

static class CookingTweaks
{

    private enum FoodType
    {
        BEAR,
        RABBIT,
        SALMON,
        WHITEFISH,
        RAINBOWTROUT,
        BASS,
        DEER,
        WOLF,
        PEACHES,
        BEANS,
        TOMATOSOUP,
        REISHITEA,
        ROSETEA,
        COFFEE,
        TOTAL,
    }

    struct xml_parameters
    {
        public float cookingTime;
        public float readyTime;
    }

    static string configFileName = "CookingTweaks.xml";
    static xml_parameters[] config = new xml_parameters[(int)FoodType.TOTAL];

    static float meltingTime;
    static float boilingTime;
    static float dryingTime;

    static bool multiply_weigth;

    static public void OnLoad()
    {
        Debug.LogFormat("CookingTweaks: init");

        string modsDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string configPath = Path.Combine(modsDir, configFileName);

        XmlDocument xml = new XmlDocument();
        xml.Load(configPath);

        parse_xml_node(xml.DocumentElement.SelectNodes("/config/bear")[0], (int)FoodType.BEAR);
        parse_xml_node(xml.DocumentElement.SelectNodes("/config/rabbit")[0], (int)FoodType.RABBIT);
        parse_xml_node(xml.DocumentElement.SelectNodes("/config/salmon")[0], (int)FoodType.SALMON);
        parse_xml_node(xml.DocumentElement.SelectNodes("/config/whitefish")[0], (int)FoodType.WHITEFISH);
        parse_xml_node(xml.DocumentElement.SelectNodes("/config/rainbowtrout")[0], (int)FoodType.RAINBOWTROUT);
        parse_xml_node(xml.DocumentElement.SelectNodes("/config/bass")[0], (int)FoodType.BASS);
        parse_xml_node(xml.DocumentElement.SelectNodes("/config/deer")[0], (int)FoodType.DEER);
        parse_xml_node(xml.DocumentElement.SelectNodes("/config/wolf")[0], (int)FoodType.WOLF);
        parse_xml_node(xml.DocumentElement.SelectNodes("/config/peaches")[0], (int)FoodType.PEACHES);
        parse_xml_node(xml.DocumentElement.SelectNodes("/config/beans")[0], (int)FoodType.BEANS);
        parse_xml_node(xml.DocumentElement.SelectNodes("/config/tomatosoup")[0], (int)FoodType.TOMATOSOUP);
        parse_xml_node(xml.DocumentElement.SelectNodes("/config/reishitea")[0], (int)FoodType.REISHITEA);
        parse_xml_node(xml.DocumentElement.SelectNodes("/config/rosetea")[0], (int)FoodType.ROSETEA);
        parse_xml_node(xml.DocumentElement.SelectNodes("/config/coffee")[0], (int)FoodType.COFFEE);

        if (!GetNodeFloat(xml.SelectSingleNode("/config/water/melting"), out meltingTime))
        {
            Debug.LogFormat("CookingTweaks: missing/invalid 'melting' entry");
            meltingTime = -1f;
        }
        if (!GetNodeFloat(xml.SelectSingleNode("/config/water/boiling"), out boilingTime))
        {
            Debug.LogFormat("CookingTweaks: missing/invalid 'boiling' entry");
            boilingTime = -1f;
        }
        if (!GetNodeFloat(xml.SelectSingleNode("/config/water/drying"), out dryingTime))
        {
            Debug.LogFormat("CookingTweaks: missing/invalid 'drying' entry");
            dryingTime = -1f;
        }
        if (!GetNodeBool(xml.SelectSingleNode("/config/multiply_weight"), out multiply_weigth))
        {
            Debug.LogFormat("CookingTweaks: missing/invalid 'multiply_weight' entry");
            multiply_weigth = false;
        }
    }

    static void parse_xml_node(XmlNode node, int idx)
    {
        if (!GetNodeFloat(node.SelectSingleNode("cooking_time"), out config[idx].cookingTime))
        {
            Debug.LogFormat("CookingTweaks: missing 'cooking_time' entry for '{0}' section", GetFoodName(idx));
            config[idx].cookingTime = -1f;
        }
        if (!GetNodeFloat(node.SelectSingleNode("ready_time"), out config[idx].readyTime))
        {
            Debug.LogFormat("CookingTweaks: missing 'ready_time' entry for '{0}' section", GetFoodName(idx));
            config[idx].readyTime = -1f;
        }
    }

    static private string GetFoodName(int idx)
    {
        switch (idx)
        {
            case (int)FoodType.BEAR:
                return "bear";
            case (int)FoodType.RABBIT:
                return "rabbit";
            case (int)FoodType.SALMON:
                return "salmon";
            case (int)FoodType.WHITEFISH:
                return "whitefish";
            case (int)FoodType.RAINBOWTROUT:
                return "rainbowtrout";
            case (int)FoodType.BASS:
                return "bass";
            case (int)FoodType.DEER:
                return "deer";
            case (int)FoodType.WOLF:
                return "wolf";
            case (int)FoodType.PEACHES:
                return "peaches";
            case (int)FoodType.BEANS:
                return "beans";
            case (int)FoodType.TOMATOSOUP:
                return "tomatosoup";
            case (int)FoodType.REISHITEA:
                return "reishitea";
            case (int)FoodType.ROSETEA:
                return "rosetea";
            case (int)FoodType.COFFEE:
                return "coffee";
        }

        return string.Empty;
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

    [HarmonyPatch(typeof(CookingPotItem), "StartCooking")]
    public class CookingTweaksCookingFood
    { 
        public static void Prefix(CookingPotItem __instance, GearItem gearItemToCook)
        {
            Debug.LogFormat("CookingTweaks: item id: \"{0}\", cooking time: \"{1}\", burning time: \"{2}\"", gearItemToCook.m_LocalizedDisplayName.m_LocalizationID, gearItemToCook.m_Cookable.m_CookTimeMinutes, gearItemToCook.m_Cookable.m_ReadyTimeMinutes);
            switch (gearItemToCook.m_LocalizedDisplayName.m_LocalizationID)
            {
                case "GAMEPLAY_BearMeatRaw":
                    UpdateValues(gearItemToCook, config[(int)FoodType.BEAR], multiply_weigth);
                    break;
                case "GAMEPLAY_RabbitRaw":
                    UpdateValues(gearItemToCook, config[(int)FoodType.RABBIT], multiply_weigth);
                    break;
                case "GAMEPLAY_RawCohoSalmon":
                    UpdateValues(gearItemToCook, config[(int)FoodType.SALMON], multiply_weigth);
                    break;
                case "GAMEPLAY_RawLakeWhiteFish":
                    UpdateValues(gearItemToCook, config[(int)FoodType.WHITEFISH], multiply_weigth);
                    break;
                case "GAMEPLAY_RawRainbowTrout":
                    UpdateValues(gearItemToCook, config[(int)FoodType.RAINBOWTROUT], multiply_weigth);
                    break;
                case "GAMEPLAY_RawSmallMouthBass":
                    UpdateValues(gearItemToCook, config[(int)FoodType.BASS], multiply_weigth);
                    break;
                case "GAMEPLAY_VenisonRaw":
                    UpdateValues(gearItemToCook, config[(int)FoodType.DEER], multiply_weigth);
                    break;
                case "GAMEPLAY_WolfMeatRaw":
                    UpdateValues(gearItemToCook, config[(int)FoodType.WOLF], multiply_weigth);
                    break;
                case "GAMEPLAY_PinnaclePeaches":
                    UpdateValues(gearItemToCook, config[(int)FoodType.PEACHES]);
                    break;
                case "GAMEPLAY_PorkandBeans":
                    UpdateValues(gearItemToCook, config[(int)FoodType.BEANS]);
                    break;
                case "GAMEPLAY_TomatoSoupCan":
                    UpdateValues(gearItemToCook, config[(int)FoodType.TOMATOSOUP]);
                    break;
                case "GAMEPLAY_ReishiTea":
                    UpdateValues(gearItemToCook, config[(int)FoodType.REISHITEA]);
                    break;
                case "GAMEPLAY_RoseHipTea":
                    UpdateValues(gearItemToCook, config[(int)FoodType.ROSETEA]);
                    break;
                case "GAMEPLAY_CoffeeCup":
                    UpdateValues(gearItemToCook, config[(int)FoodType.COFFEE]);
                    break;
            }
        }
    }

    static private void UpdateValues(GearItem gearItem, xml_parameters settings, bool weigth = false)
    {
        if (settings.cookingTime >= 0f)
        {
            gearItem.m_Cookable.m_CookTimeMinutes = settings.cookingTime * (weigth ? gearItem.GetItemWeightKG() / 1f : 1f);
        }
        if (settings.readyTime >= 0f)
        {
            gearItem.m_Cookable.m_ReadyTimeMinutes = settings.readyTime;
        }
    }

    [HarmonyPatch(typeof(CookingPotItem), "StartMeltingSnow")]
    public class CookingTweaksMeltingSnow
    {
        public static void Postfix()
        {
            if (meltingTime >= 0f)
            {
                InterfaceManager.m_Panel_Cooking.m_MinutesToMeltSnowPerLiter = meltingTime;
            }
        }
    }

    [HarmonyPatch(typeof(CookingPotItem), "StartBoilingWater")]
    public class CookingTweaksBoilingWater
    {
        public static void Postfix()
        {
            if (boilingTime >= 0f)
            {
                InterfaceManager.m_Panel_Cooking.m_MinutesToBoilWaterPerLiter = boilingTime;
            }
            if (dryingTime >= 0f)
            {
                InterfaceManager.m_Panel_Cooking.m_MinutesReadyTimeBoilingWater = dryingTime;
            }
        }
    }
}