﻿using System;
using System.Collections.Generic;
using Luna;
using UnityEngine;
using UnityEngine.UI;
using UniverseLib;
using UniverseLib.Config;
using UniverseLib.UI;
using UniverseLib.UI.Models;
using UniverseLib.UI.Panels;

namespace Lunistice_DebugConsole.UI
{
    public class DebugMenuUI : PanelBase, ITabbable
    {
        public override string Name => $"{Plugin.Name} by {Plugin.Author}";
        public override int MinWidth => 100;
        public override int MinHeight => 50;
        public override bool CanDragAndResize => true;
        public override Vector2 DefaultAnchorMin => new(0.25f, 0.25f);
        public override Vector2 DefaultAnchorMax => new(0.5f, 0.5f);

        private int _selectedTab = 0;
        private readonly List<TabPage> _tabPages = new();
        private readonly List<ButtonRef> _tabButtons = new();

        public DebugMenuUI(UIBase owner) : base(owner)
        {
            Player.OnPlayerLoaded += OnPlayerLoad;
        }

        public override void ConstructUI()
        {
            base.ConstructUI();

            uiRoot.GetComponent<Image>().color = new(0f,0f,0f,0f);
            ContentRoot.GetComponent<Image>().color = new(0.185f, 0.192f, 0.211f, 1f);
        }
        
        protected override void ConstructPanelContent()
        {
            GameObject tabGroup = UIFactory.CreateHorizontalGroup(ContentRoot, "TabGroup", true, true, true, true);
            UIFactory.SetLayoutElement(tabGroup, minHeight: 25, flexibleHeight: 0);

            AddTab<GameTab>(tabGroup, "Game", new object[] {ContentRoot});
            AddTab<LevelTab>(tabGroup, "Level", new object[] {ContentRoot});
            AddTab<PlayerTab>(tabGroup, "Player", new object[] {ContentRoot});
            AddTab<SettingsTab>(tabGroup, "Settings", new object[] {ContentRoot});
            
            SetTab(0);
            if (Plugin.ShowOnStart.Value) Show();
            else Hide();
        }

        protected override void OnClosePanelClicked() => Hide();

        public void Show()
        {
            SetActive(true);
            ConfigManager.Force_Unlock_Mouse = true;
            Game.Pause(true);
            UiManager.EnableUIInput = false;
        }

        public void Hide()
        {
            SetActive(false);
            ConfigManager.Force_Unlock_Mouse = false;
            UiManager.EnableUIInput = true;
            
            switch (UiManager.CurrentUIState)
            {
                case UiManager.UIState.Options:
                case UiManager.UIState.OptionsSubGameplay:
                case UiManager.UIState.OptionsSubAudio:
                case UiManager.UIState.OptionsSubCamera:
                case UiManager.UIState.OptionsSubGraphics:
                    break; // Do nothing for these UI states
                default:
                    if (Game.GameState == GameState.Mission)
                        UiManager.CurrentUIState = UiManager.UIState.Mission;
                    Game.Pause(false);
                    break;
            }
        }
        
        public void SetTab(int tabIndex)
        {
            if (_selectedTab != -1)
                DisableTab(_selectedTab);

            if (_tabPages.Count <= 0 || _tabButtons.Count <= 0 || tabIndex < 0 || tabIndex > _tabPages.Count - 1)
                return;
            
            RuntimeHelper.SetColorBlock(_tabButtons[tabIndex].Component, UniversalUI.EnabledButtonColor, UniversalUI.EnabledButtonColor * 1.2f);

            UIModel content = _tabPages[tabIndex];
            content.SetActive(true);

            _selectedTab = tabIndex;
        }

        public void DisableTab(int tabIndex)
        {
            if (_tabPages.Count <= 0 || _tabButtons.Count <= 0 || tabIndex < 0 || tabIndex > _tabPages.Count -1)
                return;
            _tabPages[tabIndex].SetActive(false);
            RuntimeHelper.SetColorBlock(_tabButtons[tabIndex].Component, UniversalUI.DisabledButtonColor, UniversalUI.DisabledButtonColor * 1.2f);
        }

        public void AddTab<T>(GameObject tabGroup,  string label, object[] args) where T : TabPage
        {
            var tab = (T) Activator.CreateInstance(typeof(T), args);
            tab.ConstructUI(ContentRoot);
            _tabPages.Add(tab);

            ButtonRef button = UIFactory.CreateButton(tabGroup, $"{label}__Button", label);
            int id = _tabButtons.Count;
            
            button.OnClick += () => { SetTab(id); };
            _tabButtons.Add(button);
            DisableTab(_tabButtons.Count - 1);
        }

        private static void OnPlayerLoad(Timer.Character character)
        {
            Player.Gravity = new Vector3(Plugin.GravityX.Value, Plugin.GravityY.Value,
                Plugin.GravityZ.Value);
            Player.GravityReset = Player.Gravity;
            Time.timeScale = Plugin.TimeScale.Value;
            switch (character)
            {
                case Timer.Character.Hana: 
                    Player.MaxLife          = Plugin.HanaMaxLife.Value;
                    Player.SprintSpeed      = Plugin.HanaSprintSpeed.Value; 
                    Player.RunSpeed         = Plugin.HanaRunSpeed.Value;
                    Player.TurboSpeed       = Plugin.HanaTurboSpeed.Value;
                    Player.JumpHeight       = Plugin.HanaJumpHeight.Value;
                    Player.AttackJumpHeight = Plugin.HanaAttackJump.Value;
                    Player.MaxDoubleJumps   = Plugin.HanaMaxDoubleJumps.Value;
                    Player.CoyoteTime       = Plugin.HanaCoyoteTime.Value;
                    Player.Friction         = Plugin.HanaFriction.Value;
                    Player.AirFriction      = Plugin.HanaAirFriction.Value;
                    Player.Acceleration     = Plugin.HanaAcceleration.Value;
                    break;
                case Timer.Character.Toree:
                    Player.MaxLife          = Plugin.ToreeMaxLife.Value;
                    Player.SprintSpeed      = Plugin.ToreeSprintSpeed.Value; 
                    Player.RunSpeed         = Plugin.ToreeRunSpeed.Value;
                    Player.TurboSpeed       = Plugin.ToreeTurboSpeed.Value;
                    Player.JumpHeight       = Plugin.ToreeJumpHeight.Value;
                    Player.AttackJumpHeight = Plugin.ToreeAttackJump.Value;
                    Player.MaxDoubleJumps   = Plugin.ToreeMaxDoubleJumps.Value;
                    Player.CoyoteTime       = Plugin.ToreeCoyoteTime.Value;
                    Player.Friction         = Plugin.ToreeFriction.Value;
                    Player.AirFriction      = Plugin.ToreeAirFriction.Value;
                    Player.Acceleration     = Plugin.ToreeAcceleration.Value;
                    break;
                case Timer.Character.Toukie:
                    Player.MaxLife          = Plugin.ToukieMaxLife.Value;
                    Player.SprintSpeed      = Plugin.ToukieSprintSpeed.Value; 
                    Player.RunSpeed         = Plugin.ToukieRunSpeed.Value;
                    Player.TurboSpeed       = Plugin.ToukieTurboSpeed.Value;
                    Player.JumpHeight       = Plugin.ToukieJumpHeight.Value;
                    Player.AttackJumpHeight = Plugin.ToukieAttackJump.Value;
                    Player.MaxDoubleJumps   = Plugin.ToukieMaxDoubleJumps.Value;
                    Player.CoyoteTime       = Plugin.ToukieCoyoteTime.Value;
                    Player.Friction         = Plugin.ToukieFriction.Value;
                    Player.AirFriction      = Plugin.ToukieAirFriction.Value;
                    Player.Acceleration     = Plugin.ToukieAcceleration.Value;
                    break;
            }
        }
    }
}