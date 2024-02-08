using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.CommandLine.Binding;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThumbGen.App
{
    internal class LoggerFactoryBinder : BinderBase<ILoggerFactory>
    {
        protected override ILoggerFactory GetBoundValue(
            BindingContext bindingContext)
        {
            var loggerFactory = LoggerFactory.Create(
                builder => builder
                    .AddConsole()
                    .SetMinimumLevel(LogLevel.Information));

            return loggerFactory;
        }
    }
}
