﻿using System;
using System.Collections.Generic;
using System.Text;

namespace NGramSearch
{
    public class ResultItem<TKeyType>
    {
        public TKeyType Id { get; internal set; }
        public double Similarity { get; internal set; }

    }
}
