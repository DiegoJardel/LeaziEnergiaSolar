# Parte 8: CRUD de Usuários

## Incluído

- `UsuarioDto`, `SalvarUsuarioDto` e `RedefinirSenhaDto`.
- `IUsuarioService` e `UsuarioService`.
- Ampliação do `IUsuarioRepository` e `UsuarioRepository`.
- Cadastro e edição de usuários.
- Pesquisa por nome ou login.
- Perfis Administrador e Operador.
- Ativação e inativação.
- Redefinição segura de senha com BCrypt.
- Senha opcional na edição, preservando o hash existente quando vazia.
- Validação de senha com mínimo de 8 caracteres, letra maiúscula, letra minúscula e número.
- Bloqueio de login duplicado.
- Normalização do login em letras minúsculas e sem espaços.
- Proteção administrativa no serviço e na navegação.
- Bloqueio para o usuário logado inativar a própria conta.
- Bloqueio para o usuário logado remover o próprio perfil administrativo.
- Garantia de pelo menos um administrador ativo.
- Tela WPF Material Design com formulário e DataGrid.
- Sessão usada para autorizar todas as operações administrativas.

## Exclusão

Usuários não são excluídos fisicamente. O cadastro é ativado ou inativado, preservando rastreabilidade e evitando perda de referência futura.

## Próxima etapa

Parte 9: qualidade, confirmação de exclusão, tratamento global de erros, testes automatizados, backup do SQLite e publicação.
