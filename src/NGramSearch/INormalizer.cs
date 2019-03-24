using System;
using System.Collections.Generic;
using System.Text;

namespace NGramSearch
{
    public interface INormalizer
    {
        string Normalize(string raw);
    }
}
