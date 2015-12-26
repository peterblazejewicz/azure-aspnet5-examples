using System;

namespace AzureQueueApp.Models
{
    [Flags]
	public enum Operation
    {
		Unknown = 0,
        InsertMessage = 1,
        PeekMessage = 2,
        ChangeMessage = 3,
        RemoveMessage = 4,
        GetLength = 5,
        ClearMessages = 6,
    }
}
