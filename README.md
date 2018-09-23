# QuickRun
绝赞WIP

这是一个追求简洁的快捷启动器, 你可以将文件/应用程序/网页URL等添加至程序中; 它们实际上会变成一个个按钮, 这之后你可以对按钮的行为进行自定义, 同时也能对样式进行有限的修改.

目前版本: 0.5.5 - [更新日志](ChangeLog.md)

## 文件
- QuickRun - 程序本体
- QuickRun.Setting - 编辑器, 用来初始化+编辑+生成(Todo:弱化这玩意)

## 使用
- [下载程序](https://github.com/Zaeworks/QuickRun/releases)
- 打开编辑器 > 文件 > 打开 - 这会生成配置模板
- 编辑并保存, 最后别忘了生成 - 之后就可以正常启动QuickRun了

## 自定义
上面生成的配置模板位于`%APPDATA%\QuickRun\design.xml`, 模板如下:
```xml
<?xml version="1.0" encoding="utf-8"?>
<QuickRunSetting>
  <Item Name="QuickRun选项">
    <Item Name="打开配置目录" Uri="%APPDATA%\QuickRun\" />
    <Item Name="打开Github" Uri="https://github.com/Zaeworks/QuickRun" />
    <Item Name="&lt;返回" Type="BackButton" />
  </Item>
</QuickRunSetting>
```
你可以直接修改它, 然后通过编辑器生成`design.map.xml`和`design.xaml`.
编辑器一开始的设计目的是方便编辑启动项, 目前似乎只有拖拽功能比较方便, 编辑器反而要承担配置生成的工作, 有待优化

你可以手动修改`%APPDATA%\QuickRun\styles.xml`来配置样式, 模板如下:
```xml
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation">
    <Style TargetType="Button" >
        <Setter Property="Padding" Value="5"/>
        <Setter Property="Background" Value="White"/>
        <Setter Property="BorderThickness" Value="0"/>
    </Style>
</ResourceDictionary>
```
这个实际上就是WPF的样式配置, 此文件会作为主窗体的Resource被直接加载.

## Todo
- 拖拽支持
- 高级启动参数
- 弱化编辑器
- 样式进一步支持
- 生成优化
- 写Wiki
