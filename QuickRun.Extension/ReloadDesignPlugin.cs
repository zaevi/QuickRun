using QuickRun.Plugin;

namespace QuickRun.Extension
{
    [Plugin("$ReloadDesign", "重载配置")]
    public class ReloadDesignPlugin : IPlugin
    {
        public void Execute()
            => typeof(Main).GetMethod("Action_Reload")?.Invoke(Main.Instance, null);
    }
}
