# BaseModules 基础模块

## 0x00 ReferencePool 引用池
- 引用池一般用来储存普通的C#类型对象；
- 适用于高频复用的对象；

### IReference 引用接口
- void IReference.Clear();
- Clear() 会在对象回收时被调用，每一个需要被引用池储存的类型都需要实现此接口，以能清空当前状态，恢复到初始状态，供下次使用；

### ReferenceCollection 引用的容器类
1. 一个类型所对应的一个引用池的存储类，ReferencePool类的私有内部类；
2. 存储结构
    - Queue\<IReference> m_References;
    - 使用Queue类型存储；
3. Type m_ReferenceType
	- 该引用池持有对象的类型；
4. Acquire\<T>() 获取
    - 复用队列中的或者新建，Acquire\<T>() -> (T)m_References.Dequeue() or new T();
5. Release(IReference reference) 释放
    - 先调用reference.Clear()，再释放到队列：Release(reference) -> reference.Clear() -> m_References.Enqueue(reference);
6. 关于数量的属性
    - UnusedReferenceCount:目前池子可用的数量（剩余可被取出的数量）；
    - UsingReferenceCount:被取出未归还的数量；
    - AcquireReferenceCount:请求获取的次数；
    - ReleaseReferenceCount:释放的次数；
    - AddReferenceCount:实际实例化次数；
    - RemoveReferenceCount:主动移除次数；
    - 这些数据会在调用引用池相应接口时进行计算，外部可获取这些数据进行Debug；

### ReferencePool 引用池
1. 静态类，负责管理所有类型的引用池，也是外部访问引用池的入口；
2. 存储结构 static Dictionary
    - static Dictionary\<Type, ReferenceCollection>() s_ReferenceCollections;
    - 用处存储所有所有引用池实例；
3. Acquire<T>() 获取
  	- Acquire\<T>() -> ReferenceCollection.Acquire\<T>();
    - 调用到内部ReferenceCollection的Acquire接口；
4. void Release(IReference reference)
	- Release(reference) -> ReferenceCollection.Release(reference);
	- 调用到内部ReferenceCollection的Release接口；
5. 使用
    - 直接使用 T ReferencePool.Acquire\<T>() 获取；
    - 直接使用 ReferencePool.Release(IReference) 释放；
6. ReferencePoolInfo 引用池信息
	- 结构体类型
    - 通过GetAllReferencePoolInfos()获取所有引用池的信息，可将数据显示在编辑器上观察引用的情况；

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
3. 使用范围
    - Assets/BaseFramework/Libraries/... 框架之内的使用BaseEventArgs作为事件数据类；
    - EventPool (Assets/BaseFramework/Libraries/BaseFramework/Base/EventPool/EventPool.cs) 事件池使用的是 BaseEventArgs 作为事件数据基类
   
### GameEventArgs 
1. BaseFramework.Event 命令空间下
    - BaseFramework.Event.GameEventArgs;
2. GameEventArgs : BaseEventArgs
    - abstract class，抽象类；
    - 游戏逻辑事件数据基类；
3. 使用范围
    - Assets/BaseFramework/Libraries 框架之外的使用GameEventArgs作为事件数据类；
    - Assets/BaseFramework/Scripts/Runtime/...
    - Assets/GameMain/Scripts/...
    - EventManager中的EventPool使用GameEventArgs作为事件数据基类；
		- EventPool\<GameEventArgs> m_EventPool;
4. 示例
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
    - 维护一个 m_Dictionary 字典，来记录每个键TKey在链表的开始结束节点，以此增删改查；
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
	- DefaultTextHelper主要是使用 StringBuilder 提前申请好一片内存空间， 通过 StringBuilder 进行字符串的格式化拼接，以减少字符串的GC；
4. 调用
	- BaseFramework.Utility.Text.Format\<T>(string format, T arg, ...) 字符串格式化；
5. 总结
	- 在设计好 BaseFramework 中字符串格式化接口后，实际编码中我们只需要实现此接口，初始化时注册进通用类，就可以使用了；
	- 接口既是对此功能入口的设计规划，也是对编码规范的限制，防止代码入口变得混乱；
	- 对接口的实现可以有不同的方式，也就是字符串的处理也是自由的；
	- 最终的入口和出口是固定的，在Utility.Text类中；
	- 所以对于某个功能来说，先将接口和功能的出入口设计出来，具体的编码实现则释放出来是不错的选择；

## 0x04 Log 模块
1. static BaseFramework.BaseFrameworkLog 静态类；
2. BaseFramework.BaseFrameworkLog.ILogHelper 规范了日志打印的接口；
3. BaseFramework.BaseFrameworkLog 中设置 ILogHelper，使用ILogHelper进行日志输出；
	- 如果 ILogHelper 为空，则不打印日志；
4. UnityBaseFramework.Runtime.DefaultLogHelper 实现了 ILogHelper 接口；
	- 在框架初始化的时候，将 DefaultLogHelper 注册进 BaseFramework.BaseFrameworkLog，则最终使用 DefaultLogHelper 进行打印；
	- DefaultLogHelper 则根据日志等级进行了不同种类的打印，使用的还是 UnityEngine.Debug.Log() 等等；
5. UnityBaseFramework.Runtime.Log
	- 此类对 BaseFramework.BaseFrameworkLog 进行了预编译封装，只有在某些编译条件下才能打印日志；
	- 这样既减少了打印，也减少了打印过程中可能出现的字符串拼接格式化等问题；
6. 调用
	- UnityBaseFramework.Runtime.Log.Debug()/Info()/Warning()/Error()/Fatal() 日志打印输出；
7. 总结
	- 首先规范日志打印的接口，然后自定义的类实现打印接口，最终使用Unity的打印，还是接第三方打印，具体问题再具体分析；
	- 在设计完整个流程后，入口固定，但是实现方式可自定义；
	- 对于某个功能来说，先将接口和功能的出入口设计出来，具体的编码实现则是自定义；
8. 扩展
	- 如果是项目中已经使用了大量的 UnityEngine.Debug.Log，改起来比较麻烦，可以将 Debug.unityLogger.logHandler 截取过来，再赋值自定义的 ILogHandler，在自定的 ILogHandler 中实现自己想要的东西，以便对所有的Log进行管理；比如第三方插件可能会有很多 UnityEngine.Debug；

## 0x05 ObjectPool
1. ObjectBase 对象基类
    1. ObjectBase : IReference
       - BaseFramework.ObjectPool 命名空间；
       - abstract class 抽象类；
    2. 结构
		- string Name; 对象名称；
		- object Target; 存储的对象；
		- bool Locked; 对象是否被加锁；
		- int Priority; 对象的优先级；
		- bool CustomCanReleaseFlag; 自定义释放检查标记；
		- DateTime LastUseTime; 上次使用时间；
    3. Function
		- void Initialize(string name, object target, bool locked, int priority) 初始化对象;
        - void Clear() 清理对象；
		- void OnSpawn() 获取对象时的事件；
		- void OnUnspawn() 回收对象时的事件；
		- abstract void Release(bool isShutdown) 释放对象；
    4. 总结
		- 要存储的对象存储在 Target 中；
		- ObjectBase实现IReference接口，可以复用；同时在外部获得其子类时应该从引用池获取；

2. Object\<T>
	1. Object\<T>: IReference where T : ObjectBase
        - BaseFramework.ObjectPool.ObjectPoolManager 命名空间；
		- 属于ObjectPoolManager内部私有类；
	2. 结构
		- private T m_Object; 存储的是 ObjectBase；
		- private int m_SpawnCount; 对象的获取计数；
		- string Name; 名称；
		- bool Locked; 是否被加锁；
		- int Priority; 优先级；
		- bool CustomCanReleaseFlag; 自定义释放检查标记；
		- DateTime LastUseTime; 对象上次使用时间；
		- bool IsInUse; 对象是否正在使用；
		- int SpawnCount; 对象的获取计数；
	3. Function
		- public static Object<T> Create(T obj, bool spawned) 创建内部对象；
		- void Clear(); 清理内部对象，m_Object设为null，m_SpawnCount设为0；
		- T Peek(); 查看对象，返回m_Object；
		- T Spawn(); 获取对象；
			- m_SpawnCount++;
            - m_Object.LastUseTime = DateTime.UtcNow;
            - m_Object.OnSpawn();
            - 引用计数加1，记录时间，调用ObjectBase的OnSpawn()；
		- void Unspawn(); 回收对象；
			- m_Object.OnUnspawn();
            - m_Object.LastUseTime = DateTime.UtcNow;
        	- m_SpawnCount--;
        	- 调用ObjectBase的OnUnspawn()，记录时间，引用计数减1；
		- void Release(bool isShutdown); 释放对象；
			- m_Object.Release(isShutdown);
             	- 自定义的释放逻辑； 
            - ReferencePool.Release(m_Object);
				- 先m_Object.Clear()；
				- 再添加到ReferencePool中的缓存中；
    4. 总结
		- Object\<T>同样实现了IReference接口，但对象池内部已经自行管理，会在注册对象时向引用池获取，外部无需关心，且Object是ObjectPoolManager的内部私有类，外部并不知晓它的存在；
		- ObjectBase对象不是对象池直接储存的对象，只是间接对象；对象池直接储存的是泛型类Object\<T>的对象；
		- Object\<T>泛型参数T约束ObjectBase的类型；
      	- Object\<T>类内大部分属性是直接返回ObjectBase对象的对应属性，除此之外Object\<T>会记录目标对象的是否正在使用状态以及获取计数；

3. ObjectPool\<T>
	1. ObjectPool\<T> :  ObjectPoolBase, IObjectPool\<T> where T : ObjectBase
		- BaseFramework.ObjectPool.ObjectPoolManager 命名空间；
		- 属于ObjectPoolManager内部私有类；
	2. 存储结构
		- BaseFrameworkMultiDictionary\<string, Object\<T>> m_Objects;
    		- 以Object\<T>的Name为Key，拥有相同Name的Object\<T>集合为Value；
		- Dictionary<object, Object\<T>> m_ObjectMap;
    		- 以目标对象（ObjectBase里的Target）为Key，Object\<T>为Value；
	3. AllowMultiSpawn
		- 把对象池分为两种类型，一种是允许对象被多次获取，另一种是不允许；
		- 两者区别在于，如果允许对象被多次获取，那么即使一个对象已经处于被使用状态时（即上一次获取后还没归还对象池），仍然可以再次获取，显然一般情况下是不允许这种做法的。在GF的资源模块中会使用允许对象被多次获取的对象池来管理资源对象，因为资源对象我们只需要其在内存中存在一份。这个属性会在创建对象池时从参数带入，创建对象池后无法再改变；
	4. function
		- void Register(T obj, bool spawned) 创建对象；
			- 创建对象或者注册对象，参数类型T为ObjectBase；
			- 通过 Object\<T>.Create(obj, spawned) 创建，Object\<T>通过引用池获取；
			- 添加到m_Objects和m_ObjectMap中；
  		- T Spawn(string name) 获取对象
    		- 参数为Object\<T>的Name；
    		- 如果m_Objects中存在这个key，则取出对应的Object\<T>的集合，并检查其中是否有可用的，若存在可用的，就调用Object\<T>的Spawn()->ObjectBase的OnSpawn()，最后返回ObjectBase的子类；
  		- Unspawn(object target) 回收对象
    		- 参数为ObjectBase中的Target；
    		- 如果对象没有通过注册Register过，也就是不存在字典m_ObjectMap中，会抛出错误；
    		- 如果m_ObjectMap存在这个key，则会调用Object\<T>的Unspawn->ObjectBase的OnUnSpawn()，完成回收逻辑；
		- GetAllObjectInfos()
    		- 返回ObjectInfo结构体数组，包含对象池内所有物体的信息，包括名字、锁定状态、自定义释放检查标记、优先级、使用状态、上次使用时间、获取计数、是否处于使用中状态；
	5. 自动释放
		- 对象池具有自动释放对象的功能，总的来说每过一段时间会调用一次Release执行释放逻辑，这个时间由AutoReleaseInterval属性决定，每个对象池可以有各自不一样的释放时间间隔；Release过程会先获取可释放对象序列，然后通过委托ReleaseObjectFilterCallback对可释放物体序列进行筛选后，最后仅对筛选后的对象调用ReleaseObject进行释放；
		- Release方法就是释放过程的主要逻辑，先调用GetCanReleaseObjects获取可释放对象序列，然后用releaseObjectFilterCallback对序列进行筛选，最后对筛选后的对象逐个调用ReleaseObject进行释放；
		- GetCanReleaseObjects方法获取当前可进行释放的对象，会遍历m_ObjectMap的Value值，对于在处于非使用中状态、非锁定状态、以及自定义释放标记为True时，才被认为是可释放对象；
		- DefaultReleaseObjectFilterCallback是ReleaseObjectFilterCallback委托类型方法，这个方法负责从可释放对象序列中进一步选出符合要求的对象，之后再进行释放。DefaultReleaseObjectFilterCallback是对象池内部的默认实现，我们也可以在构造对象池时传入自定义的方法，根据自定义的逻辑进行筛选；
		- ReleaseObject内部会把对应的Object\<T>从m_Objects和m_ObjectMap中移除，调用到ObjectBase的Release（如果目标对象是GameObject，对Release的重写应该是GameObject.Destroy相关操作），最后把ObjectBase子类对象和Object对象归还引用池；（ReleaseObject方法参数是应该传入目标物体，也就是ObjectBase中的Target，ReleaseObject方法还有一个重载是传入ObjectBase对象）;
    		- 除了Release方法、对象池还提供了ReleaseAllUnused该方法会直接释放所有可释放对象，而不经过筛选。
			- 对象池的ExpireTime属性决定了对象池里所有对象的过期时间，对象池每过一段间隔时间，就会自动执行释放，根据DefaultReleaseObjectFilterCallback的实现，执行释放时会优先获取距对象最后一次使用时间的时长大于过期时间的对象。另外如果执行ReleaseAllUnused，会无视这一过期规则，只要被认为是可释放对象，都会进行回收。
			- SetLocked方法提供锁定某一对象的功能，即使对象处于未被使用的状态也不会被认为是可释放对象。
			- CustomCanReleaseFlag提供自定义释放检查标记功能，CustomCanReleaseFlag是一个虚属性，默认返回True，也就是对象默认是依赖IsInUse这一状态来判断是否使用中，来判断是否能释放，而IsInUse这一属性是由Spawn与Unspawn的计数来判断的，当这种计数方式不适用的情况下，我们可以重写CustomCanReleaseFlag，自定义逻辑判定是否可释放。

4. ObjectPoolBase与IObjectPool
	1. ObjectPoolBase是个抽象类，IObjectPool是一个泛型接口；
	2. 从ObjectPoolBase和IObjectPool的成员可以看到，他们的内容有大部分重叠，IObjectPool涵盖了ObjectPoolBase的绝大部分成员，而对象池类ObjectPool类同时继承ObjectPoolBase并实现IObjectPool接口；
	3. 为何设计大部分内容相同的一个抽象类、和一个接口，然后最终只由一个类去继承和实现它们：
		- 其实关键点是在于IObjectPool是一个泛型接口，而ObjectPoolBase不是泛型；
		- 当我们需要同时获取多个不同对象池的一些通用数据时，我们可以以ObjectPoolBase[]的形式获取到不同的对象池集合，并获取它们各自的名字、数量等数据；
		- 而IObjectPool则明确清楚某个池子储存的对象的类型，且具有编译时类型安全，所以Register、Spawn、Unspawn等需要关心目标物体类型的方法仅在IObjectPool接口中声明；
		- 我们可以在不同的情况下，以不同的类型持有对象池，如ObjectPoolManager、以及UGF中用以在Inspector面板显示对象池数据的ObjectPoolComponentInspector，都是以 ObjectPoolBase 集合的形式持有对象池；
		- 而UIManager、EntityGroup中则以 IObjectPool 形式持有；
		- 显然前者更适合处理集合通用逻辑，而后者适合处理对具体目标对象的操作；

5. ObjectPoolManager
	1. ObjectPoolManager用Dictionary\<TypeNamePair, ObjectPoolBase>类型的m_ObjectPools字段储存所有对象池，一个ObjectBase子类类型Type对象，与创建对象池的传入参数字符串name组成一个TypeNamePair对象作为唯一Key，如果我们希望两个对象池储存同样的类型对象，在创建对象池时传入不同的name参数即可；
	2. CreateSingleSpawnObjectPool和CreateMultiSpawnObjectPool方法创建对象池，分别对应一个对象同时只能被获取一次的对象，以及一个对象能被同时获取多次两种类型的对象池（区别详见上面ObjectPool部分的介绍）。这两个创建对象池的方法，GF提供了非常丰富的重载，可以在创建时指定对象池的名字、自动释放时间间隔、容量、物体过期时间、优先级等；
	3. HasObjectPool、GetObjectPool、GetObjectPools、GetAllObjectPools提供对象池查询功能；
	4. Release、ReleaseAllUnused会对所有对象池执行Release、ReleaseAllUnused方法，作用在上文已经说明；
	5. DestroyObjectPool可主动销毁对象池，会回收ObjectBase、Object对象到引用池，执行ObjectBase的Release方法；

6. 参考
   - [GameFramework解析：对象池](https://www.drflower.top/posts/6b0e0248/)


## 0x06 VersionHelper
1. IVersionHelper 版本号辅助器接口
	- BaseFramework 命名空间
	- Version.IVersionHelper
    	- IVersionHelper 属于Version的内部类；
        - string GameVersion；游戏版本号；
        - int InternalGameVersion；内部游戏版本号；
2. BaseFramework.Version
	- SetVersionHelper(IVersionHelper versionHelper) 
    	- 框架初始化的时候设置版本号辅助器；
	- Version.GameVersion -> s_VersionHelper.GameVersion;
	- Version.InternalGameVersion -> s_VersionHelper.InternalGameVersion;
3. DefaultVersionHelper : Version.IVersionHelper
	- UnityBaseFramework.Runtime 命名空间；
	- 实现IVersionHelper接口
        - string GameVersion;
    	- int InternalGameVersion;
	- 框架初始化的时候，将DefaultVersionHelper注册进框架
4. 使用
	- BaseFramework.Version.GameVersion;
	- BaseFramework.Version.InternalGameVersion;
5. 总结
	- 定义 版本辅助器接口，版本类 通过接口获取数据；
	- 默认版本辅助器 实现接口，在框架之初将 默认版本辅助器 注册进框架；
	- 从而实现 版本类 间接调用 默认版本辅助器，将调用与逻辑解耦；

## 0x07 CompressionHelper
1. ICompressionHelper 压缩解压缩辅助器接口
	- BaseFramework 命名空间；
    - Compression.ICompressionHelper, ICompressionHelper属于Compression内部接口；
  	- bool Compress(byte[] bytes, int offset, int length, Stream compressedStream);
  	- bool Compress(Stream stream, Stream compressedStream);
  	- bool Decompress(byte[] bytes, int offset, int length, Stream decompressedStream);
  	- bool Decompress(Stream stream, Stream decompressedStream);
2. Compression 压缩解压缩相关的实用函数
	- BaseFramework 命名空间；
	- Utility.Compression 属于Utility内部类；
	- void SetCompressionHelper(ICompressionHelper compressionHelper)
    	- 初始化时设置压缩解压缩辅助器；
	- 压缩解压缩调用辅助器相应的函数来进行压缩解压缩；
3. DefaultCompressionHelper
	- UnityBaseFramework.Runtime 命名空间；
	- DefaultCompressionHelper : Utility.Compression.ICompressionHelper
    	- 实现ICompressionHelper接口
	- 使用SharpZipLib类库进行最终的压缩解压缩的实现；
    	- SharpZipLib是一个开源的C#压缩解压库；
4. 使用
	- byte[] compressedBytes = Utility.Compression.Compress(bytes);
	- byte[] decompressedBytes = Utility.Compression.Decompress(bytes);
5. 总结
	- 设计接口（压缩解压缩辅助器接口）；
	- 默认压缩解压缩辅助器类 实现接口；
	- 初始化时将 辅助器类 注册进 压缩解压缩的实用类；
	- 压缩解压缩实用类 调用 压缩解压缩函数时，使用注册进来的辅助器进行解压缩；

## 0x08 JsonHelper
1. IJsonHelper JSON辅助器接口
	- BaseFramework 命名空间；
	- Json.IJsonHelper，IJsonHelper属于Json的内部接口；
	- string ToJson(object obj); 将对象序列化为 JSON 字符串；
	- T ToObject<T>(string json); 将 JSON 字符串反序列化为对象；
	- object ToObject(Type objectType, string json); 将 JSON 字符串反序列化为对象；
2. Json JSON相关的实用函数
	- Utility.Json 属于Utility内部类；
	- SetJsonHelper(IJsonHelper jsonHelper)
    	- 初始化时设置Json辅助器；
  	- 序列化和反序列化时调用对应的Json辅助器的来进行序列化反序列化；
3. DefaultJsonHelper 默认JSON函数集辅助器
	- UnityBaseFramework.Runtime命名空间；
	- DefaultJsonHelper : Utility.Json.IJsonHelper；
	- 使用UnityEngine.JsonUtility来进行json数据的序列化反序列化；
    	- JsonUtility.ToJson(object obj);
    	- JsonUtility.FromJson<T>(string json);
    	- JsonUtility.FromJson(string json, Type type);
4. 使用
	- string ret = Utility.Json.ToJson(obj)
	- m_VersionInfo = Utility.Json.ToObject\<VersionInfo>(versionInfoString)
5. 总结
	- 设计JSON辅助器接口；
	- 默认Json函数辅助器实现接口；
	- 框架初始化时，将 Json函数辅助器 注册进 Json实用函数类；
	- 调用Json实用函数序列化和反序列化时，使用注册的 Json函数辅助器 进行序列化和反序列化；

## 0x09 Entry
1. BaseFrameworkModule 基础模块抽象类
    1. abstract class 抽象类；
        - int Priority 模块优先级；
        - Update(float elapseSeconds, float realElapseSeconds) 模块轮询；
        - Shutdown() 关闭并清理模块
    2. 一般需要优先级，需要Update的模块都需要继承此类，比如；
		- ObjectPoolManager : BaseFrameworkModule, IObjectPoolManager;
		- WebRequestManager : BaseFrameworkModule, IWebRequestManager
		- EventManager : BaseFrameworkModule, IEventManager
2. BaseFrameworkEntry 基础框架入口
    1. static class 静态类；
    2. BaseFrameworkLinkedList\<BaseFrameworkModule> s_BaseFrameworkModules;
		- 维护一个BaseFrameworkModule的链表；
    3. Update(float elapseSeconds, float realElapseSeconds);
		- 更新s_BaseFrameworkModules链表中所有模块；
	4. Shutdown()
        - Shutdown s_BaseFrameworkModules链表中所有模块；
      	- 清理s_BaseFrameworkModules；
      	- ReferencePool.ClearAll();
      	- Utility.Marshal.FreeCachedHGlobal();释放缓存中的从进程的非托管内存中分配的内存；
      	- BaseFrameworkLog.SetLogHelper(null);
	5. T GetModule<T>() where T : class 获取基础框架模块
        - 根据T的接口名，拼出要穿件的模块类型；
        - interfaceType.Namespace + interfaceType.Name.Substring(1);
            - IEventManager -> BaseFramework.Event.EventManager
        - 转到CreateModule(Type moduleType)；
	6. BaseFrameworkModule CreateModule(Type moduleType) 创建模块
		- Activator.CreateInstance(moduleType)创建；
		- 根据优先级，添加到链表指定位置；
	7. 在 BaseComponent 的 Update 中更新 BaseFrameworkEntry
        - BaseFrameworkEntry.Update(Time.deltaTime, Time.unscaledDeltaTime); 
	8. 在 BaseComponent 的 OnDestroy 中Shutdown BaseFrameworkEntry
		- BaseFrameworkEntry.Shutdown();
	9.  示例
		- EventComponent 中 Awake()
    		- m_EventManager = BaseFrameworkEntry.GetModule\<IEventManager>();
  		- WebRequestComponent 中 Awake()
    		- m_WebRequestManager = BaseFrameworkEntry.GetModule\<IWebRequestManager>();
  
3. BaseFrameworkComponent 基础框架组件抽象类
	1. BaseFrameworkComponent : MonoBehaviour;
	2. Awake中将此组件注册进BaseEntry；
        - BaseEntry.RegisterComponent(this);
	3. 所有继承BaseFrameworkComponent的组件，都需要挂载在一个GameObject上，默认挂载在BaseFramework/Builtin/...同名的GameObject上；
	4. 所有继承BaseFrameworkComponent的组件，在Awake时，都会将自己注册进BaseEntry；
	5. 示例：
        - BaseComponent : BaseFrameworkComponent;
        - EventComponent : BaseFrameworkComponent;
        - WebRequestComponent : BaseFrameworkComponent;

4. BaseEntry 框架入口
	- static class; 静态类；
    	- 所有成员函数也是 static；
	- BaseFrameworkLinkedList\<BaseFrameworkComponent> s_BaseFrameworkCompomemts;
    	- 维护一个基础组件的链表；
  	- RegisterComponent(BaseFrameworkComponent baseFrameworkComponent)
    	- 注册基础框架组件，添加到s_BaseFrameworkCompomemts中；
    	- 不能重复添加，一个组件只能有一个实例；
  	- BaseFrameworkComponent GetComponent(Type type)
    	- 获取框架组件；
    	- 从s_BaseFrameworkCompomemts链表中遍历获取；
  	- void Shutdown(ShutdownType shutdownType)
		- 基础组件Shutdown；
		- 清理s_BaseFrameworkCompomemts；
		- 根据ShutdownType来判断是重启还是退出；
	- 总结
    	- BaseFrameworkComponent组件会在Awake()中自动注册进链表；
    	- BaseEntry维护了继承自BaseFrameworkComponent的所有组件；
    	- 通过BaseEntry.GetComponent()来获取组件；

5. BaseComponent 基础组件
	- BaseComponent : BaseFrameworkComponent; 会在初始时注册进BaseEntry；
	- EditorResourceMode : 是否使用编辑器资源模式；
	- EditorLanguage : 编辑器语言；
	- EditorResourceHelper : 编辑器资源辅助器；
	- FrameRate : 游戏帧率；
	- GameSpeed : 游戏速度；
	- TextHelper : UnityBaseFramework.Runtime.DefaultTextHelper；
    	- Activator.CreateInstance(textHelperType);
    	- Utility.Text.SetTextHelper(textHelper);
	- VersionHelper : UnityBaseFramework.Runtime.DefaultVersionHelper；
    	- Activator.CreateInstance(versionHelperType);
    	- BaseFramework.Version.SetVersionHelper(versionHelper);
	- LogHelper : UnityBaseFramework.Runtime.DefaultLogHelper；
    	- Activator.CreateInstance(logHelperType);
    	- BaseFrameworkLog.SetLogHelper(logHelper);
	- CompressionHelper : UnityBaseFramework.Runtime.DefaultCompressionHelper；
    	- Activator.CreateInstance(compressionHelperType);
    	- Utility.Compression.SetCompressionHelper(compressionHelper);
	- JsonHelper : UnityBaseFramework.Runtime.DefaultJsonHelper；
    	- Activator.CreateInstance(jsonHelperType);
    	- Utility.Json.SetJsonHelper(jsonHelper);
  	- 总结
    	- 主要设置游戏的一些基础设置；
    	- 初始化一些工具类Helper,并注册进相应的工具类中（静态类）；

6. GameEntry 游戏入口
	- GameEntry : MonoBehaviour;
    	- 挂在 BaseFramework 上；
	- Start()
    	- 初始化内置基础组件；
    	- 初始化自定义组件；
  	- InitBuiltinComponents()
    	- Base = BaseEntry.GetComponent\<BaseComponent>();
        	- 挂在 BaseFramework\Builtin 上；
    	- Event = BaseEntry.GetComponent\<EventComponent>();
        	- 挂在 BaseFramework\Builtin\Event 上；
    	- WebRequest = BaseEntry.GetComponent\<WebRequestComponent>();
        	- 挂在 BaseFramework\Builtin\WebRequest 上；
		- ...
    - InitCustomComponents()
        - HPBar = BaseEntry.GetComponent\<HPBarComponent>();
            - 挂在 BaseFramework\Customs\HPBar 上；
        - ...
    - 总结
        - GameEntry在获取基础组件的时候，所有基础组件已经在 Awake 时注册进了BaseEntry;
        - GameEntry.Base 
            - UnityBaseFramework.Runtime.BaseComponent
        - GameEntry.Event
            - UnityBaseFramework.Runtime.EventComponent
                - BaseFramework.Event.EventManager
        - GameEntry.WebRequest
            - UnityBaseFramework.Runtime.WebRequestComponent
                - BaseFramework.WebRequest.WebRequestManager
                - UnityBaseFramework.Runtime.EventComponent

## 0x0A Utility
- Utility.Assembly 程序集相关的实用函数；
- Utility.Convertrt 类型转换相关的实用函数；
- Utility.Encryption 加密解密相关的实用函数；
- Utility.Masrshal Marshal相关的实用函数；
- Utility.Path 路径相关的实用函数；
- Utility.Random 随机相关的实用函数；
- Utility.Verifier.Crc32 CRC32算法；
- Utility.Verifier 校验相关的实用函数；
- BinaryExtension 对BinaryReader和BinaryWriter的扩展方法；
- StringExtension 对string的扩展方法；
- UnityExtension 对Unity的扩展方法；
		
## 0x0B Event
1. BaseEventArgs 事件基类
	- BaseFramework 命名空间；
	- BaseEventArgs : BaseFrameworkEventArgs;
		- BaseFrameworkEventArgs : EventArgs, IReference;
    - abstract class 抽象类；
        - abstract int Id;
            - 以此ID为key存储委托EventHandler；
    - 示例
        - GameEntry.Event.Subscribe(WebRequestSuccessEventArgs.EventId, OnWebRequestSuccess);
        - WebRequestSuccessEventArgs:BaseEventArgs;
        - WebRequestSuccessEventArgs.Id -> EventId;
2. Event 事件节点
    - BaseFramework 命名空间；
    - EventPool.Event : IReference
        - Event 属于 EventPool 的内部私有类；
        - 实现IReference接口，使用ReferencePool获取；
    - 结构
        - object m_Sender; 为事件注册者；
        - T m_EventArgs; T为BaseEventArgs；
    - 总结
        - Event属于EventPool内部类，外界不知晓其存在；
        - 主要是EventPool抛出事件时，用此结构包装一下数据，在下一帧的Update中通过这个结构数据进行事件分发；
3. EventPoolMode 事件池模式
	- BaseFramework 命名空间
	- enum EventPoolMode : byte
    	- Default : 默认事件池模式，即必须存在有且只有一个事件处理函数；
    	- AllowNoHandler : 允许不存在事件处理函数；
    	- AllowMultiHandler : 允许存在多个事件处理函数；
    	- AllowDuplicateHandler : 允许存在重复的事件处理函数；
4. EventPool 事件池
	- BaseFramework 命名空间
	- EventPool\<T> where T : BaseEventArgs
    	- 泛型参数T是BaseEventArgs；
  	- 存储结构
    	- BaseFrameworkMultiDictionary\<int, EventHandler\<T>> m_EventHandlers
        	- key 是 BaseEventArgs.Id；
        	- value 是泛型类型是 BaseEventArgs 的 EventHandler 委托；
        	- 一个 key 对应多个 value ；
      	- Queue\<Event> m_Events
        	- Fire(object sender, T e) 抛出事件时，为了线程安全，会 lock (m_Events)，将Fire的参数生成内部事件节点 Event eventNode = Event.Create(sender, e)，添加到 m_Events 中，之后在下一帧的Update中再根据事件参数中的Id，查找到事件委托，最终调用委托；
        	- 总结下来，是为了线程安全而在内部使用的存储队列；
      	- 两个缓存字典
        	- Dictionary\<object, LinkedListNode\<EventHandler\<T>>> m_CachedNodes；
        	- Dictionary\<object, LinkedListNode\<EventHandler\<T>>> m_TempNodes;
        	- 在 HandleEvent(object sender, T e) 处理事件节点时，通过事件参数Id，取出此事件参数对应的事件委托链表的开始节点和结束节点，从第一个节点 range.First 开始遍历，m_CachedNodes[e] 记录为将处理的下一个节点，然后执行当前委托，执行完之后当前节点赋值为 m_CachedNodes[e]，继续下一轮循环；
        	- 当处在循环中时，收到了取消订阅的函数；
            	- Unsubscribe(int id, EventHandler\<T> handler);
            	- 如果 m_CachedNodes.Count > 0， 则处在事件处理中；
            	- 遍历 m_CachedNodes 中的所有即将处理的节点，如果取消订阅的事件处理函数在 m_CachedNodes 中，则，通过m_TempNodes缓存一下，将即将处理的节点的值变成 cachedNode.Value.Next 下一个；
            	- 然后 m_EventHandlers.Remove(id, handler)；
          	- 这两个缓存容器，是为了处理这种情况
            	- 即即将被调用的事件委托函数，突然被取消订阅了，怎么把这个函数删除的问题；
  	- Subscribe(int id, EventHandler<T> handler) 订阅事件处理函数
    	- id 为 泛型参数 T （即 BaseEventArgs）的Id；
    	- 如果 m_EventHandlers 不包含此Id，则直接添加；
    	- 如果 m_EventHandlers 已经包含此 Id， 
        	- 事件池模式不允许存在多个事件处理函数，则抛出异常；
        	- 事件池模式不允许存在重复的事件处理函数，并且已经存在了，则抛出异常；
        	- 否则就是允许存在多个事件处理函数，直接添加；
      	- m_EventHandlers.Add(id, handler)；
  	- Unsubscribe(int id, EventHandler<T> handler) 取消订阅事件处理函数
		- 取消订阅事件处理函数，主要处理当前的事件是否为即将处理的事件；详见EventPool存储结构的第三条；
		- m_EventHandlers.Remove(id, handler)，直接删除此订阅事件处理函数；
    - Fire(object sender, T e) 
        - 抛出事件，这个操作是线程安全的，即使不在主线程中抛出，也可保证在主线程中回调事件处理函数，但事件会在抛出后的下一帧分发；
		- 根据参数创建内部事件节点；
    		- Event eventNode = Event.Create(sender, e);
		- 添加到 m_Events队列中；
    		- m_Events.Enqueue(eventNode)；
		- 在下一帧的Update中处理事件节点；
    		- Event eventNode = m_Events.Dequeue()
    		- HandleEvent(eventNode.Sender, eventNode.EventArgs)；
	- FireNow(object sender, T e) 
    	- 抛出事件立即模式，这个操作不是线程安全的，事件会立刻分发；
		- HandleEvent(sender, e)；
    - HandleEvent(object sender, T e) 处理事件结点
        - 从 m_EventHandlers 查找出对应事件委托链表的开始节点和结束节点；
        - 遍历并处理节点；
            - 取出的数据为 LinkedListNode\<EventHandler\<T>> current;
            - EventHandler\<T>委托数据存储在LinkedListNode的Value中；
            - 直接执行 current.Value(sender, e)；
		- 根据事件池类型判断是否抛出异常等；
5. GameEventArgs 游戏逻辑事件基类
	- BaseFramework.Event 命名空间；
	- abstract class 抽象类；
	- GameEventArgs : BaseEventArgs；
6. EventManager 事件管理器
	- 命名空间 BaseFramework.Event；
	- EventManager : BaseFrameworkModule, IEventManager
    	- 继承BaseFrameworkModule
    	- Update顺序
        	- BaseComponent(MonoBehaviour).Update()
        	- BaseFrameworkEntry.Update(Time.deltaTime, Time.unscaledDeltaTime);
        	- BaseFrameworkModule.Update(...)，即EventManager.Update(...)；
  	- 存储结构
    	- EventPool\<GameEventArgs> m_EventPool = new EventPool\<GameEventArgs>(EventPoolMode.AllowNoHandler | EventPoolMode.AllowMultiHandler);
    	- 允许不存在事件处理函数；
    	- 允许存在多个事件处理函数；
  	- Subscribe(int id, EventHandler<GameEventArgs> handler)
    	- 订阅事件处理函数；
    	- m_EventPool.Subscribe(id, handler)；
  	- Unsubscribe(int id, EventHandler<GameEventArgs> handler)
    	- 取消订阅事件处理函数；
    	- m_EventPool.Unsubscribe(id, handler)；
  	- Fire(object sender, GameEventArgs e)
    	- 抛出事件，这个操作是线程安全的，即使不在主线程中抛出，也可保证在主线程中回调事件处理函数，但事件会在抛出后的下一帧分发；
    	- m_EventPool.Fire(sender, e)；
  	- FireNow(object sender, GameEventArgs e)
    	- 抛出事件立即模式，这个操作不是线程安全的，事件会立刻分发；
    	- m_EventPool.FireNow(sender, e)
7. EventComponent
	- UnityBaseFramework.Runtime 命名空间
	- EventComponent : BaseFrameworkComponent : MonoBehaviour
    	- 挂在 BaseFramework\Builtin\Event 上； 
  	- 事件基类定为 GameEventArgs
	- Awake()
    	- m_EventManager = BaseFrameworkEntry.GetModule\<IEventManager>();
    	- Activator.CreateInstance(BaseFramework.Event.EventManager);
  	- Subscribe(int id, EventHandler\<GameEventArgs> handler)
    	- m_EventManager.Subscribe(id, handler);
  	- Unsubscribe(int id, EventHandler<GameEventArgs> handler)
    	- m_EventManager.Unsubscribe(id, handler);
  	- Fire(object sender, GameEventArgs e)
    	- m_EventManager.Fire(sender, e);
	- FireNow(object sender, GameEventArgs e)
    	-  m_EventManager.FireNow(sender, e);

## 0x0C Task
1. TaskStatus
    - BaseFramework 命名空间 
	- enum TaskStatus : byte
    	- Todo : 未开始；
    	- Doing : 执行中；
    	- Done : 完成；
2. StartTaskStatus 开始处理任务的状态
	- BaseFramework 命名空间
	- enum StartTaskStatus : byte
    	- Done : 可以立刻处理完成此任务；
    	- CanResume : 可以继续处理此任务；
    	- HasToWait : 不能继续处理此任务，需等待其它任务执行完成；
    	- UnknownError : 不能继续处理此任务，出现未知错误；
3. TaskBase 任务基类
	1. 基础
        - BaseFramework 命名空间
        - abstract class 抽象类
        - TaskBase : IReference
	2. 属性
        - int SerialId 任务的序列编号；
        - string Tag 任务的标签；
        - int Priority 任务的优先级；
        - object UserData 任务的用户自定义数据；
        - bool Done 任务是否完成；
        - string Description 任务描述；
	3. Function
		- Initialize(int serialId, string tag, int priority, object userData)
    		- 初始化任务基类
		- Clear()
    		- 清理任务基类
	4. TaskBase 类只负责存储数据，不负责具体行为；
4. ITaskAgent\<T> 任务代理接口
    - BaseFramework 命名空间
    - interface ITaskAgent\<T> where T : TaskBase
    - T Task，获取任务；
    - Initialize()，初始化任务代理；
    - Update(float elapseSeconds, float realElapseSeconds)，任务代理轮询；
    - Shutdown()，关闭并清理任务代理；
    - Start(T task)，开始处理任务；
    - Reset()，停止正在处理的任务并重置任务代理；
5. TaskPool 任务池
	1. 基础
		- BaseFramework 命名空间
		- TaskPool\<T> where T : TaskBase
	2. 存储结构
        - Stack\<ITaskAgent\<T>> m_FreeAgents；空闲的代理集合；
        - BaseFrameworkLinkedList\<ITaskAgent\<T>> m_WorkingAgents；正在工作的代理链表；
        - BaseFrameworkLinkedList\<T> m_WaitingTasks；等待处理的任务链表；
	3. AddAgent(ITaskAgent<T> agent) 增加任务代理
        - 初始化agent，添加到m_FreeAgents等待使用；
			- agent.Initialize();
            - m_FreeAgents.Push(agent);
        - ITaskAgent 由外部初始化时传入，传入数量可指定；
        - 示例 
            - WebRequestComponent.Start()
                - AddWebRequestAgentHelper(int index)
                    - webRequestAgentHelper = Helper.CreateHelper("UnityBaseFramework.Runtime.UnityWebRequestAgentHelper")
                    - m_WebRequestManager.AddWebRequestAgentHelper(webRequestAgentHelper)
            - WebRequestManager.AddWebRequestAgentHelper(webRequestAgentHelper)
                - WebRequestAgent agent = new WebRequestAgent(webRequestAgentHelper)
                - m_TaskPool.AddAgent(agent)
	4. AddTask(T task) 增加任务
        - 将 task 插入到 m_WaitingTasks 的合适位置；
        - 只将数据添加进来，何时处理，如何处理，与task无关；
	5. Update(...) 任务池每帧更新
        1. 从里更新调用到这里，示例
			- BaseComponent.Update
			- BaseFrameworkEntry.Update()
			- WebRequestManager.Update()
			- m_TaskPool.Update() 
		2. 先处理当前有的任务，这一帧如果处理完，则释放代理器，空闲的代理可以继续处理等待的任务；
        3. ProcessRunningTasks 处理正在运行的任务
            1. 从 m_WorkingAgents 取一个代理节点 LinkedListNode\<ITaskAgent\<T>> current，current是链表节点，current.Value是代理类（agent），current.Value.Task是任务数据(task)；
            2. T task = current.Value.Task;
                - 根据 task.Done 判断task是否完成；
                - 如果未完成，agent 继续 Update();
                - 循环继续判断下一个；
                - 如果完成，则：
                    - agent重置，重新将agent添加到m_FreeAgents中；
                    - m_WorkingAgents 删除current节点；
                    - ReferencePool.Release(task)，处理完，回收task，因为task是引用池创建的；
                - 循环继续判断下一个；
        4. ProcessWaitingTasks 处理等待的任务
			1. 从 m_WaitingTasks 中的取一个 Task 数据，为current；
			2. 判断是否有空闲的Agent可以处理Task，有则继续，没有则退出；
			3. 取一个空闲的agent，将agent添加到 m_WorkingAgents 中；
			4. agent 开始处理 task 数据，agent.Start(task)；
			5. 根据4中处理的结果，判断下一步执行：
            	- 如果立刻完成了任务（Done），或者需要等待其他任务完成（HasToWait），或者出现错误（UnknownError），则此取消此代理agent，agent重置，然后添加到m_FreeAgents，m_WorkingAgents删除agentNode；
            	- 如果已经完成了任务（Done），或者可以继续处理此任务（CanResume），或者出现错误（UnknownError），则将m_WaitingTasks删除此任务数据，此任务已经处理，或者处理中，或者不需要处理；
            	- 如果已经完成了任务（Done），或者出现错误（UnknownError），怎上面删除任务数据后， ReferencePool.Release(task)回收task，因为task是引用池创建的；
          	1.  回到2继续循环，判断是否有任务数据要处理，是否有空闲的agent可以处理；
    6. RemoveTask(int serialId)\RemoveTasks(string tag) 移除任务
        - 从 m_WaitingTasks 移除等待处理的task；
       	- 从 m_WorkingAgents 正在执行的代理中删除task，不管是否完成；
	7. GetAllTaskInfos() 获取所有任务的信息
        - 编辑器获取信息展示在Inspect面板上，供查看调试；
6. 总结
	- 任务池不关心处理的数据，也不关心处理的逻辑；
	- 通过AddAgent添加处理数据的代理器；
	- 通过AddTask添加任务数据；
	- 在Update中使用代理器agent处理task数据；
	- 这样解耦了数据和数据处理的逻辑；

