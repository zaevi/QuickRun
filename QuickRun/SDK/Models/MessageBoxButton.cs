using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Forms = System.Windows.Forms;

namespace QuickRun.SDK
{
    [Config]
    public class MessageBoxButton : BaseButton
    {
        [Config("消息")]
        public string Message
        {
            get => GetValue<string>("Message");
            set => SetValue("Message", value);
        }

        public override void OnClick(object sender, RoutedEventArgs e)
        {
            Task.Run(() => Forms.MessageBox.Show(Message, "", Forms.MessageBoxButtons.OK, Forms.MessageBoxIcon.None));
        }
    }
}
