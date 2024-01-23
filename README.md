# MetaSql.Parser

[![.NET](https://github.com/mrsixx/MetaSql.Parser/actions/workflows/dotnet.yml/badge.svg?branch=master)](https://github.com/mrsixx/MetaSql.Parser/actions/workflows/dotnet.yml)

## Overview

**MetaSql.Parser** is a C# .NET Standard 2.0 library designed to extend the SQL language from its grammar. The primary purpose of this parser is to read SQL queries and extract ANSI SQL metadata (tables, columns) along with extended metadata (filter statements, etc.).

## Usage

To get started with MetaSql.Parser, follow these simple steps:

1. **Installation**
   ```bash
   dotnet add package MetaSql.Parser
  
2. **Examples**
```
IQueryParser parser = new QueryParser();
var query = @"EFILTER &filtroOntem DATE DEFAULT ontem;
              EFILTER &filtroHoje DATE DEFAULT hoje;
                SELECT m.maquina AS maquina,
                       o.nome AS operador,
                       t.inicio AS inicio,
                       t.os AS os,
                       procs.referencia AS processo
                FROM apontamento t
                JOIN eventos e ON (e.codseq = t.codevento)
                JOIN operadores o ON (o.codseq = t.codoperador)
                JOIN maquinas m ON (m.codseq = t.codmaquina)
                JOIN operacoes ON (processos.codseq = t.codproc)
                WHERE (1=1)
                  AND t.inicio BETWEEN &filtroOntem AND &filtroHoje;";
var metadata = parser.ExtractQueryMetadata(query);
// metadata.Tables => [{TableName: 'apontamento'},{TableName: 'operadores'},{TableName: 'maquinas'},{TableName: 'operacoes'}]
// metadata.Relations => [{LeftTable: 'EVENTOS.CODSEQ', RightTable: 'APONTAMENTO.CODEVENTO'},{LeftTable: 'OPERADORES.CODSEQ', RightTable: 'APONTAMENTO.CODEVENTO'}...]
// metadata.Filters => [{Name: 'filtroOntem', Type: FilterType.Date, ...},{Name: 'filtroHoje', Type: FilterType.Date, ...} ]
