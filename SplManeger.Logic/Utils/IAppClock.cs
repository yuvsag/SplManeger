using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplManeger.Logic.Utils;

public interface IAppClock
{
    DateTimeOffset UtcNow();



    public static readonly IAppClock Default = new DefaultClock();
    class DefaultClock : IAppClock
    {
        public DateTimeOffset UtcNow() => DateTimeOffset.UtcNow;
    }
}
