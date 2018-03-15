using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudyUp
{
    public interface IClock
    {
        DateTime Now { get; }
    }
}
