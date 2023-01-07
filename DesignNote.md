# Note 笔记

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





