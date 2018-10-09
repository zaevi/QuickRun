using System.Windows;

namespace QuickRun.Plugin
{
    /// <summary>
    /// 用于QuickRun的可执行(Execute)插件接口
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// 执行插件
        /// </summary>
        void Execute();
    }

    /// <summary>
    /// 用于QuickRun的插件可拖拽接口
    /// </summary>
    public interface IDragDrop
    {
        /// <summary>
        /// 获取并解析拖拽相关的数据对象
        /// </summary>
        /// <param name="dragData">拖拽事件相关联的数据对象</param>
        /// <returns>数据对象是否被成功解析存储</returns>
        bool GetDragData(IDataObject dragData);
    }
}
