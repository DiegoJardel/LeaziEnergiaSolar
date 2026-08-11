# Parte 5: Dashboard conectado ao SQLite

## Incluído

- `DashboardDto` com indicadores consolidados.
- `ResumoMensalDto` para vendas e comissões por mês.
- `IDashboardService` e `DashboardService`.
- `IDashboardRepository` e `DashboardRepository`.
- Totais reais de vendas e comissões.
- Contagem real de registros, pagos e pendentes.
- Filtro por mês e ano.
- Gráficos simples de barras para vendas e comissões mensais, sem nova dependência externa.
- Últimos oito lançamentos do período selecionado.
- Atualização manual pelo botão Atualizar.
- Dashboard aberto automaticamente depois do login.
- Navegação do menu Dashboard conectada à tela real.
- Consultas assíncronas e somente leitura com `AsNoTracking`.
- Estado de carregamento e mensagem de erro.

## Regra do período

- Quando estiver selecionado `Todos os meses`, os cards e últimos lançamentos apresentam o ano completo.
- Quando um mês estiver selecionado, os cards e últimos lançamentos apresentam somente aquele mês.
- Os gráficos sempre mostram os doze meses do ano escolhido para permitir comparação anual.

## Próxima etapa

Parte 6: Controle Mensal com filtros, totais consolidados e listagem detalhada.
