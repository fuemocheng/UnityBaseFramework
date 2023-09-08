using BaseFramework.Runtime;

namespace Server
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Server server = new Server();
                server.Start();

                while(true)
                {
                    try
                    {
                        Thread.Sleep(2);
                        server.Update();
                    }
                    catch(ThreadAbortException e)
                    {
                        Log.Error(e.ToString());
                        return;
                    }
                    catch (Exception e)
                    {
                        Log.Error(e.ToString());
                    }
                }
            }
            catch (ThreadAbortException e)
            {
                Log.Error(e.ToString());
                return;
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
        }
    }
}