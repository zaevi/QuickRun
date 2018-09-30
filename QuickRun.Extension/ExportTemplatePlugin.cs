using QuickRun.Plugin;
using System.IO;
using Forms = System.Windows.Forms;

namespace QuickRun.Extension
{
    static class ExportDialog
    {
        public static void Show(string filename, string content)
        {
            var dialog = new Forms.SaveFileDialog()
            {
                FileName = filename,
                Title = "导出模板",
                InitialDirectory = Path.GetDirectoryName(typeof(App).Assembly.Location)
            };
            if(dialog.ShowDialog() == Forms.DialogResult.OK)
                File.WriteAllText(dialog.FileName, content);
        }
    }

    [Plugin("导出样式模板", "$ExportStyle")]
    public class ExportStyleTemplatePlugin : NoDragPlugin
    {
        public override void Execute()
            => ExportDialog.Show("styles.xaml", Properties.Resources.styles);
    }

    [Plugin("导出配置模板", "$ExportDesign")]
    public class ExportDesignTemplatePlugin : NoDragPlugin
    {
        public override void Execute()
            => ExportDialog.Show("design.xml", Properties.Resources.design);
    }
}
