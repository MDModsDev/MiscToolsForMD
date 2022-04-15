using Assets.Scripts.Database;
using Assets.Scripts.GameCore.HostComponent;
using Assets.Scripts.GameCore.Managers;
using Assets.Scripts.PeroTools.Commons;
using Assets.Scripts.PeroTools.Managers;
using FormulaBase;
using GameLogic;
using HarmonyLib;
using Newtonsoft.Json;
using PeroPeroGames.GlobalDefines;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using System;

namespace MiscToolsForMD.SDK
{
    public class GameStatisticsProvider : ISingleOnly
    {
        private static int skippedNum = 0;
        private static readonly List<MusicData> musicDatas = new List<MusicData>();
        private int recordedMaxId = 0;
        private string id;

        /// <summary>
        /// Get controller keys in game config.
        /// </summary>
        /// <returns>
        /// A list contains all the key name's string.
        /// Only include Fever key when AutoFever is off and set a Fever key.
        /// <seealso cref="KeyCode"/>
        /// <see cref="https://docs.unity3d.com/ScriptReference/KeyCode.html"/>
        /// </returns>
        public List<string> GetControlKeys()
        {
            // See Assets.Scripts.GameCore.Controller.StandloneController.GetDefaultKeyList
            string text = "{\"Keylist\":{ \"Custom\":[{\"Key\":\"None\",\"Type\":\"BattleAir\"},{\"Key\":\"None\",\"Type\":\"BattleAir\"}," +
                "{\"Key\":\"None\",\"Type\":\"BattleAir\"},{\"Key\":\"None\",\"Type\":\"BattleAir\"},{\"Key\":\"None\",\"Type\":\"BattleGround\"}," +
                "{\"Key\":\"None\",\"Type\":\"BattleGround\"},{\"Key\":\"None\",\"Type\":\"BattleGround\"},{\"Key\":\"None\",\"Type\":\"BattleGround\"}]}," +
                "\"IsChanged\":\"false\",\"KeyBoardProposal\":\"Default\",\"HandleProposal\":\"Default\",\"IsVibration\":\"true\",\"FeverKey\":\"Space\"}";

            if (PlayerPrefs.HasKey("Controller"))
            {
                text = Singleton<ConfigManager>.instance.GetString("Controller");
            }
            KeyConfigObj keyConfig = JsonConvert.DeserializeObject<KeyConfigObj>(text);
            List<KeyObj> customizedKeys = keyConfig.KeyList.Custom.FindAll(key => key.Key != "None");
            List<KeyObj> airKeys = customizedKeys.FindAll(key => key.Type == "BattleAir");
            List<KeyObj> groundKeys = customizedKeys.FindAll(key => key.Type == "BattleGround");
            List<string> keys = new List<string>();
            airKeys.ForEach(key => keys.Add(key.Key));
            groundKeys.ForEach(key => keys.Add(key.Key));
            if (!DataHelper.isAutoFever && keyConfig.FeverKey != "None")
            {
                keys.Insert(keys.Count / 2, keyConfig.FeverKey);
            }
            return keys;
        }

        /// <summary>
        /// Get music info of current playing music.
        /// </summary>
        /// <returns>
        /// A MusicDisplayInfo object
        /// <seealso cref="MusicDisplayInfo"/>
        /// </returns>
        public MusicDisplayInfo GetMusicDisplayInfo()
        {
            // See SetSelectedMusicNameTxt.GetSelectedMusicName
            // and SetSelectedMusicNameTxt.GetSelectedMusicAuthor
            string musicName, musicAuthor;
            if (DataHelper.selectedAlbumUid != "collection" || DataHelper.selectedMusicIndex < 0)
            {
                musicName = Singleton<ConfigManager>.instance.GetConfigStringValue(DataHelper.selectedAlbumName, "uid", "name", DataHelper.selectedMusicUidFromInfoList);
                musicAuthor = Singleton<ConfigManager>.instance.GetConfigStringValue(DataHelper.selectedAlbumName, "uid", "author", DataHelper.selectedMusicUidFromInfoList);
            }
            else if (DataHelper.collections.Count == 0 || DataHelper.collections.Count < DataHelper.selectedMusicIndex)
            {
                musicName = "?????";
                musicAuthor = "???";
            }
            else
            {
                musicName = Singleton<ConfigManager>.instance.GetConfigStringValue(DataHelper.selectedAlbumName, "uid", "name", DataHelper.collections[DataHelper.selectedMusicIndex]);
                musicAuthor = Singleton<ConfigManager>.instance.GetConfigStringValue(DataHelper.selectedAlbumName, "uid", "author", DataHelper.collections[DataHelper.selectedMusicIndex]);
            }
            return new MusicDisplayInfo()
            {
                musicName = musicName,
                authorName = musicAuthor
            };
        }

        /// <summary>
        /// Get current actual weight.
        /// Accuracy is target weight divides actual weight.
        /// </summary>
        /// <returns>
        /// Current actual weight recorded by game.
        /// NOTE: If player skipped note, game will not record it immidiately!
        /// </returns>
        public int GetCurrentActualWeightInGame()
        {
            // See Assets.Scripts.GameCore.HostComponent.TaskStageTarget.GetTrueAccuracyNew
            TaskStageTarget targetInstance = Singleton<TaskStageTarget>.instance;
            int actualPerfectNum = targetInstance.GetHitCountByResult(TaskResult.Prefect);
            int actualGreatNum = targetInstance.GetHitCountByResult(TaskResult.Great);
            int actualTouchNum = targetInstance.GetCountValue(TaskCount.Music) + targetInstance.GetCountValue(TaskCount.Energy) +
                targetInstance.GetCountValue(TaskCount.TouhouRedPoint);
            int actualBlockNum = targetInstance.GetCountValue(TaskCount.Block);
            return (actualTouchNum + actualPerfectNum + actualBlockNum) * 2 + actualGreatNum;
        }

        /// <summary>
        /// Get current target weight.
        /// Accuracy is target weight divides actual weight.
        /// </summary>
        /// <returns>
        /// Current target weight,calculated from actual weight.
        /// NOTE: may not equals to game's value.
        /// </returns>
        [Fixme("Improve skippedNum calculation.")]
        public int GetCurrentTargetWeightInGame()
        {
            // See Assets.Scripts.GameCore.HostComponent.TaskStageTarget.GetTrueAccuracyNew
            TaskStageTarget targetInstance = Singleton<TaskStageTarget>.instance;
            int actualPerfectNum = targetInstance.GetHitCountByResult(TaskResult.Prefect);
            int actualGreatNum = targetInstance.GetHitCountByResult(TaskResult.Great);
            int actualTouchNum = targetInstance.GetCountValue(TaskCount.Music) + targetInstance.GetCountValue(TaskCount.Energy) +
                targetInstance.GetCountValue(TaskCount.TouhouRedPoint);
            int actualBlockNum = targetInstance.GetCountValue(TaskCount.Block);
            int actualMissNum = targetInstance.GetCountValue(TaskCount.Miss);
            return (actualBlockNum + actualGreatNum + actualPerfectNum + actualTouchNum + actualMissNum + skippedNum) * 2;
        }

        /// <summary>
        /// Get current weight, this is charactor's raw result(not changed by game).
        /// Accuracy is target weight divides actual weight.
        /// </summary>
        /// <returns>
        /// Current actual weight, has not been changed by game.
        /// </returns>
        public int GetCurrentRawActualWeightInGame()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get current target weight.
        /// Accuracy is target weight divides actual weight.
        /// </summary>
        /// <param name="idx">
        /// Current note's idx, you can get it from many game's method such as SetPlayResult...
        /// </param>
        /// <returns>
        /// Current target weight, calculated from MusicData.
        /// <seealso cref="StageBattleComponent.GetMusicData"/>
        /// </returns>
        [Fixme("Improve validMusicDatas calculation.")]
        public int GetCurrentTargetWeightByIdx(int idx)
        {
            // See Assets.Scripts.GameCore.HostComponent.TaskStageTarget.GetTrueAccuracyNew
            int validIdx = idx > recordedMaxId ? idx : recordedMaxId;
            List<MusicData> validMusicDatas = musicDatas.FindAll(musicData => musicData.objId <= validIdx && musicData.objId > 0);
            int touchNum = validMusicDatas.Count(musicData => musicData.noteData.type == (uint)NoteType.Hp || musicData.noteData.type == (uint)NoteType.Music);
            int normalNum = validMusicDatas.Count(musicData => musicData.noteData.addCombo && !musicData.isLongPressing);
            int blockNum = validMusicDatas.Count(musicData => musicData.noteData.type == (uint)NoteType.Block);
            recordedMaxId = idx > recordedMaxId ? idx : recordedMaxId;
            return (touchNum + normalNum + blockNum) * 2;
        }

        /// <summary>
        /// Return a bool value flag describes if player has skipped some note.
        /// </summary>
        /// <returns>
        /// If player has skipped some note.
        /// </returns>
        public bool IsPlayerSkipped()
        {
            return skippedNum > 0;
        }

        /// <summary>
        /// Export note info to a JSON document.
        /// </summary>
        /// <param name="path">
        /// Where to save JSON document.
        /// </param>
        public void ExportMusicDatasTo(string path)
        {
            string musicDatasJsonStr = JsonConvert.SerializeObject(musicDatas, Formatting.Indented);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, musicDatasJsonStr);
        }

        [Fixme("Calculate skippedNum correctly by provider and remove this method.")]
        [Obsolete(
            "This is a temp workaround for MiscToolsForMD and will be removed in the future. " +
            "Everyone using this should remove related code. " +
            "The skippedNum will be caclulated automatically by provider.")]
        public void AddSkippedNum(int num = 1)
        {
            skippedNum += num;
        }

        // --------------------------------- API Ends ---------------------------------
        // Methods bellow should not be used by user.
        public GameStatisticsProvider()
        {
            HarmonyLib.Harmony harmony = InstancesManager.GetHarmony();
            MethodInfo init = typeof(StageBattleComponent).GetMethod(nameof(StageBattleComponent.OnLoadComplete));
            MethodInfo initPatch = typeof(GameStatisticsProvider).GetMethod(nameof(GameStatisticsProvider.RefreshMusicDatas), BindingFlags.Static | BindingFlags.NonPublic);
            harmony.Patch(init, null, new HarmonyMethod(initPatch));
            MethodInfo onNoteResult = typeof(StatisticsManager).GetMethod(nameof(StatisticsManager.OnNoteResult));
            MethodInfo onNoteResultPatch = typeof(GameStatisticsProvider).GetMethod(nameof(GameStatisticsProvider.AddSkippedNumByResult), BindingFlags.Static | BindingFlags.NonPublic);
            harmony.Patch(onNoteResult, null, new HarmonyMethod(onNoteResultPatch));
        }

        public string GetID()
        {
            return id;
        }

        public void SetID(string id)
        {
            this.id = id;
        }

        public void OnRemove()
        {
            musicDatas.Clear();
            id = null;
            recordedMaxId = 0;
            skippedNum = 0;
        }

        private static void RefreshMusicDatas()
        {
            musicDatas.Clear();
            foreach (MusicData musicData in Singleton<StageBattleComponent>.instance.GetMusicData())
            {
                musicDatas.Add(musicData);
            }
        }

        private static void AddSkippedNumByResult(int result)
        {
            if (result == (int)TaskResult.None)
            {
                skippedNum++;
            }
        }
    }
}
