# Parte 3: CRUD de Vendedores

## Incluído

- DTOs de leitura e gravação de vendedores.
- `IVendedorService` e `VendedorService`.
- `IVendedorRepository` e `VendedorRepository`.
- Cadastro e edição no SQLite.
- Ativação e inativação, preservando histórico.
- Pesquisa por nome, CPF/CNPJ ou e-mail.
- Validação real dos dígitos de CPF e CNPJ.
- Bloqueio de CPF/CNPJ duplicado.
- Validação de nome, telefone, e-mail e percentual.
- Normalização de documento, telefone e e-mail antes da gravação.
- Máscaras de CPF/CNPJ e telefone durante o preenchimento.
- ViewModel MVVM com comandos assíncronos.
- Tela WPF Material Design com formulário e DataGrid.
- Feedback de sucesso e erro dentro da tela.
- Indicador de carregamento.
- Navegação do menu Vendedores para a tela real.
- Índice único para CPF/CNPJ no modelo do Entity Framework Core.

## Decisão de exclusão

O sistema não apaga fisicamente vendedores nesta etapa. O usuário ativa ou inativa o cadastro. Isso evita perda de referência quando os lançamentos forem implementados.

## Próxima etapa

Parte 4: CRUD de Lançamentos, cálculo automático da comissão, status Pago/Pendente e vínculo com vendedor.
