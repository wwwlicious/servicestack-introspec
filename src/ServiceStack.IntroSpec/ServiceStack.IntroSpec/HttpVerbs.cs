// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/. 
namespace ServiceStack.IntroSpec
{
    using System;

    [Flags]
    public enum HttpVerbs
    {
        Get = 1,
        Post = 2,
        Put = 4,
        Delete = 8,
        Head = 16, // 0x00000010
        Patch = 32, // 0x00000020
        Options = 64, // 0x00000040
    }
}