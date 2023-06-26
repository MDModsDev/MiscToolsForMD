using Il2CppAssets.Scripts.Database;
using Il2CppAssets.Scripts.PeroTools.Commons;
using Il2CppFormulaBase;
using MelonLoader;
using System.Reflection;

//TODO: GameTouchPlay.TouchResult
namespace MiscToolsForMD.SDK
{
    public class Events : MelonMod
    {
        /// <summary>
        /// Event with play result has not been modified by game.<br/>
        ///
        /// </summary>
        public static readonly BeforeResultGeneratedEvent beforeResultGenerated = InstancesManager.GetInstance<BeforeResultGeneratedEvent>();

        /// <summary>
        /// Event with play result has been modified by game.<br/>
        ///
        /// </summary>
        public static readonly AfterResultGeneratedEvent afterResultGenerated = InstancesManager.GetInstance<AfterResultGeneratedEvent>();

        /// <summary>
        /// Event that updates current chart's info.<br/>
        /// Will be triggered after <see cref="StageBattleComponent.InitGame"/> is called.<br/>
        /// So you may use it as a trigger for showing your in-game gui(s)
        /// </summary>
        public static readonly MusicInfoUpdatedEvent musicInfoUpdated = InstancesManager.GetInstance<MusicInfoUpdatedEvent>();

        public override void OnInitializeMelon()
        {
            MethodInfo? initGame = typeof(StageBattleComponent).GetMethod(nameof(StageBattleComponent.InitGame));
            MethodInfo? initGamePatch = typeof(Events).GetMethod(nameof(InitGame), BindingFlags.Static | BindingFlags.NonPublic);
            this.TryPatch("StageBattleComponent.InitGame", initGame, postfix: initGamePatch);

            foreach (Type t in Assembly.GetCallingAssembly().GetTypes().Where(t => t.GetCustomAttribute<FixmeAttribute>() != null))
            {
                AttributeChecker.Check(t);
            }
            LoggerInstance.Msg("MiscToolsForMD.Events is initialized!");
        }

        private static void InitGame()
        {
            musicInfoUpdated.InvokeCallbacks(
                new MusicDisplayInfo(
                    GlobalDataBase.dbBattleStage.selectedMusicInfo.name,
                    GlobalDataBase.dbBattleStage.selectedMusicInfo.author,
                    GlobalDataBase.dbBattleStage.selectedMusicInfo.albumJsonName,
                    GlobalDataBase.dbBattleStage.selectedMusicLevel,
                    (Difficulty)GlobalDataBase.dbBattleStage.selectedDifficulty)
            );
        }
    }
}
