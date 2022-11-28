using Assets.Scripts.PeroTools.Commons;
using FormulaBase;
using UnityEngine;

namespace MiscToolsForMD.MOD
{
    internal class KeyInfo
    {
        public ControlType type;
        public uint count;
        public KeyCode code;
        public GUIStyle style;
        public Color displayColor;

        public override string ToString()
        {
            return "type:" + type + ";code:" + code;
        }

        public void AddCount(uint countToAdd = 1)
        {
            if (Singleton<StageBattleComponent>.instance.isInGame)
            {
                count += countToAdd;
            }
        }

        public void SetColor(Color color)
        {
            if (Singleton<StageBattleComponent>.instance.isInGame)
            {
                style.normal.textColor = color;
            }
            else
            {
                ResetColor();
            }
        }

        public void ResetColor()
        {
            style.normal.textColor = displayColor;
        }
    }

    internal enum ControlType
    {
        Air,
        Fever,
        Ground
    }
}
