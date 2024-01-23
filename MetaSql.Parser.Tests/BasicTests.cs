using MetaSql.Parser;
using MetaSql.Parser.Interfaces;
using System.Text.RegularExpressions;

namespace MetaSql.Tests
{
    public class BasicTests
    {
        private readonly IQueryParser _queryParser;

        public BasicTests()
        {
            _queryParser = new QueryParser();
        }

        [Fact]
        public void SimpleQuery()
        {
            var metadata = _queryParser.ExtractQueryMetadata("SELECT * FROM SENHA WHERE CODSEQ = 0");
            Assert.Single(metadata.Tables);
            Assert.Contains(metadata.Tables, table => table.TableName == "SENHA");
            Assert.Empty(metadata.Relations);
        }


        [Fact]
        public void QueryIfMultipleJoins()
        {
            var query = @"SELECT m.maquina AS maquina,
                                   o.nome AS operador,
                                   t.inicio AS inicio,
                                   t.os AS os,
                                   procs.referencia AS processo
                            FROM tapont t
                            JOIN eventos e ON (e.codseq = t.codevento)
                            JOIN operador o ON (o.codseq = t.codoperador)
                            JOIN maquinas m ON (m.codseq = t.codmaquina)
                            JOIN procs ON (procs.codseq = t.codproc)
                            WHERE (1=1)
                              AND e.prod = 0;";

            var metadata = _queryParser.ExtractQueryMetadata(query);

            Assert.Equal(4, metadata.Relations.Count);
            Assert.Contains(metadata.Relations,
                (rel) => rel.LeftTable == "EVENTOS.CODSEQ" && rel.RightTable == "TAPONT.CODEVENTO");
            Assert.Contains(metadata.Relations,
                (rel) => rel.LeftTable == "OPERADOR.CODSEQ" && rel.RightTable == "TAPONT.CODOPERADOR");
            Assert.Contains(metadata.Relations,
                (rel) => rel.LeftTable == "MAQUINAS.CODSEQ" && rel.RightTable == "TAPONT.CODMAQUINA");
            Assert.Contains(metadata.Relations,
                (rel) => rel.LeftTable == "PROCS.CODSEQ" && rel.RightTable == "TAPONT.CODPROC");
        }

        [Fact]
        public void ExtractRealSqlQuery()
        {
            var extendedQuery = @"EFILTER &filtroDtDe DATE 'Cdata de';
                        EFILTER &filtroDtAte DATE DEFAULT hoje;
                        select first 10 * from arqos
                            where (1=1)
	                        and cdata between &filtroDtDe and &filtroDtAte
                            order by cdata desc";

            var sqlQuery = "select first 10 * from arqos where (1=1) and cdata between @filtroDtDe and @filtroDtAte order by cdata desc";

            var metadata = _queryParser.ExtractQueryMetadata(extendedQuery);


            Assert.Equal(sqlQuery, metadata.ResultQuery);
            Assert.DoesNotContain("EFILTER", metadata.ResultQuery);
        }

        [Fact]
        public void SelectAllFromISqlProcedure()
        {
            var extendedQuery = @"
                    EFILTER &filtro1 INTEGER AS 'Cod.';
                    EFILTER &filtro2 INTEGER AS 'Cod 2.';
                    SELECT * FROM PROCEDURE(&filtro1, &filtro2);";

            var sqlQuery = "SELECT * FROM PROCEDURE(@filtro1, @filtro2);";

            var metadata = _queryParser.ExtractQueryMetadata(extendedQuery);
            Assert.Equal(sqlQuery, metadata.ResultQuery);
        }


        [Fact]
        public void SelectColumnsFromISqlProcedure()
        {
            var extendedQuery = @"
                    EFILTER &filtro1 INTEGER AS 'Cod.';
                    EFILTER &filtro2 INTEGER AS 'Cod 2.';
                    SELECT UUID, NOME, SOBRENOME AS 'Sobrenome' FROM PROCEDURE(&filtro1, &filtro2)
                    WHERE 1 = 1;";

            var sqlQuery = "SELECT UUID, NOME, SOBRENOME AS 'Sobrenome' FROM PROCEDURE(@filtro1, @filtro2) WHERE 1 = 1;";

            var metadata = _queryParser.ExtractQueryMetadata(extendedQuery);
            Assert.Equal(sqlQuery, metadata.ResultQuery);
        }

        [Fact]
        public void ExecuteProcedureWithSelectQuery()
        {
            var extendedQuery = @"
                    EFILTER &filtro1 INTEGER AS 'Cod.';
                    EFILTER &filtro2 INTEGER AS 'Cod 2.';
                    EXEC PROCEDURE_NAME &filtro1, &filtro2;
                    SELECT UUID, NOME, SOBRENOME AS 'Sobrenome' FROM #TempTable
                    WHERE 1 = 1;";

            var sqlQuery = "SELECT UUID, NOME, SOBRENOME AS 'Sobrenome' FROM #TempTable WHERE 1 = 1;";
            var sqlExec = "EXEC PROCEDURE_NAME @filtro1, @filtro2;";

            var metadata = _queryParser.ExtractQueryMetadata(extendedQuery);
            Assert.Equal(sqlQuery, metadata.ResultQuery);
            Assert.Single(metadata.Executes);
            Assert.Contains(metadata.Executes, e => e.Arguments.Contains("filtro1"));
            Assert.Contains(metadata.Executes, e => e.Arguments.Contains("filtro2"));
            Assert.Contains(metadata.Executes, e => e.Text.Equals(sqlExec));
        }
    }
}