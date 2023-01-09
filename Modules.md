# Modules 模块

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
	- 如果 ILogHelper 为空，则不打印日志；
4. UnityBaseFramework.Runtime.DefaultLogHelper 实现了 ILogHelper 接口；
	- 在框架初始化的时候，将 DefaultLogHelper 注册进 BaseFramework.BaseFrameworkLog，则最终使用 DefaultLogHelper 进行打印；
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

		
		


