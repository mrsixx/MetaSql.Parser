using System;
using System.Collections.Generic;

namespace MetaSql.Parser.Interfaces
{
    public interface IQueryParser
    {
        QueryMetadata ExtractQueryMetadata(string query);
    }
}
