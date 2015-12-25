using System;

namespace AzureQueueApp
{
    [Flags]
	public enum Operation
    {
		Unknown = 0,
        InsertMessage = 1,
        PeekMessage = 2,
        PeekMessages = 3,
        GetMessage = 4,
        GetMessages = 5
    }
}
