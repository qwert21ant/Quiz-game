using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

public class JsonPersistenceService<T> {
  private string filePath;
  private T? defaultValue;
  private T? value;

  protected T Value => value!;

  protected JsonPersistenceService(string filePath, T? defaultValue) {
    this.filePath = filePath;
    this.defaultValue = defaultValue;

    Load().ConfigureAwait(false).GetAwaiter().GetResult();
  }

  protected async Task Load() {
    if (!File.Exists(filePath)) {
      value = defaultValue;
      return;
    }
    
    var json = await File.ReadAllTextAsync(filePath);
    value = JsonSerializer.Deserialize<T>(json);
  }

  protected async Task Save() {
    var json = JsonSerializer.Serialize(value);
    await File.WriteAllTextAsync(filePath, json);
  }

  protected async Task Mutate(Action<T> mutator) {
    mutator(value!);
    await Save();
  }
}