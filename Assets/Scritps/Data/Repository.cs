
// A generic repository class that handles loading and saving data using a file path.
using System.Collections.Generic;

public class Repository<T>
{
    protected readonly string path;

    public Repository(string path) => this.path = path;

    // Loads data of type T from the file specified by the path.
    protected virtual T Get() => JsonSaveService.LoadData<T>(path);

    // Saves the provided data of type T to the file specified by the path.
    protected virtual void Save(T data) => JsonSaveService.SaveData(path, data);
}
