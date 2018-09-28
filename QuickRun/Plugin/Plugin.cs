using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace QuickRun.Plugin
{
    /// <summary>
    /// QuickRun的扩展插件
    /// </summary>
    public abstract class BasePlugin
    {
        /// <summary>
        /// Item中携带的执行参数
        /// </summary>
        public string Arguments { get; internal set; }

        /// <summary>
        /// 表明调用Execute后标记是否调用成功
        /// </summary>
        public bool Success { get; set; } = true;

        /// <summary>
        /// 表示此次调用是否由拖拽引发
        /// </summary>
        public bool IsDrag { get; internal set; }

        /// <summary>
        /// 默认情况下处理拖拽数据得到的文件路径列表
        /// </summary>
        public string[] FileNames { get; internal set; }

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
        public virtual bool GetDragData(IDataObject dragData)
        {
            if (!dragData.GetFormats().Contains("FileName")) return false;
                FileNames = dragData.GetData(DataFormats.FileDrop) as string[];
            return true;
        }
    }

    public class PluginAttribute : Attribute
    {
        /// <summary>
        /// 用于展示插件功能名
        /// </summary>
        public string Name { get; set; } = null;

        /// <summary>
        /// 用于Item使用Key来调用功能
        /// </summary>
        public string Key { get; set; } = null;

        public PluginAttribute(string name, string key)
        {
            Key = "$" + key.TrimStart('$');
            Name = name;
        }
    }
}
