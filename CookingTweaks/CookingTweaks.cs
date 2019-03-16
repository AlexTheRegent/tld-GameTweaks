using System;
using System.Xml;
using System.IO;
using System.Reflection;
using Harmony;
using UnityEngine;
using ModSettings;
using System.Collections.Generic;
using System.Linq;

static class CookingTweaks
{
    internal struct BoilingTime
    {
        public bool enabled;
        public float melting;
        public float boiling;
        public float drying;
    }

    internal static BoilingTime m_water;
    internal static Dictionary<string, CookingTime> m_food = new Dictionary<string, CookingTime>();

    internal struct SaveDataProxy
    {
        public BoilingTime water;
        public Dictionary<string, CookingTime> food;
    }

    internal class WaterSettings : ModSettingsBase
    {
        [Section("Water")]

        [Name("Enable override")]
        public bool enabled = false;

        [Name("Ice melting time")]
        [Description("ingame minutes, per 1 unit")]
        [Slider(0f, 600f, 601)]
        public float melting = 0f;

        [Name("Water boiling time")]
        [Description("ingame minutes, per 1 unit")]
        [Slider(0f, 600f, 601)]
        public float boiling = 0f;

        [Name("Water drying time")]
        [Description("ingame minutes, per 1 unit")]
        [Slider(0f, 600f, 601)]
        public float drying = 0f;

        protected override void OnChange(FieldInfo field, object oldValue, object newValue)
        {
            if (field.Name == "enabled")
            {
                FieldInfo[] fields = GetType().GetFields();
                this.SetFieldVisible(fields[1], (bool)newValue);
                this.SetFieldVisible(fields[2], (bool)newValue);
                this.SetFieldVisible(fields[3], (bool)newValue);
            }
        }

        protected override void OnConfirm()
        {
            m_water.enabled = enabled;
            m_water.melting = melting;
            m_water.boiling = boiling;
            m_water.drying = drying;

            SaveDataProxy data = new SaveDataProxy
            {
                water = m_water,
                food = m_food
            };

            string settings = FastJson.Serialize(data);
            File.WriteAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "CookingTweaks.json"), settings);
        }
    }

    internal struct CookingTime
    {
        public string name;
        public bool enabled;
        public float cooking;
        public float burning;
    }

    internal class FoodSettings : ModSettingsBase
    {
        [Section("Food")]

        [Name("Edit")]
        public string foodType;

        [Name("Enable override")]
        public bool enabled = false;

        [Name("Cooking time")]
        [Description("ingame minutes, per 1 unit")]
        [Slider(0f, 600f, 601)]
        public float cooking = 0f;

        [Name("Burning time")]
        [Description("ingame minutes, per 1 unit")]
        [Slider(0f, 600f, 601)]
        public float burning = 0f;

        internal string lastFood;
        internal static Dictionary<string, CookingTime> m_changes = new Dictionary<string, CookingTime>();

        public FoodSettings(string foodName)
        {
            lastFood = foodName;
        }

        protected override void OnChange(FieldInfo field, object oldValue, object newValue)
        {
            if (field.Name == "foodType")
            {
                CookingTime foodData;
                foodType = (string)newValue;
                if (m_changes.ContainsKey(foodType))
                {
                    foodData = m_changes[foodType];
                }
                else if (m_food.ContainsKey(foodType))
                {
                    foodData = m_food[foodType];
                }
                else
                {
                    foodData = GetEmptyStructure();
                }

                m_changes[lastFood] = new CookingTime
                {
                    name = "",
                    enabled = enabled,
                    cooking = cooking,
                    burning = burning,
                };

                enabled = foodData.enabled;
                cooking = foodData.cooking;
                burning = foodData.burning;
                lastFood = foodType;
                RefreshGUI();
            }
            else if (field.Name == "enabled")
            {
                FieldInfo[] fields = GetType().GetFields();
                this.SetFieldVisible(fields[2], (bool)newValue);
                this.SetFieldVisible(fields[3], (bool)newValue);
            }
        }
        protected override void OnConfirm()
        {
            string food = foodType.ToString();
            m_changes[food] = new CookingTime
            {
                name = "",
                enabled = enabled,
                cooking = cooking,
                burning = burning,
            };
            m_changes.ToList().ForEach(delegate(KeyValuePair<string, CookingTime> x)
            {
                m_food[x.Key] = new CookingTime
                {
                    name = m_food[x.Key].name,
                    enabled = x.Value.enabled,
                    cooking = x.Value.cooking,
                    burning = x.Value.burning,
                }; ;
            });
            SaveDataProxy data = new SaveDataProxy
            {
                water = m_water,
                food = m_food
            };

            string settings = FastJson.Serialize(data);
            File.WriteAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "CookingTweaks.json"), settings);
        }
    }

    static public void OnLoad()
    {
        Debug.LogFormat("CookingTweaks: init");

        string settings = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "CookingTweaks.json"));
        SaveDataProxy data = FastJson.Deserialize<SaveDataProxy>(settings);

        WaterSettings waterSettings = new WaterSettings();
        FoodSettings foodSettings = new FoodSettings(data.food.ElementAt(0).Key);

        // m_water = data.water;
        // m_food = data.food;
        // 
        // waterSettings.enabled = m_water.enabled;
        // waterSettings.melting = m_water.melting;
        // waterSettings.boiling = m_water.boiling;
        // waterSettings.drying = m_water.drying;
        // 
        // string firstFood = ((FoodTypes)0).ToString();
        // foodSettings.foodType = 0;
        // if (!m_food.ContainsKey(firstFood))
        // {
        //     m_food[((FoodTypes)0).ToString()] = GetEmptyStructure();
        // }
        // 
        // foodSettings.enabled = m_food[firstFood].enabled;
        // foodSettings.cooking = m_food[firstFood].cooking;
        // foodSettings.burning = m_food[firstFood].burning;
        // 
        // waterSettings.AddToModSettings("Cooking Tweaks");
        // foodSettings.AddToModSettings("Cooking Tweaks");
    }

    static CookingTime GetEmptyStructure()
    {
        return new CookingTime
        {
            name = "",
            enabled = false,
            cooking = 0f,
            burning = 0f,
        };
    }
}