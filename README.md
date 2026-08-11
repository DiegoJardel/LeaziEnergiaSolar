# Leazi Energia Solar

Fundação do sistema desktop em WPF, .NET 8, MVVM em camadas, Material Design, EF Core e SQLite.

## Etapa 1 incluída
- Solução com Domain, Application, Infrastructure e Wpf.
- Entidades Usuario, Vendedor e Lancamento.
- Enums de perfil e status.
- Interfaces de repositório.
- DbContext SQLite e criação automática do banco.
- Usuário inicial `admin` / `admin` somente para desenvolvimento.
- Logo fornecido incluído em Assets/Images.
- Tema Leazi verde, amarelo, azul e branco.
- Tela de login funcional.
- Shell principal com menu lateral.
- Dashboard visual inicial.
- Pasta Utils com filtros de entrada, máscara e validação CPF/CNPJ.

## Parte 2 concluída
- Hash seguro BCrypt e autenticação por serviço.
- Login consultando o SQLite.
- Sessão, perfil, logout e restrição visual de usuários.

## Parte 3 concluída
- CRUD de vendedores com cadastro, edição, pesquisa e status.
- Validações e máscaras de CPF/CNPJ e telefone.
- Persistência no SQLite e bloqueio de documento duplicado.

## Parte 4 concluída
- CRUD de lançamentos com vínculo ao vendedor.
- Cálculo automático de comissão e status Pago/Pendente.
- Pesquisa, filtros, edição e exclusão.

## Parte 5 concluída
- Dashboard conectado ao SQLite.
- Indicadores, gráficos mensais e últimos lançamentos.
- Filtro por mês e ano.

## Parte 6 concluída
- Controle Mensal conectado ao SQLite.
- Filtros por mês, ano, vendedor, status e texto.
- Cards e listagem detalhada consolidados.

## Parte 7 concluída
- Controle Anual conectado ao SQLite.
- Resumo dos doze meses, totais, gráficos e filtro por vendedor.
- Consolidação de registros, pagos e pendentes.

## Parte 8 concluída
- CRUD de usuários com Administrador e Operador.
- Ativação, inativação e redefinição segura de senha.
- Autorização administrativa no serviço e na navegação.

## Parte 9 concluída
- Tratamento global de erros e logs.
- Backup automático e manual do SQLite.
- Confirmação antes da exclusão de lançamento.
- Perfil e script de publicação Windows x64.
- Sem testes automatizados e sem troca obrigatória de senha, conforme solicitado.

## Ainda não implementado
- Exportação para Excel.

## Como abrir
1. Windows 10 ou 11.
2. Visual Studio 2022 com workload Desenvolvimento para desktop com .NET.
3. Abrir `LeaziEnergiaSolar.sln`.
4. Restaurar os pacotes NuGet.
5. Definir `LeaziEnergiaSolar.Wpf` como projeto de inicialização.
6. Executar.

> Acesso inicial: `admin` / `admin`. A senha é armazenada como hash BCrypt. Redefina manualmente no módulo Usuários antes do uso real.
