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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MiscToolsForMD.SDK
{
    public class GameStatisticsProvider : ISingleOnly
    {
        private static readonly List<MusicData> musicDatas = new List<MusicData>();
        private static int targetWeightBySelf = 0;
        private static int actualWeightBySelf = 0;
        private static int skippedNum = 0;
        private static int recordedMaxId = 0;

        /// <summary>
        /// Get controller keys in game config. See
        /// <seealso cref="Assets.Scripts.GameCore.Controller.StandloneController.GetDefaultKeyList"/>
        /// </summary>
        /// <returns>
        /// A list contains all the key name's string.
        /// Only include Fever key when AutoFever is off and set a Fever key. See
        /// <seealso cref="KeyCode"/> and
        /// <see cref="https://docs.unity3d.com/ScriptReference/KeyCode.html"/>
        /// </returns>
        public List<string> GetControlKeys()
        {
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
        /// Get music info of current playing music. See
        /// <seealso cref="StatisticsManager.OnBattleStart">
        /// </summary>
        /// <returns>
        /// A <seealso cref="MusicDisplayInfo"/> object
        /// </returns>
        public MusicDisplayInfo GetMusicDisplayInfo()
        {
            string musicName, musicAuthor;
            string selectedAlbumName = DataHelper.selectedAlbumName;
            string selectedMusicUid = DataHelper.selectedMusicUid;
            ConfigManager configManager = Singleton<ConfigManager>.instance;
            musicAuthor = configManager.GetConfigStringValue(selectedAlbumName, "uid", "author", selectedMusicUid);
            musicName = configManager.GetConfigStringValue(selectedAlbumName, "uid", "name", selectedMusicUid);
            // See https://github.com/MDModsDev/SongDesc/blob/master/Patch.cs
            return new MusicDisplayInfo()
            {
                musicName = musicName,
                authorName = musicAuthor,
                musicLevel = DataHelper.selectedMusicLevel,
                difficulty = (Difficulty)DataHelper.selectedDifficulty
            };
        }

        /// <summary>
        /// Get current actual weight. See
        /// <seealso cref="TaskStageTarget.GetTrueAccuracyNew"/>
        /// </summary>
        /// <returns>
        /// Current actual weight recorded by game.
        /// NOTE: If player skipped note, game will not record it immidiately!
        /// </returns>
        public int GetCurrentActualWeightInGame()
        {
            TaskStageTarget targetInstance = Singleton<TaskStageTarget>.instance;
            int actualPerfectNum = targetInstance.GetHitCountByResult(TaskResult.Prefect);
            int actualGreatNum = targetInstance.GetHitCountByResult(TaskResult.Great);
            int actualTouchNum = targetInstance.GetCountValue(TaskCount.Music) + targetInstance.GetCountValue(TaskCount.Energy) +
                targetInstance.GetCountValue(TaskCount.TouhouRedPoint);
            int actualBlockNum = targetInstance.GetCountValue(TaskCount.Block);
            return (actualTouchNum + actualPerfectNum + actualBlockNum) * 2 + actualGreatNum;
        }

        /// <summary>
        /// Get current target weight calculated from actual weight. See
        /// <seealso cref="TaskStageTarget.GetTrueAccuracyNew"/>
        /// </summary>
        /// <returns>
        /// Current target weight.
        /// NOTE: may not equals to game's value.
        /// </returns>
        [Fixme("Improve skippedNum calculation.")]
        public int GetCurrentTargetWeightInGame()
        {
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
        /// </summary>
        /// <returns>
        /// Current actual weight, has not been changed by game.
        /// </returns>
        public int GetCurrentRawActualWeightInGame()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get current target weight calculated from MusicData. See
        /// <seealso cref="TaskStageTarget.GetTrueAccuracyNew"/>
        /// </summary>
        /// <param name="idx">
        /// Current note's idx, you can get it from many game's method such as SetPlayResult...
        /// </param>
        /// <returns>
        /// Current target weight. See
        /// <seealso cref="StageBattleComponent.GetMusicData"/>
        /// </returns>
        [Fixme("Improve validMusicDatas calculation.")]
        public int GetCurrentTargetWeightInGameByIdx(int idx)
        {
            int validIdx = idx > recordedMaxId ? idx : recordedMaxId;
            List<MusicData> validMusicDatas = musicDatas.FindAll(musicData => musicData.objId <= validIdx && musicData.objId > 0);
            int touchNum = validMusicDatas.Count(musicData => musicData.noteData.type == (uint)NoteType.Hp || musicData.noteData.type == (uint)NoteType.Music);
            int normalNum = validMusicDatas.Count(musicData => musicData.noteData.addCombo && !musicData.isLongPressing);
            int blockNum = validMusicDatas.Count(musicData => musicData.noteData.type == (uint)NoteType.Block);
            recordedMaxId = idx > recordedMaxId ? idx : recordedMaxId;
            return (touchNum + normalNum + blockNum) * 2;
        }

        /// <summary>
        /// Get current actual weight calculated by provider.
        /// </summary>
        /// <returns>
        /// Current actual weight.
        /// </returns>
        public int GetCurrentActualWeightBySelf()
        {
            return actualWeightBySelf;
        }

        /// <summary>
        /// Get current target weight calculated by provider.
        /// </summary>
        /// <returns>
        /// Current target weight.
        /// </returns>
        public int GetCurrentTargetWeightBySelf()
        {
            return targetWeightBySelf;
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
        /// Return a bool value flag describes if player has skipped some note or game thinks player is Missed.
        /// </summary>
        /// <returns>
        /// If player strictly missed.
        /// </returns>
        public bool IsStrictlyMissed()
        {
            return IsPlayerSkipped() || (Singleton<TaskStageTarget>.instance.GetMiss() > 0);
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

        // --------------------------------- API Ends ---------------------------------
        // Methods bellow should not be used by user.

        public GameStatisticsProvider()
        {
            HarmonyLib.Harmony harmony = InstancesManager.GetHarmony();

            MethodInfo init = typeof(StageBattleComponent).GetMethod(nameof(StageBattleComponent.InitGame));
            MethodInfo initPatch = typeof(GameStatisticsProvider).GetMethod(nameof(GameStatisticsProvider.RefreshMusicDatas), BindingFlags.Static | BindingFlags.NonPublic);
            harmony.Patch(init, null, new HarmonyMethod(initPatch));

            MethodInfo onNoteResult = typeof(StatisticsManager).GetMethod(nameof(StatisticsManager.OnNoteResult));
            MethodInfo onNoteResultPatch = typeof(GameStatisticsProvider).GetMethod(nameof(GameStatisticsProvider.AddSkippedNumWhenSkippedMusicOrHeart), BindingFlags.Static | BindingFlags.NonPublic);
            harmony.Patch(onNoteResult, null, new HarmonyMethod(onNoteResultPatch));

            MethodInfo setPlayResult = typeof(BattleEnemyManager).GetMethod(nameof(BattleEnemyManager.SetPlayResult));
            MethodInfo setPlayResultPatch = typeof(GameStatisticsProvider).GetMethod(nameof(GameStatisticsProvider.SetWeightsByResult), BindingFlags.Static | BindingFlags.NonPublic);
            harmony.Patch(setPlayResult, null, new HarmonyMethod(setPlayResultPatch));

            // TODO:Fix patch target
            MethodInfo addComboMiss = typeof(StageBattleComponent).GetMethod(nameof(StageBattleComponent.SetCombo));
            MethodInfo addComboMissPatch = typeof(GameStatisticsProvider).GetMethod(nameof(GameStatisticsProvider.AddSkippedNumWhenSkippedNormalNote), BindingFlags.Static | BindingFlags.NonPublic);
            harmony.Patch(addComboMiss, null, new HarmonyMethod(addComboMissPatch));
        }

        private static void AddSkippedNum(int value = 1)
        {
            skippedNum += value;
            targetWeightBySelf += 2 * value;
        }

        // Harmony patches to update statistics:
        private static void RefreshMusicDatas()
        {
            musicDatas.Clear();
            foreach (MusicData musicData in Singleton<StageBattleComponent>.instance.GetMusicData())
            {
                musicDatas.Add(musicData);
            }
            recordedMaxId = 0;
            skippedNum = 0;
            actualWeightBySelf = 0;
            targetWeightBySelf = 0;
        }

        [Fixme("Find correct patch target")]
        private static void AddSkippedNumWhenSkippedNormalNote(int combo, bool addCount)
        {
            if (combo == 0)
            {
                AddSkippedNum();
            }
        }

        private static void AddSkippedNumWhenSkippedMusicOrHeart(int result)
        {
            if (result == (int)TaskResult.None)
            {
                AddSkippedNum();
            }
        }

        private static void SetWeightsByResult(int idx, byte result, bool isMulStart, bool isMulEnd, bool isLeft)
        {
            // See Assets.Scripts.GameCore.HostComponent.TaskStageTarget.GetTrueAccuracyNew,
            // Assets.Scripts.GameCore.HostComponent.TaskStageTarget.SetPlayResult
            // and Assets.Scripts.GameCore.HostComponent.BattleEnemyManager.SetPlayResult
            // AddComboMiss -> SetPlayResult -> OnNoteResult (Not sure)
            TaskResult resultEasier = (TaskResult)result;
            if (!isMulStart)
            {
                if (resultEasier <= TaskResult.None)
                {
                    return;
                }
                MusicData musicData = Singleton<StageBattleComponent>.instance.GetMusicDataByIdx(idx);
                TaskResult playResult = (TaskResult)Singleton<BattleEnemyManager>.instance.GetPlayResult(idx);
                if (musicData.isLongPressing)
                {
                    return;
                }
                if (!musicData.noteData.addCombo)
                {
                    targetWeightBySelf += 2;
                    if (resultEasier == TaskResult.Prefect)
                    {
                        actualWeightBySelf += 2;
                    }
                }
                else if (musicData.isLongPressEnd || musicData.isLongPressStart)
                {
                    targetWeightBySelf += 2;
                    switch (resultEasier)
                    {
                        case TaskResult.Prefect:
                            actualWeightBySelf += 2;
                            break;

                        case TaskResult.Great:
                            actualWeightBySelf += 1;
                            break;
                    }
                }
                else if (playResult == resultEasier || isMulEnd || musicData.doubleIdx > 0)
                // playResult has bee recorded when run this method, that is the difference with TaskStageTarget.SetPlayResult
                {
                    if (musicData.isDouble)
                    {
                        TaskResult playResult2 = (TaskResult)Singleton<BattleEnemyManager>.instance.GetPlayResult(musicData.doubleIdx);
                        if (playResult2 != TaskResult.None)
                        {
                            targetWeightBySelf += 4;
                            if (
                                (playResult2 == TaskResult.Prefect && resultEasier == TaskResult.Great) ||
                                (playResult2 == TaskResult.Great && resultEasier == TaskResult.Prefect) ||
                                (playResult2 == TaskResult.Great && resultEasier == TaskResult.Great)
                                )
                            {
                                actualWeightBySelf += 2;
                            }
                            else if (resultEasier == TaskResult.Prefect && playResult2 == TaskResult.Prefect)
                            {
                                actualWeightBySelf += 4;
                            }
                        }
                    }
                    else
                    {
                        targetWeightBySelf += 2;
                        switch (resultEasier)
                        {
                            case TaskResult.Prefect:
                                actualWeightBySelf += 2;
                                break;

                            case TaskResult.Great:
                                actualWeightBySelf += 1;
                                break;

                            case TaskResult.Miss:
                                if (musicData.noteData.type == (uint)NoteType.Hide)
                                {
                                    skippedNum++;
                                }
                                break;
                        }
                    }
                }
            }
        }
    }
}
