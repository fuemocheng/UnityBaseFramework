# Modules 模块

## 0x00 Utility.Text 字符串格式化相关处理类
1.BaseModule.Utility.Text.ITextHelper 接口限制了关于字符串格式化的处理函数；

2.BaseModule.Utility.Text 中设置 ITextHelper，并使用设置的 ITextHelper 对字符串格式化进行处理；
如果没设置ITextHelper，则使用 System.String 进行处理；

3.GameModule.DefaultTextHelper 实现 ITextHelper 接口；
在框架初始化的时候，将 DefaultTextHelper 注册到 BaseModule.Utility.Text 中；
DefaultTextHelper主要是使用 StringBuilder 提前申请好一片内存空间，
通过 StringBuilder 进行字符串的格式化拼接，以减少字符串的GC；

4.调用
直接使用 BaseModule.Utility.Text.Format<T>(string format, T arg, ...) 进行字符串格式化；

5.设计启发
在设计好 BaseModule中字符串格式化接口后，实际编码中我们只需要实现此接口，初始化时注册进通用类，就可以使用了；
接口既是对此功能入口的设计规划，也是对编码规范的限制，防止代码入口变得混乱；
对接口的实现可以有不同的方式，也就是字符串的处理也是自由的；
最终的入口和出口是固定的，在Utility.Text类中；
所以对于某个功能来说，先将接口和功能的出入口设计出来，具体的编码实现则释放出来是不错的选择；

## 0x01 Log 模块
1.BaseModule.BaseModuleLog.ILogHelper 规范了日志打印的接口；

2.BaseModule.BaseModuleLog 中设置 ILogHelper，使用ILogHelper进行日志输出；
如果 ILogHelper 为空，则不处理；

3.GameModule.DefaultLogHelper 实现了 ILogHelper 接口；
在框架初始化的时候，将 DefaultLogHelper 注册进 BaseModule.BaseModuleLog，则最终是使用 DefaultLogHelper 进行打印的；
DefaultLogHelper 则根据日志等级进行了不同种类的打印，使用的还是 UnityEngine.Debug.Log() 等等；

4.GameModule.Log
此类对 BaseModule.BaseModuleLog 进行了预编译封装，只有在某些编译条件下才能打印日志；
这样既减少了打印，也减少了打印过程中可能出现的字符串拼接格式化等问题；

5.调用
直接使用 GameModule.Log.Debug()/Info()/Warning()/Error()/Fatal() 进行日志打印输出；

6.设计启发
首先规范日志打印的接口，然后自定义的类实现打印接口，最终使用Unity的打印，还是接第三方打印，具体问题再具体分析；
在设计完整个流程后，入口固定，但是实现方式可自定义；
对于某个功能来说，先将接口和功能的出入口设计出来，具体的编码实现则是自定义；

7.扩展
如果是项目中已经使用了大量的 UnityEngine.Debug.Log，改起来比较麻烦，
可以将 Debug.unityLogger.logHandler 截取过来，再赋值自定义的 ILogHandler，
在自定的 ILogHandler 中实现自己想要的东西，以便对所有的Log进行管理；
比如第三方插件可能会有很多 UnityEngine.Debug；

## 0x02 ReferencePool 引用池

1.ReferencePool 存储结构 static
static Dictionary<Type, ReferenceCollection> s_ReferenceCollections 

获取 T Acquire<T>();
释放 Release(IReference);

2.ReferenceCollection 存储结构
Queue<IReference> m_References;

获取 T Acquire<T>();
释放 Release(IReference);

3.使用
直接使用 T ReferencePool.Acquire<T>() 获取；
直接使用 ReferencePool.Release() 释放；

4.ReferencePoolInfo
通过ReferencePoolInfo获取当前引用池信息，可将数据显示在编辑器上观察引用的情况；

## 0x03 ObjectPool

1.Object<T> 是在ObjectBase基础上又包装了一层，加了SpawnCount;

## 0x04 VersionHelper

## 0x05 CompressionHelper

## 0x06 JsonHelper

## 0x07 GameEntry BaseEntry

## 0x08 Utility
		Utility.Assembly
		Utility.Convertrt
		Utility.Encryption
		Utility.Masrshal
		Utility.Path
		Utility.Random
		Utility.Verifier.Crc32
		Utility.Verifier
		
		BinaryExtension
		StringExtension
		UnityExtension
		


