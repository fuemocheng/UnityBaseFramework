namespace XGame
{
    public abstract class ProcedureBase : BaseFramework.Procedure.ProcedureBase
    {
        // 获取流程是否使用原生对话框
        // 在一些特殊的流程（如游戏逻辑对话框资源更新完成前的流程）中，可以考虑调用原生对话框进行消息提示行为
        public abstract bool UseNativeDialog
        {
            get;
        }
    }
}
