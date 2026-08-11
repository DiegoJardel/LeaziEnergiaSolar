# Parte 9: Qualidade e publicação

Esta etapa foi mantida simples, conforme solicitado. Não foram adicionados testes automatizados nem alteração obrigatória da senha inicial.

## Incluído

- Banco SQLite salvo em `%LocalAppData%\LeaziEnergiaSolar\leazi.db`.
- Migração simples do banco antigo existente ao lado do executável.
- Pastas padronizadas para dados, backups e logs.
- Tratamento global de erros da interface, aplicação e tarefas assíncronas.
- Log mensal em arquivo texto.
- Backup automático diário ao abrir o sistema.
- Retenção dos 10 backups automáticos mais recentes.
- Botão `Criar backup` no menu principal.
- Confirmação antes de excluir um lançamento.
- Perfil de publicação Windows x64 autocontido.
- Script PowerShell simples para publicação.
- Código identado e sem nova biblioteca externa.

## Pastas usadas no Windows

```text
%LocalAppData%\LeaziEnergiaSolar
├── leazi.db
├── Backups
│   ├── leazi-auto-AAAAMMDD.db
│   └── leazi-backup-AAAAMMDD-HHMMSS.db
└── Logs
    └── leazi-AAAA-MM.log
```

## Publicação pelo Visual Studio

1. Abra `LeaziEnergiaSolar.sln`.
2. Clique com o botão direito em `LeaziEnergiaSolar.Wpf`.
3. Selecione `Publicar`.
4. Escolha o perfil `Windows-x64`.
5. Execute a publicação.

## Publicação pelo PowerShell

Na raiz da solução, execute:

```powershell
.\publicar-windows-x64.ps1
```

O resultado será criado em:

```text
publicacao\win-x64
```

A publicação é autocontida. O computador de destino não precisa ter o .NET instalado separadamente.

## Instalação simples

1. Copie a pasta `publicacao\win-x64` para o computador de destino.
2. Execute `LeaziEnergiaSolar.Wpf.exe`.
3. O banco e as pastas operacionais serão criados automaticamente no perfil do usuário do Windows.

## Acesso inicial

```text
Login: admin
Senha: admin
```

Não foi implementada troca obrigatória da senha inicial, conforme solicitado. Recomenda-se redefinir a senha manualmente no módulo Usuários antes de uso real.
