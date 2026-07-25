using System.Text.Json;
using ResolutionChanger.Constants;
using ResolutionChanger.Models;

namespace ResolutionChanger.Configuration;

internal sealed class BindingStore
{
    private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };

    private readonly string _path = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "ResolutionChanger",
        "bindings.json"
    );

    private List<ResolutionBinding> _bindings;

    public BindingStore()
    {
        _bindings = Load();
    }

    public IReadOnlyList<ResolutionBinding> Bindings => _bindings;

    public void Upsert(ResolutionBinding binding)
    {
        if (binding.HotkeyText != BindingDefaults.UnassignedHotkeyText)
        {
            _bindings =
            [
                .. _bindings.Select(x =>
                    x.Id != binding.Id
                    && string.Equals(x.HotkeyText, binding.HotkeyText, StringComparison.OrdinalIgnoreCase)
                        ? x with
                        {
                            HotkeyText = BindingDefaults.UnassignedHotkeyText,
                        }
                        : x
                ),
            ];
        }

        _bindings.RemoveAll(x => x.Id == binding.Id);
        _bindings.Add(binding);
        Save();
    }

    public void Remove(Guid id)
    {
        _bindings.RemoveAll(x => x.Id == id);
        Save();
    }

    private List<ResolutionBinding> Load()
    {
        try
        {
            return File.Exists(_path)
                ? JsonSerializer.Deserialize<List<ResolutionBinding>>(File.ReadAllText(_path)) ?? []
                : [];
        }
        catch (JsonException)
        {
            return [];
        }
    }

    private void Save()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        File.WriteAllText(_path, JsonSerializer.Serialize(_bindings, SerializerOptions));
    }
}
