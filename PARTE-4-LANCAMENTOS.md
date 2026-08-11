# Parte 4: CRUD de Lançamentos

## Incluído

- DTOs de leitura, gravação e filtros.
- `ILancamentoService` e `LancamentoService`.
- `ILancamentoRepository` e `LancamentoRepository`.
- Cadastro, edição e exclusão física de lançamentos.
- Alteração rápida entre Pago e Pendente.
- Vínculo obrigatório com vendedor.
- Lista de vendedores ativos para novos lançamentos.
- Percentual preenchido a partir do vendedor selecionado.
- Cálculo automático de `ValorVenda × Percentual / 100`.
- Arredondamento monetário para duas casas decimais.
- Validações de data, cliente, documento opcional, vendedor, valor, percentual, status e observação.
- Pesquisa por cliente, vendedor ou CPF/CNPJ.
- Filtros por data inicial, data final, vendedor e status.
- Tela WPF Material Design com formulário, filtros e DataGrid.
- Comandos assíncronos no ViewModel.
- Feedback de sucesso e erro.
- Navegação real pelo menu Lançamentos.

## Regra de comissão

O percentual do vendedor é sugerido ao selecionar o vendedor, mas fica gravado no lançamento. Assim, alterações futuras no cadastro do vendedor não modificam comissões históricas.

## Exclusão

A exclusão é física, pois o lançamento é o próprio registro transacional. A confirmação visual poderá ser adicionada na etapa de acabamento para reduzir exclusões acidentais.

## Próxima etapa

Parte 5: Dashboard conectado ao SQLite, com indicadores, últimos lançamentos e consolidação mensal.
