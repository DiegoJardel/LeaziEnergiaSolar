# Parte 2: autenticação segura

## Incluído

- `BCrypt.Net-Next` para hash e verificação de senha.
- `IAutenticacaoService` e `AutenticacaoService`.
- `UsuarioRepository` consultando o SQLite.
- `LoginViewModel` com estado de carregamento e mensagem de erro.
- `UsuarioSessaoService` para a sessão do usuário autenticado.
- Login pelo banco, sem comparação direta na tela.
- Migração automática do usuário `admin` antigo em texto simples para hash BCrypt.
- Exibição do nome e perfil do usuário na tela principal.
- Restrição visual do módulo Usuários para o perfil Administrador.
- Logout com encerramento da sessão e retorno ao login.
- Entrada pela tecla Enter no campo de senha.
- Código reorganizado e identado.

## Acesso inicial de desenvolvimento

- Login: `admin`
- Senha: `admin`

A senha é gravada no banco somente como hash BCrypt. Antes da publicação, o sistema deverá obrigar a alteração da senha inicial.

## Próxima etapa

Parte 3: CRUD de vendedores com CPF/CNPJ, telefone, e-mail, percentual de comissão, status, pesquisa e validações.
