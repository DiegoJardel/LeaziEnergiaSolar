# Leazi Energia Solar - Comandos rápidos

## 1. Instalar packages na Infrastructure

```powershell
Install-Package Microsoft.EntityFrameworkCore -Version 8.0.30 -ProjectName LeaziEnergiaSolar.Infrastructure
Install-Package Microsoft.EntityFrameworkCore.Sqlite -Version 8.0.30 -ProjectName LeaziEnergiaSolar.Infrastructure
Install-Package Microsoft.EntityFrameworkCore.Design -Version 8.0.30 -ProjectName LeaziEnergiaSolar.Infrastructure
Install-Package Microsoft.EntityFrameworkCore.Tools -Version 8.0.30 -ProjectName LeaziEnergiaSolar.Infrastructure
```

## 2. Instalar packages no WPF

```powershell
Install-Package Microsoft.EntityFrameworkCore -Version 8.0.30 -ProjectName LeaziEnergiaSolar.Wpf
Install-Package Microsoft.EntityFrameworkCore.Relational -Version 8.0.30 -ProjectName LeaziEnergiaSolar.Wpf
Install-Package Microsoft.EntityFrameworkCore.Design -Version 8.0.30 -ProjectName LeaziEnergiaSolar.Wpf
Install-Package CommunityToolkit.Mvvm -Version 8.3.2 -ProjectName LeaziEnergiaSolar.Wpf
Install-Package MaterialDesignThemes -Version 5.1.0 -ProjectName LeaziEnergiaSolar.Wpf
Install-Package Microsoft.Extensions.DependencyInjection -Version 8.0.1 -ProjectName LeaziEnergiaSolar.Wpf
Install-Package Microsoft.Extensions.DependencyInjection.Abstractions -Version 8.0.2 -ProjectName LeaziEnergiaSolar.Wpf
```

## 3. Restaurar e compilar

```powershell
dotnet restore --force-evaluate
dotnet build
Get-Help about_EntityFrameworkCore
```

## 4. Criar migration e banco

```powershell
Add-Migration InitialCreate -Project LeaziEnergiaSolar.Infrastructure -StartupProject LeaziEnergiaSolar.Wpf
Update-Database -Project LeaziEnergiaSolar.Infrastructure -StartupProject LeaziEnergiaSolar.Wpf
```

## 5. Próximas alterações no banco

```powershell
dotnet build
Add-Migration NomeDaAlteracao -Project LeaziEnergiaSolar.Infrastructure -StartupProject LeaziEnergiaSolar.Wpf
Update-Database -Project LeaziEnergiaSolar.Infrastructure -StartupProject LeaziEnergiaSolar.Wpf
```

## 6. Subir para o Git

```bash
git status
git add .
git commit -m "SQCWB-148032: atualiza estrutura do banco e migrations"
git push
```

## Atenção

- Execute uma linha por vez.
- Todos os pacotes `Microsoft.EntityFrameworkCore.*` devem usar a versão `8.0.30`.
- Não instale `Microsoft.EntityFrameworkCore.SqlServer`, pois o projeto usa SQLite.
- Não envie arquivos do banco SQLite, `bin` ou `obj`.
- Envie a pasta `Migrations`.
