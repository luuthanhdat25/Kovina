using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanguageRepository : Repository<LocalizationData>
{
    private LocalizationData LocalizationData;
    public LanguageRepository(string path) : base(path)
    {
        LocalizationData = Get();
        if (LocalizationData == null)
        {
            LocalizationData = new LocalizationData();

        }
    }
}
