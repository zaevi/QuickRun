using System.Windows;

namespace QuickRun.Plugin
{
    /// <summary>
    /// QuickRun扩展插件基类
    /// </summary>
    public abstract class Plugin
    {
        /// <summary>
        /// Item中携带的执行参数
        /// </summary>
        public string Arguments { get; set; }

        /// <summary>
        /// 表明调用Execute后标记是否调用成功
        /// </summary>
        public bool Success { get; set; } = true;

        /// <summary>
        /// 表示此次调用是否由拖拽引发
        /// </summary>
        public bool IsDrag { get; set; }

        /// <summary>
        /// 预留的结果展示接口
        /// </summary>
        object ResultData { get; set; }

        /// <summary>
        /// 执行QuickRun插件
        /// </summary>
        public abstract void Execute();

        /// <summary>
        /// 用于解析并存储拖拽相关的数据对象
        /// </summary>
        /// <param name="dragData">拖拽事件相关联的数据对象</param>
        /// <returns>解析是否成功</returns>
        public abstract bool GetDragData(IDataObject dragData);
    }
}
