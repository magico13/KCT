using KSP.UI.Screens;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace KerbalConstructionTime
{
    /// <summary>
    /// This class attempts to override the KSC Upgrade buttons so that KCT can implement it's own form of KSC upgrading
    /// </summary>
    public class KCT_KSCContextMenuOverrider
    {
        KSCFacilityContextMenu _menu = null;
        public KCT_KSCContextMenuOverrider(KSCFacilityContextMenu menu)
        {
            _menu = menu;
        }

        public IEnumerator OnContextMenuSpawn()
        {
            yield return new WaitForFixedUpdate();
            if (KCT_PresetManager.Instance.ActivePreset.generalSettings.KSCUpgradeTimes && _menu != null)
            {
                SpaceCenterBuilding hostBuilding = getMember<SpaceCenterBuilding>("host");
                KCTDebug.Log("Trying to override upgrade button of menu for "+hostBuilding.facilityName);
                UnityEngine.UI.Button button = getMember<UnityEngine.UI.Button>("UpgradeButton");
                if (button == null)
                {
                    KCTDebug.Log("Could not find UpgradeButton by name, using index instead.", true);
                    button = getMember<UnityEngine.UI.Button>(2);
                }
                if (button != null)
                {
                    KCTDebug.Log("Found upgrade button, overriding it.");
                    button.onClick = new UnityEngine.UI.Button.ButtonClickedEvent(); //Clear existing KSP listener
                    
                    button.onClick.AddListener(handleUpgrade);
                }
                else
                {
                    throw new Exception("UpgradeButton not found. Cannot override.");
                }
            }
        }

        internal T getMember<T>(string name)
        {
            
            MemberInfo member = _menu.GetType().GetMember(name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy)?.FirstOrDefault();
            if (member == null)
            {
                KCTDebug.Log($"Member was null when trying to find '{name}'", true);
                return default(T);
            }
            object o = KCT_Utilities.GetMemberInfoValue(member, _menu);
            if (o is T)
            {
                return (T)o;
            }
            return default(T);
        }

        internal T getMember<T>(int index)
        {
            IEnumerable<MemberInfo> memberList = _menu.GetType().GetMembers(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy).Where(m => m.ToString().Contains(typeof(T).ToString()));
            KCTDebug.Log($"Found {memberList.Count()} matches for {typeof(T)}");
            MemberInfo member = memberList.Count() >= index ? memberList.ElementAt(index) : null;
            if (member == null)
            {
                KCTDebug.Log($"Member was null when trying to find element at index {index} for type '{typeof(T).ToString()}'", true);
                return default(T);
            }
            object o = KCT_Utilities.GetMemberInfoValue(member, _menu);
            if (o is T)
            {
                return (T)o;
            }
            return default(T);
        }

        internal void handleUpgrade()
        {
            int oldLevel = getMember<int>("level");
            KCTDebug.Log($"Upgrading from level {oldLevel}");

            string facilityID = GetFacilityID();

            KCT_UpgradingBuilding upgrading = new KCT_UpgradingBuilding(facilityID, oldLevel+1, oldLevel, facilityID.Split('/').Last());

            upgrading.isLaunchpad = facilityID.ToLower().Contains("launchpad");
            if (upgrading.isLaunchpad)
            {
                upgrading.launchpadID = KCT_GameStates.ActiveKSC.ActiveLaunchPadID;
                if (upgrading.launchpadID > 0)
                    upgrading.commonName += KCT_GameStates.ActiveKSC.ActiveLPInstance.name;
            }

            if (!upgrading.AlreadyInProgress())
            {
                float cost = getMember<float>("upgradeCost");

                if (Funding.CanAfford(cost))
                {
                    Funding.Instance.AddFunds(-cost, TransactionReasons.Structures);
                    KCT_GameStates.ActiveKSC.KSCTech.Add(upgrading);
                    upgrading.SetBP(cost);
                    upgrading.cost = cost;

                    ScreenMessages.PostScreenMessage("Facility upgrade requested!", 4.0f, ScreenMessageStyle.UPPER_CENTER);
                    KCTDebug.Log($"Facility {facilityID} upgrade requested to lvl {oldLevel + 1} for {cost} funds, resulting in a BP of {upgrading.BP}");
                }
                else
                {
                    KCTDebug.Log("Couldn't afford to upgrade.");
                    ScreenMessages.PostScreenMessage("Not enough funds to upgrade facility!", 4.0f, ScreenMessageStyle.UPPER_CENTER);
                }
            }
            else if (oldLevel+1 != upgrading.currentLevel)
            {
                ScreenMessages.PostScreenMessage("Facility is already being upgraded!", 4.0f, ScreenMessageStyle.UPPER_CENTER);
                KCTDebug.Log($"Facility {facilityID} tried to upgrade to lvl {oldLevel+1} but already in list!");
            }

            _menu.Dismiss(KSCFacilityContextMenu.DismissAction.None);
        }


        public string GetFacilityID()
        {
            return getMember<SpaceCenterBuilding>("host").Facility.id;
        }
    }
}
