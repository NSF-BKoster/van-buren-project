// Copyright (C) 2006-2008 NeoAxis Group Ltd.
using System;
using System.Collections.Generic;
using System.Text;
using Engine;
using Engine.UISystem;
using Engine.MathEx;
using Engine.Renderer;
using System.IO;
using Engine.FileSystem;
using ConversationalAPI;
using ProjectEntities;

namespace Engine.UISystem
{
    /// <summary>
    /// Defines a about us window.
    /// </summary>
    public class ChatWindow : Control
    {
        Control window;

        public string BotName;
        
        VBCharacter starter;
        VBCharacter convPartner;

        ListBox conversationResponseListBox;
        TextBox conversationSayTextBox;

        public void SetPartners(VBCharacter Starter, VBCharacter Partner)
        {
            starter = Starter;
            convPartner = Partner;
        }

        protected override void OnAttach()
        {
            base.OnAttach();

            window = ControlDeclarationManager.Instance.CreateControl("VB//GUI//HUD//ChatWindow.gui");
            Controls.Add(window);

            conversationSayTextBox = (TextBox)window.Controls["conversationSayTextBox"];
            conversationResponseListBox = (ListBox)window.Controls["conversationResponseListBox"];


            conversationResponseListBox.SelectedIndexChange += new ListBox.SelectedIndexChangeDelegate(conversationResponseListBox_SelectedIndexChange);
        }

        void ParseEngineCommand(string[] commandData)
        {
            switch (commandData[0])
            {
                case "addobj":
                    starter.AttemptAddObjective(commandData[1]);
                    Log.Info(commandData[1] + " added");
                    break;

                case "follow":
                    (convPartner.Intellect as CompanionUnitAI).ToFollow = starter;
                    break;
                   
                case "stop":
                    (convPartner.Intellect as CompanionUnitAI).ToFollow = null;
                    break;

                case "weaponusebest":
                    convPartner.SetBestWeapon();
                    Log.Info("{0} equipped {1}", convPartner.Name, convPartner.ActiveHeldItem.Type.Name);
                    break;

                case "weaponputaway":
                    convPartner.ResetSlot(convPartner.PrimaryActive);
                    break;

                case "quit":
                    EngineApp.Instance.SetNeedExit();
                    break;

                case "attack":
                    VBFactionManager.Instance.GetFactionItemByType(convPartner.InitialFaction).Enemies.Add(starter.InitialFaction);
                    break;

                case "kill":
                    convPartner.Die();
                    break;

                case "setobjstatus":
                    VBFactionManager.Instance.GetFactionItemByType(starter.InitialFaction).ObjectiveManager.SetObjectiveStatus(commandData[1], Convert.ToInt32(commandData[2]));
                    break;

                case "getsex":
                    if (commandData[1] == "tramp")
                    Log.Info("You just had sex with a tramp. Well aren't you SPECIAL...");
                    break;

                default:
                    break;
            }
        }
        bool ParseVisibilityCommand(string[] commandArgs)
        {
            switch (commandArgs[0])
            {
                case "quest":
                    if (commandArgs.Length > 2)
                    {
                        if (VBFactionManager.Instance.GetFactionItemByType(starter.InitialFaction).ObjectiveManager.ObjectiveStatus(commandArgs[1]) == Convert.ToInt32(commandArgs[2]))
                            return true;
                    }
                    else
                    {
                        if (VBFactionManager.Instance.GetFactionItemByType(starter.InitialFaction).ObjectiveManager.ObjectiveStatus(commandArgs[1]) != -1)
                            return true;
                    }
                    break;

                default:
                    break;
            }

            return false;
        }

        void conversationResponseListBox_SelectedIndexChange(ListBox sender)
        {
            int selectedItem = conversationResponseListBox.SelectedIndex;
            if (selectedItem == -1)
            {
            }
            else
            {
                ConversationalResponseItem cri = (ConversationalResponseItem)conversationResponseListBox.Items[conversationResponseListBox.SelectedIndex];

                if (cri.EngineCommand != "")
                {
                    string[] commandsList = cri.EngineCommand.Split(' ');

                    foreach (string command in commandsList)
                        ParseEngineCommand(command.Split('$'));
                }

                if (cri.To != 0)
                {
                        ConversationalItem ci = Conversational.Instance.GetBotConversationByID(BotName, cri.To);

                        if (ci != null)
                        {
                            ChangeConversation(ci);
                        }
                }
                else
                {
                    SetShouldDetach();
                }
            }
        }

        private void ChangeConversation(ConversationalItem ci)
        {
            string selectedBot = ci.BotName;
            conversationSayTextBox.Text = ci.Say;
            conversationResponseListBox.Items.Clear();

            foreach (ConversationalResponseItem responseItem in ci.ConversationalResponseItems.ResponseItems)
            {
                if (responseItem.VisibilityOptions != "")
                {
                    bool shouldAdd = false;

                    if (responseItem.VisibilityOptions != "")
                    {
                        string[] commandsList = responseItem.VisibilityOptions.Split(' ');
                        
                        foreach (string command in commandsList)
                            shouldAdd = ParseVisibilityCommand(command.Split('$'));
                    }
                    else
                        shouldAdd = true;

                    if (shouldAdd)
                        conversationResponseListBox.Items.Add(responseItem);
                }
                else
                    AddListboxResponse(conversationResponseListBox, responseItem);
            }
        }

        void AddListboxResponse(ListBox list, ConversationalResponseItem responseItem)
        {
            if (starter.GetCharStat("intelligence") < 4 && responseItem.AltResponse != "")
                responseItem.altResponse = true;
            
            list.Items.Add(responseItem);
        }

        public void LoadConversation(string selectedBot, VBCharacter tmpent)
        {
            BotName = selectedBot;
            convPartner = tmpent;

            ConversationalItem ci = Conversational.Instance.GetBotFirstConversationItem(selectedBot);

            if (ci != null)
            {
                ChangeConversation(ci);
            }
            else
            {
                // Bot Not Found In File!
            }
        }

        protected override bool OnKeyDown(KeyEvent e)
        {
            if (base.OnKeyDown(e))
                return true;
            if (e.Key == EKeys.Escape)
            {
                SetShouldDetach();
                return true;
            }
            return false;
        }
    }
}
