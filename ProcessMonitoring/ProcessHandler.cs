using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.JustMock.AutoMock.Ninject.Infrastructure.Language;

namespace ProcessMonitoring
{
    public class ProcessHandler : IProcessHandler
    {
        public IProcessWrapper[] GetProcessesByName(string name)
        {
            return Array.ConvertAll(Process.GetProcessesByName(name), process => new ProcessWrapper(process));
        }
    }
}
