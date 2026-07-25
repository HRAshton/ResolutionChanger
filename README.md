# Resolution Changer

## Local validation

Restore the repository-local formatter, then run the same checks used for pull requests:

```powershell
dotnet tool restore
dotnet csharpier check ResolutionChanger
dotnet format ResolutionChanger.slnx --verify-no-changes --no-restore
dotnet build ResolutionChanger.slnx --configuration Release --no-restore
```

Apply formatting with:

```powershell
dotnet csharpier format ResolutionChanger
```
