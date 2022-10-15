using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.PlayerLoop;
using UnityEngine.ResourceManagement.AsyncOperations;

public delegate void GameplayDataLoaded(GameplayConfiguration gameplayConfig);

public static class DataManager
{
    public static GameplayDataLoaded OnGameplayDataLoaded;
    private static AsyncOperationHandle<GameplayConfiguration> _loadGameplayConfOp;

    public static void Init(AssetReference assetReference)
    {
        _loadGameplayConfOp = Addressables.LoadAssetAsync<GameplayConfiguration>(assetReference);
        _loadGameplayConfOp.Completed += LoadGameplayConfOpOnCompleted;
    }

    private static void LoadGameplayConfOpOnCompleted(AsyncOperationHandle<GameplayConfiguration> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
            OnGameplayDataLoaded?.Invoke(obj.Result);
    }

    public static void Dispose()
    {
       Addressables.Release(_loadGameplayConfOp); 
    }
}
