using MetaSql.Parser;
using MetaSql.Parser.Exceptions;
using MetaSql.Parser.Interfaces;
using MetaSql.Parser.Models;
using static MetaSql.Parser.Enums.DefaultValueFuncionEnum;

namespace MetaSql.Tests
{
    public class FiltersTests
    {
        private readonly IQueryParser _queryParser;
        public FiltersTests()
        {

            _queryParser = new QueryParser();
        }

        [Fact]
        public void QueryWithCustomFilters()
        {
            var query = @"EFILTER &filtroDtDe DATE 'Cdata de' DEFAULT ontem();
                        EFILTER &filtroDtAte DATE AS 'Cdata até' DEFAULT hoje();
                        select first 10 * from arqos
                            where (1=1)
	                        and cdata between &filtroDtDe and &filtroDtAte
                            order by cdata desc";

            var metadata = _queryParser.ExtractQueryMetadata(query);

            Assert.Equal(2, metadata.Filters.Count);
            Assert.Contains(metadata.Filters,
                filter => filter.Name == "filtroDtDe"
                && filter.Alias == "Cdata de"
                && filter.DefaultValue is FilterFunctionValue defaultValue
                && defaultValue.Equals(DefaultValueFunction.Ontem));
            Assert.Contains(metadata.Filters,
                filter => filter.Name == "filtroDtAte"
                && filter.Alias == "Cdata até"
                && filter.DefaultValue is FilterFunctionValue defaultValue
                && defaultValue.Equals(DefaultValueFunction.Hoje));

        }

        [Fact]
        public void QueryWithCustomFilterDetail()
        {
            var query = @"EFILTER &filtro1 INTEGER 'Código x' DEFAULT 20 DETAIL;
                        EFILTER &filtro2 INTEGER AS 'Código pai' DETAIL;
                        select first 10 * from arqos
                            where (1=1)
	                        and codseq between &filtro1 and &filtro2
                            order by cdata desc";

            var metadata = _queryParser.ExtractQueryMetadata(query);

            Assert.Equal(2, metadata.Filters.Count);
            Assert.Contains(metadata.Filters,
                filter => filter.Name == "filtro1"
                && !filter.IsDetail
                && filter.DefaultValue is FilterIntegerValue defaultValue
                && defaultValue.Equals(20));
            Assert.Contains(metadata.Filters,
                filter => filter.Name == "filtro2"
                && filter.IsDetail
                && filter.DefaultValue is null);
        }

        [Fact]
        public void QueryWithCustomFiltersRequiredParameters()
        {
            var query = @"EFILTER &filtroDtDe DATE 'Cdata de';
                        EFILTER &filtroDtAte DATE DEFAULT hoje();
                        select first 10 * from arqos
                            where (1=1)
	                        and cdata between &filtroDtDe and &filtroDtAte
                            or cdata > &filtroDtDe
                            order by cdata desc";

            var metadata = _queryParser.ExtractQueryMetadata(query);

            Assert.Equal(2, metadata.Filters.Count);

            Assert.Equal(1, metadata.Filters.Count(filter => filter.HasAlias));
            Assert.Equal(1, metadata.Filters.Count(filter => filter.HasDefaultValue));
            Assert.Contains(metadata.Filters,
                filter => filter.Name == "filtroDtDe" && filter.Alias == "Cdata de");
            Assert.Contains(metadata.Filters,
                filter => filter.Name == "filtroDtAte"
                && filter.DefaultValue is FilterFunctionValue defaultValue
                && defaultValue.Equals(DefaultValueFunction.Hoje));

        }

        [Fact]
        public void QueryWithCustomFiltersMismatchedDefaultValues()
        {
            Assert.Throws<UnrecognizedTypeException>(() => _queryParser.ExtractQueryMetadata("EFILTER &filtro1 DATE DEFAULT diaatual;"));
            Assert.Throws<InvalidFunctionTypeException>(() => _queryParser.ExtractQueryMetadata("EFILTER &filtro1 DATE DEFAULT diaatual();"));
            Assert.Throws<InvalidFunctionTypeException>(() => _queryParser.ExtractQueryMetadata("EFILTER &filtro2 DATETIME DEFAULT diaatual();"));
            Assert.Throws<InvalidFunctionTypeException>(() => _queryParser.ExtractQueryMetadata("EFILTER &filtro4 TEXT DEFAULT anoatual();"));
            Assert.Throws<InvalidFunctionTypeException>(() => _queryParser.ExtractQueryMetadata("EFILTER &filtro5 DATE DEFAULT usuario();"));
            Assert.Throws<MismatchedTypesException>(() => _queryParser.ExtractQueryMetadata("EFILTER &filtro6 DATE DEFAULT 50.2;"));
            Assert.Throws<MismatchedTypesException>(() => _queryParser.ExtractQueryMetadata("EFILTER &filtro7 DATE DEFAULT 'Hakuna Matata';"));
            //Assert.Throws<UnrecognizedTypeException>(() => _queryParser.ExtractQueryMetadata("EFILTER &filtro8 DATE DEFAULT Excelsior()"));
        }

        [Fact]
        public void QueryWithUnmapedCustomFilter()
        {
            var query = @"SELECT * FROM USUARIO WHERE ID = &filtroUsuario";
            Assert.Throws<UndeclaredFilterException>(() => _queryParser.ExtractQueryMetadata(query));
        }


        [Fact]
        public void QueryWithDuplicatedCustomFilterName()
        {
            var query = @"   EFILTER &filtroOntem DATE DEFAULT ontem();
                            EFILTER &filtroOntem DATE DEFAULT hoje();

                        select first 10 * from arqos
                            where (1=1)
	                        and (cdata between &filtroOntem and &filtroOntem)
                            order by cdata desc";
            Assert.Throws<InvalidOperationException>(() => _queryParser.ExtractQueryMetadata(query));
        }

        [Fact]
        public void QueryWithCustomFiltersDefaultValueFunction()
        {
            var query = @"
                        EFILTER &filtro1 INTEGER DEFAULT usuario();
                        EFILTER &filtro2 INTEGER DEFAULT diaatual();
                        EFILTER &filtro3 INTEGER DEFAULT mesatual();
                        EFILTER &filtro4 INTEGER DEFAULT anoatual();
                        EFILTER &filtro5 DATE DEFAULT ontem();
                        EFILTER &filtro6 DATE DEFAULT hoje();
                        EFILTER &filtro7 DATE DEFAULT inicio_mes_atual();
                        EFILTER &filtro8 DATE DEFAULT fim_mes_atual();

                        select first 10 * from arqos
                            where (1=1)
	                        and (cdata between &filtro5 and &filtro6
                            order by cdata desc";

            var metadata = _queryParser.ExtractQueryMetadata(query);


            Assert.Equal(8, metadata.Filters.Count);
            Assert.Contains(metadata.Filters,
                filter => filter.Name == "filtro1"
                          && filter.DefaultValue is FilterFunctionValue functionValue
                          && functionValue.Equals(DefaultValueFunction.Usuario));
            Assert.Contains(metadata.Filters,
                filter => filter.Name == "filtro2"
                          && filter.DefaultValue is FilterFunctionValue functionValue
                          && functionValue.Equals(DefaultValueFunction.DiaAtual));
            Assert.Contains(metadata.Filters,
                filter => filter.Name == "filtro3"
                          && filter.DefaultValue is FilterFunctionValue functionValue
                          && functionValue.Equals(DefaultValueFunction.MesAtual));
            Assert.Contains(metadata.Filters,
                filter => filter.Name == "filtro4"
                          && filter.DefaultValue is FilterFunctionValue functionValue
                          && functionValue.Equals(DefaultValueFunction.AnoAtual));
            Assert.Contains(metadata.Filters,
                filter => filter.Name == "filtro5"
                          && filter.DefaultValue is FilterFunctionValue functionValue
                          && functionValue.Equals(DefaultValueFunction.Ontem));
            Assert.Contains(metadata.Filters,
                filter => filter.Name == "filtro6"
                          && filter.DefaultValue is FilterFunctionValue functionValue
                          && functionValue.Equals(DefaultValueFunction.Hoje));
            Assert.Contains(metadata.Filters,
                filter => filter.Name == "filtro7"
                          && filter.DefaultValue is FilterFunctionValue functionValue
                          && functionValue.Equals(DefaultValueFunction.InicioMesAtual));
            Assert.Contains(metadata.Filters,
                filter => filter.Name == "filtro8"
                          && filter.DefaultValue is FilterFunctionValue functionValue
                          && functionValue.Equals(DefaultValueFunction.FimMesAtual));

        }

        [Fact]
        public void QueryWithEcalcFiltersDefaultValueConstant()
        {
            var query = @"
                        EFILTER &filtro1 INTEGER DEFAULT 10;
                        EFILTER &filtro2 DECIMAL DEFAULT 2123.5;
                        EFILTER &filtro3 DATE DEFAULT '2024-04-01';
                        EFILTER &filtro4 DATETIME DEFAULT '2024-04-01 12:35:00';
                        EFILTER &filtro5 TEXT DEFAULT 'Lorem ipsum dolor sit amet';

                        select first 10 * from arqos
                            where (1=1)
	                        and (cdata between &filtro5 and &filtro4)
                            and (codseq = &filtro1 or codseq != &filtro2 or codseq != &filtro2)
                            order by cdata desc";

            var metadata = _queryParser.ExtractQueryMetadata(query);

            Assert.Equal(5, metadata.Filters.Count);

            Assert.Contains(metadata.Filters,
                filter => filter.Name == "filtro1"
                          && filter.DefaultValue is FilterIntegerValue defaultValue
                          && defaultValue.Equals(10)
                          && defaultValue.SqlFormattedValue.Equals("10"));

            Assert.Contains(metadata.Filters,
                filter => filter.Name == "filtro2"
                          && filter.DefaultValue is FilterDecimalValue defaultValue
                          && defaultValue.Equals(2123.5m)
                          && defaultValue.SqlFormattedValue.Equals("2123.5"));

            Assert.Contains(metadata.Filters,
                filter => filter.Name == "filtro3"
                          && filter.DefaultValue is FilterDateValue defaultValue
                          && defaultValue.Equals(new DateTime(2024, 4, 1))
                          && defaultValue.SqlFormattedValue.Equals("'2024-04-01'"));

            Assert.Contains(metadata.Filters,
                filter => filter.Name == "filtro4"
                          && filter.DefaultValue is FilterDateTimeValue defaultValue
                          && defaultValue.Equals(new DateTime(2024, 4, 1, 12, 35, 0))
                          && defaultValue.SqlFormattedValue.Equals("'2024-04-01 12:35:00'"));

            Assert.Contains(metadata.Filters,
                filter => filter.Name == "filtro5"
                          && filter.DefaultValue is FilterTextValue defaultValue
                          && defaultValue.Equals("Lorem ipsum dolor sit amet")
                          && defaultValue.SqlFormattedValue.Equals("'Lorem ipsum dolor sit amet'"));
        }

        [Fact]
        public void QueryWithHiddenCustomFilter()
        {
            var query = @"EFILTER &filtroOntem DATE HIDDEN DEFAULT ontem();
                        EFILTER &filtroHoje DATE DEFAULT hoje();

                        select first 10 * from arqos
                            where (1=1)
	                        and (cdata between &filtroOntem and &filtroHoje)
                            order by cdata desc";
            var metadata = _queryParser.ExtractQueryMetadata(query);
            Assert.Contains(metadata.Filters, filter => filter.Name == "filtroOntem" && filter.Hidden);
            Assert.Contains(metadata.Filters, filter => filter.Name == "filtroHoje" && !filter.Hidden);
        }

        [Fact]
        public void QueryWithHiddenCustomFilterNameWithoutDefaultValue()
        {
            var query = @"   EFILTER &filtroOntem DATE HIDDEN;

                        select first 10 * from arqos
                            where (1=1)
	                        and cdata > &filtroOntem
                            order by cdata desc";
            Assert.Throws<InvalidOperationException>(() => _queryParser.ExtractQueryMetadata(query));
        }

        [Fact]
        public void QueryWithRangeFunctionFilters()
        {
            var query = @"EFILTER &filtroDt DATETIMERANGE 'Cdata' DEFAULT (ontem(),hoje());
                        select first 10 * from arqos
                            where (1=1)
	                        and cdata between &filtroDt[0] and &filtroDt[1]
                            order by cdata desc";

            var metadata = _queryParser.ExtractQueryMetadata(query);

            Assert.Single(metadata.Filters);
            Assert.Contains(metadata.Filters,
                filter => filter.Name == "filtroDt"
                && filter.Alias == "Cdata"
                && filter.Type == Parser.Enums.FilterTypeEnum.FilterType.DateTimeRange
                && filter.DefaultValue is FilterDatetimeRangeValue defaultValue
                && defaultValue.Equals(new Tuple<FilterValue, FilterValue>(
                    new FilterFunctionValue(DefaultValueFunction.Ontem), 
                    new FilterFunctionValue(DefaultValueFunction.Hoje))
                ));
        }

        [Fact]
        public void QueryWithRangeLiteralValueFilters()
        {
            var query = @"EFILTER &filtroDt DATETIMERANGE 'Cdata' DEFAULT ('2024-04-01 12:35:00','2024-04-01 12:40:30');
                        select first 10 * from arqos
                            where (1=1)
	                        and cdata between &filtroDt:start and &filtroDt:end
                            order by cdata desc";

            var metadata = _queryParser.ExtractQueryMetadata(query);

            Assert.Single(metadata.Filters);
            Assert.Contains(metadata.Filters,
                filter => filter.Name == "filtroDt"
                && filter.Alias == "Cdata"
                && filter.Type == Parser.Enums.FilterTypeEnum.FilterType.DateTimeRange
                && filter.DefaultValue is FilterDatetimeRangeValue defaultValue
                && defaultValue.Equals(new Tuple<FilterValue, FilterValue>(
                    new FilterDateTimeValue(new DateTime(2024, 4, 1, 12, 35, 0)),
                    new FilterDateTimeValue(new DateTime(2024, 4, 1, 12, 40, 30)))
                ));
        }

        [Fact]
        public void QueryWithExpressionFilters()
        {
            var query =
                @"EFILTER &vendedor TEXT AS 'Código do Vendedor' DEFAULT a.codvend1;
                select
                    extract(month from a.cdata) as mes,
                    extract(year from a.cdata) as ano,
                    extract(month from a.cdata) || '/' || extract(year from a.cdata) as data,
                    sum(a.cpfinal) as total_venda
                from arqos a
                join vendedor v on (v.ccodigo = a.codvend1 )
                where (1 = 1)
                and a.cdata BETWEEN  DATEADD(YEAR, -2, CURRENT_DATE) AND CURRENT_DATE
                --AND (v.ccodigo IN (&vendedor) OR v.ccodigo = a.codvend1)
                  AND v.ccodigo = (&vendedor) 
                and a.cgeraos <> -1
                group by 1,2,3
                order by 1,2
                ";
            var metadata = _queryParser.ExtractQueryMetadata(query);

            Assert.Single(metadata.Filters);
            Assert.Contains(metadata.Filters,
                filter => filter.Name == "vendedor"
                          && filter is
                          {
                              Alias: "Código do Vendedor", 
                              Type: Parser.Enums.FilterTypeEnum.FilterType.Text, 
                              DefaultValue: FilterSQLExpressionValue defaultValue
                          }
                          && defaultValue.Equals("a.codvend1"));
        }
    }
}
