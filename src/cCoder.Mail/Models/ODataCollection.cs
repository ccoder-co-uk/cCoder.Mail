// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace cCoder.Mail.Models;

public class ODataCollection<TCollectionType>
{
    [JsonProperty("@odata.context")]
    public string ODataContext { get; set; }

    public IEnumerable<TCollectionType> Value { get; set; }
}