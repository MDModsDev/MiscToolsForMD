using Il2CppAssets.Scripts.Database;
using Il2CppAssets.Scripts.PeroTools.Commons;
using Il2CppFormulaBase;
using Il2CppGameLogic;
using MelonLoader;
using System.Reflection;
using System.Text;
using System.Text.Json;

//TODO: GameTouchPlay.TouchResult
namespace MiscToolsForMD.SDK
{
    public class Events : MelonMod
    {
        /// <summary>
        /// Event that updates current chart's info.<br/>
        /// Will be triggered after <see cref="StageBattleComponent.InitGame"/> is called.
        /// </summary>
        public delegate void MusicInfoUpdatedEventHandler(MusicDisplayInfo info);

        /// <summary>
        /// Event handler for <see cref="MusicInfoUpdatedEventHandler"/>
        /// </summary>
        private static event MusicInfoUpdatedEventHandler MusicInfoUpdated = new(
            delegate (MusicDisplayInfo displayInfo)
            {
                SDKLogger.Debug(
                    string.Format(
                        "[{0}]: Calling callbacks with argument {1}",
                        DiagnosticUtils.GetCallerFullName(),
                        Encoding.UTF8.GetString(JsonSerializer.SerializeToUtf8Bytes(displayInfo, jsonSerializeOptions))
                    )
                );
            }
        );

        /// <summary>
        /// Event with play result has not been modified by game.<br/>
        /// Will be triggered before <see cref="GameTouchPlay.TouchResult(int, byte, uint, TimeNodeOrder, bool)"/> is called.
        /// </summary>
        public delegate void BeforeResultGeneratedEventHandler(ref byte result, PlayResultInfo resultInfo);

        /// <summary>
        /// Event handler for <see cref="BeforeResultGeneratedEventHandler"/>
        /// </summary>
        private static event BeforeResultGeneratedEventHandler BeforeResultGenerated = new(
            delegate (ref byte result, PlayResultInfo resultInfo)
            {
                SDKLogger.Debug(
                    string.Format(
                        "[{0}]: Calling callbacks with argument {1}",
                        DiagnosticUtils.GetCallerFullName(),
                        Encoding.UTF8.GetString(JsonSerializer.SerializeToUtf8Bytes(resultInfo, jsonSerializeOptions))
                    )
                );
            }
        );

        /// <summary>
        /// Event with play result has been modified by game.<br/>
        /// Will be triggered after <see cref="GameTouchPlay.TouchResult(int, byte, uint, TimeNodeOrder, bool)"/> is called.
        /// </summary>
        public delegate void AfterResultGeneratedEventHandler(ref byte result, PlayResultInfo resultInfo);

        /// <summary>
        /// Event handler for <see cref="AfterResultGeneratedEventHandler"/>
        /// </summary>
        private static event AfterResultGeneratedEventHandler AfterResultGenerated = new(
            delegate (ref byte result, PlayResultInfo resultInfo)
            {
                SDKLogger.Debug(
                    string.Format(
                        "[{0}]: Calling callbacks with argument {1}",
                        DiagnosticUtils.GetCallerFullName(),
                        Encoding.UTF8.GetString(JsonSerializer.SerializeToUtf8Bytes(resultInfo, jsonSerializeOptions))
                    )
                );
            }
        );

        /// <summary>
        /// JSON serialize option for printing objects
        /// </summary>
        private static readonly JsonSerializerOptions jsonSerializeOptions = new()
        {
            WriteIndented = true,
        };

        public override void OnInitializeMelon()
        {
            MethodInfo? initGame = typeof(StageBattleComponent).GetMethod(nameof(StageBattleComponent.InitGame));
            MethodInfo? postInitGamePatch = typeof(Events).GetMethod(nameof(PostInitGame), BindingFlags.Static | BindingFlags.NonPublic);
            this.TryPatch("StageBattleComponent.InitGame", initGame, postfix: postInitGamePatch);

            MethodInfo? touchResult = typeof(GameTouchPlay).GetMethod(nameof(GameTouchPlay.TouchResult));
            MethodInfo? preTouchResultPatch = typeof(Events).GetMethod(nameof(PreTouchResult), BindingFlags.Static | BindingFlags.NonPublic);
            MethodInfo? postTouchResultPatch = typeof(Events).GetMethod(nameof(PostTouchRestlt), BindingFlags.Static | BindingFlags.NonPublic);
            this.TryPatch("GameTouchPlay.TouchResult", touchResult, prefix: preTouchResultPatch, postfix: postTouchResultPatch);

            foreach (Type t in Assembly.GetCallingAssembly().GetTypes().Where(t => t.GetCustomAttribute<FixmeAttribute>() != null))
            {
                AttributeChecker.Check(t);
            }
            LoggerInstance.Msg("MiscToolsForMD.Events is initialized!");
        }

        /// <summary>
        /// Add a callback for <see cref="MusicInfoUpdated"/> event
        /// </summary>
        /// <param name="eventCallback">The callback delegate</param>
        public static void AddCallback(MusicInfoUpdatedEventHandler eventCallback) =>
            MusicInfoUpdated += delegate (MusicDisplayInfo displayInfo)
            {
                try { eventCallback(displayInfo); }
                catch (Exception) { }
            };

        /// <summary>
        /// Add a callback for <see cref="BeforeResultGenerated"/> event
        /// </summary>
        /// <param name="eventCallback">The callback delegate</param>
        public static void AddCallback(BeforeResultGeneratedEventHandler eventCallback) =>
            BeforeResultGenerated += delegate (ref byte result, PlayResultInfo resultInfo)
            {
                try { eventCallback(ref result, resultInfo); }
                catch (Exception) { }
            };

        /// <summary>
        /// Add a callback for <see cref="AfterResultGenerated"/> event
        /// </summary>
        /// <param name="eventCallback">The callback delegate</param>
        public static void AddCallback(AfterResultGeneratedEventHandler eventCallback) =>
            AfterResultGenerated += delegate (ref byte result, PlayResultInfo resultInfo)
            {
                try { eventCallback(ref result, resultInfo); }
                catch (Exception) { }
            };

        private static void PostInitGame()
        {
            MusicInfoUpdated(
                new MusicDisplayInfo(
                    GlobalDataBase.dbBattleStage.selectedMusicInfo.name,
                    GlobalDataBase.dbBattleStage.selectedMusicInfo.author,
                    GlobalDataBase.dbBattleStage.selectedMusicInfo.albumJsonName,
                    GlobalDataBase.dbBattleStage.selectedMusicLevel,
                    (Difficulty)GlobalDataBase.dbBattleStage.selectedDifficulty)
            );
        }

        private static bool PreTouchResult(int idx, ref byte resultCode, uint actionType, TimeNodeOrder? tno = null, bool isSkill = false)
        {
            return true;
        }

        private static void PostTouchRestlt(int idx, ref byte resultCode, uint actionType, TimeNodeOrder? tno = null, bool isSkill = false)
        {
        }
    }
}
