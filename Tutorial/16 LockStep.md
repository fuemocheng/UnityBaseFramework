# LockStep 帧同步模块
## 1.笔记

Client：
Launcher.cs ->DoStart -> _mgrContainer.AllMgrs.DoStart() -> 
NetworkService.DoStart() -> RoomMsgManager.ConnectToGameServer() -> 
SendTcp(EMsgSC.C2L_JoinRoom...)


Server：
收到加入房间的消息：
Server.cs -> OnNetMsg -> C2L_JoinRoom -> PlayerConnected ->
人数足够 -> Game.DoStart(...) -> 
OnRecvPlayerGameData() -> 所有人的GameData都构建好 ->
每个玩家给自己发送 EMsgSC.G2C_Hello ->
同时广播所有人 EMsgSC.G2C_GameStartInfo，GameStartInfo


Client:
收到 G2C_Hello
RoomMsgManager.OnNetMsg -> G2C_Hello() -> 
SimulatorService.OnEvent_OnServerHello -> 
LocalActorId = msg.LocalId 记录 LocalId；
收到 G2C_GameStartInfo
RoomMsgManager.OnNetMsg -> G2C_GameStartInfo() -> 
处理一些逻辑，记录一些状态 -> 
SimulatorService.OnGameCreate ->
_world.StartSimulate() 初始化，并Start各个系统 ...... ->
然后发送事件 EventHelper.Trigger(EEvent.LevelLoadProgress, 1f);  ->
NetworkService.OnEvent_LevelLoadProgress() ->
RoomMsgManager.OnLevelLoadProgress() -> 
当进度达到100，则 CurGameState = EGameState.PartLoaded，加载部分完成，
并发送 C2G_LoadingProgress；

Server:
收到 C2G_LoadingProgress
Game.OnNetMsg(...) -> Game.C2G_LoadingProgress() ->
记录每个玩家的加载进度，并广播给其他玩家， BorderTcp(EMsgSC.G2C_LoadingProgress ... ) ->
如果每个玩家都加载完成，则 OnFinishedLoaded() ->
更新状态 State = EGameState.PartLoaded; ->
广播所有人加载完成的消息 BorderTcp(EMsgSC.G2C_AllFinishedLoaded ...);

Client：
收到 G2C_AllFinishedLoaded
RoomMsgManager.OnNetMsg -> G2C_AllFinishedLoaded() -> 
NetworkMsgHandler.OnAllFinishedLoaded() ->
EventHelper.Trigger(EEvent.OnAllPlayerFinishedLoad, level); ->
SimulatorService.OnEvent_OnAllPlayerFinishedLoad() ->
SimulatorService.StartSimulate()->
_world.StartGame(_gameStartInfo, LocalActorId);
	CreateEntity(...)
EventHelper.Trigger(EEvent.SimulationStart, null); 客户端启动游戏生命周期；
	...
SendInputs(inputTick++)
	从第一帧开始，一些帧逻辑的判断
	FrameBuffer.SendInput(input) -> 
	NetworkService.SendInput -> 
	RoomMsgManager.SendInput(msg) ->
	SendTcp(EMsgSC.C2G_PlayerInput, ...)
	

Server:
收到 C2G_PlayerInput
Game.OnNetMsg(...) -> Game.C2G_PlayerInput() ->
State = EGameState.Playing; 收到任意第一个输入，则开始；->
处理一些帧数据的逻辑 ->
_CheckBorderServerFrame 检查是否向客户端广播帧；->
BorderTcp(EMsgSC.G2C_FrameData, msg); 广播帧数据；


Client
收到 G2C_FrameData
RoomMsgManager.OnNetMsg -> G2C_FrameData() -> 
NetworkMsgHandler.OnServerFrames() ->
EventHelper.Trigger(EEvent.OnServerFrame, msg); ->
SimulatorService.OnEvent_OnServerFrame() ->
FrameBuffer.PushServerFrames(...)






	


















































