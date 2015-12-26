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
    }
}
