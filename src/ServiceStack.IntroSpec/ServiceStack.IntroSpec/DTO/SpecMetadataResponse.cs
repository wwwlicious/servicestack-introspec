// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this 
// file, You can obtain one at http://mozilla.org/MPL/2.0/.

namespace ServiceStack.IntroSpec.DTO
{
    public class SpecMetadataResponse
    {
        public string[] DtoNames { get; set; }
        public string[] Categories { get; set; }
        public string[] Tags { get; set; }

        public static SpecMetadataResponse Create(string[] dtoNames, string[] categories, string[] tags)
            => new SpecMetadataResponse { Categories = categories, DtoNames = dtoNames, Tags = tags };
    }
}