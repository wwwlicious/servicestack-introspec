// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.Models
{
    public class RelativePath
    {
        public string Path { get; set; }
        public string Source { get; set; }

        public static implicit operator RelativePath(string path)
            => new RelativePath { Path = path };

        public static implicit operator string(RelativePath path)
            => path?.Path;

        public bool IsAutoRoute => Source == Constants.RouteSources.AutoRoute;
        public bool IsFallback => Source == Constants.RouteSources.FallbackRoute;
    }
}