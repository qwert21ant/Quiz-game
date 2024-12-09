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

    Load();
  }

  protected void Load() {
    if (!File.Exists(filePath)) {
      value = defaultValue;
      return;
    }
    
    var json = File.ReadAllText(filePath);
    value = JsonSerializer.Deserialize<T>(json);
  }

  protected void Save() {
    var json = JsonSerializer.Serialize(value);
    File.WriteAllText(filePath, json);
  }

  protected void Mutate(Action<T> mutator) {
    mutator(value!);
    Save();
  }
}