# WebRequest 模块
1. Assets/BaseFramework/Libraries/BaseFramework/WebReuqest/
	- 以下所有命名空间为 BaseFramework.WebRequest，属于BaseFramework基础框架内；

	1. WebRequestAgentHelperCompleteEventArgs : BaseFrameworkEventArgs
        - Web请求代理辅助器完成事件；
        - static Create(byte[] webResponseBytes)， 通过ReferencePool创建；
        - byte[] GetWebResponseBytes()，获取Web响应的数据流；
		- 是从 WebRequestAgentHelper 传递到 WebRequestAgent 的事件参数；

	2. WebRequestAgentHelperErrorEventArgs : BaseFrameworkEventArgs
        - Web请求代理辅助器错误事件；
        - static Create(string errorMessage)，通过ReferencePool创建；
        - string ErrorMessage，获取错误信息；
        - 是从 WebRequestAgentHelper 传递到 WebRequestAgent 的事件参数；

	3. WebRequestStartEventArgs : BaseFrameworkEventArgs
        - Web请求开始事件；
        - int SerialId，任务的序列编号；
        - string WebRequestUri，请求地址；
        - object UserData，用户自定义数据；
        - static Create(int serialId, string webRequestUri, object userData)，通过ReferencePool创建；
        - 是从 WebRequestManager 传递到 WebRequestComponent 的开始事件 m_WebRequestStartEventHandler 的泛型参数；

	4. WebRequestSuccessEventArgs : BaseFrameworkEventArgs
        - Web请求成功事件；
        - int SerialId，任务的序列编号；
        - string WebRequestUri，请求地址；
        - object UserData，用户自定义数据；
        - static Create(int serialId, string webRequestUri, byte[] webResponseBytes, object userData)，通过ReferencePool创建；
        - byte[] GetWebResponseBytes()，获取Web响应的数据流；
        - 是从 WebRequestManager 传递到 WebRequestComponent 的成功事件 m_WebRequestSuccessEventHandler 的泛型参数；

	5. WebRequestFailureEventArgs : BaseFrameworkEventArgs
        - Web请求失败事件；
        - int SerialId，任务的序列编号；
        - string WebRequestUri，请求地址；
        - string ErrorMessage，错误信息；
        - object UserData，用户自定义数据；
        - static Create(int serialId, string webRequestUri, string errorMessage, object userData)，通过ReferencePool创建；
        - 是从 WebRequestManager 传递到 WebRequestComponent 的失败事件 m_WebRequestFailureEventHandler 的泛型参数；

	6. WebRequestManager
        1. WebRequestTaskStatus : byte
			- Todo，准备请求；
			- Doing，请求中；
			- Done，请求完成；
			- Error，请求错误；

      	1. WebRequestTask : TaskBase
			- Web请求任务（数据）；
			- WebRequestTaskStatus Status，Web请求任务的状态，默认Todo；
			- string WebRequestUri，要发送的远程地址；
			- float Timeout，超时时长，以秒为单位；
			- static Create(string webRequestUri, byte[] postData, string tag, int priority, float timeout, object userData)，通过ReferencePool创建；
			- byte[] GetPostData()，要发送的数据流；
      	
		2. WebRequestAgent : ITaskAgent\<WebRequestTask>
			- Web请求代理（主要负责处理WebRequestTask数据）；
    			- IWebRequestAgentHelper m_Helper，Web请求代理辅助器；
    			- WebRequestTask m_Task，任务数据；
        		- BaseFrameworkAction\<WebRequestAgent> WebRequestAgentStart;
        		- BaseFrameworkAction\<WebRequestAgent, byte[]> WebRequestAgentSuccess;
        		- BaseFrameworkAction\<WebRequestAgent, string> WebRequestAgentFailure;
        		- float m_WaitTime;
			
			- WebRequestManager.AddWebRequestAgentHelper(IWebRequestAgentHelper) 创建Web代理；
    			- WebRequestAgent agent = new WebRequestAgent(webRequestAgentHelper);
    			- agent.WebRequestAgentStart += WebRequestManager.OnWebRequestAgentStart;
            	- agent.WebRequestAgentSuccess += WebRequestManager.OnWebRequestAgentSuccess;
                - agent.WebRequestAgentFailure += WebRequestManager.OnWebRequestAgentFailure;
                - WebRequestManager.m_TaskPool.AddAgent(agent)
                    - agent.Initialize()，初始化
                        - WebRequestAgent初始化时，将IWebRequestAgentHelper m_Helper 的委托绑定WebRequestAgent的定义的函数；
                        - m_Helper.WebRequestAgentHelperComplete += OnWebRequestAgentHelperComplete;
                		- m_Helper.WebRequestAgentHelperError += OnWebRequestAgentHelperError;
            
			- void OnWebRequestAgentHelperComplete(object sender, WebRequestAgentHelperCompleteEventArgs e)
                - m_Helper完成事件回调到此；
                - m_Helper重置，释放UnityWebRequest.Dispose()；
                - m_Task状态设置为Done；
                - 调用委托WebRequestAgentSuccess()，然后调用到WebRequestManager的完成事件；
            
			- void OnWebRequestAgentHelperError(object sender, WebRequestAgentHelperErrorEventArgs e)
                - m_Helper错误事件回调到此；
                - m_Helper重置，释放UnityWebRequest.Dispose()；
                - m_Task状态设置为Error；
                - 调用委托WebRequestAgentFailure()，然后调用到WebRequestManager的错误事件；
			
			- StartTaskStatus Start(WebRequestTask task) 开始处理Web请求任务
    			- 赋值m_Task数据，调用委托WebRequestAgentStart（然后调用到WebRequestManager的开始事件），取数据，m_Helper.Request(...)开始请求，开始记录是将m_WaitTime重置，返回可以继续任务的状态；

  			- void Update(float elapseSeconds, float realElapseSeconds)
      			- 每帧检测m_Task的状态
      			- m_Task状态是Doing，则继续计时，一直等到超时；
        			- 超时，则调用OnWebRequestAgentHelperError出错事件；
      			- 如果完成了，或者出错了，则不会继续执行；
      	
		4. WebRequestManager : BaseFrameworkModule, IWebRequestManager
            - TaskPool\<WebRequestTask> m_TaskPool;
            - float m_Timeout;
            - EventHandler\<WebRequestStartEventArgs> m_WebRequestStartEventHandler;
                - 开始事件由此调用到外部 WebRequestComponent.OnWebRequestStart；
            - EventHandler\<WebRequestSuccessEventArgs> m_WebRequestSuccessEventHandler;
                - 成功事件由此调用到外部 WebRequestComponent.OnWebRequestSuccess；
            - EventHandler\<WebRequestFailureEventArgs> m_WebRequestFailureEventHandler;
                - 开始事件由此调用到外部 WebRequestComponent.OnWebRequestFailure；
			- AddWebRequestAgentHelper(IWebRequestAgentHelper webRequestAgentHelper)
    			- 添加Web请求代理辅助器；
    			- 创建WebRequestAgent代理，同时agent的委托赋值为当前处理函数；
        			- WebRequestAgent agent = new WebRequestAgent(webRequestAgentHelper);
        			- agent.WebRequestAgentStart += OnWebRequestAgentStart;
        			- agent.WebRequestAgentSuccess += OnWebRequestAgentSuccess;
        			- agent.WebRequestAgentFailure += OnWebRequestAgentFailure;
        			- 将agent添加到m_TaskPool中；
			- AddWebRequest(string webRequestUri, byte[] postData, string tag, int priority, object userData)
    			- 添加 Web 请求任务；
    			- WebRequestTask.Create(...)；
    			- 添加到 m_TaskPool 中；
    			- 返回任务序列编号SerialId；
  			- bool RemoveWebRequest(int serialId)
      			- 根据 Web 请求任务的序列编号移除 Web 请求任务；
      			- _TaskPool.RemoveTask(serialId);
			- Update(float elapseSeconds, float realElapseSeconds)
    			- m_TaskPool.Update(elapseSeconds, realElapseSeconds);
    			- 处理正在进行的任务，和等待的任务；
        			- 正在进行的任务完成了，释放代理器和删除任务数据；
        			- 正在等待的任务判断是否有空闲代理器去处理这个任务；
        	- OnWebRequestAgentStart(WebRequestAgent sender)
            	- 创建WebRequestStartEventArgs事件参数 webRequestStartEventArgs ；
            	- m_WebRequestStartEventHandler(this, webRequestStartEventArgs)，回调到 WebRequestComponent.OnWebRequestStart；
        	- OnWebRequestAgentSuccess(WebRequestAgent sender, byte[] webResponseBytes)
            	- 创建WebRequestSuccessEventArgs事件参数 webRequestSuccessEventArgs ；
            	- m_WebRequestSuccessEventHandler(this, webRequestSuccessEventArgs)，回调到 WebRequestComponent.OnWebRequestSuccess；
          	- OnWebRequestAgentFailure(WebRequestAgent sender, string errorMessage)
              	- 创建WebRequestFailureEventArgs事件参数 webRequestFailureEventArgs ；
            	- m_WebRequestFailureEventHandler(this, webRequestFailureEventArgs)，回调到 WebRequestComponent.OnWebRequestFailure；

2. Assets/BaseFramework/Scripts/Runtime/WebRequest/
    - 以下命名空间 UnityBaseFramework.Runtime
	
	1. WebRequestStartEventArgs : GameEventArgs
        - static readonly int EventId，请求开始事件编号；
        - int Id -> EventId，请求开始事件编号；
        - int SerialId，请求任务的序列编号；
        - string WebRequestUri，请求地址；
        - object UserData，用户自定义数据；
        - static Create(WebRequestStartEventArgs e)，通过ReferencePool创建；
        - 从 WebRequestComponent 中抛出事件；
            - m_EventComponent.Fire(this, WebRequestStartEventArgs.Create(e));
	
	2. WebRequestSuccessEventArgs : GameEventArgs
        - static readonly int EventId，请求成功事件编号；
        - int Id -> EventId，请求成功事件编号；
        - int SerialId，请求任务的序列编号；
        - string WebRequestUri，请求地址；
        - object UserData，用户自定义数据；
        - static Create(WebRequestSuccessEventArgs e)，通过ReferencePool创建；
        - 从 WebRequestComponent 中抛出事件；
            - m_EventComponent.Fire(this, WebRequestSuccessEventArgs.Create(e));
	
	3. WebRequestFailureEventArgs : GameEventArgs
		- static readonly int EventId，请求失败事件编号；
        - int Id -> EventId，请求失败事件编号；
        - int SerialId，请求任务的序列编号；
        - string WebRequestUri，请求地址；
        - string ErrorMessage，错误信息；
        - object UserData，用户自定义数据；
        - static Create(WebRequestFailureEventArgs e)，通过ReferencePool创建；
        - 从 WebRequestComponent 中抛出事件；
            - m_EventComponent.Fire(this, WebRequestFailureEventArgs.Create(e));
	
	4. WWWFormInfo : IReference
		- UnityEngine.WWWForm WWWForm，www表单数据；
		- object UserData，用户自定义数据；
		- static WWWFormInfo Create(WWWForm wwwForm, object userData)；
    		- 通过ReferencePool创建；
	
	5. UnityWebRequestAgentHelper : WebRequestAgentHelperBase, IDisposable
		- 通过初始化注册，最终由 WebRequestAgent 直接使用；
		- event EventHandler\<WebRequestAgentHelperCompleteEventArgs> WebRequestAgentHelperComplete，请求代理辅助器完成事件
		- event EventHandler\<WebRequestAgentHelperErrorEventArgs> WebRequestAgentHelperError，请求代理辅助器错误事件
		- Request(string webRequestUri, object userData)，请求代理辅助器发送请求；
    		- userData 为 WWWFormInfo 数据；
    		- 通过 UnityWebRequest.Get(webRequestUri)，或者 UnityWebRequest.Post(webRequestUri, wwwFormInfo.WWWForm)，来设置要发送的数据；
    		- m_UnityWebRequest.SendWebRequest()，通过UnityWebRequest来发送HTTP请求；
  		- Update()
      		- 每帧判断 m_UnityWebRequest 是否获取完成，或者出错；
      		- 成功，则创建成功的事件参数 WebRequestAgentHelperCompleteEventArgs，m_WebRequestAgentHelperErrorEventHandler(this, webRequestAgentHelperErrorEventArgs)；
      		- 失败，则创建失败的事件参数 WebRequestAgentHelperErrorEventArgs，调用委托  m_WebRequestAgentHelperErrorEventHandler(this, webRequestAgentHelperErrorEventArgs)；
	
	6. WebRequestComponent : BaseFrameworkComponent
        - 成员变量
        	- IWebRequestManager m_WebRequestManager；
      		- EventComponent m_EventComponent；
      		- string m_WebRequestAgentHelperTypeName = "UnityBaseFramework.Runtime.UnityWebRequestAgentHelper"；
      		- int m_WebRequestAgentHelperCount = 1;
      		- float m_Timeout = 30f;
        - 初始化
            - Awake() 
                - 通过 BaseFrameworkEntry.GetModule\<IWebRequestManager>() 获取 m_WebRequestManager；
                - m_WebRequestManager 的请求开始，成功，失败的委托，绑定当前对应函数；
                    - m_WebRequestManager.WebRequestStart += OnWebRequestStart;
            		- m_WebRequestManager.WebRequestSuccess += OnWebRequestSuccess;
            		- m_WebRequestManager.WebRequestFailure += OnWebRequestFailure;
          		- m_WebRequestManager.Timeout = m_Timeout，超时时间；
      		- Start()
        		- 获取事件组件 m_EventComponent = BaseEntry.GetComponent\<EventComponent>()；
        		- 根据预设的代理辅助器数量，初始化代理辅助器；
            		- AddWebRequestAgentHelper(i);
                		- 创建 UnityWebRequestAgentHelper 对应的GameObject;
                		- 添加到 BaseFramework/Builtin/WebRequest/Web Request Agent Instances/ 下；
        - AddWebRequest(string webRequestUri, byte[] postData, WWWForm wwwForm, string tag, int priority, object userData)
            - m_WebRequestManager.AddWebRequest(...)
            - 通过 WebRequestManager 添加 Web 请求任务；
        - OnWebRequestStart(object sender, WebRequestStartEventArgs e)
			- m_WebRequestManager 请求开始，调用至此；
			- m_EventComponent.Fire(this, WebRequestStartEventArgs.Create(e));
		- OnWebRequestSuccess(object sender, WebRequestSuccessEventArgs e)
			- m_WebRequestManager 请求成功，调用至此；
			- m_EventComponent.Fire(this, WebRequestSuccessEventArgs.Create(e));
		- OnWebRequestFailure(object sender, WebRequestFailureEventArgs e)
			- m_WebRequestManager 请求失败，调用至此；
			- m_EventComponent.Fire(this, WebRequestFailureEventArgs.Create(e));

3. 使用 
    1. 先注册 WebRequest 请求成功或者失败的事件
        - GameEntry.Event.Subscribe(WebRequestSuccessEventArgs.EventId, OnWebRequestSuccess);
            - 第一个参数是Web请求成功事件编号；
        
		- GameEntry.Event.Subscribe(WebRequestFailureEventArgs.EventId, OnWebRequestFailure);
            - 第一个参数是Web请求失败事件编号；
		
		- OnWebRequestSuccess(object sender, GameEventArgs e)
    		- 成功的回调；
    		- 第二个参数 e 为 WebRequestSuccessEventArgs；
    			- WebRequestSuccessEventArgs ne = (WebRequestSuccessEventArgs)e;
    			- ne.UserData 为 this（即发送请求者）；
    			- ne.GetWebResponseBytes() 即为我们需要的二进制数据；
  			- 处理返回的数据，进行之后的逻辑；

		- OnWebRequestFailure(object sender, GameEventArgs e)
    		- 失败的回调；
    		- 第二个参数 e 为 WebRequestSuccessEventArgs；
        		- WebRequestFailureEventArgs ne = (WebRequestFailureEventArgs)e;
        		- ne.UserData 为 this（即发送请求者）；
      		- 进行错误后的处理；

	2. 发送Web请求
		- GameEntry.WebRequest.AddWebRequest(CheckVersionUrl, this);

4. 总结流程
	1. 发送流程
        1. 注册成功失败事件
            - GameEntry.Event.Subscribe(WebRequestSuccessEventArgs.EventId, OnWebRequestSuccess)；
            - GameEntry.Event.Subscribe(WebRequestFailureEventArgs.EventId, OnWebRequestFailure)；
        2. 发送Web请求
            - GameEntry.WebRequest.AddWebRequest(url, this) -> WebRequestComponent.AddWebRequest(url, ..., userdata)；
      	1. m_WebRequestManager.AddWebRequest(url, ..., WWWFormInfo.Create(wwwForm, userData))；
            - wwwForm表单数据可能为空；
            - 以下的userdata则全部为 WWWFormInfo，发送者数据为 userData.UserData；
      	2. 创建 WebRequestTask 数据，添加到TaskPool中；
            - WebRequestTask webRequestTask = WebRequestTask.Create(url, ..., userdata)；
            - m_TaskPool.AddTask(webRequestTask); 
      	3. WebRequestManager.Update()
			- m_TaskPool.Update()
    			- 取空闲的 WebRequestAgent ，将 WebRequestTask 数据交给 WebRequestAgent 处理；
    			- WebRequestAgent 开始处理任务，agent.Start(WebRequestTask task)；
        			- 使用 请求代理辅助器（IWebRequestAgentHelper m_Helper）即UnityWebRequestAgentHelper 来进行发送请求；
            		- m_Helper.Request(m_Task.WebRequestUri, m_Task.UserData)；
        			- UnityWebRequestAgentHelper.Request(url, userData);
                    	- 使用了 UnityEngine.Networking.UnityWebRequest 进行HTTP请求；
                        	- m_UnityWebRequest = UnityWebRequest.Get(webRequestUri);
                          	- m_UnityWebRequest.SendWebRequest();
        3. 至此，从外部调用，最终使用了自定义的web请求辅助器来发送请求；

	2. 接收流程
        1. 请求代理辅助器，即 UnityWebRequestAgentHelper(继承MonoBehaviour) 中的Update()
            - 每帧判断 m_UnityWebRequest.isDone 是否完成；
            - 完成且成功，则调用委托m_WebRequestAgentHelperErrorEventHandler；
                - 创建 WebRequestAgentHelperCompleteEventArgs 完成事件参数 args；
                - 回调 m_WebRequestAgentHelperErrorEventHandler(this, args)；
			- 完成但失败，则调用委托m_WebRequestAgentHelperErrorEventHandler；
    			- 创建 WebRequestAgentHelperErrorEventArgs 失败事件参数 args；
    			- 回调 m_WebRequestAgentHelperCompleteEventHandler(this, args)；
		
		2. 上面辅助器中的委托事件在 WebRequestAgent 的 Initialize() 中赋值；
			- 上面成功事件回调到 WebRequestAgent 的 OnWebRequestAgentHelperComplete(object sender, WebRequestAgentHelperCompleteEventArgs e)；
    			- 再调用委托 WebRequestAgentSuccess(this, e.GetWebResponseBytes());
  			- 上面失败事件回调到  WebRequestAgent 的 OnWebRequestAgentHelperError(object sender, WebRequestAgentHelperErrorEventArgs e)；
    			- 再调用委托 WebRequestAgentFailure(this, e.ErrorMessage);
    	
		3. 上面的委托是在 WebRequestManager 的添加Web请求代理辅助器中赋值，即AddWebRequestAgentHelper；
            - 上面成功事件回调到 WebRequestManager 的 OnWebRequestAgentSuccess(WebRequestAgent sender, byte[] webResponseBytes)；
                - 先创建事件参数 WebRequestSuccessEventArgs.Create(sender.Task.SerialId, ..., sender.Task.UserData)；
                - 再调用到委托 m_WebRequestSuccessEventHandler(this, webRequestSuccessEventArgs)；
            - 上面失败事件回调到 WebRequestManager 的 OnWebRequestAgentFailure(WebRequestAgent sender, string errorMessage)；
                - 先创建事件参数 WebRequestFailureEventArgs.Create(sender.Task.SerialId, ..., sender.Task.UserData)；
                - 再调用到委托 m_WebRequestFailureEventHandler(this, webRequestFailureEventArgs)；
		
		4. 上面的委托是在 WebRequestComponent的Awake()中赋值的的；
            - 上面成功事件回调到 OnWebRequestSuccess(object sender, BaseFramework.WebRequest.WebRequestSuccessEventArgs e)；
        		- 先创建 WebRequestSuccessEventArgs 事件参数，其中 e.UserData 为 WWWFormInfo wwwFormInfo，wwwFormInfo.UserData 则为发送者；
        		- m_EventComponent.Fire(this, WebRequestSuccessEventArgs.Create(e))，抛出事件；
      		- 上面失败事件回调到 OnWebRequestFailure(object sender, BaseFramework.WebRequest.WebRequestFailureEventArgs e)；
        		- 先创建 WebRequestFailureEventArgs 事件参数，其中 e.UserData 为 WWWFormInfo wwwFormInfo，wwwFormInfo.UserData 则为发送者；
        		- m_EventComponent.Fire(this, WebRequestFailureEventArgs.Create(e));
        
		5. 上面抛出事件，回调到注册事件的地方
            - EventComponent.Fire(object sender, T e) -> EventManager.Fire(...) -> EventPool.Fire(...) -> EventPool.HandleEvent(object sender, T e);
            - HandleEvent(object sender, T e)
                - m_EventHandlers.TryGetValue(e.Id, out range) 根据事件参数的Id，获取事件委托；
                - 直接调用上面获取到的委托；
            - 回调到注册的函数，参考3.1 的 OnWebRequestSuccess()，OnWebRequestFailure()；
		
