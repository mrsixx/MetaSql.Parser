using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using MetaSql.Parser.Interfaces;
using System.IO;

namespace MetaSql.Parser
{
    public class QueryParser : IQueryParser
    {
        private readonly ParseTreeWalker _walker;

        public QueryParser()
        {
            _walker = new ParseTreeWalker();
        }

        public QueryMetadata ExtractQueryMetadata(string query)
        {
            var reader = new StringReader(query);
            var charStream = new CaseChangingCharStream(new AntlrInputStream(reader), true);
            var lexer = new TSqlLexer(charStream);
            var tokenStream = new CommonTokenStream(lexer);
            var parser = new TSqlParser(tokenStream);
            var fileCtx = parser.tsql_file();
            var listener = new TSqListener(query);
            _walker.Walk(listener, fileCtx);
            return listener.Metadata;
        }
    }
}
