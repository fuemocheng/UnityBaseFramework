# DesignNote 设计笔记

## 0x00 BaseModule 基础模块

1. IReference 接口
所有可以被回收重用的都需要实现接口Clear;

2. BaseModuleEventArgs : EventArgs , IReference
2.1 abstract class
事件数据类的基类

## 0x01 自定义存储结构

### BaseFrameworkLinkedList 链表
1.数据存储在链表 LinkedList<T> 里，同时维护一份缓存队列Queue<LinkedListNode<T>> m_CachedNodes；
2.添加节点时，先判断缓存队列是否有节点，有的话用缓存队列中的；
3.删除节点时，从 LinkedList<T> 中删除，还原默认值，添加到缓存队列中；
4.缓存队列需要手动清理；
5.实现迭代器；
总结：比传统的链表增加了缓存队列，在频繁创建删除的链表中，具有优势；

### BaseFrameworkLinkedListRange 链表范围结构体
1.是结构体；
2.只记录两个节点，第一个和最后一个；
3.实现First，Terminal，IsValid，Count，Contains()；
4.实现迭代器；
总结：为了实现多值字典类而定义的结构体；

### BaseFrameworkMultiDictionary 多值字典类
1.数据存储在 BaseFrameworkLinkedList<TValue> m_LinkedList 链表中；
2.维护一个 Dictionary<TKey, BaseFrameworkLinkedListRange<TValue>> m_Dictionary 字典，
来记录键值在链表的开始结束位置，方便查询获取；
3.添加时，查找字典中此键对应的的最后一个值，然后添加到此值之前；
4.删除时，需要判断是否是Range的第一个值，需要特殊判断，然后m_LinkedList中删除即可；
5.实现迭代器；






## 0x0A WebRequest
1. 结构
UnityWebRequestAgentHelper : WebRequestAgentHelperBase : IWebRequestAgentHelper

WebRequestManager : IWebRequestManager

WebRequestComponent -> AddWebRequestAgentHelper() 
根据类型名，创建Web请求代理辅助器实例，并传到WebRequestManager中;
订阅WebRequestManager的WebRequestStart、WebRequestSuccess、WebRequestFailure事件;

WebRequestManager -> AddWebRequestAgentHelper()
根据传入的helper创建 Web请求代理-WebRequestAgent;
WebRequestAgent订阅WebRequestAgentStart、WebRequestAgentSuccess、WebRequestAgentFailure事件;
TaskPool.AddAgent(WebRequestAgent);
m_FreeAgents.Push(WebRequestAgent)

2. Web请求
WebRequestComponent->AddWebRequest()->
WebRequestManager.AddWebRequest()-> WebRequestTask.Create()
TaskPool.AddTask(WebRequestTask)
m_WaitingTasks.Add(...)

3. TaskPool.Update()
ProcessRunningTask()
WorkingAgents队列取一个Agent;
如果Agent的Task是否完成，未完成，Agent.Update()，继续Update;
如果Task完成，则Agent.Reset()，Agent重新加入FreeAgnet队列，
WorkingAgent队列删除此Agent，回收Task;
下一个循环;

ProcessWaitingTasks()
WaitingTasks队列取一个Task，FreeAgents队列取一个Agent，然后Agent.Start(Task);
Agent添加到WorkingAgent队列;
如果Task完成，或者需要等待，或者出现错误，则重置Agent，此Agent重新加入FreeAgnet队列;
如果Task完成，或者可以继续处理，或者出错，WaitingTask队列删除此Task;
如果Task完成，或者出错，回收Task;
进行下一个Task,Agent的循环;

这里的循环主要处理超时回调，具体在WebRequestAgent.Update();

4. 回调
在TaskPool.Update()期间，还未超时;

UnityWebRequestAgentHelper 请求成功或者失败事件,
UnityWebRequestAgentHelper.m_WebRequestAgentHelperCompleteEventHandler() ->
UnityWebRequestAgentHelper.WebRequestAgentHelperComplete += WebRequestAgent.OnWebRequestAgentHelperComplete

WebRequestAgent.OnWebRequestAgentHelperComplete()-> WebRequestAgent.WebRequestAgentSuccess()
WebRequestAgent.WebRequestAgentSuccess +=  WebRequestManager.OnWebRequestAgentSuccess

WebRequestManager.OnWebRequestAgentSuccess() -> WebRequestManager.m_WebRequestSuccessEventHandler()
WebRequestManager.WebRequestSuccess += WebRequestComponent.OnWebRequestSuccess

最终由事件系统分发
WebRequestComponent.OnWebRequestSuccess()->EventComponent.Fire(this, WebRequestSuccessEventArgs.Create(e));

5. 框架之外的处理
需要一个WebRequestComponent实例持有框架内的WebRequestManager，对WebRequest的请求接口和回调进行处理;
	WebRequestManager持有一个或者多个WebRequestAgent，并持有WebRequestTask的TaskPool,对所有Task进行管理更新;
	所以WebRequestManager一类的管理类需要被统一管理并更新;
	
需要一个WebRequestAgentHelper实际发起请求的实例，实现IWebRequestAgentHelper接口;
	WebRequestAgentHelper是实现类，管理类将请求接口和回调注册到请求代理器上，代理器循环执行;
	
入口和出口都在WebRequestComponent，最终调用的也是我们实现的WebRequestAgentHelper，
中间的接口和管理器，是在限制我们的编码规范和统一性;





