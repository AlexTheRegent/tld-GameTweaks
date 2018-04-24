using System.Xml;
using System.IO;
using System.Reflection;
using Harmony;
using UnityEngine;

static class DetectionRange
{
    private enum AnimalType
    {
        WOLF = 0,
        DEER = 1,
        RABBIT = 2,
        BEAR = 3,
        MOOSE = 4,
        TOTAL = 5,
    }

    // Configuration 
    static string configFileName = "DetectionRange.xml";
    struct xml_parameters
    {
        public float hear_range;
        public float hear_range_while_feeding;
        public float smell_range;
        public float detection_range;
        public float detection_range_while_feeding;
    }

    static xml_parameters[] config = new xml_parameters[(int)AnimalType.TOTAL];

    static public void OnLoad()
    {
        Debug.LogFormat("DetectionRange: init");

        string modsDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string configPath = Path.Combine(modsDir, configFileName);

        XmlDocument xml = new XmlDocument();
        xml.Load(configPath);

        parse_xml_node(xml.DocumentElement.SelectNodes("/config/rabbits")[0], (int)AnimalType.RABBIT);
        parse_xml_node(xml.DocumentElement.SelectNodes("/config/wolves")[0], (int)AnimalType.WOLF);
        parse_xml_node(xml.DocumentElement.SelectNodes("/config/mooses")[0], (int)AnimalType.MOOSE);
        parse_xml_node(xml.DocumentElement.SelectNodes("/config/deers")[0], (int)AnimalType.DEER);
        parse_xml_node(xml.DocumentElement.SelectNodes("/config/bears")[0], (int)AnimalType.BEAR);
    }

    static void parse_xml_node(XmlNode node, int idx)
    {
        if (!GetNodeFloat(node.SelectSingleNode("hear_range"), out config[idx].hear_range))
        {
            Debug.LogFormat("DetectionRange: missing 'hear_range' entry for '{0}' section", get_animal_name(idx));
            config[idx].hear_range = -1f;
        }
        if (!GetNodeFloat(node.SelectSingleNode("hear_range_while_feeding"), out config[idx].hear_range_while_feeding))
        {
            Debug.LogFormat("DetectionRange: missing 'hear_range_while_feeding' entry for '{0}' section", get_animal_name(idx));
            config[idx].hear_range_while_feeding = -1f;
        }
        if (!GetNodeFloat(node.SelectSingleNode("smell_range"), out config[idx].smell_range))
        {
            Debug.LogFormat("DetectionRange: missing 'smell_range' entry for '{0}' section", get_animal_name(idx));
            config[idx].smell_range = -1f;
        }
        if (!GetNodeFloat(node.SelectSingleNode("detection_range"), out config[idx].detection_range))
        {
            Debug.LogFormat("DetectionRange: missing 'detection_range' entry for '{0}' section", get_animal_name(idx));
            config[idx].detection_range = -1f;
        }
        if (!GetNodeFloat(node.SelectSingleNode("detection_range_while_feeding"), out config[idx].detection_range_while_feeding))
        {
            Debug.LogFormat("DetectionRange: missing 'detection_range_while_feeding' entry for '{0}' section", get_animal_name(idx));
            config[idx].detection_range_while_feeding = -1f;
        }
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

    static string get_animal_name(int idx)
    {
        switch (idx)
        {
            case (int)AnimalType.WOLF:
                return "wolves";
            case (int)AnimalType.RABBIT:
                return "rabbits";
            case (int)AnimalType.MOOSE:
                return "mooses";
            case (int)AnimalType.BEAR:
                return "bears";
            case (int)AnimalType.DEER:
                return "deers";
        }

        return "unknown section";
    }

    // Modify values 
    [HarmonyPatch(typeof(BaseAi), "DoCustomModeModifiers")]
    public class DetectionRangeModify
    {
        public static void Postfix(BaseAi __instance)
        {
            switch (__instance.m_AiSubType)
            {
                case AiSubType.Wolf:
                    adjust_ranges(__instance, (int)AnimalType.WOLF);
                    break;
                case AiSubType.Stag:
                    adjust_ranges(__instance, (int)AnimalType.DEER);
                    break;
                case AiSubType.Moose:
                    adjust_ranges(__instance, (int)AnimalType.MOOSE);
                    break;
                case AiSubType.Rabbit:
                    adjust_ranges(__instance, (int)AnimalType.RABBIT);
                    break;
                case AiSubType.Bear:
                    adjust_ranges(__instance, (int)AnimalType.BEAR);
                    break;
            }
            
        }
    }

    static void adjust_ranges(BaseAi inst, int idx)
    {
        if (config[idx].hear_range_while_feeding >= 0f)
            inst.m_HearFootstepsRangeWhileFeeding = config[idx].hear_range_while_feeding;

        if (config[idx].hear_range >= 0f)
            inst.m_HearFootstepsRange = config[idx].hear_range;

        if (config[idx].detection_range_while_feeding >= 0f)
            inst.m_DetectionRangeWhileFeeding = config[idx].detection_range_while_feeding;

        if (config[idx].detection_range >= 0f)
            inst.m_DetectionRange = config[idx].detection_range;

        if (config[idx].smell_range >= 0f)
            inst.m_SmellRange = config[idx].smell_range;
    }
}
