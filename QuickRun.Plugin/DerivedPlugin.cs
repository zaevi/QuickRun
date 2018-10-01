using System.Linq;
using System.Windows;

namespace QuickRun.Plugin
{
    /// <summary>
    /// 自动处理文件拖入类型的QuickRun插件衍生类;
    /// 拖拽执行此类型时, 将拖拽数据转换为文件路径列表至FileNames属性中
    /// </summary>
    public abstract class FileDropPlugin : Plugin
    {
        /// <summary>
        /// 处理拖拽数据得到的文件路径列表
        /// </summary>
        public string[] FileNames { get; internal set; }

        /// <summary>
        /// 用于解析并存储拖拽相关的数据对象
        /// </summary>
        /// <param name="dragData">拖拽事件相关联的数据对象</param>
        /// <returns>解析是否成功</returns>
        public override bool GetDragData(IDataObject dragData)
        {
            if (!dragData.GetFormats().Contains("FileName")) return false;
            FileNames = dragData.GetData(DataFormats.FileDrop) as string[];
            return true;
        }
    }

    /// <summary>
    /// 不处理拖拽的Plugin衍生类
    /// </summary>
    public abstract class NoDragPlugin : Plugin
    {
        public override bool GetDragData(IDataObject dragData)
            => false;
    }
}
