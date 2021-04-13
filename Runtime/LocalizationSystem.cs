﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace personaltools.textlocalizedtool
{
    [Serializable]
    public class LocalizationSystem
    {
        [SerializeField]
        private static Languages language = Languages.English;
        public static Languages Language
        {
            get => language;
            set
            {
                language = value;
                Init();
            }
        }

        [SerializeField]
        private static Languages defaultLanguage = Languages.English;
        public static Languages DefaultLanguage
        {
            get => defaultLanguage;
            set
            {
                defaultLanguage = value;
                Init();
            }
        }

        private static Dictionary<string, string> defaultlocalised;
        private static Dictionary<string, string> localised;

        private static bool isInit;

        public static CSVLoader csvLoader;
        private static TextAsset assetCSV;
        public static TextAsset AssetCSV { 
            get 
            {
                return assetCSV;
            } 
            set 
            {
                if(value != null)
                {
                    assetCSV = value;
                    Init();
                }
            } 
        }

        public static void Init()
        {
            Debug.Assert(AssetCSV != null, "Please Insert Localised File in Localization Manager");

            csvLoader = csvLoader ?? new CSVLoader();
            csvLoader.LoadCSV(AssetCSV);
            UpdateDictionaries();

            isInit = true;
        }

        public static List<string> GetLanguageNames()
        {
            List<string> languagesNames = new List<string>();

            foreach (string language in Enum.GetNames(typeof(Languages)))
            {
                languagesNames.Add(language);
            }
            return languagesNames;
        }

        public static Languages GetEnumLanguage(string name)
        {
            foreach (Languages lan in Enum.GetValues(typeof(Languages)))
            {
                if (lan.ToString() == name)
                    return lan;
            }
            return Languages.English;
        }

        public static void UpdateDictionaries()
        {
            string defaultCode = DefaultLanguage.GetStringValue();
            defaultlocalised = csvLoader.GetDictionary(defaultCode);

            string currentCode = Language.GetStringValue();
            localised = csvLoader.GetDictionary(currentCode);
        }

        public static Dictionary<string, string> GetDictionartForEditor()
        {
            if (!isInit) { Init(); }
            return defaultlocalised;
        }

        public static string GetLocalisedValue(string key)
        {
            if (!isInit) { Init(); }
            var value = key;

            localised.TryGetValue(key, out value);

            return value;
        }

#if UNITY_EDITOR
        public static void Add(string key, string value)
        {
            if (value.Contains("\""))
            {
                value.Replace('"', '\"');
            }

            if (csvLoader == null)
            {
                csvLoader = new CSVLoader();
            }

            csvLoader.LoadCSV(AssetCSV);
            csvLoader.Add(key, value);
            csvLoader.LoadCSV(AssetCSV);

            UpdateDictionaries();
        }

        public static void Replace(string key, string value)
        {
            if (value.Contains("\""))
            {
                value.Replace('"', '\"');
            }

            if (csvLoader == null)
            {
                csvLoader = new CSVLoader();
            }

            csvLoader.LoadCSV(AssetCSV);
            csvLoader.Edit(key, value);
            csvLoader.LoadCSV(AssetCSV);

            UpdateDictionaries();
        }

        public static void Remove(string key)
        {
            if (csvLoader == null)
            {
                csvLoader = new CSVLoader();
            }

            csvLoader.LoadCSV(AssetCSV);
            csvLoader.Remove(key);
            csvLoader.LoadCSV(AssetCSV);

            UpdateDictionaries();
        }
#endif
    }
}