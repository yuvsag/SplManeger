using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SplManeger.Logic.Utils;

namespace SplManeger.Tests.Logic.Utils
{

    internal class FrozenClock : IAppClock
    {
        private readonly DateTimeOffset _date;
        public FrozenClock(DateTimeOffset atDate)
        {
            _date = atDate;
        }
        public DateTimeOffset UtcNow()
        {
            return _date;
        }


        /// <summary>
        /// stuck at 9/11 EST since we must never forget :(
        /// </summary>
        public static FrozenClock Default = new(new(new DateTime(2001, 9, 11, 8, 46, 32), TimeSpan.FromHours(-5)));
    }
}
