﻿namespace Tripod
{
    internal class FakeEntityWithIntId : EntityWithId<int>
    {
        internal FakeEntityWithIntId(int id)
        {
            Id = id;
        }
    }
}