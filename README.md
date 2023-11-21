# UnityBaseFramework

## 简介  
- 基于开源框架 [GameFramework](https://github.com/EllanJiang/GameFramework) 修改和扩展，添加详解教程；
- 在框架基础上实现一个帧同步的框架帧（包含服务器）， 基本的同步、预测、回滚、录像播放、追帧、断线重连等；

## Demo概览
- 基础同步  
    ![PVP.gif](https://github.com/fuemocheng/UnityBaseFramework/blob/main/Tutorial/Gif/PVP.gif)  
- 单机模式  
    ![ClientMode.gif](https://github.com/fuemocheng/UnityBaseFramework/blob/main/Tutorial/Gif/ClientMode.gif)  
- 逐帧查看  
    ![FrameByFrame.gif](https://github.com/fuemocheng/UnityBaseFramework/blob/main/Tutorial/Gif/FrameByFrame.gif)   
- 断线重连
    ![Reconnect.gif](https://github.com/fuemocheng/UnityBaseFramework/blob/main/Tutorial/Gif/Reconnect.gif)  

## 运行
1. 服务器  
    - Proto 生成  
        点击Unity 菜单 Tools/Generate Server Proto，即可生成服务器Proto；  
    - 打开GameServer解决方案  
        Server/GameServer/GameServer.sln；  
    - 运行  
        Server项目设为启动项，即可启动；
2. 打包资源
    - 通过扩展编辑器 BaseFramework/ResourceTools/ResourceRuleEditor 编辑要打包的资源；  
    - 通过扩展编辑器 BaseFramework/ResourceTools/ResourceBuilder 打包资源到 HFS/Output/ 目录下；
    - 配置版本信息文件，HFS/version.txt；  
        - 根据输出目录的BuildReport/xxx/BuildLog.txt将资源信息填入到version.txt中；  
        - UpdatePrefixUri下载资源的地址配置为正确的地址；
    - 启动HFS服务，HFS/hfs.exe，作为下载资源的服务器；
3. 配置
    - Assets/GameMain/Configs/Buildinfo.txt 中，CheckVersionUrl 设置为2中的资源服务器地址中的版本信息；
    - Launcher.scene 中的BaseFramework/Builtin 的EditorResourceMode 设置为资源更新模式；
4. Build
    - 打包输出；
    - 启动两个客户端即可运行Demo；

## 模块详解
1. [基础模块](https://github.com/fuemocheng/UnityBaseFramework/blob/main/Tutorial/01%20BaseModules.md)
    - [ReferencePool 引用池](https://github.com/fuemocheng/UnityBaseFramework/blob/main/Tutorial/01%20BaseModules.md#0x00-referencepool-%E5%BC%95%E7%94%A8%E6%B1%A0)
    - [EventArgs 事件数据类](https://github.com/fuemocheng/UnityBaseFramework/blob/main/Tutorial/01%20BaseModules.md#0x01-eventargs-%E4%BA%8B%E4%BB%B6%E6%95%B0%E6%8D%AE%E7%B1%BB)
    - [自定义存储结构](https://github.com/fuemocheng/UnityBaseFramework/blob/main/Tutorial/01%20BaseModules.md#0x02-%E8%87%AA%E5%AE%9A%E4%B9%89%E5%AD%98%E5%82%A8%E7%BB%93%E6%9E%84)
        - [BaseFrameworkLinkedList\<T> 链表](https://github.com/fuemocheng/UnityBaseFramework/blob/main/Tutorial/01%20BaseModules.md#baseframeworklinkedlistt-%E9%93%BE%E8%A1%A8)
        - [BaseFrameworkMultiDictionary\<TKey, TValue> 多值字典类](https://github.com/fuemocheng/UnityBaseFramework/blob/main/Tutorial/01%20BaseModules.md#baseframeworkmultidictionarytkey-tvalue-%E5%A4%9A%E5%80%BC%E5%AD%97%E5%85%B8%E7%B1%BB)
    - [Utility.Text 字符串格式化相关处理类](https://github.com/fuemocheng/UnityBaseFramework/blob/main/Tutorial/01%20BaseModules.md#0x03-utilitytext-%E5%AD%97%E7%AC%A6%E4%B8%B2%E6%A0%BC%E5%BC%8F%E5%8C%96%E7%9B%B8%E5%85%B3%E5%A4%84%E7%90%86%E7%B1%BB)
    - [Log 模块](https://github.com/fuemocheng/UnityBaseFramework/blob/main/Tutorial/01%20BaseModules.md#0x04-log-%E6%A8%A1%E5%9D%97)
    - [ObjectPool](https://github.com/fuemocheng/UnityBaseFramework/blob/main/Tutorial/01%20BaseModules.md#0x05-objectpool)
    - [VersionHelper](https://github.com/fuemocheng/UnityBaseFramework/blob/main/Tutorial/01%20BaseModules.md#0x06-versionhelper)
    - [CompressionHelper](https://github.com/fuemocheng/UnityBaseFramework/blob/main/Tutorial/01%20BaseModules.md#0x07-compressionhelper)
    - [JsonHelper](https://github.com/fuemocheng/UnityBaseFramework/blob/main/Tutorial/01%20BaseModules.md#0x08-jsonhelper)
    - [Entry](https://github.com/fuemocheng/UnityBaseFramework/blob/main/Tutorial/01%20BaseModules.md#0x09-entry)
    - [Utility](https://github.com/fuemocheng/UnityBaseFramework/blob/main/Tutorial/01%20BaseModules.md#0x0a-utility)
    - [Event](https://github.com/fuemocheng/UnityBaseFramework/blob/main/Tutorial/01%20BaseModules.md#0x0b-event)
    - [Task](https://github.com/fuemocheng/UnityBaseFramework/blob/main/Tutorial/01%20BaseModules.md#0x0c-task)

2. [WebRequest 模块](https://github.com/fuemocheng/UnityBaseFramework/blob/main/Tutorial/02%20WebRequest.md)
   
3. [Download 模块](https://github.com/fuemocheng/UnityBaseFramework/blob/main/Tutorial/03%20Download.md)
   
4. [Network 模块](https://github.com/fuemocheng/UnityBaseFramework/blob/main/Tutorial/04%20Network.md)
   
5. [FileSystem 模块](https://github.com/fuemocheng/UnityBaseFramework/blob/main/Tutorial/05%20FileSystem.md)
   
6. [Resource 模块](https://github.com/fuemocheng/UnityBaseFramework/blob/main/Tutorial/06%2000%20Resource.md)  
   [ResourceTools 打包构建模块](https://github.com/fuemocheng/UnityBaseFramework/blob/main/Tutorial/06%2001%20ResourceTools.md)

7. [Config 模块](https://github.com/fuemocheng/UnityBaseFramework/blob/main/Tutorial/07%20Config.md)
   
8. [DataNode 模块](https://github.com/fuemocheng/UnityBaseFramework/blob/main/Tutorial/08%20DataNode.md)
   
9. [DataTable 模块](https://github.com/fuemocheng/UnityBaseFramework/blob/main/Tutorial/09%20DataTable.md)
    
10. [Setting 模块](https://github.com/fuemocheng/UnityBaseFramework/blob/main/Tutorial/10%20Setting.md)
    
11. [FSM 模块](https://github.com/fuemocheng/UnityBaseFramework/blob/main/Tutorial/11%20FSM.md)
    
12. [Procedure 模块](https://github.com/fuemocheng/UnityBaseFramework/blob/main/Tutorial/12%20Procedure.md)
    
13. [UI 模块](https://github.com/fuemocheng/UnityBaseFramework/blob/main/Tutorial/13%20UI.md)
    
14. [Scene 模块](https://github.com/fuemocheng/UnityBaseFramework/blob/main/Tutorial/14%20Scene.md)
    
15. [Sound 模块](https://github.com/fuemocheng/UnityBaseFramework/blob/main/Tutorial/15%20Sound.md)


## 感谢
[GameFramework](https://github.com/EllanJiang/UnityGameFramework)  
[LockstepEngine](https://github.com/JiepengTan/LockstepEngine)  
[ET](https://github.com/egametang/ET)  
