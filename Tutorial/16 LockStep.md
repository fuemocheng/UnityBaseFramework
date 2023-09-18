# LockStep 帧同步模块
## 1.笔记

Client：
Launcher.cs ->DoStart -> 
_mgrContainer.AllMgrs.DoStart() -> 
	NetworkService.DoStart() -> 
		RoomMsgManager.ConnectToGameServer() -> 
			SendTcp(EMsgSC.C2L_JoinRoom...)


Server：
收到加入房间的消息：
Server.cs -> OnNetMsg -> C2L_JoinRoom -> 
	PlayerConnected 人数足够时 ->
		Game.DoStart(...) -> 
			状态设置为 State = EGameState.Loading; 构建每个人的 GameData;
			OnRecvPlayerGameData(), 所有玩家的数据GameData都构建好 ->
				每个玩家给自己发送 EMsgSC.G2C_Hello ->
				同时广播所有人此局游戏信息，EMsgSC.G2C_GameStartInfo (GameStartInfo)


Client:
收到 G2C_Hello：
RoomMsgManager.OnNetMsg -> G2C_Hello() -> 
	EventHelper.Trigger(EEvent.OnServerHello, msg) ->
		SimulatorService.OnEvent_OnServerHello -> 
			记录每个玩家此局的LocalId;
			LocalActorId = msg.LocalId;

收到 G2C_GameStartInfo:
RoomMsgManager.OnNetMsg -> G2C_GameStartInfo() -> 
	记录数据，处理一些逻辑；
	CurGameState = EGameState.Loading; 状态设置为 Loading；
	EventHelper.Trigger(EEvent.OnGameCreate, msg); ->
		SimulatorService.OnEvent_OnGameCreate
		SimulatorService.OnGameCreate() ->
			初始化 FrameBuffer，记录 allActors;
			_world.StartSimulate() 初始化，并Start各个系统 ......
			EventHelper.Trigger(EEvent.LevelLoadProgress, 1f);  ->
				NetworkService.OnEvent_LevelLoadProgress() ->
					RoomMsgManager.OnLevelLoadProgress() -> 
						当进度达到100，则 CurGameState = EGameState.PartLoaded，加载部分完成，
						并发送 C2G_LoadingProgress；
					CheckLoadingProgress()->
						处理断线重连的逻辑；
		EventHelper.Trigger(EEvent.SimulationInit, null); ->
			--DoNothing;
			
			
Server:
收到 C2G_LoadingProgress：
Game.OnNetMsg(...) -> Game.C2G_LoadingProgress() ->
	记录每个玩家的加载进度，并广播给其他玩家，BorderTcp(EMsgSC.G2C_LoadingProgress ... ) ->
	如果每个玩家都加载完成，则 OnFinishedLoaded() ->
		更新状态 State = EGameState.PartLoaded; ->
		广播所有人加载完成的消息 BorderTcp(EMsgSC.G2C_AllFinishedLoaded ...);


Client：
收到 G2C_AllFinishedLoaded：
RoomMsgManager.OnNetMsg -> G2C_AllFinishedLoaded() -> 
	NetworkMsgHandler.OnAllFinishedLoaded() -> 
	EventHelper.Trigger(EEvent.OnAllPlayerFinishedLoad, level); ->
		SimulatorService.OnEvent_OnAllPlayerFinishedLoad() ->
			SimulatorService.StartSimulate()->
			_world.StartGame(_gameStartInfo, LocalActorId);
				CreateEntity(...) 创建Player之类的。
			EventHelper.Trigger(EEvent.SimulationStart, null); 客户端启动游戏生命周期；
				...do nothing
			(inputTick < PreSendInputCount) -> SendInputs(inputTick++)
				从第一帧开始，一些帧逻辑的判断
				FrameBuffer.SendInput(input) -> 
					NetworkService.SendInput -> 
						RoomMsgManager.SendInput(msg) ->
							SendTcp(EMsgSC.C2G_PlayerInput, ...)
	

Server:
收到 C2G_PlayerInput:
Game.OnNetMsg(...) -> Game.C2G_PlayerInput() ->
	收到任意第一个输入，则 State = EGameState.Playing，设置状态为 Playing，
	处理一些帧数据的逻辑 ->
	_CheckBorderServerFrame 检查是否向客户端广播帧；->
		检查所有的输入是否都已经等到，时间未到，包未到，则退出不广播；
		时间到了，则未到的输入包则给予默认输入；
		BorderTcp(EMsgSC.G2C_FrameData, msg); 广播帧数据；


Client
收到 G2C_FrameData：
RoomMsgManager.OnNetMsg -> G2C_FrameData() -> 
	NetworkMsgHandler.OnServerFrames() ->
	EventHelper.Trigger(EEvent.OnServerFrame, msg); ->
		SimulatorService.OnEvent_OnServerFrame() ->
			_hasRecvInputMsg = true;
			FrameBuffer.PushServerFrames(...)
			

在不断更新中，客户端不断发送 C2G_PlayerInput；
服务器在接收到所有人输入或者时间帧到了，则回 G2C_FrameData；





	












