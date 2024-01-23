using System;
using System.Collections.Generic;
using System.Text;

namespace withersdk.Net.General.Nodes
{
    public interface INode
    {
        string Type { get; }
        byte[] Body { get; }

        string Tag { get; }
        DateTime Timestamp { get; }
        byte[] PackToStream();
    }
}
