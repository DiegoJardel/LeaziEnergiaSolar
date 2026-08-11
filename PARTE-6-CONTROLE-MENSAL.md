# Parte 6: Controle Mensal

## Incluído

- `ControleMensalDto` com totais e lançamentos.
- `FiltroControleMensalDto`.
- `IControleMensalService` e `ControleMensalService`.
- Reutilização do `ILancamentoService`, evitando duplicação de consulta e regra.
- Filtro obrigatório por mês e ano.
- Filtros opcionais por vendedor, status e texto.
- Cards de total vendido, comissão, registros, pagos e pendentes.
- Listagem detalhada com data, cliente, vendedor, venda, percentual, comissão, status e observação.
- Seleção inicial no mês e ano atuais.
- Botão Limpar que retorna ao período atual e remove filtros opcionais.
- Todos os vendedores disponíveis no filtro, inclusive inativos, preservando consultas históricas.
- Tela Material Design e navegação real pelo menu Controle Mensal.
- Operações assíncronas, indicador de carregamento e mensagem de erro.

## Regra dos cards

Os cards refletem exatamente os registros exibidos após todos os filtros. Se o usuário filtrar apenas os pendentes de um vendedor, os totais serão recalculados com esse mesmo subconjunto.

## Próxima etapa

Parte 7: Controle Anual com resumo dos doze meses, totais anuais e filtro por vendedor.
