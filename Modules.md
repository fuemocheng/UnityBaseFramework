# Modules 模块

## 0x00 IReference 引用接口
1. void IReference.Clear();
2. 所有可以被回收重用的都需要实现接口Clear();

### ReferencePool 引用池
1. ReferencePool 存储结构 static Dictionary
   	- 类型-队列，每个类型对应一个队列存储；
	- static Dictionary\<Type, ReferenceCollection>() s_ReferenceCollections;
	- 获取，从队列获取：Acquire\<T>() -> ReferenceCollection.Acquire\<T>();
	- 释放，释放到队列：Release(IReference) -> ReferenceCollection.Release(IReference);

2. ReferenceCollection 存储结构 Queue
    - 使用队列存储；
	- Queue\<IReference> m_References;
	- 获取，复用队列中的或者新建：T Acquire\<T>() -> (T)m_References.Dequeue() or new T();
	- 释放，释放到队列： Release(IReference) -> m_References.Enqueue(IReference);
	- UsingCount、AcquireCount、ReleaseeCount、AddCount、RemoveCount;

3. ReferencePool 使用
	- 直接使用 T ReferencePool.Acquire\<T>() 获取；
	- 直接使用 ReferencePool.Release(IReference) 释放；

4. ReferencePoolInfo
	- 通过ReferencePoolInfo获取当前引用池信息，可将数据显示在编辑器上观察引用的情况；

## 0x01 EventArgs 事件数据类
1. System 命令空间下;
   - System.EventArgs;
2. public class EventArgs
   - public static readonly EventArgs Empty;
   - public EventArgs();

### BaseFrameworkEventArgs 事件数据的类的基类
1. BaseFramework 命令空间下
   - BaseFramework.BaseFrameworkEventArgs;
2. BaseFrameworkEventArgs : EventArgs, IReference
   - abstract class，抽象类;
   - public abstract void Clear()，实现Clear接口；

### BaseEventArgs 事件数据抽象类
1. BaseFramework 命令空间下
   - BaseFramework.BaseEventArgs;
2. BaseEventArgs : BaseFrameworkEventArgs
   - abstract class，抽象类；
   - abstract int Id，获取类型编号；
3. EventPool 事件池使用的是 BaseEventArgs 作为事件数据基类
   
### GameEventArgs 
1. BaseFramework.Event 命令空间下
   - BaseFramework.Event.GameEventArgs;
2. GameEventArgs : BaseEventArgs
   - abstract class，抽象类；
   - 游戏逻辑事件数据基类；
   - BaseFramework/Libraries 之外的使用GameEventArgs作为事件数据类；
   - EventManager中的EventPool使用GameEventArgs作为事件数据基类；
		- EventPool\<GameEventArgs> m_EventPool;
3. 示例
   - WebRequestStartEventArgs : GameEventArgs;
   - WebRequestSuccessEventArgs : GameEventArgs;

## 0x02 自定义存储结构
1. C#中的双向链表
   1. System.Collections.Generic.LinkedList\<T> 双向链表
      - AddAfter(LinkedListNode<T> node, T value);
      - AddBefore(LinkedListNode<T> node, T value);
      - AddFirst(T value);
      - AddLast(T value);
      - ...
   2. System.Collections.Generic.LinkedListNode\<T> 双向链表的节点
      - LinkedList<T> List;
      - LinkedListNode<T> Next;
      - LinkedListNode<T> Previous;
      - T Value;

### BaseFrameworkLinkedList\<T> 链表
1. 存储结构
   - LinkedList\<T> m_LinkedList，双向链表；
   - Queue\<LinkedListNode\<T>> m_CachedNodes，缓存节点队列；
   - 数据存储在链表 LinkedList\<T> 里，同时维护一份缓存节点队列Queue\<LinkedListNode\<T>> m_CachedNodes;
2. 添加节点时，先判断缓存队列是否有节点，有的话用缓存队列中的；
3. 删除节点时，从m_LinkedList中删除，还原默认值，添加到缓存队列中；
4. 缓存队列需要手动清理；
5. 实现迭代器；

总结：比传统的链表增加了缓存队列，在频繁创建节点删除节点的链表中，具有优势；

### BaseFrameworkLinkedListRange\<T> 链表范围结构体
1. struct BaseFrameworkLinkedListRange\<T> 存储结构
   - LinkedListNode\<T> m_First;
   - LinkedListNode\<T> m_Terminal;
2. 只记录两个节点，第一个和最后一个；
3. 实现First，Terminal，IsValid，Count，Contains()；
4. 实现迭代器；

总结：为了实现多值字典类而定义的结构体；

### BaseFrameworkMultiDictionary\<TKey, TValue> 多值字典类
1. 存储结构
   - BaseFrameworkLinkedList\<TValue> m_LinkedList;
   - Dictionary\<TKey, BaseFrameworkLinkedListRange\<TValue>> m_Dictionary;
   - 数据存储在 m_LinkedList 链表中；
   - 维护一个 m_Dictionary 字典，来记录每个键TKey在链表的开始结束位置，方便查询获取；
2. 添加时，查找字典中此键TKey对应的的最后一个值，然后添加到此值之前；
3. 删除时，需要判断是否是Range的第一个值，需要特殊判断，然后m_LinkedList中删除即可；
4. 实现迭代器；

总结：所有数据存储在同一个链表中，通过维护一份额外的字典来确定每个键TKey的所对应值在链表中的存储范围；

## 0x03 Utility.Text 字符串格式化相关处理类
1. BaseFramework.Utility.Text.ITextHelper 接口限制了关于字符串格式化的处理函数；
2. BaseFramework.Utility.Text 中设置 ITextHelper，并使用设置的 ITextHelper 对字符串格式化进行处理；
	- 如果没设置ITextHelper，则使用 System.String 进行处理；
3. UnityBaseFramework.Runtime.DefaultTextHelper 实现 ITextHelper 接口；
	- 在框架初始化的时候，将 DefaultTextHelper 注册到 BaseFramework.Utility.Text 中；
	- DefaultTextHelper主要是使用 StringBuilder 提前申请好一片内存空间，
	- 通过 StringBuilder 进行字符串的格式化拼接，以减少字符串的GC；
4. 调用
	- 直接使用 BaseFramework.Utility.Text.Format\<T>(string format, T arg, ...) 进行字符串格式化；
5. 设计启发
	- 在设计好 BaseFramework 中字符串格式化接口后，实际编码中我们只需要实现此接口，初始化时注册进通用类，就可以使用了；
	- 接口既是对此功能入口的设计规划，也是对编码规范的限制，防止代码入口变得混乱；
	- 对接口的实现可以有不同的方式，也就是字符串的处理也是自由的；
	- 最终的入口和出口是固定的，在Utility.Text类中；
	- 所以对于某个功能来说，先将接口和功能的出入口设计出来，具体的编码实现则释放出来是不错的选择；

## 0x04 Log 模块
1. static BaseFramework.BaseFrameworkLog 静态类；
2. BaseFramework.BaseFrameworkLog.ILogHelper 规范了日志打印的接口；
3. BaseFramework.BaseFrameworkLog 中设置 ILogHelper，使用ILogHelper进行日志输出；
	- 如果 ILogHelper 为空，则不处理；
4. UnityBaseFramework.Runtime.DefaultLogHelper 实现了 ILogHelper 接口；
	- 在框架初始化的时候，将 DefaultLogHelper 注册进 BaseFramework.BaseFrameworkLog，则最终是使用 DefaultLogHelper 进行打印的；
	- DefaultLogHelper 则根据日志等级进行了不同种类的打印，使用的还是 UnityEngine.Debug.Log() 等等；
5. UnityBaseFramework.Runtime.Log
	- 此类对 BaseFramework.BaseFrameworkLog 进行了预编译封装，只有在某些编译条件下才能打印日志；
	- 这样既减少了打印，也减少了打印过程中可能出现的字符串拼接格式化等问题；
6. 调用
	- 直接使用 UnityBaseFramework.Runtime.Log.Debug()/Info()/Warning()/Error()/Fatal() 进行日志打印输出；
7. 设计启发
	- 首先规范日志打印的接口，然后自定义的类实现打印接口，最终使用Unity的打印，还是接第三方打印，具体问题再具体分析；
	- 在设计完整个流程后，入口固定，但是实现方式可自定义；
	- 对于某个功能来说，先将接口和功能的出入口设计出来，具体的编码实现则是自定义；
8. 扩展
	- 如果是项目中已经使用了大量的 UnityEngine.Debug.Log，改起来比较麻烦，可以将 Debug.unityLogger.logHandler 截取过来，再赋值自定义的 ILogHandler，在自定的 ILogHandler 中实现自己想要的东西，以便对所有的Log进行管理；比如第三方插件可能会有很多 UnityEngine.Debug；

## 0x05 ObjectPool
1. Object<T> 是在ObjectBase基础上又包装了一层，加了SpawnCount;

## 0x06 VersionHelper

## 0x07 CompressionHelper

## 0x08 JsonHelper

## 0x09 GameEntry BaseEntry

## 0x0A Utility
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
		
## 0x0B Event

## 0x0C Task

## 0x0D WebRequest

## 0x0E 

## 0x0F

		
		


