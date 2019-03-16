using System.IO;
using System.Reflection;
using Harmony;
using UnityEngine;
using System.Collections.Generic;
using ModSettings;

static class DetectionRange
{
    struct AnimalRanges
    {
        public bool enabled;  
        public float smell_range;

        public float hear_range;
        public float hear_range_while_feeding;

        public float detection_range;
        public float detection_range_while_feeding;
    };

    static Dictionary<string, AnimalRanges> m_animals = new Dictionary<string, AnimalRanges>();

    class WolfRangesSettings : ModSettingsBase
    {
        [Section("Wolf")]

        [Name("Enable override")]
        [Description("Default value is no")]
        public bool enabled = false;  

        [Name("Smell range")]
        [Description("Default value is 100 (as of February 09'th, 2019)")]
        [Slider(0f, 500f, 501)]
        public float smell_range = 100f;

        [Name("Hear range")]
        [Description("Default value is 75 (as of February 09'th, 2019)")]
        [Slider(0f, 500f, 501)]
        public float hear_range = 75f;
        [Name("Hear range while feeding")]
        [Description("Default value is 20 (as of February 09'th, 2019)")]
        [Slider(0f, 500f, 501)]
        public float hear_range_while_feeding = 20f;

        [Name("Detection range")]
        [Description("Default value is 60 (as of February 09'th, 2019)")]
        [Slider(0f, 500f, 501)]
        public float detection_range = 60f;
        [Name("Detection range while feeding")]
        [Description("Default value is 25 (as of February 09'th, 2019)")]
        [Slider(0f, 500f, 501)]
        public float detection_range_while_feeding = 25f;

        protected override void OnChange(FieldInfo field, object oldValue, object newValue)
        {
            if (field.Name == "enabled")
            {
                FieldInfo[] fields = GetType().GetFields();
                this.SetFieldVisible(fields[1], (bool)newValue);
                this.SetFieldVisible(fields[2], (bool)newValue);
                this.SetFieldVisible(fields[3], (bool)newValue);
                this.SetFieldVisible(fields[4], (bool)newValue);
                this.SetFieldVisible(fields[5], (bool)newValue);
            }
        }

        protected override void OnConfirm()
        {
            DetectionRange.m_animals["wolf"] = GetSettings();

            string settings = FastJson.Serialize(m_animals);
            File.WriteAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "DetectionRange.json"), settings);
        }

        public AnimalRanges GetSettings()
        {
            return new AnimalRanges
            {
                enabled = enabled,
                smell_range = smell_range,
                hear_range = hear_range,
                hear_range_while_feeding = hear_range_while_feeding,
                detection_range = detection_range,
                detection_range_while_feeding = detection_range_while_feeding
            };
        }

        public void SetSettings(AnimalRanges ranges)
        {
            enabled = ranges.enabled;
            smell_range = ranges.smell_range;
            hear_range = ranges.hear_range;
            hear_range_while_feeding = ranges.hear_range_while_feeding;
            detection_range = ranges.detection_range;
            detection_range_while_feeding = ranges.detection_range_while_feeding;
        }
    }

    class DeerRangesSettings : ModSettingsBase
    {
        [Section("Deer")]

        [Name("Enable override")]
        [Description("Default value is no")]
        public bool enabled = false;

        [Name("Smell range")]
        [Description("Default value is 0 (as of February 09'th, 2019)")]
        [Slider(0f, 500f, 501)]
        public float smell_range = 0f;

        [Name("Hear range")]
        [Description("Default value is 40 (as of February 09'th, 2019)")]
        [Slider(0f, 500f, 501)]
        public float hear_range = 40f;
        [Name("Hear range while feeding")]
        [Description("Default value is 30 (as of February 09'th, 2019)")]
        [Slider(0f, 500f, 501)]
        public float hear_range_while_feeding = 30f;

        [Name("Detection range")]
        [Description("Default value is 60 (as of February 09'th, 2019)")]
        [Slider(0f, 500f, 501)]
        public float detection_range = 60f;
        [Name("Detection range while feeding")]
        [Description("Default value is 20 (as of February 09'th, 2019)")]
        [Slider(0f, 500f, 501)]
        public float detection_range_while_feeding = 20f;

        protected override void OnChange(FieldInfo field, object oldValue, object newValue)
        {
            if (field.Name == "enabled")
            {
                FieldInfo[] fields = GetType().GetFields();
                this.SetFieldVisible(fields[1], (bool)newValue);
                this.SetFieldVisible(fields[2], (bool)newValue);
                this.SetFieldVisible(fields[3], (bool)newValue);
                this.SetFieldVisible(fields[4], (bool)newValue);
                this.SetFieldVisible(fields[5], (bool)newValue);
            }
        }

        protected override void OnConfirm()
        {
            DetectionRange.m_animals["deer"] = GetSettings();

            string settings = FastJson.Serialize(m_animals);
            File.WriteAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "DetectionRange.json"), settings);
        }

        public AnimalRanges GetSettings()
        {
            return new AnimalRanges
            {
                enabled = enabled,
                smell_range = smell_range,
                hear_range = hear_range,
                hear_range_while_feeding = hear_range_while_feeding,
                detection_range = detection_range,
                detection_range_while_feeding = detection_range_while_feeding
            };
        }

        public void SetSettings(AnimalRanges ranges)
        {
            enabled = ranges.enabled;
            smell_range = ranges.smell_range;
            hear_range = ranges.hear_range;
            hear_range_while_feeding = ranges.hear_range_while_feeding;
            detection_range = ranges.detection_range;
            detection_range_while_feeding = ranges.detection_range_while_feeding;
        }
    }

    class MooseRangesSettings : ModSettingsBase
    {
        [Section("Moose")]

        [Name("Enable override")]
        [Description("Default value is no")]
        public bool enabled = false;  

        [Name("Smell range")]
        [Description("Default value is 0 (as of February 09'th, 2019)")]
        [Slider(0f, 500f, 501)]
        public float smell_range = 0f;

        [Name("Hear range")]
        [Description("Default value is 50 (as of February 09'th, 2019)")]
        [Slider(0f, 500f, 501)]
        public float hear_range = 50f;
        [Name("Hear range while feeding")]
        [Description("Default value is 30 (as of February 09'th, 2019)")]
        [Slider(0f, 500f, 501)]
        public float hear_range_while_feeding = 30f;

        [Name("Detection range")]
        [Description("Default value is 60 (as of February 09'th, 2019)")]
        [Slider(0f, 500f, 501)]
        public float detection_range = 60f;
        [Name("Detection range while feeding")]
        [Description("Default value is 20 (as of February 09'th, 2019)")]
        [Slider(0f, 500f, 501)]
        public float detection_range_while_feeding = 20f;

        protected override void OnChange(FieldInfo field, object oldValue, object newValue)
        {
            if (field.Name == "enabled")
            {
                FieldInfo[] fields = GetType().GetFields();
                this.SetFieldVisible(fields[1], (bool)newValue);
                this.SetFieldVisible(fields[2], (bool)newValue);
                this.SetFieldVisible(fields[3], (bool)newValue);
                this.SetFieldVisible(fields[4], (bool)newValue);
                this.SetFieldVisible(fields[5], (bool)newValue);
            }
        }

        protected override void OnConfirm()
        {
            DetectionRange.m_animals["moose"] = GetSettings();

            string settings = FastJson.Serialize(m_animals);
            File.WriteAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "DetectionRange.json"), settings);
        }

        public AnimalRanges GetSettings()
        {
            return new AnimalRanges
            {
                enabled = enabled,
                smell_range = smell_range,
                hear_range = hear_range,
                hear_range_while_feeding = hear_range_while_feeding,
                detection_range = detection_range,
                detection_range_while_feeding = detection_range_while_feeding
            };
        }

        public void SetSettings(AnimalRanges ranges)
        {
            enabled = ranges.enabled;
            smell_range = ranges.smell_range;
            hear_range = ranges.hear_range;
            hear_range_while_feeding = ranges.hear_range_while_feeding;
            detection_range = ranges.detection_range;
            detection_range_while_feeding = ranges.detection_range_while_feeding;
        }
    }

    class RabbitRangesSettings : ModSettingsBase
    {
        [Section("Rabbit")]

        [Name("Enable override")]
        [Description("Default value is no")]
        public bool enabled = false;  

        [Name("Smell range")]
        [Description("Default value is 0 (as of February 09'th, 2019)")]
        [Slider(0f, 500f, 501)]
        public float smell_range = 0f;

        [Name("Hear range")]
        [Description("Default value is 12 (as of February 09'th, 2019)")]
        [Slider(0f, 500f, 501)]
        public float hear_range = 12f;
        [Name("Hear range while feeding")]
        [Description("Default value is 30 (as of February 09'th, 2019)")]
        [Slider(0f, 500f, 501)]
        public float hear_range_while_feeding = 30f;

        [Name("Detection range")]
        [Description("Default value is 7 (as of February 09'th, 2019)")]
        [Slider(0f, 500f, 501)]
        public float detection_range = 7f;
        [Name("Detection range while feeding")]
        [Description("Default value is 5 (as of February 09'th, 2019)")]
        [Slider(0f, 500f, 501)]
        public float detection_range_while_feeding = 5f;

        protected override void OnChange(FieldInfo field, object oldValue, object newValue)
        {
            if (field.Name == "enabled")
            {
                FieldInfo[] fields = GetType().GetFields();
                this.SetFieldVisible(fields[1], (bool)newValue);
                this.SetFieldVisible(fields[2], (bool)newValue);
                this.SetFieldVisible(fields[3], (bool)newValue);
                this.SetFieldVisible(fields[4], (bool)newValue);
                this.SetFieldVisible(fields[5], (bool)newValue);
            }
        }

        protected override void OnConfirm()
        {
            DetectionRange.m_animals["rabbit"] = GetSettings();

            string settings = FastJson.Serialize(m_animals);
            File.WriteAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "DetectionRange.json"), settings);
        }

        public AnimalRanges GetSettings()
        {
            return new AnimalRanges
            {
                enabled = enabled,
                smell_range = smell_range,
                hear_range = hear_range,
                hear_range_while_feeding = hear_range_while_feeding,
                detection_range = detection_range,
                detection_range_while_feeding = detection_range_while_feeding
            };
        }

        public void SetSettings(AnimalRanges ranges)
        {
            enabled = ranges.enabled;
            smell_range = ranges.smell_range;
            hear_range = ranges.hear_range;
            hear_range_while_feeding = ranges.hear_range_while_feeding;
            detection_range = ranges.detection_range;
            detection_range_while_feeding = ranges.detection_range_while_feeding;
        }
    }

    class BearRangesSettings : ModSettingsBase
    {
        [Section("Bear")]

        [Name("Enable override")]
        [Description("Default value is no")]
        public bool enabled = false;  

        [Name("Smell range")]
        [Description("Default value is 150 (as of February 09'th, 2019)")]
        [Slider(0f, 500f, 501)]
        public float smell_range = 150f;

        [Name("Hear range")]
        [Description("Default value is 60 (as of February 09'th, 2019)")]
        [Slider(0f, 500f, 501)]
        public float hear_range = 60f;
        [Name("Hear range while feeding")]
        [Description("Default value is 15 (as of February 09'th, 2019)")]
        [Slider(0f, 500f, 501)]
        public float hear_range_while_feeding = 15f;

        [Name("Detection range")]
        [Description("Default value is 60 (as of February 09'th, 2019)")]
        [Slider(0f, 500f, 501)]
        public float detection_range = 60f;
        [Name("Detection range while feeding")]
        [Description("Default value is 25 (as of February 09'th, 2019)")]
        [Slider(0f, 500f, 501)]
        public float detection_range_while_feeding = 25f;

        protected override void OnChange(FieldInfo field, object oldValue, object newValue)
        {
            if (field.Name == "enabled")
            {
                FieldInfo[] fields = GetType().GetFields();
                this.SetFieldVisible(fields[1], (bool)newValue);
                this.SetFieldVisible(fields[2], (bool)newValue);
                this.SetFieldVisible(fields[3], (bool)newValue);
                this.SetFieldVisible(fields[4], (bool)newValue);
                this.SetFieldVisible(fields[5], (bool)newValue);
            }
        }

        protected override void OnConfirm()
        {
            DetectionRange.m_animals["bear"] = GetSettings();

            string settings = FastJson.Serialize(m_animals);
            File.WriteAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "DetectionRange.json"), settings);
        }

        public AnimalRanges GetSettings()
        {
            return new AnimalRanges
            {
                enabled = enabled,
                smell_range = smell_range,
                hear_range = hear_range,
                hear_range_while_feeding = hear_range_while_feeding,
                detection_range = detection_range,
                detection_range_while_feeding = detection_range_while_feeding
            };
        }

        public void SetSettings(AnimalRanges ranges)
        {
            enabled = ranges.enabled;
            smell_range = ranges.smell_range;
            hear_range = ranges.hear_range;
            hear_range_while_feeding = ranges.hear_range_while_feeding;
            detection_range = ranges.detection_range;
            detection_range_while_feeding = ranges.detection_range_while_feeding;
        }
    }

    static public void OnLoad()
    {
        Debug.LogFormat("DetectionRange: init");

        string opts = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "DetectionRange.json"));
        Dictionary<string, AnimalRanges> settings = FastJson.Deserialize<Dictionary<string, AnimalRanges>>(opts);

        WolfRangesSettings wolves = new WolfRangesSettings();
        if (!settings.ContainsKey("wolf"))
        {
            settings["wolf"] = GetEmptyStructure();
        }
        wolves.SetSettings(settings["wolf"]);
        wolves.AddToModSettings("Detection Range");
        m_animals.Add("wolf", wolves.GetSettings());

        DeerRangesSettings deers = new DeerRangesSettings();
        if (!settings.ContainsKey("deer"))
        {
            settings["deer"] = GetEmptyStructure();
        }
        deers.SetSettings(settings["deer"]);
        deers.AddToModSettings("Detection Range");
        m_animals.Add("deer", deers.GetSettings());

        MooseRangesSettings mooses = new MooseRangesSettings();
        if (!settings.ContainsKey("moose"))
        {
            settings["moose"] = GetEmptyStructure();
        }
        mooses.SetSettings(settings["moose"]);
        mooses.AddToModSettings("Detection Range");
        m_animals.Add("moose", mooses.GetSettings());

        RabbitRangesSettings rabbits = new RabbitRangesSettings();
        if (!settings.ContainsKey("rabbit"))
        {
            settings["rabbit"] = GetEmptyStructure();
        }
        rabbits.SetSettings(settings["rabbit"]);
        rabbits.AddToModSettings("Detection Range");
        m_animals.Add("rabbit", rabbits.GetSettings());

        BearRangesSettings bears = new BearRangesSettings();
        if (!settings.ContainsKey("bear"))
        {
            settings["bear"] = GetEmptyStructure();
        }
        bears.SetSettings(settings["bear"]);
        bears.AddToModSettings("Detection Range");
        m_animals.Add("bear", bears.GetSettings());
    }

    static AnimalRanges GetEmptyStructure()
    {
        return new AnimalRanges
        {
            enabled = false,
            smell_range = 0f,
            hear_range = 0f,
            hear_range_while_feeding = 0f,
            detection_range = 0f,
            detection_range_while_feeding = 0f
        };
    }

    [HarmonyPatch(typeof(BaseAi), "DoCustomModeModifiers")]
    public class DetectionRangeModify
    {
        public static void Postfix(BaseAi __instance)
        {
            switch (__instance.m_AiSubType)
            {
                case AiSubType.Wolf:
                    AdjustRanges(__instance, "wolf");
                    break;
                case AiSubType.Stag:
                    AdjustRanges(__instance, "deer");
                    break;
                case AiSubType.Moose:
                    AdjustRanges(__instance, "moose");
                    break;
                case AiSubType.Rabbit:
                    AdjustRanges(__instance, "rabbit");
                    break;
                case AiSubType.Bear:
                    AdjustRanges(__instance, "bear");
                    break;
            }

        }
    }

    static void AdjustRanges(BaseAi inst, string animal)
    {
        // Debug.LogFormat("{0} {1} {2} {3} {4} {5}", animal, inst.m_SmellRange, inst.m_DetectionRange, inst.m_DetectionRangeWhileFeeding, inst.m_HearFootstepsRange, inst.m_HearFootstepsRangeWhileFeeding);
        if (m_animals[animal].enabled)
        {
            if (m_animals[animal].hear_range_while_feeding >= 0f)
                inst.m_HearFootstepsRangeWhileFeeding = m_animals[animal].hear_range_while_feeding;

            if (m_animals[animal].hear_range >= 0f)
                inst.m_HearFootstepsRange = m_animals[animal].hear_range;

            if (m_animals[animal].detection_range_while_feeding >= 0f)
                inst.m_DetectionRangeWhileFeeding = m_animals[animal].detection_range_while_feeding;

            if (m_animals[animal].detection_range >= 0f)
                inst.m_DetectionRange = m_animals[animal].detection_range;

            if (m_animals[animal].smell_range >= 0f)
                inst.m_SmellRange = m_animals[animal].smell_range;
        }
    }
}
