using System.ServiceProcess;

namespace Foundation.ServiceProcess
{
    public interface IService
    {
        bool CanPauseAndContinue { get; }
        void Initialize(ServiceBase serviceBase);
        void Start(string[] args);
        void Stop();
        void Pause();
        void Continue();
    }
}