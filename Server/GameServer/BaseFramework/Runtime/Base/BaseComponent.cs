using BaseFramework;

namespace BaseFramework.Runtime
{
    /// <summary>
    /// 基础组件。
    /// </summary>
    public sealed class BaseComponent : BaseFrameworkComponent
    {
        private string m_TextHelperTypeName = "BaseFramework.Runtime.DefaultTextHelper";

        private string m_LogHelperTypeName = "BaseFramework.Runtime.DefaultLogHelper";

        private string m_JsonHelperTypeName = "BaseFramework.Runtime.DefaultJsonHelper";


        /// <summary>
        /// 游戏框架组件初始化。
        /// </summary>
        public override void Awake()
        {
            base.Awake();

            InitTextHelper();
            InitLogHelper();
            InitJsonHelper();
        }

        public override void Start()
        {
        }

        internal void Shutdown()
        {
        }

        private void InitTextHelper()
        {
            if (string.IsNullOrEmpty(m_TextHelperTypeName))
            {
                return;
            }

            Type textHelperType = Utility.Assembly.GetType(m_TextHelperTypeName);
            if (textHelperType == null)
            {
                Log.Error("Can not find text helper type '{0}'.", m_TextHelperTypeName);
                return;
            }

            Utility.Text.ITextHelper textHelper = (Utility.Text.ITextHelper)Activator.CreateInstance(textHelperType);
            if (textHelper == null)
            {
                Log.Error("Can not create text helper instance '{0}'.", m_TextHelperTypeName);
                return;
            }

            Utility.Text.SetTextHelper(textHelper);
        }

        private void InitLogHelper()
        {
            if (string.IsNullOrEmpty(m_LogHelperTypeName))
            {
                return;
            }

            Type logHelperType = Utility.Assembly.GetType(m_LogHelperTypeName);
            if (logHelperType == null)
            {
                throw new BaseFrameworkException(Utility.Text.Format("Can not find log helper type '{0}'.", m_LogHelperTypeName));
            }

            BaseFrameworkLog.ILogHelper logHelper = (BaseFrameworkLog.ILogHelper)Activator.CreateInstance(logHelperType);
            if (logHelper == null)
            {
                throw new BaseFrameworkException(Utility.Text.Format("Can not create log helper instance '{0}'.", m_LogHelperTypeName));
            }

            BaseFrameworkLog.SetLogHelper(logHelper);
        }

        private void InitJsonHelper()
        {
            if (string.IsNullOrEmpty(m_JsonHelperTypeName))
            {
                return;
            }

            Type jsonHelperType = Utility.Assembly.GetType(m_JsonHelperTypeName);
            if (jsonHelperType == null)
            {
                Log.Error("Can not find JSON helper type '{0}'.", m_JsonHelperTypeName);
                return;
            }

            Utility.Json.IJsonHelper jsonHelper = (Utility.Json.IJsonHelper)Activator.CreateInstance(jsonHelperType);
            if (jsonHelper == null)
            {
                Log.Error("Can not create JSON helper instance '{0}'.", m_JsonHelperTypeName);
                return;
            }

            Utility.Json.SetJsonHelper(jsonHelper);
        }
    }
}
