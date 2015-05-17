using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KerbalConstructionTime
{
    class KCT_PresetManager
    {
        public static KCT_PresetManager Instance;

        public KCT_PresetManager()
        {
            Instance = this;
        }

    }

    class KCT_Preset : ConfigNodeStorage
    {
        public KCT_Preset_General generalSettings;
        public KCT_Preset_Time timeSettings;
        public KCT_Preset_Formula formulaSettings;

        [Persistent] string name, description, author;

        public KCT_Preset(string presetName, string presetDescription, string presetAuthor)
        {
            name = presetName;
            description = presetDescription;
            author = presetAuthor;

            generalSettings = new KCT_Preset_General();
            timeSettings = new KCT_Preset_Time();
            formulaSettings = new KCT_Preset_Formula();
        }

        public ConfigNode AsConfigNode()
        {

        }

    }

    class KCT_Preset_General : ConfigNodeStorage
    {

    }

    class KCT_Preset_Time : ConfigNodeStorage
    {

    }

    class KCT_Preset_Formula : ConfigNodeStorage
    {

    }
}
