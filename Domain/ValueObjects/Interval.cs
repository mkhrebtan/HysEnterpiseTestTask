using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ValueObjects;

public class Interval : ValueObject
{
    public DateTime Start { get; }

    public DateTime End { get; }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Start;
        yield return End;
    }

    public Interval(DateTime start, DateTime end)
    {
        if (start >= end)
        {
            throw new ArgumentException("Start time must be earlier than end time.");
        }

        Start = start;
        End = end;
    }
}
