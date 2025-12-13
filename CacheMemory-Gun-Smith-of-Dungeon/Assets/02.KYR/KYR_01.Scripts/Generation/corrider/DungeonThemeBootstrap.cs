using UnityEngine;

public class DungeonThemeBootstrap : MonoBehaviour
{
    [SerializeField] private DungeonGen dungeonGen;
    [SerializeField] private DungeonThemeListSO themeList;

    private void Start()
    {
        var themes = themeList.themes;
        if (themes == null || themes.Length == 0)
        {
            Debug.LogError("[DungeonThemeBootstrap] themeList 비어있음");
            return;
        }

        var theme = themes[Random.Range(0, themes.Length)];

        dungeonGen.ApplyTheme(theme);
        dungeonGen.GeneratePublic(); // 또는 dungeonGen.Generate();
    }
}