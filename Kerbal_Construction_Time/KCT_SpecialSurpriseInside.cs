using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP;

namespace KerbalConstructionTime
{
    //shhhh! Don't look at this! You'll spoil yourself! Wait till April 1st!
    class KCT_SpecialSurpriseInside
    {
        public static KCT_SpecialSurpriseInside instance = null;
        public bool activated = false, showRace = false, showAd = false, challengeCompleted = false, disableBlocks = false;
        public Texture2D jebCoinTex, rt10Tex, Ad1;

        public int TotalJebCoins = 2, sceneChanges = 0, sceneChangeLimit = 6;
        public Rect raceRect = new Rect((Screen.width-512) / 2, Screen.height / 2, 512, 1);
        public Rect adRect = new Rect(0, 0, Screen.width, Screen.height);
        private ProtoCrewMember chosenKerbal = null;
        private SRBRace raceInstance = null;


        public KCT_SpecialSurpriseInside()
        {
            instance = this;
            if (KCT_GameStates.settings.DisableSpecialSurprise || DateTime.Today.Month != 4 || DateTime.Today.Day != 1)
                return;
            else
                DeploySurprise();

            
        }

        private void DeploySurprise()
        {
            Debug.Log("[KCT] Deploying surprise in 5...4...3...2...1... Let's start the fun! ;) HAHAHAHA >:D");

            String filePath = KSPUtil.ApplicationRootPath + "GameData/KerbalConstructionTime/Icons/jebcoin_32.png";
            activated = true;
            jebCoinTex = new Texture2D(2, 2);
            byte[] fileData;
            if (System.IO.File.Exists(filePath))
            {
                fileData = System.IO.File.ReadAllBytes(filePath);
                jebCoinTex.LoadImage(fileData);
            }

            rt10Tex = new Texture2D(2, 2);
            filePath = KSPUtil.ApplicationRootPath + "GameData/KerbalConstructionTime/Icons/RT-10_32.png";
            if (System.IO.File.Exists(filePath))
            {
                fileData = System.IO.File.ReadAllBytes(filePath);
                rt10Tex.LoadImage(fileData);
            }

            Ad1 = new Texture2D(2, 2);
            filePath = KSPUtil.ApplicationRootPath + "GameData/KerbalConstructionTime/Icons/ad1.png";
            if (System.IO.File.Exists(filePath))
            {
                fileData = System.IO.File.ReadAllBytes(filePath);
                Ad1.LoadImage(fileData);
            }
            
            string popupString = "Welcome to Kerbal Crush Saga: Rocket Fire Age, the replacement for Kerbal Construction Time that makes KSP more like your FAVORITE smart phone games!\n\n"+
                "Take part in DAILY CHALLENGES or SRB RACES to earn JEB COIN to spend on speeding up rocket construction! Build REALISTIC rockets at REALISTIC speeds, in REAL TIME!";
            PopupDialog.SpawnPopupDialog("Kerbal Crush Saga: Rocket Fire Age", popupString, "Let's Go!", false, HighLogic.Skin);
        }

        public void CheckShipForChallengeComplete()
        {
            Vessel active = FlightGlobals.ActiveVessel;
            double targetLat = -0.092222, targetLon = 285.4475, targetAlt = 102.0;
            if (active != null && !challengeCompleted)
            {
                if (active.Landed && KCT_Utilities.ApproximatelyEqual(targetLat, active.latitude, 0.006) && KCT_Utilities.ApproximatelyEqual(targetLon, active.longitude, 0.006) && KCT_Utilities.ApproximatelyEqual(targetAlt, active.altitude, 5))
                {
                    bool hasChutes = false;
                    bool hasSRB = false;
                    foreach (Part p in active.parts)
                    {
                        if (p.Modules != null && (p.Modules.Contains("ModuleParachute") || p.Modules.Contains("RealChuteModule")))
                        { 
                            hasChutes = true;
                        }
                        if (p.partInfo.title.ToLower().Contains("rt-10"))
                        {
                            if (p.Modules.Contains("ModuleEngines"))
                            {
                                ModuleEngines engine = (ModuleEngines)p.Modules["ModuleEngines"];
                                if (engine.EngineIgnited)
                                    hasSRB = true;
                            }
                        }
                    }

                    if (!hasChutes && hasSRB)
                    {
                        challengeCompleted = true;
                        string popupString = "Congratulations! You completed the Daily Challenge! Take some pictures to show your worth and post them on the forum thread!\n\n" +
                            "(Ok, seriously? You seriously completed the Daily Challenge? Screw it, you get over 9000 Jeb Coins (a $45,000 value). Please, for the love of all things Kerbal, post your result on the KCT forum thread. I HAVE to see this.)";
                        PopupDialog.SpawnPopupDialog("Challenge Complete!", popupString, "Collect Reward!", false, HighLogic.Skin);
                        TotalJebCoins += 9001;
                    }
                }
            }
        }

        public void JebCoinStore()
        {
            DialogOption[] options = new DialogOption[6];
            options[0] = new DialogOption("1 Jeb Coin - $5", OpenBrowserToJebCoinWebsite);
            options[1] = new DialogOption("2 Jeb Coins - $9 (Save 10%!)", OpenBrowserToJebCoinWebsite);
            options[2] = new DialogOption("5 Jeb Coins - $20 (Save 20%!)", OpenBrowserToJebCoinWebsite);
            options[3] = new DialogOption("10 Jeb Coins - $35 (Save 30%!)", DisablePopup);
            options[4] = new DialogOption("100 Jeb Coins - $250 (Save 50%! BEST DEAL!)", OpenBrowserToJebCoinWebsite);
            options[5] = new DialogOption("Close Store", EmptyVoid);
            MultiOptionDialog popup = new MultiOptionDialog("Purchase additional Jeb Coin from the Store for new, low rates!\n"+
            "Note: You will be redirected to the store website to complete your purchase.", "Jeb Coin Store", HighLogic.Skin, options);
            PopupDialog.SpawnPopupDialog(popup, false, HighLogic.Skin);
        }

        public void DailyChallengePopup()
        {
            string message = "Werhner accidentally forgot an important experiment near the launchpad and needs your help retrieving it. "
            + "You agreed before he mentioned that it's on top of the Tier 3 water tower, and that all the parachutes are being repacked and can't be used, and that Kerbodyne needs an RT-10 tested ASAP.\n"
            + "GOAL: Land a vessel on top of the water tower, without parachutes, with an activated RT-10 Solid Fuel Booster. You may use any additional parts you desire. This can be performed during a Simulation."
            + "\nREWARD: 5 Jeb Coins";
            PopupDialog.SpawnPopupDialog("Daily Challenge: April 1st", message, "Close", false, HighLogic.Skin);
        }

        public void EditorUnlockPopup()
        {
            if (KCT_SpecialSurpriseInside.instance.TotalJebCoins < 2)
                PopupDialog.SpawnPopupDialog("Stop!", "You must unlock vessel construction by spending 2 Jeb Coins!", "Close", false, HighLogic.Skin);
            else
            {
                DialogOption[] options = new DialogOption[2];
                options[0] = new DialogOption("Spend Jeb Coins", DoError);
                options[1] = new DialogOption("Close", EmptyVoid);
                MultiOptionDialog popup = new MultiOptionDialog("You must unlock vessel construction by spending 2 Jeb Coins!", "Stop!", HighLogic.Skin, options);
                PopupDialog.SpawnPopupDialog(popup, false, HighLogic.Skin);
            }
        }

        public void SRBRace(int windowIndex)
        {
            raceRect.y = (Screen.height - raceRect.height) / 2;

            List<ProtoCrewMember> available = new List<ProtoCrewMember>();
            foreach (ProtoCrewMember crew in HighLogic.CurrentGame.CrewRoster.Crew)
            {
                if (crew.rosterStatus == ProtoCrewMember.RosterStatus.Available)
                    available.Add(crew);
            }
            GUILayout.BeginVertical();
            if (chosenKerbal == null)
            {
                //choose kerbal
                GUILayout.Label("Choose your racer for the SRB Race!");
                foreach (ProtoCrewMember crew in available)
                {
                    if (GUILayout.Button(crew.name))
                    {
                        chosenKerbal = crew;
                        raceInstance = new SRBRace(crew);
                        raceRect.height = 1;
                    }
                }
                if (GUILayout.Button("Cancel"))
                {
                    showRace = false;
                    raceRect.height = 1;
                }
            }
            else
            {
                //do race
                int index = 0;
                foreach (KeyValuePair<string, float> racer in raceInstance.RacerProgress)
                {
                    GUILayout.BeginHorizontal(HighLogic.Skin.textArea);
                    GUILayout.Label(racer.Key + ": ", GUILayout.Width(125));
                    //GUILayout.Label(racer.Value.ToString());
                    GUI.DrawTexture(new Rect(125+(512-125-40)*(float)Math.Min(racer.Value, 1.0), 37 * index + 38, 32, 15), rt10Tex);
                    GUILayout.EndHorizontal();
                    index++;
                }
                if (raceInstance.finished)
                {
                    GUILayout.Label(raceInstance.winner.name + " won!");
                    if (raceInstance.winner.name == chosenKerbal.name)
                    {
                        //you won!
                        GUILayout.Label("Reward: 1 Jeb Coin!");
                        if (GUILayout.Button("Collect Reward!"))
                        {
                            chosenKerbal = null;
                            TotalJebCoins++;
                            showRace = false;
                            raceRect.height = 1;
                        }
                    }
                    else
                    {
                        //You lost :(
                        GUILayout.Label("Better luck next time!");
                        if (GUILayout.Button("Return To KSC in Shame"))
                        {
                            chosenKerbal = null;
                            showRace = false;
                            raceRect.height = 1;
                        }
                    }
                    
                }
                else
                {
                    raceInstance.Step();
                }
            }

            GUILayout.EndVertical();
        }

        private void OpenBrowserToJebCoinWebsite()
        {
            Application.OpenURL("http://magico13.net/KCT/KCSRFA.html");
        }

        private void EmptyVoid()
        {

        }

        private void DoError()
        {
            PopupDialog.SpawnPopupDialog("Error", "An error occurred while processing your request, please try again!", "Close", false, HighLogic.Skin);
            TotalJebCoins -= 2;
        }

        public void FullPageAd(int windowID)
        {
            GUILayout.BeginVertical();
            if (GUILayout.Button("X", GUILayout.ExpandWidth(false)))
                showAd = false;
            GUILayout.Label(Ad1);
            GUILayout.EndVertical();
        }

        public void DisablePopup()
        {
            DialogOption[] options = new DialogOption[3];
            options[0] = new DialogOption("Disable Advertisements", DisableAdverts);
            options[1] = new DialogOption("Disable All Parts", DisableAll);
            options[2] = new DialogOption("Do Nothing", EmptyVoid);
            string msg = "Alright, I've had my fun with you. You can disable Kerbal Crush Saga: Rocket Fire Age and go back to using Kerbal Construction Time now."+
                " If you want to still play around with the prank (for today only), you can choose to disable advertisements and the block on building vessels, but keep the SRB races, daily challenge, and Jeb Coins."+
                " Just go back to the Jeb Coin Store and press the button for 10 Jeb Coins to bring this back up."+
                "\n\nSorry if I drove you a little nuts ;) - magico13";
            MultiOptionDialog popup = new MultiOptionDialog(msg, "Thanks For Playing Along", HighLogic.Skin, options);
            PopupDialog.SpawnPopupDialog(popup, false, HighLogic.Skin);
        }

        private void DisableAdverts()
        {
            disableBlocks = true;
        }

        private void DisableAll()
        {
            activated = false;
            KCT_GameStates.settings.DisableSpecialSurprise = true;
            KCT_GameStates.settings.Save();
        }
    }

    class SRBRace
    {
        private List<ProtoCrewMember> racers;
        private ProtoCrewMember chosen;
        public Dictionary<string, float> RacerProgress;
        private System.Random rand;
        public bool finished;
        public ProtoCrewMember winner;
        private int stepSize = 5, internalCounter = 0;

        public SRBRace(ProtoCrewMember chosenOne)
        {
            rand = new System.Random((int)Time.time);
            finished = false;
            chosen = chosenOne;
            racers = new List<ProtoCrewMember>();
            RacerProgress = new Dictionary<string, float>();
            racers.Add(chosen);
            RacerProgress.Add(chosen.name, 0.0f);

            List<ProtoCrewMember> available = new List<ProtoCrewMember>();

            foreach (ProtoCrewMember crew in HighLogic.CurrentGame.CrewRoster.Crew)
            {
                if (crew.rosterStatus == ProtoCrewMember.RosterStatus.Available && crew != chosen)
                {
                    available.Add(crew);
                }
            }
            foreach (ProtoCrewMember crew in HighLogic.CurrentGame.CrewRoster.Applicants)
            {
                if (crew.rosterStatus == ProtoCrewMember.RosterStatus.Available)
                {
                    available.Add(crew);
                }
            }
            while (racers.Count < 5 && available.Count > 0)
            {
                int selected = rand.Next(available.Count - 1);
                racers.Add(available[selected]);
                RacerProgress.Add(available[selected].name, 0.0f);
                available.RemoveAt(selected);
            }
        }

        public void Step()
        {
            if (internalCounter < stepSize)
            {
                internalCounter++;
                return;
            }
            internalCounter = 0;
            for (int i=0; i<racers.Count; i++)
            {
                string name = racers[i].name;
                RacerProgress[name] += ((float)rand.NextDouble() / 100)*(racers[i].experienceLevel+1);
                if (RacerProgress[name] >= 1.0)
                {
                    finished = true;
                    if (winner == null)
                        winner = racers[i];
                    else if (RacerProgress[name] > RacerProgress[winner.name])
                        winner = racers[i];
                }
            }
        }
    }
}
