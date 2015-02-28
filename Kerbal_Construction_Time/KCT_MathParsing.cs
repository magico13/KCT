using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace KerbalConstructionTime
{
    public class KCT_MathParsing
    {
        public static double GetStandardFormulaValue(string formulaName, Dictionary<string, string> variables)
        {
            switch (formulaName)
            {
                case "Node": return ParseMath(KCT_GameStates.formulaSettings.NodeFormula, variables);
                case "UpgradeFunds": return ParseMath(KCT_GameStates.formulaSettings.UpgradeFundsFormula, variables);
                case "UpgradeScience": return ParseMath(KCT_GameStates.formulaSettings.UpgradeScienceFormula, variables);
                case "Research": return ParseMath(KCT_GameStates.formulaSettings.ResearchFormula, variables);
                case "EffectivePart": return ParseMath(KCT_GameStates.formulaSettings.EffectivePartFormula, variables);
                case "ProceduralPart": return ParseMath(KCT_GameStates.formulaSettings.ProceduralPartFormula, variables);
                case "BP": return ParseMath(KCT_GameStates.formulaSettings.BPFormula, variables);
                default: return 0;
            }
        }

        public static double ParseMath(string input, Dictionary<string, string> variables)
        {
            KCTDebug.Log("Input_raw: " + input);
            foreach (KeyValuePair<string, string> kvp in variables)
            {
                if (input.Contains("[" + kvp.Key + "]"))
                {
                    input = input.Replace("[" + kvp.Key + "]", kvp.Value);
                }
            }
            KCTDebug.Log("Input_replaced: " + input);

            double currentVal = 0;
            string stack = "";
            string lastOp = "+";
            string[] ops = { "+", "-", "*", "/", "%", "^", "(", "e", "l", "L", "m" };
            for (int i = 0; i < input.Length; ++i)
            {
                string ch = input[i].ToString();
                bool isOp = false;
                //  KCTDebug.Log(ch);
                foreach (string op in ops)
                {
                    if (op == ch)
                    {
                        isOp = true;
                        break;
                    }
                }
                if (isOp)
                {
                    if (ch == "-" && (stack.Trim() == ""))
                    {
                        stack += ch;
                    }
                    else if (ch == "e" || ch == "E")
                    {
                        int index;
                        for (index = i; index < input.Length; ++index)
                        {
                            string ch2 = input[index].ToString();
                            if (ops.Contains(ch2))
                                break;
                        }
                        string sub = input.Substring(i + 1, index - i - 1);
                        double exp = ParseMath(sub, variables);
                        double newVal = double.Parse(stack) * Math.Pow(10, exp);
                        currentVal = DoMath(currentVal, lastOp, newVal.ToString());
                        stack = "0";
                        lastOp = "+";
                        i = index - 1;
                    }
                    else if (ch == "(")
                    {
                        int j = FindEndParenthesis(input, i);
                        string sub = input.Substring(i + 1, j - i - 1);
                        string val = ParseMath(sub, variables).ToString();
                        input = input.Substring(0, i) + val + input.Substring(j + 1);
                        --i;
                    }
                    else if (ch == "l" && input[i + 1].ToString() == "(")
                    {
                        int j = FindEndParenthesis(input, i + 1);
                        string sub = input.Substring(i + 2, j - i - 2);
                        double val = ParseMath(sub, variables);
                        val = Math.Log(val);
                        input = input.Substring(0, i) + val.ToString() + input.Substring(j + 1);
                        --i;
                    }
                    else if (ch == "L" && input[i + 1].ToString() == "(")
                    {
                        int j = FindEndParenthesis(input, i + 1);
                        string sub = input.Substring(i + 2, j - i - 2);
                        double val = ParseMath(sub, variables);
                        val = Math.Log10(val);
                        input = input.Substring(0, i) + val.ToString() + input.Substring(j + 1);
                        --i;
                    }
                    else if (ch == "m")
                    {
                        int j = FindEndParenthesis(input, i + 4);
                        string[] parts = input.Substring(i + 4, j - i - 4).Split(',');
                        if (parts.Length > 2)
                        {
                            for (int k = 2; k < parts.Length; k++)
                                parts[1] += "," + parts[k];
                        }
                        //KCTDebug.Log(parts[0]);
                        double sub1 = ParseMath(parts[0], variables);
                        double sub2 = ParseMath(parts[1], variables);
                        double val = 0;
                        if (input.Substring(i, 4) == "max(")
                            val = Math.Max(sub1, sub2);
                        else if (input.Substring(i, 4) == "min(")
                            val = Math.Min(sub1, sub2);

                        input = input.Substring(0, i) + val.ToString() + input.Substring(j + 1);
                        --i;
                    }
                    else
                    {
                        currentVal = DoMath(currentVal, lastOp, stack);
                        lastOp = ch;
                        stack = "";
                    }
                }
                else
                {
                    stack += ch;
                }
            }
            currentVal = DoMath(currentVal, lastOp, stack);
            KCTDebug.Log("Result: " + currentVal);
            return currentVal;
        }

        private static int FindEndParenthesis(string str, int curPos)
        {
            int depth = 0;
            int j = 0;
            for (j = curPos + 1; j < str.Length; ++j)
            {
                if (str[j] == '(') depth++;
                if (str[j] == ')') depth--;

                if (depth < 0)
                {
                    break;
                }
            }
            return j;
        }

        private static double DoMath(double currentVal, string operation, string newVal)
        {
            double newValue = 0;
            if (!double.TryParse(newVal, out newValue))
            {
                Debug.LogError("[KCT] Tried to parse a non-double value: " + newVal);
                return currentVal;
            }
            switch (operation)
            {
                case "+": currentVal += newValue; break;
                case "-": currentVal -= newValue; break;
                case "*": currentVal *= newValue; break;
                case "/": currentVal /= newValue; break;
                case "%": currentVal = currentVal % long.Parse(newVal); break;
                case "^": currentVal = Math.Pow(currentVal, newValue); break;
                default: break;
            }

            return currentVal;
        }
    }
}
