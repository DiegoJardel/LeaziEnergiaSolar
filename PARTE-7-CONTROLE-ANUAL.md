# Parte 7: Controle Anual

## Incluído

- `ControleAnualDto` com totais anuais.
- `ResumoAnualMesDto` com consolidação mensal.
- `FiltroControleAnualDto`.
- `IControleAnualService` e `ControleAnualService`.
- Reutilização do `ILancamentoService` para consulta anual.
- Filtros por ano e vendedor.
- Total anual vendido e total anual de comissões.
- Quantidade anual de registros, pagos e pendentes.
- Resumo completo dos doze meses, inclusive meses sem movimento.
- Gráfico de vendas mensais.
- Gráfico de comissões mensais.
- Tabela mensal com vendas, comissões, registros, pagos e pendentes.
- Todos os vendedores no filtro, inclusive inativos, para preservar consultas históricas.
- Tela Material Design e navegação real pelo menu Controle Anual.
- Operações assíncronas, indicador de carregamento e tratamento de erro.

## Regra dos gráficos

As barras usam escala proporcional ao maior mês do ano filtrado. Meses sem movimento permanecem visíveis com altura mínima. Os valores completos aparecem no tooltip da barra.

## Próxima etapa

Parte 8: CRUD de Usuários, perfis Administrador e Operador, redefinição de senha e autorização do módulo.
